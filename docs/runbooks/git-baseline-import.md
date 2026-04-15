# Git Baseline Import Runbook

Use this runbook to bring `F:\MUSF` under Git without pulling heavy runtime outputs into the first commit.

## Goal

Track source, scripts, configs, docs, templates, and selected tools.

Do not track:

- APKs
- reports
- backups
- runtime mirrors
- generated Unity clones
- local toolchains
- screenshots and loose diagnostic artifacts

## First import scope

Recommended first-pass tracked roots:

- `docs`
- `configs`
- `scripts`
- `env-profiles`
- `templates`
- `skills`
- `Client\Unity`
- `Server\Config`
- `Server\FileServer`
- `Server\Server`
- `Server\Tools`
- `Tools\BundlePatch`
- `Tools\webgm`
- `Tools\webgm_deploy_stage`
- top-level workflow docs and manifests

## First import exclusions

Do not force-add these during the first baseline:

- `reports`
- `archive`
- `backups`
- `baseline`
- `toolchain`
- `Release_Test`
- `Server\Release`
- generated Unity clone trees
- APK extracts
- loose screenshots and logs under `Tools`

## Suggested commands

```powershell
git init -b main
git status --short
git add .gitignore .gitattributes .github docs configs scripts env-profiles templates skills toolchain-manifest.json apk-ui-baseline-manifest.json
git add Client/Unity
git add Server/Config Server/FileServer Server/Server Server/Tools
git add Tools/BundlePatch Tools/webgm Tools/webgm_deploy_stage
git status --short
```

## Review checkpoints

- confirm `Client\Unity\Library` is ignored
- confirm `reports` is ignored
- confirm `Release_Test` is ignored
- confirm `Server\Release` is ignored
- confirm loose `Tools\*.png` and `Tools\*.log` are ignored
- confirm primary source files are still visible to Git

## Before the first commit

- check for any remaining accidentally included large binaries
- check line ending normalization with `.gitattributes`
- verify no local credentials or device state files are staged
