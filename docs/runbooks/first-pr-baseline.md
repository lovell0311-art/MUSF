# First PR Baseline

## Target

- Base branch: `main`
- Head branch: `codex/git-baseline-import`
- Current head: `93eccf272`

## PR Title

```text
chore: import musf workflow and source baseline
```

## PR Summary

```text
- bootstrap Git, GitHub templates, workflow docs, and runbooks
- import MUSF automation scripts and guarded publish tooling
- track server Unity, server config, and operational helper source
- track client Unity scene, map, runtime, UI, effects, models, and art baselines
- add ignore rules for generated outputs, local caches, work copies, and release artifacts
- leave existing local edits in chooserole governance, release checklist, and publish flow out of this baseline
```

## Reviewer Notes

- This PR is a baseline import, not a feature change.
- Most diff volume comes from previously unmanaged Unity assets and resource trees.
- The main review focus should be:
  - ignore rules are correct
  - required source trees are now tracked
  - generated artifacts and local caches remain excluded
  - guarded publish and workflow docs are present

## Known Local Dirty Files

These were intentionally not included in the baseline branch final commits:

- `docs/chooserole-variant-governance.md`
- `docs/runbooks/release-candidate-checklist.md`
- `scripts/python/musf_tools/publish.py`

## Recommended Merge Sequence

1. Merge this baseline PR first.
2. Rebase or restack follow-up workflow changes on top of the merged baseline.
3. Submit the 3 local dirty files as a separate focused PR if still needed.

## Post-Merge Checks

```text
- confirm branch protection and PR template are active
- confirm no generated Unity or build output directories entered Git
- confirm future changes stay inside approved write scope
```
