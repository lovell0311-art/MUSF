param(
    [switch]$CodeOnly,
    [switch]$SkipAngeBaselineReport,
    [switch]$FailOnAngeCriticalDiff,
    [string]$AngeApkPath = "F:\MUSF\ange.apk",
    [string[]]$PublishCategories = @("LoginFlow", "RoleModel", "WorldUI", "GameplayUI", "SocialUI", "OperationsUI", "WorldMap", "WorldAudio", "Audio")
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$projectRoot = "F:\MUSF"
$sourceDir = Join-Path $projectRoot "Client\Unity\Assets\StreamingAssets"
$targets = @(
    (Join-Path $projectRoot "Server\Release\Android\StreamingAssets"),
    (Join-Path $projectRoot "Release_Test\Android\StreamingAssets")
)
$criticalBundleListPath = Join-Path $projectRoot "Tools\UIClone\ange\critical-bundles.txt"
$angeToolsModulePath = Join-Path $projectRoot "Tools\UIClone\ange\AngeBaselineTools.psm1"
$lockedMainUiConfigPath = Join-Path $projectRoot "Tools\LockedBundles\MainUiLayout\lock-config.json"
$lockedMainUiSnapshotDir = Join-Path $projectRoot "Tools\LockedBundles\MainUiLayout\snapshot"
$lockedLieFengUiConfigPath = Join-Path $projectRoot "Tools\LockedBundles\LieFengUi\lock-config.json"
$lockedLieFengUiSnapshotDir = Join-Path $projectRoot "Tools\LockedBundles\LieFengUi\snapshot"
$publishIgnoreRegexes = @(
    '\.meta$',
    '\.(bak|bad|pre|test|temp)($|[-.])',
    '^version\.txt$',
    '^login-trace.*\.txt$',
    '\.(log|png|jpg|jpeg)$'
)

$coreVersionFiles = @(
    "code.unity3d",
    "code.unity3d.manifest",
    "config.unity3d",
    "config.unity3d.manifest",
    "navgriddata.unity3d",
    "navgriddata.unity3d.manifest",
    "qiao.unity3d",
    "qiao.unity3d.manifest",
    "shaders.unity3d",
    "shaders.unity3d.manifest",
    "chooserole.unity3d",
    "chooserole.unity3d.manifest",
    "StreamingAssets",
    "StreamingAssets.manifest",
    "ui_hud.unity3d",
    "ui_hud.unity3d.manifest",
    "uilogin.unity3d",
    "uilogin.unity3d.manifest",
    "uimaincanvas.unity3d",
    "uimaincanvas.unity3d.manifest",
    "uiselectserver.unity3d",
    "uiselectserver.unity3d.manifest"
)

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$angeCompareScript = Join-Path $projectRoot "Tools\UIClone\ange\Compare-AngeBaseline.ps1"

if (-not (Test-Path -LiteralPath $angeToolsModulePath)) {
    throw "ange tools module not found: $angeToolsModulePath"
}

Import-Module $angeToolsModulePath -Force -DisableNameChecking

function Add-UniqueFile {
    param(
        [System.Collections.Generic.List[string]]$List,
        [string]$FileName
    )

    if ([string]::IsNullOrWhiteSpace($FileName)) {
        return
    }

    if (-not $List.Contains($FileName)) {
        $List.Add($FileName)
    }
}

function Test-PublishableFileName {
    param(
        [string]$FileName
    )

    if ([string]::IsNullOrWhiteSpace($FileName)) {
        return $false
    }

    foreach ($pattern in $publishIgnoreRegexes) {
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

function Get-CategorizedPatchFiles {
    param(
        [string]$BundlesPath,
        [string[]]$Categories,
        [string[]]$SceneNames
    )

    $files = New-Object 'System.Collections.Generic.List[string]'

    if ($Categories.Count -eq 0) {
        return $files
    }

    $categorySet = @{}
    foreach ($category in $Categories) {
        if (-not [string]::IsNullOrWhiteSpace($category)) {
            $categorySet[$category.Trim()] = $true
        }
    }

    foreach ($bundle in Get-ChildItem -LiteralPath $BundlesPath -File | Sort-Object Name) {
        if (-not (Test-PublishableFileName -FileName $bundle.Name)) {
            continue
        }

        $category = Get-BundleCategory -FileName $bundle.Name -SceneNames $SceneNames
        if (-not $categorySet.ContainsKey($category)) {
            continue
        }

        Add-UniqueFile -List $files -FileName $bundle.Name
    }

    return $files
}

function Test-ListContains {
    param(
        [System.Collections.Generic.List[string]]$List,
        [string]$FileName
    )

    return $List.Contains($FileName)
}

function Assert-RequiredPublishedFiles {
    param(
        [string]$BundlesPath,
        [System.Collections.Generic.List[string]]$VersionFiles,
        [string[]]$SceneNames
    )

    $requiredExact = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::OrdinalIgnoreCase)
    $requiredPrefixes = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::OrdinalIgnoreCase)

    if (
        (Test-ListContains -List $VersionFiles -FileName 'chooserole.unity3d') -or
        (Test-ListContains -List $VersionFiles -FileName 'uichooserole.unity3d')
    ) {
        foreach ($fileName in @(
            'uichooserole.unity3d',
            'uichooserole.unity3d.manifest',
            'uicreatrole.unity3d',
            'uicreatrole.unity3d.manifest',
            'rolehalf.unity3d',
            'rolehalf.unity3d.manifest',
            'shaders.unity3d',
            'shaders.unity3d.manifest'
        )) {
            $null = $requiredExact.Add($fileName)
        }

        foreach ($prefix in @('role_', 'animator_role_')) {
            $null = $requiredPrefixes.Add($prefix)
        }
    }

    $publishedSceneBundles = @(
        $SceneNames |
            ForEach-Object { "$_.unity3d" } |
            Where-Object { Test-ListContains -List $VersionFiles -FileName $_ }
    )
    if ($publishedSceneBundles.Count -gt 0) {
        foreach ($fileName in @(
            'qiao.unity3d',
            'qiao.unity3d.manifest',
            'shaders.unity3d',
            'shaders.unity3d.manifest'
        )) {
            $null = $requiredExact.Add($fileName)
        }
    }

    $missing = New-Object 'System.Collections.Generic.List[string]'

    foreach ($fileName in $requiredExact) {
        if (-not (Test-ListContains -List $VersionFiles -FileName $fileName)) {
            $missing.Add($fileName)
        }
    }

    if ($requiredPrefixes.Count -gt 0) {
        foreach ($prefix in $requiredPrefixes) {
            $matchedFiles = @(
                Get-ChildItem -LiteralPath $BundlesPath -File |
                    Where-Object {
                        (Test-PublishableFileName -FileName $_.Name) -and
                        $_.Name.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase)
                    } |
                    ForEach-Object { $_.Name }
            )

            if ($matchedFiles.Count -eq 0) {
                $missing.Add("$prefix* (source missing)")
                continue
            }

            foreach ($fileName in $matchedFiles) {
                if (-not (Test-ListContains -List $VersionFiles -FileName $fileName)) {
                    $missing.Add($fileName)
                }

                $manifestName = "$fileName.manifest"
                if (Test-Path -LiteralPath (Join-Path $BundlesPath $manifestName)) {
                    if (-not (Test-ListContains -List $VersionFiles -FileName $manifestName)) {
                        $missing.Add($manifestName)
                    }
                }
            }
        }
    }

    if ($missing.Count -gt 0) {
        $details = ($missing | Sort-Object -Unique) -join ', '
        throw "Publish validation failed. Missing required rendering bundles: $details"
    }
}

