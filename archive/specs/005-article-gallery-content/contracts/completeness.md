# Completeness and publication contract

**Version**: 1.0.0 | **Date**: 2026-08-22 | **Feature**: `005-article-gallery-content`

Extends existing canon validation. CRM CSV contracts are unchanged. No new export
dataset types.

## GET `/api/canon/validate`

Same endpoint as [001 export-api.md](../../001-turpinverse-universe/contracts/export-api.md)
(`DemoExport` policy). Response `counts` MUST include:

| Key | Source |
|-----|--------|
| `articles` | Number of article records (including drafts if any) |
| `galleries` | Number of gallery records |

`violations[]` items keep `{ rule, message, entityType, entityId }`.

| HTTP | When |
|------|------|
| 200 | `valid: true` including article/gallery rules |
| 422 | Any VR-028–VR-035 or schema failure |

## Validation codes

| Code | EntityType examples | Pass condition |
|------|---------------------|----------------|
| VR-028 | Article | Unique ids; required fields; non-empty title/body/collection |
| VR-029 | Article | Author persona exists and is a `turpin-enterprises` member |
| VR-030 | Canon / Article | 10 published; 3 by `dick-turpin`; 7 by other TE members |
| VR-031 | Article | 1× `black-bess-route-optimiser`; 2× same TE support case |
| VR-032 | Article | ≥1 article with tags, featured image, excerpt, TOC hint |
| VR-033 | Gallery | ≥1 gallery; ≥4 images; caption or alt on every image; viewer enabled |
| VR-034 | Article | Related project/case ids resolve; case account is Turpin Enterprises when used for the mix |
| VR-035 | Gallery | `subject` in `team` \| `workplace` \| `brand`; image `src` present |

`entityId` SHOULD be the article or gallery `id`, or `articles` / `galleries` for volume failures.

Recommended case id for VR-031: `case-007` (see [research.md](../research.md)).

## Human-facing surfaces (not new APIs)

| Channel | Contract |
|---------|----------|
| Hugo article list | `/articles/` linked from primary nav and/or home; collection label visible; published only |
| Hugo article detail | `/articles/{id}/` — author as named person; collection label; named project/case link when related |
| Hugo gallery | `/galleries/` and `/galleries/{id}/` from nav and/or home; captions visible; no org-page gallery link required |
| Hugo persona page | Lists that persona’s published articles; omit section if none |
| Blazor / CSV | Unchanged; MUST NOT add article or gallery datasets |

See [hugo-showcase.md](./hugo-showcase.md).

## Out of scope

- New export dataset types or contact-page article lists in the export app
- Collection index / taxonomy pages
- Reverse article lists on project, case, or organisation pages
- Named CMS or theme file layout as stored format
- Draft fixture (optional later; must stay off published lists)
