---
title: "ADR-0002: Public export API gate"
status: "Accepted"
date: "2026-07-15"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0002: Public export API gate

## Status

**Accepted**

## Context

Demo CSV and validation endpoints must be safe to disable on production deployments.

## Decision

Register /api/export/* and /api/canon/validate only when Export:PublicApiEnabled is true; use DemoExport policy and RFC 7807 errors.

## Consequences

### Positive

- Explicit configuration gate for anonymous demo APIs
- No endpoints registered when disabled

### Negative

- Operators must remember to keep gate false on public hosts

## Alternatives Considered

Rejected AllowAnonymous without documented gate and full OAuth for fictional data.
