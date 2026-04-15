param(
    [string]$AssetRoot = "",
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "musf-common.ps1")

$root = Get-MUSFRoot
if (-not $AssetRoot) {
    $AssetRoot = Join-Path $root "_apktool_20260310\assets"
}

if (-not $OutputPath) {
    $OutputPath = Join-Path $root "apk-ui-baseline-manifest.json"
}

$explicitNames = @(
    "uimaincanvas.unity3d",
    "ui_hud.unity3d",
    "ui_mainpanels.unity3d",
    "ui_skills.unity3d",
    "minmap.unity3d"
)

$files = Get-ChildItem -Path $AssetRoot -File |
    Where-Object {
        ($_.Name -like "ui*.unity3d" -or $_.Name -like "minmap*.unity3d" -or $explicitNames -contains $_.Name) -and
        $_.FullName -notmatch "backup-codebundle-"
    } |
    Sort-Object Name

function Get-Purpose {
    param([string]$Name)

    switch -Regex ($Name) {
        "^uimaincanvas\.unity3d$" { return "Main HUD canvas baseline" }
        "^ui_hud\.unity3d$" { return "HUD sub-layout baseline" }
        "^ui_mainpanels\.unity3d$" { return "Primary gameplay panel baseline" }
        "^ui_skills\.unity3d$" { return "Skills panel baseline" }
        "^minmap.*\.unity3d$" { return "Minimap baseline" }
        "^uilogin.*\.unity3d$" { return "Login UI baseline" }
        "^uiselectserver.*\.unity3d$" { return "Server select baseline" }
        "^uichooserole.*\.unity3d$" { return "Role select baseline" }
        "^ui.*\.unity3d$" { return "Frozen APK UI baseline" }
        default { return "APK baseline asset" }
    }
}

$entries = foreach ($file in $files) {
    $hash = Get-FileHash -Algorithm MD5 -Path $file.FullName
    [PSCustomObject]@{
        filename = $file.Name
        size = $file.Length
        md5 = $hash.Hash.ToLowerInvariant()
        purpose = Get-Purpose -Name $file.Name
        frozen = $true
        sourcePath = $file.FullName
    }
}

$manifest = [PSCustomObject]@{
    schemaVersion = 1
    generatedAt = (Get-Date).ToString("yyyy-MM-ddTHH:mm:sszzz")
    sourceRoot = $AssetRoot
    fileCount = @($entries).Count
    entries = $entries
}

Write-MUSFJson -InputObject $manifest -Path $OutputPath
Write-Host ("MANIFEST={0}" -f $OutputPath)
Write-Host ("COUNT={0}" -f @($entries).Count)
