# Research: CRM Join Graph Alignment

**Date**: 2026-09-06 | **Plan**: [plan.md](./plan.md) | **Spec**: [spec.md](./spec.md)

Join-graph source of truth: [`docs/entity-relationships.md`](../../docs/entity-relationships.md).

## 1. Optional main contact + stakeholders on existing pipeline records

**Decision**: On `Deal`, `Case`, and `Project`, change `contactId` from required
string to optional (`string?`). Add `stakeholderContactIds` (`IReadOnlyList<string>`,
default empty). When `contactId` is set it MUST exist and MUST be a member of the
record’s account (VR-054). Stakeholders MUST exist, MUST NOT equal the main
contact, and NEED NOT be members (VR-055). Reinterpret today’s VR-005 / VR-006
“unknown contact” as “when set, identity must exist”. VR-007 (deceased must not
own **active** deals) runs only when `contactId` is set.

**Rationale**: Matches the target graph and FR-004–FR-006. Arrays on the same
record are the smallest change; the spec forbids a second stakeholder file unless
planning finds a reason to split — it does not.

**Alternatives considered**:
- *Keep required `contactId` and use a sentinel*: Lies about optionality; Hugo
  would still emit empty-id copy.
- *Join table / `stakeholders.json`*: Extra file and ids for a short list on
  22+17+3 records.
- *Encode stakeholders in `notes`*: Not machine-checkable (constitution III).

## 2. Project people: drop undifferentiated `personaIds`

**Decision**: Remove `Project.personaIds` from models, JSON Schema, Hugo front
matter, and export. Store optional `contactId` + `stakeholderContactIds`. Add a
single helper (model or presenter) **LinkedPersonaIds** = main (if any) ∪
stakeholders, unique, main first. `CareerPortfolioPresenter.GetProjectsForPersona`,
Hugo project chips, and Blazor project filters MUST use that union. Add optional
`dealId` and `caseIds` (VR-056) without authoring lineage this increment unless
already implied.

**Authored remap** (FR-008 / FR-010):

| Project | Main | Stakeholders |
|---------|------|----------------|
| `palmer-identity-vault` | `mary-brazier` | `dick-turpin` |
| `black-bess-route-optimiser` | `dick-turpin` (first listed TE member) | `ned-palmer` |
| `essex-procurement-hub` | `dick-turpin` | (none) |

**Rationale**: Spec clarification 2026-09-06. VR-022 “project must have ≥1
personaIds” MUST stop applying to projects (achievements keep VR-022). Demo data
still has people on all three projects.

**Alternatives considered**:
- *Keep `personaIds` as a computed JSON field*: Two sources of truth; schema
  `additionalProperties: false` already forbids extras.
- *Empty-union demo project*: Spec forbids inventing rows to illustrate
  optionality.

## 3. Account `primaryContactId` authored on all ten orgs (planning)

**Decision**: Schema and model: optional `primaryContactId`; VR-052 if set and
not a member. **This increment authors a primary on every existing organisation**
even though spec FR-003 / assumptions say demo NEED NOT. That is a planning
choice, not a spec edit and not a new VR requiring primaries forever.

| Organisation | `primaryContactId` |
|--------------|--------------------|
| `turpin-enterprises` | `dick-turpin` |
| `essex-gang` | `samuel-gregory` |
| `millington-inn` | `elizabeth-millington` |
| `york-assize-court` | `james-smith` |
| `king-equine-trading` | `matthew-king` |
| `brazier-legal` | `mary-brazier` |
| `bayes-horsemanship` | `richard-bayes` |
| `york-racing-society` | `sarah-thornton` |
| `epping-forest-authority` | `william-hargreaves` |
| `highway-commission` | `robert-finch` |

Do **not** reorder `dick-turpin.organisationIds` (contact primary account stays
Essex Gang first). Account primary and contact primary account remain distinct
(FR-003).

