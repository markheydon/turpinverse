# Theme presets

Turpinverse loads **`../theme.css`** from `App.razor`. That file `@import`s one preset from this folder.

## Active preset

Edit `../theme.css` and change the `@import` line, for example:

```css
@import url('./presets/forest.css');
```

## Add another official preset (Ocean, Midnight, …)

1. Copy [Theming](https://blazorblueprintui.com/docs/theming) preset CSS, or export from the [Theme Playground](https://blazorblueprintui.com/docs/playground).
2. Merge into a **full** file: use `default.css` here as the token checklist (alerts, `destructive-foreground`, sidebar, charts).
3. Save as `presets/{name}.css` and point `theme.css` at it.
4. After a big palette change, clear browser `localStorage` key `bb-theme` if `BbThemeSwitcher` overrides look wrong.

`BbThemeSwitcher` in the app header still adjusts base grey, primary accent, radius, and dark mode via package `themes.css`.

If buttons stay dark grey after a preset change, clear `localStorage` key `bb-theme` (stale zinc overrides) and hard-refresh. Preset CSS is loaded **after** `themes.css` so Forest green wins until you pick a custom primary in the switcher.
