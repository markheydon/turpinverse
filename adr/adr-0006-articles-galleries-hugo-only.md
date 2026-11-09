---
title: "ADR-0006: Articles and galleries Hugo-only"
status: "Accepted"
date: "2026-08-15"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0006: Articles and galleries Hugo-only

## Status

**Accepted**

## Context

Narrative content is not CRM export data.

## Decision

Articles and galleries stay canon records for Hugo only with no Blazor CSV pages.

## Consequences

### Positive

- Keeps export surface CRM/finance focused

### Negative

- Blazor cannot browse articles

## Alternatives Considered

Rejected treating articles as another export type.
