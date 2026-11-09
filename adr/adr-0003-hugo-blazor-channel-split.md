---
title: "ADR-0003: Hugo vs Blazor channel split"
status: "Accepted"
date: "2026-08-01"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0003: Hugo vs Blazor channel split

## Status

**Accepted**

## Context

Two bundled consumers share canon but serve different audiences.

## Decision

Hugo showcases human-readable data without export plumbing; Blazor explores, filters, and downloads CSV. Shared export filter returns 409 on zero matches.

## Consequences

### Positive

- Clear channel charter
- Single canon source of truth

### Negative

- Two UIs to keep aligned

## Alternatives Considered

Rejected deals/cases as spreadsheet-only on public site and silent empty downloads.
