from __future__ import annotations

import json
import subprocess
import shutil
import time
import zlib
from pathlib import Path
from typing import Any, Iterable

from .common import (
    adb,
    adb_path,
    assert_publish_source_dir,
    assert_release_target_dir,
    env_profile,
    file_md5,
    musf_root,
    read_json,
    resolve_tool,
    run,
    timestamp_iso,
    write_json,
    write_text,
)


# Must stay aligned with ETModel.PathHelper.PreferredHotfixFolderName.
HOTFIX_FOLDER_NAME = "星辰战纪"
DEFAULT_PACKAGE_NAME = "com.quanzhilieshou.bytedance.gamecenter"
DEFAULT_SYNC_FILES = [
    "code.unity3d",
    "code.unity3d.manifest",
    "config.unity3d",
    "config.unity3d.manifest",
    "equipplane.unity3d",
    "AttributeNode_InfoConfig.txt",
    "Auto_AreaConfig.txt",
    "BattleCareer_SpellConfig.txt",
    "BattleMaster_ALLConfig.txt",
    "BattleMaster_CareerConfig.txt",
    "BattleMaster_HolyteacherConfig.txt",
    "BattleMaster_SpellConfig.txt",
    "BloodAwakening_InfoConfig.txt",
    "ItemAdvanEntry_BaseConfig.txt",
    "LimitedPurchase_RewardPropsConfig.txt",
    "Reincarnate_InfoConfig.txt",
    "ValueGift_ItemInfoConfig.txt",
    "ValueGift_TypeConfig.txt",
    "Version.txt",
]
FROZEN_UI_BUNDLE_NAMES = {
    "uimaincanvas.unity3d",
    "ui_hud.unity3d",
    "ui_mainpanels.unity3d",
    "ui_skills.unity3d",
    "minmap.unity3d",
}


def default_source_dir() -> Path:
    return musf_root() / "Client" / "UnityBatchShadow" / "Assets" / "StreamingAssets"


def default_live_target_dir() -> Path:
    return musf_root() / "Server" / "Release" / "Android" / "StreamingAssets"


def default_mirror_target_dirs() -> list[Path]:
    root = musf_root()
    return [
        root / "Release_Test" / "Android" / "StreamingAssets",
        root / "Server" / "Release" / "update" / "2.0TestGame" / "Android" / "StreamingAssets",
    ]


def ui_baseline_manifest_path() -> Path:
    return musf_root() / "apk-ui-baseline-manifest.json"


def formal_guard_manifest_path() -> Path:
    return musf_root() / "configs" / "login-stack-manifest.json"


def frozen_ui_baseline_drift(source_dir: Path) -> dict[str, Any]:
    manifest_path = ui_baseline_manifest_path()
    result: dict[str, Any] = {
        "manifestPath": str(manifest_path),
        "checked": [],
        "drift": [],
        "notes": [],
        "passed": True,
    }

    present_names = sorted(name for name in FROZEN_UI_BUNDLE_NAMES if (source_dir / name).exists())
    if not present_names:
        result["notes"].append("No frozen UI bundles present in the candidate source directory.")
        return result

    if not manifest_path.exists():
        result["passed"] = False
        result["notes"].append("APK UI baseline manifest is missing. Run scripts\\run-musf.cmd generate-baseline.")
        return result

    manifest = read_json(manifest_path)
    baseline_entries = {
        str(entry["filename"]): entry
        for entry in manifest.get("entries", [])
        if str(entry.get("filename", "")).lower() in FROZEN_UI_BUNDLE_NAMES
    }

    for name in present_names:
        current = file_state(source_dir / name)
        baseline = baseline_entries.get(name.lower())
        checked_entry = {
            "name": name,
            "sourceSize": current["size"],
            "sourceMD5": current["md5"],
            "baselineSize": None,
            "baselineMD5": None,
            "matched": False,
        }
        if baseline is None:
            result["passed"] = False
            result["drift"].append(
                {
                    "name": name,
                    "reason": "MissingBaselineEntry",
                    "sourceSize": current["size"],
                    "sourceMD5": current["md5"],
                }
            )
            result["checked"].append(checked_entry)
            continue

        checked_entry["baselineSize"] = int(baseline["size"])
        checked_entry["baselineMD5"] = str(baseline["md5"])
        checked_entry["matched"] = checked_entry["baselineSize"] == current["size"] and checked_entry["baselineMD5"] == current["md5"]
        if not checked_entry["matched"]:
            result["passed"] = False
            result["drift"].append(
                {
                    "name": name,
                    "reason": "BaselineMismatch",
                    "baselineSize": checked_entry["baselineSize"],
                    "baselineMD5": checked_entry["baselineMD5"],
                    "sourceSize": current["size"],
                    "sourceMD5": current["md5"],
                    "frozen": True,
                }
            )
        result["checked"].append(checked_entry)

    if result["passed"]:
        result["notes"].append("Frozen UI bundles still match the 20260310.apk baseline.")
    else:
        result["notes"].append("Publish blocked by frozen UI baseline drift.")

    return result


def chooserole_variant_registry_path(manifest: dict[str, Any] | None = None) -> Path:
    configured = ""
    if manifest:
        configured = str(manifest.get("ChooseroleVariantRegistryPath", "")).strip()
    if configured:
        return Path(configured)
    return musf_root() / "configs" / "chooserole-variants.json"


def bundlepatch_path() -> Path:
    root = musf_root()
    candidate = root / "Tools" / "BundlePatch" / "bin" / "Debug" / "netcoreapp3.1" / "BundlePatch.exe"
    if candidate.exists():
        return candidate
    return resolve_tool("bundlepatch")


def release_report_path(stem: str) -> Path:
    root = musf_root()
    return root / "reports" / "releases" / f"{stem}-{time.strftime('%Y%m%d-%H%M%S')}.json"


def guarded_publish_summary_path(report_path: Path) -> Path:
    return report_path.with_name("guarded_publish_summary.md")


