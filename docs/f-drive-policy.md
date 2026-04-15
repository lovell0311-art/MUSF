# F Drive Policy

## Rule

All new MUSF project-owned artifacts must be created on `F:` by default.

## Applies To

- toolchains
- snapshots
- reports
- archives
- build outputs
- backups
- skill source folders
- external helper repositories

## Exceptions

- Existing system installations already located on `C:` may be used as transitional fallbacks
- Lightweight compatibility links under `C:\Users\ZM\.codex\skills\` are allowed when the actual skill content remains on `F:`

## Current Transitional Fallbacks

- Unity Editor
- Android SDK / ADB
- JDK 8
- JDK 17
- dotnet runtime

These are still detected from `C:` today, but the formal manifest and future migration target remain `F:\MUSF\toolchain\`.
