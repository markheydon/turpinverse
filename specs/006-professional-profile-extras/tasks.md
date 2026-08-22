---

description: "Task list for Professional Profile Extras feature implementation"
---

# Tasks: Professional Profile Extras

**Input**: Design documents from `/specs/006-professional-profile-extras/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Failing-first CanonValidation, Hugo generation, and Blazor contact-detail tests per plan.md, quickstart.md, and Constitution Principle III.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story. Each task traces to spec requirements FR-001–FR-013 and US1–US3.

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
- Developer docs: `docs/product-surfaces.md`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Canon file scaffolding and feature branch readiness

- [x] T001 Create empty canon array file `professional-extras.json` (`[]`) in `src/Turpinverse.Data/canon/`
- [x] T002 Verify embedded resource glob in `src/Turpinverse.Data/Turpinverse.Data.csproj` includes new canon files via `canon\*.json`
- [x] T003 Confirm feature branch `006-professional-profile-extras` is active and `specs/006-professional-profile-extras/` design artifacts are present

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core models, schema, repository, validator, and presenter that MUST complete before user story work

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Tests for Foundational (failing-first) ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [x] T004 [P] Add failing CanonValidator professional extras fixture tests (VR-036–VR-043) in `tests/Turpinverse.Core.UnitTests/Validation/ProfessionalExtrasValidatorTests.cs` with `[Trait("Category", "CanonValidation")]`
- [x] T005 [P] Add failing CanonSchemaValidator tests for IntroCopy, ContactExtras, ProfessionalSocial, and ProfessionalExtras in `tests/Turpinverse.Core.UnitTests/Validation/ProfessionalExtrasSchemaValidatorTests.cs`
- [x] T006 [P] Add failing JsonCanonRepository load test for `professional-extras.json` in `tests/Turpinverse.Core.UnitTests/Data/JsonCanonRepositoryTests.cs`
- [x] T007 [P] Add failing ProfessionalExtrasPresenter lookup and visibility-flag tests in `tests/Turpinverse.Core.UnitTests/Profile/ProfessionalExtrasPresenterTests.cs`

### Implementation for Foundational

- [x] T008 [P] Create `IntroCopy` model in `src/Turpinverse.Core/Models/IntroCopy.cs`
- [x] T009 [P] Create `ContactExtras` model in `src/Turpinverse.Core/Models/ContactExtras.cs`
- [x] T010 [P] Create `ProfessionalSocial` model in `src/Turpinverse.Core/Models/ProfessionalSocial.cs`
- [x] T011 Create `ProfessionalExtras` model in `src/Turpinverse.Core/Models/ProfessionalExtras.cs`
- [x] T012 Extend `Canon` with `ProfessionalExtras` collection in `src/Turpinverse.Core/Models/Canon.cs`
- [x] T013 Update `JsonCanonRepository` to load `professional-extras.json` in `src/Turpinverse.Data/Repositories/JsonCanonRepository.cs`
- [x] T014 Merge additive types from `specs/006-professional-profile-extras/contracts/professional-extras-schema.json` into `specs/001-turpinverse-universe/contracts/canon-schema.json` (`professionalExtras` required array and `$defs`)
- [x] T015 Extend `CanonSchemaValidator` for professional extras entity types in `src/Turpinverse.Core/Validation/CanonSchemaValidator.cs`
- [x] T016 Implement VR-036–VR-043 validation methods and add `professionalExtras` to result counts in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [x] T017 Create `ProfessionalExtrasPresenter` with lookup, author-order lists, and title/biography/header-email visibility flags in `src/Turpinverse.Core/Profile/ProfessionalExtrasPresenter.cs`
- [x] T018 Register `ProfessionalExtrasPresenter` in `src/Turpinverse.Core/DependencyInjection/ServiceCollectionExtensions.cs`

**Checkpoint**: Foundation ready — models load, schema validates structure, validator tests fail on primary volume until data ships, presenter tests pass

---

## Phase 3: User Story 1 - Intro and hero copy for the primary subject (Priority: P1) 🎯 MVP

**Goal**: Short intro/hero copy for Richard Turpin (`dick-turpin`) as the profile header on Hugo persona pages and Blazor contact detail, seeded from existing persona fields, with title suppressed as a competing header line

**Independent Test**: Primary persona intro meets FR-003–FR-004; short intro, headline, and subtitle visible on Hugo `/personas/dick-turpin/` and Blazor `/contacts/dick-turpin`; stored job title is not a second competing header line; display name remains; optional photo and CTA MAY appear; `Category=ProfessionalExtras` intro tests pass

### Tests for User Story 1 (failing-first) ⚠️

- [x] T019 [P] [US1] Add failing HugoContentGenerator intro extras emission tests in `tests/Turpinverse.Core.UnitTests/Hugo/ProfessionalExtrasHugoTests.cs` with `[Trait("Category", "ProfessionalExtras")]`
- [x] T020 [P] [US1] Add failing Blazor intro header and title-suppression tests in `tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs` with `[Trait("Category", "ProfessionalExtras")]`

### Implementation for User Story 1

- [x] T021 [US1] Author intro block for `dick-turpin` in `src/Turpinverse.Data/canon/professional-extras.json` (seed `shortIntro`, `headline`, `subtitle` from display name, title, biography; optional photo and single CTA allowed)
- [x] T022 [US1] Extend `HugoContentGenerator` to emit extras into `site/data/` keyed by persona id in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [x] T023 [US1] Add intro header section and suppress competing `jobTitle` when intro present in `site/layouts/personas/single.html`
- [x] T024 [US1] Extend `ContactDetail.razor` with intro header and title suppression using `ProfessionalExtrasPresenter` in `src/Turpinverse.Web/Components/Pages/ContactDetail.razor`
- [x] T025 [P] [US1] Create `IntroSection.razor` in `src/Turpinverse.Web/Components/Profile/IntroSection.razor`
- [x] T026 [P] [US1] Add intro header styles in `src/Turpinverse.Web/Styles/app.css` (or `wwwroot/app.css` if that is the build source)

**Checkpoint**: User Story 1 functional on Hugo and Blazor for intro header — title not duplicated; CanonValidation still fails VR-041/VR-042 until US2/US3 data ships

---

## Phase 4: User Story 2 - About text and skills (Priority: P2)

**Goal**: About narrative and skills list for the primary subject on both person surfaces, in sandwich order before career/portfolio blocks, with biography suppressed as a competing narrative block

**Independent Test**: Primary persona meets FR-005–FR-006; about is longer narrative under intro; skills heading plus ≥5 unique names in author order before Experience/Education/Projects/Achievements; biography not shown as second block when about present; empty about/skills omitted on personas without them

### Tests for User Story 2 (failing-first) ⚠️

- [x] T027 [P] [US2] Add failing Hugo about/skills sandwich-order tests in `tests/Turpinverse.Core.UnitTests/Hugo/ProfessionalExtrasHugoTests.cs`
- [x] T028 [P] [US2] Add failing Blazor about/skills and biography-suppression tests in `tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs`

### Implementation for User Story 2

- [x] T029 [US2] Add `about`, `skillsHeading`, and ≥5 unique `skills` to `dick-turpin` row in `src/Turpinverse.Data/canon/professional-extras.json` (names only, author order, no career/portfolio lists)
- [x] T030 [US2] Add about and skills sections before career blocks in `site/layouts/personas/single.html` (omit empty; preserve author order; suppress biography when about present)
- [x] T031 [P] [US2] Create `AboutSection.razor` in `src/Turpinverse.Web/Components/Profile/AboutSection.razor`
- [x] T032 [P] [US2] Create `SkillsSection.razor` in `src/Turpinverse.Web/Components/Profile/SkillsSection.razor`
- [x] T033 [US2] Integrate about and skills into `ContactDetail.razor` in sandwich order before career sections with biography suppression

**Checkpoint**: User Stories 1 and 2 verifiable — intro, about, skills on Hugo and Blazor; career/portfolio unchanged; CanonValidation still fails VR-042 until contact/socials ship

---

## Phase 5: User Story 3 - Contact copy and professional socials (Priority: P3)

**Goal**: Contact presentation and professional socials for the primary subject after career/portfolio blocks, with email matching persona record and header email suppressed when contact extras exist

**Independent Test**: Primary persona meets FR-007–FR-008; contact then socials after career/portfolio; email equals `richard.turpin@turpinverse.uk`; ≥3 uniquely named socials in author order; no form-endpoint or site-chrome fields; `Category=CanonValidation` passes all VR-036–VR-043

### Tests for User Story 3 (failing-first) ⚠️

- [x] T034 [P] [US3] Add failing Hugo contact/socials placement-after-career tests in `tests/Turpinverse.Core.UnitTests/Hugo/ProfessionalExtrasHugoTests.cs`
- [x] T035 [P] [US3] Add failing Blazor contact/socials and header-email-suppression tests in `tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs`

### Implementation for User Story 3

- [x] T036 [US3] Complete `contact` and ≥3 unique `socials` on `dick-turpin` row in `src/Turpinverse.Data/canon/professional-extras.json` (email matches persona; optional phone/CTA; no forbidden keys)
- [x] T037 [US3] Add contact and socials sections after career blocks in `site/layouts/personas/single.html` (omit empty; suppress header email when contact present)
- [x] T038 [P] [US3] Create `ContactExtrasSection.razor` in `src/Turpinverse.Web/Components/Profile/ContactExtrasSection.razor`
- [x] T039 [P] [US3] Create `SocialsSection.razor` in `src/Turpinverse.Web/Components/Profile/SocialsSection.razor`
- [x] T040 [US3] Integrate contact and socials into `ContactDetail.razor` after career sections with header-email suppression

**Checkpoint**: All three user stories complete — full sandwich on Hugo and Blazor; loaded canon passes CanonValidation including primary volume gates

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validation gates, content regeneration, product-surface inventory, and quickstart verification

- [x] T041 Bump `canon.json` version in `src/Turpinverse.Data/canon/canon.json` when professional extras dataset ships
- [x] T042 Run `dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"` and resolve failures
- [x] T043 [P] Run `dotnet test tests/Turpinverse.Web.UnitTests --filter "Category=ProfessionalExtras"` and resolve failures
- [x] T044 Regenerate Hugo content via `src/Turpinverse.Tools.GenerateHugoContent/Program.cs` and confirm `site/data/` extras files for `dick-turpin`
- [x] T045 [P] Build Hugo site (`site/` with `hugo --minify` or `scripts/Invoke-HugoSite.ps1`) and spot-check `/personas/dick-turpin/`
- [x] T046 Verify `/api/canon/validate` returns 200 with `professionalExtras` count per `specs/006-professional-profile-extras/contracts/completeness.md`
- [x] T047 Update dual-publication inventory for professional extras on person pages in `docs/product-surfaces.md`
- [x] T048 Execute quickstart.md Scenarios A–E (manual Scenarios A–D; Scenario E negative fixtures in test code only)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — **BLOCKS all user stories**
- **User Story 1 (Phase 3)**: Depends on Foundational — MVP intro slice
- **User Story 2 (Phase 4)**: Depends on Foundational; extends US1 publication surfaces; about/skills data is independently testable via fixtures once foundation exists
- **User Story 3 (Phase 5)**: Depends on Foundational; extends US1/US2 contact detail and Hugo layout; contact/socials data completes volume gates
- **Polish (Phase 6)**: Depends on US1–US3 completion for full gate

