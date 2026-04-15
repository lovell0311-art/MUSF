@echo off
setlocal
call "%~dp0run-musf.cmd" server-health %*
exit /b %errorlevel%
