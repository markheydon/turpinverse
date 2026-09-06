# Quickstart: CRM Join Graph Alignment

**Date**: 2026-09-06 | **Plan**: [plan.md](./plan.md)

Validates join rules, four-row (plus project/primary) canon, membership export,
and dual-channel publication. Does not replace
[001 quickstart](../001-turpinverse-universe/quickstart.md).

## Prerequisites

- .NET 10 SDK (`global.json`)
- Hugo Extended **or** `scripts/Invoke-HugoSite.ps1` (Podman/Docker)
- Restore: `dotnet restore`

## Setup

```bash
dotnet build
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent
```

Regenerate Hugo content after changing canon JSON so deal/case/project/organisation
front matter match [data-model.md](./data-model.md).

## Automated completeness (SC-001, FR-015)

These MUST fail until models, JSON Schema merge, validator rules, and canon
repairs exist; they MUST pass when the feature is done.

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CsvExport"
dotnet test tests/Turpinverse.IntegrationTests --filter "Category=CsvExport"
```

Expect `CanonValidator.Validate` on loaded canon: `valid == true`, including
VR-052–VR-058. Export tests: contact row count equals memberships (VR-059);
`dick-turpin` has two rows (Essex Gang + Turpin Enterprises) with the same email.

Optional (API enabled in Development):

```bash
# After Aspire / Turpinverse.Web is running
curl -sS http://localhost:{port}/api/canon/validate
curl -sS http://localhost:{port}/api/export/contacts -o /tmp/contacts.csv
```

422 with VR-052–VR-058 until data and rules are complete; 200 when they are.
See [contracts/completeness.md](./contracts/completeness.md).

## Scenario A — Join rules (P1, FR-001–FR-009, FR-015)

1. Fixture: deal main contact who is **not** an account member → VR-054, names
   the deal id.
2. Same person as stakeholder only (or omitted main) → that membership violation
   gone.
3. Account `primaryContactId` pointing at a non-member → VR-052.
4. Schema/rules allow `memberPersonaIds: []`; demo still has members.
5. Project without `personaIds`; people only via main ∪ stakeholders.

## Scenario B — Story rows + primaries (P2, FR-010, FR-008, planning)

1. `deal-008`: Epping Forest Authority, William Hargreaves main, Henry Clayton
   stakeholder. Henry is **not** an Epping member.
2. `case-001` and `palmer-identity-vault`: Brazier Legal, Mary Brazier main,
   Richard Turpin stakeholder. Richard is **not** a Brazier member.
3. `case-017`: Turpin Enterprises, Henry Clayton main, Thomas Collier
   stakeholder. Thomas is **not** a TE member.
4. Hugo deal/case/project pages show those **names**; no empty-id block.
5. Organisation `/organisations/turpin-enterprises/`: primary contact shown as
   **Richard Turpin** (name, not slug as label). Essex Solutions shows Samuel
   Gregory.
6. Account CSV `primaryContactId` matches the ten-row table in data-model.md.

## Scenario C — Membership export + collision (P3, FR-012, FR-013)

1. Contact CSV row count = number of person–account membership links.
2. Preview two `dick-turpin` rows: same email/phone/mailing, different
   `accountId`.
3. Deal/case/project CSV: optional empty `contactId`; `stakeholderContactIds`
   joined; project has no undifferentiated `contactIds` people list.
4. Blazor `/contacts` (or export help on that page) states the email collision
   policy in under five minutes. Hugo person/org pages do **not** lead with that
   essay.

## Hugo generator

If optional FKs are omitted, generated markdown MUST NOT write empty
`contactId: ""` as visible-adjacent front matter that layouts treat as a person.
Stakeholders and primary contact emit only when set.

## Out of scope here

Leads, activities, products, quotes (#32–#38). Empty-account demo rows.
Per-account emails. New export files. Spec.md edits for the primary-contact
table (planning-owned).
