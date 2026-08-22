# Quickstart: Channel alignment validation

**Channels**: Public reference site (Hugo) and interactive export app (Blazor).  
**Contracts**: [hugo-showcase.md](./contracts/hugo-showcase.md), [export-filter-api.md](./contracts/export-filter-api.md)  
**Data**: [data-model.md](./data-model.md)

This guide proves the feature end-to-end. It is not an implementation checklist.

## Prerequisites

- .NET SDK 10.x
- Node.js 20+ (Tailwind build for the web app)
- Hugo Extended **or** `scripts/Invoke-HugoSite.ps1` (Docker/Podman)

## 1. Tests first (failing before the work lands)

```bash
dotnet test --filter "FullyQualifiedName~HugoDeal|FullyQualifiedName~HugoCase|FullyQualifiedName~ExportFilter|FullyQualifiedName~CrmEntityPage"
```

**Expected before implementation**: new tests for generated deal/case content, identity rules, filtered CSV, empty-match 409, and Blazor filter UI **fail**.  
**Expected after**: those tests pass together with existing `Category=CsvExport` and `Category=CrossReference`.

## 2. Public site — read the universe (no export app)

```bash
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent
```

Then build/serve the Hugo site (`site/README.md`).

**Expected**:

- Primary nav or home links to Personas, Organisations, Timeline, **Deals**, and **Cases**
- Every canon deal and case has a readable detail page (name/subject, narrative, stage/status, named related people/orgs)
- Persona, organisation, and timeline views show named links to related deals/cases when canon has FKs
- Showcase pages have **no** download/import controls
- Visible labels are display names (or fallback copy), not `contactId`-style tokens
- Site works without Aspire / Blazor running; Getting Started may say downloads require running the app locally

## 3. Export app — explore, filter, download

```bash
dotnet run --project src/Turpinverse.AppHost
```

Open `/contacts`, `/accounts`, `/deals`, `/cases`.

**Expected**:

- Preview table still shows import-oriented columns (IDs allowed)
- Applying a facet (for example deals `stage=Negotiation`) updates the table
- Download CSV contains **only** those rows; unfiltered download is the full file
- Filter that matches nothing: empty-state copy; download blocked or warned — **not** a silent empty or full file
- `/contacts/{id}` still shows career/portfolio where present, facts agreeing with the matching Hugo persona page

## 4. Charter copy check

Skim Getting Started and `docs/product-surfaces.md`: Hugo = read/showcase; Blazor = explore/filter/download. Known gaps for “deals/cases not built” and “IDs in Hugo body copy” should be closed or explicitly owned.
