# Feature Specification: Human-Facing Channel Alignment

**Feature Branch**: `004-hugo-blazor-charter`

**Created**: 2026-08-22

**Status**: Draft

**Input**: User description: "Bring Hugo and Blazor up to date with the Human-Facing Channel Charter (`docs/product-surfaces.md` / PR 24): Hugo must showcase all human-readable demo data without export plumbing or technical IDs as primary content; Blazor remains explore/filter/download."

**Channels affected**: Public reference site (showcase / read) and interactive export app (explore / filter / download). Shared canon remains the single source of truth. This feature implements Constitution Principle IX and the operational charter in `docs/product-surfaces.md`.

## Clarifications

### Session 2026-08-22

- Q: When someone filters a dataset in the export app and then downloads, should the file contain only the filtered rows? → A: Yes. Preview and download both use the current filters (empty match → block or warn; no silent empty file).
- Q: How should a first-time visitor discover deals and cases on the public reference site? → A: Dedicated readable indexes and detail pages, linked from primary navigation or home (same discoverability as people, organisations, and timeline).
- Q: On public-site pages, what must never appear as the main way a reader identifies a record? → A: Ban join keys and export column names as visible identity (contactId-style tokens, foreign-key strings, import column headers). Display names, titles, and demo emails OK; slugs OK in URLs only.
- Q: Should people, organisations, and timeline pages also list related deals and cases as named links? → A: Bidirectional named links wherever the shared data has a relationship (person, organisation, and timeline pages list related deals/cases; deal/case pages list related people/orgs/events).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Read the full universe on the public site (Priority: P1)

A visitor opens the public reference site to reuse Turpinverse as demo material (CRM samples, presentations, or amusement). They can browse every kind of demo data a person would want to *read*: people, organisations, timeline, career and portfolio, and the stories behind pipeline deals and support cases. Pages use names, relationships, and narrative copy. They do not have to run software or interpret import-column names.

**Why this priority**: The charter’s known gap is that deals and cases exist only as tabular export data, so the public site under-represents the dataset. Completing the readable showcase is the primary user-facing outcome.

**Independent Test**: Open the public site with no export app running. Confirm every human-readable entity type in the shared dataset has a browsable, linkable page (or equivalent readable view) and that a first-time visitor can follow people ↔ organisations ↔ events ↔ deals/cases using display names.

**Acceptance Scenarios**:

1. **Given** the public site is published from current canon, **When** a visitor looks for people, organisations, timeline, and career/portfolio material, **Then** they find complete, readable pages for those records (not empty sections or “see the export app” as the only view).
2. **Given** the shared dataset includes deals and cases, **When** a first-time visitor looks for those records the same way they look for people or organisations, **Then** they find dedicated readable indexes and detail pages linked from primary navigation or home, presenting names, status/stage, amounts or priority where relevant, descriptions, and named related people and organisations (not only a spreadsheet of import columns).
3. **Given** a persona page, **When** the visitor reads career history and portfolio, **Then** that material appears as part of the showcase (same underlying records as the export app’s contact exploration, presented for reading).
4. **Given** a person, organisation, timeline, deal, or case page, **When** the shared data records a relationship, **Then** the visitor sees display-name links in both directions (for example a persona lists related deals/cases; a deal lists related people and organisations).

---

### User Story 2 - Public site stays a reader, not an importer (Priority: P1)

A visitor who only wants to *read* never hits download buttons, machine-oriented preview tables, API-key instructions, or identifier columns as the main content. If they need files they can import, the site points them to the interactive export app (for example via Getting Started) rather than offering import tooling on the public site itself.

**Why this priority**: Channel drift is the failure mode the charter exists to prevent. Showcase completeness without this split would duplicate the export app.

**Independent Test**: Walk every primary public-site section a visitor uses to consume demo data. Confirm there is no export/download/import workflow on those pages, and that technical identifiers are not presented as primary content.

**Acceptance Scenarios**:

1. **Given** a visitor is on a showcase page (home, people, organisations, timeline, deals, cases, persona/org detail), **When** they look for a way to download importable files, **Then** they do not find download or export controls on that page; they may find a short pointer to the export app.
2. **Given** body copy, headings, and relationship lists on those pages, **When** a non-technical reader scans them, **Then** records are identified by display names (and titles or demo emails where relevant), not by join keys, foreign-key tokens, or export column names. URL slugs may appear in the address bar only.
3. **Given** a visitor needs importable files, **When** they follow the site’s guidance for that job, **Then** they are directed to the interactive export app, not given a second importer on the public site.

---

### User Story 3 - Explore, filter, and download in the export app (Priority: P2)

Someone who wants to populate another system opens the interactive export app. They can browse CRM-shaped datasets, narrow what they need, inspect a contact (including career and portfolio as exploration-before-export), and download files that still contain the technical identifiers and cross-references importers require.

**Why this priority**: The export app already owns this job; this story locks the charter split and closes the intended filter/facet gap so “explore / filter / download” is actually true, without turning the app into a second public encyclopedia.

**Independent Test**: With the export app running, complete browse → optional filter → preview → download for contacts, accounts, deals, and cases, and open at least one contact detail that includes career/portfolio exploration.

