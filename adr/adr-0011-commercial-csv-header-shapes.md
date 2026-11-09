---
title: "ADR-0011: Commercial CSV header shapes"
status: "Accepted"
date: "2026-09-15"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0011: Commercial CSV header shapes

## Status

**Accepted**

## Context

v1 exports prioritize importer-friendly headers over flattened lines.

## Decision

Header-only quotes, invoices, and credit notes CSV; nested lines remain in canon and Hugo detail.

## Consequences

### Positive

- Smaller CSV surface for v1

### Negative

- No line-level quote CSV yet

## Alternatives Considered

Rejected flattened document lines in v1.
