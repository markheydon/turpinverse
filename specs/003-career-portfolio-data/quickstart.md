# Quickstart: Career and Portfolio Demo Data

**Date**: 2026-08-22 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

Validates publication parity and automated completeness. Does not replace
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

Regenerate Hugo content after changing canon JSON so `site/content/personas/` and
`site/data/` match [data-model.md](./data-model.md).

## Automated completeness (SC-002, FR-011)

These MUST fail until models, JSON, schema merge, and validator rules exist; they
MUST pass when the feature is done.

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
```

Expect `CanonValidator.Validate` on the loaded canon: `valid == true`, zero
violations, and counts for experience/education/projects/achievements.

Optional (API enabled in Development):

```bash
# After Aspire / Turpinverse.Web is running
curl -sS http://localhost:{port}/api/canon/validate
```

422 with VR-020–VR-027 until data and links are complete; 200 when they are.
See [contracts/completeness.md](./contracts/completeness.md).

## Scenario A — Primary career history (P1, SC-001, SC-003)

1. Open Hugo persona page: `site/content/personas/dick-turpin.md` after generate,
   or serve the site and open `/personas/dick-turpin/`.
2. Confirm **Experience**: at least three organisation groupings, each with ≥1
   role (title, date range, description); reverse chronological per FR-015.
3. Confirm at least one role extra/tooltip and at least one featured link.
4. Confirm **Education**: at least two programmes; at least one featured link;
   contemporary dates, not 1730s chronology.
5. Click organisation names that claim a canon id; they MUST resolve to existing
   organisation pages (no invented ids).

Expected: reviewer can assemble the CV in under five minutes from that page
alone (plus in-product detail below).

## Scenario B — In-product person page (FR-013)

1. `dotnet run --project src/Turpinverse.AppHost` (or `Turpinverse.Web`).
2. Open Contacts, follow the row for Richard Turpin (`dick-turpin`) to
   `/contacts/dick-turpin`.
3. Same four section types as Hugo when records exist; same omit-empty rule.

Expected: no new product and no CSV-only inspection.

## Scenario C — Portfolio and sharing (P2, FR-014)

On the same two person pages for the primary subject:

1. At least three projects: title, summary, image reference, tags, links,
   organisation (account).
2. At least one project featured CTA.
3. At least four achievements; mix of with/without URL.
4. Open the other persona who shares a catalog item; same project or
   achievement **id and title** (not a forked copy).
5. Open a persona with no education (or no experience/projects/achievements):
   that heading MUST be absent (no empty list).

## Scenario D — Mapping note (P3, SC-005)

1. Open `docs/career-portfolio-mapping.md` in the repo (not the public site).
2. Map generic fields to a typical profile layout: experience, education,
   projects, achievements (optional project detail).
3. Confirm source of truth is canon JSON, not a theme `params` tree.

Expected: mapping in under 15 minutes; public site theme unchanged.

## Scenario E — Negative completeness

Using a test canon or temporarily invalid fixture (do not commit broken default
canon):

| Inject | Expect |
|--------|--------|
| Unknown `personaId` | VR-020, validate not successful |
| Unknown `organisationId` | VR-021 |
| Project with empty `personaIds` | VR-022 |
| Two experience groupings for same persona+org | VR-025 |
| Role `end` before `start` | VR-026 |

Restore valid canon before finishing.

## Commands cheat sheet

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
dotnet test tests/Turpinverse.Web.UnitTests --filter "Category=CareerPortfolio"
dotnet test tests/Turpinverse.Core.UnitTests --filter "FullyQualifiedName~HugoContentGenerator"
cd site && hugo --minify
# or: pwsh ./scripts/Invoke-HugoSite.ps1 build
```

Hugo/Blazor UI tests may use a `CareerPortfolio` trait; implement during
`/speckit-tasks`. Until then, CanonValidation plus manual Scenarios A–C are the
gate.
