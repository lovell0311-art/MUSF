from __future__ import annotations

import json
import os
import subprocess
import time
from pathlib import Path
from typing import Any
from urllib.parse import urlparse

from .common import (
    adb,
    add_gate,
    assert_path_under_roots,
    assert_primary_client_project,
    default_device,
    env_profile,
    finalize_report,
    http_check,
    load_process_manifest,
    musf_root,
    process_manifest_path,
    read_json,
    release_report_template,
    resolve_tool,
    run,
    save_process_manifest,
    tcp_check,
    udp_listener_ports,
    write_json,
    write_text,
)
from .map_regression import extract_latest_scene_entry
from .publish import (
    copy_named_files,
    default_live_target_dir,
    formal_guard_manifest_path,
    frozen_ui_baseline_drift,
    sync_hotfix_to_devices,
)


def server_specs(profile_name: str) -> list[dict[str, str | list[str]]]:
    root = musf_root()
    bin_dir = root / "Server" / "Bin"
    file_server_dll = root / "Server" / "FileServer" / "FileServer.dll"
    node_exe = resolve_tool("node18")
    nginx_exe = resolve_tool("nginx")
    return [
        {"name": "realm", "command": [str(bin_dir / "App.exe"), "--AppId=2", "--AppType=Realm", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "gate-20004", "command": [str(bin_dir / "App.exe"), "--AppId=20004", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "gate-20005", "command": [str(bin_dir / "App.exe"), "--AppId=20005", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "gate-20006", "command": [str(bin_dir / "App.exe"), "--AppId=20006", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "gate-20007", "command": [str(bin_dir / "App.exe"), "--AppId=20007", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "gate-20008", "command": [str(bin_dir / "App.exe"), "--AppId=20008", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "gm-app", "command": [str(bin_dir / "App.exe"), "--AppId=99", "--AppType=GM", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "db-10", "command": [str(bin_dir / "App.exe"), "--AppId=10", "--AppType=DB", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "match-11", "command": [str(bin_dir / "App.exe"), "--AppId=11", "--AppType=Match", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "login-center-25", "command": [str(bin_dir / "App.exe"), "--AppId=25", "--AppType=LoginCenter", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "db-21", "command": [str(bin_dir / "App.exe"), "--AppId=21", "--AppType=DB", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "db-278", "command": [str(bin_dir / "App.exe"), "--AppId=278", "--AppType=DB", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "db-279", "command": [str(bin_dir / "App.exe"), "--AppId=279", "--AppType=DB", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "mgmt-280", "command": [str(bin_dir / "App.exe"), "--AppId=280", "--AppType=MGMT", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "game-257", "command": [str(bin_dir / "App.exe"), "--AppId=257", "--AppType=Game", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "game-259", "command": [str(bin_dir / "App.exe"), "--AppId=259", "--AppType=Game", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "game-262", "command": [str(bin_dir / "App.exe"), "--AppId=262", "--AppType=Game", "--ConfigPath=../Config", "--LogLevel=Info"], "cwd": str(bin_dir)},
        {"name": "fileserver", "command": [str(resolve_tool("dotnet-runtime")), str(file_server_dll)], "cwd": str(file_server_dll.parent)},
        {"name": "webgm-node", "command": [str(node_exe), "server.js"], "cwd": str(root / "Tools" / "webgm")},
        {"name": "nginx", "command": [str(nginx_exe)], "cwd": str(nginx_exe.parent)},
    ]


def discover_managed_processes() -> list[dict[str, str | int]]:
    query = (
        "Get-CimInstance Win32_Process | "
        "Where-Object { "
        "$_.Name -in @('App.exe','node.exe','nginx.exe','dotnet.exe') "
        "} | Select-Object Name,ProcessId,CommandLine | ConvertTo-Json -Depth 3"
    )
    result = run(["powershell.exe", "-NoProfile", "-Command", query], check=False)
    raw = (result.stdout or "").strip()
    if not raw:
        return []

    data = json.loads(raw)
    if isinstance(data, dict):
        data = [data]

    managed: list[dict[str, str | int]] = []
    for entry in data:
        command_line = entry.get("CommandLine") or ""
        name = entry.get("Name") or ""
        pid = int(entry["ProcessId"])

        if name == "App.exe" and "F:\\MUSF\\Server\\Bin\\App.exe" in command_line:
            managed.append({"name": "app", "pid": pid, "commandLine": command_line})
            continue

        if name == "node.exe" and (
            "local_static_server.js" in command_line
            or "F:\\MUSF\\Tools\\webgm\\server.js" in command_line
            or command_line.endswith(" server.js")
            or '"server.js"' in command_line
        ):
            managed.append({"name": "node", "pid": pid, "commandLine": command_line})
            continue

        if name == "nginx.exe" and "F:\\MUSF\\Tools\\nginx\\nginx-1.25.0\\nginx.exe" in command_line:
            managed.append({"name": "nginx", "pid": pid, "commandLine": command_line})
            continue

        if name == "dotnet.exe" and "F:\\MUSF\\Server\\FileServer\\FileServer.dll" in command_line:
            managed.append({"name": "fileserver", "pid": pid, "commandLine": command_line})

    return managed


def stop_process_pid(pid: int) -> str:
    result = run(["taskkill", "/PID", str(pid), "/T", "/F"], check=False)
    if result.returncode == 0:
        return "terminated"
    return (result.stdout or result.stderr or "").strip() or f"failed:{result.returncode}"


def tcp_check_url(url: str) -> dict[str, Any]:
    parsed = urlparse(url)
    host = parsed.hostname or ""
    port = parsed.port or (443 if parsed.scheme == "https" else 80)
    ok = bool(host) and tcp_check(host, port)
    return {
        "ok": ok,
        "url": url,
        "host": host,
        "port": port,
    }


def server_health(profile_name: str) -> Path:
    root = musf_root()
    profile = env_profile(profile_name)
    report = release_report_template("manual", profile_name, "server-health")

    mongo_ok = tcp_check(profile["mongo"]["host"], int(profile["mongo"]["port"]))
    add_gate(report, "mongo", "pass" if mongo_ok else "fail", f'{profile["mongo"]["host"]}:{profile["mongo"]["port"]}')

    udp_ports = udp_listener_ports([profile["ports"]["realm"], *profile["ports"]["gates"]])
    realm_port = int(profile["ports"]["realm"])
    realm_ok = realm_port in udp_ports
    add_gate(report, "realm", "pass" if realm_ok else "fail", f'udp:{realm_port}')

    gate_failures = [str(port) for port in profile["ports"]["gates"] if int(port) not in udp_ports]
    add_gate(report, "gates", "pass" if not gate_failures else "fail", "missing udp: " + ",".join(gate_failures) if gate_failures else "all udp listeners present")

    fileserver = http_check(profile["client"]["streamingAssetsVersionUrl"])
    add_gate(report, "fileserver", "pass" if fileserver["ok"] else "fail", str(fileserver))

    gm_web = http_check(profile["gm"]["webUrl"])
    add_gate(report, "gm-web", "pass" if gm_web["ok"] else "fail", str(gm_web))

    gm_node = tcp_check_url(profile["gm"]["nodeUrl"])
    add_gate(report, "gm-node", "pass" if gm_node["ok"] else "fail", str(gm_node))

    gm_api = tcp_check_url(profile["gm"]["apiUrl"])
    add_gate(report, "gm-api", "pass" if gm_api["ok"] else "fail", str(gm_api))

    output = root / "reports" / "releases" / "server-health.json"
    write_json(output, finalize_report(report))
    return output


def server_start(profile_name: str, clean: bool = False) -> Path:
    root = musf_root()
    processes: list[dict[str, str | int]] = []

    from .phase0 import sync_network_config

    sync_network_config(profile_name)

    if clean:
        for entry in discover_managed_processes():
            stop_process_pid(int(entry["pid"]))
        time.sleep(2)

    mongo_bat = root / "MongoDB" / "启动MongoDB.bat"
    profile = env_profile(profile_name)
    mongo_running = tcp_check(profile["mongo"]["host"], int(profile["mongo"]["port"]))
    if mongo_bat.exists() and not mongo_running:
        proc = subprocess.Popen(["cmd.exe", "/c", str(mongo_bat)], cwd=str(mongo_bat.parent))
        processes.append({"name": "mongo", "pid": proc.pid, "cwd": str(mongo_bat.parent), "command": str(mongo_bat)})
        time.sleep(2)

    for spec in server_specs(profile_name):
        proc = subprocess.Popen(spec["command"], cwd=spec["cwd"])
        processes.append({"name": spec["name"], "pid": proc.pid, "cwd": spec["cwd"], "command": " ".join(spec["command"])})
        time.sleep(1)

    return save_process_manifest(processes)


def server_stop(profile_name: str, all_managed: bool = False) -> Path:
    entries = []
    target_entries = load_process_manifest()
    if all_managed:
        target_entries = discover_managed_processes()

    for entry in target_entries:
        pid = int(entry["pid"])
        status = stop_process_pid(pid)
        entries.append({"name": entry["name"], "pid": pid, "status": status})

    output = musf_root() / "reports" / "runtime" / "server-stop.json"
    write_json(output, {"generatedAt": time.strftime("%Y-%m-%dT%H:%M:%S%z"), "entries": entries})
    return output


def build_android(profile_name: str) -> Path:
    root = musf_root()
    unity = resolve_tool("unity-editor")
    project = root / "Client" / "Unity"
    assert_primary_client_project(project, "Android build project")
    log_path = root / "reports" / "releases" / "unity-build-android.log"
    output = root / "reports" / "releases" / "build-android.json"

    args = [
        str(unity),
        "-quit",
        "-batchmode",
        "-projectPath",
        str(project),
        "-executeMethod",
        "ETEditor.BatchBuildAndroid.PerformBuild",
        "-logFile",
        str(log_path),
    ]

    result = run(args, cwd=project, check=False)
    write_json(
        output,
        {
            "generatedAt": time.strftime("%Y-%m-%dT%H:%M:%S%z"),
            "command": args,
            "returncode": result.returncode,
            "stdout": result.stdout,
            "stderr": result.stderr,
            "logPath": str(log_path),
        },
    )
    return output


def run_gate(profile_name: str) -> Path:
    root = musf_root()
    profile = env_profile(profile_name)
    report = release_report_template("manual", profile_name, "run-gate")
    print(f"[run-gate] profile={profile_name}", flush=True)

    from .phase0 import verify_toolchain
    from .android_regression import (
        backup_device_state,
        drive_to_main_hud,
        ensure_canary_account_state,
        ensure_reference_images,
        logcat_fatal_summary,
        open_bag,
        open_skills,
        start_logcat_capture,
        stop_logcat_capture,
        ui_family_policy,
        wait_for_state,
        write_android_regression_artifact,
    )

    tool_report = json.loads(verify_toolchain().read_text(encoding="utf-8"))
    missing_required = [item["name"] for item in tool_report["items"] if item["required"] and item["status"] == "missing"]
    add_gate(report, "toolchain", "pass" if not missing_required else "fail", ", ".join(missing_required) if missing_required else "all required tools resolved")
    print("[run-gate] toolchain checked", flush=True)

    health_report = json.loads(server_health(profile_name).read_text(encoding="utf-8"))
    add_gate(report, "server-health", health_report["overall"], "see server-health.json")
    print("[run-gate] server health checked", flush=True)

    device = default_device()
    add_gate(report, "adb-device", "pass" if device else "fail", device or "no device detected")

    package_name = profile["client"]["packageName"]
    package_ok = False
    if device:
        package_result = adb(["-s", device, "shell", "pm", "list", "packages", package_name], check=False)
        package_ok = package_name in (package_result.stdout or "")
        add_gate(report, "apk-installed", "pass" if package_ok else "fail", package_name)
    else:
        add_gate(report, "apk-installed", "fail", "device unavailable")

    baseline_path = root / "apk-ui-baseline-manifest.json"
    add_gate(report, "ui-baseline-manifest", "pass" if baseline_path.exists() else "fail", str(baseline_path))

    frozen_ui_gate = frozen_ui_baseline_drift(default_live_target_dir())
    frozen_ui_details = (
        "frozen UI bundles match 20260310.apk baseline"
        if frozen_ui_gate["passed"]
        else ", ".join(entry["name"] for entry in frozen_ui_gate.get("drift", [])[:5]) or "frozen UI baseline drift detected"
    )
    add_gate(
        report,
        "ui-baseline-drift",
        "pass" if frozen_ui_gate["passed"] else "fail",
        frozen_ui_details,
        artifacts=[str(root / "apk-ui-baseline-manifest.json")],
    )

    ui_name = "pojun"
    try:
        ui_name = str(ui_family_policy()["expectedUiName"])
        ensure_reference_images()
        add_gate(report, "entry-ui-baseline", "pass", f"{ui_name} login / role-select / HUD / bag references ready")
    except (FileNotFoundError, RuntimeError) as exc:
        add_gate(report, "entry-ui-baseline", "fail", str(exc))

    if not device or not package_ok:
        output = root / "reports" / "releases" / "run-gate.json"
        persist_run_gate_report(output, finalize_report(report))
        return output

    run_dir = root / "reports" / "releases" / "runs" / f"run_gate_{time.strftime('%Y%m%d-%H%M%S')}"
    run_dir.mkdir(parents=True, exist_ok=True)
    report["artifacts"].append(str(run_dir))

    backup = backup_device_state(device, package_name, run_dir)
    add_gate(
        report,
        "device-backup",
        "pass",
        backup["packagePath"] or package_name,
        artifacts=[backup["snapshotPath"], *backup["pulledFiles"]],
    )
    print(f"[run-gate] device backup ready: {device}", flush=True)

    local_account_state = ensure_canary_account_state()
    login_state = {
        "generatedAt": time.strftime("%Y-%m-%dT%H:%M:%S%z"),
        "account": "blaike",
        "passwordMasked": "******",
        "localAccountStatePath": str(local_account_state),
        "mode": "ui-input",
    }
    login_state_artifact = write_android_regression_artifact(run_dir, "prepared-login-state", login_state)
    add_gate(
        report,
        "login-state",
        "pass",
        "prepared blaike / 123456 for UI-input login",
        artifacts=[str(login_state_artifact)],
    )
    print("[run-gate] prepared login state", flush=True)

    log_path = run_dir / "logcat.txt"
    logcat_proc, logcat_handle = start_logcat_capture(device, log_path)
    hud_result: dict[str, Any] | None = None
    bag_result: dict[str, Any] | None = None
    skills_result: dict[str, Any] | None = None
    return_to_hud_result: dict[str, Any] | None = None

    try:
        print("[run-gate] driving to main HUD", flush=True)
        hud_result = drive_to_main_hud(device, package_name, run_dir)
        hud_artifact = write_android_regression_artifact(run_dir, "entry-result", hud_result)
        print(f"[run-gate] entry result => {hud_result['state']}", flush=True)
        if hud_result["state"] == "main-hud":
            add_gate(
                report,
                "login-smoke",
                "pass",
                f"entered {ui_name} main HUD",
                artifacts=[str(hud_artifact), hud_result["screenshotPath"]],
            )
            add_gate(
                report,
                "main-hud",
                "pass",
                "main HUD visible",
                artifacts=[hud_result["screenshotPath"]],
            )
        elif hud_result["state"] == "unity-init-error":
            add_gate(
                report,
                "login-smoke",
                "fail",
                f"Unity initialize error page shown instead of {ui_name} entry UI",
                artifacts=[str(hud_artifact), hud_result["screenshotPath"]],
            )
            add_gate(report, "main-hud", "fail", "blocked by Unity init error")
        else:
            add_gate(
                report,
                "login-smoke",
                "fail",
                f'unexpected entry state: {hud_result["state"]}',
                artifacts=[str(hud_artifact), hud_result.get("screenshotPath", "")],
            )
            add_gate(report, "main-hud", "fail", f'not reached: {hud_result["state"]}')

        if hud_result["state"] == "main-hud":
            hud_screenshot = Path(hud_result["screenshotPath"])
            print("[run-gate] opening bag", flush=True)
            bag_result = open_bag(device, run_dir, hud_screenshot)
            bag_artifact = write_android_regression_artifact(run_dir, "bag-result", bag_result)
            bag_shortcut_visible = bool(bag_result.get("shortcutStrip", {}).get("passed", False))
            bag_pass = bag_result["state"] == "bag-open"
            bag_status = "pass" if bag_pass else "warn" if bag_shortcut_visible else "fail"
            bag_details = (
                "bag panel visible"
                if bag_pass
                else "bag shortcut strip visible on HUD; panel automation unavailable in current HUD"
                if bag_shortcut_visible
                else f'bag open failed: {bag_result["state"]}'
            )
            add_gate(
                report,
                "bag-visible",
                bag_status,
                bag_details,
                artifacts=[str(bag_artifact), bag_result["screenshotPath"]],
            )

            if bag_result["state"] == "bag-open":
                from .android_regression import close_panel

                close_panel(device, Path(bag_result["screenshotPath"]))
                return_to_hud_result = wait_for_state(device, run_dir / "screens", {"main-hud"}, timeout_seconds=20, poll_seconds=3, prefix="return-hud")
                return_hud_artifact = write_android_regression_artifact(run_dir, "return-hud-result", return_to_hud_result)
                hud_anchor = Path(return_to_hud_result.get("screenshotPath") or hud_result["screenshotPath"])
                add_gate(
                    report,
                    "hud-restored",
                    "pass" if return_to_hud_result["state"] == "main-hud" else "fail",
                    "returned to main HUD after bag close" if return_to_hud_result["state"] == "main-hud" else f'return state: {return_to_hud_result["state"]}',
                    artifacts=[str(return_hud_artifact), return_to_hud_result.get("screenshotPath", "")],
                )
            else:
                hud_anchor = hud_screenshot

            print("[run-gate] opening skills", flush=True)
            skills_result = open_skills(device, run_dir, hud_anchor)
            skills_artifact = write_android_regression_artifact(run_dir, "skills-result", skills_result)
            skills_shortcut_visible = bool(skills_result.get("shortcutStrip", {}).get("passed", False))
            skills_pass = skills_result["state"] == "skills-panel"
            skills_status = "pass" if skills_pass else "warn" if skills_shortcut_visible else "fail"
            skills_details = (
                "skills panel visible"
                if skills_pass
                else "skills shortcut strip visible on HUD; panel automation unavailable in current HUD"
                if skills_shortcut_visible
                else "skills panel heuristic did not match"
            )
            add_gate(
                report,
                "skills-visible",
                skills_status,
                skills_details,
                artifacts=[str(skills_artifact), skills_result["screenshotPath"]],
            )
    finally:
        stop_logcat_capture(logcat_proc, logcat_handle)

    logcat_summary = logcat_fatal_summary(log_path)
    write_android_regression_artifact(run_dir, "logcat-summary", logcat_summary)
    print(f"[run-gate] logcat clean => {logcat_summary['ok']}", flush=True)
    add_gate(
        report,
        "logcat-clean",
        "pass" if logcat_summary["ok"] else "fail",
        "no fatal markers" if logcat_summary["ok"] else "; ".join(logcat_summary["matches"][:3]),
        artifacts=[str(log_path)],
    )

    latest_scene = extract_latest_scene_entry(log_path)
    add_gate(
        report,
        "map-log-evidence",
        "pass" if latest_scene else "fail",
        (
            f"{latest_scene['sceneName']} minimap={latest_scene['minimapName']} offset=({latest_scene['offset']['x']},{latest_scene['offset']['y']}) scale={latest_scene['scale']}"
            if latest_scene
            else "No SwitchMiniMap evidence found in run logcat."
        ),
        artifacts=[str(log_path), latest_scene["screenshotPath"]] if latest_scene and latest_scene.get("screenshotPath") else [str(log_path)],
    )

    output = root / "reports" / "releases" / "run-gate.json"
    persist_run_gate_report(output, finalize_report(report))
    print(f"[run-gate] finished => {finalize_report(report)['overall']}", flush=True)
    return output


def render_run_gate_summary(report: dict[str, Any]) -> str:
    failed = [gate for gate in report.get("gates", []) if gate["status"] == "fail"]
    warned = [gate for gate in report.get("gates", []) if gate["status"] == "warn"]
    lines = [
        "# Run Gate Summary",
        "",
        f"- Overall: {report.get('overall', 'pending')}",
        f"- Profile: {report.get('profile', '')}",
        f"- Phase: {report.get('phase', '')}",
    ]
    if report.get("artifacts"):
        lines.append(f"- Artifacts: {', '.join(str(item) for item in report['artifacts'])}")

    lines.extend(["", "## Failed Gates"])
    if failed:
        for gate in failed:
            details = gate.get("details", "")
            lines.append(f"- {gate['name']}: {details}")
    else:
        lines.append("- none")

    lines.extend(["", "## Warn Gates"])
    if warned:
        for gate in warned:
            details = gate.get("details", "")
            lines.append(f"- {gate['name']}: {details}")
    else:
        lines.append("- none")

    lines.extend(["", "## Gate Snapshot"])
    for gate in report.get("gates", []):
        details = gate.get("details", "")
        lines.append(f"- [{gate['status']}] {gate['name']}: {details}")

    return "\n".join(lines) + "\n"


def persist_run_gate_report(path: Path, report: dict[str, Any]) -> None:
    write_json(path, report)
    write_text(path.with_name(f"{path.stem}-summary.md"), render_run_gate_summary(report))


def rollback_source_roots() -> list[Path]:
    root = musf_root()
    return [
        root / "baseline",
        root / "backups",
        root / "archive",
        root / "reports" / "releases",
        root / "Server" / "Release",
        root / "Release_Test",
    ]


def default_rollback_source_dir() -> Path:
    manifest = read_json(formal_guard_manifest_path())
    configured = str(manifest.get("StableBaselineSourceDir", "")).strip()
    if configured:
        return Path(configured)
    return musf_root() / "baseline" / "login-stack" / "StreamingAssets"


def resolve_rollback_source(source: str) -> tuple[Path, dict[str, Any]]:
    root = musf_root()
    candidate = Path(source) if source else default_rollback_source_dir()
    source_kind = "directory"

    if candidate.is_file() and candidate.suffix.lower() == ".json":
        payload = read_json(candidate)
        snapshot_dir_value = str(payload.get("snapshotDir", "")).strip()
        if snapshot_dir_value:
            candidate = Path(snapshot_dir_value) / "files"
            source_kind = "report-snapshot"
        elif candidate.name.lower() == "snapshot.json":
            candidate = candidate.parent / "files"
            source_kind = "snapshot-manifest"

    elif candidate.is_dir() and (candidate / "snapshot.json").exists() and (candidate / "files").exists():
        candidate = candidate / "files"
        source_kind = "snapshot-dir"

    resolved = assert_path_under_roots(candidate, rollback_source_roots(), "Rollback source")
    if not resolved.exists():
        raise FileNotFoundError(f"Rollback source not found: {resolved}")

    return resolved, {
        "sourceKind": source_kind,
        "requestedSource": source,
        "resolvedSource": str(resolved),
    }


def rollback_file_names(source_dir: Path) -> list[str]:
    return sorted(path.name for path in source_dir.iterdir() if path.is_file())


def rollback(profile_name: str, source: str, *, dry_run: bool = False, skip_device_sync: bool = False) -> Path:
    root = musf_root()
    manifest = read_json(formal_guard_manifest_path())
    source_dir, source_metadata = resolve_rollback_source(source)
    live_target_dir = Path(manifest.get("LiveTargetDir") or default_live_target_dir())
    mirror_dirs = [Path(path) for path in manifest.get("MirrorTargetDirs", [])]
    if not mirror_dirs:
        mirror_dirs = [root / "Release_Test" / "Android" / "StreamingAssets", root / "Server" / "Release" / "update" / "2.0TestGame" / "Android" / "StreamingAssets"]

    file_names = rollback_file_names(source_dir)
    if not file_names:
        raise RuntimeError(f"No rollback files found in source directory: {source_dir}")

    timestamp = time.strftime("%Y%m%d-%H%M%S")
    live_restore: list[dict[str, Any]] = []
    mirror_restores: list[dict[str, Any]] = []
    if dry_run:
        live_restore = [
            {
                "name": name,
                "targetPath": str(live_target_dir / name),
                "backupPath": str((live_target_dir / name).with_name(f"{name}.bak-{timestamp}")) if (live_target_dir / name).exists() else None,
            }
            for name in file_names
        ]
        for mirror_dir in mirror_dirs:
            mirror_restores.append(
                {
                    "targetDir": str(mirror_dir),
                    "copied": [
                        {
                            "name": name,
                            "targetPath": str(mirror_dir / name),
                            "backupPath": str((mirror_dir / name).with_name(f"{name}.bak-{timestamp}")) if (mirror_dir / name).exists() else None,
                        }
                        for name in file_names
                    ],
                }
            )
    else:
        live_restore = copy_named_files(source_dir, live_target_dir, file_names, timestamp=timestamp)
        for mirror_dir in mirror_dirs:
            copied = copy_named_files(source_dir, mirror_dir, file_names, timestamp=timestamp)
            mirror_restores.append({"targetDir": str(mirror_dir), "copied": copied})

    device_sync_report = ""
    device_sync_error = ""
    if not skip_device_sync:
        try:
            device_sync_report = str(
                sync_hotfix_to_devices(
                    profile_name,
                    source_dir=str(live_target_dir),
                    files_to_sync=file_names,
                    dry_run=dry_run,
                )
            )
        except Exception as exc:
            device_sync_error = str(exc)

    output = root / "reports" / "releases" / "rollback.json"
    write_json(
        output,
        {
            "generatedAt": time.strftime("%Y-%m-%dT%H:%M:%S%z"),
            "profile": profile_name,
            "source": source_metadata,
            "status": "dry-run" if dry_run and not device_sync_error else "partial" if device_sync_error else "success",
            "dryRun": dry_run,
            "skipDeviceSync": skip_device_sync,
            "liveTargetDir": str(live_target_dir),
            "fileCount": len(file_names),
            "files": file_names,
            "liveRestore": live_restore,
            "mirrorRestores": mirror_restores,
            "deviceSyncReport": device_sync_report,
            "notes": [
                "Rollback restored the selected hotfix files into the live and mirror StreamingAssets targets.",
                "Device hotfix rollback was attempted from the restored live target.",
                "APK reinstall is still manual; this command currently restores hotfix/runtime assets rather than reinstalling base.apk.",
            ],
            "errors": [device_sync_error] if device_sync_error else [],
        },
    )
    return output
