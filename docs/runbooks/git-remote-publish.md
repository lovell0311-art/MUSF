# Git Remote Publish Runbook

## Current Baseline

- Branch: `codex/git-baseline-import`
- Head: `d047b31ae`
- Baseline commits after bootstrap: `25`
- Untracked files: `0`

## Local Dirty Files

These files were intentionally left untouched and should be reviewed before pushing or opening the first PR:

- `docs/chooserole-variant-governance.md`
- `docs/runbooks/release-candidate-checklist.md`
- `scripts/python/musf_tools/publish.py`

## Add Remote

```powershell
git remote add origin <github-repo-url>
git remote -v
```

Examples:

```powershell
git remote add origin git@github.com:<owner>/<repo>.git
git remote add origin https://github.com/<owner>/<repo>.git
```

## Push Branch

```powershell
git push -u origin codex/git-baseline-import
```

## Recommended First PR

Base branch:

```text
main
```

PR title:

```text
chore: import musf workflow and source baseline
```

PR summary:

```text
- bootstrap Git/GitHub workflow files and runbooks
- import MUSF automation scripts and guarded publish pipeline
- track core server/client Unity source baselines
- track scene/resource/effect/runtime asset baselines
- add ignore rules for generated outputs, local caches, and work copies
- leave local user edits in chooserole governance, release checklist, and publish flow uncommitted
```

## Post-Push Checks

```powershell
git fetch origin
git status --short
git branch -vv
```

Expected result:

- branch tracks `origin/codex/git-baseline-import`
- only the 3 local dirty files remain modified
- no untracked files remain

## Follow-Up

After the first PR is open:

1. Decide whether the 3 local dirty files belong in this PR or a follow-up PR.
2. Add branch protection and required PR templates on the remote repository.
3. Continue with targeted workflow hardening instead of more bulk asset imports.
