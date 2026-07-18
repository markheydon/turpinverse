---
description: "Task list for Turpinverse Universe & Demo Data"
---

# Tasks: Turpinverse Universe & Demo Data

**Input**: Design documents from `/specs/001-turpinverse-universe/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Included per plan.md test strategy and constitution Principle III (canon validation, export contract, integration tests per quickstart.md).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization, OSS hygiene, and solution scaffolding

- [X] T001 Create repository directory structure per plan.md (`src/`, `tests/`, `site/`, `tools/`, `.github/workflows/`)
- [X] T002 Create `global.json` pinning .NET 10 SDK version at repository root
- [X] T003 [P] Create `.editorconfig` at repository root with C# and JSON conventions
- [X] T004 [P] Create `.gitattributes` at repository root for line-ending normalisation
- [X] T005 [P] Create `.gitignore` at repository root covering .NET, Node, Hugo, and IDE artifacts
- [X] T006 [P] Add MIT `LICENSE` file at repository root
- [X] T007 Create `Turpinverse.slnx` solution file at repository root referencing all projects
- [X] T008 Create `src/Turpinverse.Data/Turpinverse.Data.csproj` class library for canon JSON resources
- [X] T009 Create `src/Turpinverse.Core/Turpinverse.Core.csproj` class library referencing Turpinverse.Data
- [X] T010 Create `src/Turpinverse.Web/Turpinverse.Web.csproj` Blazor Server project referencing Turpinverse.Core
- [X] T011 Create `src/Turpinverse.ServiceDefaults/Turpinverse.ServiceDefaults.csproj` Aspire shared defaults project
- [X] T012 Create `src/Turpinverse.AppHost/Turpinverse.AppHost.csproj` Aspire AppHost referencing Turpinverse.Web
- [X] T013 [P] Create `tests/Turpinverse.Core.UnitTests/Turpinverse.Core.UnitTests.csproj` with xUnit, FluentAssertions, coverlet
- [X] T014 [P] Create `tests/Turpinverse.Web.UnitTests/Turpinverse.Web.UnitTests.csproj` with bUnit and xUnit
- [X] T015 [P] Create `tests/Turpinverse.IntegrationTests/Turpinverse.IntegrationTests.csproj` with WebApplicationFactory support
- [X] T016 Create `tools/generate-hugo-content/Turpinverse.Tools.GenerateHugoContent/Turpinverse.Tools.GenerateHugoContent.csproj` console app referencing Turpinverse.Core
- [X] T017 [P] Create `site/hugo.toml` with `publishDir = "../docs"` and base URL for GitHub Pages
- [X] T018 [P] Create `src/Turpinverse.Web/package.json` with Tailwind CSS CLI and build scripts
- [X] T019 [P] Create `README.md` at repository root with project overview and placeholder quickstart link

**Checkpoint**: Solution builds with `dotnet build` (empty projects)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core domain models, canon loading, validation framework, and Aspire wiring that MUST be complete before ANY user story

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T020 [P] Create `Persona` record in `src/Turpinverse.Core/Models/Persona.cs` per data-model.md
- [X] T021 [P] Create `Organisation` record in `src/Turpinverse.Core/Models/Organisation.cs` per data-model.md
- [X] T022 [P] Create `CanonEvent` record in `src/Turpinverse.Core/Models/CanonEvent.cs` per data-model.md
- [X] T023 [P] Create `AliasMap` record in `src/Turpinverse.Core/Models/AliasMap.cs` per data-model.md
- [X] T024 [P] Create `ToneGuidelines` record in `src/Turpinverse.Core/Models/ToneGuidelines.cs` per data-model.md
- [X] T025 [P] Create `Deal` record in `src/Turpinverse.Core/Models/Deal.cs` per data-model.md
- [X] T026 [P] Create `Case` record in `src/Turpinverse.Core/Models/Case.cs` per data-model.md
- [X] T027 Create `Canon` root model in `src/Turpinverse.Core/Models/Canon.cs` wrapping all entity collections
- [X] T028 Create `ICanonRepository` interface in `src/Turpinverse.Core/Abstractions/ICanonRepository.cs`
- [X] T029 Implement `JsonCanonRepository` in `src/Turpinverse.Data/Repositories/JsonCanonRepository.cs` loading from `src/Turpinverse.Data/canon/*.json`
- [X] T030 Create `CanonValidationResult` and `ValidationViolation` types in `src/Turpinverse.Core/Validation/CanonValidationResult.cs`
- [X] T031 Implement `CanonValidator` in `src/Turpinverse.Core/Validation/CanonValidator.cs` enforcing VR-001 through VR-010 from data-model.md
- [X] T032 Create `ServiceCollectionExtensions` in `src/Turpinverse.Core/DependencyInjection/ServiceCollectionExtensions.cs` registering canon services
- [X] T033 Configure Aspire defaults in `src/Turpinverse.ServiceDefaults/Extensions.cs` (OTel, health checks, resilience)
- [X] T034 Wire `Turpinverse.Web` project in `src/Turpinverse.AppHost/Program.cs` as Aspire resource
- [X] T035 Register canon services in `src/Turpinverse.Web/Program.cs` via Core DI extension
- [X] T036 [P] Write failing canon validation unit tests in `tests/Turpinverse.Core.UnitTests/Validation/CanonValidatorTests.cs` with `[Trait("Category", "CanonValidation")]`
- [X] T037 [P] Write failing JSON load tests in `tests/Turpinverse.Core.UnitTests/Data/JsonCanonRepositoryTests.cs`

**Checkpoint**: `dotnet test tests/Turpinverse.Core.UnitTests` runs (tests fail until canon data exists in US1)

---

## Phase 3: User Story 1 - Establish the Turpinverse Canon (Priority: P1) 🎯 MVP

**Goal**: Publish a complete, internally consistent canon dataset and Hugo documentation site with hyperlinked personas, organisations, timeline, and org charts

**Independent Test**: Run `dotnet test --filter Category=CanonValidation` (≥15 personas, ≥8 orgs, zero violations); build Hugo site and confirm hyperlinked pages in `docs/`

### Implementation for User Story 1

- [X] T038 [P] [US1] Create `src/Turpinverse.Data/canon/tone-guidelines.json` with humour principles and examples per FR-011
- [X] T039 [P] [US1] Create `src/Turpinverse.Data/canon/personas.json` with ≥15 personas (Dick Turpin, Elizabeth Millington, Essex Gang members, Matthew King, Mary Brazier, Richard Bayes, etc.) per FR-003
- [X] T040 [P] [US1] Create `src/Turpinverse.Data/canon/organisations.json` with ≥8 organisations and parent hierarchies per FR-004
- [X] T041 [P] [US1] Create `src/Turpinverse.Data/canon/events.json` with ≥10 timeline events (historical and legend categories) per data-model.md
- [X] T042 [P] [US1] Create `src/Turpinverse.Data/canon/aliases.json` including John Palmer → dick-turpin mapping per FR-012
- [X] T043 [US1] Ensure bidirectional persona ↔ organisation membership consistency across `personas.json` and `organisations.json` per VR-003
- [X] T044 [US1] Mark fictional extensions in persona biographies with `> *Fictional extension*` convention per data-model.md
- [X] T045 [US1] Configure embedded resources in `src/Turpinverse.Data/Turpinverse.Data.csproj` for all canon JSON files
- [X] T046 [US1] Implement canon schema validation against `specs/001-turpinverse-universe/contracts/canon-schema.json` in `src/Turpinverse.Core/Validation/CanonSchemaValidator.cs`
- [X] T047 [US1] Update `tests/Turpinverse.Core.UnitTests/Validation/CanonValidatorTests.cs` to pass all VR-001–VR-010 checks
- [X] T048 [US1] Implement Hugo content generator in `tools/generate-hugo-content/Turpinverse.Tools.GenerateHugoContent/Program.cs` reading canon JSON and emitting markdown to `site/content/`
- [X] T049 [P] [US1] Create persona page layout in `site/layouts/personas/single.html` with cross-links to organisations
- [X] T050 [P] [US1] Create organisation page layout in `site/layouts/organisations/single.html` with member links
- [X] T051 [P] [US1] Create timeline list layout in `site/layouts/timeline/list.html` sorted by event date
- [X] T052 [US1] Create org-chart partial in `site/layouts/partials/org-chart.html` rendering hierarchy from `site/data/organisations.json`
- [X] T053 [P] [US1] Create site home page in `site/content/_index.md` explaining Turpinverse goals and usage
- [X] T054 [P] [US1] Create getting-started guide in `site/content/guides/getting-started.md`
- [X] T055 [P] [US1] Create base HTML layout in `site/layouts/_default/baseof.html` with navigation
- [X] T056 [US1] Create GitHub Actions workflow in `.github/workflows/hugo-pages.yml` to generate content, build Hugo to `docs/`, and deploy GitHub Pages
- [X] T057 [US1] Create GitHub Actions CI workflow in `.github/workflows/ci.yml` for `dotnet build` and `dotnet test`
- [X] T058 [US1] Write integration test in `tests/Turpinverse.IntegrationTests/HugoContentGeneratorTests.cs` verifying generated content structure

**Checkpoint**: Canon validation tests pass; `hugo --minify` in `site/` produces hyperlinked `docs/` output

---

## Phase 4: User Story 2 - Generate CRM-Ready Sample Datasets (Priority: P2)

**Goal**: Blazor Server app exports contacts, accounts, deals, and cases as CSV files with intact cross-references

**Independent Test**: Download all 4 CSVs from `/export` dashboard; run `dotnet test --filter Category=CsvExport` and `Category=CrossReference` — zero orphan references

### Tests for User Story 2 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T059 [P] [US2] Write failing CSV export integration tests in `tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs` with `[Trait("Category", "CsvExport")]`
- [X] T060 [P] [US2] Write failing cross-reference tests in `tests/Turpinverse.IntegrationTests/Export/CrossReferenceTests.cs` with `[Trait("Category", "CrossReference")]`
- [X] T061 [P] [US2] Write failing export API contract tests in `tests/Turpinverse.IntegrationTests/Export/ExportApiTests.cs` per `specs/001-turpinverse-universe/contracts/export-api.md`

### Implementation for User Story 2

- [X] T062 [P] [US2] Create `src/Turpinverse.Data/canon/deals.json` with ≥20 deals referencing valid persona and organisation IDs per FR-014
- [X] T063 [P] [US2] Create `src/Turpinverse.Data/canon/cases.json` with ≥15 cases referencing valid persona and organisation IDs per FR-014
- [X] T064 [P] [US2] Create `Contact` export DTO in `src/Turpinverse.Core/Export/ContactExport.cs` mapped from Persona per data-model.md
- [X] T065 [P] [US2] Create `AccountExport` DTO in `src/Turpinverse.Core/Export/AccountExport.cs` mapped from Organisation
- [X] T066 [P] [US2] Create `DealExport` DTO in `src/Turpinverse.Core/Export/DealExport.cs`
- [X] T067 [P] [US2] Create `CaseExport` DTO in `src/Turpinverse.Core/Export/CaseExport.cs`
- [X] T068 [US2] Implement `IExportService` interface in `src/Turpinverse.Core/Abstractions/IExportService.cs`
- [X] T069 [US2] Implement `CsvExportService` in `src/Turpinverse.Core/Export/CsvExportService.cs` using CsvHelper with UTF-8 BOM per export-api.md
- [X] T070 [US2] Implement `ExportMapper` in `src/Turpinverse.Core/Export/ExportMapper.cs` deriving Contact and Account records from canon personas and organisations
- [X] T071 [US2] Create `ExportEndpoints` in `src/Turpinverse.Web/Endpoints/ExportEndpoints.cs` implementing `GET /api/export/{dataset}` per export-api.md
- [X] T072 [US2] Add `GET /api/export/manifest` endpoint in `src/Turpinverse.Web/Endpoints/ExportEndpoints.cs`
- [X] T073 [US2] Add `GET /api/canon/validate` endpoint in `src/Turpinverse.Web/Endpoints/CanonEndpoints.cs` returning validation results per export-api.md
- [X] T074 [US2] Create export dashboard page in `src/Turpinverse.Web/Components/Pages/Export.razor` with dataset cards and download buttons
- [X] T075 [US2] Create `DatasetCard.razor` component in `src/Turpinverse.Web/Components/Export/DatasetCard.razor` showing row count and description
- [X] T076 [US2] Create `PreviewTable.razor` component in `src/Turpinverse.Web/Components/Export/PreviewTable.razor` showing first 5 rows of selected dataset
- [X] T077 [US2] Register export and canon endpoints in `src/Turpinverse.Web/Program.cs`
- [X] T078 [US2] Update export integration tests in `tests/Turpinverse.IntegrationTests/Export/` to pass all contract and cross-reference checks

**Checkpoint**: All 4 CSVs download with correct headers and row counts; cross-reference tests pass

---

## Phase 5: User Story 3 - Deliver Recognisable Humour Without Breaking Demo Credibility (Priority: P3)

**Goal**: Dashboard visualisations and tone validation ensure humour lands in a semi-professional demo context

**Independent Test**: Present 10 sample records to reviewers; ≥80% rate ≥3/5 professionalism; ≥2/3 identify Turpinverse theme within 5 records (SC-003, SC-006)

### Implementation for User Story 3

- [X] T079 [US3] Implement `ToneValidator` in `src/Turpinverse.Core/Validation/ToneValidator.cs` checking records against forbidden patterns in tone-guidelines.json
- [X] T080 [US3] Add tone validation to `CanonValidator` in `src/Turpinverse.Core/Validation/CanonValidator.cs` for persona notes and organisation descriptions
- [X] T081 [P] [US3] Create `LucideIcon.razor` component in `src/Turpinverse.Web/Components/Shared/LucideIcon.razor` rendering inline SVG icons
- [X] T082 [P] [US3] Create Tailwind input stylesheet in `src/Turpinverse.Web/Styles/app.css` with Turpinverse theme tokens
- [X] T083 [US3] Configure Tailwind build target in `src/Turpinverse.Web/Turpinverse.Web.csproj` outputting to `wwwroot/css/app.min.css`
- [X] T084 [US3] Create Chart.js interop service in `src/Turpinverse.Web/wwwroot/js/chartInterop.js`
- [X] T085 [US3] Create `ChartComponent.razor` in `src/Turpinverse.Web/Components/Charts/ChartComponent.razor` wrapping Chart.js via `IJSRuntime`
- [X] T086 [US3] Create `PipelineChart.razor` in `src/Turpinverse.Web/Components/Charts/PipelineChart.razor` showing deal stage distribution bar chart
- [X] T087 [US3] Create `SummaryChart.razor` in `src/Turpinverse.Web/Components/Charts/SummaryChart.razor` showing entity count doughnut chart
- [X] T088 [US3] Add pipeline and summary charts to `src/Turpinverse.Web/Components/Pages/Export.razor`
- [X] T089 [US3] Add validation status badge to export dashboard in `src/Turpinverse.Web/Components/Pages/Export.razor` calling `/api/canon/validate`
- [X] T090 [US3] Apply Tailwind styling and Lucide icons across `src/Turpinverse.Web/Components/Layout/MainLayout.razor` and navigation
- [X] T091 [US3] Write tone validation unit tests in `tests/Turpinverse.Core.UnitTests/Validation/ToneValidatorTests.cs`
- [X] T092 [US3] Write bUnit component tests for export dashboard in `tests/Turpinverse.Web.UnitTests/Components/ExportPageTests.cs`

**Checkpoint**: Dashboard renders charts and icons; tone validator rejects forbidden patterns; export page passes bUnit tests

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Documentation, OSS polish, and end-to-end validation

- [X] T093 [P] Update `README.md` with full quickstart instructions from `specs/001-turpinverse-universe/quickstart.md`
- [X] T094 [P] Add NuGet package references documentation to `README.md` (Aspire, CsvHelper, xUnit, bUnit)
- [X] T095 Verify Aspire dashboard launches `Turpinverse.Web` via `dotnet run --project src/Turpinverse.AppHost`
- [X] T096 Run full quickstart validation scenarios 1–6 from `specs/001-turpinverse-universe/quickstart.md`
- [X] T097 [P] Add code coverage reporting to `.github/workflows/ci.yml` using coverlet
- [X] T098 Review all canon entries for Wikipedia accuracy and tongue-in-cheek tone balance per SC-001 and SC-006
- [X] T099 Ensure `docs/` output is gitignored locally but committed by CI workflow in `.gitignore`
- [X] T100 Final `dotnet test` run across all test projects confirming 100% pass rate

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **US1 (Phase 3)**: Depends on Foundational — delivers MVP canon + Hugo site
- **US2 (Phase 4)**: Depends on Foundational + US1 canon data files — export requires populated canon
- **US3 (Phase 5)**: Depends on US2 export dashboard existing — adds visualisation and tone polish
- **Polish (Phase 6)**: Depends on US1, US2, and US3 completion

### User Story Dependencies

- **US1 (P1)**: Can start after Foundational — no dependency on US2/US3
- **US2 (P2)**: Requires US1 canon JSON files (T038–T042) — deals/cases can be added in US2 but personas/orgs/events/aliases must exist
- **US3 (P3)**: Requires US2 export dashboard — tone validation can run on canon independently but charts need export data

### Within Each User Story

- Tests (where included) MUST be written and FAIL before implementation
- Models before services
- Services before endpoints/UI
- Canon data before content generator
- Content generator before Hugo build
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel (T003–T006, T013–T015, T017–T018)
- All Foundational model tasks T020–T026 can run in parallel
- US1 canon JSON files T038–T042 can be authored in parallel by different contributors
- US1 Hugo layouts T049–T051, T053–T055 can run in parallel after content generator exists
- US2 export DTOs T064–T067 and test files T059–T061 can run in parallel
- US3 Lucide, Tailwind, and Chart.js tasks T081–T082 can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all canon JSON authoring in parallel:
Task T039: "Create personas.json in src/Turpinverse.Data/canon/personas.json"
Task T040: "Create organisations.json in src/Turpinverse.Data/canon/organisations.json"
Task T041: "Create events.json in src/Turpinverse.Data/canon/events.json"
Task T042: "Create aliases.json in src/Turpinverse.Data/canon/aliases.json"

# Launch all Hugo layouts in parallel (after T048):
Task T049: "Create persona layout in site/layouts/personas/single.html"
Task T050: "Create organisation layout in site/layouts/organisations/single.html"
Task T051: "Create timeline layout in site/layouts/timeline/list.html"
```

---

## Parallel Example: User Story 2

```bash
# Launch failing tests first in parallel:
Task T059: "CsvExportTests in tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs"
Task T060: "CrossReferenceTests in tests/Turpinverse.IntegrationTests/Export/CrossReferenceTests.cs"
Task T061: "ExportApiTests in tests/Turpinverse.IntegrationTests/Export/ExportApiTests.cs"

# Then launch DTOs in parallel:
Task T064–T067: Export DTOs in src/Turpinverse.Core/Export/
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1 (canon + Hugo site)
4. **STOP and VALIDATE**: Canon tests pass; Hugo site builds with hyperlinked content
5. Deploy docs to GitHub Pages

### Incremental Delivery

1. Setup + Foundational → foundation ready
2. US1 → Canon + Hugo docs → Validate → Deploy (MVP!)
3. US2 → Blazor CSV export → Validate → Demo-ready datasets
4. US3 → Charts, tone polish → Validate → Full demo experience
5. Polish → CI, README, end-to-end validation

### Suggested MVP Scope

**User Story 1 only** (Phases 1–3): Delivers the Turpinverse canon dataset, validation framework, Hugo documentation site, and GitHub Pages deployment. This satisfies FR-001 through FR-004 and FR-011–FR-012 without requiring the Blazor app.

---

## Notes

- [P] tasks = different files, no dependencies on incomplete tasks in the same phase
- [Story] label maps task to user story for traceability
- Canon JSON in `src/Turpinverse.Data/canon/` is the single source of truth for both Hugo and Blazor
- Run `dotnet run --project tools/generate-hugo-content/...` before every Hugo build
- Commit after each phase checkpoint
- Total tasks: **100** (Setup: 19, Foundational: 18, US1: 21, US2: 20, US3: 14, Polish: 8)
