---
name: musf-baseline-guard
description: Protect the MUSF 20260310.apk UI and layout baseline. Use when editing or publishing client UI bundles, checking `ui*.unity3d`, `uimaincanvas.unity3d`, `ui_hud.unity3d`, `ui_mainpanels.unity3d`, `ui_skills.unity3d`, or `minmap.unity3d`, and when a change must be blocked unless the frozen APK baseline still matches.
---

# MUSF Baseline Guard

Use the APK baseline at `F:\MUSF\20260310.apk` and the extracted baseline assets at `F:\MUSF\_apktool_20260310\assets` as the only visual source of truth.

## Guardrails

- Treat the following bundles as frozen unless the user explicitly approves a UI change:
  - `uimaincanvas.unity3d`
  - `ui_hud.unity3d`
  - `ui_mainpanels.unity3d`
  - `ui_skills.unity3d`
  - `minmap.unity3d`
- Treat `破军之刃 / 20260310.apk` as the only accepted UI family. Do not accept `命运王座` screenshots, references, or replacement bundles as valid baseline input.
- Reject any publish flow that changes a frozen bundle without a matching approved baseline update.
- Prefer `F:\MUSF\apk-ui-baseline-manifest.json` and `F:\MUSF\reports\toolchain\toolchain-status.json` over ad hoc file comparison.

## Workflow

1. Regenerate the baseline manifest with `scripts\run-musf.cmd generate-baseline` if the manifest is missing or stale.
2. Compare candidate bundle hashes and sizes against `apk-ui-baseline-manifest.json`.
3. If only gameplay logic changed, confirm that every frozen UI bundle still matches the manifest.
4. If any frozen bundle differs, stop and report the exact file names, sizes, and hashes instead of publishing.
5. Only after explicit approval should a new baseline be generated from a replacement APK.

## Reporting

- Report drift with file name, expected hash, actual hash, and whether the bundle is frozen.
- Say explicitly when the publish path is blocked by baseline drift.
- When no drift exists, state that the UI baseline still matches `20260310.apk`.
