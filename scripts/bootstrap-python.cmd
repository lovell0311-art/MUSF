@echo off
setlocal

set "MUSF_ROOT=%~dp0.."
for %%I in ("%MUSF_ROOT%") do set "MUSF_ROOT=%%~fI"
set "PY_ROOT=%MUSF_ROOT%\toolchain\python311"
set "PY_ZIP=%MUSF_ROOT%\archive\snapshots\python-3.11.9-embed-amd64.zip"
set "PY_URL=https://www.python.org/ftp/python/3.11.9/python-3.11.9-embed-amd64.zip"

if exist "%PY_ROOT%\python.exe" (
    echo Python already installed: %PY_ROOT%\python.exe
    exit /b 0
)

if not exist "%MUSF_ROOT%\archive\snapshots" mkdir "%MUSF_ROOT%\archive\snapshots"
if not exist "%PY_ROOT%" mkdir "%PY_ROOT%"

echo Downloading embedded Python to %PY_ZIP%
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "Invoke-WebRequest -UseBasicParsing -Uri '%PY_URL%' -OutFile '%PY_ZIP%'"
if errorlevel 1 exit /b 1

echo Extracting Python to %PY_ROOT%
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "Expand-Archive -LiteralPath '%PY_ZIP%' -DestinationPath '%PY_ROOT%' -Force"
if errorlevel 1 exit /b 1

echo Python installed at %PY_ROOT%
exit /b 0
