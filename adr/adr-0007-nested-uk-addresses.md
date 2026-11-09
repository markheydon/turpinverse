---
title: "ADR-0007: Nested UK addresses"
status: "Accepted"
date: "2026-09-01"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0007: Nested UK addresses

## Status

**Accepted**

## Context

Addresses need a consistent shape for organisations and personas.

## Decision

Shared nested Address on organisations.registeredOffice and optional persona mailing address.

## Consequences

### Positive

- Validates once in schema
- Readable in JSON

### Negative

- No geocoding

## Alternatives Considered

Rejected third addresses.json and flattened address columns.
