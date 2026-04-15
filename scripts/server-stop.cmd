@echo off
setlocal
call "%~dp0run-musf.cmd" server-stop %*
exit /b %errorlevel%
