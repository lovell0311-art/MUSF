---
name: musf-server-ops
description: Operate the MUSF local/LAN server stack. Use when starting, stopping, checking, or recovering MongoDB, Realm, Gate, Game, FileServer, GM, or nginx; when syncing server addresses to the local LAN profile; and when validating that the client should connect only to the local/LAN environment instead of public or emulator loopback addresses.
---

# MUSF Server Ops

Use the formal environment profile in `F:\MUSF\env-profiles\local-lan.json` and keep server operations on the local/LAN stack.

## Guardrails

- Default profile is `local-lan`
- Do not switch the stack to public internet, `127.0.0.1`, or `10.0.2.2` for client-facing addresses unless the selected profile explicitly requires it
- Prefer the formal Python entrypoints under `scripts\run-musf.cmd`
- Treat `F:\MUSF\Server\运行.bat` as a legacy reference, not the formal control plane

## Workflow

1. Sync network-facing addresses from the selected env profile.
2. Start or stop services through the formal MUSF scripts.
3. Validate health for MongoDB, Realm, Gate ports, FileServer, and GM web.
4. Write reports to `F:\MUSF\reports\releases\`.
5. If the stack is unhealthy, report the failing component and the exact endpoint or port.

## Key Paths

- Env profile: `F:\MUSF\env-profiles\local-lan.json`
- Server config: `F:\MUSF\Server\Config\StartUpConfig\StartUp_ServerConfig.json`
- Client config: `F:\MUSF\Client\Unity\Assets\Res\Config\GlobalProto.txt`
- FileServer version check: `http://10.10.10.192:8080/Android/StreamingAssets/Version.txt`

## Reporting

- Always state the active profile, resolved LAN IP, and the report path.
- When health fails, separate toolchain issues from service runtime issues.
