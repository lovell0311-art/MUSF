@echo off
cd /d %~dp0
set "PORT=5000"
set "EXISTING_PID="
for /f %%i in ('powershell -NoProfile -Command "$conn = Get-NetTCPConnection -LocalPort %PORT% -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1; if ($conn) { $conn.OwningProcess }"') do set "EXISTING_PID=%%i"
if defined EXISTING_PID (
    echo Port %PORT% already in use by PID %EXISTING_PID%.
    echo Reusing existing server and starting client only.
    call npm run client
) else (
    call npm run dev
)

pause
