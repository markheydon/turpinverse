<!--
Sync Impact Report
- Version change: 1.1.0 → 2.0.0
- Modified principles:
  - I. Spec-Driven Development → I. Spec-Driven Traceability
  - II. Incremental Delivery → II. Incremental Independence
  - III. Test-First → III. Verifiable Testability
  - IV. Simplicity & Justified Complexity → IV. Separation of Concerns (new)
  - V. Documentation as Contract → V. Explicit Error Handling (new)
  - (added) VI. Security by Design
  - (added) VII. Justified Complexity
  - (added) VIII. Documentation Contract
- Added sections: Compliance Gates (verifiable yes/no checks)
- Removed sections: Technology & Workflow Constraints, Development Workflow
- Templates requiring updates:
  - .specify/templates/plan-template.md ✅ updated (yes/no Constitution Check gates)
  - .specify/templates/spec-template.md ✅ updated (security & error-handling sections)
  - .specify/templates/tasks-template.md ✅ aligned (traceability and testability notes)
  - .specify/templates/checklist-template.md ✅ aligned (no constitution-specific changes)
  - .specify/templates/commands/*.md ⚠ not present (N/A)
  - README.md ✅ aligned (tech stack and testing standards remain in README, not constitution)
- Follow-up TODOs: None
-->

# Turpinverse Constitution

## Core Principles

### I. Spec-Driven Traceability

Every feature MUST have a specification before implementation begins.
Specifications MUST define prioritized user stories, functional requirements, and
measurable success criteria. Plans, tasks, and pull requests MUST trace to spec
artifacts.

**Rationale**: Traceability prevents scope drift and makes every change auditable
against stated intent.

### II. Incremental Independence

Features MUST decompose into prioritized, independently testable increments
(P1, P2, P3, …). Each increment MUST deliver standalone value. No increment
may depend on a lower-priority increment for its core functionality.

**Rationale**: Independent slices enable early validation, parallel work, and
safe rollback of individual increments.

### III. Verifiable Testability (NON-NEGOTIABLE)

When testing is in scope for a feature, automated tests MUST exist and be
confirmed failing before implementation begins. External boundaries (APIs,
schemas, inter-component contracts) MUST have contract or integration test
coverage.

**Rationale**: Tests written after implementation confirm existing behavior, not
desired behavior. Failing-first tests prove the test is meaningful.

### IV. Separation of Concerns

Each component MUST have one clear responsibility. Presentation, domain logic,
persistence, and infrastructure MUST NOT be entangled in the same module
without documented justification in the plan's Complexity Tracking table.

**Rationale**: Clear boundaries reduce coupling, simplify testing, and keep
changes localized.

### V. Explicit Error Handling

Every specified failure mode MUST have an explicit handling path. Errors at
system boundaries MUST NOT be swallowed, ignored, or propagated without
defined behavior documented in the spec, plan, or contract.

**Rationale**: Silent or undefined failures erode reliability and make incidents
harder to diagnose.

### VI. Security by Design

Features that access sensitive data, authenticate users, or expose external
interfaces MUST identify threats and access controls in the specification or
plan before implementation begins.

**Rationale**: Security retrofitted after implementation consistently misses
threats that are cheaper to address during design.

### VII. Justified Complexity

The simplest solution that satisfies the current requirement MUST be chosen.
Any deviation MUST be documented in the plan's Complexity Tracking table with a
rejected simpler alternative and explicit justification.

**Rationale**: Unjustified complexity compounds maintenance cost and obscures
intent. Documented exceptions preserve accountability.

### VIII. Documentation Contract

Specifications, contracts, and quickstart guides MUST match implemented behavior.
When implementation diverges from docs, either update the docs or revert the
code—never leave them out of sync.

**Rationale**: Stale documentation is worse than none; it erodes trust and
blocks onboarding.

## Compliance Gates

Every `plan.md` MUST include a Constitution Check with a yes/no result for each
principle before Phase 0 research and again after Phase 1 design. Plans that
fail any principle without Complexity Tracking justification MUST NOT proceed.

Every pull request MUST be reviewable against these gates:

| Gate | Question |
|------|----------|
| Traceability | Does every change trace to a spec requirement or task? |
| Independence | Can each delivered increment be validated on its own? |
| Testability | When tests are in scope, do they exist, fail first, and cover boundaries? |
| Separation | Does each module have a single, clear responsibility? |
| Error handling | Is every specified failure mode handled explicitly? |
| Security | Are threats and access controls defined for sensitive or exposed features? |
| Complexity | Is added complexity justified with a rejected simpler alternative? |
| Documentation | Do specs, contracts, and quickstart match the implementation? |

Technology choices, libraries, frameworks, and conventions belong in
`README.md`, `docs/tech-stack.md`, guidelines, or decision logs—not in this
constitution.

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

**Version**: 2.0.0 | **Ratified**: 2026-07-18 | **Last Amended**: 2026-07-22
