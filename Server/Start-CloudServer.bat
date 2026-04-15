@echo off
setlocal

if "%~1"=="" (
    echo Usage: %~nx0 PUBLIC_HOST [GM_HTTP_PREFIX]
    echo Example: %~nx0 203.0.113.10 http://127.0.0.1:65001/
    exit /b 1
)

set "PUBLIC_HOST=%~1"
set "GM_HTTP_PREFIX=%~2"
if "%GM_HTTP_PREFIX%"=="" set "GM_HTTP_PREFIX=http://127.0.0.1:65001/"

call "%~dp0scripts\Start-CloudServerNative.cmd" "%PUBLIC_HOST%" "%GM_HTTP_PREFIX%"
exit /b %errorlevel%
