---
description: "Task list for CRM Join Graph Alignment feature implementation"
---

# Tasks: CRM Join Graph Alignment

**Input**: Design documents from `/specs/008-crm-join-graph/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Failing-first tests are **in scope** per FR-015 and Constitution Principle III. VR-052–VR-059 and export/Hugo boundaries MUST fail before implementation.

**Organization**: Tasks grouped by user story (P1 join rules → P2 canon repairs → P3 membership export). Each story is independently testable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: User story label (US1, US2, US3) for story-phase tasks only
- Include exact file paths in descriptions

## Path Conventions

- **Core models**: `src/Turpinverse.Core/Models/`
- **Validation**: `src/Turpinverse.Core/Validation/CanonValidator.cs`
- **Export**: `src/Turpinverse.Core/Export/`
- **Canon data**: `src/Turpinverse.Data/canon/`
- **Blazor**: `src/Turpinverse.Web/Components/`
- **Hugo**: `site/layouts/`, `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- **Tests**: `tests/Turpinverse.Core.UnitTests/`, `tests/Turpinverse.IntegrationTests/`
- **Contracts**: `specs/001-turpinverse-universe/contracts/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm feature context and contract merge targets before model/validator work

- [X] T001 Confirm active branch `008-crm-join-graph` and review design artifacts in `specs/008-crm-join-graph/` (plan.md, spec.md, data-model.md, contracts/)
- [X] T002 [P] Read join-graph source of truth in `docs/entity-relationships.md` and channel split in `docs/product-surfaces.md` before editing models or surfaces

**Checkpoint**: Feature folder and merge targets understood

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core model, schema, and export-column changes that MUST complete before user story work

**⚠️ CRITICAL**: No user story implementation can begin until this phase is complete

- [X] T003 [P] Add optional `PrimaryContactId` property to `Organisation` in `src/Turpinverse.Core/Models/Organisation.cs`
- [X] T004 [P] Make `ContactId` optional (`string?`) and add `StakeholderContactIds` to `Deal` in `src/Turpinverse.Core/Models/Deal.cs`
- [X] T005 [P] Make `ContactId` optional (`string?`) and add `StakeholderContactIds` to `Case` in `src/Turpinverse.Core/Models/Case.cs`
- [X] T006 [P] Remove `PersonaIds` from `Project`; add optional `ContactId`, `StakeholderContactIds`, `DealId`, and `CaseIds` in `src/Turpinverse.Core/Models/Project.cs`
- [X] T007 [P] Add optional `DealIds` and `CaseIds` to `CanonEvent` in `src/Turpinverse.Core/Models/CanonEvent.cs`
- [X] T008 Merge `specs/008-crm-join-graph/contracts/join-canon-schema.json` into `specs/001-turpinverse-universe/contracts/canon-schema.json` (optional main contact, stakeholders, account `primaryContactId`, project lineage, `memberPersonaIds` min 0)
- [X] T009 Merge `specs/008-crm-join-graph/contracts/crm-export-schema.json` into `specs/001-turpinverse-universe/contracts/crm-export-schema.json` (membership contacts, optional main/stakeholder columns, account `primaryContactId`)
- [X] T010 Update `ExportCsvColumns` and export DTOs (`ContactExport`, `AccountExport`, `DealExport`, `CaseExport`, `ProjectExport`) in `src/Turpinverse.Core/Export/` to match merged CRM schema
- [X] T011 Add `LinkedPersonaIds` helper (main ∪ stakeholders, main first) on `Project` or in `src/Turpinverse.Core/Career/CareerPortfolioPresenter.cs`
- [X] T012 Update `JsonCanonRepository` deserialization in `src/Turpinverse.Data/Repositories/JsonCanonRepository.cs` for new optional fields and removed `Project.PersonaIds`
- [X] T013 [P] Update `CanonSchemaValidatorTests` in `tests/Turpinverse.Core.UnitTests/Validation/CanonSchemaValidatorTests.cs` for optional main contact and organisation `memberPersonaIds` min 0

**Checkpoint**: Models, merged schemas, and export column definitions align with data-model.md — user story work can begin

---

## Phase 3: User Story 1 - Join rules match a real CRM account graph (Priority: P1) 🎯 MVP

**Goal**: Completeness checks enforce the target join graph — optional main contact (member when set), stakeholders (need not be members), account primary contact, project people as main ∪ stakeholders only

**Independent Test**: Fixture with non-member main contact fails VR-054; same person as stakeholder only passes. Account `primaryContactId` non-member fails VR-052. Rules allow empty account members; every contact still has ≥1 account. No canon row repairs required to prove rules.

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T014 [P] [US1] Add failing VR-052 fixture (account primary not a member) in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs` with `[Trait("Category", "CanonValidation")]`
- [X] T015 [P] [US1] Add failing VR-054 fixture (deal/case/project main not an account member) in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`
- [X] T016 [P] [US1] Add failing VR-055 fixtures (unknown, duplicate, and main-equals-stakeholder) in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`
- [X] T017 [P] [US1] Add failing VR-056 and VR-057 fixtures (unknown project deal/case and event pipeline ids) in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`
- [X] T018 [P] [US1] Add failing VR-053 fixture (missing/invalid account on pipeline record) in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`
- [X] T019 [P] [US1] Add failing pass-through fixture (stakeholder-only non-member, omitted main) in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`

### Implementation for User Story 1

- [X] T020 [US1] Implement VR-052 (account `primaryContactId` must be a member when set) in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [X] T021 [US1] Implement VR-053 (deal/case/project must reference exactly one existing account) in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [X] T022 [US1] Implement VR-054 (main `contactId` optional; when set must exist and be an account member) in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [X] T023 [US1] Implement VR-055 (stakeholders exist, unique, not equal to main) in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [X] T024 [US1] Implement VR-056 (project `dealId` / `caseIds` exist when set) in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [X] T025 [US1] Implement VR-057 (event `dealIds` / `caseIds` exist when set) in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [X] T026 [US1] Reinterpret VR-005 / VR-006 in `src/Turpinverse.Core/Validation/CanonValidator.cs` as “when main contact is set, identity must exist” (remove required-main assumption)
- [X] T027 [US1] Stop applying VR-022 “project must have ≥1 people” to projects in `src/Turpinverse.Core/Validation/CanonValidator.cs` (achievements keep VR-022)
- [X] T028 [US1] Verify VR-052–VR-057 unit tests pass in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`

