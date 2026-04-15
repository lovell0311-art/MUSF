# MUSF Cloud Deployment

## Scope

This repository's current server build is prepared for a single Windows cloud server.

Reasons:

- `App.exe` imports `winmm` from Windows.
- The server bundle ships `kcp.dll` and `libkcp.dylib`, but no Linux server-side `.so`.
- The checked-in startup flow is Windows batch based.

## Required Runtime

- .NET Core Runtime 3.1 for `Bin\App.exe`
- ASP.NET Core Runtime / Hosting Bundle 2.1 for `FileServer\FileServer.dll`
- MongoDB 4.0 compatible instance if you want to restore the current local data

## Local Dependencies Found

- MongoDB service: `MongoDB_20230615`
- MongoDB listen port: `58030`
- MongoDB config: `F:\MUSF\Tools\MongoDB\MongoDB\MongoDB\Server\4.0\bin\20230615.conf`
- MongoDB data path: `F:\MUSF\MongoDB\MongoDBData`
- MongoDB log path: `F:\MUSF\MongoDB\MongoDBDataLog\test.log`

Current game server config points all DB connections to:

- `mongodb://127.0.0.1:58030`

The MySQL helper code exists, but the current game startup path does not enable it by default.

## Files To Deploy

Copy these directories and files to the cloud server while preserving relative paths:

- `Bin`
- `Config`
- `FileServer`
- `Release`
- `scripts`
- `Start-CloudServer.bat`
- `Stop-CloudServer.bat`

## Start Order

1. Install the required runtimes on the Windows cloud server.
2. Restore Mongo data to the cloud MongoDB instance, or point `Config\StartUpConfig\Server_DataConfig.json` to an existing MongoDB instance.
3. Run:

```bat
Start-CloudServer.bat YOUR_PUBLIC_IP
```

This wrapper will:

- rewrite `Config\StartUpConfig\StartUp_ServerConfig.json` for a single-host cloud layout
- keep all inner services on `127.0.0.1`
- bind realm and gates to `0.0.0.0`
- set public gateway addresses to `YOUR_PUBLIC_IP`
- start `FileServer.dll`
- start the current app set from `运行.bat`

## Stop

```bat
Stop-CloudServer.bat
```

## External Ports

Open these ports in the cloud security group / firewall:

- UDP `10002`
- UDP `10004-10008`
- TCP `8080`
- TCP `8088`

Optional:

- TCP `65001` if you want the GM HTTP service reachable from outside
- TCP `58030` only if MongoDB must be reachable remotely

## Mongo Backup

Create a portable Mongo archive from the local machine:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Export-MongoBackup.ps1
```

That script writes a gzip archive into:

- `Deploy\mongo-YYYYMMDD-HHMMSS.archive.gz`

## Notes

- The original local `运行.bat` is left unchanged because it depends on a workstation-specific script path.
- The cloud startup path created here is isolated and safe to use on the server.
