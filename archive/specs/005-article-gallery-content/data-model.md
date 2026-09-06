# Data Model: Generic Article and Gallery Demo Data

**Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Overview

Articles and galleries are **authored canon**. They load from JSON beside existing
files in `src/Turpinverse.Data/canon/` and hang off `Canon`. They are not CRM
export rows. CSV datasets and Blazor explore/download surfaces do not gain these
types.

Existing **Persona**, **Organisation**, **Project**, and **Case** records are
unchanged except that Hugo reads them for bylines and named related links.

## New canon files

| File | Entity | Cardinality |
|------|--------|-------------|
| `articles.json` | Article | Ten published required; extra drafts allowed later |
| `galleries.json` | Gallery | At least one |

## Entities

### Article

Reusable thought-leadership / blog-like item. Not a persona, org, timeline, deal,
case, or project page.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique among articles |
| `title` | string | ✅ | Non-empty; human-facing identity with date |
| `publishedAt` | string (`YYYY-MM-DD`) | ✅ | Valid calendar date |
| `draft` | boolean | ✅ | Ten required articles have `false` |
| `body` | string | ✅ | Human-authored copy; markdown allowed |
| `authorPersonaId` | string (slug) | ✅ | Existing persona; member of `turpin-enterprises` |
| `collection` | string | ✅ | Generic grouping label (not a CMS path) |
| `tags` | string[] | ❌ | FR-005: ≥1 article includes a non-empty list |
| `featuredImage` | string | ❌ | Path or URL; FR-005 example |
| `excerpt` | string | ❌ | Description/summary; FR-005 example |
| `showTableOfContents` | boolean | ❌ | Generic TOC hint; FR-005 example `true` |
| `enableSpecialRendering` | boolean | ❌ | Optional math/special flag; shipping a `true` is optional |
| `relatedProjectId` | string (slug) | ❌ | If set, existing `Project.id` |
| `relatedCaseId` | string (slug) | ❌ | If set, existing `Case.caseId` |

**Uniqueness**: `id` unique (VR-028). Duplicate titles allowed if ids differ.

**Volume and mix (published = `draft == false`)** — VR-030, VR-031:

- Count published = 10
- Count `authorPersonaId == dick-turpin` among published = 3
- Count other published authors = 7, each still a Turpin Enterprises member, none `dick-turpin`
- Exactly one published article with `relatedProjectId == black-bess-route-optimiser`
- Exactly two published articles with the same `relatedCaseId`, and that case’s `accountId == turpin-enterprises`
- Remaining seven published articles MUST NOT be required to set project/case FKs (they MAY omit both)

**FR-005**: At least one article (published) has non-empty `tags`, non-empty `featuredImage`, non-empty `excerpt`, and `showTableOfContents == true` (VR-032).

**Relationships**:

- Many-to-one Persona (author). Publication lists articles on that persona’s existing page.
- Optional many-to-one Project / Case. Reader-facing named links only on the article page; reverse lists not required.
- Collection is a label, not an entity with pages.

### Gallery

Ordered Turpin Enterprises team or workplace image set.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique among galleries |
| `title` | string | ✅ | Non-empty |
| `description` | string | ❌ | Optional |
| `subject` | enum | ✅ | `team` \| `workplace` \| `brand` (machine stand-in for FR-006 theme) |
| `images` | GalleryImage[] | ✅ | Min 4; order is significant (array order) |
| `viewer` | ViewerHint | ✅ | `enabled` must be true on the required gallery |

### GalleryImage

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `src` | string | ✅ | Path or URL; non-empty |
| `caption` | string | ❌* | *At least one of `caption` or `alt` non-empty |
| `alt` | string | ❌* | Same as caption rule |

### ViewerHint

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `enabled` | boolean | ✅ | Required gallery: `true` |
| `mode` | string | ❌ | Generic, e.g. `lightbox` — not a theme layout key |
| `options` | object | ❌ | Additional generic hints; additionalProperties allowed as strings/numbers/booleans only in schema if present |

## Existing entities (read-only for this feature)

### Persona

Author of articles. No new people. Public person page lists published articles when `count > 0`.

### Organisation `turpin-enterprises`

Membership gate for authors. Organisation page is **not** required to link the gallery.

### Project `black-bess-route-optimiser`

Subject of exactly one article. Project page need not list the article.

### Case (Turpin Enterprises account)

Exactly one of `case-007`, `case-013`, `case-017` is chosen as `relatedCaseId` for two articles. Case page need not list the articles.

## State transitions

| Entity | States | Publication |
|--------|--------|-------------|
| Article | `draft: true` \| `false` | Only `false` gets a public article page and list entry |
| Gallery | No draft flag in this increment | All galleries in canon are eligible for public gallery pages |

## Validation codes

| Code | Pass condition |
|------|----------------|
| VR-028 | Article ids unique; required fields present; body/title non-empty |
| VR-029 | `authorPersonaId` exists and is in `turpin-enterprises.memberPersonaIds` |
| VR-030 | Exactly 10 published; 3 by `dick-turpin`; 7 by other TE members |
| VR-031 | Topic mix: 1 Black Bess project relation; 2 same TE case; rest unconstrained on those FKs |
| VR-032 | ≥1 article exercises tags + featuredImage + excerpt + TOC hint |
| VR-033 | ≥1 gallery with title, ≥4 images, each with caption or alt, `viewer.enabled` |
| VR-034 | `relatedProjectId` / `relatedCaseId` resolve when set; chosen case account is TE |
| VR-035 | Gallery `subject` is `team`, `workplace`, or `brand`; image `src` non-empty |

Schema failures remain `SCHEMA-*` from `CanonSchemaValidator`.

Off-theme **binaries** (product shots mislabelled as workplace) remain a content-review gate on top of VR-035.

## Publication projection (not stored format)

| Canon | Hugo |
|-------|------|
| Article (published) | `content/articles/{id}.md` — `title`, date, collection label, author display name, body; FKs only in front matter / `data/articles.json` |
| Article (draft) | No published markdown page |
| Gallery | `content/galleries/{id}.md` plus list `_index.md` |
| Persona articles | Section on existing `personas/{id}.md` layout from `data/articles.json` |
| Collection | Visible label only |
| `showTableOfContents` | MAY set generated `ShowToc` / equivalent; not a stored PaperMod key |

Join keys (`id`, `authorPersonaId`, `relatedProjectId`, `relatedCaseId`) MUST NOT be the visible identity in body copy (titles and display names are).
