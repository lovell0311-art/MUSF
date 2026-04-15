@echo off
setlocal
call "%~dp0run-musf.cmd" server-start %*
exit /b %errorlevel%
