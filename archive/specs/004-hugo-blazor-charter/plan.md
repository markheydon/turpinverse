# Implementation Plan: Human-Facing Channel Alignment

**Branch**: `004-hugo-blazor-charter` | **Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/004-hugo-blazor-charter/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

**Channels affected**: Public reference site (Hugo — showcase / read) and interactive export app (Blazor — explore / filter / download). Shared canon JSON remains the single source of truth. This plan implements Constitution Principle IX and `docs/product-surfaces.md`.

## Summary

Close the charter gaps: the public site must showcase **all** human-readable demo types (including dedicated deal and case indexes/details, bidirectional named links, no export plumbing, no join keys as visible identity). The Blazor app stays the import surface and must actually **filter** dataset lists so preview and CSV download share the same predicate (empty match → block/warn, never a silent empty or unfiltered file).

Technical approach: extend `HugoContentGenerator` + layouts/nav; extend `IExportService` and `/api/export/{dataset}` with optional filter query params; add facets on `CrmEntityPage`. No new canon plot, no new import formats.

## Technical Context

**Language/Version**: C# / .NET 10 (`net10.0`); Hugo Extended (site module `min = "0.146.0"`)

**Primary Dependencies**: ASP.NET Core Blazor Server, .NET Aspire (AppHost + ServiceDefaults), CsvHelper, Tailwind CSS 4.x, Chart.js, Hugo PaperMod theme

**Storage**: File-based JSON canon in `src/Turpinverse.Data/canon/` (no database)

**Testing**: xUnit v3, NSubstitute, bUnit, coverlet; `Microsoft.AspNetCore.Mvc.Testing` for export API

**Target Platform**: Local Aspire-hosted Blazor Server; static Hugo site (GitHub Pages / turpinverse.uk)

**Project Type**: Distributed demo dataset — static public site + local interactive export web app

**Performance Goals**: Full-canon Hugo generate and CSV export of tens of rows in well under SC-004’s 3-minute browse/filter/download budget; no new throughput SLO

**Constraints**: Principle IX channel split; FR-010 no silent empty/unfiltered download; public site usable offline from the Blazor app; existing CSV column contracts unchanged

**Scale/Scope**: Current canon volumes (~25 contacts, ~10 orgs, 22 deals, 17 cases, timeline events, career/portfolio); two channels; no SaaS hosting of the export app

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Answer **Yes** or **No** for each principle. Any **No** without a Complexity Tracking justification blocks the gate.

| Principle | Pre-Design | Post-Design | Evidence |
|-----------|------------|-------------|----------|
| I. Spec-Driven Traceability | Yes | Yes | [spec.md](./spec.md) US1–US3, FR-001–FR-012, SC-001–SC-006; contracts cite FRs |
| II. Incremental Independence | Yes | Yes | P1 Hugo showcase (US1–US2) is independently testable without Blazor; P2 filter/download (US3) is independently testable without new Hugo pages |
| III. Verifiable Testability | Yes | Yes | Failing-first generator, identity, export-filter, 409, and bUnit tests in [quickstart.md](./quickstart.md); API boundary in [export-filter-api.md](./contracts/export-filter-api.md) |
| IV. Separation of Concerns | Yes | Yes | Canon stays in Data; Hugo projection in Core `Hugo/`; CSV/filter in Core `Export/`; Blazor UI in Web; layouts only present |
| V. Explicit Error Handling | Yes | Yes | Spec failure modes: unresolved related party fallback; load error; empty-filter 409; no in-page download when app is down |
| VI. Security by Design | Yes | Yes | Fictional demo data; no new public write surface; IDs kept off Hugo primary copy; export remains demo-labelled local app |
| VII. Justified Complexity | Yes | Yes | Reuse generator + `CrmEntityPage`; query-param filters vs new search stack; Complexity Tracking empty |
| VIII. Documentation Contract | Yes | Yes | [research.md](./research.md), [data-model.md](./data-model.md), [contracts/](./contracts/), [quickstart.md](./quickstart.md); update `docs/product-surfaces.md` gaps at implement |
| IX. Human-Facing Channel Charter | Yes | Yes | Plan names both channels; Hugo = read/showcase; Blazor = explore/filter/download; no importer on Hugo, no encyclopedia mandate on Blazor |

**Gate result**: PASS

## Project Structure

### Documentation (this feature)

```text
specs/004-hugo-blazor-charter/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   ├── hugo-showcase.md
│   └── export-filter-api.md
└── tasks.md             # Phase 2 output (/speckit-tasks — NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/Turpinverse.Data/canon/          # deals.json, cases.json, personas, orgs, events (unchanged story)
src/Turpinverse.Core/Hugo/           # HugoContentGenerator — emit deals/cases + data JSON
src/Turpinverse.Core/Export/         # CsvExportService, ExportFilter, preview/CSV predicates
src/Turpinverse.Core/Abstractions/   # IHugoContentGenerator, IExportService
src/Turpinverse.Tools.GenerateHugoContent/
src/Turpinverse.Web/Endpoints/       # ExportEndpoints query params + 409
src/Turpinverse.Web/Components/Crm/  # CrmEntityPage + DataTable facets
src/Turpinverse.Web/Components/Pages/# Contacts, Accounts, Deals, Cases
site/content/                        # generated personas, orgs, timeline, deals, cases
site/data/                           # organisations.json, events.json, deals.json, cases.json, career/
site/layouts/                        # personas, organisations, timeline, deals, cases, identity partials
site/hugo.toml                       # menu.main
docs/product-surfaces.md             # close known-gap rows when shipped
tests/Turpinverse.Core.UnitTests/Hugo/
tests/Turpinverse.Core.UnitTests/Export/
tests/Turpinverse.Web.UnitTests/Components/
tests/Turpinverse.IntegrationTests/Export/
```

**Structure Decision**: Keep the existing three-layer layout (Data canon, Core projections, Web + Hugo site). Do not add a fourth app. Deals/cases pages live beside current Hugo sections; filters live on the existing CRM dataset pages.

## Complexity Tracking

> No constitution violations. Simplest path: generate Hugo pages like personas; add query-param AND-facets on the existing export GET endpoints.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| — | — | — |
