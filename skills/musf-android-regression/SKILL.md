---
name: musf-android-regression
description: Run MUSF Android regression on MuMu or ADB-connected devices. Use when installing or updating the APK, backing up device state, validating login for `blaike / 123456`, capturing screenshots, checking logcat, verifying main HUD and panels, or confirming that maps and minimaps still align after a hotfix.
---

# MUSF Android Regression

Use ADB through the formal MUSF toolchain and keep all logs, screenshots, and backups under `F:\MUSF`.

## Guardrails

- Default package name: `com.quanzhilieshou.bytedance.gamecenter`
- Default account: `blaike`
- Default password: `123456`
- Default connection target: local/LAN server only
- Default UI family: `破军之刃`; do not treat `命运王座` login or panel screenshots as acceptable regression targets
- Never replace or modify frozen UI bundles as part of regression work
- Before installing or syncing a new build, back up the current APK path, device cache, and hot-update directory

## Workflow

1. Detect the target device with ADB and record the serial.
2. Back up the currently installed package path and `/sdcard/Android/data/<package>/files/Android`.
3. Install or update the candidate build.
4. Launch the game, log in with the default account, and capture screenshots for:
   - login page
   - main HUD
   - bag panel
   - skills panel
   - target maps under test
5. Collect logcat and fail fast on crash, fatal exception, or missing UI flow.
6. Save artifacts under `F:\MUSF\reports\releases\` or `F:\MUSF\backups\`.

## Required Checks

- APK installed
- login succeeds
- main HUD visible
- bag visible
- skills visible
- no fatal crash in logcat
- map/minimap screenshots captured for requested scenes

## Reporting

- Report the device serial, package version path, screenshot paths, and log path.
- If regression fails, say whether the correct next action is rollback or continue fixing.
