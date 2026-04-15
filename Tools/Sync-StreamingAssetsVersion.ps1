param(
    [string]$ReleaseDir = 'F:\MUSF\Server\Release\Android\StreamingAssets',
    [string]$UpdateDir = 'F:\MUSF\Server\Release\update\2.0TestGame\Android\StreamingAssets',
    [string]$ArchiveRoot = 'F:\MUSF\Server\_archive\StreamingAssets'
)

$ErrorActionPreference = 'Stop'

function Get-NormalizedRelativePath([string]$path) {
    if ([string]::IsNullOrWhiteSpace($path)) {
        return ''
    }

    return $path.Replace('/', '\').Trim('\')
}

function Should-SkipEntry([string]$relativePath) {
    $normalizedPath = Get-NormalizedRelativePath $relativePath
    if ([string]::IsNullOrWhiteSpace($normalizedPath)) {
        return $true
    }

    $segments = $normalizedPath.Split('\', [System.StringSplitOptions]::RemoveEmptyEntries)
    if ($segments.Count -eq 0) {
        return $true
    }

    $leafName = $segments[-1]
    if ($leafName.Equals('Version.txt', [System.StringComparison]::OrdinalIgnoreCase)) {
        return $true
    }

    if ($leafName.EndsWith('-trace.txt', [System.StringComparison]::OrdinalIgnoreCase)) {
        return $true
    }

    foreach ($segment in $segments) {
        if ($segment.StartsWith('backup-', [System.StringComparison]::OrdinalIgnoreCase) -or
            $segment.StartsWith('backup_', [System.StringComparison]::OrdinalIgnoreCase) -or
            $segment.IndexOf('.bak', [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
            return $true
        }
    }

    return $false
}

function Get-Md5([string]$path) {
    return (Get-FileHash -Algorithm MD5 -LiteralPath $path).Hash.ToLowerInvariant()
}

function Get-UnixTimeSeconds {
    return [int][DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
}

function Get-RelativePath([string]$root, [string]$fullPath) {
    $rootUri = [Uri]((Resolve-Path -LiteralPath $root).Path.TrimEnd('\') + '\')
    $fileUri = [Uri](Resolve-Path -LiteralPath $fullPath).Path
    return [Uri]::UnescapeDataString($rootUri.MakeRelativeUri($fileUri).ToString()).Replace('/', '\')
}

function Move-SkippedRootEntriesToArchive([string]$rootDir, [string]$label) {
    if (!(Test-Path -LiteralPath $rootDir)) {
        return
    }

    $entriesToMove = @(
        Get-ChildItem -LiteralPath $rootDir -Force |
            Where-Object { Should-SkipEntry $_.Name }
    )

    if ($entriesToMove.Count -eq 0) {
        return
    }

    $safeLabel = ($label -replace '[^A-Za-z0-9._-]', '_').Trim('_')
    $archiveDir = Join-Path $ArchiveRoot ("{0}-{1}" -f $safeLabel, (Get-Date -Format 'yyyyMMdd-HHmmss'))
    New-Item -ItemType Directory -Path $archiveDir -Force | Out-Null

    foreach ($entry in $entriesToMove) {
        Move-Item -LiteralPath $entry.FullName -Destination (Join-Path $archiveDir $entry.Name)
    }
}

function New-VersionObject([string]$rootDir) {
    $entries = [ordered]@{}
    $files = Get-ChildItem -LiteralPath $rootDir -Recurse -File |
        Sort-Object { $_.FullName.Replace('/', '\') }, Name

    foreach ($file in $files) {
        $relativePath = Get-RelativePath $rootDir $file.FullName
        if (Should-SkipEntry $relativePath) {
            continue
        }

        $entryKey = $relativePath.Replace('\', '/')
        $entries[$entryKey] = [ordered]@{
            File = $entryKey
            MD5 = (Get-Md5 $file.FullName)
            Size = [int64]$file.Length
        }
    }

    $versionPath = Join-Path $rootDir 'Version.txt'
    $existingVersion = 0
    if (Test-Path -LiteralPath $versionPath) {
        try {
            $existing = Get-Content -LiteralPath $versionPath -Raw | ConvertFrom-Json
            if ($existing -and $existing.FileInfoDict) {
                $existingJson = ($existing.FileInfoDict | ConvertTo-Json -Compress -Depth 8)
                $currentJson = ($entries | ConvertTo-Json -Compress -Depth 8)
                if ($existingJson -eq $currentJson) {
                    $existingVersion = [int]$existing.Version
                }
            }
        }
        catch {
        }
    }

    return [ordered]@{
        Version = $(if ($existingVersion -gt 0) { $existingVersion } else { Get-UnixTimeSeconds })
        TotalSize = 0
        FileInfoDict = $entries
    }
}

function Write-VersionFile([string]$rootDir, [hashtable]$versionObject) {
    $json = $versionObject | ConvertTo-Json -Compress -Depth 8
    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText((Join-Path $rootDir 'Version.txt'), $json, $utf8NoBom)
}

function Validate-VersionDirectory([string]$rootDir) {
    $version = Get-Content -LiteralPath (Join-Path $rootDir 'Version.txt') -Raw | ConvertFrom-Json
    $mismatches = New-Object System.Collections.Generic.List[string]
    foreach ($prop in $version.FileInfoDict.PSObject.Properties | Sort-Object Name) {
        $entry = $prop.Value
        if (Should-SkipEntry $entry.File) {
            $mismatches.Add("$($entry.File): unexpected skipped entry")
            if ($mismatches.Count -ge 8) {
                break
            }
            continue
        }

        $path = [System.IO.Path]::Combine($rootDir, $entry.File.Replace('/', '\'))
        if (!(Test-Path -LiteralPath $path)) {
            $mismatches.Add("$($entry.File): missing")
        }
        else {
            $item = Get-Item -LiteralPath $path
            if ($item.Length -ne [int64]$entry.Size) {
                $mismatches.Add("$($entry.File): size $($item.Length) != $($entry.Size)")
            }
            else {
                $actual = Get-Md5 $path
                if ($actual -ne $entry.MD5) {
                    $mismatches.Add("$($entry.File): md5 $actual != $($entry.MD5)")
                }
            }
        }

        if ($mismatches.Count -ge 8) {
            break
        }
    }

    if ($mismatches.Count -gt 0) {
        throw "Validation failed for $rootDir. $($mismatches -join ' | ')"
    }

    return @($version.FileInfoDict.PSObject.Properties).Count
}

if (!(Test-Path -LiteralPath $ReleaseDir)) {
    throw "ReleaseDir not found: $ReleaseDir"
}

Move-SkippedRootEntriesToArchive -rootDir $ReleaseDir -label 'release'

$versionObject = New-VersionObject $ReleaseDir
Write-VersionFile $ReleaseDir $versionObject

$backupDir = $null
if (Test-Path -LiteralPath $UpdateDir) {
    $backupDir = "$UpdateDir.bak-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    Move-Item -LiteralPath $UpdateDir -Destination $backupDir
}

Copy-Item -LiteralPath $ReleaseDir -Destination $UpdateDir -Recurse -Force

$releaseCount = Validate-VersionDirectory $ReleaseDir
$updateCount = Validate-VersionDirectory $UpdateDir

Write-Output "ReleaseDir=$ReleaseDir"
Write-Output "UpdateDir=$UpdateDir"
Write-Output "BackupDir=$backupDir"
Write-Output "Version=$($versionObject.Version)"
Write-Output "Entries=$releaseCount"
Write-Output "ReleaseVersionMd5=$(Get-Md5 (Join-Path $ReleaseDir 'Version.txt'))"
Write-Output "UpdateVersionMd5=$(Get-Md5 (Join-Path $UpdateDir 'Version.txt'))"
Write-Output "Status=OK"
