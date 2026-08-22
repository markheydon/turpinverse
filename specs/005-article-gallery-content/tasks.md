---

description: "Task list for Generic Article and Gallery Demo Data feature implementation"
---

# Tasks: Generic Article and Gallery Demo Data

**Input**: Design documents from `/specs/005-article-gallery-content/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Failing-first CanonValidation and Hugo generator tests per plan.md, quickstart.md, and Constitution Principle III. No Blazor or bUnit tests (export app and Web UI are out of scope).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story. Each task traces to spec requirements FR-001–FR-016 and US1–US3.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- Canon JSON: `src/Turpinverse.Data/canon/`
- Core models/validation: `src/Turpinverse.Core/`
- Hugo generator: `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- Hugo layouts/data: `site/layouts/`, `site/data/`, `site/hugo.toml`
- Tests: `tests/Turpinverse.Core.UnitTests/`
- Runtime schema: `specs/001-turpinverse-universe/contracts/canon-schema.json`
- Additive schema: `specs/005-article-gallery-content/contracts/article-gallery-schema.json`
- Developer docs: `docs/article-gallery-mapping.md`, `docs/product-surfaces.md`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Canon file scaffolding and feature branch readiness

- [ ] T001 Create empty canon array files `articles.json` and `galleries.json` in `src/Turpinverse.Data/canon/` (each `[]` until authored)
- [ ] T002 Verify embedded resource glob in `src/Turpinverse.Data/Turpinverse.Data.csproj` includes new canon files via `canon\*.json`
- [ ] T003 Confirm feature branch `005-article-gallery-content` is active and `specs/005-article-gallery-content/` design artifacts are present

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core models, schema, repository, validator, and API counts that MUST complete before user story work

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Tests for Foundational (failing-first) ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T004 [P] Add failing CanonValidator article/gallery fixture tests (VR-028–VR-035) in `tests/Turpinverse.Core.UnitTests/Validation/ArticleGalleryValidatorTests.cs` with `[Trait("Category", "CanonValidation")]`
- [ ] T005 [P] Add failing CanonSchemaValidator tests for Article, Gallery, GalleryImage, and ViewerHint in `tests/Turpinverse.Core.UnitTests/Validation/ArticleGallerySchemaValidatorTests.cs`
- [ ] T006 [P] Add failing JsonCanonRepository load tests for `articles.json` and `galleries.json` in `tests/Turpinverse.Core.UnitTests/Data/JsonCanonRepositoryTests.cs`
- [ ] T007 [P] Add failing validate-response count assertions for `articles` and `galleries` in `tests/Turpinverse.Core.UnitTests/Validation/CanonValidatorTests.cs` (or API integration test targeting `src/Turpinverse.Web/Endpoints/CanonEndpoints.cs`)

### Implementation for Foundational

- [ ] T008 [P] Create `GalleryImage` model in `src/Turpinverse.Core/Models/GalleryImage.cs`
- [ ] T009 [P] Create `ViewerHint` model in `src/Turpinverse.Core/Models/ViewerHint.cs`
- [ ] T010 [P] Create `Article` model in `src/Turpinverse.Core/Models/Article.cs`
- [ ] T011 [P] Create `Gallery` model in `src/Turpinverse.Core/Models/Gallery.cs`
- [ ] T012 Extend `Canon` with `Articles` and `Galleries` collections in `src/Turpinverse.Core/Models/Canon.cs`
- [ ] T013 Update `JsonCanonRepository` to load `articles.json` and `galleries.json` in `src/Turpinverse.Data/Repositories/JsonCanonRepository.cs`
- [ ] T014 Merge additive types from `specs/005-article-gallery-content/contracts/article-gallery-schema.json` into `specs/001-turpinverse-universe/contracts/canon-schema.json` (`articles`, `galleries` required arrays and `$defs`)
- [ ] T015 Extend `CanonSchemaValidator` for Article and Gallery entity types in `src/Turpinverse.Core/Validation/CanonSchemaValidator.cs`
- [ ] T016 Implement VR-028–VR-035 validation methods and add `articles` and `galleries` to result counts in `src/Turpinverse.Core/Validation/CanonValidator.cs`
- [ ] T017 Add `articles` and `galleries` keys to `/api/canon/validate` response counts in `src/Turpinverse.Web/Endpoints/CanonEndpoints.cs`

**Checkpoint**: Foundation ready — models load, schema validates structure, validator tests fail on empty/missing volume until data ships

---

## Phase 3: User Story 1 - Reusable in-universe articles (Priority: P1) 🎯 MVP