**Checkpoint**: Join-rule validation works on fixtures independently of canon content repairs

---

## Phase 4: User Story 2 - Four story rows stay true without fake membership (Priority: P2)

**Goal**: Repair `deal-008`, `case-001`, `case-017`, `palmer-identity-vault`; remap remaining projects; author `primaryContactId` on all ten organisations; update Hugo/Career surfaces for names and optional FKs

**Independent Test**: Loaded canon passes VR-058. Each of the four named records keeps its account, uses the confirmed deputy as main, and lists the previous non-member as stakeholder only. Organisation pages show primary contacts by name. No new people, empty accounts, or contact-less pipeline rows.

### Tests for User Story 2 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before canon repairs**

- [X] T029 [P] [US2] Add failing VR-058 tests for `deal-008`, `case-001`, `case-017`, and `palmer-identity-vault` in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`
- [X] T030 [P] [US2] Add failing loaded-canon test asserting ten organisation `primaryContactId` values per `specs/008-crm-join-graph/data-model.md` in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`
- [X] T031 [P] [US2] Add failing test that projects have no `personaIds` in loaded canon in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs`

### Canon repairs for User Story 2

- [X] T032 [P] [US2] Repair `deal-008` (William Hargreaves main, Henry Clayton stakeholder) in `src/Turpinverse.Data/canon/deals.json`
- [X] T033 [P] [US2] Repair `case-001` and `case-017` (Mary Brazier / Henry Clayton mains; Richard Turpin / Thomas Collier stakeholders) in `src/Turpinverse.Data/canon/cases.json`
- [X] T034 [US2] Remap all projects per FR-008/FR-010 (`palmer-identity-vault`, `black-bess-route-optimiser`, `essex-procurement-hub`) in `src/Turpinverse.Data/canon/projects.json`
- [X] T035 [P] [US2] Add `primaryContactId` to all ten organisations per planning table in `src/Turpinverse.Data/canon/organisations.json`
- [X] T036 [US2] Implement VR-058 (four named rows + no project `personaIds`) in `src/Turpinverse.Core/Validation/CanonValidator.cs`

### Publication and career for User Story 2

- [X] T037 [P] [US2] Update `GetProjectsForPersona` and related filters to use `LinkedPersonaIds` in `src/Turpinverse.Core/Career/CareerPortfolioPresenter.cs`
- [X] T038 [US2] Update `HugoContentGenerator` to omit empty `contactId`, emit `stakeholderContactIds` and `primaryContactId` only when set in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [X] T039 [P] [US2] Update `site/layouts/partials/related-parties.html` to omit empty main-contact block and show stakeholder names
- [X] T040 [P] [US2] Update `site/layouts/deals/single.html`, `site/layouts/cases/single.html`, and `site/layouts/projects/single.html` for optional main contact and stakeholders by name
- [X] T041 [P] [US2] Update `site/layouts/organisations/single.html` to show account primary contact by display name when set
- [X] T042 [P] [US2] Update persona related deals/cases to include records where person is main **or** stakeholder in `site/layouts/personas/single.html` and related partials
- [X] T043 [US2] Verify `CanonValidatorTests.Validate_LoadedCanon_PassesAllRules` passes with VR-052–VR-058 in `tests/Turpinverse.Core.UnitTests/Validation/CanonValidatorTests.cs`

**Checkpoint**: Full authored dataset passes completeness; public site shows names without empty-id blocks

---

## Phase 5: User Story 3 - Export one contact row per account membership (Priority: P3)

**Goal**: Contact CSV emits one row per membership with copied identity; deal/case/project exports expose optional main and stakeholders; account export includes optional primary contact; collision policy documented on Blazor export surface

**Independent Test**: Contact row count equals membership link count. `dick-turpin` appears twice with same email and different `accountId`. Deal/case/project CSV columns match merged schema. Collision policy readable from Blazor contacts/export help, not Hugo primary copy.

### Tests for User Story 3 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before export implementation**

- [X] T044 [P] [US3] Add failing VR-059 membership row-count test in `tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs` with `[Trait("Category", "CsvExport")]`
- [X] T045 [P] [US3] Add failing `dick-turpin` dual-row test (same email/phone/mailing, different `accountId`) in `tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs`
- [X] T046 [P] [US3] Update `contacts` `minRows` from 25 to ~31 and add stakeholder/optional-main column assertions in `tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs`
- [X] T047 [P] [US3] Add failing export mapper unit tests for membership explode in `tests/Turpinverse.Core.UnitTests/Export/ExportMapperTests.cs`
- [X] T048 [P] [US3] Update `CrossReferenceTests` for optional deal/case `contactId` and project people from union in `tests/Turpinverse.IntegrationTests/Export/CrossReferenceTests.cs`

### Implementation for User Story 3

- [X] T049 [US3] Refactor `MapContacts` to emit one row per persona × `organisationIds` entry with copied identity in `src/Turpinverse.Core/Export/ExportMapper.cs`
- [X] T050 [US3] Add optional `contactId` and joined `stakeholderContactIds` to deal/case/project export mapping in `src/Turpinverse.Core/Export/ExportMapper.cs`
- [X] T051 [US3] Add optional `primaryContactId` to account export mapping in `src/Turpinverse.Core/Export/ExportMapper.cs`
- [X] T052 [US3] Remove undifferentiated project people list from project export (use main ∪ stakeholders and lineage columns only) in `src/Turpinverse.Core/Export/ExportMapper.cs`
- [X] T053 [US3] Add email collision policy copy near contact download on `src/Turpinverse.Web/Components/Pages/Contacts.razor` (or shared export help partial under `src/Turpinverse.Web/Components/`)
- [X] T054 [US3] Document collision policy in `specs/001-turpinverse-universe/contracts/export-api.md` per `specs/008-crm-join-graph/contracts/completeness.md`
- [X] T055 [US3] Verify VR-059 and updated export tests pass (`Category=CsvExport`) in `tests/Turpinverse.IntegrationTests/Export/` and `tests/Turpinverse.Core.UnitTests/Export/`

**Checkpoint**: Export fidelity matches join graph; importers can read collision policy from Blazor/docs

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Documentation alignment, API validation surface, and quickstart verification

- [X] T056 [P] Update project people guidance in `docs/career-portfolio-mapping.md` (main ∪ stakeholders, not `personaIds`)
- [X] T057 [P] Sync `docs/entity-relationships.md` only if implementation field names diverge from the documented graph
- [X] T058 [P] Verify `/api/canon/validate` returns 200 with VR-052–VR-058 on loaded canon in `tests/Turpinverse.IntegrationTests/Export/CanonValidateApiTests.cs`
- [X] T059 Regenerate Hugo content via `src/Turpinverse.Tools.GenerateHugoContent` and confirm no empty `contactId: ""` front matter
- [X] T060 Run quickstart scenarios A–C from `specs/008-crm-join-graph/quickstart.md` (`Category=CanonValidation` and `Category=CsvExport` filters)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Setup — **BLOCKS all user stories**
- **User Story 1 (Phase 3)**: Depends on Foundational — defines join rules on fixtures (MVP validation layer)
- **User Story 2 (Phase 4)**: Depends on Foundational + US1 validator rules — canon repairs and publication
- **User Story 3 (Phase 5)**: Depends on Foundational; export membership logic needs stable persona `organisationIds` (US2 canon edits affect row count but not mapper shape)
- **Polish (Phase 6)**: Depends on desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational — no canon repairs required; proves rules via fixtures
- **User Story 2 (P2)**: Requires US1 validator (VR-052–VR-057, VR-058 logic) — independently testable via loaded canon + VR-058
- **User Story 3 (P3)**: Requires Foundational export columns; full SC-004 needs US2 canon (membership count ~31); collision copy and mapper are independently testable once columns exist

### Within Each User Story

- Tests MUST be written and FAIL before implementation (FR-015)
- Models/schemas (Phase 2) before validator rules (US1)
- Validator rules before canon repairs (US2)
- Export tests before `ExportMapper` changes (US3)
- Hugo generator updates after canon shape is stable (US2)

### Parallel Opportunities

- All Phase 2 model tasks T003–T007 can run in parallel
- US1 failing tests T014–T019 can run in parallel
- US2 canon JSON edits T032–T035 can run in parallel (different files)
- US2 Hugo layout tasks T039–T042 can run in parallel
- US3 failing export tests T044–T048 can run in parallel
- Polish doc tasks T056–T057 can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all failing join-rule tests together:
Task: "Add failing VR-052 fixture in tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs"
Task: "Add failing VR-054 fixture in tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs"
Task: "Add failing VR-055 fixtures in tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs"

# After T020–T025, verify together:
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation&FullyQualifiedName~JoinGraph"
```

