param(
    [string]$MongoBinRoot = "F:\MUSF\Tools\MongoDB\MongoDB\MongoDB\Server\4.0\bin",
    [string]$MongoUri = "mongodb://127.0.0.1:58030",
    [string]$OutputRoot = (Join-Path (Join-Path $PSScriptRoot "..") "Deploy"),
    [string]$ArchiveName = ""
)

$ErrorActionPreference = "Stop"

$mongoDumpExe = Join-Path $MongoBinRoot "mongodump.exe"
if (-not (Test-Path -LiteralPath $mongoDumpExe)) {
    throw "mongodump.exe not found: $mongoDumpExe"
}

if (-not (Test-Path -LiteralPath $OutputRoot)) {
    New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null
}

if ([string]::IsNullOrWhiteSpace($ArchiveName)) {
    $ArchiveName = "mongo-{0}.archive.gz" -f (Get-Date -Format "yyyyMMdd-HHmmss")
}

$archivePath = Join-Path $OutputRoot $ArchiveName
& $mongoDumpExe "--uri=$MongoUri" "--archive=$archivePath" "--gzip"

if (-not (Test-Path -LiteralPath $archivePath)) {
    throw "Mongo backup was not created: $archivePath"
}

$item = Get-Item -LiteralPath $archivePath
Write-Output ("Mongo backup created: {0}" -f $item.FullName)
Write-Output ("Size: {0} bytes" -f $item.Length)
