from __future__ import annotations

import json

from .common import parse_args
from .choose_role_calibration import choose_role_calibrate
from .phase0 import classify_legacy_scripts, generate_baseline_manifest, sync_network_config, verify_toolchain
from .phase1 import build_android, rollback, run_gate, server_health, server_start, server_stop
from .publish import (
    generate_version,
    publish_code_bundle,
    publish_hotfix_guarded,
    report_chooserole_variants,
    sync_hotfix_to_devices,
)

COMMAND_TOOLCHAIN_REQUIREMENTS: dict[str, set[str]] = {
    "build-android": {"unity-editor", "android-sdk", "adb", "jdk8", "python311"},
    "run-gate": {"adb", "python311"},
    "publish-code-bundle": {"python311"},
    "sync-hotfix-to-devices": {"adb", "python311"},
    "publish-hotfix-guarded": {"adb", "python311", "bundlepatch"},
}


def enforce_toolchain_preflight(command: str) -> None:
    required_tools = COMMAND_TOOLCHAIN_REQUIREMENTS.get(command)
    if not required_tools:
        return

    report_path = verify_toolchain()
    report = json.loads(report_path.read_text(encoding="utf-8"))
    missing_required = [
        item["name"]
        for item in report["items"]
        if item["name"] in required_tools and item["status"] == "missing"
    ]
    if missing_required:
        names = ", ".join(sorted(missing_required))
        raise RuntimeError(f"{command} blocked by toolchain preflight: missing required tools: {names}. See {report_path}")


def main(argv: list[str] | None = None) -> int:
    args = parse_args(argv)
    enforce_toolchain_preflight(args.command)

    if args.command == "verify-toolchain":
        print(verify_toolchain())
        return 0
    if args.command == "generate-baseline":
        print(generate_baseline_manifest())
        return 0
    if args.command == "classify-legacy-scripts":
        print(classify_legacy_scripts())
        return 0
    if args.command == "sync-network-config":
        changed = sync_network_config(args.profile)
        for path in changed:
            print(path)
        return 0
    if args.command == "choose-role-calibrate":
        print(
            choose_role_calibrate(
                args.image,
                args.point,
                plane_y=args.plane_y,
                annotated_out=args.annotated_out,
                json_out=args.json_out,
            )
        )
        return 0
    if args.command == "server-health":
        print(server_health(args.profile))
        return 0
    if args.command == "server-start":
        print(server_start(args.profile, clean=args.clean))
        return 0
    if args.command == "server-stop":
        print(server_stop(args.profile, all_managed=args.all_managed))
        return 0
    if args.command == "build-android":
        print(build_android(args.profile))
        return 0
    if args.command == "run-gate":
        print(run_gate(args.profile))
        return 0
    if args.command == "rollback":
        print(rollback(args.profile, args.source))
        return 0
    if args.command == "generate-version":
        print(generate_version(args.profile, target_dir=args.target_dir or None))
        return 0
    if args.command == "report-chooserole-variants":
        print(report_chooserole_variants(args.profile, source_dir=args.source_dir or None))
        return 0
    if args.command == "publish-code-bundle":
        print(
            publish_code_bundle(
                args.profile,
                source_dir=args.source_dir or None,
                target_dirs=args.target_dir or None,
                dry_run=args.dry_run,
            )
        )
        return 0
    if args.command == "sync-hotfix-to-devices":
        print(
            sync_hotfix_to_devices(
                args.profile,
                source_dir=args.source_dir or None,
                package_name=args.package_name or None,
                remote_dir=args.remote_dir,
                devices=args.device or None,
                files_to_sync=args.file or None,
                launch_after_sync=args.launch_after_sync,
                dry_run=args.dry_run,
            )
        )
        return 0
    if args.command == "publish-hotfix-guarded":
        print(
            publish_hotfix_guarded(
                args.profile,
                mode=args.mode,
                canary_device=args.canary_device,
                manifest_path=args.manifest_path,
                source_dir=args.source_dir or None,
                dry_run=args.dry_run,
                skip_canary_smoke=args.skip_canary_smoke,
            )
        )
        return 0

    raise ValueError(f"Unknown command: {args.command}")
