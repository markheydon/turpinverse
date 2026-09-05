# Data Model: Turpinverse Universe & Demo Data

**Date**: 2026-07-19 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Overview

Canon entities are authored as JSON in `src/Turpinverse.Data/canon/`. CRM export
entities are **derived** at runtime from canon — they are not separately authored.
Validation rules enforce referential integrity across all relationships.

## Canon Entities

### Persona

A character in the Turpinverse universe.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique; kebab-case; stable for FK integrity; e.g. `dick-turpin` |
| `displayName` | string | ✅ | Modern user-facing name; max 100 chars; e.g. `Richard Turpin` not `Dick Turpin` |
| `historicalName` | string | ✅ | Legendary or historical inspiration name |
| `aliases` | string[] | ❌ | Informal or assumed identities; each alias unique across all personas (via AliasMap) |
| `title` | string | ✅ | CRM-suitable job title with wordplay; max 80 chars |
| `biography` | string | ✅ | Markdown; lead with witty hook; mark extensions with `> *Legend basis:*` or `> *Fictional extension:*` |
| `historicalAnchor` | string | ✅ | Legend inspiration note (e.g. "Turpin legend — Essex Gang leadership"); not a Wikipedia citation |
| `isFictionalExtension` | boolean | ✅ | `true` if persona is wholly invented |
| `temperament` | string | ❌ | Short tone note for humour guidelines |
| `organisationIds` | string[] | ✅ | Must reference existing Organisation.id |
| `birthYear` | int | ❌ | 1700–1800 range if present |
| `deathYear` | int | ❌ | Must be ≥ birthYear if both present |
| `status` | enum | ✅ | `active`, `deceased`, `legend`, `archived` |
| `email` | string | ✅ | Format: `{slug}@turpinverse.uk` |
| `phone` | string | ❌ | Fictional period-appropriate number |
| `notes` | string | ❌ | CRM notes field; tongue-in-cheek but professional |

**Relationships**: Many-to-many with Organisation via `organisationIds`.

### Organisation

A group, business, or institution reframed for demo use.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique; kebab-case |
| `tradingName` | string | ✅ | Professional-sounding; max 80 chars (short display name) |
| `legalName` | string | ❌ | Longer humorous full name |
| `description` | string | ✅ | Max 500 chars |
| `industry` | string | ✅ | e.g. `Hospitality`, `Equine Trade`, `Legal Services` |
| `historicalAnchor` | string | ✅ | Legend or folklore inspiration note |
| `parentOrganisationId` | string | ❌ | Must reference existing Organisation.id; no cycles |
| `memberPersonaIds` | string[] | ✅ | Must reference existing Persona.id |
| `foundedYear` | int | ❌ | 1700–1800 |
| `status` | enum | ✅ | `active`, `dissolved`, `legend` |
| `website` | string | ❌ | Format: `https://{slug}.turpinverse.uk` |

**Relationships**: Self-referential parent hierarchy; many-to-many with Persona.

### CanonEvent

A dated timeline entry.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `id` | string (slug) | ✅ | Unique |
| `date` | string (ISO date) | ✅ | `YYYY-MM-DD` or `YYYY-MM` for approximate |
| `title` | string | ✅ | Max 120 chars |
| `description` | string | ✅ | Markdown |
| `category` | enum | ✅ | `historical`, `legend`, `fictional` |
| `personaIds` | string[] | ❌ | Valid Persona references |
| `organisationIds` | string[] | ❌ | Valid Organisation references |
| `location` | string | ❌ | Period place name |

### AliasMap

Maps alternate identities to canonical personas.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `alias` | string | ✅ | Unique across all aliases |
| `personaId` | string | ✅ | Valid Persona.id |
| `context` | string | ❌ | e.g. `"Assumed identity, 1737–1738"` |

### ToneGuidelines

Humour and professionalism rules (single document, not per-entity).

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `version` | string | ✅ | Semver |
| `principles` | string[] | ✅ | Min 3 entries |
| `examples` | object[] | ✅ | `{ good: string, bad: string, reason: string }` |
| `forbiddenPatterns` | string[] | ✅ | e.g. profanity, slapstick descriptors |

### Canon (root document)

Wrapper loaded at startup.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `version` | string | ✅ | Semver for canon dataset |
| `personas` | Persona[] | ✅ | Min 15 entries (FR-003) |
| `organisations` | Organisation[] | ✅ | Min 8 entries (FR-004) |
| `events` | CanonEvent[] | ✅ | Min 10 entries |
| `aliases` | AliasMap[] | ✅ | Min 1 entry (John Palmer → dick-turpin) |
| `toneGuidelines` | ToneGuidelines | ✅ | FR-011 |

