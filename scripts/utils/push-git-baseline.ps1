param(
    [Parameter(Mandatory = $true)]
    [string]$RemoteUrl,

    [string]$RemoteName = "origin",

    [string]$BranchName = "codex/git-baseline-import"
)

$ErrorActionPreference = "Stop"

function Ensure-GitRepo {
    git rev-parse --is-inside-work-tree 1>$null
}

function Get-CurrentBranch {
    (git branch --show-current).Trim()
}

function Get-ExistingRemoteUrl([string]$name) {
    $url = git remote get-url $name 2>$null
    if ($LASTEXITCODE -ne 0) {
        return $null
    }

    return $url.Trim()
}

Ensure-GitRepo

$currentBranch = Get-CurrentBranch
if ($currentBranch -ne $BranchName) {
    throw "Current branch is '$currentBranch'. Expected '$BranchName'."
}

$existingRemote = Get-ExistingRemoteUrl $RemoteName
if ($null -eq $existingRemote) {
    git remote add $RemoteName $RemoteUrl
}
elseif ($existingRemote -ne $RemoteUrl) {
    throw "Remote '$RemoteName' already exists and points to '$existingRemote'."
}

Write-Host "Remote '$RemoteName' => $RemoteUrl"
git push -u $RemoteName $BranchName
