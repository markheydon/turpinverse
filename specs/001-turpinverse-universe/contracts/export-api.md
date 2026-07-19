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
    "deals": 20,
    "cases": 15
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

The export dashboard (`/export`) provides:

| UI Element | Behaviour |
|------------|-----------|
| Dataset cards | One card per export type showing row count and description |
| Download button | Triggers `GET /api/export/{dataset}` download via browser |
| Preview table | Shows first 5 rows of selected dataset via `GET /api/export/{dataset}/preview` |
| Pipeline chart | Chart.js bar chart of deal stage distribution |
| Summary chart | Chart.js doughnut chart of entity counts |
| Validation badge | Green/red indicator from `GET /api/canon/validate` |

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