**Acceptance Scenarios**:

1. **Given** the export app is running, **When** a user opens contacts, accounts, deals, or cases, **Then** they see an interactive preview of that dataset and can download it in the current importable format with identifiers and cross-references intact.
2. **Given** a dataset list that is larger than a handful of rows, **When** the user applies filters and downloads, **Then** they can filter or facet by fields that matter for import (for example stage, status, organisation, or similar) and the downloaded file contains **only** the rows matching the current filters (unfiltered view downloads the full dataset).
3. **Given** a user opens a contact’s detail, **When** they explore before export, **Then** they see career and portfolio alongside CRM fields, and technical identifiers remain available where they help join records for import.
4. **Given** the public site exists, **When** a user uses the export app, **Then** it does not replace the public site as the primary readable reference (no requirement to host the full narrative encyclopedia inside the app).

---

### Edge Cases

- A deal or case references a person or organisation that has no showcase page: the readable view still shows a human-readable name (or a clear “unknown related party” treatment) and does not dump a raw identifier as the only label.
- Canon adds a new human-readable entity type later: the public site is expected to grow; until then, this feature’s completeness bar is the entity types named in the charter (people, organisations, timeline, career/portfolio, deals, cases).
- URL paths may use stable slugs for linking; those slugs must not be presented as the reader’s primary way to understand who or what a record is (address bar only, not as the visible name).
- Demo emails and job titles on persona pages are allowed as human-readable demo fields; they are not treated as technical identifiers.
- The export app is unavailable (not running locally): the public site remains fully usable for reading; pointers to the app explain that download requires running it.
- Empty filter results: the export app tells the user nothing matched; a download in that state is blocked or warned and MUST NOT silently emit an empty file or the unfiltered full dataset.
- Dual publication: career/portfolio on a persona page and on contact detail may differ in layout but must not contradict facts (same underlying records).
- Missing relationship: if the shared data has no deal/case for a person, that page simply omits the empty list (or an equivalent empty state) rather than showing join keys.

## Failure Modes & Error Handling *(required when feature defines external boundaries or user-facing failures)*

| Failure Mode | Trigger | Expected Handling | User/System Outcome |
|--------------|---------|-------------------|----------------------|
| Showcase page cannot resolve a related record | Broken or missing link in shared data | Show a readable fallback (name if known, otherwise a generic missing-related-record message); do not show a raw technical ID as the only label | Reader still understands the page; maintainers can detect the gap |
| Export app cannot load the dataset | Shared data missing or invalid at runtime | Surface a clear load failure; do not offer a download that would be empty or corrupt without saying so | User knows to retry or report; no silent empty file |
| Download requested when a filter matches nothing | User filters to zero rows then downloads | Block or clearly warn; do not silently emit an empty file or substitute the unfiltered full dataset | User can clear filters or abandon download |
| Public site generated from stale or partial data | Generation skipped or failed for a section | Incomplete sections must not ship as if complete; prefer omitting a section with an honest gap over publishing identifier dumps | Readers are not misled about coverage |
| Visitor follows an export pointer but the app is not running | Public site used without local app | Pointer copy explains the app is run locally for download; reading still works | No broken in-page download |

## Security & Access *(required when feature handles sensitive data, authentication, or external exposure)*

All content is **fictional demo data** intended for public reuse. There is no real-person confidential information in scope.

| Asset / Surface | Threat | Access Control | Verification |
|-----------------|--------|----------------|--------------|
| Public reference site | Readers treat technical IDs or emails as real credentials / PII | Keep identifiers out of primary reader copy; fiction framing remains visible | Review showcase pages for ID-as-content and for “this is demo data” framing |
| Public reference site | Accidental exposure of export/import mechanics that look like a live CRM | No download/import tooling on showcase pages | Walk primary pages; only pointers to the local export app |
| Interactive export app (local) | Downloaded files contain join keys that could be mistaken for production IDs | App and file context remain “demo / import sample”; no production auth or live customer data | Confirm downloads are labelled/used as demo imports |
| Shared canon | Tampering would pollute both channels | Existing project controls for who can change canon; this feature does not add public write access | No new unauthenticated write surface |

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The public reference site MUST showcase all human-readable demo data defined by the channel charter: personas (biographies, relationships, aliases), organisations (profiles and org relationships), timeline events, career history and portfolio on persona pages, and human-readable views of deals and cases.
- **FR-011**: Deals and cases MUST have dedicated readable index and detail pages, linked from the public site’s primary navigation or home with the same discoverability as people, organisations, and timeline. Reaching them only via related-record asides is not sufficient.
- **FR-012**: Wherever the shared data records a relationship among people, organisations, timeline events, deals, and cases, the public site MUST present it as bidirectional display-name links (person/organisation/timeline pages list related deals and cases; deal and case pages list related people, organisations, and events). Empty relationship sets MUST NOT be filled with join keys.
- **FR-002**: Human-readable deal and case views MUST present narrative and relationship content (names, descriptions, stage/status, and other reader-relevant facts) rather than only a spreadsheet of import columns.
- **FR-003**: Reader-facing body copy, headings, and relationship lists on the public site MUST identify records by display names (and titles or demo emails where relevant). They MUST NOT use join keys, foreign-key tokens, or export column names as the visible identity of a record. Stable URL slugs MAY exist for linking but MUST NOT replace the display name on the page.
- **FR-004**: The public site MUST NOT offer download, import-file, credential, or machine-oriented preview-table workflows on showcase pages. It MAY point readers who need importable files to the interactive export app.
- **FR-005**: The interactive export app MUST remain the surface for exploring CRM datasets (contacts, accounts, deals, cases), filtering or faceting those lists, inspecting contact detail (including career and portfolio), and downloading importable files with technical identifiers and cross-references intact. Preview and download MUST use the same current filters; an unfiltered view downloads the full dataset.
- **FR-010**: If the current filters match zero rows, the export app MUST tell the user nothing matched and MUST block or warn on download rather than silently writing an empty file or the unfiltered dataset.
- **FR-006**: Career and portfolio records MUST remain dually published: readable on public persona pages and available on export-app contact detail for exploration before export, without contradictory facts.
- **FR-007**: Specs, plans, and user-facing channel copy produced or updated for this feature MUST name which channel is affected and MUST NOT recast the public site as an importer or the export app as the primary readable encyclopedia.
- **FR-008**: When a showcase page cannot resolve a related party, the site MUST show a human-readable fallback rather than a raw identifier as the only label (see Failure Modes).
- **FR-009**: Dataset previews in the export app MAY show identifier columns because they serve import; the public site MUST NOT treat those columns as showcase content.