**Goal**: Ten published Turpin Enterprises team-blog article records with author split, topic mix, collection labels, and FR-005 metadata example; automated completeness passes article rules (FR-003–FR-005, FR-008, FR-013, FR-016)

**Independent Test**: Inspect `src/Turpinverse.Data/canon/articles.json` (or generated `site/data/articles.json` after P3). Confirm ten `draft: false` records with title, `publishedAt`, body, `authorPersonaId`, `collection`; three by `dick-turpin` and seven by other TE members; one `relatedProjectId` `black-bess-route-optimiser`; two sharing the same TE `relatedCaseId` (recommended `case-007`); seven role-shaped pieces; at least one article with tags, featured image, excerpt, and `showTableOfContents`; `dotnet test --filter "Category=CanonValidation"` passes article VR rules

### Tests for User Story 1 (failing-first) ⚠️

- [ ] T018 [P] [US1] Add failing article volume and author-split integration test (VR-030) in `tests/Turpinverse.Core.UnitTests/Validation/ArticleGalleryValidatorTests.cs`
- [ ] T019 [P] [US1] Add failing topic-mix and FR-005 metadata example tests (VR-031, VR-032) in `tests/Turpinverse.Core.UnitTests/Validation/ArticleGalleryValidatorTests.cs`

### Implementation for User Story 1

- [ ] T020 [US1] Author ten published articles in `src/Turpinverse.Data/canon/articles.json` (3× `dick-turpin`; 7× other `turpin-enterprises` members; shared `collection` label; in-universe copy per FR-008)
- [ ] T021 [US1] Set `relatedProjectId` to `black-bess-route-optimiser` on exactly one published article in `src/Turpinverse.Data/canon/articles.json`
- [ ] T022 [US1] Set the same `relatedCaseId` (recommended `case-007`) on exactly two published articles in `src/Turpinverse.Data/canon/articles.json`
- [ ] T023 [US1] Exercise FR-005 on at least one published article (non-empty `tags`, `featuredImage`, `excerpt`, `showTableOfContents: true`) in `src/Turpinverse.Data/canon/articles.json`
- [ ] T024 [US1] Confirm `CanonValidator.Validate` passes VR-028–VR-032 and VR-034 for articles with `Category=CanonValidation` in `tests/Turpinverse.Core.UnitTests/Validation/ArticleGalleryValidatorTests.cs`

**Checkpoint**: User Story 1 fully functional — ten published articles in canon; article completeness rules pass; records are listable as generic content without a consumer theme tree

---

## Phase 4: User Story 2 - Reusable image gallery (Priority: P2)

**Goal**: At least one Turpin Enterprises team/workplace gallery with four captioned images and lightbox viewer preference; automated completeness passes gallery rules (FR-006–FR-007, FR-013)

**Independent Test**: Open `src/Turpinverse.Data/canon/galleries.json`. Confirm ≥1 gallery with title, `subject` `team`/`workplace`/`brand`, ≥4 ordered images each with `src` and caption or alt, `viewer.enabled` true; completeness fails if image count &lt; 4 or any slot lacks caption/alt; `Category=CanonValidation` passes VR-033 and VR-035

### Tests for User Story 2 (failing-first) ⚠️

- [ ] T025 [P] [US2] Add failing gallery minimum and caption tests (VR-033, VR-035) in `tests/Turpinverse.Core.UnitTests/Validation/ArticleGalleryValidatorTests.cs`

### Implementation for User Story 2

- [ ] T026 [US2] Author one workplace/team/brand gallery with four placeholder or remote images in `src/Turpinverse.Data/canon/galleries.json`
- [ ] T027 [US2] Set `viewer.enabled` true and generic `mode` (e.g. `lightbox`) on the required gallery in `src/Turpinverse.Data/canon/galleries.json`
- [ ] T028 [US2] Ensure every gallery image has non-empty `caption` or `alt` in `src/Turpinverse.Data/canon/galleries.json`
- [ ] T029 [US2] Confirm full canon validation passes including galleries (`valid == true`, zero violations) via `Category=CanonValidation` in `tests/Turpinverse.Core.UnitTests/Validation/ArticleGalleryValidatorTests.cs`

**Checkpoint**: User Story 2 fully functional — gallery record meets volume, caption, subject, and viewer rules; full canon validation passes

---

## Phase 5: User Story 3 - Public showcase of articles and galleries (Priority: P3)

