# Data Model: Professional Profile Extras

**Date**: 2026-08-23 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Overview

Professional extras are **authored canon**, not derived CRM rows and not website
configuration. They load from `src/Turpinverse.Data/canon/professional-extras.json`
and hang off `Canon`. Contacts/accounts CSV columns do not change.

Existing **Persona** fields (`displayName`, `title`, `biography`, `email`,
optional `phone`) remain stored as seeds. Career and portfolio collections
remain the only source for jobs, schools, projects, and awards.

## New canon file

| File | Entity | Cardinality |
|------|--------|-------------|
| `professional-extras.json` | Professional extras set | Many rows; **at most one per `personaId`** |

## Entities

### ProfessionalExtras

One person-linked presentation set. Types are optional parts of this set.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `personaId` | string (slug) | ✅ | Existing `Persona.id`; unique among extras rows (VR-036, VR-037) |
| `intro` | IntroCopy | ❌ | Primary subject MUST have this (VR-040) |
| `about` | string | ❌ | Non-empty when present; primary MUST have this (VR-041) |
| `skillsHeading` | string | ❌ | Required when `skills` is present; primary MUST have heading + list (VR-041) |
| `skills` | string[] | ❌ | Names only; unique ignoring letter case; author order is array order (VR-038) |
| `contact` | ContactExtras | ❌ | Primary MUST have this (VR-042) |
| `socials` | ProfessionalSocial[] | ❌ | Unique network names ignoring letter case; author order is array order (VR-039) |

**Uniqueness**: At most one row per `personaId` (trim, case-sensitive id match
as elsewhere in canon). Duplicate `personaId` → VR-037.

**Absent types**: Omit the property (do not store empty strings, empty arrays, or
a second extras row). Publication treats missing/empty as omit-section.

**Forbidden properties**: Must not appear (schema `additionalProperties: false`
+ VR-043), including but not limited to: `brandTitle`, `favicon`, `navigation`,
`sticky`, `sections`, `formEndpoint`, `cms`, `recentPosts`, `experience`,
`education`, `projects`, `achievements`.

**Relationships**: Many-to-one Persona. Does not contain experience, education,
projects, or achievements.

### IntroCopy

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `shortIntro` | string | ✅ | Non-empty; profile header intro line |
| `headline` | string | ✅ | Non-empty; seeded from display name / title where they fit |
| `subtitle` | string | ✅ | Non-empty; MUST NOT invent a conflicting job/employer vs career records |
| `photo` | string | ❌ | Path or URL; placeholders allowed |
| `cta` | FeaturedLink | ❌ | At most one; `url` plus `label` or `name` |

Seeding is editorial: extras MAY refine wording. Completeness does not NLP-check
contradiction; content review covers FR-004 / FR-009. Automated gates cover
presence, links, uniqueness, and email match.

### ContactExtras

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `copy` | string | ✅ | Non-empty short contact copy |
| `email` | string | ✅ | MUST equal that persona’s `Persona.email` (VR-042) |
| `phone` | string | ❌ | Optional; MAY reuse `Persona.phone` |
| `cta` | FeaturedLink | ❌ | Optional name/label plus URL; no form endpoint |

### ProfessionalSocial

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| `network` | string | ✅ | Non-empty; unique on a set when letter case is ignored |
| `url` | string | ✅ | Well-formed URL (absolute `http(s)` or in-universe URL/path consistent with existing FeaturedLink URL rules) |

### FeaturedLink (existing)

Reused for intro and contact CTAs. See [003 data-model](../003-career-portfolio-data/data-model.md).
At most one intro CTA and at most one contact CTA per extras set.

## Canon root additions

`Canon` gains:

| Field | Type | Required |
|-------|------|----------|
| `professionalExtras` | ProfessionalExtras[] | ✅ (array may be empty in schema; volume rules in validator) |

`JsonCanonRepository` reads `professional-extras.json`. `canon.json` version MAY
bump when the dataset ships this collection.

## Volume and field gates (primary subject)

Primary persona id: **`dick-turpin`**.

| Rule | Minimum |
|------|---------|
| Intro | `shortIntro`, `headline`, `subtitle` (photo and CTA optional) |
| About | Non-empty `about` |
| Skills | Non-empty `skillsHeading` and ≥5 distinct skill names (letter case ignored) in array order |
| Contact | Non-empty `copy` and `email` equal to persona email (phone optional) |
| Socials | ≥3 entries with distinct network names (letter case ignored) in array order |

Deputies are not volume-gated. If a deputy has skills or socials, uniqueness
still applies to that set.

## Display rules (not extra stored fields)

Computed/applied by Core presenter + layouts:

| Condition | Header | Longer narrative | Email |
|-----------|--------|------------------|-------|
| Intro present | Intro (short intro, headline, subtitle); do not show `title` as a second header line; display name stays | — | — |
| Intro absent | Existing title MAY remain | — | — |
| About present | — | About; do not show biography as a second block | — |
| About absent | — | Existing biography MAY remain | — |
| Contact present | — | — | Contact block; do not keep header email as a competing line |
| Contact absent | — | — | Existing header email MAY remain |

**Sandwich** when types have content: intro header → about → skills → experience →
education → projects → achievements → contact → socials. Career/portfolio
internal sort stays FR-015 from 003. Skills and socials MUST NOT be sorted
alphabetically.

Empty types: no heading, no empty list.

## Entity relationship diagram

```text
Persona (existing: displayName, title, biography, email, phone)
  │ 1
  │ at most one
  └──────────────► ProfessionalExtras
                        ├── IntroCopy (optional photo, optional FeaturedLink CTA)
                        ├── about
                        ├── skillsHeading + skills[]
                        ├── ContactExtras (email = Persona.email)
                        └── ProfessionalSocial[]

Canon ──embeds──► professional-extras.json
Hugo / Blazor ──present──► person pages (sandwich, omit empty)
CSV Contact/Account ──unchanged──► no new columns
Experience / Education / Project / Achievement ──unchanged──► not copied into extras
```

## State transitions

None. Records are static demo canon.

## Validation rules (completeness)

| Code | Check | Failure mode (spec) |
|------|--------|---------------------|
| VR-036 | `personaId` resolves | Orphan persona link |
| VR-037 | Unique `personaId` among extras rows | Duplicate extras set |
| VR-038 | Skill names unique ignoring letter case when `skills` present | Duplicate skill name |
| VR-039 | Social `network` unique ignoring letter case when `socials` present | Duplicate social network |
| VR-040 | Primary intro minima (`shortIntro`, `headline`, `subtitle`) | Incomplete required fields |
| VR-041 | Primary about + skills heading + ≥5 distinct skills | Incomplete required fields |
| VR-042 | Primary contact copy + email match + ≥3 distinct socials; any contact email matches that persona | Incomplete required fields |
| VR-043 | No website-chrome, form/CMS, or career/portfolio list fields | Website-config fields / duplicate career-portfolio payload |

Schema-level required properties stay in JSON Schema (`contracts/` + 001 runtime
schema). Publication omit-empty is a generator/UI contract, not a VR code.

## Existing entities (no field changes)

- **Persona**: still title, biography, email, phone; pages suppress competing
  display when extras exist.
- **Experience, Education, Project, Achievement**: unchanged; still rendered in
  the sandwich middle.
- **CSV Contact/Account**: unchanged columns.