### Key Entities

- **Persona (contact)**: A fictional person; biography, aliases, relationships, career, and portfolio for reading; CRM contact fields and IDs for export.
- **Organisation (account)**: A fictional company or group; profile and membership for reading; CRM account fields and IDs for export.
- **Timeline event**: A dated story beat linking people and organisations for reading.
- **Career and portfolio records**: Experience, education, projects, and achievements belonging to a persona; shown on both channels for different jobs.
- **Deal**: A pipeline story attached to people and organisations; readable on the public site; tabular with IDs in the export app.
- **Case**: A support/issue story attached to people and organisations (and optionally a timeline event); readable on the public site; tabular with IDs in the export app.
- **Importable dataset**: A downloadable projection of canon for external systems; exists only on the export-app channel.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A first-time visitor can reach readable views for **100%** of charter-listed human-readable types (people, organisations, timeline, career/portfolio, deals, cases) from the public site’s **primary navigation or home**, without starting the export app. Deals and cases are first-class sections, not only in-page asides.
- **SC-002**: In a review of all primary showcase pages, **zero** pages present download/import controls, and **zero** pages use join keys, foreign-key tokens, or export column names as the visible identity of people, organisations, deals, or cases (URL paths excepted). Display names, titles, and demo emails may appear.
- **SC-003**: **90%** of reviewers who are given the two-channel description correctly choose the public site for “read the universe” and the export app for “download data to import,” on the first attempt.
- **SC-004**: A user who knows which dataset they want can browse, apply at least one filter, and obtain an importable download of **only the matching rows** in **under 3 minutes** in the export app.
- **SC-005**: Spot-check of **10** persona pages and matching contact details shows career/portfolio facts that agree (same roles, dates, and achievements) even if layout differs.
- **SC-006**: After this feature, the charter’s “public-site pages for deals and cases — not built” and “technical IDs in public-site body copy” gaps are **closed** for current canon (or explicitly listed as remaining with owner and reason — target is closed).

## Assumptions

- “Hugo” and “Blazor” in the request name the existing public reference site and local interactive export app; this spec constrains **channel jobs**, not a particular theme, generator, or UI toolkit.
- Completing **human-readable deals and cases** on the public site is in scope, including **dedicated indexes and detail pages** linked from primary navigation or home (clarified 2026-08-22).
- Completing **filter/facet** on export-app dataset lists is in scope. **Preview and download both apply the current filters** (clarified 2026-08-22).
- **Bidirectional named links** among people, organisations, timeline, deals, and cases are in scope wherever the shared data has a relationship (clarified 2026-08-22).
- New import formats beyond the current downloadable format are **out of scope**.
- Changing the fictional canon’s plot or inventing new story entities is **out of scope** unless required to give deals/cases readable names and descriptions that already exist in data.
- Pointers from the public site to the export app (Getting Started, short home-page notes) are allowed and are not “export plumbing.”
- URL slugs used for routing are not “technical IDs as primary content” unless they are shown as the visible name of a person or organisation. **Join keys and export column names** are the banned identity form; demo emails and titles are allowed (clarified 2026-08-22).
- `docs/product-surfaces.md` remains the operational charter; this feature must not contradict it.

## Out of Scope

- Replacing the public site with the export app (or vice versa).
- Public-site CSV/API download, API keys, or CRM-import wizards.
- Using the export app as the canonical narrative encyclopedia for the universe.
- Authentication, multi-user accounts, or hosting the export app as a public SaaS.
- Historical accuracy of the Dick Turpin legend.
