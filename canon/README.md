# Turpinverse canon datasets

Fictional Dick Turpin universe demo data: personas, organisations, deals, cases, timeline, career, articles, and more. **This folder is the product.** The Hugo site and Blazor export app in this repository are optional consumers; you can use these JSON files from any other project.

## Quick start

1. Clone or submodule this repository (or copy this `canon/` folder).
2. Read `canon.json` for the dataset version (currently `1.3.0`).
3. Load the entity files listed below. Property names are **camelCase** JSON.
4. Validate against [`schema/canon-schema.json`](schema/canon-schema.json) if your toolchain supports JSON Schema.
5. For CRM-shaped CSV columns (optional), see [`schema/crm-export-schema.json`](schema/crm-export-schema.json).

Example (relative path from another repo):

```text
../turpinverse/canon/personas.json
../turpinverse/canon/organisations.json
```

## Files

| File | Contents |
|------|----------|
| `canon.json` | Dataset version |
| `personas.json` | Characters (CRM: contacts source) |
| `organisations.json` | Accounts source; includes `registeredOffice` |
| `events.json` | Timeline |
| `aliases.json` | Alternate identities |
| `deals.json` | Pipeline opportunities |
| `cases.json` | Support tickets |
| `projects.json` | Project catalog |
| `experience.json` | Career history groupings |
| `education.json` | Education records |
| `achievements.json` | Achievement catalog |
| `articles.json` | In-universe articles |
| `galleries.json` | Image galleries |
| `professional-extras.json` | Intro, about, skills, contact, socials |
| `tone-guidelines.json` | Humour rules and forbidden patterns |
| `schema/canon-schema.json` | JSON Schema for all record types |
| `schema/crm-export-schema.json` | CSV column contract for CRM import |

## How records join

Membership, deals, cases, projects, and export behaviour are documented in human-readable form:

- [docs/entity-relationships.md](../docs/entity-relationships.md) — join graph (Mermaid)
- [docs/validation-rules.md](../docs/validation-rules.md) — automated rule codes (VR-001…)

**Contact export rule:** one CSV row per `(persona, organisation)` membership when using this repo's Blazor exporter — not one row per person.

## Voice and tone

Copy conventions (modern names, euphemism, legend-first humour): [docs/universe-voice.md](../docs/universe-voice.md).

## CSV instead of JSON

This repository's Blazor app can filter and download CRM CSV. HTTP contract: [docs/export-api.md](../docs/export-api.md). Run locally via `dotnet run --project src/Turpinverse.AppHost` (see root [README](../README.md)).

## Human-readable showcase

The public site at [turpinverse.uk](https://turpinverse.uk) is generated from this canon. Channel split: [docs/product-surfaces.md](../docs/product-surfaces.md).

## Not current

Historical Spec Kit feature packs under [`archive/specs/`](../archive/specs/) are frozen and **not** the source of truth. Use this folder and `docs/` instead.

## Licence

MIT — see [LICENSE](../LICENSE).
