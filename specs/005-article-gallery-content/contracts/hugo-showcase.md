# Contract: Public reference site (Hugo) — articles and galleries

**Channel**: Public reference site (showcase / read) only  
**Surfaces**: Generated markdown under `site/content/`, layouts under `site/layouts/`, menu in `site/hugo.toml`  
**Trace**: FR-001, FR-002, FR-009, FR-010, FR-011, FR-012, FR-015, FR-016

The public site MUST remain readable without the export app. It MUST NOT expose
download/import workflows on article or gallery pages.

## Information architecture

| Path | Kind | Content |
|------|------|---------|
| `/articles/` | **new** list | Dedicated article list; collection label per item; published only |
| `/articles/{id}/` | **new** detail | Title, date, body, author as person, collection label, related named links |
| `/galleries/` | **new** list | Dedicated gallery list (one or more) |
| `/galleries/{id}/` | **new** detail | Ordered images with visible caption or alt text |
| `/personas/{slug}/` | existing | Add published-articles list; omit if empty |
| `/organisations/turpin-enterprises/` | existing | No new gallery link required |
| `/projects/{id}/`, `/cases/{id}/` | existing | No reverse article list required |

Do **not** add `/collections/` or taxonomy collection indexes.

Primary navigation (`menu.main`) and home quick links MUST include Articles and
Gallery (or Galleries) with the same discoverability as People, Organisations, and
Deals (FR-010). Home stats MAY include article and gallery counts.

## Generated files (from `IHugoContentGenerator`)

- `content/articles/_index.md` and `content/articles/{id}.md` for each **published** article
- `content/galleries/_index.md` and `content/galleries/{id}.md` for each gallery
- YAML `title` = article title or gallery title (not the slug)
- Body = human-readable markdown from canon `body` / gallery copy
- Join keys only in front matter or `site/data/articles.json` / `galleries.json`
- Skip `draft: true` articles entirely for published content and list pages
- Project `showTableOfContents` to generated front matter if useful; do not store PaperMod keys in canon

## Visible identity rules

On showcase HTML:

- **MUST** identify articles by title (and date if needed); authors by persona display name
- **MUST** identify related project/case by **named** link (project title / case subject), not only a hidden join
- **MUST** show collection as a human-readable label on list and detail
- **MUST NOT** use `article id`, `authorPersonaId`, export column names, or case/project ids as the visible identity
- **MAY** use slugs in URLs

## Forbidden on showcase pages

- Download CSV / import-file controls / filter builders
- Technical identifiers as the primary way to name a piece
- Empty “Articles” heading on a persona with no published articles
- Collection index as a primary browse path

## Lightbox / captions

Gallery pages MUST render each image’s caption or alternative description as
reader-visible text. Viewer/lightbox MAY be a generic HTML/CSS behaviour driven
by canon `viewer.enabled`; MUST NOT require a named theme’s gallery layout key
in stored data.

## Completeness of generation

A published site built from current canon MUST include:

- Ten published article pages and a list that links them
- At least one gallery page with ≥4 captioned images
- Author name on each article page linking to `/personas/{authorPersonaId}/`
- On Richard Turpin and every other authoring persona, the articles list matching their published set
- Named links on the Black Bess article and both case articles to `/projects/black-bess-route-optimiser/` and `/cases/{chosenCaseId}/` respectively
