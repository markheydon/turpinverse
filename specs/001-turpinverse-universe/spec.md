# Feature Specification: Turpinverse Universe & Demo Data

**Feature Branch**: `001-turpinverse-universe`

**Created**: 2026-07-18

**Status**: Draft

**Input**: User description: "Turpinverse: a fictional but internally consistent universe centred on the myth of Dick Turpin. The goal is to provide a consistent and realistic completely made set of personas and organisations based on the legend of Richard 'Dick' Turpin and his gang. This Dick Turpin related universe should be based on the details documented in the Wikipedia page Dick Turpin. The universe created will be used to fuel sample data for business applications for demo purposes. Applications such as CRMs used for Sales or Customer Services for example will get generated from the data. Therefore it should be expected that data suitable for that kind of scenario will need to be generated so contacts, organisations, cases, deals, etc. The made up characters and organisations should be humorous in a tongue-in-cheek kind of a way. The idea being anybody see the data will instantly recognise the characters used in a semi-professional manner but still be able to bring a smirk to their face understanding the play on the myth that is being portrayed."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Establish the Turpinverse Canon (Priority: P1)

A product demonstrator or developer needs a coherent fictional universe grounded in the
historical Dick Turpin legend so that all future sample data shares the same characters,
organisations, timeline, and tone. They browse the canon reference and can identify who
the key personas are, which organisations exist, how they relate, and what historical
events anchor the fiction.

**Why this priority**: Without a shared canon, generated CRM data would be inconsistent
and lose the recognisable Turpinverse identity. The canon is the foundation every other
artifact depends on.

**Independent Test**: Can be fully tested by reviewing the canon document and confirming
it lists at least 15 named personas, 8 organisations, a timeline of key events, and
explicit consistency rules — without requiring any CRM records to exist yet.

**Acceptance Scenarios**:

1. **Given** no prior Turpinverse content exists, **When** the canon is published,
   **Then** it defines named personas drawn from Wikipedia-documented figures (e.g., Dick
   Turpin, Elizabeth Millington, Essex Gang members, Matthew King, Mary Brazier) plus
   plausible fictional extensions.
2. **Given** a reader familiar with the Dick Turpin legend, **When** they review persona
   entries, **Then** they can identify the historical inspiration within 30 seconds per
   character.
3. **Given** two personas in the canon, **When** a reader checks their relationship
   entries, **Then** no contradictory affiliations, dates, or locations appear.
4. **Given** the Wikipedia article on Dick Turpin, **When** a reviewer cross-checks major
   canon events (Essex Gang raids, alias John Palmer, York trial, Knavesmire execution),
   **Then** the canon does not contradict documented historical facts, only extends them
   fictionally.

---

### User Story 2 - Generate CRM-Ready Sample Datasets (Priority: P2)

A sales or customer-service demo presenter needs realistic-looking CRM records populated
with Turpinverse characters and organisations so they can showcase pipelines, account
hierarchies, support cases, and contact management in a live application.

**Why this priority**: The primary consumer value is demo-ready business data. Canon
without exportable records cannot fuel CRM demonstrations.

**Independent Test**: Can be fully tested by loading generated datasets into a generic
CRM schema (contacts, accounts, opportunities, cases) and confirming every record maps
to a canon persona or organisation with no orphan references.

**Acceptance Scenarios**:

1. **Given** the published canon, **When** sample datasets are generated, **Then** they
   include contacts, organisations (accounts), deals (opportunities), and cases (support
   tickets) with cross-references intact.
2. **Given** a contact record, **When** a demo user views it, **Then** it contains
   semi-professional fields (name, title, organisation, email-style address, phone, notes)
   that read plausibly in a business context.
3. **Given** an organisation record, **When** a demo user views it, **Then** it uses a
   tongue-in-cheek business name derived from a canon entity (e.g., a coaching inn
   reframed as a hospitality group, the Essex Gang reframed as a loosely structured
   "consultancy").
