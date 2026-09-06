# Quickstart: UK Postal Addresses

**Date**: 2026-09-03 | **Plan**: [plan.md](./plan.md)

Validates completeness, CSV flatten, and dual-channel publication. Does not
replace [001 quickstart](../001-turpinverse-universe/quickstart.md).

## Prerequisites

- .NET 10 SDK (`global.json`)
- Hugo Extended **or** `scripts/Invoke-HugoSite.ps1` (Podman/Docker)
- Restore: `dotnet restore`

## Setup

```bash
dotnet build
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent
```

Regenerate Hugo content after changing canon JSON so organisation and persona
front matter match [data-model.md](./data-model.md).

## Automated completeness (SC-007, FR-009)

These MUST fail until models, JSON, schema merge, and validator rules exist; they
MUST pass when the feature is done.

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
```

Expect `CanonValidator.Validate` on the loaded canon: `valid == true`, zero
violations, every organisation has `registeredOffice`, exactly three personas
have `address`.

Optional (API enabled in Development):

```bash
# After Aspire / Turpinverse.Web is running
curl -sS http://localhost:{port}/api/canon/validate
```

422 with VR-044–VR-051 until data is complete; 200 when they are.
See [contracts/completeness.md](./contracts/completeness.md).

## Scenario A — Organisation registered offices (P1, FR-001, FR-007, FR-010)

1. Confirm all ten organisations in canon have a complete registered office.
2. Open Hugo `/organisations/turpin-enterprises/` (after generate) and at least
   one parent/subsidiary pair (e.g. Essex Solutions Group and Turpin Enterprises).
3. Confirm each page always shows a **Registered office** block in postal layout.
4. Confirm parent and subsidiary may share a town but not the same first line and
   postcode.
5. Download `/api/export/accounts` and confirm seven `registeredOffice*` columns
   per [crm-export-schema.json](./contracts/crm-export-schema.json).

Expected: reviewer sees an office on any organisation page in under two minutes
per record (SC-001). No new account-detail Blazor page.

## Scenario B — Contact minority (P2, FR-005, FR-006, FR-011)

1. Richard Turpin has a complete mailing address distinct from Turpin Enterprises.
2. Mary Brazier and James Smith also have complete addresses.
3. Black Bess and Elizabeth Millington have no address; most other personas omit
   `address` (exactly three of twenty-five have one).
4. Hugo `/personas/dick-turpin/` shows a mailing address; `/personas/black-bess/`
   shows **no** empty address heading.
5. Blazor `/contacts/dick-turpin` shows the address; `/contacts/black-bess` does
   not.
6. Contact CSV always includes mailing columns; rows without an address use empty
   strings (not the organisation office).

## Scenario C — UK field set (P3, FR-003, FR-008, FR-012)

1. Every stored address has first line, town, postcode, country `United Kingdom`.
2. At least one organisation and one contact populate `address3`; many leave it
   unused.
3. Professional extras contact still has email/copy/phone only — no postal lines.
4. No maps or coordinates.

## Scenario D — Optional preview polish

On Blazor `/accounts`, preview MAY include `registeredOfficeTown`. On
`/contacts`, preview MAY include `mailingTown`. Neither is required to close P1.

## Scenario E — Negative completeness

Using a test canon or temporarily invalid fixture (do not commit broken default
canon):

| Inject | Expect |
|--------|--------|
| Organisation without `registeredOffice` | VR-044 or schema failure |
| Address missing `town` | VR-044 (org) or VR-045 (persona) |
| `dick-turpin` without `address` | VR-046 |
| `black-bess` with an address | VR-047 |
| Four personas with addresses | VR-048 |
| Two orgs with same `address1` and `postcode` | VR-049 |
| Richard’s door key equals Turpin Enterprises | VR-050 |
| No `address3` anywhere | VR-051 |
| Forbidden tone pattern in `address1` | TONE-001 |

Restore valid canon before finishing.

## Commands cheat sheet

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
dotnet test tests/Turpinverse.Core.UnitTests --filter "FullyQualifiedName~Export"
dotnet test tests/Turpinverse.Core.UnitTests --filter "FullyQualifiedName~HugoContentGenerator"
dotnet test tests/Turpinverse.Web.UnitTests --filter "FullyQualifiedName~ContactDetail"
dotnet test tests/Turpinverse.IntegrationTests --filter "Category=CsvExport"
cd site && hugo --minify
# or: pwsh ./scripts/Invoke-HugoSite.ps1 build
```

Hugo/Blazor UI tests MAY use an `Address` or `PostalAddress` trait during
`/speckit-tasks`. Until then, CanonValidation plus CsvExport plus Scenarios A–C
are the gate.
