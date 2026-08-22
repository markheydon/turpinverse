# Feature Specification: Career and Portfolio Demo Data

**Feature Branch**: `003-career-portfolio-data`

**Created**: 2026-08-22

**Status**: Draft

**Input**: User description: "Review GitHub issues #19 and #20, which are the same feature with different bits of data: generic experience and education demo data, plus generic projects and achievements portfolio demo data, as first-class Turpinverse domain records (not consumer-specific profile trees). Parent: #18 Profile / career / content demo data."

**Source issues**:

- [#19 Add generic experience and education demo data](https://github.com/markheydon/turpinverse/issues/19)
- [#20 Add generic projects and achievements portfolio demo data](https://github.com/markheydon/turpinverse/issues/20)
- Parent: [#18 Profile / career / content demo data](https://github.com/markheydon/turpinverse/issues/18)

## Clarifications

### Session 2026-08-22

- Q: Should experience and education dates follow the personas’ established life spans, or use a contemporary professional overlay as if they are living demo subjects today? → A: Contemporary overlay — present-day (or recent) professional dates; historical life span is lore, not a CV constraint.
- Q: For this feature to count as done, where must a reviewer be able to see experience, education, projects, and achievements? → A: Publication parity — the same human-facing places other Turpinverse records already appear, including internal/in-product views (the existing web app) and external/public reference views; not records-only, and not a new downstream profile-demo site.
- Q: Can a project or achievement belong to more than one persona, or is each one owned by a single subject? → A: Shared catalog — one project or achievement can be linked to many personas.
- Q: Should education schools link to existing organisations the same way employers do, or stay as names and URLs only? → A: Same as jobs — link an organisation when it exists; otherwise school name and optional URL.
- Q: Should projects link like CRM cases (person and company)? → A: Yes — every project has one canon organisation (CRM account) plus one or more personas (contacts), matching Case `accountId` + `contactId`. Shared catalog still uses one account on the project, not a different company per member. Achievements stay persona-linked only unless later specified.
- Q: If a persona has no experience, education, projects, or achievements, should those blocks appear on their existing person pages? → A: Omit a section when that person has no records of that type.
- Q: When a career or portfolio record is missing required fields or points at an unknown persona or organisation, must an automated completeness check fail, or is a human review enough? → A: Automated completeness check must fail until links, required fields, and volume minima are valid.
- Q: In what order should a person’s experience and education appear on their existing person page? → A: Reverse chronological: organisations by most recent role, roles and education most-recent-first (current/open-ended count as most recent).
- Q: Where must a reviewer find the mapping note that shows how generic records could fill a typical profile site? → A: Repository-only developer docs; not required on the public/reference site.
- Q: If a persona leaves an employer and later returns, should that be one organisation grouping with two roles, or two separate experience records for the same company? → A: One grouping per persona per organisation; rehire is another role in the same grouping.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Career history for the primary profile subject (Priority: P1)

A product demonstrator building a career or personal-site demo needs structured **work experience** and **education** for the primary Turpinverse profile subject (Richard Turpin). They can show a plausible professional history: organisations, nested roles with date ranges and descriptions, and schools/programmes — written in Turpinverse voice — without inventing a separate universe.

**Why this priority**: Experience and education are the minimum for a credible career/profile demo. Without them, existing persona titles and biographies are only seeds, not usable career records.

**Independent Test**: Confirm the primary persona’s career records meet volume and field coverage (FR-003–FR-005), organisation links for jobs and schools resolve when claimed, those records are visible on each existing human-facing surface that already presents that persona or their derived contact (public/reference site and in-product web app), and the automated completeness check passes.

**Acceptance Scenarios**:

1. **Given** the existing Turpinverse persona and organisation canon, **When** a reviewer opens the primary subject's career history, **Then** they see experience tied to that persona covering at least three organisations or companies, each with one or more roles (title, date range, description).
2. **Given** a role on the primary subject's experience, **When** a reviewer inspects optional extras, **Then** at least one role includes featured links (label or icon, URL, optional tooltip) and at least one role includes extra info or tooltip text.
3. **Given** the primary subject's education list, **When** a reviewer inspects it, **Then** there are at least two programmes (degree or programme title, school/institution name, optional date and grade display, optional description), and at least one education entry includes a featured call-to-action link (name and URL).
4. **Given** an experience or education record that names a known Turpinverse organisation (for example Essex Solutions Group or Turpin Enterprises), **When** a reviewer checks the link, **Then** it refers to that existing organisation rather than a disconnected duplicate name with no relationship.
5. **Given** a reviewer familiar with Turpinverse tone rules, **When** they read role titles, dates, and education copy, **Then** the history reads as a coherent contemporary professional CV (recent or present-day ranges), not a 1730s chronology and not a dump of highwayman trivia.
6. **Given** the public/reference site and the in-product web app already show other person records, **When** a reviewer looks up the primary subject on those same channels, **Then** experience and education are visible there without opening a new, separate product.
7. **Given** the primary subject’s experience and education on those pages, **When** a reviewer reads the lists, **Then** organisations appear by most recent role, and roles and programmes appear most-recent-first (a missing/current end date counts as most recent).

---

### User Story 2 - Portfolio projects and achievements (Priority: P2)

A demonstrator needs **projects** and **achievements** for the same primary profile subject so a portfolio homepage or awards section can be populated with reusable domain records (corridor-optimisation products, alias tooling, racing-society sponsorships, and similar legend-as-business pieces).

**Why this priority**: Career history alone does not cover portfolio and awards sections that profile demos typically show. This slice is independently valuable once a primary subject exists.

**Independent Test**: Confirm the primary persona’s portfolio records meet volume and field coverage (FR-007–FR-008) and that those records are visible on the same existing human-facing surfaces used for career history (public/reference site and in-product web app), without requiring a separate downstream profile-demo site.

**Acceptance Scenarios**:

1. **Given** the primary profile persona, **When** a reviewer lists their projects, **Then** they see at least three projects each with title, summary, image reference (placeholder path or URL allowed), tags/badges, and links (label or icon plus URL).
2. **Given** the project list, **When** a reviewer looks for a highlighted piece, **Then** at least one project has a featured call-to-action (name and URL).
3. **Given** the primary persona, **When** a reviewer lists achievements, **Then** they see at least four entries with title and summary, including at least one with a URL and at least one without a URL, and optional images where provided.
4. **Given** a reviewer familiar with Turpinverse voice, **When** they scan project and achievement copy, **Then** pieces feel like in-universe products, awards, or sponsorships — not generic lorem ipsum and not consumer-theme markup.
5. **Given** the public/reference site and the in-product web app already show other person records, **When** a reviewer looks up the primary subject on those same channels, **Then** projects and achievements are visible there.
6. **Given** a project or achievement linked to more than one persona, **When** a reviewer opens each linked subject, **Then** they see the same catalog piece (same title and identity), not disconnected copies that can drift apart.
7. **Given** a project, **When** a reviewer inspects its CRM-style links, **Then** it names a canon organisation (account) as well as its persona members (contacts), in the same spirit as a case’s account and contact.
8. **Given** a persona with no education (or no experience, projects, or achievements), **When** a reviewer opens that person’s existing page, **Then** that empty type is omitted — no empty heading or empty list — while types they do have still appear.

---

### User Story 3 - Consumer-agnostic records with a mapping note (Priority: P3)

A developer preparing a later personal-site or CMS demo needs confidence that Turpinverse stores **generic** career and portfolio entities, plus a short note showing those entities *can* be mapped to a typical personal-site layout (experience, education, projects, achievements) without Turpinverse treating that layout as the source of truth.

**Why this priority**: Mapping is validation and documentation, not the demo content itself. Content in P1 and P2 remains useful even if mapping is a stub.

**Independent Test**: Read the mapping note in repository developer documentation and compare it to the domain entities; confirm every mapped field has a generic source field, that the note is not required on the public/reference site, and that no requirement exists to change the public Turpinverse site theme as part of this feature.

**Acceptance Scenarios**:

1. **Given** the published entities, **When** a reviewer reads the mapping note in repository developer documentation, **Then** they can see how experience/education and projects/achievements could populate a generic profile site’s career and portfolio sections.
2. **Given** the mapping note, **When** a reviewer inspects stored records, **Then** source records are not nested solely in a single consumer’s parameter tree (for example they are not stored only as that consumer’s `experience` / `education` / `projects` / `achievements` params).
3. **Given** this feature is delivered, **When** a reviewer checks the public/reference site, **Then** career and portfolio content appears using the **existing** site theme (no theme switch), and a separate downstream profile-demo site is still not required.

---

### Edge Cases

- What happens when a role’s organisation has no matching canon organisation? Prefer linking an existing organisation when the legend-as-business fit is clear; otherwise store a display name (and optional URL) without a false organisation link.
- What happens when a school is not a canon organisation? Same rule as employers: optional organisation link when the institution already exists; otherwise name and optional URL only. Do not invent a false organisation id.
- How are open-ended roles represented? End date may be absent or marked current; date ranges must still be displayable.
- What happens when a project or achievement image asset is not shipped yet? Placeholder paths or URLs are valid; missing binary assets must not block the records from being complete.
- What happens if a deputy persona is given career or portfolio data? Optional extra subjects are allowed but not required; they must still link to valid persona identities and follow the same entity shapes. On existing person pages, show only the section types that person actually has (for example a deputy who shares a project sees that project; they do not get empty experience or education blocks).
- What happens on a person page with no career or portfolio records? Omit experience, education, projects, and achievements sections entirely. Do not render empty headings or empty lists.
- How are overlapping roles or concurrent education handled? Overlaps are allowed if the CV’s own dates and copy stay internally consistent (no end-before-start; concurrent roles are fine). Display order is still reverse chronological (most recent / current first), not “story order.”
- What if a persona leaves an employer and later returns? Keep a single experience grouping for that organisation; add another role with its own dates. Do not create a second grouping for the same persona and organisation.
- How do career dates relate to historical birth and death years? They do not: experience and education use a contemporary professional overlay. Established life span remains legend/lore and MUST NOT fail a CV date check.
- What happens when a consumer needs a different nesting (for example organisations wrapping jobs)? Mapping may regroup generic roles under organisations; storage must not require that nesting as the only shape.
- Which screens must show the new records? Every existing human-facing channel that already presents personas or their derived person records (public/reference documentation and the in-product web app), as sections on those person pages when the person has records of that type. A new standalone profile-demo site is out of scope. CRM tabular datasets (accounts, deals, cases) need not grow new column types unless a person/contact detail view is the natural place to show career and portfolio.
- What if two personas claim the same project or achievement? That is allowed: projects and achievements are a shared catalog. Each catalog item MUST have at least one persona link; volume gates still count distinct items linked to the primary subject.
- What if a catalog item has no persona links? Treat as incomplete: the automated completeness check MUST fail. Unpublished orphan catalog rows are not in scope.
- How does a project relate to companies? Like a case: required organisation (account) plus persona memberships (contacts). The organisation is the project’s account, not a per-member employer override. Personas need not all list that organisation as their only employer.

## Failure Modes & Error Handling *(required when feature defines external boundaries or user-facing failures)*

This increment extends the published canon and the **existing** places those records are already shown. It does not add a separate downstream profile-demo product.

| Failure Mode | Trigger | Expected Handling | User/System Outcome |
|--------------|---------|-------------------|----------------------|
| Orphan persona link | Experience, education, or a catalog membership points at an unknown persona | Automated completeness check fails and lists the offending record and id | Records are not treated as complete until every subject id resolves |
| Orphan catalog item | Project or achievement has no persona links | Automated completeness check fails | Every catalog piece is attached to at least one valid persona |
| False organisation link | Experience, education, or project claims an organisation id that does not exist | Automated completeness check fails; do not silently drop the id | Reviewer sees an explicit check failure, not a dangling company or school |
| Incomplete required fields | Primary subject missing minimum volumes or required role/education/project/achievement fields | Automated completeness check fails against this feature’s volume rules | Delivery is not accepted until minima in FR-003–FR-008 are met |
| Duplicate experience grouping | Same persona has two experience groupings for one organisation | Automated completeness check fails | Rehire must be extra roles on the single grouping |
| Consumer-shaped source of truth | Records exist only as a single downstream theme’s parameter tree | Out of scope / invalid for this feature | Generic entities must exist independently of any one consumer |

## Security & Access *(required when feature handles sensitive data, authentication, or external exposure)*

This feature adds **fictional** career and portfolio demo content on the same surfaces that already publish other Turpinverse records. It does not introduce authentication or personal data of real people, and it does not add a new public download surface beyond existing channels.

| Asset / Surface | Threat | Access Control | Verification |
|-----------------|--------|----------------|--------------|
| Career and portfolio demo records | Mistaken identity: readers treating invented CVs, schools, or awards as real | Content remains clearly in-universe Turpinverse fiction; no real individuals’ private records | Review copy for real PII; names and institutions stay fictional or already-canon |
| Image paths / URLs | Linking to unexpected external assets | Prefer in-universe placeholder paths; external URLs only as fictional featured links | Mapping note and content review |
| Existing public/reference site and in-product web app | Career and portfolio copy appears wherever other person records already appear | Same access rules as existing canon and demo screens; no new anonymous bulk-download surface | Reviewer can open the same channels used for other records; public site theme is unchanged |

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST treat experience and education as first-class records owned by a single persona, and projects and achievements as first-class **shared catalog** records that MAY be linked to one or more personas — all independent of any one website theme or CRM export shape.
- **FR-002**: The system MUST designate Richard Turpin as the primary profile subject for this feature’s volume and field-coverage rules.
- **FR-003**: The primary subject MUST have experience covering at least three organisations or companies, with one or more roles per organisation grouping (or equivalent nested roles). A persona MUST have at most one experience grouping per organisation; leaving and returning is another role in that same grouping, not a second grouping.
- **FR-004**: Each experience role MUST include a role title, a date range (display-friendly and/or structured start and optional end), and a description. Date ranges MUST use a contemporary professional overlay (present-day or recent years), as if the subject is a living demo professional. Historical birth and death years MUST NOT constrain these dates. The set MUST include at least one example of extra info or tooltip text and at least one example of featured links (`label` or icon, URL, optional tooltip).
- **FR-005**: The primary subject MUST have at least two education entries. Each MUST include a programme or degree title and a school or institution name. Optional fields that MUST be representable: institution URL, optional canon organisation link, date, grade or GPA display, description, and a featured link (name and URL). Education dates, when present, MUST follow the same contemporary overlay as experience. At least one education entry MUST include a featured link.
- **FR-006**: Experience and education SHOULD link to existing canon organisations when the employer or school is already in the universe; display name and optional URL MUST still be available when a dedicated organisation record is not used. Claimed organisation ids MUST resolve.
- **FR-007**: The primary subject MUST be linked to at least three projects. Each project MUST include a stable identity, title, summary or description, image reference (path or URL; placeholders allowed), tags or badges (list of strings), links (`label` or icon plus URL), and a canon **organisation** id (the CRM account for that work). At least one project MUST include a featured call-to-action (name and URL). Optional “featured / show on home” flags MAY be present if useful to more than one consumer.
- **FR-008**: The primary subject MUST be linked to at least four achievements. Each MUST include a stable identity, title, and summary or description. Optional URL and image MUST be representable. The set MUST include at least one achievement with a URL and at least one without a URL.
- **FR-014**: At least one project **or** achievement in the catalog MUST be linked to the primary subject **and** at least one other valid persona, so shared membership is demonstrated rather than only described.
- **FR-009**: Career and portfolio copy MUST follow existing Turpinverse creative rules (legend-as-business, modern professional names, euphemism over explicit criminal language, tongue-in-cheek but demo-credible).
- **FR-010**: The system MUST publish a mapping note in **repository developer documentation** (stub acceptable) showing that these entities can be mapped to a typical personal-site profile’s experience, education, projects, and achievements sections — including optional project detail pages — without storing that consumer’s structures as the source of truth. The public/reference site MUST NOT be required to host this note.
- **FR-011**: Completeness and cross-reference rules for these records MUST be enforced by an **automated completeness check** that fails until they hold: persona links resolve; claimed organisation links resolve; every project has a valid organisation id; every project and achievement has at least one persona link; required volumes and field examples are present; CV date ranges are displayable and do not end before they start; a persona does not have two experience groupings for the same organisation. Human review alone is not sufficient.
- **FR-012**: This feature MUST NOT switch the public/reference site theme, MUST NOT implement a separate downstream profile-demo site, and MUST NOT replace existing CRM sample datasets (accounts, deals, cases).
- **FR-013**: Career and portfolio records MUST be visible on every existing human-facing channel that already presents personas or their derived person records, including the public/reference documentation site and the in-product web application. Reviewers MUST NOT need a new product to inspect the data. Each person page MUST show only the section types that person has at least one record for; empty experience, education, projects, or achievements blocks MUST NOT appear. Dual publication respects each channel's purpose per [`docs/product-surfaces.md`](../../docs/product-surfaces.md): Hugo showcases readable content; Blazor supports exploration ahead of export — not two competing showcase products.
- **FR-015**: On existing person pages, experience MUST list organisations ordered by each grouping’s most recent role, and roles within a grouping MUST appear most-recent-first. Education MUST appear most-recent-first. A missing or current end date counts as more recent than a closed range. Portfolio lists (projects, achievements) have no required date order in this feature.

### Key Entities

- **Persona (existing)**: Profile subject. This feature adds career and portfolio history to existing identities; it does not replace biography, title, or organisation membership fields already in the canon.
- **Organisation (existing)**: Canon company or institution that experience and education SHOULD reference when the employer or school is already in-universe.
- **Experience**: A persona’s association with an employer or organisation, including organisation name, optional URL, optional canon organisation link, and one or more **roles**. Unique per persona per organisation (one grouping; multiple roles cover title changes and rehire).
- **Role**: A job within an experience: title, contemporary date range, description, optional extra/tooltip text, optional featured links.
- **Education**: A programme or qualification owned by one persona: title, school name, optional URL, optional canon organisation link, contemporary date, grade display, description, optional featured link.
- **Project**: A reusable portfolio catalog item (stable identity, title, summary, image reference, tags/badges, links, optional featured CTA, optional home/featured flags) linked to **one canon organisation** (CRM account) and one or more personas (contacts), analogous to a case’s account + contact(s).
- **Achievement**: A reusable honour or result catalog item (stable identity, title, summary, optional URL, optional image) linked to one or more personas.
- **Catalog membership**: The association between a persona and a shared project or achievement. The same catalog item MUST appear consistently for every linked persona. For projects, membership does not change the project’s single organisation (account).
- **Featured link**: A labelled (or icon) URL with optional tooltip, used on roles, education, or projects.
- **Consumer mapping note**: Documentation that describes a *possible* projection onto a profile-site layout; not a stored duplicate of that layout.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A reviewer can assemble a complete primary-subject career narrative (experience plus education) in under five minutes using only the published records, with no missing employer, role, school, or programme required by this spec. Experience and education on person pages appear reverse chronological as in FR-015 so current or latest items are found first.
- **SC-002**: 100% of career and portfolio records that name a persona or canon organisation resolve to an existing identity; every project and achievement has at least one persona membership; zero orphan links. An automated completeness check reports pass only when these rules and the volume/field minima hold, and fail with the offending record ids otherwise.
- **SC-003**: Volume gates hold for the primary subject: at least 3 organisations in experience, 2 education entries, 3 linked projects, and 4 linked achievements, including the required featured-link, CTA, tag, with/without-URL, and at least one catalog item shared with another persona.
- **SC-004**: At least 80% of sampled role titles, project names, and achievement titles (minimum 10 items) are rated plausible for a client-facing professional demo (3/5 or higher) by reviewers who also recognise Turpinverse voice.
- **SC-005**: A reviewer familiar with personal-site profile layouts can map every required generic field to a typical experience, education, project, or achievement slot using only the repository mapping note, in under 15 minutes, without treating any one theme’s parameter tree as the stored source and without needing the public/reference site to host that note.
- **SC-006**: A reviewer who already knows where personas or contacts are shown can find the primary subject’s experience, education, projects, and achievements on those same public/reference and in-product channels in under five minutes, without a theme change or a new profile-demo site. A reviewer opening a persona with no records of a given type MUST NOT see an empty section for that type.

## Assumptions

- Issues #19 and #20 are one feature with two entity groups; they share the same primary subject, tone, and consumer-agnostic rule.
- Richard Turpin is the required primary portfolio subject; one or two deputies with the same entity shapes are optional extras, not part of the volume gates. Deputies MAY share catalog projects/achievements with the primary subject. Person pages omit any career or portfolio section type that person does not have.
- Experience and education remain single-persona records; only projects and achievements are a shared catalog. Projects additionally require one organisation (account), like cases; achievements do not.
- Education institutions follow the same organisation-link rule as employers: link when the school already exists in canon; otherwise name and optional URL.
- Existing Creative Universe Rules from the Turpinverse canon (modern names, legend-as-business, euphemism, demo-credible humour) apply unchanged.
- Descriptions MAY use lightweight markup; summaries MAY be plain text. Exact formatting is a documentation choice, not a consumer-specific template.
- Dates may be stored as display strings and/or structured start/end; both are acceptable if humans can read a range and consistency checks can detect end-before-start when structured dates exist. Career and education dates are a contemporary overlay; they are not required to fit historical persona life spans.
- Placeholder image paths (for example under a projects or achievements images folder on a future consumer site) are sufficient; shipping binary assets with a downstream site is out of scope.
- Mapping to a specific personal-site theme is a documented projection only, not an implementation of that site in this feature. The mapping note lives in repository developer docs, not on the public/reference site. No named CMS or theme is a Turpinverse export requirement.
- CRM contact/account/deal/case **tabular** datasets are unchanged except insofar as new canon entities must remain consistent with existing personas and organisations. Person/contact **detail** views that already show a person MUST also show that person’s career and portfolio where this feature requires publication parity.
- Career and portfolio MUST appear on existing human-facing publication channels (public/reference site and in-product web app) in the same spirit as other records; a separate downstream personal-site or CMS demo remains out of scope. See [`docs/product-surfaces.md`](../../docs/product-surfaces.md) for how dual publication fits the Hugo showcase vs Blazor explore/export split.
- Completeness for this feature is a machine gate (same spirit as existing canon cross-reference rules), not a reviewer-only checklist.
- No new public download or authentication surface is required for this increment.
- English-only copy, consistent with the existing canon.
