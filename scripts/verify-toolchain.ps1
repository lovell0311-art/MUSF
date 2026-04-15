param(
    [string]$ManifestPath = "",
    [string]$OutputPath = "",
    [switch]$Quiet
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "utils\musf-common.ps1")

$root = Get-MUSFRoot
if (-not $ManifestPath) {
    $ManifestPath = Join-Path $root "toolchain-manifest.json"
}

$reportRoot = Join-Path $root "reports\toolchain"
if (-not (Test-Path $reportRoot)) {
    New-Item -ItemType Directory -Path $reportRoot | Out-Null
}

if (-not $OutputPath) {
    $OutputPath = Join-Path $reportRoot ("toolchain-status-{0}.json" -f (Get-Date -Format "yyyyMMdd-HHmmss"))
}

$manifest = Read-MUSFJson -Path $ManifestPath

function Get-DetectedVersion {
    param(
        [string]$ToolName,
        [string]$ResolvedPath
    )

    try {
        switch ($ToolName) {
            "unity-editor" {
                if (Test-Path $ResolvedPath) {
                    return (Get-Item $ResolvedPath).VersionInfo.ProductVersion
                }
            }
            "adb" {
                if (Test-Path $ResolvedPath) {
                    return (& $ResolvedPath version 2>&1 | Select-Object -First 1).ToString().Trim()
                }
            }
            "node18" {
                if (Test-Path $ResolvedPath) {
                    return (& $ResolvedPath -v).Trim()
                }
                elseif (Get-Command node -ErrorAction SilentlyContinue) {
                    return (& node -v).Trim()
                }
            }
            "dotnet-runtime" {
                if (Test-Path $ResolvedPath) {
                    return (& $ResolvedPath --list-runtimes | Out-String).Trim()
                }
            }
            "mongodb" {
                if (Test-Path $ResolvedPath) {
                    return (& $ResolvedPath --version 2>&1 | Select-Object -First 1).ToString().Trim()
                }
            }
            "nginx" {
                if (Test-Path $ResolvedPath) {
                    return (& $ResolvedPath -v 2>&1).ToString().Trim()
                }
            }
        }
    }
    catch {
        return $_.Exception.Message
    }

    return ""
}

$results = foreach ($item in $manifest.items) {
    $formalExists = $item.path -and (Test-Path $item.path)
    $fallbackExists = $item.currentPath -and (Test-Path $item.currentPath)
    $resolvedPath = if ($formalExists) { $item.path } elseif ($fallbackExists) { $item.currentPath } else { $item.path }
    $status = if ($formalExists) { "installed" } elseif ($fallbackExists) { "mismatch" } else { "missing" }
    $detectedVersion = Get-DetectedVersion -ToolName $item.name -ResolvedPath $resolvedPath

    [PSCustomObject]@{
        name = $item.name
        expectedVersion = $item.version
        formalPath = $item.path
        detectedPath = $resolvedPath
        required = [bool]$item.required
        status = $status
        detectedVersion = $detectedVersion
        notes = $item.notes
    }
}

$report = [PSCustomObject]@{
    schemaVersion = 1
    generatedAt = (Get-Date).ToString("yyyy-MM-ddTHH:mm:sszzz")
    root = $root
    manifestPath = $ManifestPath
    outputPath = $OutputPath
    items = $results
}

Write-MUSFJson -InputObject $report -Path $OutputPath

if (-not $Quiet) {
    $results |
        Select-Object name, status, formalPath, detectedPath |
        Format-Table -AutoSize |
        Out-String |
        Write-Host
    Write-Host ("REPORT={0}" -f $OutputPath)
}
