# Turpinverse

An open-source Dick Turpin universe for CRM demos — canonical characters, organisations,
and timeline data published through a public Hugo reference site and a Blazor interactive
export app.

## What is Turpinverse?

Turpinverse reframes the historical Dick Turpin legend as a tongue-in-cheek business
universe. It provides:

- **Canon dataset** — 15+ personas, 8+ organisations, timeline events, and alias maps
  grounded in Wikipedia history with clearly marked fictional extensions
- **Public reference site (Hugo)** — browsable showcase of human-readable demo data at
  [turpinverse.uk](https://turpinverse.uk); for reading the universe, using sample copy in
  presentations, or just enjoying the fiction
- **Interactive export app (Blazor)** — explore, filter, and download contacts, accounts,
  deals, and cases as CSV for import into CRMs and similar systems

Channel responsibilities are defined in [docs/product-surfaces.md](docs/product-surfaces.md).

## API security note

The Blazor export app exposes `/api/export/*` and `/api/canon/validate` when
`Export:PublicApiEnabled` is `true` (default in Development, `false` in
Production). Endpoints use the `DemoExport` authorization policy and are not
registered when disabled. This is intentional for local demos and Aspire
development. Do not enable the public export API on a network-accessible
deployment without adding authentication or network restrictions.

See [spec security table](specs/001-turpinverse-universe/spec.md#security--access-required-when-feature-handles-sensitive-data-authentication-or-external-exposure)
and [code scanning alert #3](https://github.com/markheydon/turpinverse/security/code-scanning/3).

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
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent
cd site && hugo --minify
```

### Hugo site without installing Hugo (Docker / Podman)

On Windows, use Podman Desktop or Docker Desktop — no local Hugo install needed:

```powershell
.\scripts\Invoke-HugoSite.ps1 build    # generate content + build to site/public/
.\scripts\Invoke-HugoSite.ps1 serve    # live preview at http://localhost:1313
.\scripts\Invoke-HugoSite.ps1 preview  # build + serve site/public/ at http://localhost:8080
```

Use `-Runtime docker` if you prefer Docker over Podman. Full details: [site/README.md](site/README.md).

## Scripts

PowerShell scripts in `scripts/` are linted in CI with [PSScriptAnalyzer](https://github.com/PowerShell/PSScriptAnalyzer). Script files must be named `Verb-Noun.ps1` using an approved PowerShell verb.

```powershell
# Lint scripts locally (requires PSScriptAnalyzer module)
pwsh -Command "Install-Module PSScriptAnalyzer -Scope CurrentUser -Force; Invoke-ScriptAnalyzer -Path scripts -Recurse -Settings ./PSScriptAnalyzerSettings.psd1; ./scripts/Test-ScriptConventions.ps1"
```

## Tech Stack

- .NET 10 / C# — Blazor Server, Aspire orchestration
- Hugo Extended — static documentation site
- Tailwind CSS 4.x — Blazor UI styling
- Chart.js — pipeline and summary visualisations
- CsvHelper — CRM CSV export

## Testing Standards

- xUnit v3 for all automated .NET tests
- NSubstitute for mocks, stubs, and test doubles
- Built-in xUnit `Assert` methods only
- Keep test dependencies to a minimum

Do not introduce FluentAssertions, AwesomeAssertions, Shouldly, Moq, NUnit, or MSTest
unless explicitly requested.

## License

MIT — see [LICENSE](LICENSE).
