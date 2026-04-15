@echo off
setlocal
call "%~dp0run-musf.cmd" sync-network-config %*
exit /b %errorlevel%
