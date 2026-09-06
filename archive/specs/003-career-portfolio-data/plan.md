# Implementation Plan: Career and Portfolio Demo Data

**Branch**: `003-career-portfolio-data` | **Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/003-career-portfolio-data/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add first-class Turpinverse canon records for **experience**, **education**, **projects**,
and **achievements** (issues #19 / #20), authored as JSON beside existing canon files—
not as a consumer profile-theme tree. Richard Turpin (`dick-turpin`) is the volume
subject: ≥3 employer groupings with nested contemporary roles, ≥2 education programmes,
≥3 shared catalog projects (each with a canon organisation/account), and ≥4 achievements,
with completeness enforced by `CanonValidator` (VR-020–VR-027). Publish the same records
on existing Hugo persona pages and a new Blazor contact detail route; omit empty section
types; do not switch the public theme or add CSV columns. A repository-only mapping note
shows how generic fields could fill a typical personal site.

## Technical Context

**Language/Version**: C# / .NET 10; Hugo Extended (existing site)

**Primary Dependencies**: ASP.NET Core Blazor Server, .NET Aspire, JsonSchema.Net,
existing `CanonValidator` / `HugoContentGenerator` / `JsonCanonRepository`

**Storage**: File-based JSON in `src/Turpinverse.Data/canon/`
(`experience.json`, `education.json`, `projects.json`, `achievements.json`); no database

**Testing**: xUnit v3, NSubstitute, bUnit, coverlet; `Category=CanonValidation` plus
feature tests for Hugo generation and Blazor contact detail

**Target Platform**: Cross-platform .NET 10; Hugo site on GitHub Pages from existing
`site/` → `docs/` pipeline; local Blazor via Aspire

**Project Type**: Multi-artifact OSS monorepo — static documentation site + Blazor web app
(extend existing projects; no new csproj)

**Performance Goals**: Canon load + validate including new collections in well under the
existing CSV export budget (2s); Hugo regenerate remains comparable to current generator
runtime

**Constraints**: English-only copy; Turpinverse tone (legend-as-business); contemporary
CV overlay (do not bind dates to historical life span); no public theme switch; no new
profile-demo product; no new anonymous bulk-download surface; fictional PII only

**Scale/Scope**: Volume gates on one primary persona; optional deputies; four new entity
types; two publication channels (Hugo persona single, Blazor `/contacts/{id}`); mapping
stub in `docs/`

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Answer **Yes** or **No** for each principle. Any **No** without a Complexity
Tracking justification blocks the gate.

| Principle | Pre-Design | Post-Design | Evidence |
|-----------|------------|-------------|----------|
| I. Spec-Driven Traceability | Yes | Yes | spec.md FR-001–FR-015, US1–US3; plan traces to issues #19/#20 |
| II. Incremental Independence | Yes | Yes | P1 career+Hugo/Blazor career sections; P2 portfolio+sharing; P3 mapping note |
| III. Verifiable Testability | Yes | Yes | Failing-first CanonValidation + Hugo/Blazor tests in [quickstart.md](./quickstart.md) |
| IV. Separation of Concerns | Yes | Yes | JSON canon / Core models+validator+presenter / Hugo layouts / Blazor pages / docs mapping |
| V. Explicit Error Handling | Yes | Yes | spec failure table; VR-020–VR-027; 422 on `/api/canon/validate` |
| VI. Security by Design | Yes | Yes | spec security table; fictional demo copy; no new public download; existing DemoExport policy |
| VII. Justified Complexity | Yes | Yes | Reuse JSON+validator+Hugo generator; contact detail is required for person-page sections |
| VIII. Documentation Contract | Yes | Yes | [contracts/](./contracts/), [data-model.md](./data-model.md), [quickstart.md](./quickstart.md) |

**Gate result**: PASS

## Project Structure

### Documentation (this feature)

```text
specs/003-career-portfolio-data/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/
│   ├── career-portfolio-schema.json
│   └── completeness.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/Turpinverse.Data/canon/
├── experience.json
├── education.json
├── projects.json
└── achievements.json

src/Turpinverse.Data/Repositories/JsonCanonRepository.cs
src/Turpinverse.Core/Models/          # Experience, Role, Education, Project, Achievement, FeaturedLink; Canon extras
src/Turpinverse.Core/Validation/      # CanonValidator VR-020–VR-027; schema merge into 001 canon-schema.json
src/Turpinverse.Core/Hugo/            # HugoContentGenerator writes site/data career files
src/Turpinverse.Core/Career/          # Presenter: membership + FR-015 sort (shared by Hugo + Blazor)

src/Turpinverse.Web/Components/Pages/ # Contact detail /contacts/{contactId}; contacts table links
site/layouts/personas/single.html     # Sections with omit-empty (PaperMod stays the theme)
site/data/                            # Generated experience/education/projects/achievements (or persona-career)

docs/career-portfolio-mapping.md      # P3 mapping note (not Hugo content)

tests/Turpinverse.Core.UnitTests/Validation/
tests/Turpinverse.Core.UnitTests/Hugo/
tests/Turpinverse.Web.UnitTests/      # Contact detail omit-empty / order
```

**Structure Decision**: Extend the existing Data / Core / Web / Hugo-tool split from
`001-turpinverse-universe`. No new project. Runtime JSON Schema remains embedded from
`specs/001-turpinverse-universe/contracts/canon-schema.json` and MUST be updated to
`$ref` or inline the additive types from this feature’s contract. Developer mapping
lives in repo `docs/`, not `site/content`.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

No constitution violations. Contact detail is not extra architecture: the current
contacts UI is a CSV preview table and cannot satisfy FR-013 section semantics.

## Phase 0 / Phase 1 outputs

- [research.md](./research.md) — storage, catalog, validator, publication, dates, mapping
- [data-model.md](./data-model.md) — entities, uniqueness, VR codes
- [contracts/career-portfolio-schema.json](./contracts/career-portfolio-schema.json)
- [contracts/completeness.md](./contracts/completeness.md)
- [quickstart.md](./quickstart.md)

## Implementation notes (for `/speckit-tasks`)

1. Write failing validator and schema tests (empty/missing collections, orphan ids,
   volume shortfalls) before shipping default JSON that passes.
2. Author in-universe copy for `dick-turpin` first; add one shared catalog membership
   for FR-014.
3. Merge additive schema into the 001 `canon-schema.json` `Canon` `required` / `$defs`.
4. Generate Hugo data; update `personas/single.html` only (no theme pack change).
5. Blazor: `/contacts/{contactId}` + table link; CSV columns unchanged.
6. Add `docs/career-portfolio-mapping.md`.
7. Do not treat `Persona.birthYear`/`deathYear` or `status: deceased` as CV constraints.
