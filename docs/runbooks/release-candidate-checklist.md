# Release Candidate Checklist

Use this checklist before guarded publish of any medium or high-risk MUSF change.

## 1. Task readiness

- A task spec exists.
- The write scope is explicit.
- Forbidden scope is explicit.
- The owner of the candidate is known.

## 2. Workspace sanity

- Work was performed in the correct writable source root.
- No shadow or generated client tree was used as the primary edit target.
- Required comparison inputs and baselines are identified.

## 3. Toolchain preflight

- `scripts\run-musf.cmd verify-toolchain`
- required tools resolve correctly
- any fallback use is documented and accepted

## 4. Environment checks

- `scripts\run-musf.cmd sync-network-config`
- `scripts\run-musf.cmd server-health`
- device or emulator availability confirmed if runtime gate is required

## 5. Protected asset checks

Run this section if protected login-stack assets or `chooserole` are involved.

- protected files are intentionally changed, not accidentally changed
- current `chooserole` variant is inspected
- unregistered variants are blocked
- blocked variants remain blocked until verification passes
- raw bundle replacement was not used as a shortcut

## 6. Build checks

- `scripts\run-musf.cmd build-android` if an Android build is required
- build log path recorded
- build output report recorded

## 7. Runtime gate

- `scripts\run-musf.cmd run-gate`
- canary login succeeds
- main HUD is reachable
- required panels open successfully
- logcat fatal summary is clean enough for promotion

## 8. Evidence folder

The candidate has:

- task spec reference
- changed files summary
- build log
- gate report
- variant report if applicable
- screenshots
- rollback source

## 9. Publish decision

- publish only through guarded publish logic
- mirror targets are known
- rollback source is ready before publish starts

## 10. Closeout

- candidate result is recorded as promoted, blocked, or rejected
- final evidence paths are attached to the task or PR
- any follow-up risk is added to the backlog
