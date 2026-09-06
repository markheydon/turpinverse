# Feature Specification: Generic Article and Gallery Demo Data

**Feature Branch**: `005-article-gallery-content`

**Created**: 2026-08-22

**Status**: Draft

**Input**: User description: "Read details from https://github.com/markheydon/turpinverse/issues/21 — Add generic blog and gallery content demo data. Demo sites need articles and image galleries as generic content types. Turpinverse owns reusable fixtures; any CMS or static site can map them later. No consumer’s blog path, gallery layout key, or front matter is the stored format. Parent: #18."

**Source issues**:

- [#21 Add generic blog and gallery content demo data](https://github.com/markheydon/turpinverse/issues/21)
- Parent: [#18 Career, portfolio, and content demo data](https://github.com/markheydon/turpinverse/issues/18)

**Channels affected**: **Public reference site (showcase / read)** only for human-facing publication. Articles and galleries are human-readable content; when shown to readers they belong on the public showcase, not as interactive-export tabular datasets. Shared canon remains the single source of truth. This feature respects Constitution Principle IX and [`docs/product-surfaces.md`](../../docs/product-surfaces.md): reader-facing pages MUST NOT present technical identifiers or export plumbing as primary content. The interactive export app is **not** required to grow CSV datasets, filters, or download flows for these types.

## Clarifications

### Session 2026-08-22

- Q: How should a first-time visitor find articles and the gallery on the public reference site? → A: Dedicated article list and gallery page(s), linked from primary navigation or home (same discoverability as people, organisations, and deals).
- Q: How should draft versus published articles count toward the “at least three articles” rule? → A: All three required articles are published; a draft fixture is not required.
- Q: Must any article name a Turpinverse persona as its author? → A: Every article must name an existing persona as author.
- Q: Must any articles share a collection or section grouping? → A: Every article must belong to a collection (one or more groupings allowed).
- Q: Besides the article list and article pages, where should a visitor see that a persona wrote these pieces? → A: Article pages name the author as a person, and that person’s existing page lists their articles (omit the list if they have none).
- Q: How many published articles, and how should authorship and topics be distributed? → A: Ten published articles from the Turpin Enterprises team. Three authored by Richard Turpin; the other seven by any other Turpin Enterprises members. One article is about the Black Bess Route Optimiser project; two are about the same one of the three support cases linked to Turpin Enterprises; the remaining seven are thought leadership on topics related to the author’s job role.
- Q: What should the required gallery be a set of pictures of? → A: Turpin Enterprises team or workplace pictures (people, office, brand).
- Q: When a visitor opens an article about the Black Bess project or the chosen support case, must they see a named link to that project or case page? → A: Yes — those article pages must show a named link to the related project or case page.
- Q: How should a visitor see article collections on the public reference site? → A: Visible label on the article list and article pages only (no collection index pages).
- Q: Must each gallery image include a short caption or alternative description a visitor can read? → A: Every gallery image must have a caption or alternative description.
- Q: Should the existing Turpin Enterprises organisation page link to this team or workplace gallery? → A: No — gallery discovery stays on the dedicated gallery page(s) from nav or home.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Reusable in-universe articles (Priority: P1)

A product demonstrator building a blog, news, or thought-leadership demo needs a **Turpin Enterprises team blog**: ten published **article** records they can reuse in any site or CMS. Pieces read as if written by company members — three by Richard Turpin, seven by other people on that organisation — covering the Black Bess Route Optimiser, one of the firm’s support cases, and role-shaped thought leadership. Each has a publish date, body copy in Turpinverse voice, plus optional tags, featured image, excerpt, and presentation hints such as table-of-contents — without Turpinverse storing any one theme’s blog folder layout or front-matter keys as the source of truth.

**Why this priority**: Articles are the minimum generic content type this story exists to add. Without them, career, CRM, and persona showcase pages still leave a typical “blog” demo empty.

**Independent Test**: Confirm ten published article records meet field, author-split, topic, and metadata coverage (FR-003–FR-005, FR-016), completeness checks pass, copy is in-universe and distinct from existing showcase page types, and a reviewer can list and open them as generic content without a consumer-specific file tree.

**Acceptance Scenarios**:

1. **Given** Turpinverse demo data after this feature, **When** a reviewer lists articles, **Then** they find **ten** distinct **published** article records, each with a title, a publish date, a draft flag set to not-draft, a body, an author who is a Turpin Enterprises member, and a collection grouping.
2. **Given** those ten articles, **When** a reviewer checks bylines, **Then** exactly three are authored by Richard Turpin and the other seven by other Turpin Enterprises members (not Richard).
3. **Given** those ten articles, **When** a reviewer checks subjects, **Then** one is about the Black Bess Route Optimiser project, two are about the same one of the three support cases linked to Turpin Enterprises, and the remaining seven are thought leadership related to each author’s job role.
4. **Given** those articles, **When** a reviewer inspects optional fields, **Then** at least one article includes tags, a featured image reference, a description or excerpt, and a table-of-contents presentation hint.
5. **Given** a reviewer familiar with Turpinverse tone, **When** they read titles and bodies, **Then** pieces read as in-universe team thought leadership (not lorem ipsum, not a dump of highwayman trivia, and not a copy of an existing persona, organisation, timeline, deal, case, or project page — related links to the project or case are allowed).
6. **Given** stored records, **When** a reviewer looks for a consumer blog path or theme front-matter shape as the only stored form, **Then** they do not find it: articles exist as generic content independent of any one CMS or theme.

---

### User Story 2 - Reusable image gallery (Priority: P2)

A demonstrator needs at least one **gallery** — a titled, ordered set of **Turpin Enterprises team or workplace** images (people, office, or brand) with an optional description and a generic viewer or lightbox preference — so a photo or lightbox demo can be populated without adopting a particular theme’s gallery layout keys.

**Why this priority**: Galleries are a second generic content type; they are independently valuable once articles exist, and they are not implied by article featured images alone.

**Independent Test**: Confirm at least one gallery meets volume, **team/workplace subject**, caption, and viewer-preference rules (FR-006–FR-007), image references are inspectable, and completeness fails if the gallery is below the minimum image count, missing captions, off-theme, or missing required fields.

**Acceptance Scenarios**:

1. **Given** Turpinverse demo data after this feature, **When** a reviewer opens galleries, **Then** they find at least one gallery with a title and an ordered list of at least four images of Turpin Enterprises people, office, or brand, each identified by a source path or URL and each with a caption or alternative description.
2. **Given** that gallery, **When** a reviewer inspects presentation hints, **Then** a viewer or lightbox preference is enabled, with optional viewer options expressed as generic hints (not a named theme’s layout keys).
3. **Given** gallery copy and image choices, **When** a reviewer checks branding and tone, **Then** the set reads as Turpin Enterprises team or workplace photography (placeholder or remote images are allowed), is consistent with Turpinverse fiction, and is not a product-shot stand-in for the Black Bess project page or a copy of another showcase page’s primary purpose.

---

### User Story 3 - Public showcase of articles and galleries (Priority: P3)

A visitor to the public reference site can read articles and browse the gallery as **reader-facing showcase content**, on pages that are distinct from existing people, organisations, timeline, deals, cases, and project showcase routes. They do not encounter export, download, or import plumbing. Mapping these records onto a later CMS or static-site blog remains a possible projection, not a stored format and not a delivery requirement for a named theme.

**Why this priority**: Canon-only records would not meet the channel charter for human-readable content. Publication is independently testable once P1 and P2 records exist.

**Independent Test**: Open the public reference site (no export app required). Confirm a dedicated article list and gallery page(s) are linked from primary navigation or home, detail pages are readable and distinct from existing showcase types, article pages name the author as a person, that author’s existing person page lists their published articles, the project and case articles show a named link to the related project or case page, the article list and article pages show each piece’s collection as a visible label (no collection index pages), there is no export UI, and repository notes (if any) do not make a named CMS layout a completeness requirement.

**Acceptance Scenarios**:

1. **Given** the public reference site is generated from current shared data, **When** a first-time visitor looks for articles and galleries the same way they look for people, organisations, or deals, **Then** they find a dedicated article list and gallery page(s) linked from primary navigation or home, plus reader-facing detail views — not a message that these exist only inside an export tool.
2. **Given** those views, **When** a visitor compares routes and page purpose to people, organisations, timeline, deals, cases, and projects, **Then** article and gallery pages are distinct unless a piece is intentionally reused as a related link (the primary article/gallery pages are not aliases of those existing types).
3. **Given** a visitor who only wants to read, **When** they open article or gallery pages, **Then** they see titles, dates, copy, and images — not technical identifiers, filter builders, or file-download workflows as primary content. On the gallery, each image’s caption or alternative description is visible as reader-facing text.
4. **Given** this feature is delivered, **When** a reviewer checks acceptance, **Then** there is no requirement that Turpinverse emit a particular CMS or theme’s blog or gallery file layout.
5. **Given** a published article, **When** a visitor opens its reader-facing page, **Then** they see the author named as a person (not only a raw identity token).
6. **Given** a persona who authored one or more published articles, **When** a visitor opens that person’s existing public-site page, **Then** they see those articles listed. **Given** a persona who authored none, **When** a visitor opens their page, **Then** no empty articles heading or empty list appears.
7. **Given** the Black Bess project article or either case article, **When** a visitor opens its reader-facing page, **Then** they see a named link to that project or that support case page (not only a hidden data join). **Given** a project or case page, **When** a visitor opens it, **Then** this feature does not require those pages to list the articles.
8. **Given** the article list or any article detail page, **When** a visitor looks for grouping, **Then** they see that article’s collection as a visible human-readable label. **Given** this feature is delivered, **When** a reviewer looks for dedicated collection index pages, **Then** none are required and none are added as a new primary browse path.
9. **Given** the Turpin Enterprises organisation page, **When** a visitor looks for the team or workplace gallery, **Then** this feature does not require a named gallery link on that page. Discovery remains the dedicated gallery page(s) from primary navigation or home.

---

### Edge Cases

- What happens when an article is marked draft? If a draft exists, it remains a valid generic record for completeness of *shape*, but MUST NOT appear as a published reader-facing showcase page. This feature does not require shipping a draft; the ten required articles are published.
- What happens when an article has a table-of-contents hint but the body has no headings? The record is still valid; the hint is optional metadata, not a guarantee of a generated outline.
- What happens when a featured image or gallery image asset is not shipped as a binary in this repository? Placeholder paths or remote URLs are valid; missing binaries MUST NOT block completeness if the reference is present.
- What happens when a gallery has fewer than four images? The automated completeness check MUST fail until the minimum holds.
- What happens when a gallery image has a source but no caption or alternative description? Not allowed: every gallery image MUST include a human-readable caption or equivalent alternative description. Completeness MUST fail if any slot omits it.
- What if the gallery is product shots of Black Bess Route Optimiser, a support-case collage, or unrelated placeholders with no team/workplace subject? Not allowed for the required gallery: it MUST depict Turpin Enterprises people, office, or brand. A product or case image MAY appear only as incidental workplace context, not as the gallery’s primary purpose.
- What happens when two articles share a title? Allowed if each has a distinct stable identity; human-facing lists SHOULD still be distinguishable (for example by date or identity).
- What happens when collection or section grouping is omitted? Not allowed for this feature: every article MUST belong to a collection. Collections are generic labels, not CMS paths. Multiple collections are allowed; a single collection covering all required articles is sufficient. On the public site, the collection MUST appear as a visible label on the article list and on each article page. Dedicated collection index or section pages MUST NOT be added in this feature.
- What happens if someone expects these types in the interactive export app as downloadable tables? Out of scope: this feature MUST NOT treat articles or galleries as export datasets.
- How should author be represented? Every article MUST name an existing persona as author, and that persona MUST be a member of Turpin Enterprises. Display name may be derived from that persona. Omitting author, using a name with no persona, pointing at an unknown persona, or using an author who is not a Turpin Enterprises member MUST fail completeness. Article pages MUST show that person as the author. The author’s existing public-site person page MUST list their published articles; if they have none, omit the list entirely.
- Who may write the seven non-Richard articles? Any Turpin Enterprises member except Richard Turpin. The same person MAY write more than one of those seven. Richard MUST NOT be the author of those seven.
- Which support case do the two case articles cover? Exactly one of the three support cases whose account is Turpin Enterprises. Both articles MUST be about that same chosen case (not two different cases).
- Must the project and case articles duplicate those showcase pages? No. They are distinct articles that MUST relate to the existing project or case record. On the public site, those article pages MUST show a **named** link to the related project or case page. They MUST NOT replace the project or case page.
- Must organisation or other related-record pages list articles? No. This feature does not require organisation, timeline, deal, case, or project pages to list articles. Thought-leadership articles (the seven role-topic pieces) are not required to show a project or case link.
- Must the Turpin Enterprises organisation page link to the gallery? No. Gallery discovery is the dedicated gallery page(s) linked from primary navigation or home. An organisation-page gallery link is not required.
- What if a math or special-rendering flag is unused? The field MAY be omitted; at least the ability to represent it is required. Shipping an article that uses it is optional.

## Failure Modes & Error Handling *(required when feature defines external boundaries or user-facing failures)*

This increment adds generic content records and, when published, reader-facing showcase pages. It does not add an export dataset or a named CMS product.

| Failure Mode | Trigger | Expected Handling | User/System Outcome |
|--------------|---------|-------------------|----------------------|
| Incomplete article set | Fewer than ten published articles, or a required field missing | Automated completeness check fails and names the gap | Delivery is not accepted until volume and required fields hold |
| Author split or org mismatch | Fewer or more than three articles by Richard Turpin; a non-Richard slot authored by Richard; or any author who is not a Turpin Enterprises member | Automated completeness check fails | All ten pieces read as the Turpin Enterprises team blog |
| Topic mix missing | No article related to Black Bess Route Optimiser; fewer or more than two articles related to one Turpin Enterprises support case; or those two related to different cases | Automated completeness check fails | Project, case, and role topics are present as specified |
| Missing collection | Article has no collection grouping | Automated completeness check fails | Every article belongs to a generic collection |
| Incomplete gallery | No gallery, or a gallery with fewer than four images, or missing title | Automated completeness check fails | Reviewer sees an explicit failure, not a silent empty gallery |
| Gallery image without caption | A gallery image has a source path or URL but no caption or alternative description | Automated completeness check fails | Every picture in the gallery has readable text for visitors and reviewers |
| Off-theme gallery | Required gallery is primarily product, case, or unthemed placeholders rather than Turpin Enterprises people, office, or brand | Completeness or content review MUST fail | Gallery remains a team/workplace set, distinct from project and case pages |
| Metadata example missing | No article demonstrates tags, featured image, excerpt, and TOC hint together | Automated completeness check fails | Reviewers can rely on at least one fully exercised article |
| Draft leaked to readers | A draft article (if one exists) appears on the public showcase as a published piece | Publication MUST omit drafts from reader-facing published lists and pages | Visitors only see non-draft articles as published content; this feature does not require a draft fixture |
| Consumer-shaped source of truth | Records exist only as a single theme’s blog/gallery tree or layout keys | Out of scope / invalid for this feature | Generic entities must exist independently of any one consumer |
| Channel charter breach | Article or gallery pages present export plumbing or technical ids as primary content | Reject for this feature; keep reader-facing copy | Public showcase stays a reader, not an importer |
| Missing or orphan author | Article has no author, claims a persona that does not exist, or the author is not a Turpin Enterprises member | Automated completeness check fails | Every article is attributed to a valid Turpin Enterprises persona |
| Collision with existing showcase types | New pages are only relabelled persona, organisation, timeline, deal, case, or project pages | Completeness or review MUST fail unless sharing is an explicit related link | Article and gallery remain distinct content types |
| Related link hidden from readers | Project or case article has a data relation but the reader-facing article page does not show a named link to that project or case | Publication or review MUST fail | Visitors can follow the relationship the same way they follow other named links on the public site |

## Security & Access *(required when feature handles sensitive data, authentication, or external exposure)*

This feature adds **fictional** articles and gallery images on the public reference site. It does not introduce authentication or personal data of real people, and it does not add a new bulk-download surface.

| Asset / Surface | Threat | Access Control | Verification |
|-----------------|--------|----------------|--------------|
| Article and gallery copy | Mistaken identity: readers treating invented posts or photos as real news | Content remains clearly in-universe Turpinverse fiction | Review copy for real PII; names stay fictional or already-canon |
| Image paths and remote URLs | Unexpected or inappropriate external assets | Prefer in-universe placeholders; remote URLs only when consistent with branding | Content review of every image reference |
| Public reference site | Reader-facing pages must not become an import tool | Same publication rules as other showcase pages; no new anonymous bulk download | Walk article and gallery pages for export UI and identifier-as-identity |
| Draft articles | Unpublished drafts exposed as if live | Drafts excluded from published reader lists | Confirm draft flag is honoured on the public site |

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST treat **articles** and **galleries** as first-class generic content records owned by Turpinverse, independent of any one website theme, CMS path, gallery layout key, or front-matter convention.
- **FR-002**: The system MUST NOT require emitting a particular CMS or theme’s blog or gallery file layout as the stored or accepted format.
- **FR-003**: The system MUST provide **ten** **published** (non-draft) article records. Each article MUST include a stable identity, title, publish date, draft flag, body (human-authored copy; lightweight markup allowed), and an **author that is an existing persona who is a member of Turpin Enterprises** (identity MUST resolve). Exactly **three** of the ten MUST be authored by Richard Turpin. The other **seven** MUST be authored by other Turpin Enterprises members (not Richard); the same other member MAY author more than one of those seven. The volume target is ten published articles; shipping an additional draft fixture is not required.
- **FR-004**: Article records MUST be able to represent optional tags, featured image (path or URL), description or excerpt, and optional presentation hints for table-of-contents and math or special rendering. Author is **required** and MUST be a resolving Turpin Enterprises persona identity (not a free-text-only name with no persona). Every article MUST belong to a **collection** (a generic grouping, not a CMS section path). One or more collections are allowed; all required articles may share a single collection. On the public reference site, that collection MUST be shown as a visible label on the article list and on each article page. This feature MUST NOT add dedicated collection index pages.
- **FR-005**: At least one article MUST exercise tags, featured image, description or excerpt, and a table-of-contents hint together. Using a math or special-rendering flag on at least one article is optional.
- **FR-006**: The system MUST provide at least one gallery record with a title, optional description, and an ordered list of at least four images, each with a source path or URL **and** a human-readable **caption or alternative description**. That gallery MUST depict **Turpin Enterprises team or workplace** subjects (people, office, or brand). It MUST NOT exist primarily as product photography of Black Bess Route Optimiser or as a stand-in for a support-case or project showcase page.
- **FR-007**: That gallery MUST have a viewer or lightbox preference enabled. Viewer options, when present, MUST be generic presentation hints, not a named theme’s layout keys.
- **FR-008**: Article and gallery copy and image choices MUST follow existing Turpinverse creative rules (legend-as-business, modern professional names, euphemism over explicit criminal language, tongue-in-cheek but demo-credible). All ten articles MUST read as a Turpin Enterprises team blog (not guest posts from outside that organisation).
- **FR-009**: Article and gallery records MUST be distinct in purpose from existing showcase types (personas, organisations, timeline, deals, cases, projects) unless a related link is intentional. They MUST NOT exist only as renamed versions of those pages. The project-topic and case-topic articles MUST relate to the existing Black Bess Route Optimiser project record and the chosen support case respectively, without replacing those pages. On the public reference site, those article pages MUST present that relation as a **named link** to the project or case page.
- **FR-010**: When published for readers, articles and galleries MUST appear on the **public reference site** as human-readable showcase content, with a **dedicated article list** and **gallery page(s)** linked from **primary navigation or home** (same discoverability as people, organisations, and deals), plus reader-facing detail views. Reader-facing pages MUST NOT present technical identifiers or export, filter, or download plumbing as primary content. The public site MUST showcase these types rather than omit them or send readers to an export tool as the only view. The Turpin Enterprises organisation page is **not** required to list or link the gallery.
- **FR-011**: Draft articles, if any exist, MUST be excluded from published reader-facing lists and pages. The ten required articles MUST be non-draft and eligible for publication. A draft fixture is not required to complete this feature.
- **FR-012**: This feature MUST NOT add articles or galleries as interactive-export tabular datasets, and MUST NOT require the export app to offer download of these types.
- **FR-013**: Completeness and cross-reference rules for these records MUST be enforced by an **automated completeness check** that fails until they hold: volume and author-split (FR-003); gallery minima (FR-006–FR-007); topic mix and related project/case links (FR-016); required fields including a resolving Turpin Enterprises author persona and a collection on every article; FR-005 metadata example. Human review alone is not sufficient.
- **FR-014**: A mapping note in repository developer documentation MAY describe how these entities could populate a typical blog and gallery; it MUST NOT make a named theme or CMS the source of truth, and the public reference site MUST NOT be required to host that note.
- **FR-015**: On the public reference site, each article detail page MUST present the author as a named person. That persona’s **existing person page** MUST list their published articles when they have at least one. If a persona has no published articles, that list MUST be omitted (no empty heading or empty list). Organisation and other non-person related-record pages are not required to list articles **or** the gallery. Interactive export-app contact pages are not required to list articles in this increment.
- **FR-016**: Of the ten published articles: **one** MUST be on the subject of the existing **Black Bess Route Optimiser** project and MUST relate to that project record; **two** MUST be on the subject of **the same one** of the three support cases whose account is Turpin Enterprises and MUST both relate to that chosen case; the **remaining seven** MUST be thought leadership on topics related to the **job role of that article’s author**. The project and case articles MAY be written by Richard or by another team member; they still count toward the FR-003 author split. Reader-facing pages for the project and case articles MUST show the named project or case link (FR-009). The seven thought-leadership articles are not required to relate to a project or case.

### Key Entities

- **Article**: A reusable thought-leadership or blog-like content item: stable identity, title, publish date, draft flag, body, **required Turpin Enterprises author persona**, **required collection**, optional tags, featured image, excerpt/description, optional TOC and math/rendering hints. Related project is required on the Black Bess article; related case is required on the two case articles. Those relations appear as named links on the public article pages. Other articles MAY omit project/case relations.
- **Gallery**: A reusable ordered image collection of **Turpin Enterprises team or workplace** pictures (people, office, or brand): stable identity, title, optional description, ordered images, viewer/lightbox preference and optional generic viewer options. Public discovery is dedicated gallery page(s) from navigation or home; organisation pages need not link it.
- **Gallery image**: One slot in a gallery: source path or URL, **required** caption or alternative description, order significant.
- **Content collection**: A required grouping for each article (generic label, not a CMS section path). Shown as a visible label on the public article list and article pages. One or more collections may exist; all articles MAY share one collection. No dedicated collection index pages in this feature.
- **Presentation hint**: Generic flags such as “show a table of contents” or “enable lightbox,” stored without a consumer theme’s layout key vocabulary.
- **Persona (existing)**: Required author of every article; MUST be a member of Turpin Enterprises; identity MUST resolve. Articles do not create new people. On the public site, the persona’s existing page lists published articles they authored when any exist.
- **Project (existing)**: Black Bess Route Optimiser is the required subject of exactly one article, related by a named reader-facing link from that article page — not replaced by that article. The project page need not list the article.
- **Support case (existing)**: Exactly one of the three Turpin Enterprises cases is the required subject of exactly two articles, both related to that same case, each with a named reader-facing link from the article page. The case page need not list the articles.
- **Consumer mapping note (optional)**: Documentation of a *possible* projection onto a typical blog/gallery layout; not a stored duplicate of that layout.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A reviewer can list all ten published articles and open their titles, dates, and bodies in under five minutes using only the published generic records (and, for reader-facing review, the public reference site).
- **SC-002**: Volume and mix gates hold: **10** published (non-draft) articles; exactly **3** by Richard Turpin and **7** by other Turpin Enterprises members; **1** related to Black Bess Route Optimiser; **2** related to the same Turpin Enterprises support case; **7** role-shaped thought-leadership pieces; at least 1 gallery of Turpin Enterprises team or workplace pictures with at least 4 ordered images, a caption or alternative description on every image, and lightbox/viewer preference on; at least one article that includes tags, featured image, excerpt, and TOC hint. A draft article is not required.
- **SC-003**: 100% of articles name an existing Turpin Enterprises author persona; 100% of those links resolve; project and chosen-case relations resolve; the automated completeness check reports pass only when FR-003–FR-007, FR-013, and FR-016 hold, and fail with the offending record identities otherwise.
- **SC-004**: A first-time visitor to the public reference site can discover articles and the gallery from primary navigation or home (dedicated list/gallery pages, not only a remembered address) without using the interactive export app, and can tell those pages apart from people, organisations, timeline, deals, cases, and project pages, in under five minutes. From an article page they can identify the author as a person; from an authoring persona’s existing page they can see that person’s published articles listed (and they do not see an empty articles block on a persona with none). From the project article and either case article they can follow a named link to the related project or case page. From the article list and an article page they can see the collection label; they are not sent to a collection index as the way to browse articles.
- **SC-005**: Walkthrough of published article and gallery pages finds zero export/download/import controls and zero technical identifiers used as the primary way to identify a piece (display titles and dates are sufficient).
- **SC-006**: At least 80% of sampled article titles (all articles in the set) are rated plausible for a client-facing professional demo (3/5 or higher) by reviewers who also recognise Turpinverse voice.
- **SC-007**: A reviewer checking storage and acceptance finds no requirement to produce a particular CMS or theme’s blog or gallery file layout in order to mark this feature done.

## Assumptions

- Issue #21 is a sibling of career/portfolio demo data under parent #18; it adds **content** types (articles, galleries), not experience, education, projects, or achievements.
- Every article is authored by an existing Turpin Enterprises member. Exactly three bylines are Richard Turpin; the other seven are other members of that organisation (same person may write more than one of the seven). New people are not created for bylines.
- Turpin Enterprises membership means the persona already belongs to that organisation in canon. The seven non-Richard authors may include any such member except Richard.
- Topic mix is exactly: one Black Bess Route Optimiser article, two articles on one chosen Turpin Enterprises support case (the three cases on that account), seven remaining pieces aligned to the author’s job role. Project/case articles still count in the 3/7 author split.
- Ten **published** articles and one four-image gallery are the targets. The required gallery is Turpin Enterprises team or workplace photography (people, office, or brand), not a product or case showcase. A draft article fixture is not required; if one is added later, it must stay off published reader lists.
- Every article belongs to a collection (generic grouping, not a CMS path). Multiple collections are allowed; one collection for all required articles is enough. On the public site, collections appear as labels on the article list and article pages; this feature does not add collection index pages.
- Publication channel is the public reference site only; dual publication on the interactive export app is out of scope for this increment (including listing articles on export-app contact pages).
- Gallery discovery is dedicated gallery page(s) from primary navigation or home. The Turpin Enterprises organisation page need not list or link the gallery.
- Article bylines on the public site: article pages name the author as a person; that person’s existing page lists their published articles when they have any. Organisation and other non-person pages need not list articles.
- Project and case articles: public article pages show a named link to the related project or case; reverse lists on those project/case pages are not required. The seven role-topic articles need not link to a project or case.
- English-only copy, consistent with the existing canon.
- Placeholder or remote images are sufficient; shipping a large binary photo library is out of scope. Every gallery image still needs a caption or alternative description even when the asset is a placeholder or remote URL.
- Body copy may use lightweight markup; that is a content format, not a consumer theme template.
- Optional TOC and math flags are generic metadata; they do not obligate a specific rendering engine.
- Mapping to Hugo Profile, PaperMod, or any other named theme is explicitly **not** an acceptance requirement (confirmed in issue #21 comments, 2026-08-22).
- Existing Creative Universe Rules apply unchanged.
- No new authentication surface is required.
- Completeness for this feature is a machine gate in the same spirit as other canon volume and cross-reference rules.
