# Data Model: CRM Join Graph Alignment

**Date**: 2026-09-06 | **Spec**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md)

Cardinality matches [`docs/entity-relationships.md`](../../docs/entity-relationships.md).
JSON names are camelCase via existing `JsonCanonRepository` policy.

## Entities

### Contact (Persona) — unchanged identity, unchanged membership minimum

| Field | Change |
|-------|--------|
| `organisationIds` | Still min 1 (FR-001). First entry remains **contact primary account**. This increment does **not** reorder `dick-turpin`. |
| `email`, `phone`, `address` | One identity per person. Duplicated only at export (FR-011). |

Professional-extras `contact` is **not** a CRM contact (unchanged).

### Account (Organisation)

| Field | JSON | Type | Required | Validation |
|-------|------|------|----------|------------|
| MemberPersonaIds | `memberPersonaIds` | string[] | ✅ (array; may be empty) | Schema `minItems` 0. Demo still has members. Bidirectional with persona lists (VR-003). |
| PrimaryContactId | `primaryContactId` | string? | ❌ | When set: must exist and be a **member** (VR-052). Distinct from contact primary account. |

**Planning — authored primaries (all ten current orgs):**

| `id` | `primaryContactId` | Lore note |
|------|--------------------|-----------|
| `turpin-enterprises` | `dick-turpin` | Flagship; CEO |
| `essex-gang` | `samuel-gregory` | Parent / acquired-gang CEO |
| `millington-inn` | `elizabeth-millington` | Landlady |
| `york-assize-court` | `james-smith` | Only member |
| `king-equine-trading` | `matthew-king` | Principal |
| `brazier-legal` | `mary-brazier` | Named partner |
| `bayes-horsemanship` | `richard-bayes` | Only member |
| `york-racing-society` | `sarah-thornton` | Only member |
| `epping-forest-authority` | `william-hargreaves` | Only member |
| `highway-commission` | `robert-finch` | Infrastructure billing (Clayton remains dual-hat TE member) |

Dataset test (not a VR): loaded canon matches this table. Schema still allows omit.

### Membership

Not a stored row. Implied by persona `organisationIds` ∩ org `memberPersonaIds`.
Export projects **one contact CSV row per link** (VR-059).

### Deal / Case

| Field | JSON | Type | Required | Validation |
|-------|------|------|----------|------------|
| AccountId | `accountId` | string | ✅ | Must exist (VR-053). |
| ContactId | `contactId` | string? | ❌ | When set: exists and is a **member of that account** (VR-054). VR-007 if deceased and deal stage is active. |
| StakeholderContactIds | `stakeholderContactIds` | string[] | ❌ default [] | Each exists; unique; none equals `contactId` (VR-055). Need not be members. |

**FR-010 repairs:**

| Record | Account (unchanged) | Main | Stakeholder |
|--------|---------------------|------|-------------|
| `deal-008` | `epping-forest-authority` | `william-hargreaves` | `henry-clayton` |
| `case-001` | `brazier-legal` | `mary-brazier` | `dick-turpin` |
| `case-017` | `turpin-enterprises` | `henry-clayton` | `thomas-collier` |

Do not add Richard to Brazier, Henry to Epping, or Thomas to Turpin Enterprises.

Other deals/cases keep today’s `contactId` where it already satisfies VR-054.
Do not strip main contacts to illustrate optionality. Do not add extra
stakeholder casts beyond FR-010.

### Project

| Field | JSON | Type | Required | Validation |
|-------|------|------|----------|------------|
| OrganisationId | `organisationId` | string | ✅ | Sponsoring account (VR-053). |
| ContactId | `contactId` | string? | ❌ | Same as deal main (VR-054). |
| StakeholderContactIds | `stakeholderContactIds` | string[] | ❌ default [] | Same as deal stakeholders (VR-055). |
| DealId | `dealId` | string? | ❌ | When set, must exist (VR-056). |
| CaseIds | `caseIds` | string[] | ❌ default [] | When set, each must exist (VR-056). |

**Removed:** `personaIds`. “Who is on this project” = LinkedPersonaIds (main ∪
stakeholders). Achievements keep `personaIds` and VR-022.

**Authored people:** see [research.md](./research.md) §2. `palmer-identity-vault`
is FR-010 (Mary main, Richard stakeholder). Lineage `dealId` / `caseIds` MAY stay
empty this increment.

### Timeline event

| Field | JSON | Type | Required | Validation |
|-------|------|------|----------|------------|
| DealIds | `dealIds` | string[] | ❌ default [] | Each must exist (VR-057). |
| CaseIds | `caseIds` | string[] | ❌ default [] | Each must exist (VR-057). |

Do not duplicate case account/main onto the event. Do not author new event
pipeline arrays unless already present.

### Contact download row

One membership projection:

| CSV column | Source |
|------------|--------|
| `contactId` | Persona id (repeated) |
| identity / mailing | Copied from persona |
| `accountId` | This membership’s organisation id |

### Account download row

Existing columns plus optional `primaryContactId` (empty string when omitted).

### Deal / Case / Project download rows

| CSV column | Source |
|------------|--------|
| `contactId` | Main or `""` |
| `stakeholderContactIds` | Joined contact ids (`"; "`), empty when none |
| Project also | `dealId`, `caseIds` (joined), **not** a `contactIds` blob of the old people list |

## Relationships

```text
Persona *──* Organisation     (membership; contact ≥1 account; account ≥0 members)
Organisation 0..1──0..1 Persona  (primaryContactId; member when set)
Organisation 1──* Deal|Case|Project  (exactly one account)
Persona 0..1──* Deal|Case|Project    (optional main; member when set)
Persona *──* Deal|Case|Project       (stakeholders; need not be members)
Project 0..1── Deal ; Project *── Case  (optional lineage)
CanonEvent *── Deal|Case             (optional; exist when set)
```

## Validation summary (this feature)

| Code | Rule |
|------|------|
| VR-052 | Account primary, if set, is a member |
| VR-053 | Deal/case/project has exactly one existing account |
| VR-054 | Main contact, if set, exists and is an account member |
| VR-055 | Stakeholders exist, unique, ≠ main |
| VR-056 | Project deal/cases exist when set |
| VR-057 | Event deal/cases exist when set |
| VR-058 | Four named rows use member main + required stakeholder; projects have no `personaIds` |
| VR-059 | Contact export row count = membership link count (export tests) |

Older VR-005 / VR-006: when `contactId` set, identity must exist. VR-022: projects
no longer require ≥1 people; achievements unchanged.

## State transitions

None. Status/stage enums unchanged. VR-007 still applies to **active** deal stages
when a main contact is set.
