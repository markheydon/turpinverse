# Implementation Plan: Generic Article and Gallery Demo Data

**Branch**: `005-article-gallery-content` | **Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/005-article-gallery-content/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

**Channels affected**: **Public reference site (Hugo — showcase / read)** only. Articles and galleries are human-readable content; they are not interactive-export datasets. Shared canon JSON remains the single source of truth. This plan respects Constitution Principle IX and [`docs/product-surfaces.md`](../../docs/product-surfaces.md): reader pages MUST NOT present technical identifiers or export plumbing as primary content. The Blazor app is **not** required to add CSV, filters, or downloads for these types.

## Summary

Add first-class Turpinverse **article** and **gallery** canon (issue #21, parent #18):
ten published team-blog articles (3 by Richard Turpin, 7 by other Turpin Enterprises
members) plus at least one team/workplace gallery of four captioned images. Records
are JSON beside existing canon—not a CMS or theme file tree. Completeness is
`CanonValidator` VR-028–VR-035. Publish on new Hugo article list/detail and gallery
pages (nav + home), bylines on existing persona pages, and named project/case links
on the related articles. No Blazor/CSV surface. Optional repo-only mapping note.

## Technical Context

**Language/Version**: C# / .NET 10; Hugo Extended (existing `site/`, PaperMod module)

**Primary Dependencies**: Existing `JsonCanonRepository`, `CanonValidator`,
`CanonSchemaValidator` (JsonSchema.Net), `HugoContentGenerator`; ASP.NET Core /
Aspire unchanged for this increment

**Storage**: File-based JSON in `src/Turpinverse.Data/canon/`
(`articles.json`, `galleries.json`); no database

**Testing**: xUnit v3, NSubstitute, coverlet; `Category=CanonValidation` plus Hugo
generator/identity tests (bUnit not required unless a Web page is touched — it is not)

**Target Platform**: Cross-platform .NET 10; Hugo site on GitHub Pages from existing
`site/` pipeline; no new Blazor routes

**Project Type**: Multi-artifact OSS monorepo — extend Data / Core / Hugo layouts;
no new csproj

**Performance Goals**: Canon load + validate including new collections well under
existing CSV export budget (2s); Hugo regenerate comparable to current generator

**Constraints**: English-only in-universe copy; Principle IX Hugo-only publication;
no named theme as stored format; no new anonymous bulk download; fictional PII only;
placeholder/remote images allowed; no collection taxonomy pages

**Scale/Scope**: 10 published articles, ≥1 gallery (≥4 images); 6 eligible TE authors;
1 project FK, 1 chosen TE case; dedicated Hugo sections + persona article lists

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Answer **Yes** or **No** for each principle. Any **No** without a Complexity
Tracking justification blocks the gate.

| Principle | Pre-Design | Post-Design | Evidence |
|-----------|------------|-------------|----------|
| I. Spec-Driven Traceability | Yes | Yes | spec.md US1–US3, FR-001–FR-016, SC-001–SC-007; issues #21/#18 |
| II. Incremental Independence | Yes | Yes | P1 articles+validator; P2 gallery+validator; P3 Hugo showcase independently testable once records exist |
| III. Verifiable Testability | Yes | Yes | Failing-first CanonValidation + Hugo tests in [quickstart.md](./quickstart.md); `/api/canon/validate` counts |
| IV. Separation of Concerns | Yes | Yes | JSON canon / Core models+validator / Hugo generator+layouts / optional docs mapping |
| V. Explicit Error Handling | Yes | Yes | Spec failure table; VR-028–VR-035; 422 on validate; drafts omitted from generate |
| VI. Security by Design | Yes | Yes | Spec security table; fictional copy; no new public download; IDs off Hugo primary copy |
| VII. Justified Complexity | Yes | Yes | Reuse JSON+validator+Hugo generator; no CMS, no Blazor dual-publish, no taxonomy |
| VIII. Documentation Contract | Yes | Yes | [contracts/](./contracts/), [data-model.md](./data-model.md), [quickstart.md](./quickstart.md) |
| IX. Human-Facing Channel Charter | Yes | Yes | Plan names Hugo only; articles/galleries are showcase; Blazor/CSV explicitly out of scope |

**Gate result**: PASS

## Project Structure

### Documentation (this feature)

```text
specs/005-article-gallery-content/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/
│   ├── article-gallery-schema.json
│   ├── completeness.md
│   └── hugo-showcase.md
└── tasks.md             # Phase 2 output (/speckit-tasks — NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/Turpinverse.Data/canon/
├── articles.json
└── galleries.json

src/Turpinverse.Data/Repositories/JsonCanonRepository.cs
src/Turpinverse.Core/Models/          # Article, Gallery, GalleryImage, ViewerHint; Canon extras
src/Turpinverse.Core/Validation/      # CanonValidator VR-028–VR-035; merge into 001 canon-schema.json

src/Turpinverse.Core/Hugo/            # HugoContentGenerator: articles, galleries, data JSON
src/Turpinverse.Tools.GenerateHugoContent/

site/hugo.toml                        # menu.main Articles + Gallery
site/layouts/index.html               # home stats + quick links
site/layouts/articles/                # list + single
site/layouts/galleries/               # list + single
site/layouts/personas/single.html     # published articles list; omit-empty
site/layouts/partials/                # persona-title, collection label, related project/case

docs/article-gallery-mapping.md       # optional P3 mapping note (not Hugo content)
docs/product-surfaces.md              # implement: list articles/galleries on Hugo

tests/Turpinverse.Core.UnitTests/Validation/
tests/Turpinverse.Core.UnitTests/Hugo/
```

**Structure Decision**: Extend the existing Data / Core / Hugo-tool split from
`001-turpinverse-universe` and publication patterns from `003`/`004`. No new
project. Runtime JSON Schema remains embedded from
`specs/001-turpinverse-universe/contracts/canon-schema.json` and MUST be updated to
`$ref` or inline the additive types. Do not add Blazor pages or `ExportDatasets`
entries.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

No constitution violations. Separate Hugo article/gallery sections are required by
FR-010 (dedicated list and gallery pages), not extra architecture.

## Phase 0 / Phase 1 outputs

- [research.md](./research.md) — storage, validator, authorship, topic mix, Hugo-only
- [data-model.md](./data-model.md) — entities, uniqueness, VR codes
- [contracts/article-gallery-schema.json](./contracts/article-gallery-schema.json)
- [contracts/completeness.md](./contracts/completeness.md)
- [contracts/hugo-showcase.md](./contracts/hugo-showcase.md)
- [quickstart.md](./quickstart.md)

## Implementation notes (for `/speckit-tasks`)

1. Write failing validator and schema tests (empty collections, orphan authors,
   wrong author split, topic mix, gallery below four images / missing captions)
   before shipping passing default JSON.
2. Merge additive schema into `001` `canon-schema.json` (`articles`, `galleries`
   required arrays + `$defs`).
3. Author ten published articles: 3 `dick-turpin`; 7 from `mary-brazier`,
   `ned-palmer`, `henry-clayton`, `catherine-bell` (and optionally `black-bess` only
   if SC-006 plausibility still holds). Recommended case: `case-007`. Single
   collection label is enough.
4. Author one workplace/team/brand gallery with four placeholder images and
   lightbox `viewer.enabled`.
5. Generate Hugo content; add layouts, nav, home links; persona omit-empty article
   list; named project/case links; no collection indexes.
6. Optional `docs/article-gallery-mapping.md`; update `docs/product-surfaces.md`
   Hugo inventory.
7. Do not add export datasets, CSV columns, or Blazor article UI.
