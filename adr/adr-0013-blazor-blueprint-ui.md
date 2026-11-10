---
title: "ADR-0013: Blazor Blueprint UI"
status: "Accepted"
date: "2026-11-09"
authors: "Turpinverse maintainers"
tags: ["blazor", "ui"]
supersedes: ""
superseded_by: ""
---

# ADR-0013: Blazor Blueprint UI

## Status

**Accepted**

## Context

The Blazor export app used Tailwind CSS 4 (npm build), hand-maintained layout CSS, and Chart.js for dashboard charts. Maintaining a separate CSS pipeline and chart interop duplicated patterns already standardised in other repos (BlazorBlueprint v4).

## Decision

Adopt **BlazorBlueprint.Components** v4 with **`dotnet new blazorblueprint`-style theming**: app `styles/themes/theme.css` (doc presets such as Forest), package stylesheets `blazorblueprint.css`, `themes.css`, and `theme-init.js`; `AddBlazorBlueprintComponents()`; `BbThemeSwitcher` and `BbDarkModeToggle`. `BbSidebar` shell layout, BlazorBlueprint charts for home and pipeline views, Lucide icons from **BlazorBlueprint.Icons.Lucide**. No npm Tailwind build in the export app (layout uses `bb:` utilities).

## Consequences

### Positive

- Single UI component vocabulary aligned with other .NET apps
- No npm toolchain required for Blazor builds
- Charts use themed ECharts via Blueprint

### Negative

- Layout spacing on bespoke pages relies on `Bb*` components and library `bb:` utilities where no styled wrapper exists
- bUnit tests register `AddBlazorBlueprintComponents`

## Alternatives Considered

Kept Tailwind-only styling — rejected for toolchain duplication and weaker component consistency.

Custom Turpin-themed CSS tokens — rejected in favour of official Blueprint doc presets and `ThemeService` (no legacy colour matching requirement).

## Revision (2026-11-09)

Supersedes the earlier “no app-level CSS” wording: the export app now ships theme variable files under `wwwroot/styles/themes/` per [Blazor Blueprint Theming](https://blazorblueprintui.com/docs/theming), not bespoke Turpin colour invention.
