@echo off
chcp 65001 >nul 2>&1
cd /d %~dp0
set "PORT=5000"

echo ========================================
echo   WebGM 一键启动工具
echo ========================================
echo.

:: 检查端口占用
set "EXISTING_PID="
for /f %%i in ('powershell -NoProfile -Command "$conn = Get-NetTCPConnection -LocalPort %PORT% -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1; if ($conn) { $conn.OwningProcess }"') do set "EXISTING_PID=%%i"

if defined EXISTING_PID (
    echo [!] 端口 %PORT% 已被占用 (PID: %EXISTING_PID%)
    echo     如果是旧的 WebGM 进程，选择 R 重启
    echo     如果要复用现有服务，选择 O 打开浏览器
    echo.
    choice /C ROQ /N /M "[R]重启  [O]打开浏览器  [Q]退出: "
    if errorlevel 3 exit /b 0
    if errorlevel 2 (
        start "" http://127.0.0.1:%PORT%/
        exit /b 0
    )
    echo [*] 正在终止旧进程 PID %EXISTING_PID% ...
    taskkill /PID %EXISTING_PID% /F >nul 2>&1
    timeout /t 2 /nobreak >nul
)

:: 启动服务
echo [*] 正在启动 WebGM 服务 (端口 %PORT%) ...
start "WebGM-Server" cmd /k "cd /d %~dp0 && set PORT=%PORT% && node server.js"

:: 等待服务就绪
set "READY="
for /L %%s in (1,1,15) do (
    if not defined READY (
        timeout /t 1 /nobreak >nul
        powershell -NoProfile -Command "try { $r = Invoke-WebRequest -UseBasicParsing -Uri 'http://127.0.0.1:%PORT%/' -TimeoutSec 2; if ($r.StatusCode -eq 200) { exit 0 } } catch {}" >nul 2>&1 && set "READY=1"
    )
)

if defined READY (
    echo [OK] WebGM 启动成功！
) else (
    echo [!!] 等待超时，请手动检查: http://127.0.0.1:%PORT%/
)

start "" http://127.0.0.1:%PORT%/
echo.
echo 提示: 关闭此窗口不会影响 WebGM 服务
echo       要停止服务请关闭标题为 "WebGM-Server" 的窗口
pause
