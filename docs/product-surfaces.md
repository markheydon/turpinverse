# Product surfaces

Turpinverse publishes the same fictional dataset through two human-facing channels. They share one canon JSON source under `src/Turpinverse.Data/canon/`; each channel projects that data for a different job.

## Who this is for

Anyone who wants to use Turpinverse demo data for their own purposes, including:

- **CRM and business-app demos** — populate pipelines, accounts, contacts, and support cases with plausible sample data
- **Presentations and public-facing documentation** — illustrate workflows without real people's confidential information
- **Amusement** — enjoy a tongue-in-cheek Dick Turpin universe someone has built end to end

## The two channels

| Channel | URL / entry | Job |
|---------|-------------|-----|
| **Public reference site (Hugo)** | [turpinverse.uk](https://turpinverse.uk) | Browsable showcase of **human-readable** demo data |
| **Interactive export app (Blazor)** | Run locally via Aspire (see [README](../README.md)) | Explore, filter, and **download** importable datasets |

```text
Canon JSON
    ├── Hugo site  → browse and read (no export plumbing)
    └── Blazor app → explore / filter / download (CSV today)
```

### Hugo (public reference site)

**Purpose:** Let anyone discover and read the full fictitious universe and demo data without running code.

**Responsibilities:**

- Showcase all demo data a human would want to read — personas, organisations, timeline, career history, portfolio, deals, and cases
- Present narrative, relationships, and copy in a browsable, linkable form
- Omit technical identifiers from reader-facing body copy (no `contactId`, foreign-key slugs, or export column names as primary content)
- Point readers who need importable files to the Blazor app

**Not its job:** CSV download, API keys, preview tables with machine-oriented columns, or acting as a CRM import tool.

See [site/README.md](../site/README.md) for build and deploy.

### Blazor (interactive export app)

**Purpose:** Help someone understand what demo data exists, narrow what they want, and download it in a format they can import elsewhere.

**Responsibilities:**

- Interactive browse and preview of CRM datasets (contacts, accounts, deals, cases)
- Filtering and faceting on dataset pages (preview and CSV download share the same server-side filter)
- Per-contact detail for exploration ahead of export
- Download CSV (and future import formats) with technical identifiers and cross-references intact for external systems

**Not its job:** Replace the public site as the primary readable reference for the universe. Technical IDs belong here and in exports, not on Hugo reader pages.

Run via `dotnet run --project src/Turpinverse.AppHost`. Main routes: `/`, `/contacts`, `/accounts`, `/deals`, `/cases`, and `/contacts/{id}` for contact detail.

## Dual publication on person pages

Career and portfolio content (experience, education, projects, achievements) may appear on **both**:

- Hugo persona pages — as part of the browsable showcase
- Blazor contact detail — as part of interactive exploration before export

Same underlying records, different channel jobs. This is not two competing showcase products; Hugo is for reading, Blazor is for working with data you might export.

## What “all human-readable demo data” means

**On Hugo today:**

- Personas (biographies, relationships, aliases)
- Organisations (profiles, org charts)
- Timeline events
- Career history and portfolio on persona pages (experience, education, projects, achievements)
- Deals and cases (dedicated indexes and detail pages with named bidirectional links)
- Articles and galleries (team journal list/detail pages, persona article lists, captioned gallery with lightbox)

**Blazor / CSV only today:**

- Tabular deals and cases datasets with machine-oriented identifier columns in previews and exports (by design for the export channel)

## Known gaps

These are product facts, not blockers for the channel split above:

| Gap | Status |
|-----|--------|
| Hugo pages for deals and cases | **Shipped** — generated from canon with nav and home links |
| Technical IDs in Hugo body copy | **Addressed** — display-name partials and fallbacks; join keys remain in front matter / data JSON only |
| Blazor filtering / faceting | **Shipped** — dataset pages filter preview and CSV download via shared `ExportFilter` |
| Hugo completeness vs canon | Generator and layouts must grow as new human-readable entity types are added — articles and galleries **shipped** on Hugo |

## Related docs

- [README.md](../README.md) — repository overview and quickstart
- [site/README.md](../site/README.md) — Hugo build, preview, and deploy
- [tech-stack.md](./tech-stack.md) — technology summary
- [career-portfolio-mapping.md](./career-portfolio-mapping.md) — projecting career/portfolio entities to a typical personal-site layout
