# Export API Contract

**Version**: 1.0.0 | **Date**: 2026-07-18 | **Service**: `Turpinverse.Web`

## Overview

The Blazor Server application exposes HTTP endpoints for downloading CRM sample datasets
as CSV files. All exports are generated from the canonical JSON data at request time (no
cached files on disk).

## Base URL

| Environment | URL |
|-------------|-----|
| Local (Aspire) | `https://localhost:{port}` (assigned by Aspire) |
| Production | TBD — Blazor app deployment target not in v1 scope |

## Endpoints

### GET /api/export/{dataset}

Download a CSV export for the specified dataset type.

**Path parameters**:

| Parameter | Type | Values |
|-----------|------|--------|
| `dataset` | string | `contacts`, `accounts`, `deals`, `cases` |

**Query parameters**: None in v1.

**Response**:

| Status | Content-Type | Body |
|--------|-------------|------|
| 200 OK | `text/csv; charset=utf-8` | CSV with UTF-8 BOM |
| 400 Bad Request | `application/problem+json` | Invalid dataset type |
| 404 Not Found | N/A | Export API disabled (`Export:PublicApiEnabled` is false) |
| 500 Internal Server Error | `application/problem+json` | Canon load or validation failure |

**Response headers**:

| Header | Value |
|--------|-------|
| `Content-Disposition` | `attachment; filename="{filename}"` |
| `Content-Type` | `text/csv; charset=utf-8` |

**Filenames** (from [crm-export-schema.json](./crm-export-schema.json)):

| Dataset | Filename |
|---------|----------|
| `contacts` | `turpinverse-contacts.csv` |
| `accounts` | `turpinverse-accounts.csv` |
| `deals` | `turpinverse-deals.csv` |
| `cases` | `turpinverse-cases.csv` |

### GET /api/export/manifest

Returns metadata about available export types and row counts.

**Response** (200 OK, `application/json`):

```json
{
  "version": "1.0.0",
  "datasets": [
    {
      "type": "contacts",
      "filename": "turpinverse-contacts.csv",
      "rowCount": 25,
      "columns": ["contactId", "firstName", "lastName", "..."]
    }
  ]
}
```

### GET /api/export/{dataset}/preview

Returns the first N rows of a dataset as JSON (camelCase property names matching CSV columns).

**Path parameters**:

| Parameter | Type | Values |
|-----------|------|--------|
| `dataset` | string | `contacts`, `accounts`, `deals`, `cases` |

**Query parameters**:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `count` | integer | `5` | Number of preview rows (1–100) |

**Response** (200 OK, `application/json`): Array of row objects.

### GET /api/canon/validate

Runs cross-reference validation (VR-001 through VR-010) and returns results.

**Response** (200 OK if valid, 422 Unprocessable Entity if violations found):

```json
{
  "valid": true,
  "canonVersion": "1.0.0",
  "counts": {
    "personas": 25,
    "organisations": 10,
    "events": 12,
    "deals": 22,
    "cases": 17
  },
  "violations": []
}
```

**Violation object**:

```json
{
  "rule": "VR-001",
  "message": "Persona 'john-wheeler' references unknown organisation 'essex-gang'",
  "entityType": "Persona",
  "entityId": "john-wheeler"
}
```

## CSV Format Rules

| Rule | Value |
|------|-------|
| Encoding | UTF-8 with BOM |
| Delimiter | Comma (`,`) |
| Quote character | Double quote (`"`) |
| Line ending | CRLF (`\r\n`) |
| Header row | Required; matches column order in schema |
| Empty fields | Empty string (not `null`) |
| Date format | ISO 8601 (`YYYY-MM-DD`) |
| Decimal format | Period separator, no thousands separator |

## Blazor UI Contract

The Blazor app is Turpinverse's **interactive export surface**: users explore what demo
data exists, preview records (including technical identifier columns where needed for
import), and download CSV. Filtering and faceting are the intended direction for dataset
pages; v1 delivers overview, per-dataset preview tables, charts where useful, and download.
Contact detail (`/contacts/{id}`) supports exploration of a single person — including
career and portfolio — before export. Channel intent is defined in
[`docs/product-surfaces.md`](../../../docs/product-surfaces.md).

The CRM data browser is split across a home overview and per-dataset pages:

| Route | Component | Behaviour |
|-------|-----------|-----------|
| `/` | `Home.razor` | Dataset summary cards (row counts), validation badge from `GET /api/canon/validate`, summary doughnut chart from manifest counts |
| `/contacts` | `Contacts.razor` → `CrmEntityPage.razor` | Preview table and **Download CSV** button |
| `/accounts` | `Accounts.razor` → `CrmEntityPage.razor` | Preview table and **Download CSV** button |
| `/deals` | `Deals.razor` → `CrmEntityPage.razor` | Pipeline bar chart, preview table, and **Download CSV** button |
| `/cases` | `Cases.razor` → `CrmEntityPage.razor` | Preview table and **Download CSV** button |

Shared UI elements:

| UI Element | Location | Behaviour |
|------------|----------|-----------|
| Preview table | `DataTable.razor` in `CrmEntityPage` | Loads via `GET /api/export/{dataset}/preview?count=100` |
| Download button | `CrmEntityPage` | Opens `GET /api/export/{dataset}` in a new browser tab |
| Pipeline chart | `PipelineChart.razor` on `/deals` | Chart.js bar chart of deal stage distribution |
| Summary chart | `SummaryChart.razor` on `/` | Chart.js doughnut chart of dataset row counts |
| Validation badge | `Home.razor` | Green/red indicator from `GET /api/canon/validate` |
| Navigation | `NavMenu.razor` | Lucide icons and links to home and each dataset page |

## Security Model

The export API exposes **fictional demo CRM data** with no personally identifiable
information. Access is intentionally anonymous when enabled for local development
and demos.

| Setting | Development default | Production default |
|---------|-------------------|-------------------|
| `Export:PublicApiEnabled` | `true` | `false` |

When `Export:PublicApiEnabled` is `false`, export and validation endpoints are
**not registered** — requests return **404 Not Found**.

When enabled, endpoints require the `DemoExport` authorization policy, which
succeeds only while the flag is true. This documents intentional anonymous
access for CodeQL and security review (see
[code scanning alert #3](https://github.com/markheydon/turpinverse/security/code-scanning/3)).

**Deployment rule**: Do not set `Export:PublicApiEnabled` to `true` on a public
network without additional authentication or network restrictions.

## Error Responses

All errors use [RFC 7807 Problem Details](https://datatracker.ietf.org/doc/html/rfc7807):

```json
{
  "type": "https://turpinverse.dev/errors/invalid-dataset",
  "title": "Invalid dataset type",
  "status": 400,
  "detail": "Dataset 'foo' is not supported. Valid values: contacts, accounts, deals, cases."
}
```

## Versioning

- API version is included in the export manifest response.
- Breaking changes to CSV column order or names require a major version bump.
- New dataset types (future formats: JSON, Dynamics 365 package) will be added as new
  endpoints without modifying existing CSV contracts.

## Test Contract

Integration tests MUST verify:

1. Each endpoint returns 200 with correct `Content-Type` and `Content-Disposition`.
2. CSV header row matches schema column order exactly.
3. Row counts meet minimum thresholds from `crm-export-schema.json`.
4. Every foreign key in deals/cases resolves to a row in contacts/accounts exports.
5. Invalid dataset type returns 400 with problem details.
