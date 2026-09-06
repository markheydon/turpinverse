---

description: "Task list for Career and Portfolio Demo Data feature implementation"
---

# Tasks: Career and Portfolio Demo Data

**Input**: Design documents from `/specs/003-career-portfolio-data/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Failing-first CanonValidation, Hugo generation, and Blazor contact-detail tests per plan.md, quickstart.md, and Constitution Principle III.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story. Each task traces to spec requirements FR-001–FR-015 and US1–US3.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- Canon JSON: `src/Turpinverse.Data/canon/`
- Core models/validation: `src/Turpinverse.Core/`
- Blazor UI: `src/Turpinverse.Web/Components/`
- Hugo layouts/data: `site/layouts/`, `site/data/`
- Tests: `tests/Turpinverse.Core.UnitTests/`, `tests/Turpinverse.Web.UnitTests/`
- Runtime schema: `specs/001-turpinverse-universe/contracts/canon-schema.json`
- Developer docs: `docs/career-portfolio-mapping.md`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Canon file scaffolding and feature branch readiness

- [X] T001 Create empty canon array files `experience.json`, `education.json`, `projects.json`, and `achievements.json` in `src/Turpinverse.Data/canon/` (each `[]` until authored)
- [X] T002 Verify embedded resource glob in `src/Turpinverse.Data/Turpinverse.Data.csproj` includes new canon files via `canon\*.json`
- [X] T003 Confirm feature branch `003-career-portfolio-data` is active and `specs/003-career-portfolio-data/` design artifacts are present

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core models, schema, repository, validator, and presenter that MUST complete before user story work

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Tests for Foundational (failing-first) ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T004 [P] Add failing CanonValidator career/portfolio fixture tests (VR-020–VR-027) in `tests/Turpinverse.Core.UnitTests/Validation/CareerPortfolioValidatorTests.cs` with `[Trait("Category", "CanonValidation")]`
- [X] T005 [P] Add failing CanonSchemaValidator tests for Experience, Role, Education, Project, Achievement in `tests/Turpinverse.Core.UnitTests/Validation/CareerPortfolioSchemaValidatorTests.cs`
- [X] T006 [P] Add failing JsonCanonRepository load tests for the four new canon files in `tests/Turpinverse.Core.UnitTests/Data/JsonCanonRepositoryTests.cs`
- [X] T007 [P] Add failing CareerPortfolioPresenter sort-order tests (FR-015) in `tests/Turpinverse.Core.UnitTests/Career/CareerPortfolioPresenterTests.cs`

### Implementation for Foundational

- [X] T008 [P] Create `FeaturedLink` model in `src/Turpinverse.Core/Models/FeaturedLink.cs`
- [X] T009 [P] Create `Role` model in `src/Turpinverse.Core/Models/Role.cs`
- [X] T010 [P] Create `Experience` model in `src/Turpinverse.Core/Models/Experience.cs`
- [X] T011 [P] Create `Education` model in `src/Turpinverse.Core/Models/Education.cs`
- [X] T012 [P] Create `Project` model in `src/Turpinverse.Core/Models/Project.cs`
- [X] T013 [P] Create `Achievement` model in `src/Turpinverse.Core/Models/Achievement.cs`
- [X] T014 Extend `Canon` with `Experience`, `Education`, `Projects`, and `Achievements` collections in `src/Turpinverse.Core/Models/Canon.cs`
- [X] T015 Update `JsonCanonRepository` to load the four new files in `src/Turpinverse.Data/Repositories/JsonCanonRepository.cs`
- [X] T016 Merge additive types from `specs/003-career-portfolio-data/contracts/career-portfolio-schema.json` into `specs/001-turpinverse-universe/contracts/canon-schema.json` (`Canon` required arrays and `$defs`)
- [X] T017 Extend `CanonSchemaValidator` for new entity types in `src/Turpinverse.Core/Validation/CanonSchemaValidator.cs`
- [X] T018 Implement VR-020–VR-027 validation methods and add `experience`, `education`, `projects`, `achievements` to result counts in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [X] T019 Create `CareerPortfolioPresenter` with FR-015 reverse-chronological ordering in `src/Turpinverse.Core/Career/CareerPortfolioPresenter.cs`
- [X] T020 Register `CareerPortfolioPresenter` in `src/Turpinverse.Core/DependencyInjection/ServiceCollectionExtensions.cs`

**Checkpoint**: Foundation ready — models load, schema validates structure, validator tests fail on volume until data ships, presenter sort tests pass

---

## Phase 3: User Story 1 - Career history for the primary profile subject (Priority: P1) 🎯 MVP

**Goal**: Structured work experience and education for Richard Turpin (`dick-turpin`) visible on Hugo persona pages and Blazor contact detail with omit-empty sections and FR-015 ordering

**Independent Test**: Primary persona meets FR-003–FR-005 volume and field coverage; organisation links resolve; experience and education appear on Hugo `/personas/dick-turpin/` and Blazor `/contacts/dick-turpin`; `Category=CanonValidation` passes for career rows; reverse-chronological order verified

### Tests for User Story 1 (failing-first) ⚠️

- [X] T021 [P] [US1] Add failing HugoContentGenerator experience/education emission tests in `tests/Turpinverse.Core.UnitTests/Hugo/CareerPortfolioHugoTests.cs` with `[Trait("Category", "CareerPortfolio")]`
- [X] T022 [P] [US1] Add failing Blazor contact detail career section tests (experience, education, omit-empty) in `tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs` with `[Trait("Category", "CareerPortfolio")]`

### Implementation for User Story 1

- [X] T023 [US1] Author `experience.json` for `dick-turpin` in `src/Turpinverse.Data/canon/experience.json` (≥3 organisation groupings, nested roles with contemporary dates, ≥1 extra/tooltip, ≥1 featured link, canon org links where fit)
- [X] T024 [US1] Author `education.json` for `dick-turpin` in `src/Turpinverse.Data/canon/education.json` (≥2 programmes, ≥1 featured link, contemporary dates)
- [X] T025 [US1] Extend `HugoContentGenerator` to emit experience and education into `site/data/` in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [X] T026 [US1] Add Experience and Education sections with omit-empty to `site/layouts/personas/single.html` (reverse-chronological via generated data order)
- [X] T027 [US1] Create contact detail page `src/Turpinverse.Web/Components/Pages/ContactDetail.razor` at route `/contacts/{contactId}`
- [X] T028 [P] [US1] Create `ExperienceSection.razor` in `src/Turpinverse.Web/Components/Career/ExperienceSection.razor` using `CareerPortfolioPresenter`
- [X] T029 [P] [US1] Create `EducationSection.razor` in `src/Turpinverse.Web/Components/Career/EducationSection.razor` using `CareerPortfolioPresenter`
- [X] T030 [US1] Register `/contacts/{contactId}` route in `src/Turpinverse.Web/Components/Routes.razor`
- [X] T031 [US1] Link Contacts table rows to contact detail in `src/Turpinverse.Web/Components/Pages/Contacts.razor`
- [X] T032 [P] [US1] Add career section styles in `src/Turpinverse.Web/Styles/app.css` (or `wwwroot/app.css` if that is the build source)

**Checkpoint**: User Story 1 fully functional — career history on Hugo and Blazor for `dick-turpin`; portfolio sections omitted; CanonValidation passes career volume rules for experience and education

---

## Phase 4: User Story 2 - Portfolio projects and achievements (Priority: P2)

**Goal**: Shared catalog projects and achievements for the primary subject with CRM-style project organisation links, shared membership (FR-014), and publication parity on Hugo and Blazor

**Independent Test**: Primary persona meets FR-007–FR-008; projects show organisation account; shared catalog item appears with same id/title on two persona pages; persona with no rows of a type omits that section; CanonValidation passes portfolio rules

### Tests for User Story 2 (failing-first) ⚠️

- [X] T033 [P] [US2] Add failing HugoContentGenerator projects/achievements tests in `tests/Turpinverse.Core.UnitTests/Hugo/CareerPortfolioHugoTests.cs`
- [X] T034 [P] [US2] Add failing Blazor portfolio section and omit-empty tests in `tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs`
- [X] T035 [P] [US2] Add failing FR-014 shared catalog membership test (VR-027) in `tests/Turpinverse.Core.UnitTests/Validation/CareerPortfolioValidatorTests.cs`

### Implementation for User Story 2

- [X] T036 [US2] Author `projects.json` in `src/Turpinverse.Data/canon/projects.json` (≥3 with `dick-turpin` in `personaIds`, required fields, `organisationId`, ≥1 featured CTA)
- [X] T037 [US2] Author `achievements.json` in `src/Turpinverse.Data/canon/achievements.json` (≥4 with `dick-turpin`, mix of with/without URL)
- [X] T038 [US2] Add FR-014 shared membership linking `dick-turpin` and a deputy persona in `src/Turpinverse.Data/canon/projects.json` or `src/Turpinverse.Data/canon/achievements.json`
- [X] T039 [US2] Extend `HugoContentGenerator` for projects and achievements in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [X] T040 [US2] Add Projects and Achievements sections with omit-empty to `site/layouts/personas/single.html`
- [X] T041 [P] [US2] Create `ProjectsSection.razor` in `src/Turpinverse.Web/Components/Career/ProjectsSection.razor`
- [X] T042 [P] [US2] Create `AchievementsSection.razor` in `src/Turpinverse.Web/Components/Career/AchievementsSection.razor`
- [X] T043 [US2] Integrate portfolio sections into `src/Turpinverse.Web/Components/Pages/ContactDetail.razor` with omit-empty for all four types

**Checkpoint**: User Stories 1 and 2 both independently verifiable — full career and portfolio on Hugo and Blazor; shared catalog demonstrated; empty section types omitted

---

## Phase 5: User Story 3 - Consumer-agnostic records with a mapping note (Priority: P3)

**Goal**: Repository-only developer documentation mapping generic canon entities to a typical personal-site profile layout without changing the public theme or adding a downstream profile-demo site

**Independent Test**: `docs/career-portfolio-mapping.md` maps every required generic field to experience, education, projects, and achievements slots; source of truth is canon JSON not theme params; public site theme unchanged

### Implementation for User Story 3

- [X] T044 [P] [US3] Create mapping stub `docs/career-portfolio-mapping.md` covering Experience, Role, Education, Project, Achievement, and FeaturedLink fields
- [X] T045 [US3] Document projection examples (experience grouping vs flat jobs, project detail page) and explicitly state canon JSON is source of truth in `docs/career-portfolio-mapping.md`

**Checkpoint**: Developer can complete SC-005 mapping exercise from repo docs only; no public-site mapping page required

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validation gates, content regeneration, and quickstart verification

- [X] T046 Bump `canon.json` version in `src/Turpinverse.Data/canon/canon.json` when career/portfolio dataset ships
- [X] T047 Run `dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"` and resolve failures
- [X] T048 [P] Run `dotnet test tests/Turpinverse.Web.UnitTests --filter "Category=CareerPortfolio"` and resolve failures
- [X] T049 Regenerate Hugo content via `src/Turpinverse.Tools.GenerateHugoContent/Program.cs` and confirm `site/content/personas/dick-turpin.md` and `site/data/` career files
- [X] T050 [P] Build Hugo site (`site/` with `hugo --minify` or `scripts/Invoke-HugoSite.ps1`) and spot-check `/personas/dick-turpin/`
- [X] T051 Verify `/api/canon/validate` returns 200 with career/portfolio counts per `specs/003-career-portfolio-data/contracts/completeness.md`
- [X] T052 Execute quickstart.md Scenarios A–D (manual) and Scenario E negative fixtures in test code only

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — **BLOCKS all user stories**
- **User Story 1 (Phase 3)**: Depends on Foundational — MVP career slice
- **User Story 2 (Phase 4)**: Depends on Foundational; integrates with US1 contact detail and Hugo layout but portfolio data is independently testable once foundation exists
- **User Story 3 (Phase 5)**: Depends on Foundational models; can run in parallel with US1/US2 after T008–T014 define field names
- **Polish (Phase 6)**: Depends on US1–US3 completion for full gate

