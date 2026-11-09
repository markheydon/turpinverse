---
title: "ADR-0014: Aspire Project v2 resource"
status: "Accepted"
date: "2026-11-09"
authors: "Turpinverse maintainers"
tags: ["aspire", "orchestration"]
supersedes: ""
superseded_by: ""
---

# ADR-0014: Aspire Project v2 resource

## Status

**Accepted**

## Context

Turpinverse.AppHost used legacy `AddProject<Projects.Turpinverse_Web>` on Aspire AppHost SDK 13.5.4. Aspire 13.6+ promotes experimental `DotnetProjectResource` via `AddDotnetProject`.

## Decision

Upgrade AppHost SDK to **13.6.4**, add `Aspire.Hosting.Dotnet`, and register the web app with `AddDotnetProject("turpinverse-web", path)` behind narrow `ASPIREDOTNETPROJECT001` pragma suppression.

## Consequences

### Positive

- Aligns with Aspire 13.6 hosting model ahead of CLI and publishing improvements
- Keeps a single web resource with unchanged behaviour

### Negative

- Experimental API diagnostic until Aspire stabilises Project v2
- Preview package pin for `Aspire.Hosting.Dotnet` alongside SDK bundle

## Alternatives Considered

Stay on `AddProject` at 13.5.4 — rejected to complete platform upgrade in one maintenance pass.
