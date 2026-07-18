# Research: Turpinverse Universe & Demo Data

**Date**: 2026-07-18 | **Plan**: [plan.md](./plan.md)

## 1. Canon Data Single Source of Truth

**Decision**: Store canon as versioned JSON files in `src/Turpinverse.Data/canon/`, with
JSON Schema validation in `specs/001-turpinverse-universe/contracts/canon-schema.json`.

**Rationale**: JSON is natively consumed by .NET (`System.Text.Json`), easily validated,
diff-friendly in Git, and convertible to Hugo data files or markdown via a build tool.
YAML was considered but JSON schema tooling is stronger in .NET ecosystems.

**Alternatives considered**:
- *Markdown-only canon*: Human-readable but hard to validate cross-references programmatically (SC-002).
- *SQLite database*: Over-engineered for static demo data; adds migration complexity.
- *Separate canon per artifact*: Violates consistency requirements (FR-006, FR-013).

## 2. Hugo + GitHub Pages from `docs/`

**Decision**: Hugo source in `site/` with `publishDir = "../docs"` in `hugo.toml`. GitHub
Actions workflow builds Hugo on push to `main` and commits/deploys output to `docs/`.

**Rationale**: GitHub Pages "Deploy from branch /docs folder" requires static HTML at
`docs/` root. Keeping Hugo source in `site/` prevents mixing templates with published
output. The `peaceiris/actions-hugo` + `peaceiris/actions-gh-pages` pattern is well-tested
for OSS repos.

**Alternatives considered**:
- *Hugo source inside `docs/`*: Conflicts with publishDir output overwriting source.
- *gh-pages branch only*: Works but user explicitly requested `docs/` folder serving.
- *Jekyll (GitHub default)*: User specified Hugo.

## 3. Hugo Content Generation from Canon

**Decision**: A `tools/generate-hugo-content` console tool reads canon JSON and emits:
- Markdown pages under `site/content/personas/`, `site/content/organisations/`
- Data files under `site/data/` for org-chart and timeline partials
- Cross-link front matter (`aliases`, `related`) for hyperlinked navigation

**Rationale**: Maintains FR-006 (no records outside canon) and enables org-chart layouts
using Hugo's `data/` directory and custom layouts without hand-editing 15+ pages.

**Alternatives considered**:
- *Hugo modules pulling remote JSON*: Adds complexity; local build tool is simpler.
- *Manual Hugo content*: Doesn't scale; risks drift from export data.

## 4. .NET 10 Blazor Server for Export UI

**Decision**: Blazor Server interactive app in `Turpinverse.Web` with download endpoints
for CSV export. UI built with Tailwind CSS, Lucide icons via inline SVG or a thin Blazor
wrapper, and Chart.js via JS interop for pipeline visualisation.

**Rationale**: User specified Blazor Server. Server-side rendering suits a tooling/demo
data app with no offline requirement. Interactive export forms and preview tables work
well with Blazor's component model.

**Alternatives considered**:
- *Blazor WebAssembly*: Heavier initial load; no advantage for a data-export tool.
- *Minimal API + static HTML*: Loses component reusability and user-requested Blazor.
- *Console CLI only*: Doesn't meet demo-presenter UX needs from User Story 2.

## 5. CSV Export Library

**Decision**: Use **CsvHelper** for CSV generation with UTF-8 BOM option for Excel
compatibility.

**Rationale**: Mature, well-tested .NET library; supports class mapping and streaming.
Built-in `System.IO` CSV writing lacks robust escaping and header conventions.

**Alternatives considered**:
- *Manual string building*: Error-prone for fields containing commas and quotes.
- *FastCSV / other libs*: CsvHelper has broader adoption and documentation.

## 6. .NET Aspire Orchestration

**Decision**: `Turpinverse.AppHost` references `Turpinverse.Web` as a project resource
with `Turpinverse.ServiceDefaults` providing OpenTelemetry, health checks, and HTTP
resilience patterns.

**Rationale**: User explicitly requested Aspire. Even with one service today, Aspire
provides a standard dashboard for local dev and a clear path to add export microservices
or background workers when new formats arrive.

**Alternatives considered**:
- *Direct `dotnet run`*: Simpler but ignores stakeholder requirement.
- *Docker Compose*: Not requested; Aspire is the .NET-native equivalent.

## 7. Tailwind CSS in Blazor

**Decision**: Tailwind CSS via standalone CLI (`@tailwindcss/cli`) with input at
`src/Turpinverse.Web/Styles/app.css`, output to `wwwroot/css/app.min.css`. Build
integrated into MSBuild `npm/pnpm` target or a `Directory.Build.targets` hook.

**Rationale**: User requested Tailwind. Standalone CLI avoids Node bundler complexity
(webpack/vite) for a server-rendered app with modest CSS needs.

**Alternatives considered**:
- *Bootstrap (default Blazor)*: User specified Tailwind.
- *Tailwind via CDN*: Not suitable for production OSS; no purging.

## 8. Lucide Icons

**Decision**: Use Lucide static SVG icons copied into `wwwroot/icons/` or rendered via a
small `LucideIcon.razor` component that maps icon names to inline SVG paths from the
Lucide package.

**Rationale**: Lucide is lightweight, consistent, and works in both Hugo (static SVG) and
Blazor (component). No React dependency needed.

**Alternatives considered**:
- *Font Awesome*: Heavier; user specified Lucide.
- *MudBlazor icons*: Couples UI to a component library not requested.

## 9. Chart.js Usage

**Decision**: Chart.js via `IJSRuntime` interop in Blazor for:
- Deal pipeline stage distribution (bar chart)
- Dataset summary stats (doughnut chart)

Org charts on the Hugo site use a custom Hugo partial with nested HTML/CSS (or Mermaid
diagrams generated from canon relationships), not Chart.js — Chart.js is ill-suited for
hierarchical org layouts.

**Rationale**: Chart.js excels at statistical charts for the export dashboard. Org charts
are a documentation concern better served by Hugo layouts reading relationship data.

**Alternatives considered**:
- *Chart.js for org chart*: Wrong tool; org charts need hierarchical layout engines.
- *D3.js*: More powerful but heavier; Chart.js sufficient for dashboard stats.

## 10. OSS Repository Hygiene

**Decision**: Include `.editorconfig`, `.gitattributes`, `.gitignore` (`.NET` + `node` +
`Hugo`), `LICENSE` (MIT), `README.md`, `global.json` (pin .NET 10 SDK), and GitHub
Actions CI for build/test on PR and main.

**Rationale**: User requested typical OSS hygiene for a public GitHub repo. MIT license
is standard for sample-data/demo projects.

**Alternatives considered**:
- *Apache 2.0*: MIT is simpler for a demo dataset project.

## 11. Test Strategy

**Decision**:
- **Unit tests** (`Turpinverse.Core.UnitTests`): Canon loader, alias resolution, CSV
  export mapping, cross-reference validation (SC-002).
- **Component tests** (`Turpinverse.Web.UnitTests`): bUnit tests for export page, download
  button, preview table.
- **Integration tests** (`Turpinverse.IntegrationTests`): Full export pipeline from canon
  JSON to CSV bytes; Hugo content generator output structure.

**Rationale**: Constitution Principle III requires failing-first tests for export contracts
and schema validation. bUnit is the standard Blazor testing library.

**Alternatives considered**:
- *Playwright E2E only*: Slower feedback; unit/integration tests catch consistency bugs
  faster (SC-002, SC-005).
