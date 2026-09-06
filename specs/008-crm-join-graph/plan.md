# Implementation Plan: CRM Join Graph Alignment

**Branch**: `008-crm-join-graph` | **Date**: 2026-09-06 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/008-crm-join-graph/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

**Channels affected**: **Public reference site (Hugo — showcase / read)** and
**in-product web app (Blazor — explore / filter / download)**.

- **Hugo** shows named people and organisations. Deal, case, and project pages
  show optional main contact and stakeholders by **display name**. Organisation
  pages MAY show the account primary contact by name. When a main contact is
  omitted, omit the named-contact block. Reader copy MUST NOT lead with join
  keys, CSV column names, or the email collision policy.
- **Blazor** is the import surface: one contact row per membership; optional
  `primaryContactId` on accounts; optional `contactId` plus `stakeholderContactIds`
  on deals, cases, and projects; collision policy next to contact download.

Shared canon JSON remains the single source of truth. Join cardinality follows
[`docs/entity-relationships.md`](../../docs/entity-relationships.md). This plan
respects Constitution Principle IX and [`docs/product-surfaces.md`](../../docs/product-surfaces.md).

## Summary

Align canon, completeness, CSV export, and publication with the target CRM join
graph (GitHub #41). Make deal/case/project **main contact optional** but, when
set, an **account member**. Add **stakeholders** who need not be members. Replace
project `personaIds` with optional main ∪ stakeholders. Export **one contact row
per membership** with copied identity fields. Document email collision on the
export surface.

**Canon edits this increment** (no new people, orgs, or pipeline rows):

1. Four story repairs (spec FR-010): `deal-008`, `case-001`, `case-017`,
   `palmer-identity-vault`.
2. Remaining projects mapped per FR-008 (`black-bess-route-optimiser`,
   `essex-procurement-hub`).
3. **Planning (not a spec change):** populate `primaryContactId` on all ten
   existing organisations (FR-003 allows it; spec assumed demo MAY omit).
   Turpin Enterprises → Richard Turpin; Essex Solutions Group → Samuel Gregory.
   See [data-model.md](./data-model.md).

Parent epic #32 and child stories #33–#38 depend on this graph landing first.
Do not rewrite `specs/001-turpinverse-universe/spec.md` as the join-correction
record; merge live JSON Schema / CRM export contracts in place (constitution VIII).

## Technical Context

**Language/Version**: C# / .NET 10; Hugo Extended (existing `site/`)

**Primary Dependencies**: ASP.NET Core Blazor Server, .NET Aspire, JsonSchema.Net,
existing `CanonValidator` / `HugoContentGenerator` / `JsonCanonRepository` /
`ExportMapper` / `ExportCsvColumns` / `CareerPortfolioPresenter`

**Storage**: File-based JSON in `src/Turpinverse.Data/canon/` (`organisations.json`,
`deals.json`, `cases.json`, `projects.json`; events schema only — no required
event rewrites). No database.

**Testing**: xUnit v3, NSubstitute, bUnit, coverlet; `Category=CanonValidation`
plus export, Hugo generator, and Blazor tests (failing first per constitution III)

**Target Platform**: Cross-platform .NET 10; Hugo site on GitHub Pages from
existing `site/` pipeline; local Blazor via Aspire

**Project Type**: Multi-artifact OSS monorepo — static documentation site + Blazor
web app (extend existing projects; no new csproj)

**Performance Goals**: Canon validate + CSV export remain well under the existing
2s export budget; Hugo regenerate comparable to current runtime

**Constraints**: English-only; fictional data; no new auth; no second public site
or new download product; professional-extras contact unchanged; no per-account
email/phone/address in canon; schema **allows** empty accounts and omitted main
contacts but this increment does **not** author those shapes; VR-052–VR-059
allocated in the spec

**Scale/Scope**: 10 organisations, 25 personas, 22 deals, 17 cases, 3 projects;
contact CSV grows from 25 rows to one per membership (~31); two channels

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Answer **Yes** or **No** for each principle. Any **No** without a Complexity
Tracking justification blocks the gate.

| Principle | Pre-Design | Post-Design | Evidence |
|-----------|------------|-------------|----------|
| I. Spec-Driven Traceability | Yes | Yes | spec.md US1–US3, FR-001–FR-015, SC-001–SC-007; issue #41; planning primary-contact table traces to FR-003 |
| II. Incremental Independence | Yes | Yes | P1 join rules + validator; P2 four rows + project remap + authored primaries; P3 membership export + collision copy |
| III. Verifiable Testability | Yes | Yes | Failing-first VR-052–VR-059 and export/Hugo tests in [quickstart.md](./quickstart.md) |
| IV. Separation of Concerns | Yes | Yes | Models + schema; `CanonValidator`; `ExportMapper`; Hugo generator/layouts; Blazor explore/export |
| V. Explicit Error Handling | Yes | Yes | Spec failure table; VR-052–VR-059; 422 on `/api/canon/validate` |
| VI. Security by Design | Yes | Yes | Spec security table; fictional identity copy-on-export; no new auth |
| VII. Justified Complexity | Yes | Yes | Extend existing validator/mapper; stakeholders as arrays on existing records; no second contact file |
| VIII. Documentation Contract | Yes | Yes | [contracts/](./contracts/), [data-model.md](./data-model.md), [quickstart.md](./quickstart.md); merge live 001 schemas |
| IX. Human-Facing Channel Charter | Yes | Yes | Plan names Hugo names/stories vs Blazor ids/collision policy |

**Gate result**: PASS

## Project Structure

### Documentation (this feature)

```text
specs/008-crm-join-graph/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/
│   ├── join-canon-schema.json
│   ├── crm-export-schema.json
│   ├── completeness.md
│   └── channel-surfaces.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/Turpinverse.Core/Models/Organisation.cs     # optional PrimaryContactId
src/Turpinverse.Core/Models/Deal.cs             # optional ContactId; StakeholderContactIds
src/Turpinverse.Core/Models/Case.cs             # same
src/Turpinverse.Core/Models/Project.cs          # replace PersonaIds; optional DealId/CaseIds
src/Turpinverse.Core/Models/CanonEvent.cs       # optional DealIds/CaseIds
src/Turpinverse.Core/Validation/CanonValidator.cs
src/Turpinverse.Core/Export/ExportMapper.cs     # membership explode; optional FKs
src/Turpinverse.Core/Export/ExportCsvColumns.cs
src/Turpinverse.Core/Export/ContactExport.cs
src/Turpinverse.Core/Export/AccountExport.cs
src/Turpinverse.Core/Export/DealExport.cs
src/Turpinverse.Core/Export/CaseExport.cs
src/Turpinverse.Core/Export/ProjectExport.cs
src/Turpinverse.Core/Career/CareerPortfolioPresenter.cs
src/Turpinverse.Core/Hugo/HugoContentGenerator.cs
src/Turpinverse.Data/canon/organisations.json
src/Turpinverse.Data/canon/deals.json
src/Turpinverse.Data/canon/cases.json
src/Turpinverse.Data/canon/projects.json
src/Turpinverse.Web/Components/Pages/           # Contacts collision note; optional account primary column
site/layouts/                                   # omit-empty main contact; stakeholders by name
tests/Turpinverse.Core.UnitTests/Validation/
tests/Turpinverse.Core.UnitTests/Export/
tests/Turpinverse.IntegrationTests/Export/
specs/001-turpinverse-universe/contracts/       # merge live canon + CRM schemas
docs/entity-relationships.md                    # already target; keep in sync if impl names differ
docs/career-portfolio-mapping.md                # project people = main ∪ stakeholders
```

**Structure Decision**: Extend the existing Core / Data / Web / Hugo / test layout.
No new project, no new export dataset type, no new public site.

## Complexity Tracking

> No constitution violations. Table left empty on purpose.
