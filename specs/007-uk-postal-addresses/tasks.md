---

description: "Task list for UK Postal Addresses feature implementation"
---

# Tasks: UK Postal Addresses

**Input**: Design documents from `/specs/007-uk-postal-addresses/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Failing-first CanonValidation, export column, Hugo generator, and Blazor contact-detail tests per plan.md, quickstart.md, and Constitution Principle III.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story. Each task traces to spec requirements FR-001–FR-012 and US1–US3.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- Canon JSON: `src/Turpinverse.Data/canon/`
- Core models/validation/export: `src/Turpinverse.Core/`
- Blazor UI: `src/Turpinverse.Web/Components/`
- Hugo layouts/content: `site/layouts/`, `site/content/`
- Tests: `tests/Turpinverse.Core.UnitTests/`, `tests/Turpinverse.Web.UnitTests/`, `tests/Turpinverse.IntegrationTests/`
- Feature contracts: `specs/007-uk-postal-addresses/contracts/`
- Runtime schemas (merge targets): `specs/001-turpinverse-universe/contracts/canon-schema.json`, `specs/001-turpinverse-universe/contracts/crm-export-schema.json`
- Developer docs: `docs/product-surfaces.md`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Feature branch readiness and baseline verification

- [ ] T001 Verify feature branch `007-uk-postal-addresses` is active and design artifacts exist under `specs/007-uk-postal-addresses/`
- [ ] T002 Confirm baseline canon files `src/Turpinverse.Data/canon/organisations.json` and `src/Turpinverse.Data/canon/personas.json` load without `registeredOffice` / `address` fields yet
- [ ] T003 Confirm merge targets `specs/001-turpinverse-universe/contracts/canon-schema.json` and `specs/001-turpinverse-universe/contracts/crm-export-schema.json` exist per plan.md

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Address value object, schema merge, validator rules, tone scans, and CSV flatten infrastructure that MUST complete before user story work

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Tests for Foundational (failing-first) ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T004 [P] Add failing Address canon validation tests (VR-044–VR-051) in `tests/Turpinverse.Core.UnitTests/Validation/AddressValidatorTests.cs` with `[Trait("Category", "CanonValidation")]`
- [ ] T005 [P] Add failing CanonSchemaValidator tests for `$defs.Address`, required `registeredOffice`, and optional `address` in `tests/Turpinverse.Core.UnitTests/Validation/AddressSchemaValidatorTests.cs` with `[Trait("Category", "CanonValidation")]`
- [ ] T006 [P] Add failing ToneValidator address-string scan tests (TONE-001 on `address1` etc.) in `tests/Turpinverse.Core.UnitTests/Validation/ToneValidatorAddressTests.cs`
- [ ] T007 [P] Add failing ExportMapper address flatten tests (org `registeredOffice*` and contact `mailing*` with empty strings when absent) in `tests/Turpinverse.Core.UnitTests/Export/AddressExportMapperTests.cs`
- [ ] T008 [P] Add failing `ExportCsvColumns` header assertions for seven `registeredOffice*` and seven `mailing*` columns in `tests/Turpinverse.Core.UnitTests/Export/ExportCsvColumnsTests.cs`

### Implementation for Foundational

- [ ] T009 [P] Create `Address` value object (`address1`, `address2`, `address3`, `town`, `region`, `postcode`, `country`) in `src/Turpinverse.Core/Models/Address.cs`
- [ ] T010 Extend `Organisation` with required `RegisteredOffice` in `src/Turpinverse.Core/Models/Organisation.cs`
- [ ] T011 Extend `Persona` with optional `Address` in `src/Turpinverse.Core/Models/Persona.cs`
- [ ] T012 Merge `$defs.Address` and Organisation/Persona patches from `specs/007-uk-postal-addresses/contracts/address-canon-schema.json` into `specs/001-turpinverse-universe/contracts/canon-schema.json`
- [ ] T013 Merge account/contact address columns from `specs/007-uk-postal-addresses/contracts/crm-export-schema.json` into `specs/001-turpinverse-universe/contracts/crm-export-schema.json`
- [ ] T014 Extend `CanonSchemaValidator` for Address entity types in `src/Turpinverse.Core/Validation/CanonSchemaValidator.cs`
- [ ] T015 Implement VR-044–VR-051 validation methods and door-key helper in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [ ] T016 Extend `ToneValidator.ValidateCanon` to run forbidden-pattern checks on every non-empty Address string field in `src/Turpinverse.Core/Validation/ToneValidator.cs`
- [ ] T017 [P] Add `registeredOfficeAddress1`–`registeredOfficeCountry` properties to `AccountExport` in `src/Turpinverse.Core/Export/AccountExport.cs`
- [ ] T018 [P] Add `mailingAddress1`–`mailingCountry` properties to `ContactExport` in `src/Turpinverse.Core/Export/ContactExport.cs`
- [ ] T019 Extend `ExportMapper` to flatten nested addresses; emit `""` for absent optional lines and absent persona address (never copy org office onto contact) in `src/Turpinverse.Core/Export/ExportMapper.cs`
- [ ] T020 Update `ExportCsvColumns` account and contact column lists to match merged CRM schema in `src/Turpinverse.Core/Export/ExportCsvColumns.cs`

**Checkpoint**: Foundation ready — models deserialize, schema validates structure, validator/export tests fail until canon data ships in user story phases

---

## Phase 3: User Story 1 - Every organisation has a registered office (Priority: P1) 🎯 MVP

**Goal**: All ten organisations have a complete invented UK registered office; Hugo always shows it in human-readable postal layout; account CSV includes seven `registeredOffice*` columns

**Independent Test**: Every organisation has complete `registeredOffice` (first line, town, postcode, country); parent/subsidiary may share town but not the same door (`address1` + postcode); Hugo `/organisations/{slug}/` always shows a **Registered office** block; `/api/export/accounts` rows include filled `registeredOffice*` columns; `Category=CanonValidation` passes VR-044 and VR-049 for organisations

### Tests for User Story 1 (failing-first) ⚠️

- [ ] T021 [P] [US1] Add failing HugoContentGenerator `registeredOffice` front-matter emission tests in `tests/Turpinverse.Core.UnitTests/Hugo/AddressHugoTests.cs` with `[Trait("Category", "PostalAddress")]`
- [ ] T022 [P] [US1] Add failing account CSV `registeredOffice*` column integration tests in `tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs` with `[Trait("Category", "CsvExport")]`

### Implementation for User Story 1

- [ ] T023 [US1] Author complete distinct `registeredOffice` for all ten organisations in `src/Turpinverse.Data/canon/organisations.json` (invented premises from historical anchors; United Kingdom; ≥1 org uses `address3`; no shared door keys)
- [ ] T024 [US1] Extend `HugoContentGenerator` to emit nested `registeredOffice` into organisation front matter in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [ ] T025 [US1] Add always-visible **Registered office** postal layout block (lines, town, region, postcode, country — no CSV/JSON keys as visible labels) in `site/layouts/organisations/single.html`
- [ ] T026 [US1] Regenerate organisation markdown under `site/content/organisations/` via `src/Turpinverse.Tools.GenerateHugoContent/Program.cs`

**Checkpoint**: User Story 1 functional — all org offices in canon, Hugo org pages, account CSV; CanonValidation still fails VR-046–VR-048 until contact addresses ship (US2)

---

## Phase 4: User Story 2 - A minority of contacts have a mailing address (Priority: P2)

**Goal**: Exactly three personas (`dick-turpin`, `mary-brazier`, `james-smith`) have complete mailing addresses; Richard's address differs from Turpin Enterprises; Black Bess and Elizabeth Millington omit address; Hugo omits empty persona blocks; contact CSV always has mailing columns with empty strings when absent; Blazor contact detail shows address only when present

**Independent Test**: Richard has complete address distinct from Turpin Enterprises door key; Mary Brazier and James Smith have addresses; Black Bess and Elizabeth Millington have no `address`; Hugo `/personas/dick-turpin/` shows mailing address and `/personas/black-bess/` shows no address heading; Blazor `/contacts/dick-turpin` shows address and `/contacts/black-bess` does not; contact CSV mailing columns are `""` when no address; VR-046–VR-048 pass

### Tests for User Story 2 (failing-first) ⚠️

- [ ] T027 [P] [US2] Add failing Hugo persona `address` emit-when-present / omit-when-absent tests in `tests/Turpinverse.Core.UnitTests/Hugo/AddressHugoTests.cs`
- [ ] T028 [P] [US2] Add failing Blazor ContactDetail mailing-address present/omit tests in `tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs` with `[Trait("Category", "PostalAddress")]`
- [ ] T029 [P] [US2] Add failing contact CSV empty-string `mailing*` column tests (columns always present; no org office copy) in `tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs`

### Implementation for User Story 2

- [ ] T030 [US2] Author `address` only on `dick-turpin`, `mary-brazier`, and `james-smith` in `src/Turpinverse.Data/canon/personas.json` (Richard door key ≠ `turpin-enterprises`; ≥1 contact uses `address3`; all others omit property)
- [ ] T031 [US2] Extend `HugoContentGenerator` to emit nested `address` into persona front matter only when `Persona.Address` is set in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [ ] T032 [US2] Add mailing-address block with omit-empty guard (no heading when absent) in `site/layouts/personas/single.html`
- [ ] T033 [US2] Add mailing-address section to `src/Turpinverse.Web/Components/Pages/ContactDetail.razor` when `Persona.Address` is present (outside `ContactExtras`)
- [ ] T034 [US2] Regenerate persona markdown under `site/content/personas/` via `src/Turpinverse.Tools.GenerateHugoContent/Program.cs`

**Checkpoint**: User Stories 1 and 2 verifiable — org offices, contact minority, dual-channel persona surfaces; VR-044–VR-051 should pass on full canon

---

## Phase 5: User Story 3 - UK-shaped address fields on both channels (Priority: P3)

**Goal**: UK CRM field vocabulary (`address1`–`country`, `registeredOffice*` / `mailing*` CSV prefixes) is consistent across canon, export, Hugo, and Blazor; third-line usage enforced; professional-extras contact unchanged; optional list-preview town columns; channel charter documented

**Independent Test**: Every stored address has required fields and `United Kingdom` country; ≥1 org and ≥1 contact use `address3`; professional extras have email/copy/phone only (no postal fields); Blazor `/accounts` MAY show `registeredOfficeTown` and `/contacts` MAY show `mailingTown`; `docs/product-surfaces.md` reflects Hugo showcase vs Blazor export split; no maps, coordinates, billing/shipping, or account-detail page

### Tests for User Story 3 (failing-first) ⚠️

- [ ] T035 [P] [US3] Add failing regression test that `ContactExtras` and `professional-extras.json` contain no postal address fields in `tests/Turpinverse.Core.UnitTests/Validation/AddressValidatorTests.cs`
- [ ] T036 [P] [US3] Add failing optional Blazor list preview column tests for `registeredOfficeTown` and `mailingTown` in `tests/Turpinverse.Web.UnitTests/Components/CrmPageTests.cs` with `[Trait("Category", "PostalAddress")]`

### Implementation for User Story 3

- [ ] T037 [US3] Add optional `registeredOfficeTown` preview column to `src/Turpinverse.Web/Components/Pages/Accounts.razor`
- [ ] T038 [US3] Add optional `mailingTown` preview column to `src/Turpinverse.Web/Components/Pages/Contacts.razor`
- [ ] T039 [US3] Update Hugo showcase vs Blazor explore/export address surfaces in `docs/product-surfaces.md` per `specs/007-uk-postal-addresses/contracts/channel-surfaces.md`
- [ ] T040 [US3] Bump `version` in `src/Turpinverse.Data/canon/canon.json` when the address dataset ships (if project convention requires)

**Checkpoint**: All three user stories independently testable; FR-003, FR-008, FR-010–FR-012 satisfied; full `Category=CanonValidation` suite passes

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: End-to-end validation and publication completeness

- [ ] T041 Run quickstart test filters from `specs/007-uk-postal-addresses/quickstart.md` (`CanonValidation`, `Export`, `HugoContentGenerator`, `ContactDetail`, `CsvExport`) and fix any failures
- [ ] T042 [P] Run Hugo build (`cd site && hugo --minify` or `scripts/Invoke-HugoSite.ps1 build`) and spot-check Scenarios A–C from `specs/007-uk-postal-addresses/quickstart.md`
- [ ] T043 [P] Verify `GET /api/canon/validate` returns 200 with zero VR-044–VR-051 violations when Development API is running (`src/Turpinverse.Web/Endpoints/CanonEndpoints.cs`)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — **BLOCKS all user stories**
- **User Story 1 (Phase 3)**: Depends on Foundational — no dependency on US2/US3
- **User Story 2 (Phase 4)**: Depends on Foundational — org data from US1 recommended for VR-050 but validator tests can use fixtures independently
- **User Story 3 (Phase 5)**: Depends on Foundational — best after US1+US2 data exists for third-line and channel checks
- **Polish (Phase 6)**: Depends on desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational — delivers MVP (org offices + account export + Hugo org pages)
- **User Story 2 (P2)**: Can start after Foundational — independently testable with fixture canon; full VR-050 needs US1 Turpin Enterprises office authored
- **User Story 3 (P3)**: Can start after Foundational for docs/preview columns; full validation needs US1+US2 canon data

### Within Each User Story

- Tests MUST be written and FAIL before implementation (Constitution III)
- Foundational: models → schema merge → validator → export DTOs/mapper
- US1: canon org data → Hugo generator → org layout → regenerate content
- US2: canon persona data → Hugo generator → persona layout → Blazor detail → regenerate content
- US3: regression tests → optional preview columns → product-surfaces doc

### Parallel Opportunities

- All Setup tasks are sequential but lightweight
- T004–T008 (foundational tests) can run in parallel
- T009, T017, T018 (models and export DTOs) can run in parallel after T009 exists
- T021–T022 (US1 tests) can run in parallel
- T027–T029 (US2 tests) can run in parallel
- T035–T036 (US3 tests) can run in parallel
- T042–T043 (polish) can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch US1 tests together (failing-first):
Task T021: HugoContentGenerator registeredOffice tests in tests/Turpinverse.Core.UnitTests/Hugo/AddressHugoTests.cs
Task T022: Account CSV registeredOffice* tests in tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs

# After T023–T026 implementation, validate independently:
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
dotnet test tests/Turpinverse.IntegrationTests --filter "Category=CsvExport"
```

