---
title: "Getting Started"
---

# Getting Started with Turpinverse

## Prerequisites

- .NET SDK 10.x
- Node.js 20+ (for Tailwind CSS build)
- Hugo Extended **or** Docker/Podman (see [site README](../../README.md))

## Quick Start

```powershell
dotnet restore
dotnet build
dotnet test --filter "Category=CanonValidation"
dotnet run --project src/Turpinverse.AppHost
```

## Generate Documentation

```powershell
# With Docker/Podman (no Hugo install)
.\scripts\hugo.ps1 build

# Or with Hugo installed locally
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent
cd site && hugo --minify
```

The built site outputs to `docs/` for GitHub Pages deployment. For local preview, run `.\scripts\hugo.ps1 serve` or see [site/README.md](../../README.md).

## Export CRM Data

1. Start the app via Aspire
2. Navigate to `/export`
3. Download contacts, accounts, deals, and cases as CSV

All exports are generated from the canonical JSON dataset in `src/Turpinverse.Data/canon/`.
