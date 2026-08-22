# Career and portfolio field mapping

Developer reference for how generic profile/resume fields map onto Turpinverse canon
entities and publication surfaces. **Source of truth is canon JSON** under
`src/Turpinverse.Data/canon/` — not Hugo `params`, theme config, or Blazor hard-coded
copy.

Related spec: `specs/003-career-portfolio-data/`. Runtime schema:
`specs/001-turpinverse-universe/contracts/canon-schema.json`.

## Canon files and entities

| Canon file | Entity | Typical profile section |
|------------|--------|-------------------------|
| `experience.json` | Experience (grouping) + nested Role | Work history / employment |
| `education.json` | Education | Education / qualifications |
| `projects.json` | Project (shared catalog) | Portfolio / projects |
| `achievements.json` | Achievement (shared catalog) | Awards, certifications, highlights |

Contacts and accounts CSV exports are unchanged. Career data is authored canon, loaded
by `JsonCanonRepository`, validated by `CanonValidator` (VR-020–VR-027), ordered by
`CareerPortfolioPresenter` (FR-015), and projected to Hugo and Blazor.

## Field mapping

### Experience grouping → employer block

| Generic field | Canon field | Notes |
|---------------|-------------|-------|
| Company name | `organisationName` | Required display name |
| Company URL | `organisationUrl` | Optional when no canon org page |
| CRM account link | `organisationId` | Optional; must resolve to `Organisation.id` |
| Person | `personaId` | Must resolve to `Persona.id` |
| Stable key | `id` | Slug; unique among experience rows |
| Multiple jobs at same employer | `roles[]` | Rehire = another role, not a second grouping |

One grouping per persona per organisation key (`organisationId` or normalised
`organisationName`). Duplicate keys fail VR-025.

### Role → job within employer

| Generic field | Canon field | Notes |
|---------------|-------------|-------|
| Job title | `title` | Required |
| Start date | `start` | `YYYY-MM` or `YYYY-MM-DD` |
| End date | `end` | Omit for current role |
| Display range | `displayRange` | Optional human override of structured dates |
| Description | `description` | Markdown allowed |
| Tooltip / extra line | `extraInfo` | Optional |
| Related links | `featuredLinks[]` | `FeaturedLink` objects |

### Education → qualification row

| Generic field | Canon field | Notes |
|---------------|-------------|-------|
| Degree / programme | `title` | Required |
| Institution | `institutionName` | Required |
| Institution URL | `institutionUrl` | Optional |
| CRM org link | `organisationId` | Optional |
| Dates | `start`, `end`, `displayRange` | Same formats as Role |
| Grade / GPA | `grade` | Optional |
| Description | `description` | Markdown allowed |
| CTA link | `featuredLink` | Single `FeaturedLink` |

### Project → portfolio item

| Generic field | Canon field | Notes |
|---------------|-------------|-------|
| Title | `title` | Required |
| Summary | `summary` | Required |
| Thumbnail | `image` | Path or URL |
| Tags / badges | `tags[]` | Min one |
| Links | `links[]` | Min one `FeaturedLink` |
| Client / account | `organisationId` | Required; CRM account |
| Contributors | `personaIds[]` | Min one persona; shared catalog |
| Featured CTA | `featuredCta` | Optional `FeaturedLink` |
| Home highlight | `featured` | Optional boolean |

### Achievement → award / highlight

| Generic field | Canon field | Notes |
|---------------|-------------|-------|
| Title | `title` | Required |
| Summary | `summary` | Required |
| Link | `url` | Optional |
| Image | `image` | Optional |
| People | `personaIds[]` | Min one persona; shared catalog |

### FeaturedLink (shared)

| Generic field | Canon field | Notes |
|---------------|-------------|-------|
| URL | `url` | Required |
| Label | `label` or `name` | At least one of label, name, or icon |
| Icon key | `icon` | e.g. Lucide name |
| Tooltip | `tooltip` | Optional |

## Projection examples

### Experience: grouping vs flat jobs

**Canon (grouped)** — one employer, multiple roles in `experience.json`:

```json
{
  "id": "dick-turpin-turpin-enterprises",
  "personaId": "dick-turpin",
  "organisationId": "turpin-enterprises",
  "organisationName": "Turpin Enterprises",
  "roles": [
    { "title": "Chief Corridor Strategy Officer", "start": "2022-01", "description": "..." },
    { "title": "Regional Operations Lead", "start": "2018-03", "end": "2021-06", "description": "..." }
  ]
}
```

**Hugo** (`site/data/career/{personaId}.json`) — same structure after
`CareerPortfolioPresenter` sorts groupings and roles most-recent-first. Rendered in
`site/layouts/personas/single.html` under **Experience** (omit section when empty).

**Blazor** — `ExperienceSection.razor` on `/contacts/{contactId}` maps `contactId` to
`personaId` and renders the same grouped model.

### Project detail page (catalog membership)

Projects live once in `projects.json` with `personaIds: ["dick-turpin", "ned-palmer"]`.
Both persona pages show the **same** `id` and `title` (FR-014 shared catalog). Hugo
lists cards on the persona page; there is no separate per-persona project copy. A
future dedicated project URL would still read the single catalog row by `id`.

### Omit-empty publication

If a persona has no rows of a type, that section heading is **not** rendered (Hugo
`{{ with }}` blocks; Blazor section components return early). PaperMod remains the
public theme; career content comes from generated `site/data/career/*.json`, not theme
params.

## Static assets

Project and achievement images ship under `site/static/images/` (SVG, Turpinverse-branded).
Canon `image` fields use site-root paths such as `/images/projects/black-bess-route-optimiser.svg`.
Hugo copies `static/` into `public/` on build; Blazor serves the same paths from `wwwroot` when mirrored there for parity.

In-universe `*.turpinverse.uk` URLs in canon are rewritten to `/organisations/{id}/` at publication time
(`CareerLinkResolver` in Core; `resolve-career-url.html` in Hugo).

## Regenerating published data

```bash
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent --configuration Release
```

Validate canon first:

```bash
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
```

## What is not the source of truth

- Hugo front matter / `params` on persona markdown files (biography only)
- PaperMod or other theme configuration
- Blazor component markup or CSS class names
- Generated `site/data/career/*.json` (build output; regenerate after canon edits)
