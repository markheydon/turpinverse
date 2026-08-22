---

description: "Task list for Human-Facing Channel Alignment"
---

# Tasks: Human-Facing Channel Alignment

**Input**: Design documents from `/specs/004-hugo-blazor-charter/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Failing-first tests are in scope per Constitution Principle III and [quickstart.md](./quickstart.md). Test tasks MUST be written and confirmed failing before implementation in each user story phase.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story. Each task traces to spec requirements (Constitution Principle I).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Canon**: `src/Turpinverse.Data/canon/`
- **Core**: `src/Turpinverse.Core/` (Hugo, Export, Abstractions)
- **Web**: `src/Turpinverse.Web/` (Endpoints, Components)
- **Hugo site**: `site/content/`, `site/data/`, `site/layouts/`, `site/hugo.toml`
- **Tests**: `tests/Turpinverse.Core.UnitTests/`, `tests/Turpinverse.Web.UnitTests/`, `tests/Turpinverse.IntegrationTests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm prerequisites and scaffold test files for failing-first workflow

- [X] T001 Confirm canon CRM files exist at `src/Turpinverse.Data/canon/deals.json` and `src/Turpinverse.Data/canon/cases.json` with expected row counts for generator completeness checks
- [X] T002 [P] Create failing-first Hugo test stub file at `tests/Turpinverse.Core.UnitTests/Hugo/HugoDealCaseGeneratorTests.cs` (empty or `[Fact(Skip=...)]` placeholders matching quickstart filter `HugoDeal|HugoCase`)
- [X] T003 [P] Create failing-first export filter test stub files at `tests/Turpinverse.Core.UnitTests/Export/ExportFilterTests.cs` and `tests/Turpinverse.IntegrationTests/Export/ExportFilterApiTests.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared Hugo identity partials and fallbacks used by US1 and US2

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T004 [P] Create `site/layouts/partials/deal-title.html` resolving deal slug to `dealName` via `site.GetPage` with FR-008 fallback ("Unknown related party")
- [X] T005 [P] Create `site/layouts/partials/case-title.html` resolving case slug to `subject` with FR-008 fallback
- [X] T006 [P] Create `site/layouts/partials/event-title.html` resolving event slug to event title with FR-008 fallback
- [X] T007 Update `site/layouts/partials/org-title.html` and `site/layouts/partials/persona-title.html` to use human-readable fallback labels instead of echoing the slug when `GetPage` fails (FR-008)

**Checkpoint**: Foundation ready — user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Read the full universe on the public site (Priority: P1) 🎯 MVP

**Goal**: Every charter-listed human-readable entity type (people, organisations, timeline, career/portfolio, deals, cases) has browsable index and detail pages with bidirectional named links.

**Independent Test**: Open the public site with no export app running. Reach deals and cases from primary nav or home. Follow people ↔ organisations ↔ events ↔ deals/cases using display names only.

### Tests for User Story 1

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T008 [P] [US1] Write failing generator tests in `tests/Turpinverse.Core.UnitTests/Hugo/HugoDealCaseGeneratorTests.cs` asserting one markdown file per canon deal/case, `site/data/deals.json` and `site/data/cases.json`, and human-readable titles (FR-001, FR-011)
- [X] T009 [P] [US1] Write failing bidirectional link tests in `tests/Turpinverse.Core.UnitTests/Hugo/HugoShowcaseLinkTests.cs` asserting persona/org front matter or data JSON includes related deal/case slugs when canon FKs exist (FR-012)

### Implementation for User Story 1

- [X] T010 [US1] Extend `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs` to emit `site/content/deals/` and `site/content/cases/` (section `_index.md` + one `.md` per record) and `site/data/deals.json` / `site/data/cases.json` per [contracts/hugo-showcase.md](./contracts/hugo-showcase.md)
- [X] T011 [P] [US1] Implement or complete `site/layouts/deals/list.html` and `site/layouts/deals/single.html` showing deal name, description, stage, amount, close date, and named related person/org links
- [X] T012 [P] [US1] Implement or complete `site/layouts/cases/list.html` and `site/layouts/cases/single.html` showing subject, description, status, priority, and named related person/org/event links
- [X] T013 [US1] Add related deals and cases sections to `site/layouts/personas/single.html` using `site/data/deals.json` and `site/data/cases.json` filtered by `contactId` (FR-012)
- [X] T014 [US1] Add related deals and cases sections to `site/layouts/organisations/single.html` filtered by `accountId` (FR-012)
- [X] T015 [US1] Extend `site/layouts/timeline/list.html` to list related cases by `relatedEventId` under each event using `case-title.html` partial (FR-012; no invented deal↔event links)
- [X] T016 [US1] Add Deals and Cases entries to `site/hugo.toml` `menu.main` and home quick links (e.g. `site/layouts/partials/home/custom.html` or `site/layouts/index.html`) with same discoverability as Personas, Organisations, Timeline (FR-011, SC-001)

**Checkpoint**: User Story 1 is independently testable — full readable universe on Hugo without Blazor

---

## Phase 4: User Story 2 - Public site stays a reader, not an importer (Priority: P1)

**Goal**: Showcase pages have no export/download plumbing; reader-facing copy uses display names, never join keys or CSV column names as identity.

**Independent Test**: Walk every primary showcase section. Confirm zero download/import controls and zero join-key-as-label in body copy, headings, or relationship lists.

### Tests for User Story 2

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T017 [P] [US2] Write failing identity-rule tests in `tests/Turpinverse.Core.UnitTests/Hugo/HugoIdentityRulesTests.cs` asserting generated deal/case markdown and layout output do not expose `contactId`, `accountId`, `dealId`, or `caseId` as visible labels (FR-003, SC-002)

### Implementation for User Story 2

- [X] T018 [US2] Audit showcase layouts under `site/layouts/` (personas, organisations, timeline, deals, cases) and remove or avoid any download/import/API-key workflow controls per [contracts/hugo-showcase.md](./contracts/hugo-showcase.md) (FR-004)
- [X] T019 [US2] Audit `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs` output and deal/case/persona/org layouts so body copy, headings, and chip text use display-name partials only; join keys remain in front matter / `site/data` for routing (FR-003, FR-009)
- [X] T020 [US2] Verify `site/content/guides/getting-started/` (or equivalent Getting Started page) points readers to the local Blazor app for importable files and does not offer in-page CSV download (FR-004, FR-007)

**Checkpoint**: User Stories 1 and 2 together satisfy the Hugo channel charter (showcase complete + channel split)

---

## Phase 5: User Story 3 - Explore, filter, and download in the export app (Priority: P2)

**Goal**: Dataset pages support facets; preview and CSV download share the same server-side `ExportFilter`; zero-match filters block download with 409.

**Independent Test**: With Aspire running, browse → filter → preview → download for contacts, accounts, deals, and cases. Confirm filtered CSV contains only matching rows; zero-match shows empty state and blocks download.

### Tests for User Story 3

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T021 [P] [US3] Write failing unit tests in `tests/Turpinverse.Core.UnitTests/Export/ExportFilterTests.cs` for AND-facet matching, ignored params per dataset, and full-dataset when filter empty (data-model.md)
- [X] T022 [P] [US3] Write failing integration tests in `tests/Turpinverse.IntegrationTests/Export/ExportFilterApiTests.cs` for filtered preview rows, filtered CSV body, and 409 `application/problem+json` on zero-match download per [contracts/export-filter-api.md](./contracts/export-filter-api.md) (FR-005, FR-010)
- [X] T023 [P] [US3] Write failing bUnit tests in `tests/Turpinverse.Web.UnitTests/Components/CrmEntityPageFilterTests.cs` for facet UI, empty-state copy, and disabled download when filter matches zero rows

### Implementation for User Story 3

- [X] T024 [US3] Create `ExportFilter` record in `src/Turpinverse.Core/Export/ExportFilter.cs` with dataset-specific facets per [data-model.md](./data-model.md)
- [X] T025 [US3] Extend `src/Turpinverse.Core/Abstractions/IExportService.cs` so `PreviewAsync` and `ExportCsvAsync` accept optional `ExportFilter?`
- [X] T026 [US3] Apply `ExportFilter` predicate in `src/Turpinverse.Core/Export/CsvExportService.cs` for both preview and CSV export; return all matching rows when filter applied (raise truncation only for unfiltered preview cap)
- [X] T027 [US3] Parse filter query parameters and return 409 Conflict on zero-match download in `src/Turpinverse.Web/Endpoints/ExportEndpoints.cs` per [contracts/export-filter-api.md](./contracts/export-filter-api.md)
- [X] T028 [US3] Add facet controls, filtered preview reload, empty-state message, and download URL with matching query string in `src/Turpinverse.Web/Components/Crm/CrmEntityPage.razor`
- [X] T029 [P] [US3] Wire dataset-specific facets on `src/Turpinverse.Web/Components/Pages/Contacts.razor`, `Accounts.razor`, `Deals.razor`, and `Cases.razor` per facet table in research.md §4

**Checkpoint**: User Story 3 is independently testable — explore/filter/download without new Hugo pages

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Documentation, charter gap closure, and end-to-end validation

- [X] T030 [P] Update known-gap rows in `docs/product-surfaces.md` to reflect deals/cases Hugo pages and identity-rule fixes (FR-007, SC-006)
- [X] T031 Run [quickstart.md](./quickstart.md) validation: `dotnet run --project src/Turpinverse.Tools.GenerateHugoContent`, Hugo serve, and Aspire export-app browse/filter/download checks
- [X] T032 Run full test suite (`dotnet test`) including `Category=CsvExport` and `Category=CrossReference` regressions

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories (shared Hugo partials)
- **User Story 1 (Phase 3)**: Depends on Foundational — no dependency on US2/US3 implementation
- **User Story 2 (Phase 4)**: Depends on Foundational; best completed after US1 generator/layouts exist so audit targets real output
- **User Story 3 (Phase 5)**: Depends on Setup only for test stubs; independent of Hugo stories (can run in parallel with US1/US2 after Phase 2 if staffed separately)
- **Polish (Phase 6)**: Depends on US1, US2, and US3 completion

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational — delivers Hugo showcase completeness (MVP for public channel)
- **User Story 2 (P1)**: Can start after Foundational; logically follows US1 layouts but is independently testable via layout/copy audit
- **User Story 3 (P2)**: Independent of Hugo pages — Blazor + API only; shares no runtime dependency with US1/US2

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Hugo: generator before layout wiring; layouts before nav/home links
- Export: `ExportFilter` model → service → endpoints → UI
- Story complete before moving to Polish

### Parallel Opportunities

- T002 and T003 (test stubs) in parallel
- T004, T005, T006 (title partials) in parallel
- T008 and T009 (US1 tests) in parallel
- T011 and T012 (deal/case layouts) in parallel
- T021, T022, T023 (US3 tests) in parallel
- T029 (four dataset pages) in parallel
- After Phase 2: US3 (Blazor team) can proceed in parallel with US1/US2 (Hugo team)

---

## Parallel Example: User Story 1

```bash
# Launch US1 tests together (confirm they fail first):
Task: "Write failing generator tests in tests/Turpinverse.Core.UnitTests/Hugo/HugoDealCaseGeneratorTests.cs"
Task: "Write failing bidirectional link tests in tests/Turpinverse.Core.UnitTests/Hugo/HugoShowcaseLinkTests.cs"

