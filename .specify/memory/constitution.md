<!--
Sync Impact Report
- Version change: (template) → 1.0.0
- Modified principles: N/A (initial ratification)
- Added sections: Core Principles (5), Technology & Workflow Constraints, Development Workflow, Governance
- Removed sections: None
- Templates requiring updates:
  - .specify/templates/plan-template.md ✅ aligned (Constitution Check gate present)
  - .specify/templates/spec-template.md ✅ aligned (user stories, requirements, success criteria)
  - .specify/templates/tasks-template.md ✅ aligned (user-story phases, test-first notes)
  - .specify/templates/checklist-template.md ✅ aligned (no constitution-specific changes needed)
  - .specify/templates/commands/*.md ⚠ not present (N/A)
  - README.md ⚠ not present (create when project scope is defined)
- Follow-up TODOs: Define technology stack in first feature plan; add README when project scope is clarified
-->

# Turpinverse Constitution

## Core Principles

### I. Spec-Driven Development

Every feature MUST begin with a specification in `specs/[###-feature-name]/spec.md`
before any implementation work begins. Specifications MUST define prioritized user
stories, functional requirements, and measurable success criteria. Plans,
tasks, and implementation MUST trace back to spec artifacts.

**Rationale**: Specs are the contract between intent and delivery. Building
without them leads to scope drift and untestable outcomes.

### II. Incremental Delivery

Features MUST be decomposed into prioritized, independently testable user
stories (P1, P2, P3, …). Each story MUST deliver standalone value as a viable
MVP increment. No story may depend on a lower-priority story for its core
functionality.

**Rationale**: Independent slices enable early validation, parallel work, and
safe rollback of individual increments.

### III. Test-First (NON-NEGOTIABLE)

When testing is in scope for a feature, tests MUST be written and confirmed
failing before implementation begins. The Red-Green-Refactor cycle MUST be
followed. Contract and integration tests are REQUIRED for new API surfaces,
schema changes, and inter-service communication.

**Rationale**: Tests written after implementation confirm existing behavior,
not desired behavior. Failing-first tests prove the test is meaningful.

### IV. Simplicity & Justified Complexity

Start with the simplest solution that satisfies the current requirement. YAGNI
applies: do not build for hypothetical future needs. Any deviation from this
principle MUST be documented in the plan's Complexity Tracking table with a
rejected simpler alternative and explicit justification.

**Rationale**: Unjustified complexity compounds maintenance cost and obscures
intent. Documented exceptions preserve accountability.

### V. Documentation as Contract

Specifications, plans, data models, API contracts, and quickstart guides MUST
remain current with implementation. Files in `specs/[###-feature]/contracts/`
are authoritative for API behavior. When implementation diverges from docs,
either update the docs or revert the code—never leave them out of sync.

**Rationale**: Stale documentation is worse than none; it erodes trust and
blocks onboarding.

## Technology & Workflow Constraints

- Feature work MUST follow the Spec Kit workflow: constitution → specify →
  clarify → plan → tasks → implement.
- Feature branches MUST follow the `###-feature-name` naming convention
  (sequential numbering per `.specify/extensions/git/git-config.yml`).
- Technology stack choices (language, framework, storage) are deferred to
  per-feature `plan.md` until a project-wide stack is established by the first
  feature.
- All feature artifacts live under `specs/[###-feature-name]/`; shared project
  governance lives under `.specify/`.

## Development Workflow

1. **Constitution Check** — Every `plan.md` MUST include a Constitution Check
   gate before Phase 0 research and again after Phase 1 design. Plans that
   violate principles without Complexity Tracking justification MUST NOT
   proceed.
2. **Branch per feature** — Create a feature branch before specification work
   (`/speckit-specify` triggers branch creation via git extension).
3. **Checkpoint validation** — After each user story phase, validate the story
   independently before starting the next priority.
4. **Commit discipline** — Commit at logical checkpoints (per task group or
   story completion). Use the git extension auto-commit hooks or manual commits
   with descriptive messages.
5. **Review readiness** — Before marking a feature complete, run quickstart
   validation and verify all spec success criteria are met.

## Governance

This constitution supersedes ad-hoc practices for all Turpinverse feature work.
Amendments MUST be made via `/speckit-constitution`, include a version bump
following semantic versioning, and propagate consistency checks to dependent
templates.

- **MAJOR** — Backward-incompatible principle removals or redefinitions.
- **MINOR** — New principles or materially expanded guidance.
- **PATCH** — Clarifications, wording fixes, non-semantic refinements.

All pull requests and plan reviews MUST verify compliance with active
principles. Complexity without documented justification is grounds for rejection.
Runtime development guidance SHOULD be maintained in project docs (e.g.,
`README.md`, `docs/`) as the stack matures.

**Version**: 1.0.0 | **Ratified**: 2026-07-18 | **Last Amended**: 2026-07-18
