# Quickstart: Generic Article and Gallery Demo Data

**Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

Validates generic records and public-site publication. Does not replace
[001 quickstart](../001-turpinverse-universe/quickstart.md). Does **not** require
the Blazor export app for reader-facing checks.

## Prerequisites

- .NET 10 SDK (`global.json`)
- Hugo Extended **or** `scripts/Invoke-HugoSite.ps1` (Podman/Docker)
- Restore: `dotnet restore`

## Setup

```bash
dotnet build
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent
```

Regenerate Hugo content after changing canon JSON so `site/content/articles/`,
`site/content/galleries/`, persona layouts, and `site/data/` match
[data-model.md](./data-model.md).

## Automated completeness (SC-002, SC-003, FR-013)

These MUST fail until models, JSON, schema merge, and validator rules exist; they
MUST pass when the feature is done.

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
```

Expect `CanonValidator.Validate` on the loaded canon: `valid == true`, zero
violations, and counts for `articles` and `galleries`.

Also run Hugo generator tests (identity, published-only, related named-link
front matter, persona article lists):

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "FullyQualifiedName~Hugo"
```

Optional (API enabled in Development):

```bash
# After Aspire / Turpinverse.Web is running
curl -sS http://localhost:{port}/api/canon/validate
```

422 with VR-028–VR-035 until data and links are complete; 200 when they are.
See [contracts/completeness.md](./contracts/completeness.md).

## Scenario A — List generic articles (P1, SC-001, SC-002)

1. Inspect `src/Turpinverse.Data/canon/articles.json` (or generated
   `site/data/articles.json`).
2. Confirm **ten** records with `draft: false`, each with title, `publishedAt`,
   body, `authorPersonaId`, `collection`.
3. Confirm bylines: three `dick-turpin`, seven other Turpin Enterprises members.
4. Confirm topics: one `relatedProjectId` `black-bess-route-optimiser`; two sharing
   the same TE `relatedCaseId`; seven role-shaped pieces.
5. Confirm at least one article has tags, featured image, excerpt, and TOC hint.

Expected: a reviewer can list and open titles, dates, and bodies in under five
minutes from generic records (SC-001).

## Scenario B — Gallery record (P2, FR-006–FR-007)

1. Open `galleries.json`.
2. Confirm ≥1 gallery titled, `subject` team/workplace/brand, ≥4 ordered images,
   each with `src` and caption or alt, `viewer.enabled` true.

Expected: completeness fails if any slot lacks caption/alt or image count &lt; 4.

## Scenario C — Public showcase (P3, SC-004, SC-005)

Serve the Hugo site (see `site/README.md`). Without starting Blazor:

1. From home or primary nav, open **Articles** and **Gallery**.
2. Open an article: author is a named person; collection label visible; no export UI.
3. Open Richard Turpin’s persona page: published articles listed. Open a persona
   with none: no empty articles block.
4. Open the Black Bess article and a case article: named links to the project and
   case pages.
5. Open the gallery: captions visible; images read as team/workplace/brand.
6. Confirm no collection index in nav; Turpin Enterprises org page need not link
   the gallery.

Expected: first-time visitor finds articles and gallery in under five minutes
(SC-004); walkthrough finds zero export/download controls (SC-005).

## Scenario D — Channel and mapping (FR-002, FR-012, FR-014)

1. Confirm `ExportDatasets` still has no articles/galleries types.
2. If `docs/article-gallery-mapping.md` exists, it is repo-only (not a Hugo page)
   and does not make a named theme required.

Expected: SC-007 — no CMS/theme layout required to mark the feature done.