### User Story Dependencies

- **User Story 1 (P1)**: After Phase 2 — no dependency on US2/US3 for intro-only UI tests (fixtures); loaded canon passes only after US3 completes
- **User Story 2 (P2)**: After Phase 2 — extends US1 surfaces; about/skills authoring independent of contact/socials
- **User Story 3 (P3)**: After Phase 2 — completes primary row and unlocks full CanonValidation pass

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Canon JSON authoring extends the same `professional-extras.json` row incrementally
- Hugo generator before layout sections
- Blazor section components before full contact detail integration
- Story checkpoint before moving to next priority

### Parallel Opportunities

- All Foundational model tasks T008–T011 can run in parallel (after T011 for T012)
- Foundational test tasks T004–T007 can run in parallel
- US1 Blazor component T025 and styles T026 can run in parallel
- US2 section components T031–T032 can run in parallel
- US3 section components T038–T039 can run in parallel
- Polish T043 and T045 can run in parallel after tests pass

---

## Parallel Example: User Story 1

```bash
# Failing tests together:
Task: "HugoContentGenerator intro extras tests in tests/Turpinverse.Core.UnitTests/Hugo/ProfessionalExtrasHugoTests.cs"
Task: "Blazor intro header tests in tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs"

# Blazor components together:
Task: "IntroSection.razor in src/Turpinverse.Web/Components/Profile/IntroSection.razor"
Task: "Intro styles in src/Turpinverse.Web/Styles/app.css"
```

