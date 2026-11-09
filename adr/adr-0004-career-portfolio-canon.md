---
title: "ADR-0004: Career and portfolio as generic canon"
status: "Accepted"
date: "2026-08-10"
authors: "Turpinverse maintainers"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-0004: Career and portfolio as generic canon

## Status

**Accepted**

## Context

Career data should not fork per Hugo theme.

## Decision

Experience, education, projects, and achievements are first-class JSON with mapping docs for Hugo/Blazor.

## Consequences

### Positive

- Portable career model
- One schema for all surfaces

### Negative

- More entities to validate

## Alternatives Considered

Rejected Hugo-only markdown and per-theme schema forks.