---

## Parallel Example: User Story 2

```bash
# Launch US2 tests together (failing-first):
Task T027: Hugo persona address emit/omit tests in tests/Turpinverse.Core.UnitTests/Hugo/AddressHugoTests.cs
Task T028: Blazor ContactDetail tests in tests/Turpinverse.Web.UnitTests/Components/ContactDetailTests.cs
Task T029: Contact CSV mailing* tests in tests/Turpinverse.IntegrationTests/Export/CsvExportTests.cs
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: CanonValidation org rules, Hugo org pages, account CSV
5. Deploy/demo if ready

### Incremental Delivery

1. Setup + Foundational → infrastructure ready
2. User Story 1 → org offices on Hugo + account export (MVP)
3. User Story 2 → contact minority + persona surfaces + contact export
4. User Story 3 → preview polish + channel charter docs + full VR-051/third-line gate
5. Polish → quickstart scenarios and API validate endpoint

### Parallel Team Strategy

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (org canon + Hugo org + account CSV)
   - Developer B: User Story 2 (persona canon + Hugo persona + Blazor detail + contact CSV)
   - Developer C: User Story 3 (preview columns + product-surfaces.md)
3. Integrate and run Phase 6 polish together

---

## Notes

- [P] tasks = different files, no dependencies on incomplete tasks in the same batch
- [Story] label maps task to spec user story for traceability
- Invented premises only — no real private dwellings (content review, not VR code)
- Do not add postal fields to `src/Turpinverse.Core/Models/ContactExtras.cs` or `professional-extras.json`
- Do not add `/accounts/{id}`, maps, geocoding, or billing/shipping split
- Feature contract lives in `specs/007-uk-postal-addresses/contracts/`; merge into 001 runtime schemas at implementation time
- Commit after each task or logical group
