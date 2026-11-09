---
title: "ADR-0008: CRM membership export"
status: "Accepted"
date: "2026-09-05"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0008: CRM membership export

## Status

**Accepted**

## Context

Contacts CSV must reflect account membership not one row per person.

## Decision

One CSV row per membership; contactId is person key; optional main contact must be account member.

## Consequences

### Positive

- Matches CRM import patterns
- Explicit join rules

### Negative

- Email repeats on multiple rows

## Alternatives Considered

Rejected fake membership to preserve invalid main contacts.
