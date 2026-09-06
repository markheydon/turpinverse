# Contract: Filtered export preview and download

**Channel**: Interactive export app (Blazor) and its HTTP export API  
**Extends**: [specs/001-turpinverse-universe/contracts/export-api.md](../../001-turpinverse-universe/contracts/export-api.md)  
**Service**: `Turpinverse.Web`  
**Trace**: FR-005, FR-010

Unfiltered requests MUST keep existing 001 behaviour (full CSV, preview row cap). This contract adds optional query parameters and empty-match handling.

## Query parameters (optional)

Applies to:

- `GET /api/export/{dataset}`
- `GET /api/export/{dataset}/preview`

| Name | Type | Datasets | Semantics |
|------|------|----------|-----------|
| `status` | string | `contacts`, `accounts`, `cases` | Exact match, case-insensitive |
| `stage` | string | `deals` | Exact match, case-insensitive |
| `priority` | string | `cases` | Exact match, case-insensitive |
| `accountId` | string | `contacts`, `deals`, `cases` | Exact match on export `accountId` |
| `industry` | string | `accounts` | Exact match, case-insensitive |
| `count` | int | preview only | Unchanged clamp 1–100; after filtering, return min(count, match count) |

Parameters that do not apply to the dataset MUST be ignored (not 400). Combined constraints are AND.

`{dataset}` values for this feature’s filter requirement: `contacts`, `accounts`, `deals`, `cases`. Other datasets MAY ignore filter params.

## GET /api/export/{dataset} (download)

### 200 OK

Same as 001: `text/csv; charset=utf-8` attachment. Body contains **only rows matching the filter**. No filter → full dataset. Identifier columns unchanged.

### 409 Conflict

Current filter matches **zero** rows.

`Content-Type`: `application/problem+json`

```json
{
  "title": "No matching rows",
  "detail": "The current filters matched no rows. Download was not written.",
  "status": 409,
  "type": "https://turpinverse.dev/errors/empty-filter-match"
}
```

MUST NOT return 200 with an empty CSV or with the unfiltered dataset.

### Other statuses

Unchanged from 001 (400 invalid dataset, 401/403 policy, 500 canon failure).

## GET /api/export/{dataset}/preview

### 200 OK

JSON array of row objects (camelCase columns as today). Rows MUST match the same filter as download. Zero matches → `[]` with 200 (the UI tells the user nothing matched; only **download** uses 409).

## Blazor UI contract (same channel)

Dataset pages that use `CrmEntityPage` (`/contacts`, `/accounts`, `/deals`, `/cases`):

1. Expose the facets listed above for that dataset.
2. Preview table reflects the current filter.
3. Download uses the same filter (query string or equivalent).
4. Zero matches: visible empty state; download control disabled or confirmation that does not silently fetch a 200 empty file.
5. Identifier columns MAY remain visible in the table (FR-009).
6. MUST NOT become the primary narrative encyclopedia for the universe (FR-007).