### User Story Dependencies

- **User Story 1 (P1)**: After Phase 2 — no dependency on US2/US3 for career-only validation
- **User Story 2 (P2)**: After Phase 2 — extends US1 publication surfaces; portfolio canon JSON is independent of experience/education authoring
- **User Story 3 (P3)**: After Phase 2 model names are stable — documentation only; lowest priority

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Canon JSON authoring after validator/schema foundation
- Hugo generator before layout sections
- Blazor sections before full contact detail integration
- Story checkpoint before moving to next priority

### Parallel Opportunities

- All Foundational model tasks T008–T013 can run in parallel
- Foundational test tasks T004–T007 can run in parallel
- US1 Blazor section components T028–T029 can run in parallel
- US2 portfolio section components T041–T042 can run in parallel
- Polish T048 and T050 can run in parallel after tests pass

---

## Parallel Example: User Story 1

```bash
# Failing tests together:
Task: "HugoContentGenerator experience/education tests in tests/Turpinverse.Core.UnitTests/Hugo/CareerPortfolioHugoTests.cs"
Task: "Contact detail career tests in tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs"

# Blazor section components together:
Task: "ExperienceSection.razor in src/Turpinverse.Web/Components/Career/ExperienceSection.razor"
Task: "EducationSection.razor in src/Turpinverse.Web/Components/Career/EducationSection.razor"
```

