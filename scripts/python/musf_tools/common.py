from __future__ import annotations

import argparse
import hashlib
import json
import os
import re
import socket
import subprocess
import time
from pathlib import Path
from typing import Any, Iterable
from urllib.error import HTTPError, URLError
from urllib.request import urlopen


ROOT = Path(__file__).resolve().parents[3]
PRIMARY_CLIENT_SOURCE_ROOT = ROOT / "Client" / "Unity"
PRIMARY_CLIENT_STREAMING_ASSETS_ROOT = PRIMARY_CLIENT_SOURCE_ROOT / "Assets" / "StreamingAssets"
PUBLISH_SOURCE_ROOTS = (
    PRIMARY_CLIENT_STREAMING_ASSETS_ROOT,
    ROOT / "Client" / "UnityBatchShadow" / "Assets" / "StreamingAssets",
    ROOT / "Server" / "Release" / "Android" / "StreamingAssets",
    ROOT / "Release_Test" / "Android" / "StreamingAssets",
    ROOT / "Server" / "Release" / "update" / "2.0TestGame" / "Android" / "StreamingAssets",
)
RELEASE_TARGET_ROOTS = (
    ROOT / "Server" / "Release",
    ROOT / "Release_Test",
)


def musf_root() -> Path:
    return ROOT


def resolve_path(path: Path | str) -> Path:
    return Path(path).expanduser().resolve(strict=False)


def path_is_under(path: Path | str, root: Path | str) -> bool:
    try:
        resolve_path(path).relative_to(resolve_path(root))
        return True
    except ValueError:
        return False


def assert_path_under_roots(path: Path | str, roots: Iterable[Path | str], description: str) -> Path:
    resolved = resolve_path(path)
    for root in roots:
        if path_is_under(resolved, root):
            return resolved

    allowed_roots: list[str] = []
    for root in roots:
        candidate = str(resolve_path(root))
        if candidate not in allowed_roots:
            allowed_roots.append(candidate)
    allowed = ", ".join(allowed_roots)
    raise RuntimeError(f"{description} is outside the approved roots: {resolved}. Allowed roots: {allowed}")


def assert_primary_client_project(path: Path | str, description: str) -> Path:
    return assert_path_under_roots(path, [PRIMARY_CLIENT_SOURCE_ROOT], description)


def assert_primary_client_path(path: Path | str, description: str) -> Path:
    return assert_path_under_roots(path, [PRIMARY_CLIENT_SOURCE_ROOT], description)


def assert_publish_source_dir(path: Path | str, description: str) -> Path:
    return assert_path_under_roots(path, PUBLISH_SOURCE_ROOTS, description)


def assert_release_target_dir(path: Path | str, description: str) -> Path:
    return assert_path_under_roots(path, RELEASE_TARGET_ROOTS, description)


def read_json(path: Path) -> Any:
    return json.loads(path.read_text(encoding="utf-8-sig"))