def render_guarded_publish_summary(report: dict[str, Any]) -> str:
    lines = [
        "# Guarded Publish Summary",
        "",
        f"- Status: {report.get('status', 'unknown')}",
        f"- Mode: {report.get('mode', '')}",
        f"- Profile: {report.get('profile', '')}",
        f"- SourceDir: {report.get('sourceDir', '')}",
        f"- TargetDir: {report.get('targetDir', '')}",
    ]
    if report.get("blockReason"):
        lines.append(f"- BlockReason: {report['blockReason']}")
    if report.get("error"):
        lines.append(f"- Error: {report['error']}")

    protected_changes = report.get("protectedDifferences") or []
    publish_differences = report.get("publishDifferences") or []
    frozen_ui_gate = report.get("frozenUiBaselineGate") or {}
    chooserole_gate = report.get("chooseroleVariantGate") or {}

    lines.extend(
        [
            "",
            "## Snapshot",
            f"- ProtectedDifferences: {len(protected_changes)}",
            f"- PublishDifferences: {len(publish_differences)}",
            f"- FrozenUiBaselinePassed: {frozen_ui_gate.get('passed', 'n/a')}",
            f"- ChooseroleGatePassed: {chooserole_gate.get('passed', 'n/a')}",
        ]
    )

    if frozen_ui_gate.get("drift"):
        lines.extend(["", "## Frozen UI Drift"])
        for entry in frozen_ui_gate["drift"]:
            lines.append(
                f"- {entry['name']}: {entry['reason']} baseline={entry.get('baselineMD5', '')} current={entry.get('sourceMD5', '')}"
            )

    if chooserole_gate.get("notes"):
        lines.extend(["", "## Chooserole Notes"])
        for note in chooserole_gate["notes"]:
            lines.append(f"- {note}")

    if report.get("canarySmoke"):
        lines.extend(["", "## Canary"])
        lines.append(f"- Passed: {report['canarySmoke'].get('Passed')}")
        if report["canarySmoke"].get("Reason"):
            lines.append(f"- Reason: {report['canarySmoke']['Reason']}")

    if report.get("mirrorUpdates"):
        lines.extend(["", "## Mirror Updates"])
        for entry in report["mirrorUpdates"]:
            lines.append(f"- {entry['targetDir']}: {len(entry.get('copied', []))} files copied")

    return "\n".join(lines) + "\n"


def persist_guarded_publish_report(report_path: Path, report: dict[str, Any]) -> None:
    write_json(report_path, report)
    write_text(guarded_publish_summary_path(report_path), render_guarded_publish_summary(report))


def release_run_dir(prefix: str) -> Path:
    root = musf_root()
    run_dir = root / "reports" / "releases" / "runs" / f"{prefix}_{time.strftime('%Y%m%d-%H%M%S')}"
    run_dir.mkdir(parents=True, exist_ok=True)
    return run_dir


def dedupe(values: Iterable[str]) -> list[str]:
    seen: set[str] = set()
    ordered: list[str] = []
    for value in values:
        normalized = value.strip()
        if normalized and normalized not in seen:
            seen.add(normalized)
            ordered.append(normalized)
    return ordered


def assert_exists(path: Path, description: str) -> None:
    if not path.exists():
        raise FileNotFoundError(f"{description} not found: {path}")


def publishable_files(directory: Path) -> list[Path]:
    assert_exists(directory, "Directory")
    files: list[Path] = []
    for item in sorted(directory.iterdir(), key=lambda path: path.name.lower()):
        if not item.is_file():
            continue
        name = item.name
        lower_name = name.lower()
        if item.suffix.lower() == ".meta":
            continue
        if name == "Version.txt":
            continue
        if ".bak" in lower_name or ".temp-" in lower_name or lower_name.endswith(".patched") or lower_name.endswith(".unpackedwrite") or lower_name.endswith(".downloaded"):
            continue
        if item.suffix.lower() not in {".unity3d", ".manifest", ".assetbundle", ".txt", ""}:
            continue
        if item.suffix.lower() == ".txt" and not (name == "ApkConfig.txt" or name.endswith("Config.txt")):
            continue
        if item.suffix == "" and name != "build_info":
            continue
        files.append(item)
    return files


def file_state(path: Path) -> dict[str, Any]:
    item = path.stat()
    return {
        "name": path.name,
        "fullName": str(path),
        "size": int(item.st_size),
        "md5": file_md5(path),
    }


def scene_marker_presence(path: Path, marker: str) -> bool:
    if not path.exists():
        return False
    return marker in path.read_text(encoding="utf-8")


def load_chooserole_variant_registry(manifest: dict[str, Any] | None = None) -> dict[str, Any]:
    path = chooserole_variant_registry_path(manifest)
    assert_exists(path, "Chooserole variant registry")
    return read_json(path)


def find_chooserole_variant_match(
    registry: dict[str, Any],
    bundle: dict[str, Any] | None,
    manifest_state: dict[str, Any] | None,
) -> dict[str, Any] | None:
    if bundle is None:
        return None
    for variant in registry.get("Variants", []):
        bundle_rule = variant["Bundle"]
        if bundle_rule["MD5"].lower() != bundle["md5"].lower() or int(bundle_rule["Size"]) != int(bundle["size"]):
            continue
        manifest_rule = variant.get("Manifest")
        if manifest_rule:
            if manifest_state is None:
                continue
            if manifest_rule["MD5"].lower() != manifest_state["md5"].lower():
                continue
            if int(manifest_rule["Size"]) != int(manifest_state["size"]):
                continue
        return variant
    return None


def inspect_chooserole_variant(source_dir: Path, manifest: dict[str, Any] | None = None) -> dict[str, Any]:
    registry = load_chooserole_variant_registry(manifest)
    bundle_path = source_dir / registry.get("BundleName", "chooserole.unity3d")
    manifest_path = source_dir / registry.get("ManifestName", "chooserole.unity3d.manifest")
    bundle_state = file_state(bundle_path) if bundle_path.exists() else None
    manifest_state = file_state(manifest_path) if manifest_path.exists() else None
    matched_variant = find_chooserole_variant_match(registry, bundle_state, manifest_state)

    current_scene = registry.get("CurrentProjectScene", {})
    current_scene_path = Path(current_scene.get("Path", ""))
    current_scene_state = {
        "path": str(current_scene_path),
        "exists": current_scene_path.exists(),
        "md5": file_md5(current_scene_path) if current_scene_path.exists() else None,
        "expectedMd5": current_scene.get("MD5"),
        "markers": {},
    }
    for marker in registry.get("RequiredSceneMarkers", []):
        current_scene_state["markers"][marker] = scene_marker_presence(current_scene_path, marker)

    reference_scenes: list[dict[str, Any]] = []
    for reference in registry.get("ReferenceScenes", []):
        reference_path = Path(reference.get("Path", ""))
        reference_scenes.append(
            {
                "id": reference.get("Id"),
                "path": str(reference_path),
                "exists": reference_path.exists(),
                "md5": file_md5(reference_path) if reference_path.exists() else None,
                "expectedMd5": reference.get("MD5"),
                "notes": reference.get("Notes", ""),
            }
        )

    return {
        "registryPath": str(chooserole_variant_registry_path(manifest)),
        "sourceDir": str(source_dir),
        "bundle": bundle_state,
        "manifest": manifest_state,
        "matchedVariant": matched_variant,
        "currentProjectScene": current_scene_state,
        "referenceScenes": reference_scenes,
    }


def chooserole_change_names(
    protected_changes: list[dict[str, Any]],
    differences: list[dict[str, Any]],
) -> list[str]:
    relevant = []
    for entry in [*protected_changes, *differences]:
        name = str(entry.get("name", ""))
        if name in {"chooserole.unity3d", "chooserole.unity3d.manifest"}:
            relevant.append(name)
    return dedupe(relevant)


