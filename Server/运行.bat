@echo off

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "C:\Users\ZM\Documents\New project\ensure_musf_network_config.ps1" -Quiet
if errorlevel 1 (
    echo [ERROR] Failed to harden network config before startup.
    exit /b 1
)

cd /d "%~dp0Bin"

if exist .\temp\ rd .\temp\ /s/q



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
echo finish...
