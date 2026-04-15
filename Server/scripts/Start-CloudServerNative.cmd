@echo off
setlocal

set "SERVER_ROOT=%~dp0.."
for %%I in ("%SERVER_ROOT%") do set "SERVER_ROOT=%%~fI"

set "PUBLIC_HOST=%~1"
if "%PUBLIC_HOST%"=="" set "PUBLIC_HOST=82.157.103.202"

set "GM_HTTP_PREFIX=%~2"
if "%GM_HTTP_PREFIX%"=="" set "GM_HTTP_PREFIX=http://127.0.0.1:65001/"

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Prepare-CloudConfig.ps1" -ConfigRoot "%SERVER_ROOT%\Config" -PublicHost "%PUBLIC_HOST%" -GatewayListenHost "0.0.0.0" -InnerListenHost "127.0.0.1" -GmHttpPrefix "%GM_HTTP_PREFIX%"
if errorlevel 1 exit /b 1

cd /d "%SERVER_ROOT%\Bin"

if exist .\temp rd /s /q .\temp >nul 2>nul

start "Realm 2" App.exe --AppId=2 --AppType=Realm --ConfigPath=../Config --LogLevel=Info
start "Gate 20004" App.exe --AppId=20004 --AppType=Gate --ConfigPath=../Config --LogLevel=Info
start "Gate 20005" App.exe --AppId=20005 --AppType=Gate --ConfigPath=../Config --LogLevel=Info
start "Gate 20006" App.exe --AppId=20006 --AppType=Gate --ConfigPath=../Config --LogLevel=Info
start "Gate 20007" App.exe --AppId=20007 --AppType=Gate --ConfigPath=../Config --LogLevel=Info
start "Gate 20008" App.exe --AppId=20008 --AppType=Gate --ConfigPath=../Config --LogLevel=Info
start "GM 99" App.exe --AppId=99 --AppType=GM --ConfigPath=../Config --LogLevel=Info
start "DB 10" App.exe --AppId=10 --AppType=DB --ConfigPath=../Config --LogLevel=Info
start "Match 11" App.exe --AppId=11 --AppType=Match --ConfigPath=../Config --LogLevel=Info
start "LoginCenter 25" App.exe --AppId=25 --AppType=LoginCenter --ConfigPath=../Config --LogLevel=Info
start "DB 21" App.exe --AppId=21 --AppType=DB --ConfigPath=../Config --LogLevel=Info
start "DB 278" App.exe --AppId=278 --AppType=DB --ConfigPath=../Config --LogLevel=Info
start "DB 279" App.exe --AppId=279 --AppType=DB --ConfigPath=../Config --LogLevel=Info
start "MGMT 280" App.exe --AppId=280 --AppType=MGMT --ConfigPath=../Config --LogLevel=Info
start "Game 257" App.exe --AppId=257 --AppType=Game --ConfigPath=../Config --LogLevel=Info
start "Game 259" App.exe --AppId=259 --AppType=Game --ConfigPath=../Config --LogLevel=Info
start "Game 262" App.exe --AppId=262 --AppType=Game --ConfigPath=../Config --LogLevel=Info
start "StaticFileServer" powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Start-StaticFileServer.ps1" -RootPath "%SERVER_ROOT%\Release"

echo finish...
exit /b 0
