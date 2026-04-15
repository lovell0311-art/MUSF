# Hotfix Sync Governance

Use this runbook when syncing Android hotfix bundles to devices.

## Rules

1. Sync to one stable hotfix directory only.
   Current stable directory: `/storage/emulated/0/Android/data/com.quanzhilieshou.bytedance.gamecenter/files/Android/星辰战纪`
2. Tooling defaults must stay aligned with `ETModel.PathHelper.PreferredHotfixFolderName` in [PathHelper.cs](F:/MUSF/Client/Unity/Assets/Model/Helper/PathHelper.cs:7).
3. Do not treat manual copy into multiple hotfix directories as the standard workflow.
4. Sync requests for `.unity3d` bundles must expand to a dependency closure before ADB push.
5. Dependency closure must combine:
   - bundle-group seeds from [sync-bundle-groups.json](F:/MUSF/configs/sync-bundle-groups.json)
   - `StreamingAssets.manifest`
   - per-bundle `.manifest`
6. Device sync reports must record:
   - requested files
   - expanded files
   - selection reasons
   - bundle-group registry path
   - streaming manifest path
7. Every bundle group in [sync-bundle-groups.json](F:/MUSF/configs/sync-bundle-groups.json) must carry evidence paths that already exist on disk.
8. Guarded publish must block if any configured bundle group is missing evidence or points at a non-existent evidence file.
9. `Evidence.Type` is restricted to `runtime-log`, `sync-report`, or `probe-report`.
10. Every bundle group must declare:
   - `Owner`: the subsystem or feature owner
   - `WhyNeeded`: why the runtime family exists and why manifest-only closure is not sufficient
   - `FailureIfMissing`: what breaks if the family is not synced as a closure
11. `Owner` is restricted to a controlled set.
   Current allowed owners: `choose-role`, `login-stack`, `main-ui`, `map-runtime`
12. The first entry in `Members` must equal the bundle-group key itself. That first member is the canonical primary bundle for the group.
13. Every `Members` entry must exist as a real file in the target `StreamingAssets` directory used for validation or publish.

## Current Tooling

- Sync command entry: `scripts\run-musf.cmd --profile local-lan sync-hotfix-to-devices`
- Implementation: [publish.py](F:/MUSF/scripts/python/musf_tools/publish.py)
- Bundle-group registry: [sync-bundle-groups.json](F:/MUSF/configs/sync-bundle-groups.json)
- Bundle-group gate: `validate_sync_bundle_group_registry()` in [publish.py](F:/MUSF/scripts/python/musf_tools/publish.py)

## Expected Behavior

If the caller requests only `chooserole.unity3d`, the sync tool must automatically include:

- `chooserole.unity3d`
- `uichooserole.unity3d`
- `ui_common_btnss.unity3d`
- `ui_common_textss.unity3d`
- `common_matss.unity3d`
- `suit_shaderss.unity3d`
- `suit_effect_texs.unity3d`
- `skins.unity3d`

plus the matching `.manifest` files.

## Failure Pattern To Avoid

The main scene bundle can be present in the hotfix directory while runtime-shared material or shader bundles still fall back to `base.apk!/assets`. When that happens, visual regressions can persist even though the primary scene bundle is correct.

## Release Rule

If a hotfix depends on runtime-shared bundles and the dependency closure was not synced, the candidate is not ready for promotion.
