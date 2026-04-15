@echo off
setlocal
call "%~dp0..\run-musf.cmd" run-gate %*
exit /b %errorlevel%
