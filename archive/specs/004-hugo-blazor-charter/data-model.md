# Data Model: Human-Facing Channel Alignment

**Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

**Channels affected**: Hugo projects these entities for reading; Blazor projects the same rows for explore / filter / download. Canon JSON under `src/Turpinverse.Data/canon/` remains the source of truth. This feature does **not** invent new story entities.

Existing CRM records (`Deal`, `Case`, `Persona`, `Organisation`, `CanonEvent`) are specified in [specs/001-turpinverse-universe/data-model.md](../001-turpinverse-universe/data-model.md) and career/portfolio in [specs/003-career-portfolio-data/data-model.md](../003-career-portfolio-data/data-model.md). Below: projections and the new filter object.

## Entities (projections)

### ShowcaseDeal (Hugo)

Readable view of a `Deal`.

| Field | Source | Visible to reader? |
|-------|--------|--------------------|
| Slug / path | `dealId` | URL only |
| Title | `dealName` | Yes |
| Description | `description` | Yes |
| Stage | `stage` | Yes |
| Amount | `amount` | Yes |
| Close date | `closeDate` | Yes |
| Related organisation | `accountId` → organisation trading name | Name + link; never ID as label |
| Related person | `contactId` → persona display name | Name + link; never ID as label |

**Validation**: Generator emits one page per canon deal. Missing related org/person → fallback label, not raw ID (FR-008).

### ShowcaseCase (Hugo)

Readable view of a `Case`.

| Field | Source | Visible to reader? |
|-------|--------|--------------------|
| Slug / path | `caseId` | URL only |
| Title | `subject` | Yes |
| Description | `description` | Yes |
| Status, priority | `status`, `priority` | Yes |
| Related organisation / person | `accountId`, `contactId` | Display names + links |
| Related event | `relatedEventId` | Event title + in-page/timeline link when set; omit section if null |

### ExportFilter (Blazor + export API)

Request-scoped predicate applied to a single dataset. Not persisted.

| Field | Applies to | Matching |
|-------|------------|----------|
| `Dataset` | required | `contacts`, `accounts`, `deals`, `cases` |
| `Status` | contacts, accounts, cases | Exact, case-insensitive |
| `Stage` | deals | Exact, case-insensitive |
| `Priority` | cases | Exact, case-insensitive |
| `AccountId` | contacts, deals, cases | Exact slug match on `accountId` |
| `Industry` | accounts | Exact, case-insensitive |

Unset/null fields mean “do not constrain that facet”. Combining facets is AND.

**Validation**:
- Invalid dataset type: existing 400 invalid-dataset.
- Valid filter with zero matches: empty preview; download 409 (FR-010).
- Empty filter object: full dataset.

### ImportableDataset (unchanged shape)

CSV rows and identifier columns stay as in [crm-export-schema](../001-turpinverse-universe/contracts/crm-export-schema.json). Filtering changes **which rows** are included, not column contracts.

## Relationships (explicit canon FKs only)

```text
Persona 1──* Deal     (Deal.contactId)
Persona 1──* Case     (Case.contactId)
Organisation 1──* Deal (Deal.accountId)
Organisation 1──* Case (Case.accountId)
CanonEvent 0..1──* Case (Case.relatedEventId)
```

No deal↔event foreign key. Career/portfolio dual publication unchanged.

## State / matching

Deals and cases have stage/status values already validated by `CanonValidator`. ExportFilter does not transition those states; it only selects rows.

```text
[unfiltered] → all rows
[facets applied] → matching rows
[zero matches] → empty preview; download blocked (409)
```

## Hugo identity rules

| Allowed as visible identity | Forbidden as visible identity |
|-----------------------------|-------------------------------|
| Display name, deal name, case subject | `contactId`, `accountId`, `dealId`, `caseId` as label |
| Job title, demo email | CSV header names as the name of a person/org/deal/case |
| Stage, status, priority, amount | Foreign-key slugs as chip text when a title exists or a fallback should be used |

Slugs may appear in `href` and YAML used only by layouts.
