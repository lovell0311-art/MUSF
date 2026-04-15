@echo off
setlocal

set "MUSF_ROOT=%~dp0.."
for %%I in ("%MUSF_ROOT%") do set "MUSF_ROOT=%%~fI"
set "PYTHON_EXE=%MUSF_ROOT%\toolchain\python311\python.exe"

if not exist "%PYTHON_EXE%" (
    echo Missing %PYTHON_EXE%
    echo Run scripts\bootstrap-python.cmd first.
    exit /b 1
)

"%PYTHON_EXE%" "%MUSF_ROOT%\scripts\python\musf.py" %*
exit /b %errorlevel%
