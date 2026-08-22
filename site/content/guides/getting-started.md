---
title: "Getting Started"
---

> **Work in progress** — Turpinverse is under active development. The canon, documentation site, and export tooling are all evolving. Expect gaps, rough edges, and the occasional anachronism dressed up as corporate jargon.

# Getting Started with Turpinverse

Turpinverse is an open-source project that provides a fictional universe and **CRM-style sample data** derived from it — contacts, accounts, deals, and cases suitable for product demos, presentations, and import into external systems.

All data comes from a single canon dataset in `src/Turpinverse.Data/canon/`.

## Two ways to use Turpinverse

| Channel | What it is for |
|---------|----------------|
| **This site (Hugo)** | Browse and read human-readable demo data — personas, organisations, timeline, career and portfolio, deals, and cases. No export plumbing. |
| **Blazor app** | Explore datasets interactively, preview records, and download CSV for import into a CRM or similar tool. |

Run the Blazor app when you need importable files; stay on this site when you want to read the universe.

## Prerequisites

- .NET SDK 10.x
- Node.js 20+ (for Tailwind CSS build in the web app)
- Hugo Extended **or** Docker/Podman (for building this documentation site — see [site README](../../README.md))

Hugo fetches the PaperMod theme via modules on first build; no submodule setup is required.

## Quick Start

```powershell
dotnet restore
dotnet build
dotnet test --filter "Category=CanonValidation"
dotnet run --project src/Turpinverse.AppHost
```

The Aspire AppHost starts the Blazor web app. Open the URL shown in the Aspire dashboard (or the app endpoint) and use:

| Route | Purpose |
|-------|---------|
| `/` | Overview and dataset summary |
| `/contacts` | Contacts preview and CSV download |
| `/accounts` | Accounts preview and CSV download |
| `/deals` | Deals preview and CSV download |
| `/cases` | Cases preview and CSV download |
| `/contacts/{id}` | Contact detail (including career and portfolio where present) |

## Export CRM Data

1. Start the app via Aspire (`dotnet run --project src/Turpinverse.AppHost`)
2. Open `/` or a dataset page (`/contacts`, `/accounts`, `/deals`, `/cases`)
3. Use **Download CSV** on any dataset page

All exports are generated at runtime from the canonical JSON dataset — no separate database required.

## Build This Documentation Site

```powershell
# Generate markdown and data files from canon JSON
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent

# Build with Docker/Podman (no local Hugo install required)
.\scripts\Invoke-HugoSite.ps1 build

# Or with Hugo installed locally
cd site && hugo --minify
```

Output is written to `site/public/` for local preview. Production deploys use GitHub Actions.

```powershell
.\scripts\Invoke-HugoSite.ps1 serve
```

See [site/README.md](../../README.md) for troubleshooting.

## Project Layout

| Path | Purpose |
|------|---------|
| `src/Turpinverse.Data/canon/` | Canon JSON — single source of truth |
| `src/Turpinverse.Web/` | Blazor Server app — explore, preview, and download |
| `site/` | Hugo public reference site (this site) |
| `docs/` | Product and developer documentation |
