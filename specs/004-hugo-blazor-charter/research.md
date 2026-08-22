# Research: Human-Facing Channel Alignment

**Date**: 2026-08-22 | **Plan**: [plan.md](./plan.md) | **Spec**: [spec.md](./spec.md)

**Channels affected**: Public reference site (Hugo showcase / read) and interactive export app (Blazor explore / filter / download). Shared canon remains the single source of truth.

## 1. Generate Hugo deal and case pages from canon, do not author them by hand

**Decision**: Extend `HugoContentGenerator` to emit `site/content/deals/` and `site/content/cases/` (index + one markdown file per record) plus `site/data/deals.json` and `site/data/cases.json` for related-record lookups. Titles and body copy use `dealName` / `subject`, descriptions, stage/status, amount/priority, and **display names** for related parties. Join keys (`dealId`, `contactId`, `accountId`, `relatedEventId`) stay in front matter / data JSON for routing and `GetPage` resolution only.

**Rationale**: Personas and organisations already follow generate-from-canon (`IHugoContentGenerator` → markdown + `site/data`). Hand-written CRM pages would drift from `deals.json` / `cases.json`. FR-001, FR-011, and SC-001 require first-class indexes and details linked from primary nav or home — the same discoverability as `/personas/`, `/organisations/`, `/timeline/`.

**Alternatives considered**:
- *Render deals/cases only as asides on persona/org pages*: Rejected by FR-011 (dedicated indexes required).
- *Author markdown by hand*: Drifts from canon; violates the generator contract used for other showcase types.
- *Hugo as a second CRM table of import columns*: Violates FR-002, FR-003, FR-004, and Principle IX.

## 2. Bidirectional named links via resolved titles, not printed join keys

**Decision**: Keep existing slug-in-front-matter / slug-in-`site/data` patterns, but **visible** relationship lists must call display-name partials (`org-title.html`, `persona-title.html`, plus new deal-title / case-title / event-title helpers). If `GetPage` (or data lookup) fails, show a generic missing-related-record label (for example “Unknown related party”), never the raw slug as the only label (FR-008). Wire:

| Source page | Related lists (when data has a relationship) |
|-------------|-----------------------------------------------|
| Persona | Deals (`contactId`), cases (`contactId`) |
| Organisation | Deals (`accountId`), cases (`accountId`) |
| Timeline event row | Cases (`relatedEventId`); deals have no event FK — do not invent one |
| Deal | Person (`contactId`), organisation (`accountId`) |
| Case | Person, organisation, and timeline event when `relatedEventId` is set |

Empty relationship sets omit the section (or an empty-state sentence) rather than filling with IDs.

**Rationale**: Clarification 2026-08-22 requires bidirectional named links wherever shared data records a relationship. Deals only link contact + account; cases optionally link an event. Inferring deal↔event from shared people would invent relationships the canon does not store.

**Alternatives considered**:
- *Print `contactId` in body copy as the link text*: Banned by FR-003.
- *Dedicated timeline event permalinks*: Not required; the existing timeline list page can attach related cases under each event.
- *Resolve all names into markdown at generate time only*: Acceptable supplement, but layouts still need fallbacks when a page is missing.

## 3. Strip join keys from reader-facing Hugo copy; leave them in Blazor tables

**Decision**: Audit generator output and layouts so reader-facing headings, body, chips, and relationship lists never use join keys or CSV column names as identity. URL paths may keep stable slugs (`/deals/deal-001/`). Demo emails and job titles remain allowed. Blazor `DataTable` may keep identifier columns (FR-009). Fix `org-title.html` (and persona equivalent) so a missing page does **not** echo the slug.

**Rationale**: Charter gap “technical IDs in public-site body copy” and SC-002. Front matter may still hold slugs for linking — they are not primary visible identity if layouts resolve them.

**Alternatives considered**:
- *Hide all IDs by changing canon slugs to display names*: Breaking change across CSV and tests (rejected in feature 001 research).
- *Strip IDs from Blazor previews*: Undermines import exploration (FR-005 / FR-009).

## 4. Filter and download share one server-side predicate

**Decision**: Extend `IExportService.PreviewAsync` and `ExportCsvAsync` with an optional `ExportFilter` (dataset-specific discrete facets). Map the same object from query parameters on `GET /api/export/{dataset}` and `GET /api/export/{dataset}/preview`. `CrmEntityPage` loads preview with the current filter, and download uses the same query string. Unfiltered requests remain full-dataset (today’s behaviour). Facets for this feature:

| Dataset | Facets |
|---------|--------|
| contacts | `status`, `accountId` |
| accounts | `status`, `industry` |
| deals | `stage`, `accountId` |
| cases | `status`, `priority`, `accountId` |
| projects | out of this feature’s FR-005 list (contacts, accounts, deals, cases); leave unfiltered unless already trivial to share the same machinery |

Unknown query keys ignored; unknown facet **values** yield zero rows (empty match), not 400.

**Rationale**: Clarification: preview and download both apply current filters. Server-side filtering keeps CSV and table consistent and avoids a second client-only filter that `window.open('/api/export/...')` would ignore. Canon volumes are small (tens of rows), so returning all matching preview rows (raise/remove the “first 100 of unfiltered” truncation **when a filter is applied**, and for unfiltered lists still show the full set when under the existing 100 cap) is enough for SC-004.

**Alternatives considered**:
- *Client-only table filter + unfiltered CSV*: Violates FR-005.
- *POST body for filters*: Heavier than query strings; downloads are currently GET files.
- *Full-text search only*: Spec asks for fields that matter for import (stage, status, organisation).

## 5. Empty filter match: tell the user; do not write a silent empty or unfiltered file

**Decision**: When the current filter matches zero rows: show an explicit empty state on the dataset page (“Nothing matched”). Disable or intercept **Download CSV**; if the download URL is still hit, return **409 Conflict** `application/problem+json` (do not return 200 with empty CSV or the full unfiltered file). Unfiltered empty canon for a dataset is a load/data failure path already covered by existing load-error UI, not this rule.

**Rationale**: FR-010 and the failure-mode table. HTTP 409 distinguishes “query succeeded but no rows” from 400 invalid dataset.

**Alternatives considered**:
- *200 empty CSV*: Silent empty file — forbidden.
- *200 full dataset with a warning*: Forbidden substitution.
- *404*: Implies the dataset type does not exist.

## 6. Channel split in copy and IA, no new product

**Decision**: Hugo menu + home quick links gain **Deals** and **Cases**. Getting Started keeps the pointer to the local Blazor app for downloads; showcase templates gain no download/import controls. Blazor stays tabular explore/export; it does not grow a narrative encyclopedia. Dual publication of career/portfolio is unchanged (FR-006). Update `docs/product-surfaces.md` known-gap rows when the work lands.

**Rationale**: Principle IX, FR-004, FR-007, SC-003, SC-006.

**Alternatives considered**:
- *CSV buttons on Hugo*: Export plumbing on the public site.
- *Host Blazor as the public encyclopedia*: Opposite of the charter.
