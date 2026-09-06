# Research: UK Postal Addresses

**Date**: 2026-09-03 | **Plan**: [plan.md](./plan.md) | **Spec**: [spec.md](./spec.md)

## 1. Single nested Address value object on existing entities

**Decision**: Add `Turpinverse.Core.Models.Address` with JSON names `address1`,
`address2`, `address3`, `town`, `region`, `postcode`, `country`. Attach as
`Organisation.registeredOffice` (required) and `Persona.address` (optional).
Author in existing `organisations.json` and `personas.json`. Do **not** add a
third canon file, flatten fields onto Organisation/Persona roots, or nest postal
data under professional extras.

**Rationale**: FR-003 requires the same UK CRM shape for office and mailing
address. Nested object is the simplest reuse. Optional persona address is omit
vs present (FR-002), which maps cleanly to `Address?`. Existing
`JsonCanonRepository` camelCase policy yields the required JSON names without
`JsonPropertyName` attributes.

**Alternatives considered**:
- *Flattened `registeredOfficeAddress1` on Organisation*: Duplicates seven
  fields twice and invites drift between org and persona.
- *Dedicated `addresses.json` keyed by entity id*: Extra file and join for a
  1:1 nested value; worse diffs for “this org’s office”.
- *Postal fields on `ContactExtras`*: Forbidden (FR-012).

## 2. Feature owns CRM export contract; live schema still lives under 001

**Decision**: Document Contact/Account address columns in this feature’s
[contracts/crm-export-schema.json](./contracts/crm-export-schema.json).
Implementation MUST merge those columns into the runtime file
`specs/001-turpinverse-universe/contracts/crm-export-schema.json` (and
`ExportCsvColumns` / `AccountExport` / `ContactExport`). Do **not** rewrite
`specs/001-turpinverse-universe/spec.md` as the address feature. Do **not**
treat 001 as the only contract record for this work.

**Rationale**: User and spec: this is a new specification. 001’s CRM schema is
the embedded/live column list used by export tests, same pattern as merging
additive types into `canon-schema.json` for 003/005/006.

**Alternatives considered**:
- *Only edit 001 schema*: Silent rewrite; 007 would have no export contract.
- *Fork a second CSV schema file at runtime*: Two sources of truth for columns.
- *New export dataset*: Unjustified; accounts and contacts already exist.

## 3. Completeness lives in CanonValidator; tone scans address strings

**Decision**: New rule ids **VR-044–VR-051** on `CanonValidator`. Schema requires
`registeredOffice` on Organisation. C# enforces completeness, minority count,
forbidden persona addresses, distinct org doors, Richard ≠ Turpin Enterprises,
and third-line usage. Extend `ToneValidator.ValidateCanon` to run forbidden
patterns against every non-empty Address string field (same `TONE-001`). Invented
premises / no real private dwelling remains **content review**, not NLP.

**Rationale**: FR-009 forbids human-only gates for completeness. Tone already
covers org descriptions and persona notes; address lines are new public copy.

**Alternatives considered**:
- *Hugo-only lint*: Misses CSV and Blazor.
- *UK postcode regex / geocoder*: Would reject invented codes or pull real
  addresses (constitution VII, FR-004). Non-empty postcode is enough.

## 4. Dual publication: Hugo always office; persona address omit-empty

**Decision**:
- **Hugo**: `HugoContentGenerator` writes nested address YAML into organisation
  front matter always, and persona front matter only when `Persona.Address` is
  set. `site/layouts/organisations/single.html` always renders a “Registered
  office” block. `site/layouts/personas/single.html` renders mailing address
  only inside `with` / presence check. Visible copy is postal layout (lines,
  town, region, postcode, country) — not export prefixes or slugs as names.
- **Blazor**: `ContactDetail.razor` shows mailing address when present (not
  inside `ContactExtras`). `Accounts.razor` MAY add `registeredOfficeTown` to
  preview columns. `Contacts.razor` MAY add `mailingTown`. No account-detail
  route.

**Rationale**: Constitution IX / FR-010 / FR-011. Same canon, different jobs.

**Alternatives considered**:
- *Empty “Mailing address” heading on every persona*: Forbidden (FR-010).
- *New `/accounts/{id}`*: Forbidden (FR-011).
- *Maps / OpenStreetMap / coordinates*: Forbidden (FR-004, constitution VII).

## 5. CSV flatten: columns always present; empty strings when no mailing address

**Decision**: Append seven account columns (`registeredOfficeAddress1` …
`registeredOfficeCountry`) and seven contact columns (`mailingAddress1` …
`mailingCountry`). Mapper copies nested fields; null/absent optional lines and
absent persona address become `""`. Do not copy organisation office onto a
contact. Do not omit columns from the header when values are empty.

**Rationale**: FR-011 and existing CsvHelper `[Name]`/`[Index]` pattern.
Integration tests already assert header == `ExportCsvColumns`.

**Alternatives considered**:
- *Nullable CSV cells / missing columns*: Breaks UK CRM mapping (SC-005).
- *“Same as account” sentinel*: Forbidden (FR-002).

## 6. Authored minority and geography

**Decision**: All ten organisations get a complete registered office. Personas
with address: `dick-turpin`, `mary-brazier`, `james-smith` only. `black-bess`
and `elizabeth-millington` MUST omit address. Parent/subsidiary (e.g.
`essex-gang` / `turpin-enterprises`) MAY share a town; MUST NOT share
`address1`+`postcode`. Richard’s mailing address MUST differ from Turpin
Enterprises’ office on that same door key. At least one organisation and one of
the three contacts populate `address3`. Country `United Kingdom` for all.

**Rationale**: FR-001, FR-005, FR-006, FR-007, FR-008; spec assumptions (10 orgs,
25 personas, three of 25 is the minority).

**Alternatives considered**:
- *Address every persona*: Looks synthetic (US2).
- *Workplace-as-home for inn staff / Black Bess*: Explicitly invalid.

## 7. Optional JSON lines vs empty strings

**Decision**: Required fields (`address1`, `town`, `postcode`, `country`) are
non-whitespace strings. Optional `address2`, `address3`, `region` MAY be omitted
or empty; export still emits empty-string cells. Do not store an empty Address
object on a persona — omit the property.

**Rationale**: Matches spec edge cases and FR-002.

## Unresolved clarifications

None. Technical Context unknowns were resolved by existing code plus the
user-supplied constraints (JSON names, authorship list, export prefixes, Hugo vs
Blazor split, no geo).
