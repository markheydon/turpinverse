# Completeness and publication contract

**Version**: 1.0.0 | **Date**: 2026-09-03 | **Feature**: `007-uk-postal-addresses`

Extends existing canon validation. Account and contact CSV **gain** address
columns defined in [crm-export-schema.json](./crm-export-schema.json). Merge those
columns into the live 001 CRM schema at implementation time. Deals/cases/projects
CSV unchanged.

## GET `/api/canon/validate`

Same endpoint as [001 export-api.md](../../001-turpinverse-universe/contracts/export-api.md)
(`DemoExport` policy). `violations[]` items keep `{ rule, message, entityType, entityId }`.

| HTTP | When |
|------|------|
| 200 | `valid: true` including VR-044–VR-051 and schema (every org has `registeredOffice`) |
| 422 | Any VR-044–VR-051, TONE-001 on address strings, or schema failure |

`entityId` SHOULD be the organisation or persona id. For VR-051 (third-line
coverage) `entityId` MAY be a dataset-level token such as `addresses` if no
single record is at fault.

## Validation codes

| Code | EntityType examples | Pass condition |
|------|---------------------|----------------|
| VR-044 | Organisation | Complete `registeredOffice` on every organisation |
| VR-045 | Persona | Present `address` is complete |
| VR-046 | Persona (`dick-turpin`) | Richard has a complete mailing address |
| VR-047 | Persona (`black-bess`, `elizabeth-millington`) | Those ids omit `address` |
| VR-048 | Persona | Exactly three personas have an address |
| VR-049 | Organisation | Unique door key (`address1` + postcode) |
| VR-050 | Persona (`dick-turpin`) | Door key ≠ `turpin-enterprises` office |
| VR-051 | Organisation / Persona | ≥1 org and ≥1 persona use `address3` |

## CSV (`/api/export/accounts`, `/api/export/contacts`)

Headers MUST match [crm-export-schema.json](./crm-export-schema.json)
`exportManifest` column lists (after merge into live 001 schema and
`ExportCsvColumns`). Contacts without `address` still emit the seven mailing
columns as empty strings.

## Human-facing surfaces

See [channel-surfaces.md](./channel-surfaces.md).

## Out of scope

- New export dataset types
- Account detail page
- Maps, geocoding, coordinates
- Postal fields on professional-extras contact
- Rewriting `001-turpinverse-universe/spec.md` as this feature
