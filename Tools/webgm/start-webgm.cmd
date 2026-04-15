@echo off
:: WebGM startup for cloud server (scheduled task)
:: Anti-duplicate: skip if already running on target port

set "PORT=5000"
set "WEBGM_DIR=C:\MUSF\Tools\webgm"
cd /d "%WEBGM_DIR%"

:: Check if port already in use by a node process
for /f %%i in ('powershell -NoProfile -Command "$c = Get-NetTCPConnection -LocalPort %PORT% -State Listen -ErrorAction SilentlyContinue; if ($c) { $c.OwningProcess }"') do (
    echo [%date% %time%] Port %PORT% already in use by PID %%i, skipping start. >> "%WEBGM_DIR%\webgm_stdout.log"
    exit /b 0
)

echo [%date% %time%] Starting WebGM on port %PORT% ... >> "%WEBGM_DIR%\webgm_stdout.log"
set "PORT=5000"
"C:\Program Files\nodejs\node.exe" server.js >> webgm_stdout.log 2>> webgm_stderr.log
