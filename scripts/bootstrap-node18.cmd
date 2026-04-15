@echo off
setlocal

set "MUSF_ROOT=%~dp0.."
for %%I in ("%MUSF_ROOT%") do set "MUSF_ROOT=%%~fI"
set "NODE_ROOT=%MUSF_ROOT%\toolchain\node18"
set "NODE_ZIP=%MUSF_ROOT%\archive\snapshots\node-v18.20.5-win-x64.zip"
set "NODE_URL=https://nodejs.org/dist/v18.20.5/node-v18.20.5-win-x64.zip"
set "NODE_EXTRACT_ROOT=%MUSF_ROOT%\toolchain\node18-extract"

if exist "%NODE_ROOT%\node.exe" (
    echo Node already installed: %NODE_ROOT%\node.exe
    exit /b 0
)

if not exist "%MUSF_ROOT%\archive\snapshots" mkdir "%MUSF_ROOT%\archive\snapshots"
if not exist "%MUSF_ROOT%\toolchain" mkdir "%MUSF_ROOT%\toolchain"

echo Downloading Node 18 to %NODE_ZIP%
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "Invoke-WebRequest -UseBasicParsing -Uri '%NODE_URL%' -OutFile '%NODE_ZIP%'"
if errorlevel 1 exit /b 1

if exist "%NODE_EXTRACT_ROOT%" rd /s /q "%NODE_EXTRACT_ROOT%"
mkdir "%NODE_EXTRACT_ROOT%"

echo Extracting Node to %NODE_EXTRACT_ROOT%
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "Expand-Archive -LiteralPath '%NODE_ZIP%' -DestinationPath '%NODE_EXTRACT_ROOT%' -Force"
if errorlevel 1 exit /b 1

if exist "%NODE_ROOT%" rd /s /q "%NODE_ROOT%"
move "%NODE_EXTRACT_ROOT%\node-v18.20.5-win-x64" "%NODE_ROOT%"
if errorlevel 1 exit /b 1

rd /s /q "%NODE_EXTRACT_ROOT%"
echo Node installed at %NODE_ROOT%
exit /b 0
