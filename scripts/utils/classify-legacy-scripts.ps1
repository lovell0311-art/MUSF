param(
    [string]$NewProjectRoot = "C:\Users\ZM\Documents\New project",
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "musf-common.ps1")

$root = Get-MUSFRoot
if (-not $OutputPath) {
    $OutputPath = Join-Path $root "archive\inventory\script-classification.md"
}

$sources = @(
    $NewProjectRoot
    (Join-Path $root "Client\Unity")
    (Join-Path $root "Server")
    (Join-Path $root "Tools")
) | Where-Object { Test-Path $_ }

$exactMigrationMap = @{
    "publish_code_bundle.ps1" = "F:\MUSF\scripts\build\publish-code-bundle.ps1"
    "publish_hotfix_guarded.ps1" = "F:\MUSF\scripts\build\publish-hotfix-guarded.ps1"
    "generate_version_txt.ps1" = "F:\MUSF\scripts\build\generate-version-txt.ps1"
    "compare_musf_ui_assets.ps1" = "F:\MUSF\scripts\utils\compare-musf-ui-assets.ps1"
    "sync_musf_hotfix_to_devices.ps1" = "F:\MUSF\scripts\deploy\sync-hotfix-to-devices.ps1"
    "start_webgm_background.ps1" = "F:\MUSF\scripts\deploy\start-webgm-background.ps1"
    "build_android_loginfix.bat" = "F:\MUSF\scripts\build\build-android.ps1 (reference only)"
    "运行.bat" = "F:\MUSF\scripts\server-start.ps1"
}

function Get-Classification {
    param([System.IO.FileInfo]$File)

    if ($exactMigrationMap.ContainsKey($File.Name)) {
        return [PSCustomObject]@{
            Category = "preserve-migrate"
            Target = $exactMigrationMap[$File.Name]
            AllowedNow = "yes"
            Notes = "Formal entrypoint or direct reference source."
        }
    }

    if ($File.Name -like "build_musf_test_ui_clone_*.ps1") {
        return [PSCustomObject]@{
            Category = "merge-rewrite"
            Target = "F:\MUSF\scripts\build\build-ui-clone-variants.ps1"
            AllowedNow = "no"
            Notes = "Historical variants should collapse into one parameterized entrypoint."
        }
    }

    if ($File.Name -like "*ensure*network*config*.ps1" -or $File.FullName -like "*New project*") {
        return [PSCustomObject]@{
            Category = "archive-isolate"
            Target = "F:\MUSF\archive\new-project-src\"
            AllowedNow = "no"
            Notes = "Temporary or side-effect-heavy script. Do not call directly from the formal flow."
        }
    }

    return [PSCustomObject]@{
        Category = "archive-isolate"
        Target = "F:\MUSF\archive\inventory\"
        AllowedNow = "no"
        Notes = "Needs manual review before promotion."
    }
}

$files = foreach ($source in $sources) {
    Get-ChildItem -Path $source -Recurse -File -Include *.ps1, *.bat, *.cmd
}

$rows = foreach ($file in ($files | Sort-Object FullName -Unique)) {
    $classification = Get-Classification -File $file
    [PSCustomObject]@{
        SourcePath = $file.FullName
        Category = $classification.Category
        ProposedTarget = $classification.Target
        AllowedNow = $classification.AllowedNow
        Notes = $classification.Notes
    }
}

$lines = @(
    "# Script Classification",
    "",
    "- GeneratedAt: " + (Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
    "- MUSFRoot: " + $root,
    "- NewProjectRoot: " + $NewProjectRoot,
    "",
    "| SourcePath | Category | ProposedTarget | AllowedNow | Notes |",
    "| --- | --- | --- | --- | --- |"
)

foreach ($row in $rows) {
    $escaped = @(
        $row.SourcePath,
        $row.Category,
        $row.ProposedTarget,
        $row.AllowedNow,
        $row.Notes
    ) | ForEach-Object { ($_ -replace '\|', '\\|') }

    $lines += "| {0} | {1} | {2} | {3} | {4} |" -f $escaped[0], $escaped[1], $escaped[2], $escaped[3], $escaped[4]
}

$parent = Split-Path -Parent $OutputPath
if (-not (Test-Path $parent)) {
    New-Item -ItemType Directory -Path $parent | Out-Null
}

[System.IO.File]::WriteAllLines($OutputPath, $lines, [System.Text.Encoding]::UTF8)
Write-Host ("REPORT={0}" -f $OutputPath)
Write-Host ("COUNT={0}" -f @($rows).Count)
