# Contract: Person surfaces (Hugo showcase + Blazor explore)

**Channels**: Public reference site (Hugo — showcase / read) **and** in-product
web app (Blazor — explore ahead of export)  
**Surfaces**: Existing Hugo persona layout; existing Blazor `/contacts/{id}`  
**Trace**: FR-004, FR-005, FR-012, FR-013, SC-001, SC-004; Constitution IX;
[`docs/product-surfaces.md`](../../../docs/product-surfaces.md)

This feature MUST NOT add a third product, a résumé theme, or a new export file.
Hugo remains the readable showcase. Blazor remains the explore/export app.
Technical identifiers MUST NOT become primary copy on Hugo; Blazor MAY keep
contact id in the not-found path as today.

## Information architecture

| Path | Kind | Change |
|------|------|--------|
| `/personas/{slug}/` | existing Hugo | Add extras types in sandwich order; suppress competing title/biography/header email when extras exist |
| `/contacts/{contactId}` | existing Blazor | Same sandwich and suppression |
| `/personas/`, `/contacts` | existing indexes | No new extras index; no new nav item required |
| `/api/export/contacts` | existing CSV | Unchanged |

Do **not** add `/profile/`, a second site, or extras-only pages.

## Sandwich order (both channels)

When a type has content, render in this order. Skip missing types without leaving
a hole that reorders the rest.

1. **Intro** as header (short intro, headline, subtitle; optional photo; optional CTA)
2. **About**
3. **Skills** (heading + names in stored/author order)
4. **Experience** (existing; 003 order)
5. **Education** (existing; 003 order)
6. **Projects** (existing)
7. **Achievements** (existing)
8. **Contact** (copy, email, optional phone, optional CTA)
9. **Socials** (network + URL in stored/author order)

Related orgs, deals, cases, and articles on Hugo MAY remain after this sandwich
(existing page chrome). They are not extras types and MUST NOT insert between
skills and career or between career and contact.

## Suppression rules

| Extra present | MUST NOT show as competing |
|---------------|----------------------------|
| Intro | Persona `title` as a second header line |
| About | Persona `biography` as a second narrative block |
| Contact | Header email line as a second contact treatment |

Display name / `h1` identity MAY remain. Stored title, biography, and email stay
on the persona record.

## Omit empty

Do not render a heading or empty list for intro, about, skills, contact, or
socials when that type is absent for the persona. Deputies with no extras keep
today’s title + biography (+ header email) behaviour.

## Generated files (Hugo)

- `site/data/` extras keyed by persona id (same idea as `site/data/career/{id}.json`)
- Persona markdown MAY gain front-matter flags for intro/about presence; join
  keys stay in data JSON, not as reader-facing identity
- `HugoContentGenerator` MUST NOT switch theme or add a profile section to nav

## Visible identity (Hugo)

- **MUST** show headline/subtitle/about/skill names/network names as human copy
- **MUST NOT** present `personaId`, export column names, or extras JSON keys as
  primary content
- **MAY** use slugs in URLs

## Blazor explore

Contact detail MAY show status and other existing explore chrome. It MUST apply
the same sandwich, omit-empty, and suppression rules. It MUST NOT add CSV
download controls on the extras blocks (download stays on dataset pages).

## Completeness of generation

A published site and running app built from current canon MUST, for
`dick-turpin`:

- Show intro as the header without a competing title line
- Show about then skills before career/portfolio
- Show contact then socials after career/portfolio
- Show ≥5 skills and ≥3 socials in author order
- Omit empty extras types on a persona that lacks them (e.g. `john-king` if no extras)
