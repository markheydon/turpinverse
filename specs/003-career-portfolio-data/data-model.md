# Data Model: Career and Portfolio Demo Data

**Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Overview

Career and portfolio entities are **authored canon**, not derived CRM export rows.
They load from JSON beside existing files in `src/Turpinverse.Data/canon/` and hang
off `Canon`. Contacts/accounts CSV columns do not change.

Existing **Persona** and **Organisation** records are unchanged except that
publication surfaces read the new collections by persona id.

## New canon files

| File | Entity | Cardinality |
|------|--------|-------------|
| `experience.json` | Experience grouping | Many; unique per (`personaId`, organisation key) |
| `education.json` | Education | Many; owned by one persona |
| `projects.json` | Project catalog | Many; shared |
| `achievements.json` | Achievement catalog | Many; shared |

## Entities

### FeaturedLink

Reusable labelled URL for roles, education CTAs, and project links.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `url` | string (URI) | ✅ | Absolute `http(s)` or in-universe path/URL; non-empty |
| `label` | string | ❌ | Display text; at least one of `label`, `name`, `icon` required |
| `name` | string | ❌ | Alias of label (education CTA / project featured CTA) |
| `icon` | string | ❌ | Icon key (e.g. Lucide name); allowed instead of label |
| `tooltip` | string | ❌ | Extra hover/info text |

### Experience (grouping)

A persona’s association with one employer/organisation. Rehire is another **Role**,
not a second grouping.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique among experience rows |
| `personaId` | string | ✅ | Existing `Persona.id` |
| `organisationId` | string | ❌ | If set, existing `Organisation.id` |
| `organisationName` | string | ✅ | Display name even when `organisationId` is set |
| `organisationUrl` | string | ❌ | School/employer site when no/plus canon org |
| `roles` | Role[] | ✅ | Min 1 role |

**Uniqueness**: At most one grouping per persona per organisation key. Key is
`organisationId` when present; otherwise normalized `organisationName`
(trim, case-insensitive). Duplicate keys → VR-025.

**Relationships**: Many-to-one Persona. Optional many-to-one Organisation (no
bidirectional membership requirement).

### Role

A job inside an experience grouping.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `title` | string | ✅ | Role title |
| `start` | string | ✅ | `YYYY-MM` or `YYYY-MM-DD` |
| `end` | string | ❌ | Same formats; omit = current |
| `displayRange` | string | ❌ | Human range if authors want copy distinct from structured dates |
| `description` | string | ✅ | Markdown allowed |
| `extraInfo` | string | ❌ | Tooltip / extra info (FR-004 example) |
| `featuredLinks` | FeaturedLink[] | ❌ | FR-004 requires ≥1 role in the primary set with at least one link |

**Date rule**: If `start` and `end` both parse, `end` ≥ `start` (VR-026). Do not
compare to persona birth/death years.

### Education

A programme owned by one persona.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique among education rows |
| `personaId` | string | ✅ | Existing `Persona.id` |
| `title` | string | ✅ | Degree or programme name |
| `institutionName` | string | ✅ | School display name |
| `institutionUrl` | string | ❌ | |
| `organisationId` | string | ❌ | If set, existing `Organisation.id` |
| `start` | string | ❌ | Same date formats as Role |
| `end` | string | ❌ | Omit = current/in progress |
| `displayRange` | string | ❌ | |
| `grade` | string | ❌ | Grade/GPA display |
| `description` | string | ❌ | Markdown allowed |
| `featuredLink` | FeaturedLink | ❌ | FR-005: ≥1 primary-subject row includes this |

### Project (shared catalog)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique among projects |
| `title` | string | ✅ | |
| `summary` | string | ✅ | |
| `image` | string | ✅ | Path or URL; placeholders allowed |
| `tags` | string[] | ✅ | Min 1 tag/badge |
| `links` | FeaturedLink[] | ✅ | Min 1 (`label` or `icon` plus `url`) |
| `organisationId` | string | ✅ | Existing `Organisation.id` (CRM account) |
| `personaIds` | string[] | ✅ | Min 1; all existing `Persona.id`; unique within the array |
| `featuredCta` | FeaturedLink | ❌ | FR-007: ≥1 primary-linked project has name+URL CTA |
| `featured` | boolean | ❌ | Optional “show on home” style flag |