**Rationale**: User review of the spec: TE should be visibly Richard’s firm;
parent Essex reads cleaner with Gregory. All ten picks are current members, so
VR-052 passes. Deceased-as-account-primary is allowed (VR-007 is deals only).

**Alternatives considered**:
- *Leave all primaries empty*: Legal under the spec; weaker CRM demo; Richard
  would not appear as TE’s named contact.
- *Richard as primary of both Essex Gang and TE*: Allowed by cardinality but
  muddies parent vs flagship.
- *Henry Clayton as TE primary*: He is a member and active, but he is Principal
  Risk Assessor, not CEO; `case-017` already uses him as a **case** main.

## 4. Contact export: one row per membership

**Decision**: `ExportMapper.MapContacts` iterates each persona × each
`organisationIds` entry. Same `contactId`, email, phone, mailing fields; different
`accountId`. Row count MUST equal membership links (VR-059 in export tests).
Update `CsvExportTests` / `ExportApiTests` that assume one `dick-turpin` row.
`CrossReferenceTests`: skip empty deal/case `contactId`; project people from the
union. Contacts `minRows` stays ≥25 (will be ~31).

**Rationale**: FR-012 / SC-004. Current mapper uses `organisationIds[0]` only —
the bug #41 exists to fix.

**Alternatives considered**:
- *Composite `contactId` per membership*: Would invent ids; spec says person key
  stays the persona slug.
- *Per-account emails*: Forbidden (FR-011).

## 5. Collision policy lives on Blazor + exporter docs

**Decision**: Document on the in-product Contacts (export) page near Download CSV,
and in `specs/001-turpinverse-universe/contracts/export-api.md` (merge) plus this
feature’s [contracts/completeness.md](./contracts/completeness.md). Policy text:
Turpinverse unique person key is `contactId`; email is copied; multi-membership
rows repeat email on purpose; importers who unique-key email must use
contact+account or their own suffix/skip. **Not** primary Hugo copy.

**Rationale**: FR-013, constitution IX. No existing `docs/*export*` page; do not
create a new public-site section.

**Alternatives considered**:
- *Hugo FAQ page*: Would put plumbing on the showcase channel.
- *CSV comment header*: Breaks CsvHelper / CRM import.

## 6. Feature owns the join contract; live schema still merges into 001

**Decision**: This folder records the join correction. Implementation MUST merge
[join-canon-schema.json](./contracts/join-canon-schema.json) into
`specs/001-turpinverse-universe/contracts/canon-schema.json` and
[crm-export-schema.json](./contracts/crm-export-schema.json) into the live 001 CRM
schema / `ExportCsvColumns`. Update `docs/career-portfolio-mapping.md` project
people. Do **not** silently replace `001-turpinverse-universe/spec.md`.
`docs/entity-relationships.md` is already the target graph — only edit if field
names diverge.

**Rationale**: Spec FR-015 / constitution VIII; same pattern as 007.

**Alternatives considered**:
- *Only edit 001*: Silent rewrite; 008 would have no contract.
- *Runtime fork of a second schema*: Two column lists.

## 7. Hugo / generator omit-empty; membership `min` 0 on accounts

**Decision**: `HugoContentGenerator` omits `contactId` front matter when null;
emits `stakeholderContactIds` when non-empty; emits `primaryContactId` when set.
Layouts: `related-parties.html` and project singles use names; omit empty main
block; stakeholder chips allowed. Persona related deals/cases include records
where the person is **main or stakeholder** so FR-010 stories remain tellable
from person pages. JSON Schema: `Organisation.memberPersonaIds` `minItems` 0;
persona `organisationIds` stays `minItems` 1. Do not author an empty account.

Events: add optional `dealIds` / `caseIds` (VR-057). Do not invent event links
this increment; empty default is enough.

**Rationale**: FR-002, FR-014, user command to touch Hugo if FK optionality
breaks generation (today generators always write `contactId:`).

**Alternatives considered**:
- *Empty `contactId: ""` in YAML*: Risk of empty-id reader copy.
- *Keep project `personaIds` in Hugo only*: Channel drift from canon.
