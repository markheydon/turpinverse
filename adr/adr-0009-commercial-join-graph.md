---
title: "ADR-0009: Commercial join graph"
status: "Accepted"
date: "2026-09-10"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0009: Commercial join graph

## Status

**Accepted**

## Context

Commercial entities need a frozen join graph for stories #33-38.

## Decision

Organisation.roles array; invoice accountId; line-level product/project links; payment XOR invoice/bill.

## Consequences

### Positive

- Supports multi-hat orgs and lump-sum lines

### Negative

- Complex line-level graph

## Alternatives Considered

Rejected single role field and header-only project on invoices.
