@echo off
setlocal
call "%~dp0..\run-musf.cmd" sync-hotfix-to-devices %*
exit /b %errorlevel%
