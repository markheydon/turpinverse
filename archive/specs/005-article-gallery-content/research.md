# Research: Generic Article and Gallery Demo Data

**Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Decision: Store articles and galleries as JSON canon, not a theme tree

**Decision**: Add `articles.json` and `galleries.json` under `src/Turpinverse.Data/canon/`, load them through `JsonCanonRepository` onto `Canon`, and treat them as first-class records (same pattern as deals, cases, and career/portfolio).

**Rationale**: FR-001 and FR-002 forbid a named CMS path, PaperMod/Blowfish folder layout, or front-matter key set as the source of truth. Existing canon is already file-based JSON embedded in `Turpinverse.Data`. Reviewers can list records without generating Hugo.

**Alternatives considered**:

- Author only Hugo markdown under `site/content/posts/` — rejected: that *is* a consumer blog tree.
- Store front-matter YAML as canon — rejected: locks vocabulary to one static-site convention.
- Add a CMS (Decap, Ghost) — unjustified complexity for demo fixtures.

## Decision: Completeness lives in CanonValidator (VR-028–VR-035)

**Decision**: Extend `CanonValidator` and merge additive JSON Schema into `specs/001-turpinverse-universe/contracts/canon-schema.json` (runtime resource already embedded from that path). Surface new counts on `/api/canon/validate`. Do not rely on human review alone (FR-013).

**Rationale**: Career/portfolio already uses VR-020–VR-027 plus schema merge. Article volume, author split, topic mix, collection, gallery minima, captions, and resolving FKs are machine-checkable.

**Alternatives considered**:

- Hugo-only tests of generated pages — necessary for P3, insufficient for P1/P2 record gates.
- Image-content ML for “team/workplace” — overkill; store `subject` enum plus human review of assets.

## Decision: Author membership is Turpin Enterprises `MemberPersonaIds`

**Decision**: `authorPersonaId` MUST be in `Organisation` `turpin-enterprises` `memberPersonaIds` (today: `dick-turpin`, `mary-brazier`, `ned-palmer`, `black-bess`, `henry-clayton`, `catherine-bell`). Display name is derived from `Persona.displayName` at publication; it is not a free-text author field.

**Rationale**: Spec forbids orphan or non-member bylines. Bidirectional org membership is already VR-002/VR-003.

**Alternatives considered**:

- Free-text author name — fails FR-003/FR-015.
- Create new personas for bylines — out of spec assumptions.

## Decision: Topic mix uses existing project and one chosen case

**Decision**:

- Exactly one article with `relatedProjectId` = `black-bess-route-optimiser`.
- Exactly two articles with the same `relatedCaseId` among the three Turpin Enterprises cases (`case-007`, `case-013`, `case-017`). **Recommended choice**: `case-007` (brand guidelines review) so the case thread is narrative/reputation, not a second Black Bess product page.
- Seven articles with neither required project nor case relation.

Relations are optional fields on `Article`; completeness enforces the mix. Reverse lists on project/case pages are out of scope.

**Rationale**: Spec FR-016; named reader links are a Hugo projection of these FKs (FR-009).

**Alternatives considered**:

- Relate via tags only — too weak for named links and completeness.
- Duplicate project/case body into articles — forbidden by FR-009.

## Decision: Collections are a required string label, not a taxonomy

**Decision**: Every article has `collection` (human-readable label). All ten MAY share one value (recommended: a single team-journal label). Hugo shows it as a chip on list and detail. Do **not** enable a Hugo taxonomy or `_index` per collection.

**Rationale**: Spec: labels only; no collection index pages.

**Alternatives considered**:

- First-class `collections.json` with slugs and index pages — extra entity and forbidden browse path.
- Optional collection — fails FR-004.

## Decision: Hugo-only publication; no export datasets

**Decision**: `HugoContentGenerator` emits `content/articles/` and `content/galleries/` (list `_index.md` + one page per published article / gallery). Add `menu.main` and home quick-links/stats in `site/hugo.toml` and `site/layouts/index.html`. Persona `single.html` lists that person’s **published** articles or omits the block. Do **not** add `ExportDatasets` rows, CSV columns, or Blazor contact article lists (FR-012).

**Rationale**: Constitution IX and spec channel charter: articles/galleries are reader content on the public site.

**Alternatives considered**:

- Dual-publish on Blazor contact detail like career — spec explicitly out of scope.
- CSV “posts” dataset — FR-012.

## Decision: Drafts are a flag; none required

**Decision**: `draft: boolean` on every article. Generator writes markdown only when `draft` is false. Completeness requires ten published (`draft == false`). A draft fixture is optional and, if added later, must not appear in lists or detail routes.

**Rationale**: Spec FR-011.

## Decision: Images are references; captions are required on gallery slots

**Decision**: Featured and gallery images are paths or URLs (reuse `/images/...` SVG placeholders like projects). Gallery slots require `caption` or `alt` (at least one non-empty). `viewer.enabled` must be true on the required gallery; options are generic (`mode: "lightbox"`), not PaperMod shortcode names.

**Rationale**: Spec allows placeholders; FR-006/FR-007; binaries out of scope.

**Alternatives considered**:

- Ship a photo library — out of assumptions.
- Theme `gallery` shortcode as stored format — FR-002.

## Decision: Optional mapping note in docs/

**Decision**: P3 may add `docs/article-gallery-mapping.md` describing a *possible* blog/gallery projection. It MUST NOT be Hugo content or an acceptance gate for a named theme (FR-014).

**Rationale**: Same pattern as `docs/career-portfolio-mapping.md`.

## Decision: TOC and math are generic flags

**Decision**: Optional `showTableOfContents` and `enableSpecialRendering` booleans. Hugo MAY map TOC to PaperMod `ShowToc` at generate time; that mapping is a projection, not canon vocabulary. Math flag need not be exercised on a shipped article (FR-005).

**Rationale**: Spec: hints are optional metadata, not a rendering-engine contract.

## Resolved clarifications

No remaining NEEDS CLARIFICATION from Technical Context. Spec session 2026-08-22 answers volume, authorship, collections, gallery subject, nav, and related links.