**Goal**: Hugo-only reader-facing article list/detail, gallery list/detail, persona article lists, nav/home discoverability, named project/case links, collection labels, and no export plumbing (FR-009–FR-011, FR-014–FR-015)

**Independent Test**: Run `dotnet run --project src/Turpinverse.Tools.GenerateHugoContent`, serve Hugo site. From home or primary nav open Articles and Gallery; article pages show author as named person and collection label; persona pages list published articles or omit block; Black Bess and case articles show named project/case links; gallery shows captions; no collection index in nav; no export UI; `FullyQualifiedName~Hugo` tests pass

### Tests for User Story 3 (failing-first) ⚠️

- [ ] T030 [P] [US3] Add failing HugoContentGenerator article emission tests (published-only, draft omitted, collection and author front matter) in `tests/Turpinverse.Core.UnitTests/Hugo/ArticleGalleryHugoTests.cs`
- [ ] T031 [P] [US3] Add failing HugoContentGenerator gallery emission tests (ordered images, viewer hint, data JSON) in `tests/Turpinverse.Core.UnitTests/Hugo/ArticleGalleryHugoTests.cs`
- [ ] T032 [P] [US3] Add failing Hugo identity and named related-link tests for articles in `tests/Turpinverse.Core.UnitTests/Hugo/HugoIdentityRulesTests.cs` and `tests/Turpinverse.Core.UnitTests/Hugo/HugoShowcaseLinkTests.cs`
- [ ] T033 [P] [US3] Add failing persona published-articles list and omit-empty tests in `tests/Turpinverse.Core.UnitTests/Hugo/ArticleGalleryHugoTests.cs`

### Implementation for User Story 3

- [ ] T034 [US3] Extend `HugoContentGenerator` to emit `content/articles/_index.md`, `content/articles/{id}.md` (published only), and `site/data/articles.json` in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [ ] T035 [US3] Extend `HugoContentGenerator` to emit `content/galleries/_index.md`, `content/galleries/{id}.md`, and `site/data/galleries.json` in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [ ] T036 [US3] Map `showTableOfContents` to generated front matter (e.g. `ShowToc`) without storing theme keys in canon in `src/Turpinverse.Core/Hugo/HugoContentGenerator.cs`
- [ ] T037 [P] [US3] Create article list layout `site/layouts/articles/list.html` with collection label per item and published-only entries
- [ ] T038 [P] [US3] Create article detail layout `site/layouts/articles/single.html` with author as named person link, collection label, and named project/case partials
- [ ] T039 [P] [US3] Create gallery list layout `site/layouts/galleries/list.html`
- [ ] T040 [P] [US3] Create gallery detail layout `site/layouts/galleries/single.html` with visible captions/alt and generic lightbox behaviour when `viewer.enabled`
- [ ] T041 [US3] Add published-articles section with omit-empty to `site/layouts/personas/single.html` driven by `site/data/articles.json`
- [ ] T042 [P] [US3] Create partials for collection label and related project/case named links in `site/layouts/partials/` (e.g. `article-collection.html`, `article-related-links.html`)
- [ ] T043 [US3] Add Articles and Gallery entries to `menu.main` in `site/hugo.toml` (same discoverability as people, organisations, deals)
- [ ] T044 [US3] Add article and gallery quick links and/or home stats to `site/layouts/index.html`
- [ ] T045 [US3] Run `src/Turpinverse.Tools.GenerateHugoContent` and commit generated `site/content/articles/`, `site/content/galleries/`, and updated `site/data/` artifacts
- [ ] T046 [P] [US3] Add optional repo-only mapping note in `docs/article-gallery-mapping.md` (no named theme as source of truth; not Hugo content)
- [ ] T047 [US3] Update Hugo inventory for articles and galleries in `docs/product-surfaces.md`
- [ ] T048 [US3] Confirm `ExportDatasets` and Blazor export surfaces still omit articles and galleries (FR-012) in `src/Turpinverse.Core/Export/` (no new dataset types)

**Checkpoint**: User Story 3 fully functional — public site showcases articles and galleries per contracts/hugo-showcase.md; persona, nav, and related-link rules hold

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: End-to-end validation, channel charter, and documentation sync

