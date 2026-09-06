# Contract: Join-graph surfaces (Hugo showcase + Blazor explore/export)

**Channels**: Public reference site (Hugo — showcase / read) **and** in-product
web app (Blazor — explore / filter / download)  
**Surfaces**: Existing Hugo organisation, persona, deal, case, project, timeline
pages; existing Blazor contacts/accounts/deals/cases/projects + CSV export  
**Trace**: FR-012–FR-014, SC-004–SC-006; Constitution IX;
[`docs/product-surfaces.md`](../../../docs/product-surfaces.md)

Hugo remains the readable showcase. Blazor remains the explore/export app.
This feature MUST NOT add a second public site or a new download product.

## Information architecture

| Path | Kind | Change |
|------|------|--------|
| `/organisations/{slug}/` | existing Hugo | MAY show **primary contact by display name** when set; members unchanged |
| `/personas/{slug}/` | existing Hugo | Affiliations by name; related deals/cases where person is main **or** stakeholder; projects via main ∪ stakeholders |
| `/deals/{id}/`, `/cases/{id}/` | existing Hugo | Named main contact only if present; stakeholders by name; omit empty main block |
| `/projects/{slug}/` | existing Hugo | People = main ∪ stakeholders (no `personaIds` list) |
| `/contacts` | existing Blazor | Row count = memberships; collision policy copy near download |
| `/contacts/{contactId}` | existing Blazor | Still one persona; projects via union |
| `/accounts` | existing Blazor | MAY preview `primaryContactId` |
| `/deals`, `/cases`, `/projects` | existing Blazor | Optional contact + stakeholder columns |
| `/api/export/contacts` | existing CSV | One row per membership |
| `/api/export/accounts` | existing CSV | Add `primaryContactId` |
| `/api/export/deals\|cases\|projects` | existing CSV | Optional main; stakeholder field; project drops undifferentiated `contactIds` |

Do **not** add `/accounts/{id}` or a Hugo collision-policy article.

## Visible identity (Hugo)

- **MUST** use display names for people and trading names for organisations
- **MUST NOT** present `contactId`, `primaryContactId`, `stakeholderContactIds`,
  or CSV headers as primary content
- **MAY** keep join keys in front matter / `site/data` for generators
- **MUST** omit the named-contact block when main contact is absent

## Generated files (Hugo)

- Deal/case markdown: omit `contactId` key when null; include
  `stakeholderContactIds` when non-empty
- Organisation markdown: `primaryContactId` when set (layouts resolve to name)
- Project markdown: `contactId` / `stakeholderContactIds` instead of `personaIds`
- `HugoContentGenerator` MUST NOT add a new nav section for joins

## Completeness of generation

A published site and running app built from current canon MUST:

- Show Richard Turpin as Turpin Enterprises’ named primary contact on Hugo
- Show William / Mary / Henry as mains on the four repaired records, with the
  named non-members as stakeholders
- Export contact rows matching membership count, including two Richard rows
- State collision policy on Blazor contacts, not as Hugo lead copy