# Launch deal and case layouts together after T010:
Task: "Implement site/layouts/deals/list.html and site/layouts/deals/single.html"
Task: "Implement site/layouts/cases/list.html and site/layouts/cases/single.html"
```

---

## Parallel Example: User Story 3

```bash
# Launch US3 tests together (confirm they fail first):
Task: "ExportFilter unit tests in tests/Turpinverse.Core.UnitTests/Export/ExportFilterTests.cs"
Task: "API integration tests in tests/Turpinverse.IntegrationTests/Export/ExportFilterApiTests.cs"
Task: "bUnit tests in tests/Turpinverse.Web.UnitTests/Components/CrmEntityPageFilterTests.cs"

# Launch dataset page facet wiring together after T028:
Task: "Wire facets on Contacts.razor, Accounts.razor, Deals.razor, Cases.razor"
```

---

## Implementation Strategy

### MVP First (User Story 1 + User Story 2)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — shared partials)
3. Complete Phase 3: User Story 1 (Hugo showcase completeness)
4. Complete Phase 4: User Story 2 (channel split on Hugo)
5. **STOP and VALIDATE**: Public site readable universe without Blazor; no export plumbing on showcase pages
6. Deploy/demo Hugo site if ready

### Incremental Delivery

1. Setup + Foundational → shared Hugo identity infrastructure ready
2. User Story 1 → Test independently → Hugo MVP (deals/cases + bidirectional links)
3. User Story 2 → Test independently → Hugo charter compliance (no IDs, no downloads)
4. User Story 3 → Test independently → Blazor filter/download parity
5. Polish → docs and quickstart validation

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (Hugo generator + layouts + nav)
   - Developer B: User Story 2 (identity audit + Getting Started) after US1 layouts land
   - Developer C: User Story 3 (ExportFilter + API + CrmEntityPage) — fully parallel with A/B
3. Integrate in Polish phase with quickstart validation

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Career/portfolio dual publication on persona pages is unchanged (FR-006) — regression-check only in T031/T032
- Projects dataset is out of US3 facet scope unless trivial to share machinery (research.md §4)

## Phase 7: Convergence

- [ ] T033 Update `site/README.md` Hugo scope and generator output list to include deals/cases showcase pages and `site/data/deals.json` / `site/data/cases.json` per FR-007 and Constitution VIII (partial)
- [ ] T034 Update `site/content/guides/getting-started.md` “Two ways to use Turpinverse” Hugo channel row to list deals and cases alongside personas, organisations, timeline, and career/portfolio per FR-007 (partial)
- [ ] T035 Update `site/content/_index.md` welcome copy so the showcase description and browse CTA mention deals and cases (not only personas and organisations) per FR-007 and SC-001 (partial)
