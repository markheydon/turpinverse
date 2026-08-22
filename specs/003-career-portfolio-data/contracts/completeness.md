# Completeness and publication contract

**Version**: 1.0.0 | **Date**: 2026-08-22 | **Feature**: `003-career-portfolio-data`

Extends existing canon validation. CRM CSV contracts are unchanged.

## GET `/api/canon/validate`

Same endpoint as [001 export-api.md](../../001-turpinverse-universe/contracts/export-api.md)
(`DemoExport` policy). Response `counts` MUST include:

| Key | Source |
|-----|--------|
| `experience` | Number of experience groupings |
| `education` | Number of education rows |
| `projects` | Number of project catalog items |
| `achievements` | Number of achievement catalog items |

`violations[]` items keep `{ rule, message, entityType, entityId }`.

| HTTP | When |
|------|------|
| 200 | `valid: true` including career/portfolio rules |
| 422 | Any VR-020–VR-027 or schema failure |

## Validation codes

| Code | EntityType examples | Pass condition |
|------|---------------------|----------------|
| VR-020 | Experience, Education, Project, Achievement | All persona ids exist |
| VR-021 | Experience, Education, Project | Claimed organisation ids exist |
| VR-022 | Project, Achievement | `personaIds` length ≥ 1 |
| VR-023 | Project | `organisationId` present and resolved |
| VR-024 | Persona (`dick-turpin`) | FR-003–FR-008 volume and field examples |
| VR-025 | Experience | One grouping per persona per org key |
| VR-026 | Role, Education | Structured end ≥ start when both set |
| VR-027 | Project or Achievement | ≥1 catalog item shared by `dick-turpin` and another persona |

Primary subject for VR-024 and VR-027: persona id `dick-turpin`.

`entityId` SHOULD be the experience/education/project/achievement `id`, or
`dick-turpin` for volume failures.

## Human-facing surfaces (not new APIs)

| Channel | Contract |
|---------|----------|
| Hugo persona page | `/personas/{personaId}/` shows Experience, Education, Projects, Achievements only when that persona has ≥1 record of that type; order per FR-015 |
| Blazor contact detail | `/contacts/{contactId}` same omit-empty and order; `contactId` equals persona id |
| Contacts table | MAY link to the detail route; MUST NOT be the only in-product view of the four types |
| CSV `/api/export/contacts` | Unchanged columns |

## Out of scope

- New export dataset types
- Public mapping note on the Hugo site
- Theme switch or a separate profile-demo site
