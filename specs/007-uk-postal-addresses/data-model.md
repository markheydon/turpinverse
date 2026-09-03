# Data Model: UK Postal Addresses

**Date**: 2026-09-03 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

## Overview

Postal addresses are **authored canon** nested on existing organisation and
persona records. They are not a separate collection, not professional-extras
contact fields, and not geocoded. CSV rows flatten the nested object for CRM
import.

## Entities

### Address (new value object)

One UK-shaped postal address. JSON property names (camelCase): `address1`,
`address2`, `address3`, `town`, `region`, `postcode`, `country`.

| Field | JSON | Type | Required | Validation |
|-------|------|------|----------|------------|
| Address1 | `address1` | string | ✅ | Non-whitespace; premises first line |
| Address2 | `address2` | string? | ❌ | Optional second line |
| Address3 | `address3` | string? | ❌ | Optional third line; some records MUST use it (VR-051) |
| Town | `town` | string | ✅ | Non-whitespace |
| Region | `region` | string? | ❌ | Optional; UK county / ceremonial county when used |
| Postcode | `postcode` | string | ✅ | Non-whitespace; invented; no geocode |
| Country | `country` | string | ✅ | Non-whitespace; this increment **United Kingdom** |

**Forbidden on Address**: coordinates, map URLs, billing/shipping discriminators,
`sameAsOrganisation`, extra address types. Schema `additionalProperties: false`.

**Completeness**: An address is complete iff `address1`, `town`, `postcode`, and
`country` are non-whitespace. Optional fields may be null or empty.

**Door key** (org uniqueness and Richard vs employer): trim + ordinal ignore-case
of `address1` + `"\u001f"` + `postcode`.

### Organisation registered office

| Field | Type | Required |
|-------|------|----------|
| `registeredOffice` | Address | ✅ exactly one |

Every organisation in `organisations.json` MUST include a complete
`registeredOffice`. Distinct door key per organisation (town MAY be shared).

Reader-facing label: **Registered office** (not “HQ” unless narrative also makes
that true).

### Persona mailing address

| Field | Type | Required |
|-------|------|----------|
| `address` | Address? | ❌ omit when none |

When present, MUST be complete. Do not store `{}` or all-empty fields.

**This increment — must have address:**

- `dick-turpin` (door key MUST differ from organisation `turpin-enterprises`)
- `mary-brazier`
- `james-smith`

**This increment — must omit address:**

- `black-bess`
- `elizabeth-millington`
- every other persona

Count of personas with a non-null address MUST be **exactly 3**. At least one
persona MUST omit an address (implied by 3 of 25).

### Account download row (flatten)

Existing `AccountExport` plus:

| CSV column | Source |
|------------|--------|
| `registeredOfficeAddress1` | `Organisation.RegisteredOffice.Address1` |
| `registeredOfficeAddress2` | Address2 or `""` |
| `registeredOfficeAddress3` | Address3 or `""` |
| `registeredOfficeTown` | Town |
| `registeredOfficeRegion` | Region or `""` |
| `registeredOfficePostcode` | Postcode |
| `registeredOfficeCountry` | Country |

### Contact download row (flatten)

Existing `ContactExport` plus:

| CSV column | Source |
|------------|--------|
| `mailingAddress1` | `Persona.Address?.Address1` or `""` |
| `mailingAddress2` | Address2 or `""` |
| `mailingAddress3` | Address3 or `""` |
| `mailingTown` | Town or `""` |
| `mailingRegion` | Region or `""` |
| `mailingPostcode` | Postcode or `""` |
| `mailingCountry` | Country or `""` |

Absent mailing address → empty strings in all seven columns. Never copy the
organisation office.

## Canon files (no new file)

| File | Change |
|------|--------|
| `organisations.json` | Every object gains `registeredOffice` |
| `personas.json` | Three objects gain `address`; others unchanged |

`canon.json` version MAY bump when the dataset ships addresses.

## Display rules (not extra stored fields)

| Surface | When | What |
|---------|------|------|
| Hugo organisation single | Always | Registered-office postal layout from front matter |
| Hugo persona single | Address present | Mailing-address postal layout; omit heading if absent |
| Blazor `/contacts/{id}` | Address present | Mailing-address section; omit if absent |
| Blazor `/accounts` | Optional | Preview MAY include `registeredOfficeTown` |
| Blazor `/contacts` | Optional | Preview MAY include `mailingTown` |
| Professional extras contact | Never | No postal fields |

Hugo MUST NOT present `accountId`, persona/org slugs, or CSV column names as the
visible name of the address.

## Entity relationship diagram

```text
Organisation ──1──► Address (registeredOffice, required)
Persona ──0..1──► Address (address, optional)

Address ──flatten──► AccountExport registeredOffice*
Address? ──flatten──► ContactExport mailing* ("" if null)

ProfessionalExtras.Contact ──unchanged──► email / copy / phone only
```

## State transitions

None. Records are static demo canon.

## Validation rules (completeness)

| Code | Check | Failure mode (spec) |
|------|--------|---------------------|
| VR-044 | Every organisation has a complete `registeredOffice` | Organisation missing registered office / incomplete address |
| VR-045 | If a persona has `address`, it is complete | Incomplete address |
| VR-046 | Persona `dick-turpin` has a complete address | Richard without personal address |
| VR-047 | `black-bess` and `elizabeth-millington` have no address | Forbidden personal address |
| VR-048 | Exactly three personas have an address | Contact majority has addresses |
| VR-049 | No two organisations share the same door key | Identical org premises |
| VR-050 | `dick-turpin` door key ≠ `turpin-enterprises` door key | Richard matches Turpin Enterprises |
| VR-051 | ≥1 organisation and ≥1 persona address has non-empty `address3` | Every address skips the third line |

Schema-level required `registeredOffice` and Address required properties live in
JSON Schema ([contracts/address-canon-schema.json](./contracts/address-canon-schema.json)
merged into 001 `canon-schema.json`).

Tone: each Address string field is subject to existing forbidden-pattern checks
(`TONE-001`).

Content review (not a VR code): invented premises; no real private dwellings;
professional first read, Turpinverse humour on a second read.

## Existing entities (field additions only)

- **Organisation**: add `registeredOffice`; other fields unchanged.
- **Persona**: add optional `address`; title, biography, email, phone unchanged.
- **ContactExtras**: unchanged; must not gain postal lines.
- **Deal / Case / Project**: unchanged.