**Relationships**: Many-to-one Organisation. Many-to-many Persona via `personaIds`.
Membership does not change `organisationId`.

### Achievement (shared catalog)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique among achievements |
| `title` | string | ✅ | |
| `summary` | string | ✅ | |
| `url` | string | ❌ | FR-008: primary set includes ≥1 with and ≥1 without |
| `image` | string | ❌ | Path or URL; placeholders allowed |
| `personaIds` | string[] | ✅ | Min 1; all existing `Persona.id` |

**Relationships**: Many-to-many Persona only (no required organisation).

### Catalog membership

Not a separate stored table: `personaIds` on Project and Achievement. The same
catalog `id` MUST appear for every linked persona (no per-persona copies).

### Consumer mapping note

Documentation only (`docs/career-portfolio-mapping.md`). Not a persisted entity.

## Canon root additions

`Canon` gains:

| Field | Type | Required |
|-------|------|----------|
| `experience` | Experience[] | ✅ (array may be empty in schema; volume rules in validator) |
| `education` | Education[] | ✅ |
| `projects` | Project[] | ✅ |
| `achievements` | Achievement[] | ✅ |

`JsonCanonRepository` reads the four new files. `canon.json` version MAY bump when
the dataset ships these collections.

## Volume and field gates (primary subject)

Primary persona id: **`dick-turpin`**.

| Rule | Minimum |
|------|---------|
| Experience groupings for primary | 3 organisations |
| Education for primary | 2 |
| Projects with primary in `personaIds` | 3 |
| Achievements with primary in `personaIds` | 4 |
| Featured extras | See FR-004, FR-005, FR-007, FR-008, FR-014 |

FR-014: at least one project **or** achievement includes `dick-turpin` **and** another
persona id.

Other personas MAY have zero rows; publication MUST omit empty section types.

## Display ordering (not stored)

Computed by Core presenter (FR-015):

1. **Experience groupings**: by each grouping’s most recent role (`end` missing/current
   first, then latest `end`, then latest `start`).
2. **Roles within a grouping**: most-recent-first (same comparison).
3. **Education**: most-recent-first.
4. **Projects / achievements**: unspecified; stable by `id` or author order is fine.

## Entity relationship diagram

```text
Persona (existing)
  │ 1
  │ owns
  ├──────────────► Experience ──1:N── Role
  │                     │
  │                     └── optional Organisation
  │
  ├──────────────► Education ── optional Organisation
  │
  │ N:N membership
  ├──────────────► Project ── required Organisation (account)
  │
  └──────────────► Achievement

Canon ──embeds──► experience, education, projects, achievements JSON
Hugo / Blazor ──present──► person pages (omit empty types)
CSV Contact/Account ──unchanged──► no new columns
```

## State transitions

None. Records are static demo canon. Open-ended roles are represented by omitted
`end`, not a status enum.

## Validation rules (completeness)

| Code | Check | Failure mode (spec) |
|------|--------|---------------------|
| VR-020 | `personaId` / `personaIds` resolve | Orphan persona link |
| VR-021 | Claimed `organisationId` resolves | False organisation link |
| VR-022 | Every project/achievement has ≥1 persona | Orphan catalog item |
| VR-023 | Every project has `organisationId` | False/missing account |
| VR-024 | Primary volume + required field examples | Incomplete required fields |
| VR-025 | One experience grouping per persona per org key | Duplicate experience grouping |
| VR-026 | Structured `end` ≥ `start` when both present | Incomplete/invalid dates |
| VR-027 | FR-014 shared catalog membership | Incomplete required fields |

Schema-level required properties stay in JSON Schema (`contracts/` + 001 runtime schema).

## Existing entities (no field changes)

- **Persona**: still biography, title, `organisationIds`, life-span years, etc.
- **Organisation**: still trading names used for employer/school links when claimed.
- **Case**: pattern only (`accountId` + `contactId`); cases JSON is not extended.
