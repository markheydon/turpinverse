---
title: "Getting Started"
---

> **Work in progress** — Turpinverse is under active development. The canon, documentation site, and export tooling are all evolving. Expect gaps, rough edges, and the occasional anachronism dressed up as corporate jargon.

# Getting Started with Turpinverse

Turpinverse is an open-source project that provides a fictional universe (documented on this site) and tooling to export **CRM-style sample data** from it — contacts, accounts, deals, and cases suitable for product demos.

All data is generated from a single canon dataset in `src/Turpinverse.Data/canon/`. This site is the human-readable view of that canon; the Blazor app is the export interface.

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

The Aspire AppHost starts the Blazor web app. Navigate to `/export` for the CRM export dashboard.

## Export CRM Data

1. Start the app via Aspire (`dotnet run --project src/Turpinverse.AppHost`)
2. Open the export dashboard at `/export`
3. Download contacts, accounts, deals, and cases as CSV files

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
| `src/Turpinverse.Web/` | Blazor Server app with CSV export UI |
| `site/` | Hugo documentation site (this site) |
| `specs/001-turpinverse-universe/` | Feature specification and design artifacts |
