# Product surfaces

Turpinverse's **primary product** is the fictional demo dataset in [`canon/`](../canon/). Hugo and the Blazor export app are **consumers** of that JSON — same as any other project that points at this repository for sample data.

## Who this is for

Anyone who wants to use Turpinverse demo data for their own purposes, including:

- **CRM and business-app demos** — populate pipelines, accounts, contacts, and support cases with plausible sample data
- **Presentations and public-facing documentation** — illustrate workflows without real people's confidential information
- **Other codebases and libraries** — load `canon/*.json` directly (see [canon/README.md](../canon/README.md))
- **Amusement** — enjoy a tongue-in-cheek Dick Turpin universe someone has built end to end

## Bundled consumers

| Consumer | URL / entry | Job |
|----------|-------------|-----|
| **Public reference site (Hugo)** | [turpinverse.uk](https://turpinverse.uk) | Browsable showcase of **human-readable** demo data |
| **Interactive export app (Blazor)** | Run locally via Aspire (see [README](../README.md)) | Explore, filter, and **download** importable CSV |

```text
canon/  (JSON datasets + schema)
    ├── Hugo site   → browse and read (no export plumbing)
    ├── Blazor app  → explore / filter / download (CSV)
    └── your app    → read JSON directly
```

How canon records join (membership, deals, cases, career, articles): [entity-relationships.md](./entity-relationships.md).

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

- Interactive browse and preview of CRM datasets (contacts, accounts, deals, cases, projects)
- Filtering and faceting on dataset pages (preview and CSV download share the same server-side filter)
- Per-contact detail for exploration ahead of export
- Download CSV (and future import formats) with technical identifiers and cross-references intact for external systems

**Not its job:** Replace the public site as the primary readable reference for the universe. Technical IDs belong here and in exports, not on Hugo reader pages.

Run via `dotnet run --project src/Turpinverse.AppHost`. Main routes: `/`, `/contacts`, `/accounts`, `/deals`, `/cases`, `/projects`, `/activities`, `/products`, `/quotes`, `/sales-orders`, `/invoices`, `/bills`, `/payments`, `/credit-notes`, `/leads`, and `/contacts/{id}` for contact detail.

## Navigation

Both consumers group destinations the same way so the header/sidebar does not grow one flat item per entity type.

| Group | Hugo (`menu.main`) | Blazor sidebar |
|-------|-------------------|----------------|
| Directory | Personas, Organisations (top-level) | Contacts, Accounts (top-level) |
| CRM | Dropdown: Leads, Deals, Cases, Projects | Section label **CRM** (includes **Activities** list/export) |
| Finance | Dropdown: Products (more document types as they ship) | Section label **Finance** |
| Universe | Dropdown: Articles, Gallery, Timeline | *(not published on Blazor)* |

Home quick links on Hugo follow the same groups. Finance document types nest under **Finance** in Hugo and Blazor. **Activities** ship on Blazor (`/activities` + CSV) under **CRM**; on Hugo they appear as **embedded timelines** on deal, case, contact, lead, and commercial document detail pages — not a standalone `/activities/` index or CRM nav item.

## Dual publication on person pages

Career and portfolio content (experience, education, projects, achievements) and
**professional profile extras** (intro, about, skills, contact, socials) may appear on **both**:

- Hugo persona pages — as part of the browsable showcase
- Blazor contact detail — as part of interactive exploration before export

Same underlying records, different channel jobs. This is not two competing showcase products; Hugo is for reading, Blazor is for working with data you might export.

Professional extras publish in sandwich order on both channels: intro header → about → skills → career/portfolio → contact → socials. Empty types are omitted; competing title, biography, and header-email lines are suppressed when extras exist.

## What “all human-readable demo data” means

**On Hugo today:**

- Personas (biographies, relationships, aliases)
- Organisations (profiles, org charts, **registered office** postal layout on every organisation page)
- Persona **mailing addresses** when canon includes one (omitted when absent — no empty heading)
- Timeline events
- Career history and portfolio on persona pages (experience, education, projects, achievements)
- Professional profile extras on persona pages (intro, about, skills, contact, socials)
- Deals and cases (dedicated indexes and detail pages with named bidirectional links)
- Product catalogue (index and detail pages; UK VAT rate shown as human-readable name and percentage — no dedicated tax-rate pages)
- Leads (index and detail pages; converted leads link to persona display names; matched orgs link by trading name)
- Quotes (index and detail pages with nested line items; named links to account, contact, deal, products, and projects)
- Invoices (index and detail pages with nested line items; related payments on detail; named links to account, contact, deal, case, products, projects, and quotes)
- Payments (index and detail pages; named link to related invoice)
- Credit notes (index and detail pages with nested line items; optional link to related invoice)
- Organisation commercial roles on org pages when authored (Customer / Supplier / Partner chips; heading omitted when empty)
- Articles and galleries (team journal list/detail pages, persona article lists, captioned gallery with lightbox)

**Blazor / CSV today:**

- Tabular deals, cases, projects, products, quotes, invoices, payments, credit notes, and leads datasets with machine-oriented identifier columns in previews and exports (by design for the export channel)
- Quote, invoice, and credit note CSV exports are **header-only**; nested line items remain in canon JSON and on Hugo detail pages (flattened document-lines CSV deferred)
- Payment CSV includes denormalised `accountId` from the target invoice for filtering
- Account and contact CSV exports include flattened UK postal columns (`registeredOffice*` / `mailing*`)
- Account CSV includes semicolon-separated `roles` (customer / supplier / partner hats)
- Blazor `/accounts` preview MAY show `registeredOfficeTown`; `/contacts` preview MAY show `mailingTown`
- Blazor `/contacts/{id}` shows a mailing address section when the persona has one (outside professional-extras contact)

## Known gaps

These are product facts, not blockers for the channel split above:

| Gap | Status |
|-----|--------|
| Hugo pages for deals and cases | **Shipped** — generated from canon with nav and home links |
| Product catalogue and account `roles[]` | **Shipped** — Hugo product pages; Blazor `/products` CSV; accounts export includes roles |
| Leads | **Shipped** — Hugo lead pages; Blazor `/leads` CSV with status/source filters |
| Quotes | **Shipped** — Hugo quote pages with line items; Blazor `/quotes` CSV (headers only) with status/accountId filters |
| Invoices, payments, credit notes | **Shipped** — Hugo AR document pages; Blazor `/invoices`, `/payments`, `/credit-notes` CSV (invoice/credit note headers only) |
| Sales orders, bills | **Shipped** — Hugo fulfilment and AP pages; Blazor `/sales-orders`, `/bills` CSV (headers only) |
| Bills, sales orders | **Shipped** — Hugo indexes/details; Blazor `/sales-orders`, `/bills` CSV (headers only) with status/accountId filters |
| Activities | **Planned** — join graph frozen (#45); Hugo/Blazor/CSV land with #38 |
| Technical IDs in Hugo body copy | **Addressed** — display-name partials and fallbacks; join keys remain in front matter / data JSON only |
| Blazor filtering / faceting | **Shipped** — dataset pages filter preview and CSV download via shared `ExportFilter` |
| Hugo completeness vs canon | Generator and layouts must grow as new human-readable entity types are added — articles and galleries **shipped** on Hugo |

## Related docs

- [README.md](../README.md) — repository overview and quickstart
- [canon/README.md](../canon/README.md) — using the datasets from another project
- [universe-voice.md](./universe-voice.md) — tone and copy conventions
- [engineering.md](./engineering.md) — how this repo is maintained
- [decision-log.md](./decision-log.md) — why the model looks this way
- [site/README.md](../site/README.md) — Hugo build, preview, and deploy
- [tech-stack.md](./tech-stack.md) — technology summary
- [career-portfolio-mapping.md](./career-portfolio-mapping.md) — projecting career/portfolio entities to a typical personal-site layout
