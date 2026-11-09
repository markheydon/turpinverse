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

Adopt **BlazorBlueprint.Components** v4 with **ThemePresets.Standard** (via `ThemeService`), `BbSidebar` shell layout, BlazorBlueprint charts for home and pipeline views, Lucide icons from **BlazorBlueprint.Icons.Lucide**, and **no app-level CSS** (only package stylesheets `blazorblueprint.css`, `themes.css`, and `theme-init.js`).

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

Custom Turpin-themed CSS tokens — rejected in favour of Blueprint presets (no legacy colour matching requirement).
