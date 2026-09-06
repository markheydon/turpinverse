# Completeness and publication contract

**Version**: 1.0.0 | **Date**: 2026-09-06 | **Feature**: `008-crm-join-graph`

Extends existing canon validation. CSV columns follow
[crm-export-schema.json](./crm-export-schema.json). Merge into the live 001 CRM
schema at implementation time. Join graph:
[`docs/entity-relationships.md`](../../../docs/entity-relationships.md).

## GET `/api/canon/validate`

Same endpoint as [001 export-api.md](../../001-turpinverse-universe/contracts/export-api.md)
(`DemoExport` policy). `violations[]` items keep `{ rule, message, entityType, entityId }`.

| HTTP | When |
|------|------|
| 200 | `valid: true` including VR-052–VR-058 and merged schema |
| 422 | Any VR-052–VR-058, unknown optional FKs, or schema failure |

`entityId` SHOULD be the deal, case, project, organisation, or event id.

## Validation codes

| Code | EntityType examples | Pass condition |
|------|---------------------|----------------|
| VR-052 | Organisation | `primaryContactId` omitted **or** is a member |
| VR-053 | Deal, Case, Project | Exactly one existing account |
| VR-054 | Deal, Case, Project | `contactId` omitted **or** exists and is an account member |
| VR-055 | Deal, Case, Project | Stakeholders exist, unique, not equal to main |
| VR-056 | Project | `dealId` / `caseIds` omitted or exist |
| VR-057 | CanonEvent | `dealIds` / `caseIds` omitted or exist |
| VR-058 | Named records | `deal-008`, `case-001`, `case-017`, `palmer-identity-vault` match FR-010; no project `personaIds` |
| VR-059 | Contact export | Row count = membership links (export tests, not `/validate` body) |

VR-005 / VR-006: when main contact is set, identity must exist. VR-007: deceased
must not own active deals **when** main contact is set. VR-022 no longer requires
projects to have ≥1 people.

## CSV

Headers MUST match [crm-export-schema.json](./crm-export-schema.json)
`exportManifest` after merge into live 001 schema and `ExportCsvColumns`.

- Contacts: one row per membership; empty mailing cells when persona has no
  address (007 unchanged).
- Accounts: `primaryContactId` empty string when omitted.
- Deals/cases/projects: `contactId` empty string when omitted;
  `stakeholderContactIds` joined with `"; "`.

## Collision policy (export surface)

Turpinverse unique person key is `contactId`. Email is a copied attribute.
Multi-membership rows repeat the same email. Importers whose CRM unique-keys on
email MUST use `contactId` (or contact+account) or apply their own suffix/skip
rule. Document next to Blazor contact download and in 001 `export-api.md`.
MUST NOT be primary copy on Hugo.

## Human-facing surfaces

See [channel-surfaces.md](./channel-surfaces.md).

## Planning dataset (not a VR)

Loaded organisations MUST match the ten `primaryContactId` values in
[data-model.md](../data-model.md) (`turpin-enterprises` → `dick-turpin`, etc.).

## Out of scope

- New export dataset types
- Empty-account or contact-less pipeline demo rows
- Per-account email/phone/address in canon
- Rewriting `001-turpinverse-universe/spec.md` as this feature
- CRM completeness children #33–#38 (unblocked, not built)