def validate_chooserole_publish_candidate(
    source_dir: Path,
    *,
    manifest: dict[str, Any],
    protected_changes: list[dict[str, Any]],
    differences: list[dict[str, Any]],
) -> dict[str, Any]:
    inspection = inspect_chooserole_variant(source_dir, manifest)
    relevant_names = chooserole_change_names(protected_changes, differences)
    matched_variant = inspection.get("matchedVariant")
    publish_policy = matched_variant.get("PublishPolicy") if matched_variant else None
    passed = True
    block_reason = ""
    notes: list[str] = []

    if relevant_names:
        if matched_variant is None:
            passed = False
            block_reason = "UnregisteredChooseroleVariant"
            notes.append("chooserole bundle differs, but the current bundle+manifest pair is not registered.")
        elif publish_policy != "Allowed":
            passed = False
            block_reason = f"ChooseroleVariantPolicy:{publish_policy}"
            notes.append(
                f"chooserole bundle differs and matched variant {matched_variant['Id']} is marked {publish_policy}."
            )

    current_scene = inspection["currentProjectScene"]
    missing_markers = [name for name, exists in current_scene.get("markers", {}).items() if not exists]
    if missing_markers:
        notes.append(f"Current source scene is missing required markers: {', '.join(missing_markers)}")

    return {
        "relevantNames": relevant_names,
        "passed": passed,
        "blockReason": block_reason,
        "notes": notes,
        "inspection": inspection,
    }


def normalize_utf8_no_bom(path: Path) -> bool:
    raw = path.read_bytes()
    bom = b"\xef\xbb\xbf"
    if raw.startswith(bom):
        path.write_text(raw[len(bom):].decode("utf-8"), encoding="utf-8")
        return True
    return False


def version_payload_for_directory(target_dir: Path) -> dict[str, Any]:
    file_info_dict: dict[str, Any] = {}
    total_size = 0
    for item in publishable_files(target_dir):
        info = {
            "File": item.name,
            "MD5": file_md5(item),
            "Size": int(item.stat().st_size),
        }
        file_info_dict[item.name] = info
        total_size += info["Size"]
    return {
        "Version": int(time.time()),
        "TotalSize": total_size,
        "FileInfoDict": file_info_dict,
    }


def write_version_file_for_directory(target_dir: Path) -> dict[str, Any]:
    payload = version_payload_for_directory(target_dir)
    version_path = target_dir / "Version.txt"
    version_path.write_text(json.dumps(payload, ensure_ascii=False, separators=(",", ":")), encoding="utf-8")
    return {
        "versionPath": str(version_path),
        "version": payload["Version"],
        "totalSize": payload["TotalSize"],
    }


def target_difference_set(source_dir: Path, target_dir: Path) -> list[dict[str, Any]]:
    differences: list[dict[str, Any]] = []
    for source_file in publishable_files(source_dir):
        source_state = file_state(source_file)
        target_path = target_dir / source_file.name
        if not target_path.exists():
            differences.append(
                {
                    "name": source_file.name,
                    "changeType": "MissingInTarget",
                    "sourceSize": source_state["size"],
                    "sourceMD5": source_state["md5"],
                    "targetSize": None,
                    "targetMD5": None,
                    "existedInTarget": False,
                }
            )
            continue

        target_size = int(target_path.stat().st_size)
        target_md5 = file_md5(target_path) if target_size == source_state["size"] else None
        if target_size != source_state["size"] or target_md5 != source_state["md5"]:
            if target_md5 is None:
                target_md5 = file_md5(target_path)
            differences.append(
                {
                    "name": source_file.name,
                    "changeType": "Different",
                    "sourceSize": source_state["size"],
                    "sourceMD5": source_state["md5"],
                    "targetSize": target_size,
                    "targetMD5": target_md5,
                    "existedInTarget": True,
                }
            )
    return differences


def protected_differences(manifest: dict[str, Any], source_dir: Path) -> list[dict[str, Any]]:
    differences: list[dict[str, Any]] = []
    for protected in manifest.get("ProtectedFiles", []):
        source_path = source_dir / protected["Name"]
        assert_exists(source_path, "Protected source file")
        current = file_state(source_path)
        if current["size"] != int(protected["Size"]) or current["md5"] != protected["MD5"]:
            differences.append(
                {
                    "name": protected["Name"],
                    "guardMode": protected["GuardMode"],
                    "baselineSize": int(protected["Size"]),
                    "baselineMD5": protected["MD5"],
                    "sourceSize": current["size"],
                    "sourceMD5": current["md5"],
                }
            )
    return differences


def copy_named_files(source_dir: Path, target_dir: Path, file_names: Iterable[str], *, timestamp: str | None = None) -> list[dict[str, Any]]:
    target_dir.mkdir(parents=True, exist_ok=True)
    copied: list[dict[str, Any]] = []
    for name in dedupe(file_names):
        source_path = source_dir / name
        target_path = target_dir / name
        assert_exists(source_path, "Source file")
        backup_path = None
        if timestamp and target_path.exists():
            backup_path = target_path.with_name(f"{target_path.name}.bak-{timestamp}")
            shutil.copy2(target_path, backup_path)
        shutil.copy2(source_path, target_path)
        copied.append(
            {
                "name": name,
                "targetPath": str(target_path),
                "backupPath": str(backup_path) if backup_path else None,
            }
        )
    return copied


def new_publish_snapshot(target_dir: Path, tracked_files: Iterable[str], snapshot_dir: Path) -> dict[str, Any]:
    snapshot_dir.mkdir(parents=True, exist_ok=True)
    files_dir = snapshot_dir / "files"
    files_dir.mkdir(parents=True, exist_ok=True)

    tracked_entries: list[dict[str, Any]] = []
    for name in dedupe(tracked_files):
        target_path = target_dir / name
        snapshot_path = files_dir / name
        existed_before = target_path.exists()
        if existed_before:
            shutil.copy2(target_path, snapshot_path)
            tracked_entries.append(
                {
                    "name": name,
                    "existedBefore": True,
                    "snapshotPath": str(snapshot_path),
                    "size": int(target_path.stat().st_size),
                    "md5": file_md5(target_path),
                }
            )
        else:
            tracked_entries.append(
                {
                    "name": name,
                    "existedBefore": False,
                    "snapshotPath": None,
                    "size": None,
                    "md5": None,
                }
            )

    snapshot = {
        "createdAt": timestamp_iso(),
        "targetDir": str(target_dir),
        "trackedFiles": tracked_entries,
    }
    write_json(snapshot_dir / "snapshot.json", snapshot)
    return snapshot


