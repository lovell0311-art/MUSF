param(
    [string]$RootPath = (Join-Path (Join-Path $PSScriptRoot "..") "Release"),
    [string[]]$Prefixes = @("http://+:8080/", "http://+:8088/")
)

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Web

function Get-ContentType {
    param([string]$Path)

    switch ([IO.Path]::GetExtension($Path).ToLowerInvariant()) {
        ".txt" { return "text/plain" }
        ".json" { return "application/json" }
        ".xml" { return "application/xml" }
        ".html" { return "text/html" }
        ".htm" { return "text/html" }
        ".png" { return "image/png" }
        ".jpg" { return "image/jpeg" }
        ".jpeg" { return "image/jpeg" }
        ".gif" { return "image/gif" }
        default { return "application/octet-stream" }
    }
}

$listener = [System.Net.HttpListener]::new()
foreach ($prefix in $Prefixes) {
    $listener.Prefixes.Add($prefix)
}

$rootFullPath = [IO.Path]::GetFullPath($RootPath)
$updateVersionScript = Join-Path $PSScriptRoot "Rebuild-UpdateVersion.ps1"

if (Test-Path -LiteralPath $updateVersionScript -PathType Leaf) {
    try {
        $updateRoot = Join-Path $rootFullPath "update"
        if (Test-Path -LiteralPath $updateRoot -PathType Container) {
            & $updateVersionScript -RootPath $updateRoot | ForEach-Object { Write-Output $_ }
        }
    }
    catch {
        Write-Warning ("Rebuild-UpdateVersion failed: {0}" -f $_.Exception.Message)
    }
}

$listener.Start()

Write-Output ("Static file server serving {0}" -f $rootFullPath)
Write-Output ("Prefixes: {0}" -f ($Prefixes -join ", "))

while ($listener.IsListening) {
    $context = $listener.GetContext()
    try {
        $requestPath = [System.Web.HttpUtility]::UrlDecode($context.Request.Url.AbsolutePath.TrimStart('/').Replace('/', '\'))
        if ([string]::IsNullOrWhiteSpace($requestPath)) {
            $buffer = [System.Text.Encoding]::UTF8.GetBytes("MUSF static file server")
            $context.Response.ContentType = "text/plain"
            $context.Response.StatusCode = 200
            $context.Response.OutputStream.Write($buffer, 0, $buffer.Length)
            continue
        }

        $fullPath = [IO.Path]::GetFullPath((Join-Path $rootFullPath $requestPath))
        if (-not $fullPath.StartsWith($rootFullPath, [System.StringComparison]::OrdinalIgnoreCase)) {
            $context.Response.StatusCode = 403
            continue
        }

        if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
            $context.Response.StatusCode = 404
            continue
        }

        $fileInfo = Get-Item -LiteralPath $fullPath
        $context.Response.ContentType = Get-ContentType -Path $fullPath
        $context.Response.ContentLength64 = $fileInfo.Length
        $stream = [IO.File]::OpenRead($fullPath)
        try {
            $stream.CopyTo($context.Response.OutputStream)
        }
        finally {
            $stream.Dispose()
        }
    }
    catch {
        $context.Response.StatusCode = 500
        $buffer = [System.Text.Encoding]::UTF8.GetBytes($_.Exception.ToString())
        $context.Response.OutputStream.Write($buffer, 0, $buffer.Length)
    }
    finally {
        $context.Response.OutputStream.Close()
    }
}