- [ ] T049 Run full `Category=CanonValidation` and `FullyQualifiedName~Hugo` test suites per `specs/005-article-gallery-content/quickstart.md`
- [ ] T050 [P] Walk Hugo article and gallery pages for SC-005 (no export/download controls; no technical ids as primary identity)
- [ ] T051 [P] Content review pass on article titles/bodies and gallery image choices for SC-006 plausibility and fictional-PII rules in `src/Turpinverse.Data/canon/articles.json` and `src/Turpinverse.Data/canon/galleries.json`
- [ ] T052 Confirm no Hugo collection taxonomy or `/collections/` index routes were added under `site/`
- [ ] T053 Verify `docs/article-gallery-mapping.md` (if present) is not published as Hugo content and SC-007 holds

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational — delivers MVP canon articles
- **User Story 2 (Phase 4)**: Depends on Foundational — can run in parallel with US1 after Phase 2 (different canon file)
- **User Story 3 (Phase 5)**: Depends on US1 and US2 records existing (generator and layouts need real data); Hugo work can start test-first in parallel once foundational models exist
- **Polish (Phase 6)**: Depends on US1–US3 completion

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational — no dependency on US2/US3 for canon records
- **User Story 2 (P2)**: Can start after Foundational — independent of US1 for gallery JSON (validator runs on full canon at end)
- **User Story 3 (P3)**: Requires published article and gallery canon from US1/US2 for meaningful generated site; Hugo tests can be written failing-first earlier

### Within Each User Story

- Tests MUST be written and FAIL before implementation (Principle III)
- Foundational models and validator before authoring JSON
- US1/US2 canon data before final Hugo generation (T045)
- Core generator changes before layout templates that consume generated data

### Parallel Opportunities

- T004–T007 (foundational tests) in parallel
- T008–T011 (models) in parallel
- T018–T019 (US1 tests) in parallel
- T025 (US2 test) alongside US1 implementation after foundational
- T030–T033 (US3 Hugo tests) in parallel
- T037–T040, T042 (layouts/partials) in parallel after generator contract is clear
- US1 and US2 authoring (T020–T028) can proceed in parallel on different JSON files once Phase 2 completes

---

## Parallel Example: User Story 1

```bash
# Launch US1 failing tests together:
Task: "Add failing article volume and author-split integration test (VR-030) in tests/Turpinverse.Core.UnitTests/Validation/ArticleGalleryValidatorTests.cs"
Task: "Add failing topic-mix and FR-005 metadata example tests (VR-031, VR-032) in tests/Turpinverse.Core.UnitTests/Validation/ArticleGalleryValidatorTests.cs"
```

---

## Parallel Example: User Story 3

```bash
# Launch Hugo failing tests together:
Task: "Add failing HugoContentGenerator article emission tests in tests/Turpinverse.Core.UnitTests/Hugo/ArticleGalleryHugoTests.cs"
Task: "Add failing HugoContentGenerator gallery emission tests in tests/Turpinverse.Core.UnitTests/Hugo/ArticleGalleryHugoTests.cs"
Task: "Add failing Hugo identity and named related-link tests in tests/Turpinverse.Core.UnitTests/Hugo/HugoIdentityRulesTests.cs"

# Launch layouts together after generator emits paths:
Task: "Create article list layout site/layouts/articles/list.html"
Task: "Create article detail layout site/layouts/articles/single.html"
Task: "Create gallery list layout site/layouts/galleries/list.html"
Task: "Create gallery detail layout site/layouts/galleries/single.html"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories)
3. Complete Phase 3: User Story 1 (ten published articles; article VR rules pass)
4. **STOP and VALIDATE**: `Category=CanonValidation` passes article rules; reviewer can list/open generic records (SC-001, SC-002 partial)
5. Deploy/demo canon-only review before Hugo publication

### Incremental Delivery

1. Setup + Foundational → validator and schema ready
2. Add User Story 1 → article canon complete → validate independently
3. Add User Story 2 → gallery canon complete → full canon validation passes
4. Add User Story 3 → Hugo showcase → validate SC-004/SC-005 on public site
5. Polish → quickstart scenarios A–D

### Parallel Team Strategy

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (`articles.json`)
   - Developer B: User Story 2 (`galleries.json`)
   - Developer C: User Story 3 failing Hugo tests, then layouts after A/B ship data
3. Integrate with `GenerateHugoContent` and run Polish phase

---

## Notes

- [P] tasks = different files, no dependencies on incomplete tasks in the same batch
- [Story] label maps task to specific user story for traceability
- Do not add Blazor routes, CSV columns, or `ExportDatasets` entries for articles/galleries (FR-012)
- Draft articles may be added later but MUST stay off published Hugo lists (FR-011)
- Collection labels only — no Hugo taxonomy or collection index pages
- Recommended case id for the two case articles: `case-007` (see `specs/005-article-gallery-content/research.md`)
- Off-theme gallery binaries remain a human content-review gate on top of VR-035