function Restore-LockedMainUiBundles {
    param(
        [string]$SourceDir
    )

    if (-not (Test-Path -LiteralPath $lockedMainUiConfigPath)) {
        throw "Locked main UI config not found: $lockedMainUiConfigPath"
    }

    if (-not (Test-Path -LiteralPath $lockedMainUiSnapshotDir)) {
        throw "Locked main UI snapshot directory not found: $lockedMainUiSnapshotDir"
    }

    $lockedFiles = @(
        "uimaincanvas.unity3d",
        "uimaincanvas.unity3d.manifest",
        "ui_hud.unity3d",
        "ui_hud.unity3d.manifest"
    )

    foreach ($fileName in $lockedFiles) {
        $snapshotPath = Join-Path $lockedMainUiSnapshotDir $fileName
        if (-not (Test-Path -LiteralPath $snapshotPath)) {
            throw "Locked main UI bundle snapshot missing: $snapshotPath"
        }

        Copy-Item -LiteralPath $snapshotPath -Destination (Join-Path $SourceDir $fileName) -Force
    }

    Write-Host ("LOCKED_MAIN_UI_RESTORED={0}" -f $lockedMainUiSnapshotDir)
}

function Restore-LockedLieFengUiBundles {
    param(
        [string]$SourceDir
    )

    if (-not (Test-Path -LiteralPath $lockedLieFengUiConfigPath)) {
        throw "Locked LieFeng UI config not found: $lockedLieFengUiConfigPath"
    }

    if (-not (Test-Path -LiteralPath $lockedLieFengUiSnapshotDir)) {
        throw "Locked LieFeng UI snapshot directory not found: $lockedLieFengUiSnapshotDir"
    }

    $copiedFiles = New-Object 'System.Collections.Generic.List[string]'
    $snapshotFiles = @(
        Get-ChildItem -LiteralPath $lockedLieFengUiSnapshotDir -File |
            Where-Object {
                (Test-PublishableFileName -FileName $_.Name) -and
                $_.Name -match '\.(unity3d|manifest)$'
            } |
            Sort-Object Name
    )

    if ($snapshotFiles.Count -eq 0) {
        throw "Locked LieFeng UI snapshot is empty: $lockedLieFengUiSnapshotDir"
    }

    foreach ($file in $snapshotFiles) {
        Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $SourceDir $file.Name) -Force
        Add-UniqueFile -List $copiedFiles -FileName $file.Name
    }

    Write-Host ("LOCKED_LIEFENG_UI_RESTORED={0} FILES={1}" -f $lockedLieFengUiSnapshotDir, $copiedFiles.Count)
    return $copiedFiles
}

