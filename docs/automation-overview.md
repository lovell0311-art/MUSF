# MUSF Automation Overview

## Formal Root

- Formal workspace root: `F:\MUSF`
- Project-owned tools, reports, snapshots, archives, and skill sources live on `F:`
- `C:` is only used for existing system installations or lightweight compatibility links

## Formal Entrypoints

- Python bootstrap: `scripts\bootstrap-python.cmd`
- Node 18 bootstrap: `scripts\bootstrap-node18.cmd`
- Combined bootstrap: `scripts\bootstrap-toolchain.cmd`
- Main command entrypoint: `scripts\run-musf.cmd`

## Main Commands

- `scripts\run-musf.cmd generate-baseline`
- `scripts\run-musf.cmd classify-legacy-scripts`
- `scripts\run-musf.cmd verify-toolchain`
- `scripts\run-musf.cmd sync-network-config`
- `scripts\run-musf.cmd server-health`
- `scripts\run-musf.cmd server-start`
- `scripts\run-musf.cmd server-stop`
- `scripts\run-musf.cmd build-android`
- `scripts\run-musf.cmd run-gate`
- `scripts\run-musf.cmd rollback`

## Generated Outputs

- UI baseline manifest: `F:\MUSF\apk-ui-baseline-manifest.json`
- Toolchain status: `F:\MUSF\reports\toolchain\toolchain-status.json`
- Script inventory: `F:\MUSF\archive\inventory\script-classification.md`
- Release and health reports: `F:\MUSF\reports\releases\`

## Environment Policy

- Default profile: `env-profiles\local-lan.json`
- Client-facing addresses must stay on the local/LAN profile unless an explicit profile switch is requested
- Frozen UI baseline comes from `F:\MUSF\20260310.apk` and `F:\MUSF\_apktool_20260310\assets`

## Skills

- Formal skill sources: `F:\MUSF\skills\`
- Codex discovery uses lightweight junctions from `C:\Users\ZM\.codex\skills\` to the `F:` skill folders
