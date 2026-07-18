# Turpinverse

An open-source Dick Turpin universe for CRM demos — canonical characters, organisations,
and timeline data with a Hugo documentation site and Blazor Server CSV export app.

## What is Turpinverse?

Turpinverse reframes the historical Dick Turpin legend as a tongue-in-cheek business
universe. It provides:

- **Canon dataset** — 15+ personas, 8+ organisations, timeline events, and alias maps
  grounded in Wikipedia history with clearly marked fictional extensions
- **Documentation site** — Hugo static site published to GitHub Pages from `docs/`
- **CRM demo data** — Blazor Server app exporting contacts, accounts, deals, and cases
  as CSV files with intact cross-references

## Quickstart

See [specs/001-turpinverse-universe/quickstart.md](specs/001-turpinverse-universe/quickstart.md)
for full validation scenarios.

```powershell
# Restore and build
dotnet restore
dotnet build

# Run via Aspire (Blazor export app + dashboard)
dotnet run --project src/Turpinverse.AppHost

# Run canon validation tests
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"

# Build documentation site (requires Hugo installed)
dotnet run --project tools/generate-hugo-content/Turpinverse.Tools.GenerateHugoContent
cd site && hugo --minify
```

### Hugo site without installing Hugo (Docker / Podman)

On Windows, use Podman Desktop or Docker Desktop — no local Hugo install needed:

```powershell
.\scripts\hugo.ps1 build    # generate content + build to docs/
.\scripts\hugo.ps1 serve    # live preview at http://localhost:1313
.\scripts\hugo.ps1 preview  # build + serve docs/ at http://localhost:8080
```

Use `-Runtime docker` if you prefer Docker over Podman. Full details: [site/README.md](site/README.md).

## Tech Stack

- .NET 10 / C# — Blazor Server, Aspire orchestration
- Hugo Extended — static documentation site
- Tailwind CSS 4.x — Blazor UI styling
- Chart.js — pipeline and summary visualisations
- CsvHelper — CRM CSV export

## License

MIT — see [LICENSE](LICENSE).
