---
title: "ADR-0005: Professional profile extras sandwich"
status: "Accepted"
date: "2026-08-12"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0005: Professional profile extras sandwich

## Status

**Accepted**

## Context

Persona marketing copy should not duplicate core persona fields.

## Decision

professional-extras.json renders sandwich sections on Hugo and Blazor; empty sections omitted.

## Consequences

### Positive

- Consistent persona presentation
- Canon stays normalized

### Negative

- Second file to edit for profile copy

## Alternatives Considered

Rejected stuffing copy into persona root fields.
