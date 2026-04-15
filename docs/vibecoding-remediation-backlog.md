# MUSF Vibe Coding Remediation Backlog

## P0

### 1. Initialize source control for `F:\MUSF`

- Problem: the workspace is not currently a Git working tree, so GitHub planning and review cannot be fully enforced.
- Action: initialize or import the current workspace into Git with a clean ignore strategy for generated outputs, APKs, reports, caches, and toolchain binaries.
- Output: tracked source tree, `.gitignore`, initial baseline commit, branch policy.
- Acceptance: code and config changes can move through issue -> branch -> PR -> review.

### 2. Declare one editable client root

- Problem: multiple Unity copies make AI and human edits ambiguous.
- Action: formally declare `F:\MUSF\Client\Unity` as the only editable client source root.
- Output: workspace policy doc and guard checks in automation.
- Acceptance: no routine task writes into `UnityBatchShadow` or `UnityCodeBundleMini*`.

### 3. Enforce protected publish path

- Problem: protected assets can still be mishandled if humans bypass formal publish logic.
- Action: make guarded publish the only supported publish path for protected login-stack assets.
- Output: operator rule, runbook, and command-level refusal for unsupported direct publish attempts.
- Acceptance: no protected file reaches live targets without gate evidence.

### 4. Keep `chooserole` candidate blocked until runtime proof

- Problem: `source_rebuild_candidate_68eafd3c` is registered but still blocked.
- Action: require controlled runtime verification before any promotion to `Allowed`.
- Output: verification report, screenshots, decision record.
- Acceptance: variant policy changes only after evidence exists.

## P1

### 5. Turn `verify-toolchain` into a hard preflight gate

- Problem: required tools still resolve from mixed formal and fallback paths.
- Action: block `build-android`, `run-gate`, and publish operations when required toolchain items are missing or drifted beyond allowed fallback rules.
- Output: stricter preflight policy.
- Acceptance: risky commands fail fast before build or publish starts.

### 6. Normalize toolchain onto `F:`

- Problem: Unity, ADB, JDK, Python, Node, and BundlePatch are not fully formalized on `F:`.
- Action: relocate or validate formal paths for all required toolchain items.
- Output: updated toolchain layout and refreshed manifest.
- Acceptance: required toolchain status resolves to installed from formal paths.

### 7. Standardize release evidence folders

- Problem: release evidence exists but is fragmented.
- Action: require a single run directory per candidate with spec, logs, screenshots, reports, and rollback info.
- Output: stable release artifact schema.
- Acceptance: one release can be audited from one folder.

### 8. Add task spec templates

- Problem: AI and humans can start work from under-specified requests.
- Action: add templates under `docs/specs/` and require them for medium and high-risk work.
- Output: reusable spec template.
- Acceptance: every protected-asset task starts from a written scope.

## P2

### 9. Extract hard-coded runtime assumptions into config

- Problem: package names, account defaults, hotfix folder names, and screen coordinates are embedded in scripts.
- Action: move stable runtime assumptions into config files with per-profile overrides where appropriate.
- Output: cleaner config boundaries.
- Acceptance: script changes are no longer required for simple environment adjustments.

### 10. Compact logs into AI-readable failure summaries

- Problem: large logs slow down triage and make iterative AI repair harder.
- Action: emit compact summaries for build failures, gate failures, and visual smoke mismatches.
- Output: summarized JSON or Markdown failure digest.
- Acceptance: one failure can be understood from a short structured report.

### 11. Add GitHub issue and PR templates

- Problem: even after Git is enabled, work items can remain vague.
- Action: add issue and PR templates that require risk level, scope, evidence, rollback, and screenshots for protected UI work.
- Output: `.github` templates.
- Acceptance: reviewers get the same structure on every change.

### 12. Label generated and shadow directories

- Problem: operators can mistake output directories for source directories.
- Action: add clear docs and optional sentinel files marking shadow/generated trees.
- Output: visible ownership markers.
- Acceptance: no ambiguity about which directories are writable sources.

## P3

### 13. Introduce self-hosted GitHub automation carefully

- Problem: local MUSF release flows depend on Windows, Unity, ADB, and LAN services.
- Action: only after local scripts stabilize, introduce a self-hosted runner for non-destructive verification tasks.
- Output: optional GitHub automation bridge.
- Acceptance: GitHub automation reuses local guard logic instead of duplicating it.

### 14. Add architecture decision records

- Problem: key decisions around baseline freezes, source-of-truth directories, and publish restrictions are scattered across scripts and notes.
- Action: create ADRs for major operating rules.
- Output: `docs/adr/`.
- Acceptance: future changes reference explicit decisions instead of tribal knowledge.

### 15. Build a MUSF AI task library

- Problem: repeated AI task prompts can drift and lose required context.
- Action: create a small library of approved task prompts for investigation, scene repair, release verification, and rollback analysis.
- Output: reusable prompt patterns grounded in MUSF rules.
- Acceptance: repeated AI tasks become more consistent and auditable.

## Recommended execution order

1. P0-1 initialize Git
2. P0-2 declare editable root
3. P0-3 enforce protected publish path
4. P0-4 finish `chooserole` candidate governance
5. P1-5 make preflight blocking
6. P1-6 normalize toolchain
7. P1-7 standardize evidence folders
8. P1-8 add task specs
9. P2 items after the release path is stable
