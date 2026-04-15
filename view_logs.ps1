# MUSF 日志查看脚本
# 使用方法: powershell -ExecutionPolicy Bypass -File F:\MUSF\view_logs.ps1

$adbPath = "C:\Android\Sdk\platform-tools\adb.exe"
$device = "127.0.0.1:7555"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "MUSF 实时日志查看器" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "按 Ctrl+C 停止查看" -ForegroundColor Yellow
Write-Host ""

$patterns = @(
    "LoginStage",
    "Bundle",
    "uilogin",
    "ui_common",
    "ui_logins",
    "UILogin",
    "Error",
    "Exception",
    "ILRuntime"
)

$patternStr = $patterns -join "|"
Write-Host "过滤模式: $patternStr" -ForegroundColor Gray
Write-Host ""

# 启动日志监控
& $adbPath -s $device logcat -v time | ForEach-Object {
    $line = $_
    # 高亮显示关键信息
    if ($line -match "LoginStage") {
        Write-Host $line -ForegroundColor Green
    } elseif ($line -match "uilogin|ui_common|ui_logins") {
        Write-Host $line -ForegroundColor Cyan
    } elseif ($line -match "Error|Exception") {
        Write-Host $line -ForegroundColor Red
    } elseif ($line -match "Bundle") {
        Write-Host $line -ForegroundColor Yellow
    } elseif ($line -match $patternStr) {
        Write-Host $line -ForegroundColor White
    }
}
