# Career and portfolio mapping

Turpinverse stores **generic** career and portfolio entities in canon JSON. They are first-class domain records, not a copy of any one personal-site theme or CMS schema.

This note shows how those entities can be **projected** onto a typical personal-site or profile layout (experience, education, projects, achievements). The projection is illustrative — Turpinverse remains the source of truth.

Canon files involved:

| File | Entity |
|------|--------|
| `experience.json` | Work history grouped by employer |
| `education.json` | Schools and programmes |
| `projects.json` | Shared project catalog (many personas per project) |
| `achievements.json` | Shared achievement catalog (many personas per achievement) |

## Experience → “Work” / “Experience” section

| Turpinverse field | Typical profile slot |
|-------------------|----------------------|
| `organisationName` | Company name heading |
| `organisationUrl` | Company link |
| `organisationId` (resolved) | Link to organisation page in Turpinverse |
| `roles[].title` | Job title |
| `roles[].start` / `roles[].end` | Date range |
| `roles[].description` | Role summary |
| `roles[].featuredLinks` | Optional links or badges on a role |

One `Experience` record = one employer grouping. Multiple roles at the same employer (including rehire) live under `roles[]` on a single record.

## Education → “Education” section

| Turpinverse field | Typical profile slot |
|-------------------|----------------------|
| `title` | Degree or programme name |
| `institutionName` | School name |
| `institutionUrl` | School link |
| `organisationId` (when set) | Link to Turpinverse organisation |
| `start` / `end` / `displayRange` | Dates |
| `grade` | Grade or classification |
| `description` | Programme summary |
| `featuredLink` | Optional CTA (e.g. certificate link) |

## Projects → “Projects” section

| Turpinverse field | Typical profile slot |
|-------------------|----------------------|
| `title` | Project name |
| `summary` | Short description |
| `image` | Thumbnail or hero image |
| `tags` | Tech or topic tags |
| `links` | Demo, repo, or case-study links |
| `featuredCta` | Primary call-to-action |
| `organisationId` | Sponsoring company (CRM account) |
| `personaIds` | Contributors or owners |

Projects are a **shared catalog**: filter by `personaIds` to show only projects for a given person.

## Achievements → “Achievements” / “Awards” section

| Turpinverse field | Typical profile slot |
|-------------------|----------------------|
| `title` | Award or milestone name |
| `summary` | Description |
| `url` | External link |
| `image` | Badge or illustration |
| `personaIds` | Recipients |

Achievements are also shared; filter by `personaIds` for a given person.

## Where records appear in Turpinverse

| Channel | Location |
|---------|----------|
| Hugo | Persona page sections (omit empty section types) |
| Blazor | Contact detail at `/contacts/{id}` (omit empty section types) |

Person-level narrative content is published on both channels for the same records; see [product-surfaces.md](./product-surfaces.md) for why Hugo and Blazor both show person detail without duplicating the same product purpose.

CSV exports for contacts, accounts, deals, and cases are unchanged by career/portfolio data. Career and portfolio are human-facing narrative demo data, not additional CRM export columns in the current design.