$versionFiles = New-Object 'System.Collections.Generic.List[string]'
foreach ($fileName in $coreVersionFiles) {
    Add-UniqueFile -List $versionFiles -FileName $fileName
}

Restore-LockedMainUiBundles -SourceDir $sourceDir
$lockedLieFengUiFiles = Restore-LockedLieFengUiBundles -SourceDir $sourceDir
foreach ($fileName in $lockedLieFengUiFiles) {
    Add-UniqueFile -List $versionFiles -FileName $fileName
}

if (Test-Path -LiteralPath $criticalBundleListPath) {
    foreach ($fileName in Get-Content -LiteralPath $criticalBundleListPath) {
        $trimmedName = $fileName.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmedName) -or $trimmedName.StartsWith('#')) {
            continue
        }

        Add-UniqueFile -List $versionFiles -FileName $trimmedName
    }
}

if (-not $CodeOnly) {
    $sceneNames = Get-SceneNames -ProjectRoot $projectRoot
    foreach ($fileName in Get-CategorizedPatchFiles -BundlesPath $sourceDir -Categories $PublishCategories -SceneNames $sceneNames) {
        Add-UniqueFile -List $versionFiles -FileName $fileName
    }
}
else {
    $sceneNames = Get-SceneNames -ProjectRoot $projectRoot
}

Assert-RequiredPublishedFiles -BundlesPath $sourceDir -VersionFiles $versionFiles -SceneNames $sceneNames

foreach ($targetDir in $targets) {
    if (-not (Test-Path -LiteralPath $targetDir)) {
        throw "Target directory not found: $targetDir"
    }

    foreach ($fileName in $versionFiles) {
        $sourcePath = Join-Path $sourceDir $fileName
        $targetPath = Join-Path $targetDir $fileName

        if (-not (Test-Path -LiteralPath $sourcePath)) {
            throw "Source file not found: $sourcePath"
        }

        Copy-Item -LiteralPath $sourcePath -Destination $targetPath -Force
    }

    $dict = [ordered]@{}
    $total = [int64]0
    $fingerprints = New-Object 'System.Collections.Generic.List[string]'

    foreach ($fileName in $versionFiles) {
        $path = Join-Path $targetDir $fileName
        if (-not (Test-Path -LiteralPath $path)) {
            throw "Version file not found: $path"
        }

        $file = Get-Item -LiteralPath $path
        $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm MD5).Hash.ToLowerInvariant()
        $dict[$file.Name] = [ordered]@{
            File = $file.Name
            MD5 = $hash
            Size = [int64]$file.Length
        }

        $total += [int64]$file.Length
        $fingerprints.Add(("{0}|{1}|{2}" -f $file.Name, $hash, [int64]$file.Length))
    }

    $version = Get-StableVersion -Fingerprints $fingerprints

    $versionConfig = [ordered]@{
        Version = $version
        TotalSize = $total
        FileInfoDict = $dict
    }

    $json = $versionConfig | ConvertTo-Json -Depth 6 -Compress
    [System.IO.File]::WriteAllText((Join-Path $targetDir "Version.txt"), $json, $utf8NoBom)

    Write-Host ("SYNCED={0} VERSION={1} FILES={2} SIZE={3}" -f $targetDir, $version, $versionFiles.Count, $total)

    if (-not $SkipAngeBaselineReport) {
        if (-not (Test-Path -LiteralPath $AngeApkPath)) {
            throw "ange baseline APK not found: $AngeApkPath"
        }

        if (-not (Test-Path -LiteralPath $angeCompareScript)) {
            throw "ange compare script not found: $angeCompareScript"
        }

        $label = (($targetDir.Substring($projectRoot.Length)).TrimStart('\') -replace '[\\/: ]', '-')
        if ([string]::IsNullOrWhiteSpace($label)) {
            $label = "StreamingAssets"
        }

        $report = & $angeCompareScript `
            -ProjectRoot $projectRoot `
            -ApkPath $AngeApkPath `
            -CurrentPath $targetDir `
            -ReportRoot (Join-Path $projectRoot "Tools\UIClone\ange\reports") `
            -ReportLabel $label `
            -FailOnCriticalDiff:$FailOnAngeCriticalDiff

        if ($null -ne $report) {
            Write-Host ("ANGE_BASELINE_STATUS={0} LABEL={1} CRITICAL_DIFFS={2} REPORT={3}" -f $report.GateStatus, $report.Label, $report.CriticalDiffCount, $report.ReportDirectory)
        }
    }
}
