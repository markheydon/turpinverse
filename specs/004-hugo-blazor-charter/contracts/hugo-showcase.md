# Contract: Public reference site (Hugo) showcase

**Channel**: Public reference site (showcase / read)  
**Surfaces**: Generated markdown under `site/content/`, layouts under `site/layouts/`, nav in `site/hugo.toml`  
**Trace**: FR-001, FR-002, FR-003, FR-004, FR-006, FR-008, FR-011, FR-012

The public site MUST remain readable without the export app running. It MUST NOT expose download/import workflows on showcase pages.

## Information architecture

| Path | Kind | Content |
|------|------|---------|
| `/personas/`, `/personas/{slug}/` | existing | People; add related deals/cases when FKs exist |
| `/organisations/`, `/organisations/{slug}/` | existing | Orgs; add related deals/cases when FKs exist |
| `/timeline/` | existing | Events; each event with `relatedEventId` cases lists those cases by **title** |
| `/deals/`, `/deals/{dealId}/` | **new** | Index + detail for every canon deal |
| `/cases/`, `/cases/{caseId}/` | **new** | Index + detail for every canon case |
| `/guides/getting-started/` | existing | May **point** to the local Blazor app for CSV; not an in-page importer |

Primary navigation (`menu.main`) and/or home quick links MUST include Deals and Cases with the same discoverability as Personas, Organisations, and Timeline (FR-011).

## Generated files (from `IHugoContentGenerator`)

For each canon deal and case:

- Markdown at `content/deals/{dealId}.md` / `content/cases/{caseId}.md`
- YAML `title` = deal name or case subject (quoted/escaped as today)
- Body = human-readable description (and reader-relevant facts), not a CSV dump
- Join keys only in front matter or `site/data/*.json`, not as the visible name in the body
- `_index.md` for each section
- `site/data/deals.json` and `site/data/cases.json` for cross-page lookups (same role as `organisations.json` / `events.json`)

Career/portfolio dual publication on persona pages MUST continue (FR-006).

## Visible identity rules

On showcase HTML (headings, body, chips, relationship lists):

- **MUST** identify records by display name, deal name, case subject, title, or demo email
- **MUST NOT** use join keys or export column names as the visible identity
- **MAY** use slugs in URLs and `href` paths
- Unresolved related records: human-readable fallback (not the slug alone) — FR-008

## Forbidden on showcase pages

- Download CSV / import-file controls
- API-key instructions as a workflow on people/orgs/timeline/deals/cases pages (Getting Started may explain how to run the app)
- Machine-oriented preview tables of import columns as the main content of a deal/case page

## Bidirectional links

Wherever canon stores a relationship (see [data-model.md](../data-model.md)), both ends show **named** links. Empty sets omit the list.

## Completeness

A published site built from current canon MUST include a deals page and a cases page for **every** row in `deals.json` and `cases.json`. Incomplete generation MUST NOT ship a section that looks complete while dumping identifiers (spec failure modes).
