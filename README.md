# Turpinverse

An open-source Dick Turpin universe for CRM demos — fictional personas, organisations,
deals, cases, and timeline data you can use in any project.

## Using the datasets

The **product** is the JSON under [`canon/`](canon/). Start with [canon/README.md](canon/README.md).

```text
canon/
├── personas.json, organisations.json, deals.json, cases.json, …
└── schema/canon-schema.json    # JSON Schema
```

From another repository, point at a clone or submodule:

```text
../turpinverse/canon/personas.json
```

Join rules and export behaviour: [docs/entity-relationships.md](docs/entity-relationships.md).  
Copy and tone: [docs/universe-voice.md](docs/universe-voice.md).  
Validation codes: [docs/validation-rules.md](docs/validation-rules.md).

## Bundled consumers (this repo)

This repository also ships two apps that **consume** the same canon:

| Consumer | Entry | Job |
|----------|-------|-----|
| **Hugo site** | [turpinverse.uk](https://turpinverse.uk) | Human-readable showcase |
| **Blazor export app** | `dotnet run --project src/Turpinverse.AppHost` | Explore, filter, download CSV |

Details: [docs/product-surfaces.md](docs/product-surfaces.md).

## API security note

The Blazor export app exposes `/api/export/*` and `/api/canon/validate` when
`Export:PublicApiEnabled` is `true` (default in Development, `false` in
Production). Endpoints use the `DemoExport` authorization policy and are not
registered when disabled. This is intentional for local demos and Aspire
development. Do not enable the public export API on a network-accessible
deployment without adding authentication or network restrictions.

See [docs/export-api.md](docs/export-api.md) and
[code scanning alert #3](https://github.com/markheydon/turpinverse/security/code-scanning/3).

## Quickstart (build and run)

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

## Documentation

| Doc | Purpose |
|-----|---------|
| [canon/README.md](canon/README.md) | Dataset files and schema |
| [docs/entity-relationships.md](docs/entity-relationships.md) | Join graph |
| [docs/export-api.md](docs/export-api.md) | HTTP/CSV export contract |
| [docs/engineering.md](docs/engineering.md) | How this repo is maintained |
| [docs/decision-log.md](docs/decision-log.md) | Past design decisions |
| [docs/tech-stack.md](docs/tech-stack.md) | Technology summary |

## Scripts

PowerShell scripts in `scripts/` are linted in CI with [PSScriptAnalyzer](https://github.com/PowerShell/PSScriptAnalyzer). Script files must be named `Verb-Noun.ps1` using an approved PowerShell verb.

```powershell
# Lint scripts locally (requires PSScriptAnalyzer module)
pwsh -Command "Install-Module PSScriptAnalyzer -Scope CurrentUser -Force; Invoke-ScriptAnalyzer -Path scripts -Recurse -Settings ./PSScriptAnalyzerSettings.psd1; ./scripts/Test-ScriptConventions.ps1"
```

## Testing standards

- xUnit v3 for all automated .NET tests
- NSubstitute for mocks, stubs, and test doubles
- Built-in xUnit `Assert` methods only
- Keep test dependencies to a minimum

Do not introduce FluentAssertions, AwesomeAssertions, Shouldly, Moq, NUnit, or MSTest
unless explicitly requested.

## License

MIT — see [LICENSE](LICENSE).