---

## Parallel Example: User Story 2

```bash
# Failing tests together:
Task: "Hugo about/skills sandwich tests in tests/Turpinverse.Core.UnitTests/Hugo/ProfessionalExtrasHugoTests.cs"
Task: "Blazor about/skills tests in tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs"

# Section components together:
Task: "AboutSection.razor in src/Turpinverse.Web/Components/Profile/AboutSection.razor"
Task: "SkillsSection.razor in src/Turpinverse.Web/Components/Profile/SkillsSection.razor"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Intro header on Hugo and Blazor for `dick-turpin`; title not duplicated
5. Note: `Category=CanonValidation` on loaded canon still fails until US2/US3 data ships — use story-scoped tests and fixtures

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test intro independently → Demo header copy (MVP!)
3. Add User Story 2 → Test about/skills independently → Demo profile body
4. Add User Story 3 → Test contact/socials independently → Full CanonValidation pass
5. Polish → quickstart Scenarios A–E green

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (intro header Hugo + Blazor)
   - Developer B: User Story 2 (about + skills) — can start layout work in parallel after T022
   - Developer C: User Story 3 (contact + socials) — can author JSON in parallel; integrate after US2 sandwich is stable
3. Stories integrate on shared `professional-extras.json` row and `ContactDetail.razor`

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable via fixtures and UI tests
- Loaded canon CanonValidation passes only when US3 data is complete (VR-040–VR-042)
- Verify tests fail before implementing
- Do not add mapping note, CSV columns, ExportDatasets entries, theme switch, or second showcase product
- Skills and socials MUST NOT be sorted alphabetically in UI — author order only
- Commit after each task or logical group
