# Completeness and publication contract

**Version**: 1.0.0 | **Date**: 2026-08-23 | **Feature**: `006-professional-profile-extras`

Extends existing canon validation. CRM CSV contracts are unchanged. No new export
dataset types.

## GET `/api/canon/validate`

Same endpoint as [001 export-api.md](../../001-turpinverse-universe/contracts/export-api.md)
(`DemoExport` policy). Response `counts` MUST include:

| Key | Source |
|-----|--------|
| `professionalExtras` | Number of extras sets (rows in `professional-extras.json`) |

`violations[]` items keep `{ rule, message, entityType, entityId }`.

| HTTP | When |
|------|------|
| 200 | `valid: true` including professional extras rules |
| 422 | Any VR-036–VR-043 or schema failure |

## Validation codes

| Code | EntityType examples | Pass condition |
|------|---------------------|----------------|
| VR-036 | ProfessionalExtras | Every `personaId` exists |
| VR-037 | ProfessionalExtras | At most one extras set per persona |
| VR-038 | ProfessionalExtras | Skill names unique ignoring letter case when skills present |
| VR-039 | ProfessionalExtras | Social network names unique ignoring letter case when socials present |
| VR-040 | Persona (`dick-turpin`) | Intro has short intro, headline, subtitle |
| VR-041 | Persona (`dick-turpin`) | About text; skills heading; ≥5 distinct skill names |
| VR-042 | Persona (`dick-turpin`) / ProfessionalExtras | Contact copy; email equals persona email; ≥3 distinct socials |
| VR-043 | ProfessionalExtras | No website-chrome, form/CMS, or career/portfolio list fields |

Primary subject for VR-040–VR-042: persona id `dick-turpin`.

`entityId` SHOULD be the `personaId` of the extras set, or `dick-turpin` for
volume failures.

## Human-facing surfaces (not new APIs)

See [person-surfaces.md](./person-surfaces.md).

| Channel | Contract |
|---------|----------|
| Hugo persona page | `/personas/{personaId}/` sandwich + omit-empty + header/narrative suppression |
| Blazor contact detail | `/contacts/{contactId}` same rules; `contactId` equals persona id |
| Contacts table | MAY keep linking to detail; MUST NOT be the only in-product view of extras |
| CSV `/api/export/contacts` | Unchanged columns |

## Out of scope

- New export dataset types or CSV columns
- Theme switch, second public site, or profile-demo product
- Website chrome, form endpoints, CMS keys
- Copying experience, education, projects, or achievements into extras
- Mapping note (optional later; not required)
