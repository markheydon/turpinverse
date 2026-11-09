---
title: "ADR-0001: Solution shape and tooling"
status: "Accepted"
date: "2026-07-01"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0001: Solution shape and tooling

## Status

**Accepted**

## Context

Turpinverse needs separated projects for canon, validation, Hugo generation, Blazor export, and local orchestration.

## Decision

Adopt six-project layout under src/, .NET Aspire AppHost, Hugo content generator in Core, file-based canon in repo root.

## Consequences

### Positive

- Six-project separation enables boundary tests without web host
- Aspire supplies local dev dashboard and OTel

### Negative

- More repos to navigate than a monolith

## Alternatives Considered

Rejected monolithic single project and hand-maintained Hugo markdown beside JSON.
