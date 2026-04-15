# MUSF Vibe Coding Implementation Plan

## 1. Purpose

This document defines how MUSF should adopt GitHub-style vibe coding without losing release control.

The target model is not unconstrained AI automation. The target model is:

- spec-driven task definition
- bounded AI execution
- automated guard checks
- evidence-based review
- canary verification before publish
- deterministic rollback when verification fails

This plan is based on the current MUSF workspace state on `2026-04-15`.

## 2. External Principles Mapped To MUSF

GitHub and adjacent GitHub projects consistently point toward the same practical pattern:

- natural language is good for intent capture, not for replacing release discipline
- strong results come from planning-driven and test-driven workflows
- agents need owned prompts, owned context, explicit tools, compact error feedback, and small scopes
- production changes need structured state, not chat-only memory

For MUSF, that means:

1. AI can help define, inspect, patch, rebuild, compare, and verify.
2. AI must not directly publish uncontrolled bundles or bypass runtime gates.
3. Every AI task must start from a written spec and end with machine-readable evidence.

## 3. Current MUSF Baseline

### 3.1 Existing strengths

- Formal workspace root and command entrypoints already exist in [automation-overview.md](/F:/MUSF/docs/automation-overview.md:3).
- A unified launcher already exists in [run-musf.cmd](/F:/MUSF/scripts/run-musf.cmd:1).
- Python CLI orchestration already exists in [cli.py](/F:/MUSF/scripts/python/musf_tools/cli.py:16).
- Release guard rules already exist in [login-stack-manifest.json](/F:/MUSF/configs/login-stack-manifest.json:3).
- `chooserole` variant registration and block rules already exist in [chooserole-variants.json](/F:/MUSF/configs/chooserole-variants.json:3).
- Android canary smoke and UI baseline checks already exist in [phase1.py](/F:/MUSF/scripts/python/musf_tools/phase1.py:217) and [android_regression.py](/F:/MUSF/scripts/python/musf_tools/android_regression.py:71).

### 3.2 Current weaknesses

- `F:\MUSF` is not currently a Git working tree, so GitHub-based review and traceability are incomplete.
- The client side has multiple parallel Unity copies:
  - `Client\Unity`
  - `Client\UnityBatchShadow`
  - `Client\UnityCodeBundleMini`
  - `Client\UnityCodeBundleMini30Shadow`
  - `Client\Unity_BuildClone_NavGrid_20260326_1201`
- Formal toolchain resolution is not complete. Several required tools still fall back to `C:` according to [toolchain-manifest.json](/F:/MUSF/toolchain-manifest.json:9).
- Release evidence exists, but it is distributed across logs, screenshots, and JSON files without a single spec-to-release chain.
- Some runtime assumptions are still hard-coded in scripts, especially package, account, hotfix folder, and screen coordinates.
- Human operators can still be tempted to swap raw bundles directly instead of fixing source and rebuilding.

## 4. Non-Negotiable Guardrails

These rules should take effect before broader AI adoption.

1. `Client\Unity` is the only editable client source of truth.
2. `UnityBatchShadow`, `UnityCodeBundleMini*`, and historical build clones are outputs, snapshots, or experiments, not primary edit targets.
3. No raw `chooserole.unity3d` replacement is allowed unless the variant is registered and marked `Allowed`.
4. Every publish candidate must pass:
   - `verify-toolchain`
   - `server-health`
   - variant inspection if protected assets changed
   - `run-gate`
5. Every publish path must produce a JSON report and a rollback source.
6. AI may propose or implement source changes, but AI may not bypass guarded publish logic.
7. Any task touching protected login-stack assets must include visual acceptance criteria.

## 5. Target Operating Model

### 5.1 Work item flow

Each task should move through the following states:

1. spec written
2. impact area confirmed
3. AI or human implementation
4. build and local checks
5. canary regression
6. guarded publish or rejection
7. artifact archive and closeout

### 5.2 Required task spec

Every medium or high-risk task should have a short spec file in `docs/specs/`.

Minimum fields:

- title
- date
- owner
- goal
- change scope
- forbidden scope
- expected artifacts
- validation commands
- visual acceptance checkpoints
- rollback path
- risk level

### 5.3 AI execution boundary

AI should be used in these modes:

- repo exploration
- diff and dependency analysis
- Unity scene and asset governance checks
- code generation inside an approved scope
- build orchestration
- report summarization
- regression result triage

AI should not be used in these modes:

- ad hoc release operator with no spec
- direct live asset overwrite
- approving its own release without gate evidence
- modifying multiple client clones in parallel

## 6. GitHub Adoption Strategy

GitHub should be used for planning, review, and evidence linkage, not as a blind replacement for local MUSF runtime workflows.

### 6.1 Use GitHub for

- Issues for task intake and problem statements
- Projects for roadmap and status tracking
- Pull requests for bounded code and config changes
- PR templates that require build and gate artifacts
- Discussions or ADRs for design choices

### 6.2 Do not rely on GitHub-hosted runners for

- local LAN server integration
- MuMu or local ADB-connected device smoke
- Unity environments that depend on the current Windows workstation state

### 6.3 Recommended near-term GitHub structure

- one main repo rooted at `F:\MUSF`
- `docs/specs/` for task specs
- `docs/runbooks/` for operator procedures
- `docs/adr/` for architecture decisions
- `.github/PULL_REQUEST_TEMPLATE.md`
- `.github/ISSUE_TEMPLATE/`
- optional self-hosted Windows runner later, only after local scripts stabilize