## Derived CRM Entities (Export)

Generated at export time from canon. Not stored separately.

### Contact (derived from Persona)

| Field | Source | Notes |
|-------|--------|-------|
| `contactId` | `persona.id` | |
| `firstName` / `lastName` | Split from `displayName` | |
| `title` | `persona.title` | |
| `email` | `persona.email` | |
| `phone` | `persona.phone` | |
| `accountId` | Primary `organisationIds[0]` | |
| `status` | `persona.status` | |
| `notes` | `persona.notes` | |

### Account (derived from Organisation)

| Field | Source | Notes |
|-------|--------|-------|
| `accountId` | `organisation.id` | |
| `accountName` | `organisation.tradingName` | Short display name (FR edge case) |
| `legalName` | `organisation.legalName` | Full humorous name |
| `industry` | `organisation.industry` | |
| `parentAccountId` | `organisation.parentOrganisationId` | |
| `description` | `organisation.description` | |
| `website` | `organisation.website` | |

### Deal (generated from canon relationships)

| Field | Type | Validation |
|-------|------|------------|
| `dealId` | string | `deal-{n}` sequential |
| `dealName` | string | Commerce-themed; references canon entities |
| `accountId` | string | Valid Account |
| `contactId` | string | Valid Contact |
| `stage` | enum | `Prospecting`, `Qualification`, `Proposal`, `Negotiation`, `Closed Won`, `Closed Lost` |
| `amount` | decimal | GBP; positive |
| `closeDate` | date | ISO 8601 |
| `description` | string | Turpinverse scenario in business language |

**Generation rule**: Deals are authored in `src/Turpinverse.Data/canon/deals.json` as
canon extensions (20+ entries per FR-014), each referencing valid persona and
organisation IDs.

### Case (generated from canon events)

| Field | Type | Validation |
|-------|------|------------|
| `caseId` | string | `case-{n}` sequential |
| `subject` | string | Max 120 chars |
| `description` | string | Professional ticket language |
| `status` | enum | `New`, `In Progress`, `On Hold`, `Resolved`, `Closed` |
| `priority` | enum | `Low`, `Medium`, `High`, `Critical` |
| `contactId` | string | Valid Contact |
| `accountId` | string | Valid Account |
| `relatedEventId` | string | Optional CanonEvent.id |

**Generation rule**: Cases authored in `src/Turpinverse.Data/canon/cases.json` (15+
entries per FR-014).

## Entity Relationship Diagram

Target CRM-shaped join graph (including career, articles, and export names): [docs/entity-relationships.md](../../docs/entity-relationships.md). This 001 data model documents the **shipped** field tables and validation codes; when the join graph and live schema diverge, the docs page is the intended target and a separate correction spec implements the change.

## State Transitions

### Persona.status

```text
active ──► deceased    (historical death)
active ──► legend      (folklore figure, e.g. Black Bess tales)
active ──► archived    (no longer in active CRM demos)
```

### Deal.stage

```text
Prospecting → Qualification → Proposal → Negotiation → Closed Won
                                                      → Closed Lost
```

### Case.status

```text
New → In Progress → Resolved → Closed
                 → On Hold ──► In Progress
```

## Validation Rules (Cross-Entity)

| Rule ID | Description | Spec Reference |
|---------|-------------|----------------|
| VR-001 | Every `persona.organisationIds` entry must exist in `organisations` | FR-013 |
| VR-002 | Every `organisation.memberPersonaIds` entry must exist in `personas` | FR-013 |
| VR-003 | Membership is bidirectional: if persona lists org, org must list persona | FR-013 |
| VR-004 | No alias may map to more than one persona | FR-012 |
| VR-005 | `deals.json` entries must reference valid contact and account IDs | FR-006 |
| VR-006 | `cases.json` entries must reference valid contact and account IDs | FR-006 |
| VR-007 | Deceased personas must not be primary owner of active deals | Edge case |
| VR-008 | `legend` category events must not appear in factual timeline without label | Edge case |
| VR-009 | Minimum dataset volumes: 25 contacts, 10 accounts, 20 deals, 15 cases | FR-014, SC-004 |
| VR-010 | No contradictory dates for same persona across events | SC-005 |

## File Layout

```text
src/Turpinverse.Data/canon/
├── canon.json              # Root wrapper (version, tone guidelines)
├── personas.json           # Persona[]
├── organisations.json      # Organisation[]
├── events.json             # CanonEvent[]
├── aliases.json            # AliasMap[]
├── deals.json              # Deal[] (canon-authored CRM data)
└── cases.json              # Case[] (canon-authored CRM data)
```
