# MUSF Workspace Write Policy

## Purpose

This document defines which directories are writable sources, which directories are generated outputs, and which directories should be treated as evidence or archives.

The goal is to stop accidental edits across multiple Unity clones and to give AI tasks a clear write boundary.

## Writable source roots

These directories are the default allowed write roots for routine implementation work.

- `F:\MUSF\Client\Unity`
- `F:\MUSF\Server`
- `F:\MUSF\scripts`
- `F:\MUSF\configs`
- `F:\MUSF\docs`
- `F:\MUSF\skills`

## Generated or shadow roots

These directories are not routine edit targets. They may be rebuilt, compared, or refreshed by formal workflows, but they are not primary source roots.

- `F:\MUSF\Client\UnityBatchShadow`
- `F:\MUSF\Client\UnityCodeBundleMini`
- `F:\MUSF\Client\UnityCodeBundleMini+`
- `F:\MUSF\Client\UnityCodeBundleMini30Shadow`
- `F:\MUSF\Client\Unity_BuildClone_NavGrid_20260326_1201`
- `F:\MUSF\Release_Test`
- `F:\MUSF\Server\Release`
- `F:\MUSF\reports`
- `F:\MUSF\baseline`
- `F:\MUSF\backups`
- `F:\MUSF\archive`
- `F:\MUSF\tmp`
- `F:\MUSF\Temp`

## Special handling rules

### Client source

- `F:\MUSF\Client\Unity` is the only editable client source of truth.
- Any task that needs data from another client tree must treat that tree as comparison input, not as the main write target.

### Protected release targets

- `F:\MUSF\Server\Release\Android\StreamingAssets`
- `F:\MUSF\Release_Test\Android\StreamingAssets`
- `F:\MUSF\Server\Release\update\2.0TestGame\Android\StreamingAssets`

These directories must only be changed through guarded publish workflows.

### Evidence and archives

- Files under `reports`, `baseline`, `backups`, and `archive` are evidence or historical records.
- They may be added to by automation.
- They should not be silently rewritten to hide a failed experiment or to fake a clean baseline.

## AI task rules

Every AI task should state:

- write roots
- read-only comparison roots
- forbidden roots

Default rule:

- if a task does not explicitly require another write root, it should write only inside the primary source root plus `docs`, `configs`, or `scripts`

## Approval thresholds

### Normal changes

- docs
- scripts
- configs
- server source
- client source under `Client\Unity`

These can proceed under a bounded task spec.

### Restricted changes

- any release target
- any protected login-stack asset
- any `chooserole` publish candidate
- any task mutating device state or canary state

These require explicit validation evidence before promotion.