def write_json(path: Path, data: Any) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(data, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def write_text(path: Path, content: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content, encoding="utf-8")


def toolchain_manifest() -> dict[str, Any]:
    return read_json(musf_root() / "toolchain-manifest.json")


def env_profile(name: str = "local-lan") -> dict[str, Any]:
    return read_json(musf_root() / "env-profiles" / f"{name}.json")


def resolve_tool(name: str) -> Path:
    manifest = toolchain_manifest()
    for item in manifest["items"]:
        if item["name"] == name:
            formal = Path(item["path"])
            if formal.exists():
                return formal
            current_path = item.get("currentPath") or ""
            if current_path and Path(current_path).exists():
                return Path(current_path)
            return formal
    raise KeyError(f"Unknown tool in manifest: {name}")


def toolchain_status_entries() -> list[dict[str, Any]]:
    manifest = toolchain_manifest()
    items: list[dict[str, Any]] = []
    for item in manifest["items"]:
        formal = Path(item["path"])
        current = Path(item["currentPath"]) if item.get("currentPath") else None
        if formal.exists():
            status = "installed"
            detected = formal
        elif current and current.exists():
            status = "mismatch"
            detected = current
        else:
            status = "missing"
            detected = formal

        items.append(
            {
                "name": item["name"],
                "expectedVersion": item["version"],
                "formalPath": item["path"],
                "detectedPath": str(detected),
                "required": bool(item["required"]),
                "status": status,
                "detectedVersion": detect_version(item["name"], detected),
                "notes": item.get("notes", ""),
            }
        )
    return items


def now_stamp() -> str:
    return time.strftime("%Y%m%d-%H%M%S")


def timestamp_iso() -> str:
    return time.strftime("%Y-%m-%dT%H:%M:%S%z")


def file_md5(path: Path) -> str:
    md5 = hashlib.md5()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            md5.update(chunk)
    return md5.hexdigest()


def http_check(url: str, timeout: float = 5.0) -> dict[str, Any]:
    try:
        with urlopen(url, timeout=timeout) as response:
            return {"ok": True, "statusCode": int(response.status), "url": url}
    except HTTPError as exc:
        return {"ok": False, "statusCode": int(exc.code), "url": url, "error": str(exc)}
    except URLError as exc:
        return {"ok": False, "statusCode": None, "url": url, "error": str(exc)}
    except OSError as exc:
        return {"ok": False, "statusCode": None, "url": url, "error": str(exc)}
    except TimeoutError as exc:
        return {"ok": False, "statusCode": None, "url": url, "error": str(exc)}


def tcp_check(host: str, port: int, timeout: float = 1.5) -> bool:
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
        sock.settimeout(timeout)
        try:
            sock.connect((host, port))
            return True
        except OSError:
            return False


def udp_listener_ports(ports: Iterable[int]) -> set[int]:
    wanted = {int(port) for port in ports}
    if not wanted:
        return set()

    query = (
        "Get-NetUDPEndpoint | "
        "Where-Object { $_.LocalPort -in (" + ",".join(str(port) for port in sorted(wanted)) + ") } | "
        "Select-Object -ExpandProperty LocalPort | ConvertTo-Json -Depth 2"
    )
    result = run(["powershell.exe", "-NoProfile", "-Command", query], check=False)
    raw = (result.stdout or "").strip()
    if not raw:
        return set()

    data = json.loads(raw)
    if isinstance(data, int):
        return {data}
    return {int(value) for value in data}


def run(
    args: list[str],
    *,
    cwd: Path | None = None,
    check: bool = True,
    capture: bool = True,
    text: bool = True,
) -> subprocess.CompletedProcess:
    return subprocess.run(
        args,
        cwd=str(cwd) if cwd else None,
        check=check,
        capture_output=capture,
        text=text,
    )


def list_files(base: Path, patterns: Iterable[str]) -> list[Path]:
    regexes = [re.compile(pattern, re.IGNORECASE) for pattern in patterns]
    found: list[Path] = []
    for item in base.iterdir():
        if item.is_file() and any(regex.fullmatch(item.name) for regex in regexes):
            found.append(item)
    return sorted(found, key=lambda path: path.name.lower())


def release_report_template(version: str, profile: str, phase: str) -> dict[str, Any]:
    template = read_json(musf_root() / "templates" / "release-gate-report.json")
    template["version"] = version
    template["timestamp"] = timestamp_iso()
    template["profile"] = profile
    template["phase"] = phase
    template["overall"] = "pending"
    template["gates"] = []
    template["artifacts"] = []
    template["notes"] = []
    return template


def finalize_report(report: dict[str, Any]) -> dict[str, Any]:
    statuses = [gate["status"] for gate in report.get("gates", [])]
    if "fail" in statuses:
        report["overall"] = "fail"
    elif "warn" in statuses:
        report["overall"] = "warn"
    elif statuses:
        report["overall"] = "pass"
    else:
        report["overall"] = "pending"
    return report


def add_gate(report: dict[str, Any], name: str, status: str, details: str = "", artifacts: list[str] | None = None) -> None:
    report.setdefault("gates", []).append(
        {
            "name": name,
            "status": status,
            "details": details,
            "artifacts": artifacts or [],
        }
    )


def detect_version(tool_name: str, resolved: Path) -> str:
    try:
        if tool_name == "unity-editor" and resolved.exists():
            return str(resolved.stat().st_mtime_ns)
        if tool_name == "python311" and resolved.exists():
            result = run([str(resolved), "--version"], capture=True)
            return (result.stdout or result.stderr).strip()
        if tool_name == "adb" and resolved.exists():
            result = run([str(resolved), "version"], capture=True)
            return (result.stdout or result.stderr).splitlines()[0].strip()
        if tool_name == "node18" and resolved.exists():
            result = run([str(resolved), "-v"], capture=True)
            return (result.stdout or result.stderr).strip()
        if tool_name == "dotnet-runtime" and resolved.exists():
            result = run([str(resolved), "--list-runtimes"], capture=True)
            return (result.stdout or result.stderr).strip().splitlines()[0]
        if tool_name == "mongodb" and resolved.exists():
            result = run([str(resolved), "--version"], capture=True)
            return (result.stdout or result.stderr).splitlines()[0].strip()
        if tool_name == "nginx" and resolved.exists():
            result = run([str(resolved), "-v"], capture=True)
            return (result.stdout or result.stderr).strip()
    except Exception as exc:
        return str(exc)
    return ""


def iter_script_files(root: Path) -> Iterable[Path]:
    skip_dirs = {
        ".git",
        "Library",
        "Temp",
        "Logs",
        "node_modules",
        "obj",
        "bin",
        "__pycache__",
    }
    for current_root, dirnames, filenames in os.walk(root):
        dirnames[:] = [name for name in dirnames if name not in skip_dirs]
        for filename in filenames:
            if Path(filename).suffix.lower() in {".ps1", ".bat", ".cmd"}:
                yield Path(current_root) / filename


def adb_path() -> Path:
    return resolve_tool("adb")


def adb(args: list[str], check: bool = True) -> subprocess.CompletedProcess:
    return run([str(adb_path()), *args], check=check, capture=True)


def default_device() -> str | None:
    result = adb(["devices"], check=False)
    lines = (result.stdout or "").splitlines()[1:]
    for line in lines:
        line = line.strip()
        if line and "\tdevice" in line:
            return line.split("\t", 1)[0]
    return None


def ensure_parent(path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)


def replace_global_proto_content(content: str, profile: dict[str, Any]) -> str:
    content = re.sub(
        r'"AssetBundleServerUrl":"[^"]*"',
        f'"AssetBundleServerUrl":"{profile["client"]["assetBundleBaseUrl"]}"',
        content,
    )
    content = re.sub(
        r'"Address":"[^"]*"',
        f'"Address":"{profile["client"]["loginAddress"]}"',
        content,
    )
    return content


def process_manifest_path() -> Path:
    return musf_root() / "reports" / "runtime" / "server-processes.json"


def save_process_manifest(entries: list[dict[str, Any]]) -> Path:
    path = process_manifest_path()
    ensure_parent(path)
    write_json(path, {"generatedAt": timestamp_iso(), "entries": entries})
    return path


def load_process_manifest() -> list[dict[str, Any]]:
    path = process_manifest_path()
    if not path.exists():
        return []
    return read_json(path).get("entries", [])


def parse_args(argv: list[str] | None = None) -> argparse.Namespace:
    parser = argparse.ArgumentParser(prog="musf")
    parser.add_argument("--profile", default="local-lan")
    sub = parser.add_subparsers(dest="command", required=True)

    sub.add_parser("verify-toolchain")
    sub.add_parser("generate-baseline")
    sub.add_parser("classify-legacy-scripts")
    sub.add_parser("sync-network-config")
    sub.add_parser("server-health")
    choose_role_calibrate = sub.add_parser("choose-role-calibrate")
    choose_role_calibrate.add_argument("--image", required=True)
    choose_role_calibrate.add_argument("--point", action="append", default=[])
    choose_role_calibrate.add_argument("--plane-y", type=float, default=1.15)
    choose_role_calibrate.add_argument("--annotated-out", default="")
    choose_role_calibrate.add_argument("--json-out", default="")
    server_start = sub.add_parser("server-start")
    server_start.add_argument("--clean", action="store_true")
    server_stop = sub.add_parser("server-stop")
    server_stop.add_argument("--all-managed", action="store_true")
    sub.add_parser("build-android")
    sub.add_parser("run-gate")
    rollback = sub.add_parser("rollback")
    rollback.add_argument("--source", default="")

    generate_version = sub.add_parser("generate-version")
    generate_version.add_argument("--target-dir", default="")

    chooserole_report = sub.add_parser("report-chooserole-variants")
    chooserole_report.add_argument("--source-dir", default="")

    publish_code_bundle = sub.add_parser("publish-code-bundle")
    publish_code_bundle.add_argument("--source-dir", default="")
    publish_code_bundle.add_argument("--target-dir", action="append", default=[])
    publish_code_bundle.add_argument("--dry-run", action="store_true")

    sync_hotfix = sub.add_parser("sync-hotfix-to-devices")
    sync_hotfix.add_argument("--source-dir", default="")
    sync_hotfix.add_argument("--package-name", default="")
    sync_hotfix.add_argument("--remote-dir", default="")
    sync_hotfix.add_argument("--device", action="append", default=[])
    sync_hotfix.add_argument("--file", action="append", default=[])
    sync_hotfix.add_argument("--launch-after-sync", action="store_true")
    sync_hotfix.add_argument("--dry-run", action="store_true")

    guarded = sub.add_parser("publish-hotfix-guarded")
    guarded.add_argument("--mode", choices=["Routine", "LoginStack"], default="Routine")
    guarded.add_argument("--canary-device", default="")
    guarded.add_argument("--manifest-path", default="")
    guarded.add_argument("--source-dir", default="")
    guarded.add_argument("--dry-run", action="store_true")
    guarded.add_argument("--skip-canary-smoke", action="store_true")

    return parser.parse_args(argv)