def restore_publish_snapshot(snapshot: dict[str, Any]) -> None:
    target_dir = Path(snapshot["targetDir"])
    for entry in snapshot.get("trackedFiles", []):
        target_path = target_dir / entry["name"]
        if entry["existedBefore"]:
            shutil.copy2(Path(entry["snapshotPath"]), target_path)
        elif target_path.exists():
            target_path.unlink()


def adb_lines(args: list[str], *, check: bool = True) -> list[str]:
    result = adb(args, check=check)
    lines = []
    for source in (result.stdout or "", result.stderr or ""):
        for line in source.splitlines():
            text = line.strip()
            if text:
                lines.append(text)
    return lines


def connected_devices() -> list[str]:
    devices: list[str] = []
    for line in adb_lines(["devices"], check=False)[1:]:
        if line.endswith("\tdevice"):
            devices.append(line.split("\t", 1)[0])
    return devices


def remote_files_dir(package_name: str) -> str:
    return f"/storage/emulated/0/Android/data/{package_name}/files"


def remote_hotfix_dir(package_name: str, preferred_dir: str = "") -> str:
    if preferred_dir.strip():
        return preferred_dir.strip()
    return f"{remote_files_dir(package_name)}/Android/{HOTFIX_FOLDER_NAME}"


def remote_owner_group(serial: str, target_dir: str) -> str:
    lines = adb_lines(["-s", serial, "shell", f'ls -ld "{target_dir}"'])
    if not lines:
        raise RuntimeError(f"Unable to resolve owner/group for remote dir: {target_dir}")
    parts = lines[0].split()
    if len(parts) < 4:
        raise RuntimeError(f"Unexpected ls output while resolving owner/group: {lines[0]}")
    return f"{parts[2]}:{parts[3]}"


def push_file_to_remote(serial: str, local_path: Path, remote_path: str, temp_root: str = "/data/local/tmp") -> None:
    stage_dir = musf_root() / "Temp" / "adb-stage"
    stage_dir.mkdir(parents=True, exist_ok=True)
    staged_path = stage_dir / local_path.name
    shutil.copy2(local_path, staged_path)
    adb_lines(["-s", serial, "push", str(staged_path), temp_root])
    uploaded_temp_path = f"{temp_root}/{local_path.name}"
    remote_temp_path = f"{temp_root}/musf-sync-{local_path.name}"
    copy_command = f'mv "{uploaded_temp_path}" "{remote_temp_path}" && cp "{remote_temp_path}" "{remote_path}" && rm -f "{remote_temp_path}"'
    adb_lines(["-s", serial, "shell", f"su 0 sh -c '{copy_command}'"])


def capture_device_screenshot(serial: str, output_path: Path) -> Path:
    output_path.parent.mkdir(parents=True, exist_ok=True)
    exec_out = subprocess.run(
        [str(adb_path()), "-s", serial, "exec-out", "screencap", "-p"],
        check=False,
        capture_output=True,
    )
    if exec_out.returncode == 0 and exec_out.stdout:
        output_path.write_bytes(exec_out.stdout)
        if output_path.exists() and output_path.stat().st_size > 1024:
            return output_path

    stage_dir = musf_root() / "Temp" / "adb-stage"
    stage_dir.mkdir(parents=True, exist_ok=True)
    remote_path = f"/sdcard/Download/musf-guarded-{int(time.time())}-{output_path.name}"
    staged_local_path = stage_dir / output_path.name
    adb_lines(["-s", serial, "shell", "screencap", "-p", remote_path])
    adb_lines(["-s", serial, "pull", remote_path, str(staged_local_path)])
    shutil.copy2(staged_local_path, output_path)
    adb_lines(["-s", serial, "shell", "rm", "-f", remote_path], check=False)
    return output_path


def prepare_canary_login_state(manifest: dict[str, Any], serial: str) -> dict[str, Any]:
    canary = manifest["Canary"]
    package_name = canary["PackageName"]
    files_dir = remote_files_dir(package_name)
    adb_lines(["-s", serial, "shell", "mkdir", "-p", files_dir])
    adb_lines(["-s", serial, "shell", "am", "force-stop", package_name], check=False)

    owner_group = remote_owner_group(serial, files_dir)
    state_dir = Path(canary["LocalStateDir"])
    assert_exists(state_dir, "Canary local state dir")

    pushed: list[str] = []
    moved_aside: list[str] = []
    for name in canary.get("KeepAndPushFiles", []):
        local_path = state_dir / name
        assert_exists(local_path, "Canary local state file")
        remote_path = f"{files_dir}/{name}"
        push_file_to_remote(serial, local_path, remote_path)
        fix_command = f'chown {owner_group} "{remote_path}" && chmod 660 "{remote_path}"'
        adb_lines(["-s", serial, "shell", f"su 0 sh -c '{fix_command}'"])
        pushed.append(remote_path)

    for name in canary.get("ClearOrMoveFiles", []):
        remote_path = f"{files_dir}/{name}"
        backup_path = f"{remote_path}.canarybak-{int(time.time())}"
        move_command = f'if [ -f "{remote_path}" ]; then mv "{remote_path}" "{backup_path}"; fi'
        adb_lines(["-s", serial, "shell", f"su 0 sh -c '{move_command}'"], check=False)
        moved_aside.append(remote_path)

    return {
        "packageName": package_name,
        "remoteFilesDir": files_dir,
        "pushed": pushed,
        "clearedOrMoved": moved_aside,
    }


def remove_remote_hotfix_files(package_name: str, serial: str, file_names: Iterable[str]) -> None:
    hotfix_dir = remote_hotfix_dir(package_name)
    for name in dedupe(file_names):
        remote_path = f"{hotfix_dir}/{name}"
        adb_lines(["-s", serial, "shell", f"su 0 sh -c 'rm -f \"{remote_path}\"'"], check=False)


