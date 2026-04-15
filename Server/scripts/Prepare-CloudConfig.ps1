param(
    [string]$ConfigRoot = (Join-Path (Join-Path $PSScriptRoot "..") "Config"),
    [Parameter(Mandatory = $true)]
    [string]$PublicHost,
    [string]$GatewayListenHost = "0.0.0.0",
    [string]$InnerListenHost = "127.0.0.1",
    [string]$GmHttpPrefix = "http://127.0.0.1:65001/"
)

$ErrorActionPreference = "Stop"

function Set-HostKeepPort {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Address,
        [Parameter(Mandatory = $true)]
        [string]$TargetHost
    )

    $separatorIndex = $Address.LastIndexOf(":")
    if ($separatorIndex -lt 0 -or $separatorIndex -ge $Address.Length - 1) {
        throw "Invalid address format: $Address"
    }

    $port = $Address.Substring($separatorIndex + 1)
    return "{0}:{1}" -f $TargetHost, $port
}

$configRootPath = (Resolve-Path -LiteralPath $ConfigRoot).Path
$startUpPath = Join-Path $configRootPath "StartUpConfig\StartUp_ServerConfig.json"

if (-not (Test-Path -LiteralPath $startUpPath)) {
    throw "StartUp config not found: $startUpPath"
}

$config = Get-Content -LiteralPath $startUpPath -Raw -Encoding UTF8 | ConvertFrom-Json

foreach ($entry in $config) {
    if ($entry.InnerConfig -and $entry.InnerConfig.Address) {
        $entry.InnerConfig.Address = Set-HostKeepPort -Address ([string]$entry.InnerConfig.Address) -TargetHost $InnerListenHost
    }

    if ($entry.OuterConfig -and $entry.OuterConfig.Address2) {
        $entry.OuterConfig.Address2 = Set-HostKeepPort -Address ([string]$entry.OuterConfig.Address2) -TargetHost $PublicHost
    }

    if ($entry.OuterConfig -and $entry.OuterConfig.Address) {
        switch ([string]$entry.AppType) {
            "Manager" {
                $entry.OuterConfig.Address = Set-HostKeepPort -Address ([string]$entry.OuterConfig.Address) -TargetHost $PublicHost
            }
            "Realm" {
                $entry.OuterConfig.Address = Set-HostKeepPort -Address ([string]$entry.OuterConfig.Address) -TargetHost $GatewayListenHost
            }
            "Gate" {
                $entry.OuterConfig.Address = Set-HostKeepPort -Address ([string]$entry.OuterConfig.Address) -TargetHost $GatewayListenHost
            }
        }
    }

    if ([string]$entry.AppType -eq "GM" -and $entry.OuterConfig -and $entry.OuterConfig.HttpConfig) {
        $httpConfig = [pscustomobject]@{}
        if (-not [string]::IsNullOrWhiteSpace([string]$entry.OuterConfig.HttpConfig)) {
            $httpConfig = [string]$entry.OuterConfig.HttpConfig | ConvertFrom-Json
        }

        $httpConfig | Add-Member -NotePropertyName Url -NotePropertyValue $GmHttpPrefix -Force
        $entry.OuterConfig.HttpConfig = $httpConfig | ConvertTo-Json -Compress
    }
}

$backupDir = Join-Path $configRootPath "StartUpConfig\Backup"
if (-not (Test-Path -LiteralPath $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
}

$backupPath = Join-Path $backupDir ("StartUp_ServerConfig.json.bak-{0}" -f (Get-Date -Format "yyyyMMdd-HHmmss"))
Copy-Item -LiteralPath $startUpPath -Destination $backupPath -Force

$json = $config | ConvertTo-Json -Depth 8
$utf8NoBom = [System.Text.UTF8Encoding]::new($false)
[System.IO.File]::WriteAllText($startUpPath, $json, $utf8NoBom)

Write-Output ("Config rewritten: {0}" -f $startUpPath)
Write-Output ("Backup saved: {0}" -f $backupPath)
Write-Output ("Public host: {0}" -f $PublicHost)
Write-Output ("Gateway listen host: {0}" -f $GatewayListenHost)
Write-Output ("GM HTTP prefix: {0}" -f $GmHttpPrefix)