---

## Parallel Example: User Story 2

```bash
# Portfolio tests together:
Task: "Hugo portfolio tests in CareerPortfolioHugoTests.cs"
Task: "Blazor portfolio tests in ContactDetailTests.cs"
Task: "VR-027 shared catalog test in CareerPortfolioValidatorTests.cs"

# Section components together:
Task: "ProjectsSection.razor"
Task: "AchievementsSection.razor"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Hugo persona page + Blazor `/contacts/dick-turpin` show experience and education; CanonValidation passes career rules
5. Demo career slice without portfolio sections

### Incremental Delivery

1. Setup + Foundational → validator and presenter ready
2. Add User Story 1 → career history on both channels (MVP)
3. Add User Story 2 → portfolio + shared catalog on same surfaces
4. Add User Story 3 → developer mapping note
5. Polish → full quickstart and CI gates green

### Parallel Team Strategy

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: US1 canon JSON + Hugo career sections
   - Developer B: US1 Blazor contact detail
   - Developer C: US2 portfolio JSON + Hugo portfolio sections (after foundation)
3. Developer D: US3 mapping doc (can start once models exist)
4. Integrate and run Polish phase together

---

## Notes

- Primary subject persona id: `dick-turpin`; contemporary CV dates — do not constrain to historical birth/death years
- Projects require `organisationId` (CRM account); achievements persona-linked only
- One experience grouping per persona per organisation; rehire = additional role
- Omit empty section types on all person pages (Hugo and Blazor)
- CSV export columns unchanged (FR-012)
- No public theme switch; PaperMod remains the Hugo theme
- Commit after each task or logical group
- Verify tests fail before implementing
