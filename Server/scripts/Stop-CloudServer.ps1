param(
    [string]$ServerRoot = (Join-Path $PSScriptRoot "..")
)

$ErrorActionPreference = "Stop"

$serverRootPath = (Resolve-Path -LiteralPath $ServerRoot).Path
$runtimeRoot = Join-Path $serverRootPath "Runtime"
$pidFile = Join-Path $runtimeRoot "cloud-start.pids.json"
$staticServerPidFile = Join-Path $runtimeRoot "static-file-server.pid"
$stopped = New-Object System.Collections.Generic.List[string]

if (Test-Path -LiteralPath $pidFile) {
    $records = Get-Content -LiteralPath $pidFile -Raw -Encoding UTF8 | ConvertFrom-Json
    foreach ($record in @($records)) {
        try {
            $process = Get-Process -Id ([int]$record.Id) -ErrorAction Stop
            Stop-Process -Id $process.Id -Force
            $stopped.Add(("{0} => PID {1}" -f $record.Name, $process.Id))
        }
        catch {
        }
    }

    Remove-Item -LiteralPath $pidFile -Force
}
if (Test-Path -LiteralPath $staticServerPidFile) {
    try {
        $staticPid = [int](Get-Content -LiteralPath $staticServerPidFile -Raw -Encoding UTF8)
        Stop-Process -Id $staticPid -Force -ErrorAction SilentlyContinue
        $stopped.Add(("StaticFileServer => PID {0}" -f $staticPid))
    }
    catch {
    }

    Remove-Item -LiteralPath $staticServerPidFile -Force -ErrorAction SilentlyContinue
}

$appExePath = Join-Path $serverRootPath "Bin\App.exe"
$appProcesses = Get-CimInstance Win32_Process -Filter "Name='App.exe'" | Where-Object { $_.ExecutablePath -eq $appExePath }
foreach ($process in $appProcesses) {
    try {
        Stop-Process -Id $process.ProcessId -Force
        $stopped.Add(("App => PID {0}" -f $process.ProcessId))
    }
    catch {
    }
}

$fileServerProcesses = Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" | Where-Object { $_.CommandLine -like "*FileServer.dll*" }
foreach ($process in $fileServerProcesses) {
    try {
        Stop-Process -Id $process.ProcessId -Force
        $stopped.Add(("FileServer => PID {0}" -f $process.ProcessId))
    }
    catch {
    }
}

$staticServerProcesses = Get-CimInstance Win32_Process -Filter "Name='powershell.exe'" | Where-Object { $_.CommandLine -like "*Start-StaticFileServer.ps1*" }
foreach ($process in $staticServerProcesses) {
    try {
        Stop-Process -Id $process.ProcessId -Force
        $stopped.Add(("StaticFileServer => PID {0}" -f $process.ProcessId))
    }
    catch {
    }
}

$staticNodeProcesses = Get-CimInstance Win32_Process -Filter "Name='node.exe'" | Where-Object { $_.CommandLine -like "*serve_release_http_8080.js*" }
foreach ($process in $staticNodeProcesses) {
    try {
        Stop-Process -Id $process.ProcessId -Force
        $stopped.Add(("StaticNode => PID {0}" -f $process.ProcessId))
    }
    catch {
    }
}

if ($stopped.Count -eq 0) {
    Write-Output "No cloud server processes were stopped."
}
else {
    $stopped | ForEach-Object { Write-Output $_ }
}
