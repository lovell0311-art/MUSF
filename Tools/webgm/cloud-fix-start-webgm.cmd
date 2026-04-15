@echo off
chcp 65001 >nul 2>&1
set "PORT=5000"
set "WEBGM_DIR=C:\MUSF\Tools\webgm"
set "TASK_NAME=MUSF WebGM"

echo ========================================
echo   WebGM Cloud - Fix and Start
echo ========================================
echo.

:: Step 1: 停止计划任务
echo [1/5] Stopping scheduled task ...
schtasks /End /TN "%TASK_NAME%" >nul 2>&1

:: Step 2: 杀掉所有 webgm 相关 node 进程
echo [2/5] Killing old processes ...
for /f "tokens=2" %%p in ('powershell -NoProfile -Command "Get-CimInstance Win32_Process -Filter \"Name='node.exe'\" | Where-Object { $_.CommandLine -like '*webgm*' } | ForEach-Object { $_.ProcessId }"') do (
    echo       Killing PID %%p
    taskkill /PID %%p /F >nul 2>&1
)

:: 兜底: 按端口杀进程
for /f %%i in ('powershell -NoProfile -Command "$c = Get-NetTCPConnection -LocalPort %PORT% -State Listen -ErrorAction SilentlyContinue; if ($c) { $c.OwningProcess }"') do (
    echo       Killing port %PORT% holder PID %%i
    taskkill /PID %%i /F >nul 2>&1
)

:: Step 3: 等待端口释放
echo [3/5] Waiting for port %PORT% to free ...
set "STILL_USED="
for /L %%s in (1,1,10) do (
    timeout /t 1 /nobreak >nul
    set "STILL_USED="
    for /f %%i in ('powershell -NoProfile -Command "$c = Get-NetTCPConnection -LocalPort %PORT% -State Listen -ErrorAction SilentlyContinue; if ($c) { $c.OwningProcess }"') do set "STILL_USED=%%i"
    if not defined STILL_USED goto :port_free
)
echo [ERROR] Port %PORT% still occupied after 10s. Aborting.
pause
exit /b 1
:port_free
echo       Port %PORT% is free.

:: Step 4: 防火墙确认
echo [4/5] Ensuring firewall rule ...
powershell -NoProfile -Command "$r = Get-NetFirewallRule -DisplayName 'MUSF TCP %PORT%' -ErrorAction SilentlyContinue; if (-not $r) { New-NetFirewallRule -DisplayName 'MUSF TCP %PORT%' -Direction Inbound -Action Allow -Protocol TCP -LocalPort %PORT% | Out-Null; Write-Host 'Rule created.' } else { Write-Host 'Rule exists.' }"

:: Step 5: 启动
echo [5/5] Starting WebGM ...
cd /d "%WEBGM_DIR%"
set "PORT=5000"
start "WebGM-Server" cmd /c "cd /d %WEBGM_DIR% && set PORT=5000 && node server.js >> webgm_stdout.log 2>> webgm_stderr.log"

:: 等待就绪
set "READY="
for /L %%s in (1,1,15) do (
    if not defined READY (
        timeout /t 1 /nobreak >nul
        powershell -NoProfile -Command "try { $r = Invoke-WebRequest -UseBasicParsing -Uri 'http://127.0.0.1:%PORT%/' -TimeoutSec 2; if ($r.StatusCode -eq 200) { exit 0 } } catch {}" >nul 2>&1 && set "READY=1"
    )
)

echo.
if defined READY (
    echo ========================================
    echo   [OK] WebGM started successfully!
    echo   URL: http://127.0.0.1:%PORT%/
    echo ========================================
) else (
    echo ========================================
    echo   [WARN] Service did not respond in 15s
    echo   Check logs at: %WEBGM_DIR%\webgm_stderr.log
    echo ========================================
)
echo.
pause