def load_png_pixels(path: Path) -> tuple[int, int, int, list[bytes]]:
    raw = path.read_bytes()
    if raw[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError(f"Unsupported screenshot format: {path}")

    width = height = bit_depth = color_type = 0
    pos = 8
    idat = bytearray()
    while pos < len(raw):
        length = int.from_bytes(raw[pos : pos + 4], "big")
        pos += 4
        chunk_type = raw[pos : pos + 4]
        pos += 4
        chunk_data = raw[pos : pos + length]
        pos += length + 4
        if chunk_type == b"IHDR":
            width = int.from_bytes(chunk_data[0:4], "big")
            height = int.from_bytes(chunk_data[4:8], "big")
            bit_depth = chunk_data[8]
            color_type = chunk_data[9]
        elif chunk_type == b"IDAT":
            idat.extend(chunk_data)
        elif chunk_type == b"IEND":
            break

    if bit_depth != 8 or color_type not in {2, 6}:
        raise ValueError(f"Unsupported PNG color format bitDepth={bit_depth} colorType={color_type}")

    channels = 3 if color_type == 2 else 4
    stride = width * channels
    decompressed = zlib.decompress(bytes(idat))
    rows: list[bytes] = []
    index = 0
    previous = bytearray(stride)
    for _ in range(height):
        filter_type = decompressed[index]
        index += 1
        row = bytearray(decompressed[index : index + stride])
        index += stride
        apply_png_filter(row, previous, channels, filter_type)
        rows.append(bytes(row))
        previous = row
    return width, height, channels, rows


def paeth_predictor(a: int, b: int, c: int) -> int:
    p = a + b - c
    pa = abs(p - a)
    pb = abs(p - b)
    pc = abs(p - c)
    if pa <= pb and pa <= pc:
        return a
    if pb <= pc:
        return b
    return c


def apply_png_filter(row: bytearray, previous: bytearray, bpp: int, filter_type: int) -> None:
    if filter_type == 0:
        return
    if filter_type == 1:
        for idx in range(len(row)):
            left = row[idx - bpp] if idx >= bpp else 0
            row[idx] = (row[idx] + left) & 0xFF
        return
    if filter_type == 2:
        for idx in range(len(row)):
            row[idx] = (row[idx] + previous[idx]) & 0xFF
        return
    if filter_type == 3:
        for idx in range(len(row)):
            left = row[idx - bpp] if idx >= bpp else 0
            up = previous[idx]
            row[idx] = (row[idx] + ((left + up) // 2)) & 0xFF
        return
    if filter_type == 4:
        for idx in range(len(row)):
            left = row[idx - bpp] if idx >= bpp else 0
            up = previous[idx]
            up_left = previous[idx - bpp] if idx >= bpp else 0
            row[idx] = (row[idx] + paeth_predictor(left, up, up_left)) & 0xFF
        return
    raise ValueError(f"Unsupported PNG filter type: {filter_type}")


def image_region_metrics(path: Path, x: int, y: int, width: int, height: int) -> dict[str, Any]:
    image_width, image_height, channels, rows = load_png_pixels(path)
    dark = gold = green = bright = count = 0
    for yy in range(y, min(y + height, image_height), 2):
        row = rows[yy]
        for xx in range(x, min(x + width, image_width), 2):
            offset = xx * channels
            red, green_value, blue = row[offset], row[offset + 1], row[offset + 2]
            luma = (red + green_value + blue) / 3.0
            if luma < 55:
                dark += 1
            if luma > 170:
                bright += 1
            if red > 140 and green_value > 110 and blue < 110 and (red - blue) > 50:
                gold += 1
            if green_value > 120 and red < 180 and blue < 140 and (green_value - red) > 20 and (green_value - blue) > 20:
                green += 1
            count += 1
    if count == 0:
        count = 1
    return {
        "Width": image_width,
        "Height": image_height,
        "DarkRatio": round(dark / count, 4),
        "GoldRatio": round(gold / count, 4),
        "GreenRatio": round(green / count, 4),
        "BrightRatio": round(bright / count, 4),
    }


def test_login_visual_templates(manifest: dict[str, Any], screenshot_path: Path) -> dict[str, Any]:
    image_width, image_height, _, _ = load_png_pixels(screenshot_path)
    visual = manifest["VisualSmoke"]
    scale_x = image_width / float(visual["ReferenceWidth"])
    scale_y = image_height / float(visual["ReferenceHeight"])

    template_sets = visual.get("TemplateSets") or [
        {
            "Name": "default",
            "RequiredTemplateHits": int(visual["RequiredTemplateHits"]),
            "Templates": visual["Templates"],
        }
    ]

    set_results: list[dict[str, Any]] = []
    matched_set: dict[str, Any] | None = None
    for template_set in template_sets:
        results: list[dict[str, Any]] = []
        hits = 0
        required_hits = int(template_set.get("RequiredTemplateHits", visual["RequiredTemplateHits"]))
        must_pass_templates = [str(name) for name in template_set.get("MustPassTemplates", [])]
        for template in template_set["Templates"]:
            x = max(0, min(round(template["X"] * scale_x), image_width - 2))
            y = max(0, min(round(template["Y"] * scale_y), image_height - 2))
            width = max(2, min(round(template["Width"] * scale_x), image_width - x))
            height = max(2, min(round(template["Height"] * scale_y), image_height - y))
            gold_ratio_min = float(template.get("GoldRatioMin", 0.0))
            green_ratio_min = float(template.get("GreenRatioMin", 0.0))
            metrics = image_region_metrics(screenshot_path, x, y, width, height)
            passed = (
                metrics["DarkRatio"] <= float(template["DarkRatioMax"])
                and metrics["GoldRatio"] >= gold_ratio_min
                and metrics["GreenRatio"] >= green_ratio_min
            )
            if passed:
                hits += 1
            results.append(
                {
                    "Name": template["Name"],
                    "Region": {"X": x, "Y": y, "Width": width, "Height": height},
                    "DarkRatio": metrics["DarkRatio"],
                    "GoldRatio": metrics["GoldRatio"],
                    "GreenRatio": metrics["GreenRatio"],
                    "BrightRatio": metrics["BrightRatio"],
                    "DarkRatioMax": float(template["DarkRatioMax"]),
                    "GoldRatioMin": gold_ratio_min,
                    "GreenRatioMin": green_ratio_min,
                    "Passed": passed,
                }
            )

        must_pass_satisfied = True
        for required_name in must_pass_templates:
            matched = next((item for item in results if item["Name"] == required_name), None)
            if not matched or not matched["Passed"]:
                must_pass_satisfied = False
                break

        set_result = {
            "Name": template_set["Name"],
            "HitCount": hits,
            "RequiredTemplateHits": required_hits,
            "MustPassTemplates": must_pass_templates,
            "MustPassSatisfied": must_pass_satisfied,
            "Passed": hits >= required_hits and must_pass_satisfied,
            "Templates": results,
        }
        set_results.append(set_result)
        if matched_set is None and set_result["Passed"]:
            matched_set = set_result

    selected_set = matched_set or (set_results[0] if set_results else None)
    return {
        "ScreenshotPath": str(screenshot_path),
        "HitCount": selected_set["HitCount"] if selected_set else 0,
        "RequiredTemplateHits": selected_set["RequiredTemplateHits"] if selected_set else int(visual["RequiredTemplateHits"]),
        "Passed": matched_set is not None,
        "MatchedSetName": matched_set["Name"] if matched_set else None,
        "TemplateSets": set_results,
        "Templates": selected_set["Templates"] if selected_set else [],
    }


def start_canary_smoke_test(manifest: dict[str, Any], serial: str, run_dir: Path, skip_visual: bool = False) -> dict[str, Any]:
    canary = manifest["Canary"]
    package_name = canary["PackageName"]
    poll_seconds = int(canary["ScreenshotPollSeconds"])
    timeout_seconds = int(canary["AppLaunchTimeoutSeconds"])
    max_attempts = max(1, (timeout_seconds + poll_seconds - 1) // poll_seconds)
    screens_dir = run_dir / "canary_screens"
    screens_dir.mkdir(parents=True, exist_ok=True)

    adb_lines(["-s", serial, "shell", "am", "force-stop", package_name], check=False)
    adb_lines(["-s", serial, "shell", "monkey", "-p", package_name, "-c", "android.intent.category.LAUNCHER", "1"])

    last_result: dict[str, Any] | None = None
    for attempt in range(1, max_attempts + 1):
        time.sleep(poll_seconds)
        screenshot_path = screens_dir / f"canary-{attempt}.png"
        capture_device_screenshot(serial, screenshot_path)
        if skip_visual:
            last_result = {
                "Attempt": attempt,
                "Passed": True,
                "ScreenshotPath": str(screenshot_path),
                "SkippedVisualCheck": True,
            }
            return last_result

        evaluation = test_login_visual_templates(manifest, screenshot_path)
        evaluation["Attempt"] = attempt
        last_result = evaluation
        if evaluation["Passed"]:
            return evaluation

    return last_result or {"Passed": False, "Reason": "NoScreenshotCaptured"}


def extract_hotfix_dll_for_scan(code_bundle_path: Path, output_dir: Path) -> Path:
    tool = bundlepatch_path()
    assert_exists(tool, "BundlePatch tool")
    assert_exists(code_bundle_path, "Source code bundle")
    if output_dir.exists():
        shutil.rmtree(output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    result = run([str(tool), "extract", str(code_bundle_path), str(output_dir)], check=False)
    if result.returncode != 0:
        raise RuntimeError(f"BundlePatch extract failed: {(result.stdout or '')}{(result.stderr or '')}")
    dll_path = output_dir / "Hotfix.dll.bytes"
    assert_exists(dll_path, "Extracted Hotfix.dll.bytes")
    return dll_path


def test_hotfix_markers(manifest: dict[str, Any], code_bundle_path: Path, run_dir: Path) -> dict[str, Any]:
    extract_dir = run_dir / "hotfix_extract"
    dll_path = extract_hotfix_dll_for_scan(code_bundle_path, extract_dir)
    content = dll_path.read_bytes().decode("latin-1")
    violations: list[dict[str, Any]] = []
    for literal in manifest.get("ForbiddenMarkers", {}).get("Literal", []):
        if str(literal).lower() in content.lower():
            violations.append({"Type": "Literal", "Pattern": str(literal)})
    for pattern in manifest.get("ForbiddenMarkers", {}).get("Regex", []):
        import re

        for match in re.finditer(str(pattern), content, re.IGNORECASE):
            violations.append({"Type": "Regex", "Pattern": str(pattern), "Match": match.group(0)})
    return {
        "ExtractDir": str(extract_dir),
        "HotfixDllPath": str(dll_path),
        "Passed": len(violations) == 0,
        "Violations": violations,
    }


def update_manifest_baseline(manifest_path: Path, manifest: dict[str, Any], source_dir: Path, target_dir: Path) -> None:
    for protected in manifest.get("ProtectedFiles", []):
        path = target_dir / protected["Name"] if protected["Name"] == "Version.txt" else source_dir / protected["Name"]
        assert_exists(path, "Protected baseline refresh file")
        state = file_state(path)
        protected["Size"] = state["size"]
        protected["MD5"] = state["md5"]

    stable_baseline_dir = Path(manifest.get("StableBaselineSourceDir", "")).expanduser() if manifest.get("StableBaselineSourceDir") else None
    if stable_baseline_dir:
        stable_baseline_dir.mkdir(parents=True, exist_ok=True)
        for protected in manifest.get("ProtectedFiles", []):
            source_path = target_dir / protected["Name"] if protected["Name"] == "Version.txt" else source_dir / protected["Name"]
            shutil.copy2(source_path, stable_baseline_dir / protected["Name"])

    write_json(manifest_path, manifest)


def resolve_sync_source(source_dir: str | None) -> Path:
    return Path(source_dir) if source_dir else default_live_target_dir()


def sync_hotfix_to_devices(
    profile_name: str,
    *,
    source_dir: str | None = None,
    package_name: str | None = None,
    remote_dir: str = "",
    devices: list[str] | None = None,
    files_to_sync: list[str] | None = None,
    launch_after_sync: bool = False,
    dry_run: bool = False,
) -> Path:
    profile = env_profile(profile_name)
    resolved_source = resolve_sync_source(source_dir)
    assert_publish_source_dir(resolved_source, "Sync source directory")
    assert_exists(resolved_source, "Sync source directory")
    resolved_package = package_name or profile["client"].get("packageName") or DEFAULT_PACKAGE_NAME
    selected_files = dedupe(files_to_sync or DEFAULT_SYNC_FILES)
    if not selected_files:
        raise RuntimeError("No files selected for device sync")

    normalized_version = False
    version_path = resolved_source / "Version.txt"
    if "Version.txt" in selected_files and version_path.exists():
        normalized_version = normalize_utf8_no_bom(version_path)

    for name in selected_files:
        assert_exists(resolved_source / name, "Sync source file")

    resolved_devices = devices or connected_devices()
    if not resolved_devices:
        raise RuntimeError("No connected adb devices found")

    timestamp = time.strftime("%Y%m%d-%H%M%S")
    report = {
        "generatedAt": timestamp_iso(),
        "profile": profile_name,
        "sourceDir": str(resolved_source),
        "packageName": resolved_package,
        "remoteHotfixDir": remote_hotfix_dir(resolved_package, remote_dir),
        "devices": resolved_devices,
        "files": selected_files,
        "launchAfterSync": launch_after_sync,
        "dryRun": dry_run,
        "normalizedVersionNoBom": normalized_version,
        "entries": [],
    }

    for serial in resolved_devices:
        resolved_remote_dir = remote_hotfix_dir(resolved_package, remote_dir)
        entry: dict[str, Any] = {
            "serial": serial,
            "remoteHotfixDir": resolved_remote_dir,
            "status": "planned" if dry_run else "running",
            "backups": [],
            "synced": [],
        }
        report["entries"].append(entry)
        if dry_run:
            continue

        adb_lines(["-s", serial, "shell", "mkdir", "-p", resolved_remote_dir])
        adb_lines(["-s", serial, "shell", "am", "force-stop", resolved_package], check=False)
        owner_group = remote_owner_group(serial, resolved_remote_dir)

        for name in selected_files:
            remote_path = f"{resolved_remote_dir}/{name}"
            backup_path = f"{remote_path}.bak-{timestamp}"
            backup_command = f'if [ -f "{remote_path}" ]; then cp "{remote_path}" "{backup_path}"; fi'
            adb_lines(["-s", serial, "shell", backup_command], check=False)
            entry["backups"].append({"name": name, "backupPath": backup_path})

        for name in selected_files:
            local_path = resolved_source / name
            remote_path = f"{resolved_remote_dir}/{name}"
            push_file_to_remote(serial, local_path, remote_path)
            fix_command = f'chown {owner_group} "{remote_path}" && chmod 660 "{remote_path}"'
            adb_lines(["-s", serial, "shell", f"su 0 sh -c '{fix_command}'"], check=False)
            entry["synced"].append(
                {
                    "name": name,
                    "localSize": int(local_path.stat().st_size),
                    "localMD5": file_md5(local_path),
                    "remotePath": remote_path,
                }
            )

        if launch_after_sync:
            adb_lines(["-s", serial, "shell", "monkey", "-p", resolved_package, "-c", "android.intent.category.LAUNCHER", "1"])
        entry["status"] = "success"

    output = release_report_path("sync-hotfix-to-devices")
    write_json(output, report)
    return output


def report_chooserole_variants(profile_name: str, *, source_dir: str | None = None) -> Path:
    resolved_source = Path(source_dir) if source_dir else default_live_target_dir()
    assert_publish_source_dir(resolved_source, "Chooserole report source directory")
    assert_exists(resolved_source, "Chooserole report source directory")
    manifest = read_json(formal_guard_manifest_path())
    inspection = inspect_chooserole_variant(resolved_source, manifest)
    output = release_report_path("report-chooserole-variants")
    write_json(
        output,
        {
            "generatedAt": timestamp_iso(),
            "profile": profile_name,
            "sourceDir": str(resolved_source),
            "inspection": inspection,
        },
    )
    return output


def generate_version(profile_name: str, *, target_dir: str | None = None) -> Path:
    resolved_target = Path(target_dir) if target_dir else default_live_target_dir()
    assert_release_target_dir(resolved_target, "Version target directory")
    assert_exists(resolved_target, "Version target directory")
    version_info = write_version_file_for_directory(resolved_target)
    output = release_report_path("generate-version")
    write_json(
        output,
        {
            "generatedAt": timestamp_iso(),
            "profile": profile_name,
            "targetDir": str(resolved_target),
            "version": version_info,
        },
    )
    return output


def publish_code_bundle(
    profile_name: str,
    *,
    source_dir: str | None = None,
    target_dirs: list[str] | None = None,
    dry_run: bool = False,
) -> Path:
    resolved_source = Path(source_dir) if source_dir else default_source_dir()
    assert_publish_source_dir(resolved_source, "Code bundle source directory")
    assert_exists(resolved_source, "Code bundle source directory")
    bundle_name = "code.unity3d"
    manifest_name = "code.unity3d.manifest"
    assert_exists(resolved_source / bundle_name, "Source code bundle")
    assert_exists(resolved_source / manifest_name, "Source code manifest")

    targets = [Path(item) for item in (target_dirs or [str(default_live_target_dir()), *(str(path) for path in default_mirror_target_dirs())])]
    report = {
        "generatedAt": timestamp_iso(),
        "profile": profile_name,
        "sourceDir": str(resolved_source),
        "targets": [],
        "dryRun": dry_run,
        "bundle": file_state(resolved_source / bundle_name),
        "manifest": file_state(resolved_source / manifest_name),
    }
    timestamp = time.strftime("%Y%m%d-%H%M%S")

    for target_dir in targets:
        assert_release_target_dir(target_dir, "Code bundle target directory")
        assert_exists(target_dir, "Target directory")
        differences = target_difference_set(resolved_source, target_dir)
        relevant_names = [name for name in [bundle_name, manifest_name] if name in {item["name"] for item in differences}]
        target_entry = {
            "targetDir": str(target_dir),
            "differs": bool(relevant_names),
            "differenceNames": relevant_names,
            "versionPath": str(target_dir / "Version.txt"),
        }
        if not dry_run:
            copy_names = [bundle_name, manifest_name]
            if (resolved_source / "build_info").exists():
                copy_names.append("build_info")
            copied = copy_named_files(resolved_source, target_dir, copy_names, timestamp=timestamp)
            version_info = write_version_file_for_directory(target_dir)
            target_entry["copied"] = copied
            target_entry["versionInfo"] = version_info
        report["targets"].append(target_entry)

    output = release_report_path("publish-code-bundle")
    write_json(output, report)
    return output


def publish_hotfix_guarded(
    profile_name: str,
    *,
    mode: str,
    canary_device: str = "",
    manifest_path: str = "",
    source_dir: str | None = None,
    dry_run: bool = False,
    skip_canary_smoke: bool = False,
) -> Path:
    resolved_source = Path(source_dir) if source_dir else default_source_dir()
    assert_publish_source_dir(resolved_source, "Guarded publish source directory")
    assert_exists(resolved_source, "Guarded publish source directory")
    resolved_manifest_path = Path(manifest_path) if manifest_path else formal_guard_manifest_path()
    manifest = read_json(resolved_manifest_path)

    target_dir = Path(manifest.get("LiveTargetDir") or default_live_target_dir())
    mirror_dirs = [Path(path) for path in manifest.get("MirrorTargetDirs", [])]
    assert_release_target_dir(target_dir, "Live target directory")
    assert_exists(target_dir, "Live target directory")
    for mirror_dir in mirror_dirs:
        assert_release_target_dir(mirror_dir, "Mirror target directory")

    run_dir = release_run_dir("publish_guarded")
    report_path = run_dir / "guarded_publish_report.json"
    resolved_canary_device = canary_device or manifest.get("Canary", {}).get("Device", "")
    current_devices = connected_devices()
    report: dict[str, Any] = {
        "startedAt": timestamp_iso(),
        "mode": mode,
        "profile": profile_name,
        "canaryDevice": resolved_canary_device,
        "sourceDir": str(resolved_source),
        "targetDir": str(target_dir),
        "mirrorTargetDirs": [str(path) for path in mirror_dirs],
        "manifestPath": str(resolved_manifest_path),
        "runDir": str(run_dir),
        "connectedDevices": current_devices,
        "status": "Running",
        "dryRun": dry_run,
        "skipCanarySmoke": skip_canary_smoke,
    }
    persist_guarded_publish_report(report_path, report)
    if not resolved_canary_device:
        report["status"] = "Blocked"
        report["blockReason"] = "MissingCanaryDevice"
        report["error"] = "No canary device specified and manifest Canary.Device is empty"
        persist_guarded_publish_report(report_path, report)
        return report_path

    if resolved_canary_device not in current_devices:
        report["status"] = "Blocked"
        report["blockReason"] = "CanaryDeviceNotConnected"
        report["error"] = f"Canary device not connected: {resolved_canary_device}"
        persist_guarded_publish_report(report_path, report)
        return report_path

    marker_scan = test_hotfix_markers(manifest, resolved_source / "code.unity3d", run_dir)
    report["markerScan"] = marker_scan
    persist_guarded_publish_report(report_path, report)
    if not marker_scan["Passed"]:
        report["status"] = "Blocked"
        report["blockReason"] = "ForbiddenHotfixMarkers"
        persist_guarded_publish_report(report_path, report)
        return report_path

    protected_changes = protected_differences(manifest, resolved_source)
    report["protectedDifferences"] = protected_changes
    persist_guarded_publish_report(report_path, report)

    frozen_ui_gate = frozen_ui_baseline_drift(resolved_source)
    report["frozenUiBaselineGate"] = frozen_ui_gate
    persist_guarded_publish_report(report_path, report)
    if not frozen_ui_gate["passed"]:
        report["status"] = "Blocked"
        report["blockReason"] = "FrozenUiBaselineDrift"
        persist_guarded_publish_report(report_path, report)
        return report_path

    routine_blocking = [entry for entry in protected_changes if entry["guardMode"] == "BlockRoutine"]
    if mode == "Routine" and routine_blocking:
        report["status"] = "Blocked"
        report["blockReason"] = "ProtectedFilesChangedInRoutineMode"
        persist_guarded_publish_report(report_path, report)
        return report_path

    differences = target_difference_set(resolved_source, target_dir)
    report["publishDifferences"] = differences
    persist_guarded_publish_report(report_path, report)

    chooserole_gate = validate_chooserole_publish_candidate(
        resolved_source,
        manifest=manifest,
        protected_changes=protected_changes,
        differences=differences,
    )
    report["chooseroleVariantGate"] = chooserole_gate
    persist_guarded_publish_report(report_path, report)
    if not chooserole_gate["passed"]:
        report["status"] = "Blocked"
        report["blockReason"] = chooserole_gate["blockReason"]
        persist_guarded_publish_report(report_path, report)
        return report_path

    if not differences and mode == "Routine":
        report["status"] = "NoOp"
        persist_guarded_publish_report(report_path, report)
        return report_path

    protected_names = [entry["Name"] for entry in manifest.get("ProtectedFiles", [])]
    difference_names = [entry["name"] for entry in differences]

    if dry_run:
        report["status"] = "DryRun"
        report["plannedCanaryFiles"] = dedupe([*protected_names, *difference_names, "Version.txt"])
        report["plannedRolloutFiles"] = dedupe([*(protected_names if mode == "LoginStack" else []), *difference_names, "Version.txt"])
        persist_guarded_publish_report(report_path, report)
        return report_path

    snapshot = None
    files_added_by_publish: list[str] = []
    if differences:
        tracked = dedupe([*difference_names, *protected_names, "Version.txt"])
        snapshot = new_publish_snapshot(target_dir, tracked, run_dir / "snapshot")
        files_added_by_publish = [entry["name"] for entry in snapshot["trackedFiles"] if not entry["existedBefore"]]
        report["snapshotDir"] = str(run_dir / "snapshot")
        persist_guarded_publish_report(report_path, report)

    canary_passed = False
    try:
        if differences:
            report["copiedToLiveTarget"] = copy_named_files(
                resolved_source,
                target_dir,
                difference_names,
                timestamp=time.strftime("%Y%m%d-%H%M%S"),
            )
            report["targetVersion"] = write_version_file_for_directory(target_dir)
        else:
            report["targetVersion"] = {"versionPath": str(target_dir / "Version.txt"), "verificationOnly": True}

        canary_files = dedupe([*protected_names, *difference_names, "Version.txt"])
        report["canarySyncReport"] = str(
            sync_hotfix_to_devices(
                profile_name,
                source_dir=str(target_dir),
                devices=[resolved_canary_device],
                files_to_sync=canary_files,
            )
        )
        report["canaryLoginState"] = prepare_canary_login_state(manifest, resolved_canary_device)
        report["canarySmoke"] = start_canary_smoke_test(manifest, resolved_canary_device, run_dir, skip_visual=skip_canary_smoke)
        persist_guarded_publish_report(report_path, report)
        if not report["canarySmoke"]["Passed"]:
            raise RuntimeError("Canary login smoke failed")

        canary_passed = True
        if mode == "LoginStack":
            update_manifest_baseline(resolved_manifest_path, manifest, resolved_source, target_dir)
            report["manifestUpdated"] = True
        else:
            report["manifestUpdated"] = False

        rollout_files = dedupe([*(protected_names if mode == "LoginStack" else []), *difference_names, "Version.txt"])
        mirror_updates: list[dict[str, Any]] = []
        for mirror_dir in mirror_dirs:
            assert_exists(mirror_dir, "Mirror target directory")
            copied = copy_named_files(target_dir, mirror_dir, rollout_files, timestamp=time.strftime("%Y%m%d-%H%M%S"))
            mirror_updates.append({"targetDir": str(mirror_dir), "copied": copied})
        report["mirrorUpdates"] = mirror_updates

        remaining_devices = [device for device in current_devices if device != resolved_canary_device]
        if remaining_devices:
            report["rolloutDevices"] = remaining_devices
            report["rolloutFiles"] = rollout_files
            report["rolloutSyncReport"] = str(
                sync_hotfix_to_devices(
                    profile_name,
                    source_dir=str(target_dir),
                    devices=remaining_devices,
                    files_to_sync=rollout_files,
                )
            )
        else:
            report["rolloutDevices"] = []
            report["rolloutFiles"] = []

        report["status"] = "Success"
        persist_guarded_publish_report(report_path, report)
        return report_path
    except Exception as exc:
        report["error"] = str(exc)
        if not canary_passed:
            if snapshot is not None:
                restore_publish_snapshot(snapshot)
            if files_added_by_publish:
                remove_remote_hotfix_files(manifest["Canary"]["PackageName"], resolved_canary_device, files_added_by_publish)
            rollback_files = dedupe([*protected_names, *difference_names, "Version.txt"])
            report["rollbackSyncReport"] = str(
                sync_hotfix_to_devices(
                    profile_name,
                    source_dir=str(target_dir),
                    devices=[resolved_canary_device],
                    files_to_sync=rollback_files,
                )
            )
            report["rollbackLoginState"] = prepare_canary_login_state(manifest, resolved_canary_device)
            report["rollback"] = {
                "restoredTarget": snapshot is not None,
                "resyncedCanary": True,
                "rollbackFiles": rollback_files,
            }
        else:
            report["rollback"] = {
                "restoredTarget": False,
                "resyncedCanary": False,
                "reason": "Canary already passed; keeping approved live target",
            }
        report["status"] = "Failed"
        persist_guarded_publish_report(report_path, report)
        raise
