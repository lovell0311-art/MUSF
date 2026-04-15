param(
    [string]$RootPath = (Join-Path (Join-Path $PSScriptRoot "..") "Release\update"),
    [string[]]$TargetDirs
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$fileIgnoreRegexes = @(
    '\.meta$',
    '\.(bak|bad|pre|test|temp)($|[-.])',
    '^version\.txt$',
    '^login-trace.*\.txt$',
    '\.(log|png|jpg|jpeg)$'
)

function Test-PublishableFileName {
    param(
        [string]$FileName
    )

    if ([string]::IsNullOrWhiteSpace($FileName)) {
        return $false
    }

    foreach ($pattern in $fileIgnoreRegexes) {
        if ($FileName -match $pattern) {
            return $false
        }
    }

    return $true
}

function Get-StableVersion {
    param(
        [string[]]$Fingerprints
    )

    if ($null -eq $Fingerprints -or $Fingerprints.Count -eq 0) {
        return 1
    }

    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    try {
        $payload = [System.Text.Encoding]::UTF8.GetBytes((($Fingerprints | Sort-Object) -join "`n"))
        $hashBytes = $sha256.ComputeHash($payload)
        $value = [System.BitConverter]::ToUInt32($hashBytes, 0)
        if ($value -gt [int]::MaxValue) {
            $value = $value % [int]::MaxValue
        }

        if ($value -eq 0) {
            $value = 1
        }

        return [int]$value
    }
    finally {
        $sha256.Dispose()
    }
}

function Get-ResolvedTargetDirs {
    if ($null -ne $TargetDirs -and $TargetDirs.Count -gt 0) {
        foreach ($targetDir in $TargetDirs) {
            if (-not (Test-Path -LiteralPath $targetDir -PathType Container)) {
                throw "Target directory not found: $targetDir"
            }

            (Resolve-Path -LiteralPath $targetDir).Path
        }

        return
    }

    if (-not (Test-Path -LiteralPath $RootPath -PathType Container)) {
        throw "Update root not found: $RootPath"
    }

    Get-ChildItem -LiteralPath $RootPath -Recurse -Directory |
        Where-Object { $_.Name -eq "StreamingAssets" } |
        Select-Object -ExpandProperty FullName
}

$resolvedDirs = @(Get-ResolvedTargetDirs | Sort-Object -Unique)
if ($resolvedDirs.Count -eq 0) {
    throw "No StreamingAssets directories found under: $RootPath"
}

foreach ($targetDir in $resolvedDirs) {
    $dict = [ordered]@{}
    $total = [int64]0
    $fingerprints = New-Object 'System.Collections.Generic.List[string]'

    $files = @(
        Get-ChildItem -LiteralPath $targetDir -File |
            Where-Object { Test-PublishableFileName -FileName $_.Name } |
            Sort-Object Name
    )

    foreach ($file in $files) {
        $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm MD5).Hash.ToLowerInvariant()
        $dict[$file.Name] = [ordered]@{
            File = $file.Name
            MD5 = $hash
            Size = [int64]$file.Length
        }

        $total += [int64]$file.Length
        $fingerprints.Add(("{0}|{1}|{2}" -f $file.Name, $hash, [int64]$file.Length))
    }

    $versionConfig = [ordered]@{
        Version = (Get-StableVersion -Fingerprints $fingerprints)
        TotalSize = $total
        FileInfoDict = $dict
    }

    $json = $versionConfig | ConvertTo-Json -Depth 6 -Compress
    [System.IO.File]::WriteAllText((Join-Path $targetDir "Version.txt"), $json, $utf8NoBom)
    Write-Output ("REBUILT_VERSION={0} VERSION={1} FILES={2} SIZE={3}" -f $targetDir, $versionConfig.Version, $files.Count, $total)
}
