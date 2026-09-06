# Research: Professional Profile Extras

**Date**: 2026-08-23 | **Plan**: [plan.md](./plan.md) | **Spec**: [spec.md](./spec.md)

## 1. First-class extras file, not persona nesting or theme params

**Decision**: Author one extras set per persona as rows in
`src/Turpinverse.Data/canon/professional-extras.json`, loaded onto `Canon` like
experience and articles. Each row is keyed by `personaId`. Do **not** nest intro,
about, skills, contact, or socials on `Persona`. Do **not** store a consumer
theme/CMS parameter tree.

**Rationale**: FR-001 requires generic person presentation data independent of
site chrome. A dedicated file keeps diffs reviewable, matches the embedded-resource
pattern, and makes “at most one set per persona” a uniqueness rule instead of a
hidden nested blob. Nesting on `Persona` would bake presentation copy into the
identity record and invite theme-shaped trees (rejected for career in 003).

**Alternatives considered**:
- *Fields on `Persona`*: Enforces one set naturally, but mixes identity seeds
  (title, biography, email) with profile presentation and makes chrome leakage
  harder to schema-gate.
- *Five files (`intro.json`, `skills.json`, …)*: Extra files for types that the
  spec treats as optional parts of one set.
- *Hugo front matter or PaperMod params as source of truth*: Violates FR-001 and
  would drift from Blazor.

## 2. Completeness lives in `CanonValidator`

**Decision**: Extend `Turpinverse.Core.Validation.CanonValidator` and merge types
into `specs/001-turpinverse-universe/contracts/canon-schema.json`. New rule ids
start at **VR-036** (VR-028–VR-035 are articles/galleries).
`/api/canon/validate` and `dotnet test --filter Category=CanonValidation` fail
until links, uniqueness, primary volumes, and forbidden-shape rules hold.

**Rationale**: FR-010 forbids human-only review. Existing completeness already
runs in unit tests and the validate endpoint.

**Alternatives considered**:
- *Hugo-only lint*: Not automated and misses Blazor.
- *Separate `ProfileExtrasValidator` service*: Start as methods on
  `CanonValidator` (same as career/articles). Split later if the class grows.

## 3. Dual publication on existing person pages (sandwich)

**Decision**:
- **Hugo**: Emit extras into `site/data/` (alongside `site/data/career/`) and
  update `site/layouts/personas/single.html` only. PaperMod stays the theme. No
  new section, product, or résumé theme.
- **Blazor**: Extend existing `/contacts/{contactId}` with the same omit-empty
  types and sandwich order. No CSV columns, no new export dataset.
- **Order (FR-013)**: intro as header; about; skills; existing experience,
  education, projects, achievements (internal order unchanged); contact; socials.
- **Core presenter**: One `ProfessionalExtrasPresenter` (or equivalent) returns
  the extras set and visibility flags (show intro header vs title; show about vs
  biography). Layouts must not re-sort skills or socials.

**Rationale**: FR-012 and Principle IX require the same channels that already
show people. Career already dual-publishes on these pages; extras sit beside
those blocks, not in a second showcase app.

**Alternatives considered**:
- *JSON/validate only*: Spec requires human-facing person pages.
- *New profile-demo site or theme switch*: Forbidden (FR-011).
- *Sort skills/socials alphabetically in the UI*: Forbidden; stored/author order.

## 4. Header and narrative suppression (not field deletion)

**Decision**: Keep `Persona.title` and `Persona.biography` stored. When intro
extras exist, person pages MUST use intro (short intro, headline, subtitle) as
the header and MUST NOT render `title` as a second competing header line.
Display name remains the identity heading. When about extras exist, about is the
longer narrative and biography MUST NOT appear as a second competing block.
Fallbacks: no intro → existing title MAY remain; no about → existing biography
MAY remain.

When contact extras exist, do **not** keep the current header email line as a
second competing contact treatment; email belongs in the contact block. When
contact extras are absent, existing header email MAY remain (same fallback
pattern as title/biography).

**Rationale**: Clarifications: extras replace competing *display*, not the seed
fields. Header email is the same class of duplicate as header title once contact
copy exists.

**Alternatives considered**:
- *Delete title/biography from personas*: Breaks seeding and other consumers.
- *Show both always*: Explicitly rejected by the spec.
- *Keep header email plus contact section*: A competing contact block.

## 5. Forbidden shape: schema `additionalProperties: false` plus validator

**Decision**: Professional extras objects use `additionalProperties: false` and
an explicit allowed property list. `CanonValidator` VR-043 fails if a set
contains website-chrome keys (brand, favicon, navigation, sticky, section-enable
flags, footer recent-posts path/count), form/CMS endpoints, or career/portfolio
list fields (`experience`, `education`, `projects`, `achievements`). Tests inject
those keys via schema validation (and raw JSON fixtures), not by adding them to
the C# model.

**Rationale**: FR-011 / SC-006. Typed models cannot carry unknown fields;
schema + fixture tests catch authoring mistakes before they ship.

**Alternatives considered**:
- *Document-only forbidden list*: Not an automated gate.
- *Allow extra keys and ignore them*: Silent chrome would pass SC-006 falsely.

## 6. Reuse `FeaturedLink` for call-to-actions

**Decision**: Intro CTA and optional contact CTA are the existing `FeaturedLink`
shape (`url` plus `label` or `name`). At most one intro CTA (schema
`maxItems: 1` or a single object). Photo is a string path/URL (placeholder
allowed). Socials are `{ network, url }`, not `FeaturedLink`, so uniqueness can
key on network name.

**Rationale**: Career already has labelled URLs. A second CTA type would be
unjustified complexity.

**Alternatives considered**:
- *Parallel `ctaLabel` / `ctaUrl` scalars*: Works, but duplicates FeaturedLink.
- *Socials as FeaturedLink*: Network uniqueness is then an extra convention on
  `name`/`label`.

## 7. No mapping note in this increment

**Decision**: Do not add `docs/*-mapping.md` for extras. Spec assumptions say
consumer-layout mapping MAY be noted later and is not required to close the
feature. Do update `docs/product-surfaces.md` during implementation so dual
publication inventory includes extras.

**Rationale**: Principle VII. Career mapping existed because FR-010 in 003
required it; 006 does not.

**Alternatives considered**:
- *Stub mapping anyway*: Extra docs with no spec gate.

## 8. Primary subject and deputies

**Decision**: Volume and field-coverage gates apply only to persona id
`dick-turpin`. Other personas MAY omit extras entirely. A deputy who has extras
still uses at most one set; missing types are omitted, not empty parallel
records. Empty arrays for skills or socials count as absent for publication
and fail uniqueness/volume only when present with bad contents or when the
primary is missing required types.

**Rationale**: FR-002, FR-001 cardinality, omit-empty publication.

## 9. Runtime schema merge

**Decision**: Document additive types in this feature’s `contracts/`.
Implementation MUST extend the embedded 001 `canon-schema.json` (`professionalExtras`
required array, possibly empty at schema level; volume in C#). CRM CSV schemas
stay unchanged.

**Rationale**: One validator, one embedded schema (003/005 pattern).
