# Turpinverse BlazorBlueprint integration

Ground truth in the repo; prefer reading these files over memory.

**Baseline:** Align with **`dotnet new blazorblueprint`** theming: app `styles/themes/theme.css` (imports a doc preset), package `blazorblueprint.css`, `themes.css` (after your theme, for `BbThemeSwitcher`), and blocking `theme-init.js`. `AddBlazorBlueprintComponents()` with no custom `configureTheme`. Pages use **`Bb*`** components; library `bb:` utilities are acceptable for layout when no styled wrapper exists.

## Packages (central versions)

`Directory.Packages.props`:

- `BlazorBlueprint.Components` (styled; pulls in Primitives transitively)
- `BlazorBlueprint.Icons.Lucide` (`LucideIcon` from `BlazorBlueprint.Icons.Lucide.Components`)

Do not add icon packs or chart libraries unless the user explicitly requests package changes.

## Service registration

`src/Turpinverse.Web/Program.cs`:

- `builder.Services.AddBlazorBlueprintComponents()` once.
- Blazor Web App uses Interactive Server (`AddInteractiveServerComponents` / `AddInteractiveServerRenderMode`).

## Theme files and presets

| Path | Role |
|------|------|
| `wwwroot/styles/themes/theme.css` | Only theme link from `App.razor`; `@import` active preset |
| `wwwroot/styles/themes/presets/forest.css` | Current brand (Forest doc preset) |
| `wwwroot/styles/themes/presets/default.css` | Template baseline / merge checklist for new presets |
| `wwwroot/styles/themes/presets/README.md` | How to swap or add Ocean, Midnight, etc. |

Swap preset: change the `@import` in `theme.css` (not `App.razor`). See [Blazor Blueprint Theming](https://blazorblueprintui.com/docs/theming).

## Static assets (`Components/App.razor`)

Order:

1. Favicon (optional; Turpinverse uses `favicon.png`)
2. `_content/BlazorBlueprint.Components/blazorblueprint.css`
3. `_content/BlazorBlueprint.Components/css/themes.css`
4. `styles/themes/theme.css` **after** `themes.css` so doc presets (Forest) override default `data-base-color` zinc greys; `BbThemeSwitcher` primary picks still apply via `data-primary-color`
5. Blocking `_content/BlazorBlueprint.Components/js/theme-init.js` in `<head>` after stylesheets

No app Tailwind `css/app.css` build; Turpinverse markup uses `bb:` utilities. Icons on export pages use Lucide names directly.

Root `<Routes @rendermode="InteractiveServer" />`.

## Layout

`Components/Layout/MainLayout.razor`:

- `BbSidebarProvider`, `AppSidebar`, `BbSidebarInset` (header + content)
- `BbToastProvider`, `BbDialogProvider`, `BbPortalHost`
- `BbThemeSwitcher` and `BbDarkModeToggle` in header

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
