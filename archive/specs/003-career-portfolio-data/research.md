# Research: Career and Portfolio Demo Data

**Date**: 2026-08-22 | **Plan**: [plan.md](./plan.md) | **Spec**: [spec.md](./spec.md)

## 1. First-class JSON files, not persona-nested trees

**Decision**: Author career and portfolio as four canon JSON arrays loaded like deals and
cases: `experience.json`, `education.json`, `projects.json`, `achievements.json` under
`src/Turpinverse.Data/canon/`. Wire them onto `Canon` in `JsonCanonRepository`. Do **not**
store them only as fields on `Persona` or as a consumer theme’s parameter tree.

**Rationale**: FR-001 and FR-010 require generic entities independent of any one site
layout. Separate files keep diffs reviewable, match the existing embedded-resource pattern
(`canon/*.json`), and let projects/achievements be a shared catalog without duplicating
payloads on every persona.

**Alternatives considered**:
- *Nest `experience` / `education` on each Persona*: Fine for P1 ownership, but it
  encourages consumer-shaped trees and cannot model a shared project without copies.
- *Single `portfolio.json` blob*: Harder to validate and review; mixed entity types in
  one file.
- *Hugo front matter as source of truth*: Violates FR-001 and would drift from Blazor.

## 2. Shared catalog membership vs copy-per-persona

**Decision**: Projects and achievements are catalog rows with `id` plus `personaIds`
(membership). Experience and education remain owned by exactly one `personaId`. A project
also has exactly one `organisationId` (CRM account), analogous to `Case.AccountId` +
contact membership.

**Rationale**: Clarifications require a shared catalog, CRM-style project links, and
achievements that stay persona-linked only. One identity prevents title drift (US2-6).

**Alternatives considered**:
- *Duplicate project records per persona*: Simpler writes, but FR-014 and US2-6 fail
  when copies diverge.
- *Junction-only files (`project-memberships.json`)*: Extra file for little gain at
  this volume; `personaIds` on the catalog row is enough.

## 3. Completeness lives in `CanonValidator`

**Decision**: Extend `Turpinverse.Core.Validation.CanonValidator` (and JSON Schema in
`canon-schema.json`) so `/api/canon/validate` and `dotnet test --filter Category=CanonValidation`
fail until links, volumes, date sanity, and unique experience groupings hold. New rule
ids start at **VR-020**.

**Rationale**: FR-011 forbids human-only review. Existing completeness already runs in
unit tests and the validate endpoint; a second checker would split the gate.

**Alternatives considered**:
- *Hugo-only lint or a Markdown checklist*: Not automated.
- *Separate `CareerPortfolioValidator` service*: Acceptable later if the class grows;
  start as methods on `CanonValidator` to match deals/cases.

## 4. Publication parity without a new product or theme switch

**Decision**:
- **Public/reference site**: Extend `HugoContentGenerator` to emit career/portfolio into
  `site/data/` and render sections on existing `site/layouts/personas/single.html`. Omit
  a section when that persona has no rows of that type. Do not change PaperMod as the
  site theme.
- **In-product web app**: Add a **contact detail** route on the existing Blazor app
  (`/contacts/{contactId}`), linked from the contacts table. Show the same four section
  types with the same omit-empty rule. Do **not** add CSV columns or a separate
  downstream personal-site / CMS demo.

**Rationale**: FR-012–FR-013 and SC-006 require the same channels that already show
people, not records-only JSON and not a new downstream demo. The contacts page is a
table today (`CrmEntityPage`); a person page with sections does not exist yet, so a
detail route on the same app is the smallest surface that can omit empty blocks.

**Alternatives considered**:
- *JSON dump or validate API only*: Spec explicitly rejected records-only.
- *New profile-demo site*: Out of scope (FR-012).
- *Switch Hugo theme to a résumé theme*: Forbidden (FR-012).

## 5. Reverse-chronological order in one presenter

**Decision**: Implement sort rules (FR-015) once in Core (`CareerPortfolioPresenter` or
equivalent): organisations by most recent role; roles and education most-recent-first;
missing/current end date counts as most recent. Hugo data and Blazor UI both consume
that ordering. Portfolio lists have no required date order.

**Rationale**: Duplicating date comparison in Go templates and Razor risks inconsistent
“current” handling.

**Alternatives considered**:
- *Rely on JSON file order*: Fragile; authors should not be the sort engine.
- *Hugo `sort` only*: Awkward for open-ended ranges.

## 6. Dates: contemporary overlay, structured when possible

**Decision**: Store `start` and optional `end` as ISO year-month (`YYYY-MM`) or full
date (`YYYY-MM-DD`), plus optional `displayRange` for human copy. `end` absent means
current/open. Completeness checks end-before-start **only** when both structured dates
exist. Do **not** compare CV dates to `Persona.birthYear` / `deathYear`.

**Rationale**: Clarification: historical life span is lore. `dick-turpin` is `deceased`
in canon; the CV is a present-day demo overlay (spec session 2026-08-22).

**Alternatives considered**:
- *Display strings only*: Harder to sort and to fail end-before-start.
- *Enforce 18th-century ranges*: Contradicts the spec.

## 7. Organisation links for jobs and schools

**Decision**: Optional `organisationId` on experience groupings and education rows.
When present, it MUST resolve. When absent, `organisationName` (experience) or
`institutionName` (education) plus optional URL is enough. Do not invent school
organisations solely to satisfy a link. Do **not** apply VR-003 bidirectional
membership to career links (a past employer or school need not list the persona as a
current member).

**Rationale**: Matches FR-006 and the “same as jobs” education clarification. Existing
orgs (Essex Gang / Turpin Enterprises, etc.) cover employers; there is no dedicated
school org in canon today.

**Alternatives considered**:
- *Require org ids on every row*: Would force fake institutions.
- *Reuse `persona.organisationIds` as the only experience source*: Cannot express
  nested roles, featured links, or rehire.

## 8. Mapping note location

**Decision**: Add repository-only developer docs at `docs/career-portfolio-mapping.md`
(new `docs/` folder, not Hugo `site/content`). Stub is acceptable. Do not require the
note on turpinverse.uk.

**Rationale**: FR-010 and SC-005. `site/content/guides/` would publish on the public
site.

**Alternatives considered**:
- *Only in `specs/003-.../`*: Spec artifacts are not the developer mapping consumers
  will look for.
- *Public Guides section*: Explicitly not required and would mix spec mapping with
  in-universe docs.

## 9. Schema contract vs runtime schema

**Decision**: Document additive types in this feature’s `contracts/`. Implementation
**must** extend the runtime schema at
`specs/001-turpinverse-universe/contracts/canon-schema.json` (embedded by
`Turpinverse.Core`) so `CanonSchemaValidator` sees the new collections. Keep CRM CSV
schemas unchanged (FR-012).

**Rationale**: One validator, one embedded schema. A parallel schema that is never
embedded would not gate CI.

**Alternatives considered**:
- *Stop using the 001 schema*: Breaks existing validation.
- *Make new collections optional in schema and only test in C#*: Weaker; required
  arrays with feature-specific C# volume rules is the existing deals/cases pattern.

## 10. Primary subject identity

**Decision**: Volume and field-coverage gates apply to persona id `dick-turpin`
(display name Richard Turpin). FR-014 sharing uses at least one other existing
persona id (for example a deputy already in `personas.json`).

**Rationale**: FR-002. No new persona is required.