## 7. Repository Restructuring Plan

### Phase A: establish one editable source tree

Decision:

- editable client source: `F:\MUSF\Client\Unity`
- editable server source: current active server source tree under `F:\MUSF\Server`

Actions:

1. Mark non-primary client directories as `snapshot`, `shadow`, or `generated` in documentation.
2. Add a workspace policy document naming allowed write roots.
3. Update automation to reject writes to non-primary client trees unless a task explicitly says recovery or comparison.

Acceptance:

- no release task uses more than one editable Unity source tree
- no AI patch is applied to `UnityBatchShadow` except through an explicit generated-output workflow

### Phase B: convert scripts into a formal pipeline

Current unified command entry already exists through `musf.py` and `cli.py`.

Next action is to standardize the lifecycle:

1. `preflight`
2. `build`
3. `variant-check`
4. `run-gate`
5. `publish-hotfix-guarded`
6. `archive-artifacts`
7. `rollback`

Acceptance:

- each step returns machine-readable output
- each step has an operator-facing runbook entry
- failures are compact enough to feed back into the next AI iteration

### Phase C: formalize evidence

Every release candidate should produce a single run directory containing:

- task spec reference
- changed file list
- build log
- variant report
- gate report
- screenshots
- publish report
- rollback source location

Acceptance:

- one candidate equals one evidence folder
- PR review can be completed from the evidence folder plus code diff

## 8. Toolchain Rectification Plan

The formal goal is to stop depending on uncontrolled environment drift.

Priority items from [toolchain-manifest.json](/F:/MUSF/toolchain-manifest.json:9):

- Unity Editor formal path on `F:`
- Android SDK and ADB formal path on `F:`
- JDK 8 formal path on `F:`
- JDK 17 formal path on `F:`
- Python 3.11 formal path on `F:`
- Node 18 formal path on `F:`
- BundlePatch formal path on `F:`

Implementation sequence:

1. make `verify-toolchain` a hard preflight gate for build and publish
2. move missing tool binaries or validated links into formal locations
3. remove silent fallback where it creates release ambiguity
4. keep transitional fallback only for explicitly documented read-only tools

Acceptance:

- required tools resolve from manifest-defined formal paths
- release scripts no longer depend on arbitrary global installs

## 9. Release Governance Plan

### 9.1 Protected asset policy

The protected login-stack assets in [login-stack-manifest.json](/F:/MUSF/configs/login-stack-manifest.json:79) should remain blocked by default.

Any change to protected assets must include:

- task spec
- reason for change
- expected visual outcome
- canary result
- rollback source

### 9.2 `chooserole` policy

Current status already shows the right governance shape:

- historical live variant is blocked
- reference-only structural scene is separated
- new source rebuild candidate is still blocked pending verification

Required MUSF rule:

1. repair source scene
2. rebuild bundle from source
3. register new variant as blocked
4. run runtime verification
5. promote to allowed only after pass

This rule should be treated as a hard release contract.

## 10. AI Task Taxonomy For MUSF

### Low risk

- report generation
- documentation
- log analysis
- static comparison
- inventory generation

Approval level:

- AI can execute directly and attach evidence

### Medium risk

- config edits
- server address sync logic
- non-protected UI data changes
- build pipeline changes

Approval level:

- AI can implement within a bounded spec
- human reviews result and evidence

### High risk

- protected login-stack assets
- `chooserole` source and runtime layout
- hotfix publish logic
- device state mutation
- release target directories

Approval level:

- spec required
- guarded automation required
- explicit human promotion required

## 11. Six-Week Rollout

### Week 1

- create formal docs for specs, runbooks, and operating rules
- define primary editable roots
- freeze raw bundle replacement outside approved recovery workflows

### Week 2

- make `verify-toolchain` a mandatory preflight check
- inventory all write paths used by build and publish commands
- label generated directories and shadow directories

### Week 3

- standardize release evidence folder format
- add a single release checklist document
- require task spec references in release reports

### Week 4

- initialize Git for `F:\MUSF` if ownership and history import are ready
- add GitHub issue and PR templates
- enforce protected asset review checklist

### Week 5

- move or validate formal toolchain dependencies onto `F:`
- remove dangerous environment drift from release commands
- compact failures into AI-readable summaries

### Week 6

- pilot one full spec-to-release flow through the new process
- evaluate bottlenecks
- tighten rules where humans still bypass the flow

## 12. Immediate Action List

These are the next actions with the highest leverage.

1. Put the workspace under Git so GitHub review becomes real.
2. Declare `Client\Unity` as the only editable client source root.
3. Add spec templates and runbook templates.
4. Add a release candidate checklist tied to `run-musf.cmd`.
5. Make toolchain verification block risky commands.
6. Convert current release artifacts into a predictable per-run structure.
7. Keep `source_rebuild_candidate_68eafd3c` blocked until runtime proof is attached.

## 13. Success Metrics

Track these metrics after rollout starts:

- percent of tasks with written specs
- percent of release candidates with complete evidence folders
- number of direct raw bundle swaps
- number of release failures caused by environment drift
- mean time from issue to canary result
- number of high-risk changes rejected before live publish

## 14. Decision Summary

MUSF should adopt vibe coding as an engineering accelerator, not as a replacement for software governance.

The practical MUSF formula is:

- language for intent
- specs for scope
- scripts for execution
- gates for safety
- screenshots and JSON for proof
- humans for final promotion
