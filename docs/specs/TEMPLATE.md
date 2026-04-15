# Task Spec Template

## Metadata

- Title:
- Date:
- Owner:
- Risk Level: low / medium / high
- Related Issue:

## Goal

Describe the exact outcome this task should achieve.

## Scope

List the files, modules, assets, or workflows that may be changed.

## Forbidden Scope

List paths, assets, or workflows that must not be changed.

## Write Roots

- Primary write root:
- Additional write roots:

## Read-Only Inputs

List comparison roots, reference bundles, backup scenes, logs, screenshots, or baseline files used by this task.

## Acceptance Criteria

- functional result
- visual result if applicable
- required logs or JSON reports
- required screenshots if applicable

## Validation Commands

List the commands that must be run before the task is considered complete.

Example:

```powershell
scripts\run-musf.cmd verify-toolchain
scripts\run-musf.cmd server-health
scripts\run-musf.cmd run-gate
```

## Expected Artifacts

List the output paths that should exist after the task is validated.

## Rollback Plan

Describe how to revert the change or restore the previous candidate.

## Notes

Capture assumptions, open risks, and operator instructions.
