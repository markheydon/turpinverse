# Turpinverse BlazorBlueprint integration

Ground truth in the repo; prefer reading these files over memory.

**Baseline:** BlazorBlueprint **Scenario B** (no app Tailwind build): `ThemePresets.Standard` in `Program.cs`, package `blazorblueprint.css`, and `theme-init.js`. Pages use **`Bb*`** components; library `bb:` utilities are acceptable for layout when no styled wrapper exists.

## Packages (central versions)

`Directory.Packages.props`:

- `BlazorBlueprint.Components` (styled; pulls in Primitives transitively)
- `BlazorBlueprint.Icons.Lucide` (`LucideIcon` from `BlazorBlueprint.Icons.Lucide.Components`)

Do not add icon packs or chart libraries unless the user explicitly requests package changes.

## Service registration

`src/Turpinverse.Web/Program.cs`:

- `builder.Services.AddBlazorBlueprintComponents(configureTheme: ...)` once with `DefaultPreset = ThemePresets.Standard`, `PersistToLocalStorage`, `DetectSystemPreference`.
- Blazor Web App uses Interactive Server (`AddInteractiveServerComponents` / `AddInteractiveServerRenderMode`).

## Static assets (`Components/App.razor`)

Order:

1. Favicon
2. `_content/BlazorBlueprint.Components/blazorblueprint.css`
3. `_content/BlazorBlueprint.Components/css/themes.css`
4. Blocking `_content/BlazorBlueprint.Components/js/theme-init.js` in `<head>` after stylesheets

No `wwwroot` CSS and no Tailwind build (unlike the sample `dotnet new blazorblueprint` template, which adds `styles/themes/default.css` and built `css/app.css`). Icons on `ExportDatasets` use Lucide names directly.

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
| Raw download control | `BbButton` with Lucide download icon |
| Facet `<select>` | `BbFormFieldNativeSelect` |
| Raw preview table | `BbDataTable` via `ExportDataTable.razor` |
| Page title blocks | `BbSectionHeader` |

## Spec alignment

[docs/product-surfaces.md](../../../docs/product-surfaces.md): Blazor is the CSV export surface only; Hugo styling is unchanged.
