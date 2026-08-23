# Feature Specification: Professional Profile Extras

**Feature Branch**: `006-professional-profile-extras`

**Created**: 2026-08-23

**Status**: Draft

**Input**: User description: "Read details from https://github.com/markheydon/turpinverse/issues/22"

**Source issues**:

- [#22 Add professional profile extras for the primary persona](https://github.com/markheydon/turpinverse/issues/22)
- Parent: [#18 Career, portfolio, and content demo data](https://github.com/markheydon/turpinverse/issues/18)
- Related (do not duplicate): [#19 experience/education](https://github.com/markheydon/turpinverse/issues/19), [#20 projects/achievements](https://github.com/markheydon/turpinverse/issues/20)

## Clarifications

### Session 2026-08-23

- Q: When a person page already shows a biography and job title, and that person also has intro and about extras, what should a visitor see? → A: When extras exist, intro is the header and about is the longer narrative; biography and title stay on the record but are not shown as a second competing block
- Q: On an existing person page, where should about, skills, contact, and socials sit relative to the career and portfolio blocks that are already there? → A: Profile sandwich: about, then skills, then existing career/portfolio, then contact, then socials (intro stays the header)
- Q: How many professional extras sets may one persona have? → A: At most one extras set per persona; missing types are simply absent on that set
- Q: For a persona’s skills list, must skill names be unique, and in what order should they appear? → A: Unique names (letter case ignored); show them in stored/author order
- Q: May a persona have two professional socials with the same network name, and in what order should those links appear? → A: Unique network names (letter case ignored); show links in stored/author order

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Intro and hero copy for the primary subject (Priority: P1)

A product demonstrator assembling a profile-style demo needs short **introduction copy** for Richard Turpin: how he is introduced, a headline, a subtitle, an optional photo, and an optional single call-to-action. Career lists already exist; they do not tell a visitor how the subject presents himself at the top of a profile.

**Why this priority**: Without intro copy, existing titles and biographies are not enough for a credible profile header. This slice is independently useful even if about, skills, contact, and socials arrive later.

**Independent Test**: Confirm the primary persona has intro/hero fields covering FR-003, that those fields seed from existing display name, title, and biography where they already exist rather than inventing a disconnected identity, and that the intro copy is used as the header on each existing human-facing person surface (public/reference site and in-product web app) when content is present, without a second competing title line.

**Acceptance Scenarios**:

1. **Given** Richard Turpin as the primary subject, **When** a reviewer opens his professional extras, **Then** they see a short intro, a headline, and a subtitle suitable for a profile header.
2. **Given** existing persona fields (display name, title, biography), **When** a reviewer compares them to the intro copy, **Then** the extras reuse those seeds where they already fit and do not invent a second identity or a conflicting job title.
3. **Given** the intro extras, **When** a reviewer inspects optional presentation fields, **Then** a photo reference MAY be present (placeholder allowed) and at most one call-to-action (label plus URL) MAY be present.
4. **Given** the public/reference site and the in-product web app already show this persona or their derived contact, **When** a reviewer looks up the same person on those channels, **Then** the intro copy is the profile header there (short intro, headline, subtitle) without opening a new product, and the existing job title is not shown as a second competing header line.
5. **Given** a persona with no intro extras, **When** a reviewer opens that person’s existing page, **Then** no empty intro/hero heading or empty block is shown; the existing title display MAY remain.

---

### User Story 2 - About text and skills (Priority: P2)

A demonstrator needs an **about** narrative and a **skills** list for the same primary subject so a profile “about” and “skills” section can be populated without copying job history, education, projects, or awards into this record.

**Why this priority**: Header copy alone does not cover the longer self-description and capability list that profile-style demos typically show. This slice is independently valuable once a primary subject exists.

**Independent Test**: Confirm the primary persona has about text and a named skills list meeting FR-005–FR-006, that about text may start from the existing biography without replacing career/portfolio records, that about is the longer narrative on the same existing person surfaces used for intro copy (biography is not shown as a second competing block when about is present), that about then skills appear before existing career/portfolio blocks, and that empty types are omitted.

**Acceptance Scenarios**:

1. **Given** the primary subject, **When** a reviewer reads the about extras, **Then** they see about text that can stand as a profile “about” section (it MAY start from the existing biography).
2. **Given** the same extras, **When** a reviewer inspects skills, **Then** they see a skills heading and a list of at least five unique skill names in stored/author order (letter case ignored for uniqueness).
3. **Given** existing career and portfolio records for this persona, **When** a reviewer inspects the extras set, **Then** it does not contain duplicate experience, education, project, or achievement lists — those remain the source for jobs, schools, work samples, and awards.
4. **Given** the public/reference site and the in-product web app, **When** a reviewer opens the primary subject on those same channels, **Then** about is the longer self-description immediately under the intro header, skills follow about, and both appear before existing career/portfolio blocks; biography is not shown as a second competing narrative block.
5. **Given** a persona with no about text (or no skills), **When** a reviewer opens that person’s existing page, **Then** that empty type is omitted while types they do have still appear; if about is absent, the existing biography display MAY remain.

---

### User Story 3 - Contact copy and professional socials (Priority: P3)

A demonstrator needs **contact** presentation and a **professional socials** list for Richard: how to get in touch, reuse of existing email (and phone if useful), an optional contact call-to-action, and named network links. Website form endpoints and CMS integration fields are not part of this story.

**Why this priority**: Intro and about still leave “how to reach them” and “where they appear socially” unaddressed. Contact and socials are independently testable once the persona identity exists.

**Independent Test**: Confirm the primary persona has contact copy and a socials list meeting FR-007–FR-008, that email matches the existing persona email, that no form-endpoint or site-chrome fields are stored, that contact then socials appear after existing career/portfolio blocks on the same existing person surfaces, and that empty types are omitted.

**Acceptance Scenarios**:

1. **Given** the primary subject, **When** a reviewer reads contact extras, **Then** they see short contact copy and the existing email address (phone MAY be included if useful).
2. **Given** those contact extras, **When** a reviewer looks for a contact call-to-action, **Then** an optional name-plus-URL MAY be present, and no form-endpoint, CMS, or website-configuration fields are required or stored.
3. **Given** the primary subject, **When** a reviewer lists professional socials, **Then** they see at least three entries with unique network names (letter case ignored) and a URL each, in stored/author order (in-universe links are acceptable).
4. **Given** the public/reference site and the in-product web app, **When** a reviewer opens the primary subject on those same channels, **Then** contact and socials are visible after existing career/portfolio blocks (contact then socials), not mixed into the header or the about/skills band.
5. **Given** a persona with no contact extras (or no socials), **When** a reviewer opens that person’s existing page, **Then** that empty type is omitted.

---

### Edge Cases

- What happens when a deputy persona has no professional extras? Deputies are optional extras, not required to close this feature. Person pages omit any extra type that person does not have.
- What happens when only some extra types are filled (for example intro but no socials)? Show the types that have content; omit the rest. Do not render empty headings or empty lists. Remaining types keep the sandwich order: intro header; about; skills; existing career/portfolio; contact; socials.
- What order do extras use relative to career and portfolio? Profile sandwich on both existing person channels: intro is the header; about then skills; then the existing experience, education, projects, and achievements blocks (their internal order is unchanged); then contact then socials.
- What if intro copy diverges from display name, title, or biography? Seeding is the starting point; extras MAY refine wording for a profile header, but MUST NOT invent a conflicting identity, job, or employer that contradicts existing persona and career records.
- What if about text differs from biography? About MAY expand or reshape biography for a profile “about” section. Biography and title remain on the persona record as seeds; they MUST NOT be shown as a second competing block when intro/about extras are present. About MUST NOT replace career/portfolio lists.
- What if only intro extras exist (no about)? The intro is the header (existing title is not a second competing header). Biography MAY still appear as the longer narrative until about extras exist.
- What if only about extras exist (no intro)? About is the longer narrative (biography is not a second competing block). The existing title display MAY remain as the header until intro extras exist.
- What if a photo asset is not shipped yet? Placeholder paths or URLs are valid; missing binary assets MUST NOT block the extras from being complete.
- What if phone is omitted? Phone is optional. Completeness MUST NOT require a phone number.
- What if a social URL is fictional or in-universe? That is allowed. Links MUST still be well-formed URLs.
- What if someone tries to store site chrome (brand title, favicon, navigation, sticky behaviour, section-enable flags, footer “recent posts” path or count)? Those fields are out of scope and MUST NOT appear on this record.
- What if someone tries to add a second public site, a theme switch, or a product-specific export file for these extras? Out of scope. Publication uses existing person surfaces only. Tabular CRM downloads need not gain new columns unless a later export story asks.
- Which screens must show the extras? Every existing human-facing channel that already presents this persona or their derived person record (public/reference site and in-product web app), as blocks on those person pages when content exists. A new standalone profile-demo site is out of scope.
- How many extras sets may one persona have? At most one. A second extras set for the same persona is invalid. Missing types on that set are omitted on the page; they are not stored as empty parallel records.
- How many people must have extras? Volume gates apply to Richard Turpin only. Filling every persona is out of scope.
- What if two skill names differ only by letter case? Treat them as the same name. Completeness counts distinct names; a duplicate MUST fail the automated check. Skills that remain MUST appear in stored/author order, not alphabetically.
- What if two socials share a network name that differs only by letter case? Treat them as the same network. Completeness counts distinct networks; a duplicate MUST fail the automated check. Socials that remain MUST appear in stored/author order, not alphabetically.

## Failure Modes & Error Handling *(required when feature defines external boundaries or user-facing failures)*

This increment extends the published canon and the **existing** places person records are already shown. It does not add a separate product, theme, or export artifact.

| Failure Mode | Trigger | Expected Handling | User/System Outcome |
|--------------|---------|-------------------|----------------------|
| Orphan persona link | Professional extras point at an unknown persona | Automated completeness check fails and lists the offending record and id | Records are not treated as complete until the subject id resolves |
| Duplicate extras set | More than one extras set names the same persona | Automated completeness check fails and lists the offending persona id | At most one extras set per persona; types are optional parts of that set |
| Duplicate skill name | Skills list contains two names that match when letter case is ignored | Automated completeness check fails | Volume counts distinct names; author order is preserved without repeats |
| Duplicate social network | Socials list contains two network names that match when letter case is ignored | Automated completeness check fails | Volume counts distinct networks; author order is preserved without repeats |
| Incomplete required fields | Primary subject missing intro, about, skills, contact, or socials minima | Automated completeness check fails against this feature’s volume rules | Delivery is not accepted until FR-003–FR-008 hold |
| Duplicate career/portfolio payload | Extras record stores experience, education, projects, or achievements lists | Completeness or review rejects the payload as out of shape | Career and portfolio records remain the only source for those types |
| Website-config fields | Extras include brand, favicon, nav, sticky, section flags, form endpoints, or similar chrome | Treat as invalid for this feature | Generic person presentation data only; no site configuration |
| Empty block shown | A person page renders a heading or list for an extra type with no content | Publication MUST omit that type | Reviewers never see empty intro, about, skills, contact, or socials blocks |
| Consumer-shaped source of truth | Records exist only as a single downstream theme’s parameter tree or CMS export | Out of scope / invalid for this feature | Generic extras must exist independently of any one consumer |

## Security & Access *(required when feature handles sensitive data, authentication, or external exposure)*

This feature adds **fictional** professional presentation content on the same surfaces that already publish other Turpinverse person records. It does not introduce authentication or personal data of real people, and it does not add a new public download surface.

| Asset / Surface | Threat | Access Control | Verification |
|-----------------|--------|----------------|--------------|
| Professional extras (copy, email, optional phone, social URLs) | Mistaken identity: readers treating invented contact details as real | Content remains clearly in-universe Turpinverse fiction; no real individuals’ private records | Review copy for real PII; email, phone, and socials stay fictional or already-canon |
| Photo and URL fields | Linking to unexpected external assets | Prefer in-universe placeholder paths; external URLs only as fictional featured or social links | Content review |
| Existing public/reference site and in-product web app | Extras appear wherever other person records already appear | Same access rules as existing canon and demo screens; no new anonymous bulk-download surface | Reviewer can open the same channels used for other records; no new product or theme |

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST treat professional profile extras (intro/hero copy, about, skills, contact, professional socials) as first-class **person data** linked to a persona identity — not as website configuration, theme parameters, or a CMS export shape. Each persona MUST have at most one extras set. Intro, about, skills, contact, and socials are optional parts of that set; a missing type MUST NOT be modelled as a second extras set.
- **FR-002**: The system MUST designate Richard Turpin as the primary subject for this feature’s volume and field-coverage rules. Deputies MAY have extras but MUST NOT be required to close the feature.
- **FR-003**: The primary subject MUST have intro/hero extras: a short intro, a headline, and a subtitle. Optional fields that MUST be representable: a photo reference (path or URL; placeholders allowed) and a single call-to-action (label plus URL). At most one intro call-to-action is allowed.
- **FR-004**: Intro extras MUST seed from existing persona fields where they already exist (display name, title, biography). They MUST NOT invent a conflicting identity or contradict existing career records. When intro extras are present on a person page, they MUST be the header; the existing title MUST NOT appear as a second competing header line. Display name MAY remain as the identity label. Title remains stored on the persona record.
- **FR-005**: The primary subject MUST have about text suitable for a profile “about” section. About text MAY start from the existing biography and MUST NOT replace or duplicate experience, education, project, or achievement lists. When about extras are present on a person page, they MUST be the longer self-description; biography MUST NOT appear as a second competing narrative block. Biography remains stored on the persona record.
- **FR-006**: The primary subject MUST have a skills heading and a list of at least five skill names. Skills are names only for this feature (no required proficiency scores or nested taxonomies). Skill names MUST be unique when letter case is ignored. Skills MUST appear in stored/author order on person pages (not alphabetically). The volume gate counts distinct names.
- **FR-007**: The primary subject MUST have short contact copy and MUST reuse the existing persona email. Phone MAY be included. An optional contact call-to-action (name plus URL) MUST be representable. Form-endpoint, CMS, and other website-integration fields MUST NOT be stored.
- **FR-008**: The primary subject MUST have at least three professional socials, each with a network name and a URL. In-universe links are allowed. Network names MUST be unique when letter case is ignored. Socials MUST appear in stored/author order on person pages (not alphabetically). The volume gate counts distinct networks.
- **FR-009**: Professional extras copy MUST follow existing Turpinverse creative rules (legend-as-business, modern professional names, euphemism over explicit criminal language, tongue-in-cheek but demo-credible).
- **FR-010**: Completeness and cross-reference rules for these extras MUST be enforced by an **automated completeness check** that fails until they hold: the extras link resolves to a valid persona; at most one extras set exists per persona; required volumes and field examples for the primary subject are present; skill names on a set are unique when letter case is ignored; social network names on a set are unique when letter case is ignored; extras do not carry website-config or career/portfolio duplicate lists. Human review alone is not sufficient.
- **FR-011**: This feature MUST NOT add website-chrome fields (brand title, favicon, navigation, sticky behaviour, section-enable flags, footer “recent posts” path or count), MUST NOT switch the public/reference site theme, MUST NOT implement a second public site, and MUST NOT add a product-specific export file or new tabular download columns unless a later export story asks.
- **FR-012**: Professional extras MUST be visible on every existing human-facing channel that already presents this persona or their derived person record, including the public/reference documentation site and the in-product web application. Reviewers MUST NOT need a new product to inspect the data. Each person page MUST show only the extra types that person has content for; empty intro, about, skills, contact, or socials blocks MUST NOT appear. When intro extras are present they occupy the header; when about extras are present they occupy the longer narrative — existing title and biography MUST NOT be shown as competing duplicate blocks in those cases. Dual publication respects each channel’s purpose per [`docs/product-surfaces.md`](../../docs/product-surfaces.md): the public/reference site showcases readable content; the in-product app supports exploration ahead of export — not two competing showcase products.
- **FR-013**: On those person pages, extra types that have content MUST appear in this sandwich order: intro as header; about; skills; then existing career and portfolio blocks (experience, education, projects, achievements — internal ordering unchanged); then contact; then professional socials. Omitted empty types MUST NOT leave a gap that reorders the remaining types.

### Key Entities

- **Persona (existing)**: Profile subject. Existing display name, title, biography, and email are seeds and remain stored. This feature does not delete those fields or career/portfolio history. On person pages, present intro/about extras instead of a second competing title or biography block when those extras exist.
- **Professional extras**: At most one person-linked presentation set per persona: intro/hero copy, about, skills, contact, and professional socials. Sits **beside** experience, education, projects, and achievements — it does not contain them. Absent types are omitted, not extra records.
- **Intro / hero copy**: Short intro, headline, subtitle, optional photo reference, optional single call-to-action.
- **About**: Longer self-description for a profile “about” section; may start from biography.
- **Skills**: A heading plus a list of unique skill names (letter case ignored). Display order is stored/author order.
- **Contact extras**: Short contact copy, existing email, optional phone, optional call-to-action. No form endpoint.
- **Professional social**: A named network plus URL. Network names are unique when letter case is ignored. Display order is stored/author order.
- **Featured / contact call-to-action**: A labelled URL used at most once on intro and optionally on contact.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A reviewer who already knows where the primary subject is shown can find intro copy, about, skills, contact, and professional socials on those same public/reference and in-product person surfaces in under five minutes, without a theme change or a new profile-demo site. On those pages the reviewer sees the sandwich order: intro header, about, skills, existing career/portfolio, contact, socials.
- **SC-002**: 100% of extras sets that name a persona resolve to an existing identity, and no persona has more than one extras set. An automated completeness check reports pass only when these rules and the primary-subject field minima hold, and fail with the offending record ids otherwise.
- **SC-003**: Volume gates hold for the primary subject: intro (short intro, headline, subtitle), about text, a skills heading with at least five unique skill names (letter case ignored) in stored/author order, contact copy plus existing email, and at least three uniquely named social links (letter case ignored) in stored/author order.
- **SC-004**: A reviewer opening a persona with no extras of a given type MUST NOT see an empty section for that type. For the primary subject, all five types (intro, about, skills, contact, socials) are present and visible. On that subject’s person pages, a reviewer MUST NOT see the stored title as a second header line beside intro, nor biography as a second narrative beside about.
- **SC-005**: A reviewer comparing extras to career/portfolio records confirms in one pass that jobs, education, projects, and awards are still sourced only from those existing records — not copied into extras.
- **SC-006**: A reviewer scanning stored extras finds zero website-chrome fields (brand, favicon, navigation, sticky behaviour, section flags, recent-posts footer) and zero form-endpoint or CMS-integration fields.

## Assumptions

- GitHub issue #22 is the source of scope; comments narrowing the story to generic person presentation (not a named CMS/theme export, not site chrome, Richard only) are in force.
- Richard Turpin is the required primary subject; deputies with the same extra shapes are optional and are not part of the volume gates. A deputy who has extras still uses at most one extras set.
- Existing career and portfolio records (experience, education, projects, achievements) remain the source for those types. This feature only adds presentation extras beside them.
- Existing Creative Universe Rules from the Turpinverse canon apply unchanged.
- Placeholder photo paths are sufficient; shipping binary assets is out of scope.
- Phone is optional. Social URLs may be in-universe. Social network names are unique when letter case is ignored and appear in stored/author order.
- Skills are a flat list of unique names (letter case ignored) in stored/author order; ranking, years of experience per skill, and endorsement graphs are out of scope.
- Publication follows [`docs/product-surfaces.md`](../../docs/product-surfaces.md): readable on existing person surfaces; no new product; no new tabular download columns unless a later export story asks. When intro/about extras exist, they are the visitor-facing header and longer narrative; stored title and biography are not shown as competing copy. Extra body sections follow the sandwich order in FR-013; career/portfolio internal order is unchanged.
- Completeness for this feature is a machine gate (same spirit as existing canon cross-reference rules), not a reviewer-only checklist.
- No new public download or authentication surface is required for this increment.
- English-only copy, consistent with the existing canon.
- Mapping these extras to a typical personal-site layout MAY be noted later in developer documentation; it is not required to close this feature (unlike career/portfolio, which already has a mapping note).
