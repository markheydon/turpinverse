# Contract: Address surfaces (Hugo showcase + Blazor explore/export)

**Channels**: Public reference site (Hugo — showcase / read) **and** in-product
web app (Blazor — explore / filter / download)  
**Surfaces**: Existing Hugo organisation and persona singles; existing Blazor
`/contacts/{id}`, `/accounts`, `/contacts`; existing CSV export  
**Trace**: FR-010, FR-011, SC-001, SC-004, SC-005; Constitution IX;
[`docs/product-surfaces.md`](../../../docs/product-surfaces.md)

Hugo remains the readable showcase. Blazor remains the explore/export app.
This feature MUST NOT add an account-detail product page, a map, or a second
public site.

## Information architecture

| Path | Kind | Change |
|------|------|--------|
| `/organisations/{slug}/` | existing Hugo | Always show registered office in human postal layout |
| `/personas/{slug}/` | existing Hugo | Show mailing address only when present; no empty heading |
| `/contacts/{contactId}` | existing Blazor | Show mailing address only when present |
| `/accounts` | existing Blazor | Preview MAY include `registeredOfficeTown`; no `/accounts/{id}` |
| `/contacts` | existing Blazor | Preview MAY include `mailingTown` |
| `/api/export/accounts` | existing CSV | Add `registeredOffice*` columns |
| `/api/export/contacts` | existing CSV | Add `mailing*` columns; empty strings when no address |

Do **not** add `/accounts/{id}`, map pages, or extras-contact postal fields.

## Visible identity (Hugo)

- **MUST** label organisation address as **Registered office**
- **MUST** show premises lines, town, optional region, postcode, and country as
  ordinary postal copy
- **MUST NOT** present org/persona ids, `registeredOfficeAddress1`,
  `mailingTown`, or other export/JSON keys as primary content
- **MAY** keep join keys in front matter / `site/data` for generators

## Omit empty (persona)

Do not render a mailing-address heading or placeholder block when
`Persona.address` is absent (`black-bess`, `elizabeth-millington`, and the rest
of the majority).

## Generated files (Hugo)

- Organisation markdown front matter includes nested `registeredOffice` (same
  field names as canon JSON)
- Persona markdown front matter includes nested `address` **only when present**
- `HugoContentGenerator` MUST NOT add nav items or a new section for addresses

## Completeness of generation

A published site and running app built from current canon MUST:

- Show a registered office on every organisation page (all ten)
- Show Richard Turpin’s mailing address on Hugo `/personas/dick-turpin/` and
  Blazor `/contacts/dick-turpin`, distinct from Turpin Enterprises’ office
- Omit an address block on a persona without one (e.g. `black-bess`)
- Emit account CSV with seven `registeredOffice*` columns filled
- Emit contact CSV with seven `mailing*` columns; empty strings for contacts
  without an address
