# Turpinverse BlazorBlueprint integration

Ground truth in the repo; prefer reading these files over memory.

**Baseline:** BlazorBlueprint **Scenario B** (no app Tailwind build): Turpin-themed preset CSS, package `blazorblueprint.css`, and `theme-init.js`. App shell uses **`Bb*`** plus **`bb:`** utilities from `blazorblueprint.css`. Legacy layout class names in `wwwroot/css/app.css` remain during migration; prefer new `bb:` markup on touched pages.

## Packages (central versions)

`Directory.Packages.props`:

- `BlazorBlueprint.Components` (styled; pulls in Primitives transitively)
- `BlazorBlueprint.Icons.Lucide` (optional; house `LucideIcon` remains on some pages)

Do not add icon packs or chart libraries unless the user explicitly requests package changes.

## Service registration

`src/Turpinverse.Web/Program.cs`:

- `builder.Services.AddBlazorBlueprintComponents(configureTheme: ...)` once (radius **0.65**, `PersistToLocalStorage`, `DetectSystemPreference`).
- Blazor Web App uses Interactive Server (`AddInteractiveServerComponents` / `AddInteractiveServerRenderMode`).

## Static assets (`Components/App.razor`)

Order:

1. Favicon
2. `wwwroot/styles/themes/turpin.css` (Turpin amber/slate palette on OKLCH tokens)
3. `_content/BlazorBlueprint.Components/blazorblueprint.css`
4. Legacy `wwwroot/css/app.css` (shrinking as pages migrate)
5. Blocking `_content/BlazorBlueprint.Components/js/theme-init.js` in `<head>` after stylesheets

Root `<Routes @rendermode="InteractiveServer" />`.

## Layout

`Components/Layout/MainLayout.razor`:

- `BbSidebarProvider`, `AppSidebar`, `BbSidebarInset` (header + content)
- `BbToastProvider`, `BbDialogProvider`, `BbPortalHost`
- `BbDarkModeToggle` in header

## Imports

`Components/_Imports.razor`:

- `BlazorBlueprint.Components`
- `BlazorBlueprint.Primitives`
- `BlazorBlueprint.Primitives.Services`
- `BlazorBlueprint.Icons.Lucide.Components`

## Charts

Use BlazorBlueprint chart components (`BbBarChart`, `BbPieChart`, etc.) on Home and CRM pipeline views. Do not reintroduce Chart.js CDN scripts.

## UI patterns

| Instead of | Use |
|------------|-----|
| `.download-btn` | `BbButton` with download icon |
| Facet `<select>` | `BbNativeSelect` / `BbFormFieldNativeSelect` when refactoring |
| Raw `<table>` in `DataTable.razor` | `BbDataTable` (incremental) |
| Page title blocks | `BbSectionHeader` / `BbTypographyH1` |

## Spec alignment

[docs/product-surfaces.md](../../../docs/product-surfaces.md): Blazor is the CSV export surface only; Hugo styling is unchanged.
