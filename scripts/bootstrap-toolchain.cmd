@echo off
setlocal

call "%~dp0bootstrap-python.cmd"
if errorlevel 1 exit /b 1

call "%~dp0bootstrap-node18.cmd"
if errorlevel 1 exit /b 1

echo Toolchain bootstrap complete.
exit /b 0
