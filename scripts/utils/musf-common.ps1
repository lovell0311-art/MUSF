$ErrorActionPreference = "Stop"

function Get-MUSFRoot {
    if ($env:MUSF_ROOT -and (Test-Path $env:MUSF_ROOT)) {
        return (Resolve-Path $env:MUSF_ROOT).Path
    }

    $candidate = Resolve-Path (Join-Path $PSScriptRoot "..\..")
    if (Test-Path (Join-Path $candidate "Client")) {
        return $candidate.Path
    }

    return "F:\MUSF"
}

function Read-MUSFJson {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    return Get-Content -Raw -Path $Path | ConvertFrom-Json
}

function Write-MUSFJson {
    param(
        [Parameter(Mandatory = $true)]
        [object]$InputObject,
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $parent = Split-Path -Parent $Path
    if ($parent -and -not (Test-Path $parent)) {
        New-Item -ItemType Directory -Path $parent | Out-Null
    }

    $json = $InputObject | ConvertTo-Json -Depth 100
    [System.IO.File]::WriteAllText($Path, $json + [Environment]::NewLine, [System.Text.Encoding]::UTF8)
}

function Get-MUSFToolchainManifest {
    param([string]$Root = $(Get-MUSFRoot))

    $path = Join-Path $Root "toolchain-manifest.json"
    return Read-MUSFJson -Path $path
}

function Get-MUSFEnvProfile {
    param(
        [string]$ProfileName = "local-lan",
        [string]$Root = $(Get-MUSFRoot)
    )

    $path = Join-Path $Root ("env-profiles\{0}.json" -f $ProfileName)
    return Read-MUSFJson -Path $path
}

function Resolve-MUSFToolPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,
        [string]$Root = $(Get-MUSFRoot)
    )

    $manifest = Get-MUSFToolchainManifest -Root $Root
    $item = $manifest.items | Where-Object { $_.name -eq $Name } | Select-Object -First 1
    if (-not $item) {
        throw "Tool not found in manifest: $Name"
    }

    if ($item.path -and (Test-Path $item.path)) {
        return $item.path
    }

    if ($item.currentPath -and (Test-Path $item.currentPath)) {
        return $item.currentPath
    }

    return $item.path
}

function Test-MUSFTcpPort {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Host,
        [Parameter(Mandatory = $true)]
        [int]$Port,
        [int]$TimeoutMs = 1500
    )

    $client = New-Object System.Net.Sockets.TcpClient
    try {
        $iar = $client.BeginConnect($Host, $Port, $null, $null)
        $success = $iar.AsyncWaitHandle.WaitOne($TimeoutMs, $false)
        if (-not $success) {
            return $false
        }

        $client.EndConnect($iar) | Out-Null
        return $true
    }
    catch {
        return $false
    }
    finally {
        $client.Close()
    }
}

function Invoke-MUSFHttpCheck {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Url,
        [int]$TimeoutSec = 5
    )

    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec $TimeoutSec -UseBasicParsing
        return [PSCustomObject]@{
            ok = $true
            statusCode = [int]$response.StatusCode
            url = $Url
        }
    }
    catch {
        $statusCode = $null
        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode
        }

        return [PSCustomObject]@{
            ok = $false
            statusCode = $statusCode
            url = $Url
            error = $_.Exception.Message
        }
    }
}

function New-MUSFReleaseGateReport {
    param(
        [string]$ProfileName = "local-lan",
        [string]$Version = "manual",
        [string]$Phase = "phase1-gate",
        [string]$Root = $(Get-MUSFRoot)
    )

    $templatePath = Join-Path $Root "templates\release-gate-report.json"
    $report = Read-MUSFJson -Path $templatePath
    $report.version = $Version
    $report.timestamp = (Get-Date).ToString("yyyy-MM-ddTHH:mm:sszzz")
    $report.profile = $ProfileName
    $report.phase = $Phase
    $report.overall = "pending"
    $report.gates = @()
    $report.artifacts = @()
    $report.notes = @()
    return $report
}

function Add-MUSFReportGate {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Report,
        [Parameter(Mandatory = $true)]
        [string]$Name,
        [Parameter(Mandatory = $true)]
        [ValidateSet("pass", "fail", "warn", "pending")]
        [string]$Status,
        [string]$Details = "",
        [object[]]$Artifacts = @()
    )

    $Report.gates += [PSCustomObject]@{
        name = $Name
        status = $Status
        details = $Details
        artifacts = $Artifacts
    }
}

function Save-MUSFReport {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Report,
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $hasFail = @($Report.gates | Where-Object { $_.status -eq "fail" }).Count -gt 0
    $hasWarn = @($Report.gates | Where-Object { $_.status -eq "warn" }).Count -gt 0
    $Report.overall = if ($hasFail) { "fail" } elseif ($hasWarn) { "warn" } else { "pass" }
    Write-MUSFJson -InputObject $Report -Path $Path
}

function Sync-MUSFNetworkConfig {
    param(
        [string]$ProfileName = "local-lan",
        [switch]$WhatIf
    )

    $root = Get-MUSFRoot
    $profile = Get-MUSFEnvProfile -ProfileName $ProfileName -Root $root

    $serverConfigPath = Join-Path $root "Server\Config\StartUpConfig\StartUp_ServerConfig.json"
    $serverConfig = Read-MUSFJson -Path $serverConfigPath

    $managedPortMap = @{
        1 = $profile.ports.realm
        2 = $profile.ports.realm
        20004 = 10004
        20005 = 10005
        20006 = 10006
        20007 = 10007
        20008 = 10008
    }

    foreach ($entry in $serverConfig) {
        if ($managedPortMap.ContainsKey([int]$entry.AppId)) {
            if (-not $entry.OuterConfig) {
                $entry | Add-Member -MemberType NoteProperty -Name OuterConfig -Value ([PSCustomObject]@{}) -Force
            }

            $entry.OuterConfig.Address2 = "{0}:{1}" -f $profile.serverIp, $managedPortMap[[int]$entry.AppId]
        }
    }

    $globalProtoPath = Join-Path $root "Client\Unity\Assets\Res\Config\GlobalProto.txt"
    $globalProtoContent = Get-Content -Raw -Path $globalProtoPath
    $globalProtoContent = [System.Text.RegularExpressions.Regex]::Replace(
        $globalProtoContent,
        '"AssetBundleServerUrl":"[^"]*"',
        ('"AssetBundleServerUrl":"{0}"' -f $profile.client.assetBundleBaseUrl))
    $globalProtoContent = [System.Text.RegularExpressions.Regex]::Replace(
        $globalProtoContent,
        '"Address":"[^"]*"',
        ('"Address":"{0}"' -f $profile.client.loginAddress))

    if ($WhatIf) {
        return [PSCustomObject]@{
            serverConfigPath = $serverConfigPath
            globalProtoPath = $globalProtoPath
            serverIp = $profile.serverIp
            loginAddress = $profile.client.loginAddress
            assetBundleBaseUrl = $profile.client.assetBundleBaseUrl
        }
    }

    Write-MUSFJson -InputObject $serverConfig -Path $serverConfigPath
    [System.IO.File]::WriteAllText($globalProtoPath, $globalProtoContent, [System.Text.Encoding]::UTF8)

    return [PSCustomObject]@{
        serverConfigPath = $serverConfigPath
        globalProtoPath = $globalProtoPath
        serverIp = $profile.serverIp
        loginAddress = $profile.client.loginAddress
        assetBundleBaseUrl = $profile.client.assetBundleBaseUrl
    }
}