4. **Given** a deals pipeline, **When** records are reviewed, **Then** at least 80% of
   deals reference known canon personas as stakeholders and reflect plausible
   18th-century commerce reframed in modern CRM vocabulary.
5. **Given** a support cases list, **When** records are reviewed, **Then** case subjects
   and descriptions evoke Turpinverse scenarios (horse disputes, property recovery,
   alias identity mix-ups) in professional ticket language.

---

### User Story 3 - Deliver Recognisable Humour Without Breaking Demo Credibility (Priority: P3)

A demo audience member glances at CRM screens during a presentation and experiences a
moment of recognition and amusement — they spot the Turpinverse reference, smile, and
the data still looks credible enough for a serious product walkthrough.

**Why this priority**: Humour is a core differentiator requested by the stakeholder; it
must be tuned so it delights rather than undermines the demo.

**Independent Test**: Can be fully tested by presenting 10 sample records to 3 reviewers
and confirming at least 2 of 3 identify the Turpinverse reference while rating all
records at least 3/5 on "would use in a client demo" professionalism.

**Acceptance Scenarios**:

1. **Given** any generated contact or organisation name, **When** shown to a reviewer
   unaware of the project, **Then** the record appears to be a legitimate business entry
   at first glance.
2. **Given** a reviewer familiar with English highwayman folklore, **When** they scan 20
   records, **Then** they identify the Turpinverse theme within the first 5 records.
3. **Given** the tone guidelines in the canon, **When** a new record is drafted,
   **Then** it uses wry wordplay or historical allusion rather than slapstick, profanity,
   or anachronistic modern slang.
4. **Given** a case or deal description, **When** reviewed against tone guidelines,
   **Then** no record contains content that would be inappropriate in a corporate sales
   or customer-service demo setting.

---

### Edge Cases

- What happens when a historical figure has minimal documented detail (e.g., John
  Wheeler)? The canon MUST provide a plausible fictional biography clearly marked as
  extension, not historical fact.
- How does the system handle contradictory myth versions (e.g., Ainsworth's Black Bess
  ride vs. historical record)? The canon MUST adopt the Wikipedia-sourced history as
  primary and treat romanticised myths (Black Bess overnight ride) as in-universe legend
  or folklore, not as factual timeline events.
- What happens when CRM field length limits truncate a humorous organisation name?
  Records MUST include a short display name and a longer description so humour survives
  truncation.
- How are deceased historical figures represented in CRM data? They appear as historical
  accounts, legacy contacts, or archived cases — not as active pipeline owners — unless
  explicitly framed as flashback or legend entries.
- What happens when a demo requires non-English locales? Initial release is English-only;
  names and humour rely on English cultural recognition.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST maintain a Turpinverse canon document defining personas,
  organisations, locations, timeline events, and consistency rules.
- **FR-002**: The canon MUST anchor all major personas and events to the historical
  record of Richard Turpin as documented on Wikipedia, distinguishing historical fact
  from fictional extension.
- **FR-003**: The canon MUST include at least 15 named personas covering Dick Turpin,
  his associates (Essex Gang, highwaymen allies), antagonists (keepers, magistrates),
  and key supporting figures (e.g., Elizabeth Millington, Mary Brazier, Richard Bayes).
- **FR-004**: The canon MUST include at least 8 organisations with humorous
  tongue-in-cheek business equivalents (e.g., coaching inns, fencing operations, assize
  courts, horse-trading ventures) mapped to their historical inspirations.
- **FR-005**: The system MUST generate sample CRM datasets comprising contacts,
  organisations (accounts), deals (opportunities), and cases (support tickets).
- **FR-006**: Every generated CRM record MUST reference a canon persona or organisation
  and MUST NOT introduce characters or entities absent from the canon.
- **FR-007**: Generated contact records MUST include semi-professional identity fields:
  full name, job title, organisation affiliation, communication details, and a brief
  notes field.
