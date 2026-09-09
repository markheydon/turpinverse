# Export API

How the **Blazor export app** exposes CRM sample data as HTTP/CSV. This is a consumer of the canon JSON under [`canon/`](../canon/). For the dataset itself, see [canon/README.md](../canon/README.md). Column contract: [`canon/schema/crm-export-schema.json`](../canon/schema/crm-export-schema.json).

**Version**: 1.1.0 | **Service**: `Turpinverse.Web`

## Overview

The Blazor Server application exposes HTTP endpoints for downloading CRM sample datasets as CSV files. All exports are generated from the canonical JSON data at request time (no cached files on disk).

## Base URL

| Environment | URL |
|-------------|-----|
| Local (Aspire) | `https://localhost:{port}` (assigned by Aspire) |
| Production | Deployment-specific |

## Endpoints

### GET /api/export/{dataset}

Download a CSV export for the specified dataset type.

**Path parameters**:

| Parameter | Type | Values |
|-----------|------|--------|
| `dataset` | string | `contacts`, `accounts`, `deals`, `cases`, `projects`, `products` |

**Query parameters** (optional; combined with AND; ignored when not applicable to the dataset):

| Name | Type | Datasets | Semantics |
|------|------|----------|-----------|
| `status` | string | `contacts`, `accounts`, `cases`, `products` | Exact match, case-insensitive |
| `taxRateId` | string | `products` | Exact match on export `taxRateId` |
| `stage` | string | `deals` | Exact match, case-insensitive |
| `priority` | string | `cases` | Exact match, case-insensitive |
| `accountId` | string | `contacts`, `deals`, `cases` | Exact match on export `accountId` |
| `industry` | string | `accounts` | Exact match, case-insensitive |

No query parameters → full dataset. Filtered body contains **only matching rows**.

**Response**:

| Status | Content-Type | Body |
|--------|-------------|------|
| 200 OK | `text/csv; charset=utf-8` | CSV with UTF-8 BOM |
| 400 Bad Request | `application/problem+json` | Invalid dataset type |
| 404 Not Found | N/A | Export API disabled (`Export:PublicApiEnabled` is false) |
| 409 Conflict | `application/problem+json` | Current filter matches zero rows (download only) |
| 500 Internal Server Error | `application/problem+json` | Canon load or validation failure |

**409 body** (empty filter match on download):

```json
{
  "title": "No matching rows",
  "detail": "The current filters matched no rows. Download was not written.",
  "status": 409,
  "type": "https://turpinverse.dev/errors/empty-filter-match"
}
```

**Filenames** (from `crm-export-schema.json`):

| Dataset | Filename |
|---------|----------|
| `contacts` | `turpinverse-contacts.csv` |
| `accounts` | `turpinverse-accounts.csv` |
| `deals` | `turpinverse-deals.csv` |
| `cases` | `turpinverse-cases.csv` |
| `projects` | `turpinverse-projects.csv` |
| `products` | `turpinverse-products.csv` |

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

Returns the first N rows as JSON (camelCase property names matching CSV columns).

**Query parameters**:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `count` | integer | `5` | Preview rows (1–100); after filtering, returns min(count, match count) |
| `status`, `stage`, `priority`, `accountId`, `industry`, `taxRateId` | string | — | Same as download |

Zero filter matches → `[]` with 200 (preview only; download uses 409).

### GET /api/canon/validate

Runs cross-reference validation and returns results. See [validation-rules.md](./validation-rules.md).

**Response** (200 OK if valid, 422 Unprocessable Entity if violations found):

```json
{
  "valid": true,
  "canonVersion": "1.4.0",
  "counts": {
    "personas": 25,
    "organisations": 10,
    "events": 12,
    "deals": 22,
    "cases": 17,
    "experience": 3,
    "education": 2,
    "projects": 3,
    "achievements": 4,
    "articles": 10,
    "galleries": 1,
    "professionalExtras": 1,
    "products": 10,
    "taxRates": 4
  },
  "violations": []
}
```

## CSV format rules

| Rule | Value |
|------|-------|
| Encoding | UTF-8 with BOM |
| Delimiter | Comma (`,`) |
| Quote character | Double quote (`"`) |
| Line ending | CRLF (`\r\n`) |
| Header row | Required; matches column order in schema |
| Empty fields | Empty string (not `null`) |
| Contact rows | One row per person–account membership; duplicate `contactId` and email across accounts is expected (**VR-059**) |
| Date format | ISO 8601 (`YYYY-MM-DD`) |
| Decimal format | Period separator, no thousands separator |

### Contact email collision policy

Turpinverse unique person key is `contactId`. Email is a copied attribute from the persona. Multi-membership rows repeat the same email. Importers whose CRM unique-keys on email must use `contactId` (or `contactId` + `accountId`) or apply their own suffix/skip rule. Documented on the Blazor contacts export page; not primary copy on Hugo.

## Blazor UI

| Route | Behaviour |
|-------|-----------|
| `/` | Dataset summary, validation badge, summary chart |
| `/contacts`, `/accounts`, `/deals`, `/cases`, `/projects`, `/products` | Filtered preview table and download (shared filter; `projects` ignores query filters today) |
| `/contacts/{id}` | Contact detail including career/portfolio and professional extras |

Channel intent: [product-surfaces.md](./product-surfaces.md).

## Security model

Fictional demo CRM data with no PII. Access is intentionally anonymous when enabled for local development and demos.

| Setting | Development default | Production default |
|---------|-------------------|-------------------|
| `Export:PublicApiEnabled` | `true` | `false` |

When `false`, export and validation endpoints are **not registered** (404). When enabled, endpoints require the `DemoExport` policy (succeeds only while the flag is true). See [code scanning alert #3](https://github.com/markheydon/turpinverse/security/code-scanning/3).

**Deployment rule**: Do not set `Export:PublicApiEnabled` to `true` on a public network without additional authentication or network restrictions.

## Error responses

All errors use [RFC 7807 Problem Details](https://datatracker.ietf.org/doc/html/rfc7807):

```json
{
  "type": "https://turpinverse.dev/errors/invalid-dataset",
  "title": "Invalid dataset type",
  "status": 400,
  "detail": "Dataset 'foo' is not supported. Valid values: contacts, accounts, deals, cases, projects."
}
```

## Related docs

- [entity-relationships.md](./entity-relationships.md) — join graph and export projection
- [validation-rules.md](./validation-rules.md) — VR codes
- [decision-log.md](./decision-log.md) — why membership rows and API gating
