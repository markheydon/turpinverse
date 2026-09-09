# Validation rules

Canon completeness checks enforced by `CanonValidator` in `src/Turpinverse.Core/Validation/`. JSON Schema validation runs first via `CanonSchemaValidator` against [`canon/schema/canon-schema.json`](../canon/schema/canon-schema.json).

**If this table and the code disagree, the code wins.** Run `GET /api/canon/validate` or `dotnet test --filter Category=CanonValidation` after canon edits.

Violations use `{ rule, message, entityType, entityId }`.

## Core referential integrity (VR-001–VR-012)

| Code | Scope | Pass condition |
|------|-------|----------------|
| VR-001 | Persona | Every `organisationIds` entry references an existing organisation |
| VR-002 | Organisation | Every `memberPersonaIds` entry references an existing persona |
| VR-003 | Persona ↔ Organisation | Membership is bidirectional when a link exists |
| VR-004 | AliasMap | Aliases are globally unique; `personaId` exists |
| VR-005 | Deal | `contactId` omitted or references an existing persona |
| VR-006 | Case | `contactId` omitted or references an existing persona |
| VR-007 | Deal | Deceased personas must not own active deals when main contact is set |
| VR-008 | CanonEvent | Legend category events mention legend/folklore in title or description |
| VR-009 | Canon | Minimum volumes: 25 personas, 10 organisations, 20 deals, 15 cases |
| VR-010 | CanonEvent | Event year must not be after a linked persona's death year |
| VR-011 | CanonEvent | `personaIds` / `organisationIds` reference existing records |
| VR-012 | Case | `relatedEventId` omitted or references an existing event |

## Career and portfolio (VR-020–VR-027)

| Code | Scope | Pass condition |
|------|-------|----------------|
| VR-020 | Experience, Education, Project, Achievement | All persona ids exist |
| VR-021 | Experience, Education, Project | Claimed organisation ids exist |
| VR-022 | Project, Achievement | `personaIds` length ≥ 1 |
| VR-023 | Project | `organisationId` present and resolved |
| VR-024 | Persona (`dick-turpin`) | Primary profile volume and field examples |
| VR-025 | Experience | One grouping per persona per org key |
| VR-026 | Role, Education | Structured end ≥ start when both set |
| VR-027 | Project or Achievement | ≥1 catalog item shared by `dick-turpin` and another persona |

## Articles and galleries (VR-028–VR-035)

| Code | Scope | Pass condition |
|------|-------|----------------|
| VR-028 | Article | Unique ids; required fields; non-empty title/body/collection |
| VR-029 | Article | Author exists and is a `turpin-enterprises` member |
| VR-030 | Canon / Article | Published mix: 10 total; 3 by `dick-turpin`; 7 by other TE members |
| VR-031 | Article | Topic mix (route optimiser article; support-case pair) |
| VR-032 | Article | ≥1 article with tags, featured image, excerpt, TOC hint |
| VR-033 | Gallery | ≥1 gallery; ≥4 images; caption or alt on every image |
| VR-034 | Article | Related project/case ids resolve |
| VR-035 | Gallery | `subject` in `team` \| `workplace` \| `brand`; image `src` present |

## Professional extras (VR-036–VR-043)

| Code | Scope | Pass condition |
|------|-------|----------------|
| VR-036 | ProfessionalExtras | Every `personaId` exists |
| VR-037 | ProfessionalExtras | At most one extras set per persona |
| VR-038 | ProfessionalExtras | Skill names unique (case-insensitive) when present |
| VR-039 | ProfessionalExtras | Social network names unique when present |
| VR-040 | Persona (`dick-turpin`) | Intro has short intro, headline, subtitle |
| VR-041 | Persona (`dick-turpin`) | About text; skills heading; ≥5 distinct skills |
| VR-042 | Persona / ProfessionalExtras | Contact copy; email matches persona; ≥3 socials |
| VR-043 | ProfessionalExtras | No website-chrome or career-list fields in extras |

## UK addresses (VR-044–VR-051)

| Code | Scope | Pass condition |
|------|-------|----------------|
| VR-044 | Organisation | Complete `registeredOffice` on every organisation |
| VR-045 | Persona | Present `address` is complete |
| VR-046 | Persona (`dick-turpin`) | Richard has a complete mailing address |
| VR-047 | Persona | `black-bess`, `elizabeth-millington` omit `address` |
| VR-048 | Persona | Exactly three personas have an address |
| VR-049 | Organisation | Unique door key (`address1` + postcode) |
| VR-050 | Persona (`dick-turpin`) | Door key ≠ Turpin Enterprises office |
| VR-051 | Organisation / Persona | ≥1 org and ≥1 persona use `address3` |

## CRM join graph (VR-052–VR-059)

| Code | Scope | Pass condition |
|------|-------|----------------|
| VR-052 | Organisation | `primaryContactId` omitted or is a member |
| VR-053 | Deal, Case, Project | Exactly one existing account |
| VR-054 | Deal, Case, Project | `contactId` omitted or exists and is an account member |
| VR-055 | Deal, Case, Project | Stakeholders exist, unique, not equal to main |
| VR-056 | Project | `dealId` / `caseIds` omitted or exist |
| VR-057 | CanonEvent | `dealIds` / `caseIds` omitted or exist |
| VR-058 | Named records | `deal-008`, `case-001`, `case-017`, `palmer-identity-vault` match repaired deputies |
| VR-059 | Contact export | CSV row count = membership links (export tests, not `/validate` body) |

## Product catalogue (VR-060–VR-064)

| Code | Scope | Pass condition |
|------|-------|----------------|
| VR-060 | TaxRate | Unique ids; exactly four UK VAT rows (`tax-standard` 20%, `tax-reduced` 5%, `tax-zero` 0%, `tax-exempt` 0%) |
| VR-061 | Product | Unique `productId`; required fields; ≥10 products; `unitPrice` ≥ 0; `unitOfMeasure` in `hour` \| `day` \| `each` \| `retainer-month`; `status` in `active` \| `discontinued` |
| VR-062 | Product | `taxRateId` references an existing tax rate |
| VR-063 | Organisation | `roles[]` values in `customer` \| `supplier` \| `partner`; unique within org; `turpin-enterprises` has none |
| VR-064 | Organisation | Named hats: `king-equine-trading` includes `supplier`; `brazier-legal` includes `partner` and `supplier`; `york-assize-court` includes `partner` |

## Leads (VR-065–VR-067)

| Code | Scope | Pass condition |
|------|-------|----------------|
| VR-065 | Lead | Unique `leadId`; ≥10 leads; `status` in `New` \| `Contacted` \| `Qualified` \| `Disqualified` \| `Converted`; `source` in `Web` \| `Referral` \| `Event` \| `Cold outreach` \| `Tender`; optional `rating` in `Hot` \| `Warm` \| `Cold` |
| VR-066 | Lead | `convertedContactId` required when `status` is `Converted` and must reference an existing persona; must be omitted otherwise |
| VR-067 | Lead | Optional `accountId` references an existing organisation when set (no membership required) |

## Tone (TONE-001)

Forbidden patterns from `tone-guidelines.json` applied to organisation descriptions, product name/description, lead company/contact/description, persona notes, and address string fields.

## Related docs

- [entity-relationships.md](./entity-relationships.md) — join graph and repaired rows
- [export-api.md](./export-api.md) — `/api/canon/validate` HTTP contract
