param(
    [string]$ServerRoot = (Join-Path $PSScriptRoot ".."),
    [Parameter(Mandatory = $true)]
    [string]$PublicHost,
    [string]$GmHttpPrefix = "http://127.0.0.1:65001/",
    [string]$GatewayListenHost = "0.0.0.0",
    [string]$InnerListenHost = "127.0.0.1",
    [switch]$SkipFileServer
)

$ErrorActionPreference = "Stop"

$serverRootPath = (Resolve-Path -LiteralPath $ServerRoot).Path
$configRoot = Join-Path $serverRootPath "Config"
$binRoot = Join-Path $serverRootPath "Bin"
$fileServerRoot = Join-Path $serverRootPath "FileServer"
$runtimeRoot = Join-Path $serverRootPath "Runtime"
$pidFile = Join-Path $runtimeRoot "cloud-start.pids.json"
$staticServerPidFile = Join-Path $runtimeRoot "static-file-server.pid"
$staticServerScript = Join-Path $PSScriptRoot "Start-StaticFileServer.ps1"

if (-not (Test-Path -LiteralPath $runtimeRoot)) {
    New-Item -ItemType Directory -Path $runtimeRoot -Force | Out-Null
}

if (Test-Path -LiteralPath $staticServerPidFile) {
    Remove-Item -LiteralPath $staticServerPidFile -Force -ErrorAction SilentlyContinue
}

& (Join-Path $PSScriptRoot "Prepare-CloudConfig.ps1") `
    -ConfigRoot $configRoot `
    -PublicHost $PublicHost `
    -GatewayListenHost $GatewayListenHost `
    -InnerListenHost $InnerListenHost `
    -GmHttpPrefix $GmHttpPrefix

$tempRoot = Join-Path $binRoot "temp"
if (Test-Path -LiteralPath $tempRoot) {
    Remove-Item -LiteralPath $tempRoot -Recurse -Force
}

$entries = @()

function Convert-ToDetachedCommandLine {
    param(
        [string]$FilePath,
        [string[]]$ArgumentList
    )

    $parts = @($FilePath) + @($ArgumentList)
    return (($parts | ForEach-Object {
        if ($_ -match '[\s"]') {
            '"' + ($_ -replace '"', '\"') + '"'
        }
        else {
            $_
        }
    }) -join " ")
}

function Start-DetachedProcess {
    param(
        [string]$Name,
        [string]$FilePath,
        [string]$WorkingDirectory,
        [string[]]$ArgumentList
    )

    $commandLine = "cmd.exe /c cd /d {0} && {1}" -f (
        Convert-ToDetachedCommandLine -FilePath $WorkingDirectory -ArgumentList @()
    ), (
        Convert-ToDetachedCommandLine -FilePath $FilePath -ArgumentList $ArgumentList
    )
    $result = Invoke-CimMethod -ClassName Win32_Process -MethodName Create -Arguments @{
        CommandLine = $commandLine
    }

    if ($null -eq $result -or [int]$result.ReturnValue -ne 0) {
        $code = if ($null -eq $result) { "null" } else { [string]$result.ReturnValue }
        throw ("Failed to start {0}: code {1}" -f $Name, $code)
    }

    return [pscustomobject]@{
        Name = $Name
        Id = [int]$result.ProcessId
        FilePath = $FilePath
        WorkingDirectory = $WorkingDirectory
        ArgumentList = ($ArgumentList -join " ")
        StartedAt = (Get-Date).ToString("s")
    }
}

function Start-StaticFileServerFallback {
    if (-not (Test-Path -LiteralPath $staticServerScript)) {
        throw "Static file server script not found: $staticServerScript"
    }

    $releaseRoot = Join-Path $serverRootPath "Release"
    $process = Start-DetachedProcess `
        -Name "StaticFileServer" `
        -FilePath "powershell.exe" `
        -WorkingDirectory $serverRootPath `
        -ArgumentList @("-NoProfile", "-ExecutionPolicy", "Bypass", "-File", $staticServerScript, "-RootPath", $releaseRoot)

    [System.IO.File]::WriteAllText($staticServerPidFile, [string]$process.Id, [System.Text.UTF8Encoding]::new($false))
    Write-Output ("StaticFileServer => PID {0}" -f $process.Id)
}

if (-not $SkipFileServer) {
    $entries += [pscustomobject]@{
        Name = "FileServer"
        FilePath = "dotnet"
        WorkingDirectory = $fileServerRoot
        ArgumentList = @("FileServer.dll")
    }
}

$appExe = Join-Path $binRoot "App.exe"
$entries += @(
    [pscustomobject]@{ Name = "Realm-2"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=2", "--AppType=Realm", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Gate-20004"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=20004", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Gate-20005"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=20005", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Gate-20006"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=20006", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Gate-20007"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=20007", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Gate-20008"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=20008", "--AppType=Gate", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "GM-99"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=99", "--AppType=GM", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "DB-10"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=10", "--AppType=DB", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Match-11"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=11", "--AppType=Match", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "LoginCenter-25"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=25", "--AppType=LoginCenter", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "DB-21"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=21", "--AppType=DB", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "DB-278"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=278", "--AppType=DB", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "DB-279"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=279", "--AppType=DB", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "MGMT-280"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=280", "--AppType=MGMT", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Game-257"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=257", "--AppType=Game", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Game-259"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=259", "--AppType=Game", "--ConfigPath=../Config", "--LogLevel=Info") },
    [pscustomobject]@{ Name = "Game-262"; FilePath = $appExe; WorkingDirectory = $binRoot; ArgumentList = @("--AppId=262", "--AppType=Game", "--ConfigPath=../Config", "--LogLevel=Info") }
)

$started = foreach ($entry in $entries) {
    Start-DetachedProcess `
        -Name $entry.Name `
        -FilePath $entry.FilePath `
        -WorkingDirectory $entry.WorkingDirectory `
        -ArgumentList $entry.ArgumentList
}

$started | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $pidFile -Encoding UTF8

Write-Output ("Started {0} processes" -f $started.Count)
Write-Output ("PID file: {0}" -f $pidFile)
$started | ForEach-Object { Write-Output ("{0} => PID {1}" -f $_.Name, $_.Id) }

if (-not $SkipFileServer) {
    Start-Sleep -Seconds 3
    $fileServerProcesses = Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" | Where-Object { $_.CommandLine -like "*FileServer.dll*" }
    if ($null -eq $fileServerProcesses -or @($fileServerProcesses).Count -eq 0) {
        Write-Output "FileServer runtime missing or exited. Falling back to PowerShell static file server."
        Start-StaticFileServerFallback
    }
}
