# Chooserole Variant Governance

`chooserole.unity3d` has multiple historical variants with the same bundle name. Publishing by bundle filename alone is forbidden.

## Rules

1. Treat `F:\MUSF\configs\chooserole-variants.json` as the source of truth for all known `chooserole.unity3d` variants.
2. Only a variant marked `Allowed` may be published by guarded hotfix workflows.
3. Variants marked `ReferenceOnly` may be used to recover scene structure into source, but must not be pushed as raw bundles.
4. Variants marked `Blocked` must never be copied into live `StreamingAssets`.
5. `ChooseRole.unity` source recovery must happen at the scene level, then `chooserole.unity3d` must be rebuilt from source.

## Current State

- Current project scene: `F:\MUSF\Client\Unity\Assets\Scenes\GameMap\ChooseRole.unity`
- Current source marker: `ChooseRoleGrassOverlay` restored in source
- Structural reference scene: `F:\MUSF\backups\choose-role-icebaseline-20260414-182106\ChooseRole.unity`
- Bad restore source: `E:\MUNEW\Client\Unity\Assets\Scenes\GameMap\ChooseRole.unity`
- Current approved chooserole variant: `source_rebuild_candidate_68eafd3c` (`68eafd3c5c4408223b80f80f22ce42d0`)

## Required Workflow

1. Identify the current `chooserole.unity3d` variant with `report-chooserole-variants`.
2. If the variant is not `Allowed`, stop.
3. Repair `ChooseRole.unity` source objects and materials.
4. Rebuild `chooserole.unity3d` from source.
5. Register the rebuilt variant in `chooserole-variants.json` as `Blocked` until runtime verification is complete.
6. Promote the variant to `Allowed` only after controlled verification passes.
7. Only then allow guarded publish to proceed.

## Hotfix Sync Rules

`chooserole` must not be synced as a single bundle in isolation.

1. The only stable Android hotfix directory is `/storage/emulated/0/Android/data/com.quanzhilieshou.bytedance.gamecenter/files/Android/星辰战纪`.
2. Publish and sync tooling must stay aligned with `ETModel.PathHelper.PreferredHotfixFolderName` in [PathHelper.cs](F:/MUSF/Client/Unity/Assets/Model/Helper/PathHelper.cs:7).
3. `sync-hotfix-to-devices` must expand `chooserole` requests into a dependency closure instead of pushing only the requested filename.
4. Dependency expansion must combine:
   - `StreamingAssets.manifest`
   - per-bundle `.manifest`
   - explicit runtime bundle groups from `F:\MUSF\configs\sync-bundle-groups.json`
5. Manual copying into multiple hotfix directories is emergency-only and must not be the normal release path.
6. Every `chooserole`-related bundle group in `sync-bundle-groups.json` must include runtime evidence paths, and guarded publish must block if those evidence files are missing.
7. `sync-bundle-groups.json` evidence entries must use one of these types only: `runtime-log`, `sync-report`, `probe-report`.
8. Every `chooserole`-related bundle group must declare `Owner`, `WhyNeeded`, and `FailureIfMissing` so the runtime dependency family has an explicit maintainer, rationale, and failure description.
9. `Owner` must stay within the controlled sync bundle group owner set. The current chooserole owner is `choose-role`.
10. The first `Members` entry for each chooserole-related bundle group must equal the group key itself, so the primary bundle is explicit and stable.
11. Every chooserole-related `Members` entry must exist in the validated `StreamingAssets` source directory, or guarded publish must block.

## Current Sync Closure

When `chooserole.unity3d` is requested for device sync, the current toolchain expands the request to include at least:

- `chooserole.unity3d`
- `uichooserole.unity3d`
- `ui_common_btnss.unity3d`
- `ui_common_textss.unity3d`
- `common_matss.unity3d`
- `suit_shaderss.unity3d`
- `suit_effect_texs.unity3d`
- `skins.unity3d`

and each bundle's matching `.manifest` file.

## Current Verification Record

- Device: `127.0.0.1:16384`
- Remote hotfix dir: `/storage/emulated/0/Android/data/com.quanzhilieshou.bytedance.gamecenter/files/Android/星辰战纪`
- Approved bundle MD5: `68eafd3c5c4408223b80f80f22ce42d0`
- Approved manifest MD5: `655c97d2cd5ff3aed429868cf3a63f08`
- Runtime evidence: the app loaded `chooserole.unity3d` and `uichooserole.unity3d` from the `星辰战纪` hotfix directory during choose-role entry with no new fatal exception in the captured logcat.