- **FR-008**: Generated organisation records MUST include a professional-sounding trading
  name, industry classification, description, and a parent/child relationship where
  historically appropriate (e.g., gang members under a holding entity).
- **FR-009**: Generated deal records MUST include stage, value, close date, primary
  contact, and account, using commerce-themed scenarios consistent with the 1730s setting
  reframed in modern business language.
- **FR-010**: Generated case records MUST include status, priority, subject, description,
  and linked contact/account, themed around Turpinverse disputes and incidents.
- **FR-011**: The system MUST publish tone and humour guidelines specifying how
  tongue-in-cheek references are applied without breaking demo professionalism.
- **FR-012**: The system MUST provide a cross-reference index linking historical names
  to their Turpinverse CRM equivalents (e.g., "John Palmer" → Dick Turpin's alias
  account).
- **FR-013**: Generated datasets MUST be internally consistent: no orphan foreign-key
  relationships, no contradictory dates, and no duplicate identities unless explicitly
  flagged as aliases.
- **FR-014**: The system MUST document assumed data volumes for demo use: at least 25
  contacts, 10 organisations, 20 deals, and 15 cases per default dataset.

### Key Entities

- **Persona**: A character in Turpinverse with historical anchor, fictional biography
  extension, aliases, affiliations, and CRM-suitable identity attributes (title, role,
  temperament).
- **Organisation**: A group, business, institution, or gang reframed as a demo-ready
  account with trading name, industry, parent hierarchy, and key members.
- **Canon Event**: A dated entry on the Turpinverse timeline (historical or legendary)
  linking affected personas and organisations.
- **Contact**: A CRM person record derived from a Persona, suitable for sales or service
  demos.
- **Account**: A CRM organisation record derived from an Organisation entity.
- **Deal**: A CRM opportunity record representing a commercial transaction or pursuit
  between accounts and contacts.
- **Case**: A CRM support ticket record representing a dispute, inquiry, or incident.
- **Alias Map**: A mapping of alternate identities (e.g., John Palmer, Richard Turpin)
  to a single canonical persona for deduplication in demos.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Reviewers can identify the historical inspiration for any of 10 randomly
  selected personas within 30 seconds each.
- **SC-002**: 100% of generated CRM records pass an automated cross-reference check
  against the canon with zero orphan references.
- **SC-003**: At least 80% of demo reviewers (minimum 3 reviewers) rate sample records
  at 3/5 or higher on "suitable for a client-facing CRM demo" while also identifying the
  Turpinverse theme.
- **SC-004**: The default dataset meets minimum volume thresholds: 25 contacts, 10
  organisations, 20 deals, and 15 cases.
- **SC-005**: A consistency audit of the canon finds zero contradictory persona
  affiliations, dates, or locations across all entries.
- **SC-006**: At least 90% of organisation and contact names read as plausible business
  entries on first glance before the historical reference becomes apparent.

## Assumptions

- The primary audience is English-speaking demo presenters and developers working with
  CRM products (Sales and Customer Service scenarios).
- Wikipedia's Dick Turpin article is the authoritative historical baseline; romanticised
  Victorian additions (e.g., the Black Bess ride) are treated as in-universe folklore,
  not historical fact.
- Initial release is English-only; no localisation is required.
- Humour targets a tongue-in-cheek, semi-professional register — witty allusions rather
  than overt comedy or crude humour.
- CRM field shapes follow common conventions (contact, account, opportunity, case) but
  the specification does not mandate a specific CRM vendor or export format.
- Default dataset volume (25/10/20/15) is sufficient for demo scenarios; larger datasets
  are a future enhancement.
- Fictional extensions to sparsely documented historical figures will be clearly
  delineated from documented facts within the canon.
- Demo data represents a playful modern reimagining of 1730s England; records use
  contemporary business vocabulary overlaid on period-appropriate scenarios.
