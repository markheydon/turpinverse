# Quickstart: Professional Profile Extras

**Date**: 2026-08-23 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

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

Regenerate Hugo content after changing canon JSON so persona layouts and
`site/data/` match [data-model.md](./data-model.md).

## Automated completeness (SC-002, SC-003, FR-010)

These MUST fail until models, JSON, schema merge, and validator rules exist; they
MUST pass when the feature is done.

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
```

Expect `CanonValidator.Validate` on the loaded canon: `valid == true`, zero
violations, and a `professionalExtras` count.

Optional (API enabled in Development):

```bash
# After Aspire / Turpinverse.Web is running
curl -sS http://localhost:{port}/api/canon/validate
```

422 with VR-036–VR-043 until data and links are complete; 200 when they are.
See [contracts/completeness.md](./contracts/completeness.md).

## Scenario A — Intro header (P1, FR-003, FR-004, SC-001)

1. Open Hugo `/personas/dick-turpin/` (after generate) and Blazor
   `/contacts/dick-turpin`.
2. Confirm short intro, headline, and subtitle as the profile header.
3. Confirm display name remains the identity heading.
4. Confirm the stored job title is **not** a second competing header line.
5. Optional photo and at most one header CTA MAY appear; placeholders are fine.
6. Compare headline/subtitle to existing display name, title, and career records:
   same identity, no invented conflicting job.

Expected: reviewer finds intro on both existing person surfaces in under five
minutes; no new product or theme.

## Scenario B — About and skills (P2, FR-005, FR-006, FR-013)

On the same two person pages:

1. About is the longer narrative immediately under the intro header; biography is
   **not** a second competing block.
2. Skills follow about, **before** Experience / Education / Projects /
   Achievements.
3. Skills heading plus at least five unique names in stored/author order (not
   alphabetically unless that is also author order).
4. Career/portfolio lists are unchanged and not copied into extras JSON.

## Scenario C — Contact and socials (P3, FR-007, FR-008, FR-013)

On the same two person pages:

1. Contact then socials appear **after** career/portfolio blocks.
2. Contact copy plus `richard.turpin@turpinverse.uk` (existing persona email).
3. Phone MAY appear; form endpoints MUST NOT.
4. At least three uniquely named social networks with URLs, author order.
5. Header email is **not** a second competing contact line when contact extras
   exist.

## Scenario D — Omit empty (SC-004)

1. Open a persona with no extras (for example a deputy without a row in
   `professional-extras.json`).
2. Confirm no empty Intro / About / Skills / Contact / Socials headings.
3. Existing title and biography MAY still show (fallback).

## Scenario E — Negative completeness

Using a test canon or temporarily invalid fixture (do not commit broken default
canon):

| Inject | Expect |
|--------|--------|
| Unknown `personaId` | VR-036 |
| Two extras rows for `dick-turpin` | VR-037 |
| Skills `Go` and `go` | VR-038 |
| Two socials named `LinkedIn` / `linkedin` | VR-039 |
| Primary missing intro headline | VR-040 |
| Primary with fewer than five skills | VR-041 |
| Contact email ≠ persona email, or fewer than three socials | VR-042 |
| Extra field `formEndpoint` or `experience` array | VR-043 or schema failure |

Restore valid canon before finishing.

## Commands cheat sheet

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
dotnet test tests/Turpinverse.Web.UnitTests --filter "Category=ProfessionalExtras"
dotnet test tests/Turpinverse.Core.UnitTests --filter "FullyQualifiedName~HugoContentGenerator"
cd site && hugo --minify
# or: pwsh ./scripts/Invoke-HugoSite.ps1 build
```

Hugo/Blazor UI tests MAY use a `ProfessionalExtras` trait; implement during
`/speckit-tasks`. Until then, CanonValidation plus manual Scenarios A–D are the
gate.
