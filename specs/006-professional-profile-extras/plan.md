# Implementation Plan: Professional Profile Extras

**Branch**: `006-professional-profile-extras` | **Date**: 2026-08-23 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/006-professional-profile-extras/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

**Channels affected**: **Public reference site (Hugo — showcase / read)** and
**in-product web app (Blazor — explore ahead of export)**. Professional extras are
human-readable person presentation on existing persona/contact pages. Shared canon
JSON remains the single source of truth. This plan respects Constitution Principle
IX and [`docs/product-surfaces.md`](../../docs/product-surfaces.md): Hugo MUST NOT
present technical identifiers or export plumbing as primary content; Blazor remains
the explore/export surface. No second showcase product, theme switch, or new CSV
columns.

## Summary

Add first-class Turpinverse **professional extras** (issue #22, parent #18): one
person-linked set of intro/hero copy, about, skills, contact, and professional
socials. Author as JSON beside existing canon—not a CMS/theme tree and not nested
career lists. Richard Turpin (`dick-turpin`) is the volume subject. Completeness
is `CanonValidator` VR-036–VR-043. Publish on existing Hugo persona pages and
Blazor `/contacts/{id}` in sandwich order (intro header; about; skills; existing
career/portfolio; contact; socials), omitting empty types and suppressing
competing title/biography/header-email display when extras exist.

## Technical Context

**Language/Version**: C# / .NET 10; Hugo Extended (existing `site/`, PaperMod module)

**Primary Dependencies**: ASP.NET Core Blazor Server, .NET Aspire, JsonSchema.Net,
existing `CanonValidator` / `HugoContentGenerator` / `JsonCanonRepository` /
`CareerPortfolioPresenter` / `FeaturedLink`

**Storage**: File-based JSON in `src/Turpinverse.Data/canon/professional-extras.json`;
no database

**Testing**: xUnit v3, NSubstitute, bUnit, coverlet; `Category=CanonValidation`
plus Hugo generator and Blazor contact-detail tests (`ProfessionalExtras` trait)

**Target Platform**: Cross-platform .NET 10; Hugo site on GitHub Pages from existing
`site/` pipeline; local Blazor via Aspire

**Project Type**: Multi-artifact OSS monorepo — static documentation site + Blazor
web app (extend existing projects; no new csproj)

**Performance Goals**: Canon load + validate including extras in well under the
existing CSV export budget (2s); Hugo regenerate remains comparable to current
generator runtime

**Constraints**: English-only copy; Turpinverse tone; fictional PII only; no
public theme switch; no new profile-demo product; no new anonymous bulk-download
surface; no website-chrome or form-endpoint fields; skills/socials in author order

**Scale/Scope**: Volume gates on one primary persona; optional deputies; one new
entity type (extras set with optional nested types); two publication channels
(Hugo persona single, Blazor `/contacts/{id}`)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Answer **Yes** or **No** for each principle. Any **No** without a Complexity
Tracking justification blocks the gate.

| Principle | Pre-Design | Post-Design | Evidence |
|-----------|------------|-------------|----------|
| I. Spec-Driven Traceability | Yes | Yes | spec.md US1–US3, FR-001–FR-013, SC-001–SC-006; issues #22/#18 |
| II. Incremental Independence | Yes | Yes | P1 intro+header; P2 about+skills; P3 contact+socials; each testable on existing person pages |
| III. Verifiable Testability | Yes | Yes | Failing-first CanonValidation + Hugo/Blazor tests in [quickstart.md](./quickstart.md) |
| IV. Separation of Concerns | Yes | Yes | JSON canon / Core models+validator+presenter / Hugo layouts / Blazor contact detail |
| V. Explicit Error Handling | Yes | Yes | Spec failure table; VR-036–VR-043; 422 on `/api/canon/validate` |
| VI. Security by Design | Yes | Yes | Spec security table; fictional copy; no new public download; existing DemoExport policy |
| VII. Justified Complexity | Yes | Yes | Reuse JSON+validator+Hugo generator+contact detail; no mapping note; no new product |
| VIII. Documentation Contract | Yes | Yes | [contracts/](./contracts/), [data-model.md](./data-model.md), [quickstart.md](./quickstart.md) |
| IX. Human-Facing Channel Charter | Yes | Yes | Plan names Hugo showcase + Blazor explore; dual person pages; no second showcase; IDs off Hugo primary copy |

**Gate result**: PASS

## Project Structure

### Documentation (this feature)

```text
specs/006-professional-profile-extras/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/
│   ├── professional-extras-schema.json
│   ├── completeness.md
│   └── person-surfaces.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/Turpinverse.Data/canon/
└── professional-extras.json

src/Turpinverse.Data/Repositories/JsonCanonRepository.cs
src/Turpinverse.Core/Models/          # ProfessionalExtras, IntroCopy, ContactExtras, ProfessionalSocial; Canon extra
src/Turpinverse.Core/Validation/      # CanonValidator VR-036–VR-043; merge into 001 canon-schema.json
src/Turpinverse.Core/Profile/         # Presenter: lookup, visibility flags, author-order lists (shared Hugo + Blazor)
src/Turpinverse.Core/Hugo/            # HugoContentGenerator writes site/data extras

src/Turpinverse.Web/Components/Pages/ContactDetail.razor
site/layouts/personas/single.html     # Sandwich + suppression; PaperMod stays the theme
site/data/                            # Generated extras keyed by persona id

docs/product-surfaces.md              # Inventory: extras on both person channels

tests/Turpinverse.Core.UnitTests/Validation/
tests/Turpinverse.Core.UnitTests/Hugo/
tests/Turpinverse.Web.UnitTests/      # Sandwich, omit-empty, suppression
```

**Structure Decision**: Extend the existing Data / Core / Web / Hugo-tool split from
`001-turpinverse-universe` and dual-publication from `003`. No new project.
Runtime JSON Schema remains embedded from
`specs/001-turpinverse-universe/contracts/canon-schema.json` and MUST be updated to
`$ref` or inline the additive types. Do not add `ExportDatasets` entries or CSV
columns.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

No constitution violations. A Core presenter is the same pattern as career
(one order/visibility implementation for two channels), not extra architecture.

## Phase 0 / Phase 1 outputs

- [research.md](./research.md) — storage, validator, sandwich, suppression, schema
- [data-model.md](./data-model.md) — entities, uniqueness, VR codes
- [contracts/professional-extras-schema.json](./contracts/professional-extras-schema.json)
- [contracts/completeness.md](./contracts/completeness.md)
- [contracts/person-surfaces.md](./contracts/person-surfaces.md)
- [quickstart.md](./quickstart.md)

## Implementation notes (for `/speckit-tasks`)

1. Write failing validator and schema tests (orphan id, duplicate set, duplicate
   skill/network case-insensitive, primary volume shortfalls, forbidden keys)
   before shipping default JSON that passes.
2. Merge additive schema into the 001 `canon-schema.json` (`professionalExtras`
   required array + `$defs`).
3. Author in-universe extras for `dick-turpin` first; seed intro/about/contact from
   existing display name, title, biography, and
   `richard.turpin@turpinverse.uk`; do not copy career/portfolio lists.
4. Generate Hugo data; update `personas/single.html` sandwich and suppression
   (no theme pack change, no new nav).
5. Blazor: extend `/contacts/{contactId}` with the same rules; CSV unchanged.
6. Update `docs/product-surfaces.md` dual-publication inventory.
7. Do not add a mapping note, second site, or extras export columns.