---

## Parallel Example: User Story 2

```bash
# Canon repairs in parallel (separate JSON files):
Task: "Repair deal-008 in src/Turpinverse.Data/canon/deals.json"
Task: "Repair case-001 and case-017 in src/Turpinverse.Data/canon/cases.json"
Task: "Add primaryContactId to organisations in src/Turpinverse.Data/canon/organisations.json"

# Hugo layout updates in parallel:
Task: "Update site/layouts/partials/related-parties.html"
Task: "Update site/layouts/organisations/single.html"
```

---

## Parallel Example: User Story 3

```bash
# Failing export tests in parallel:
Task: "Add failing VR-059 test in tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs"
Task: "Add failing dick-turpin dual-row test in tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs"
Task: "Add failing ExportMapper unit tests in tests/Turpinverse.Core.UnitTests/Export/ExportMapperTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL)
3. Complete Phase 3: User Story 1 (failing tests + VR-052–VR-057)
4. **STOP and VALIDATE**: Join-rule fixtures pass/fail correctly
5. Demo validator behaviour without waiting for canon/Hugo/export

### Incremental Delivery

1. Setup + Foundational → schema and models ready
2. User Story 1 → Join rules enforced on fixtures (MVP validation)
3. User Story 2 → Living dataset + Hugo/Career tellable stories
4. User Story 3 → CRM-importable membership export + collision policy
5. Polish → docs, API validate, quickstart sign-off

### Parallel Team Strategy

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: US1 validator + tests
   - Developer B: US2 canon JSON + Hugo (after US1 VR-054/VR-058 land)
   - Developer C: US3 export tests + mapper (after T010 columns; full row count after US2)
3. Each story validates independently per quickstart scenarios A–C

---

## Notes

- [P] tasks = different files, no incomplete-task dependencies
- [Story] label maps task to spec user story for traceability
- VR-059 is enforced in export tests, not `/api/canon/validate` body (see contracts/completeness.md)
- Do **not** add Richard to Brazier Legal, Henry to Epping Forest, or Thomas to Turpin Enterprises as members
- Do **not** author empty accounts or strip main contacts from unrelated demo rows
- Professional-extras contact copy is unchanged
- Commit after each task or logical group; stop at any checkpoint to validate independently

---

## Phase 7: Convergence

- [X] T061 Add `[Trait("Category", "CsvExport")]` to `tests/Turpinverse.Core.UnitTests/Export/ExportMapperTests.cs` (and export-related mapper tests) so quickstart `dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CsvExport"` exercises membership export unit coverage per quickstart.md / T060 (partial)
- [X] T062 Add `CanonSchemaValidatorTests` fixtures proving a deal/case without `contactId` and an organisation with `memberPersonaIds: []` pass JSON Schema validation in `tests/Turpinverse.Core.UnitTests/Validation/CanonSchemaValidatorTests.cs` per T013 / FR-015 (partial)
- [X] T063 Add `JoinGraphValidatorTests` fixture proving an organisation with zero members passes completeness (no VR-052/VR-003 violations when primary is omitted) in `tests/Turpinverse.Core.UnitTests/Validation/JoinGraphValidatorTests.cs` per US1/AC1 / FR-002 (partial)

## Phase 8: Convergence

- [ ] T064 Refresh `docs/entity-relationships.md` (remove pre-ship implementation-status banner, document VR-052–VR-059, present-tense export projection table, replace “canon rows to fix” with shipped repair summary) and update the `docs/product-surfaces.md` cross-link so shipped schema no longer reads as lagging per SC-007 (contradicts)
