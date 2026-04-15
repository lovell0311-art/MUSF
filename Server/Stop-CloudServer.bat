@echo off
setlocal

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\Stop-CloudServer.ps1" -ServerRoot "%~dp0"
exit /b %errorlevel%
