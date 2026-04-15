@echo off
setlocal
call "%~dp0..\run-musf.cmd" rollback %*
exit /b %errorlevel%
