---
title: "ADR-0012: CRM activities surface"
status: "Accepted"
date: "2026-09-20"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0012: CRM activities surface

## Status

**Accepted**

## Context

Activities are CRM interactions distinct from universe timeline.

## Decision

~20 curated activities; no CanonEvent FK; Blazor list/export; Hugo embeds timelines on regarding records only.

## Consequences

### Positive

- Clear split timeline vs CRM activity

### Negative

- No Hugo activities index

## Alternatives Considered

Rejected standalone Hugo CRM activities nav item.
