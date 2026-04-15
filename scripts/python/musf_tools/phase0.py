from __future__ import annotations

import time
from pathlib import Path
from typing import Any

from .common import (
    assert_primary_client_path,
    env_profile,
    file_md5,
    iter_script_files,
    musf_root,
    read_json,
    replace_global_proto_content,
    toolchain_status_entries,
    write_json,
)


EXPLICIT_BASELINE_NAMES = {
    "uimaincanvas.unity3d": "Main HUD canvas baseline",
    "ui_hud.unity3d": "HUD sub-layout baseline",
    "ui_mainpanels.unity3d": "Primary gameplay panel baseline",
    "ui_skills.unity3d": "Skills panel baseline",
    "minmap.unity3d": "Minimap baseline",
}


def verify_toolchain() -> Path:
    root = musf_root()
    output = root / "reports" / "toolchain" / "toolchain-status.json"
    write_json(
        output,
        {
            "schemaVersion": 1,
            "generatedAt": time.strftime("%Y-%m-%dT%H:%M:%S%z"),
            "root": str(root),
            "items": toolchain_status_entries(),
        },
    )
    return output


def baseline_purpose(name: str) -> str:
    if name in EXPLICIT_BASELINE_NAMES:
        return EXPLICIT_BASELINE_NAMES[name]
    lowered = name.lower()
    if lowered.startswith("uilogin"):
        return "Login UI baseline"
    if lowered.startswith("uiselectserver"):
        return "Server select baseline"
    if lowered.startswith("uichooserole"):
        return "Role select baseline"
    if lowered.startswith("minmap"):
        return "Minimap baseline"
    return "Frozen APK UI baseline"


def generate_baseline_manifest() -> Path:
    root = musf_root()
    asset_root = root / "_apktool_20260310" / "assets"
    entries: list[dict[str, Any]] = []
    for item in sorted(asset_root.iterdir(), key=lambda path: path.name.lower()):
        if not item.is_file():
            continue
        name = item.name
        if "backup-codebundle-" in str(item):
            continue
        if not (
            name.startswith("ui")
            and name.endswith(".unity3d")
            or name.startswith("minmap")
            and name.endswith(".unity3d")
            or name in EXPLICIT_BASELINE_NAMES
        ):
            continue
        entries.append(
            {
                "filename": name,
                "size": item.stat().st_size,
                "md5": file_md5(item),
                "purpose": baseline_purpose(name),
                "frozen": True,
                "sourcePath": str(item),
            }
        )

    output = root / "apk-ui-baseline-manifest.json"
    write_json(
        output,
        {
            "schemaVersion": 1,
            "generatedAt": time.strftime("%Y-%m-%dT%H:%M:%S%z"),
            "sourceRoot": str(asset_root),
            "fileCount": len(entries),
            "entries": entries,
        },
    )
    return output


def classify_legacy_scripts() -> Path:
    root = musf_root()
    new_project_root = Path(r"C:\Users\ZM\Documents\New project")
    sources = [path for path in [new_project_root, root / "Client" / "Unity", root / "Server", root / "Tools"] if path.exists()]

    exact_migration_map = {
        "publish_code_bundle.ps1": r"F:\MUSF\scripts\build\publish-code-bundle.cmd",
        "publish_hotfix_guarded.ps1": r"F:\MUSF\scripts\build\publish-hotfix-guarded.cmd",
        "generate_version_txt.ps1": r"F:\MUSF\scripts\build\generate-version.cmd",
        "compare_musf_ui_assets.ps1": r"F:\MUSF\scripts\python\musf_tools\phase0.py:generate_baseline_manifest",
        "sync_musf_hotfix_to_devices.ps1": r"F:\MUSF\scripts\deploy\sync-hotfix-to-devices.cmd",
        "start_webgm_background.ps1": r"F:\MUSF\scripts\deploy\start-webgm-background.py",
        "build_android_loginfix.bat": r"F:\MUSF\scripts\python\musf_tools\phase1.py:build_android",
        "运行.bat": r"F:\MUSF\scripts\python\musf_tools\phase1.py:server_start",
    }

    rows: list[dict[str, str]] = []
    for source in sources:
        for path in iter_script_files(source):
            name = path.name
            if name in exact_migration_map:
                category = "preserve-migrate"
                target = exact_migration_map[name]
                allowed = "yes"
                notes = "Formal entrypoint or direct reference source."
            elif name.startswith("build_musf_test_ui_clone_") and name.endswith(".ps1"):
                category = "merge-rewrite"
                target = r"F:\MUSF\scripts\build\build-ui-clone-variants.py"
                allowed = "no"
                notes = "Historical variants should collapse into one parameterized entrypoint."
            elif "New project" in str(path) or ("ensure" in name.lower() and "network" in name.lower()):
                category = "archive-isolate"
                target = "F:\\MUSF\\archive\\new-project-src\\"
                allowed = "no"
                notes = "Temporary or side-effect-heavy script. Do not call directly from the formal flow."
            else:
                category = "archive-isolate"
                target = "F:\\MUSF\\archive\\inventory\\"
                allowed = "no"
                notes = "Needs manual review before promotion."

            rows.append(
                {
                    "SourcePath": str(path),
                    "Category": category,
                    "ProposedTarget": target,
                    "AllowedNow": allowed,
                    "Notes": notes,
                }
            )

    rows.sort(key=lambda item: item["SourcePath"].lower())
    lines = [
        "# Script Classification",
        "",
        f"- GeneratedAt: {time.strftime('%Y-%m-%d %H:%M:%S')}",
        f"- MUSFRoot: {root}",
        f"- NewProjectRoot: {new_project_root}",
        "",
        "| SourcePath | Category | ProposedTarget | AllowedNow | Notes |",
        "| --- | --- | --- | --- | --- |",
    ]
    for row in rows:
        escaped = [row[key].replace("|", "\\|") for key in ["SourcePath", "Category", "ProposedTarget", "AllowedNow", "Notes"]]
        lines.append("| {} | {} | {} | {} | {} |".format(*escaped))

    output = root / "archive" / "inventory" / "script-classification.md"
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text("\n".join(lines) + "\n", encoding="utf-8")
    return output


def sync_network_config(profile_name: str) -> list[Path]:
    root = musf_root()
    profile = env_profile(profile_name)
    server_config_path = root / "Server" / "Config" / "StartUpConfig" / "StartUp_ServerConfig.json"
    server_config = read_json(server_config_path)

    managed_port_map = {
        1: profile["ports"]["realm"],
        2: profile["ports"]["realm"],
        20004: 10004,
        20005: 10005,
        20006: 10006,
        20007: 10007,
        20008: 10008,
    }

    for entry in server_config:
        if entry["AppId"] in managed_port_map:
            outer = entry.setdefault("OuterConfig", {})
            outer["Address2"] = f'{profile["serverIp"]}:{managed_port_map[entry["AppId"]]}'

    write_json(server_config_path, server_config)

    global_proto_path = root / "Client" / "Unity" / "Assets" / "Res" / "Config" / "GlobalProto.txt"
    assert_primary_client_path(global_proto_path, "GlobalProto config path")
    content = global_proto_path.read_text(encoding="utf-8-sig")
    content = replace_global_proto_content(content, profile)
    global_proto_path.write_text(content, encoding="utf-8")

    return [server_config_path, global_proto_path]
