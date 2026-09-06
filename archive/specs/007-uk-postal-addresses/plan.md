# Implementation Plan: UK Postal Addresses

**Branch**: `007-uk-postal-addresses` | **Date**: 2026-09-03 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/007-uk-postal-addresses/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

**Channels affected**: **Public reference site (Hugo — showcase / read)** and
**in-product web app (Blazor — explore / filter / download)**.

- **Hugo** shows a **registered office** on every organisation page and a mailing
  address on a persona page **only when canon has one**. Reader-facing copy MUST
  NOT use join keys, export column names (`registeredOfficeAddress1`,
  `mailingTown`, …), or JSON property names as primary identity.
- **Blazor** is the import surface: account and contact CSV flatten the same
  nested address into `registeredOffice*` and `mailing*` columns. Contact detail
  MAY show a mailing address when present. Account list/preview MAY show town.
  **No new account-detail page.**

Shared canon JSON remains the single source of truth. This plan respects
Constitution Principle IX and [`docs/product-surfaces.md`](../../docs/product-surfaces.md).
Postal fields MUST NOT be added to professional-extras contact.

## Summary

Add one nested **Address** value object on organisations (required
`registeredOffice`) and personas (optional `address`) so Turpinverse CRM demo
data looks like a UK system (issue #30). Author offices for all ten organisations
and mailing addresses for a minority of contacts (`dick-turpin`, `mary-brazier`,
`james-smith`). Enforce completeness in `CanonValidator`. Flatten via
`ExportMapper` into existing account/contact CSV. Publish through existing Hugo
organisation/persona layouts and Blazor contact detail / account preview. No
maps, geocoding, billing/shipping split, or extra product surface.

## Technical Context

**Language/Version**: C# / .NET 10; Hugo Extended (existing `site/`)

**Primary Dependencies**: ASP.NET Core Blazor Server, .NET Aspire, JsonSchema.Net,
existing `CanonValidator` / `ToneValidator` / `HugoContentGenerator` /
`JsonCanonRepository` / `ExportMapper` / `ExportCsvColumns`

**Storage**: File-based JSON in `src/Turpinverse.Data/canon/organisations.json`
and `personas.json`; no database; no geo store

**Testing**: xUnit v3, NSubstitute, bUnit, coverlet; `Category=CanonValidation`
plus export column tests, Hugo generator tests, and Blazor contact-detail tests
(failing first per constitution III)

**Target Platform**: Cross-platform .NET 10; Hugo site on GitHub Pages from
existing `site/` pipeline; local Blazor via Aspire

**Project Type**: Multi-artifact OSS monorepo — static documentation site + Blazor
web app (extend existing projects; no new csproj)

**Performance Goals**: Canon load + validate including addresses well under the
existing CSV export budget (2s); Hugo regenerate comparable to current runtime

**Constraints**: English-only invented premises; no real private dwellings; no
geo APIs or coordinates; one address per entity; country United Kingdom this
increment; ToneValidator on address strings; CRM column contract owned by this
feature (merge into live 001 schema, do not treat 001 spec as the feature record)

**Scale/Scope**: 10 organisations (all with registered office); 25 personas
(exactly 3 with mailing address); two channels; additive CSV columns on existing
accounts and contacts datasets only

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Answer **Yes** or **No** for each principle. Any **No** without a Complexity
Tracking justification blocks the gate.

| Principle | Pre-Design | Post-Design | Evidence |
|-----------|------------|-------------|----------|
| I. Spec-Driven Traceability | Yes | Yes | spec.md US1–US3, FR-001–FR-012, SC-001–SC-007; issue #30 |
| II. Incremental Independence | Yes | Yes | P1 all org offices; P2 contact minority; P3 field vocabulary + dual-channel presentation |
| III. Verifiable Testability | Yes | Yes | Failing-first CanonValidation + export/Hugo/Blazor tests in [quickstart.md](./quickstart.md) |
| IV. Separation of Concerns | Yes | Yes | Nested Address VO in Core models; validator; ExportMapper flatten; Hugo layouts; Blazor pages |
| V. Explicit Error Handling | Yes | Yes | Spec failure table; VR-044–VR-051; 422 on `/api/canon/validate` |
| VI. Security by Design | Yes | Yes | Spec security table; fictional premises; no new auth or bulk surface |
| VII. Justified Complexity | Yes | Yes | Single nested Address; no geo APIs; no billing/shipping; no account-detail page |
| VIII. Documentation Contract | Yes | Yes | [contracts/](./contracts/), [data-model.md](./data-model.md), [quickstart.md](./quickstart.md) |
| IX. Human-Facing Channel Charter | Yes | Yes | Plan names Hugo showcase vs Blazor explore/export; IDs off Hugo primary copy |

**Gate result**: PASS

## Project Structure

### Documentation (this feature)

```text
specs/007-uk-postal-addresses/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/
│   ├── address-canon-schema.json
│   ├── crm-export-schema.json
│   ├── completeness.md
│   └── channel-surfaces.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/Turpinverse.Core/Models/Address.cs          # new value object
src/Turpinverse.Core/Models/Organisation.cs     # required RegisteredOffice
src/Turpinverse.Core/Models/Persona.cs          # optional Address
src/Turpinverse.Core/Validation/CanonValidator.cs
src/Turpinverse.Core/Validation/ToneValidator.cs
src/Turpinverse.Core/Export/AccountExport.cs
src/Turpinverse.Core/Export/ContactExport.cs
src/Turpinverse.Core/Export/ExportMapper.cs
src/Turpinverse.Core/Export/ExportCsvColumns.cs
src/Turpinverse.Core/Hugo/HugoContentGenerator.cs

src/Turpinverse.Data/canon/organisations.json   # all ten registeredOffice
src/Turpinverse.Data/canon/personas.json        # three mailing addresses

specs/001-turpinverse-universe/contracts/canon-schema.json      # merge Address
specs/001-turpinverse-universe/contracts/crm-export-schema.json # merge columns (live schema)

src/Turpinverse.Web/Components/Pages/ContactDetail.razor
src/Turpinverse.Web/Components/Pages/Accounts.razor             # optional town preview
src/Turpinverse.Web/Components/Pages/Contacts.razor             # optional town preview

site/layouts/organisations/single.html
site/layouts/personas/single.html
docs/product-surfaces.md
site/content/organisations/*.md                                 # regenerate
site/content/personas/*.md                                      # regenerate
```

**Structure Decision**: Extend the existing Data / Core / Web / Hugo split from
`001-turpinverse-universe`. No new project. Nested `Address` on existing entity
files (not a third canon file). Runtime JSON Schema remains embedded from
`specs/001-turpinverse-universe/contracts/canon-schema.json` and MUST gain
`$defs.Address` plus `registeredOffice` / `address`. Live CSV schema remains
`specs/001-turpinverse-universe/contracts/crm-export-schema.json` but **this
feature owns the address-column contract** in
[contracts/crm-export-schema.json](./contracts/crm-export-schema.json). Do not
rewrite `specs/001-turpinverse-universe/spec.md` as the address feature record.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

No constitution violations. Nested Address is the simplest shape that satisfies
FR-003. Geo APIs, coordinates, billing/shipping, and an account-detail page are
rejected (constitution VII / FR-004 / FR-011).

## Phase 0 / Phase 1 outputs

- [research.md](./research.md) — nested VO, contract ownership, validator, channels
- [data-model.md](./data-model.md) — Address, org/persona, uniqueness, VR codes
- [contracts/address-canon-schema.json](./contracts/address-canon-schema.json)
- [contracts/crm-export-schema.json](./contracts/crm-export-schema.json)
- [contracts/completeness.md](./contracts/completeness.md)
- [contracts/channel-surfaces.md](./contracts/channel-surfaces.md)
- [quickstart.md](./quickstart.md)

## Implementation notes (for `/speckit-tasks`)

1. Write failing validator, schema, export-column, Hugo, and Blazor tests before
   shipping complete default JSON (constitution III).
2. Add `Address` record; `Organisation.RegisteredOffice` required;
   `Persona.Address` optional. JSON names: `address1`, `address2`, `address3`,
   `town`, `region`, `postcode`, `country` (existing camelCase policy).
3. Merge `$defs.Address` into live 001 `canon-schema.json`. Merge account/contact
   address columns into live 001 `crm-export-schema.json` from this feature’s
   contract — keep 007 as the spec/plan record.
4. Author all ten `registeredOffice` objects. Author addresses only on
   `dick-turpin`, `mary-brazier`, and `james-smith`. Distinct doors; at least one
   org and one contact `address3`; country `United Kingdom`.
5. `CanonValidator` VR-044–VR-051; `ToneValidator` scans address strings.
6. `ExportMapper` flattens to `registeredOffice*` / `mailing*`; missing contact
   address → empty strings (columns still present).
7. Hugo generator writes nested address into org/persona front matter; org layout
   always renders registered office; persona layout only when present.
8. Blazor: ContactDetail shows mailing address when present; Accounts preview MAY
   include `registeredOfficeTown`; Contacts preview MAY include `mailingTown`.
   No `/accounts/{id}` page.
9. Update `docs/product-surfaces.md`. Regenerate Hugo content from canon.
10. Do not add postal fields to `ContactExtras`, maps, or a second public site.
