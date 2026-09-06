# Feature Specification: CRM Join Graph Alignment

**Feature Branch**: `cursor/crm-join-graph-7236`

**Created**: 2026-09-06

**Status**: Draft

**Input**: User description: "Implement the target CRM join graph documented in docs/entity-relationships.md." (full description from GitHub issue #41)

**Source issues**:

- [#41 Align CRM join schema, export, and canon rows](https://github.com/markheydon/turpinverse/issues/41)
- Join graph: [`docs/entity-relationships.md`](../../docs/entity-relationships.md)
- Unblocks CRM completeness epic [#32](https://github.com/markheydon/turpinverse/issues/32) and child stories #33–#38

## Publication surfaces

This feature reshapes **how records join** and **how contacts export**. It MUST name both human-facing channels (constitution IX). Detail lives in [`docs/product-surfaces.md`](../../docs/product-surfaces.md).

| Surface | Responsibility for this feature |
|---------|----------------------------------|
| **Public reference site** | Showcase people, accounts, deals, cases, projects, and timeline using **names and stories**. When a deal, case, or project has a main contact or stakeholders, show those people by name. When a main contact is omitted, omit the named-contact block — do not show empty identifiers. Reader-facing pages MUST NOT present join keys, export column names, or collision-policy plumbing as primary content. Update pages **only where** they currently assume every deal/case/project always has a main contact, or they list an undifferentiated “people” list that would mis-state membership. |
| **In-product export app** | Explore, preview, filter, and download importable files. Technical identifiers, optional main-contact columns, stakeholder columns, membership-duplicated contact rows, and the email collision policy belong here (and in exporter documentation), not on the public site. |

Shared canon remains the single source of truth. This feature MUST NOT add a second public site or a new download product beyond the existing contact, account, deal, case, and project datasets.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Join rules match a real CRM account graph (Priority: P1)

A product demonstrator importing Turpinverse into a CRM needs the **same join rules** a B2B CRM uses: every contact belongs to at least one account; an account may have zero contacts; every deal, case, and project sits on exactly one account; a main contact is optional and, when present, belongs to that account; extra people on the record may be stakeholders who are **not** account members (vendor, counsel, counterpart).

Today the shipped dataset still requires a main contact on deals and cases, does not distinguish stakeholders, and allows a main contact who is not on the account. After this story, completeness checks and authored records follow the target graph in [`docs/entity-relationships.md`](../../docs/entity-relationships.md).

**Why this priority**: Wrong joins poison every CRM demo that follows. Child completeness stories assume this graph. Without P1, export and four-row story fixes have no rule to satisfy.

**Independent Test**: Confirm account membership may be empty in the rules (even if demo data still has members); confirm every contact still has at least one account; confirm deal/case/project require an account; confirm main contact is optional in the rules and **must be an account member when set**; confirm stakeholders need not be members; confirm an automated completeness check fails a fixture that puts a non-member in the main-contact slot and passes the same fixture when that person is only a stakeholder. No empty demo accounts and no contact-less demo deals are required to prove the rule.

**Acceptance Scenarios**:

1. **Given** the target join graph, **When** a reviewer inspects contact–account membership, **Then** every contact has at least one account, membership is recorded on both sides when a link exists, and the rules allow an account with zero contacts.
2. **Given** an account with an optional primary contact, **When** that primary contact is set, **Then** that person MUST be a member of that account. The account’s primary contact is distinct from a contact’s primary account (first account on the contact).
3. **Given** a deal, case, or project, **When** it is stored, **Then** it has exactly one account. A main contact MAY be omitted. If a main contact is set, that person MUST be a member of that account.
4. **Given** stakeholders on a deal, case, or project, **When** those people are not members of the account, **Then** the record is still valid. Stakeholders MUST be existing people; they MUST NOT require new invented characters.
5. **Given** a completeness check, **When** a deal’s main contact is not a member of the deal’s account, **Then** the check fails and names the record. **When** that person is moved to stakeholders and a member is the main contact (or the main contact is omitted), **Then** that membership violation does not appear.
6. **Given** a project, **When** people are listed for “who is on this project,” **Then** the set is the main contact (if any) union stakeholders. An undifferentiated required people list is retired or derived from that union — not a third independent roster that can disagree with membership.
7. **Given** a timeline event, **When** it optionally names deals and/or cases, **Then** those references MUST exist. Linking an event to a case implies that case’s account and main contact on public timelines and export roll-up; those implied identities MUST NOT be duplicated as extra required fields on the event.

---

### User Story 2 - Four story rows stay true without fake membership (Priority: P2)

A canon reviewer needs four existing records to remain **tellable** after the membership rule lands, without inventing empty accounts, contact-less pipeline records, new people, or extra stakeholder ensembles beyond these fixes:

| Record | Problem | Required outcome |
|--------|---------|------------------|
| Forest patrol outsourcing deal (`deal-008`) | Henry Clayton is named against Epping Forest Authority but is not a member | Account stays Epping Forest Authority. Main contact is an Epping Forest member. Henry remains on the record as a stakeholder (existing person only). |
| Palmer identity collision case (`case-001`) | Richard Turpin is named against Brazier Legal but is not a member | Account stays Brazier Legal. Main contact is a Brazier Legal member. Richard remains as a stakeholder. |
| Route analytics sync case (`case-017`) | Thomas Collier is named against Turpin Enterprises but is not a member | Account stays Turpin Enterprises. Main contact is a Turpin Enterprises member. Thomas remains as a stakeholder. |
| Palmer Identity Vault project (`palmer-identity-vault`) | Richard Turpin is listed on a Brazier Legal project but is not a member | Sponsoring account stays Brazier Legal. Main contact is a Brazier Legal member. Richard remains as a stakeholder. |

Do **not** add Richard to Brazier Legal, Henry to Epping Forest, or Thomas to Turpin Enterprises solely to keep the old main-contact field. Do **not** add demo rows whose only purpose is to show empty accounts or deals with no main contact.

**Why this priority**: The join rules are unused if the living dataset still violates them, but the stories must survive. Fixes are a bounded content slice on top of P1.

**Independent Test**: After the four edits, an automated completeness check reports no main-contact-vs-membership violations on these four ids. Each still has a main contact (this increment does not use them to illustrate a missing main contact). Stakeholder lists on these four contain only the named non-members required to keep the story. No new organisations, deals, cases, projects, or people were invented for the illustration.

**Acceptance Scenarios**:

1. **Given** `deal-008`, **When** a reviewer inspects it, **Then** the account is Epping Forest Authority, the main contact is a member of that account, and Henry Clayton is a stakeholder (not the main contact unless he is first made a member — which this feature MUST NOT do).
2. **Given** `case-001`, **When** a reviewer inspects it, **Then** the account is Brazier Legal, the main contact is a member of that account, and Richard Turpin is a stakeholder.
3. **Given** `case-017`, **When** a reviewer inspects it, **Then** the account is Turpin Enterprises, the main contact is a member of that account, and Thomas Collier is a stakeholder.
4. **Given** `palmer-identity-vault`, **When** a reviewer inspects it, **Then** the sponsoring account is Brazier Legal, the main contact is a member of that account, and Richard Turpin is a stakeholder.
5. **Given** the rest of the dataset, **When** this feature ships, **Then** no extra empty accounts, contact-less deals/cases/projects, or additional stakeholder casts were added beyond these four rows (other records MAY already have valid members as main contacts and need no stakeholder theatre).

---

### User Story 3 - Export one contact row per account membership (Priority: P3)

A demonstrator downloading contacts needs **one contact row per membership**: the same person who belongs to two accounts appears twice, with the same contact identity and the same email, phone, and mailing address copied onto each row, and a different account on each row. Deal, case, and project downloads expose optional main contact and stakeholders. Account download MAY include optional primary contact.

CRMs that unique-key on email will see collisions for multi-account people. The export experience MUST document that collision policy so importers do not assume Turpinverse minted a different email per account.

**Why this priority**: Join-correct canon still fails CRM import fidelity if export emits one row per person (primary account only). Export is independently testable once P1 membership is defined; P2 row fixes only change who appears on four records.

**Independent Test**: Count membership links in canon and contact download rows — they MUST match. A person with two accounts yields two rows sharing contact identity, email, phone, and mailing address, differing by account. Deal/case/project files include optional main contact and a stakeholder field. Collision policy is readable from exporter documentation or the in-product export surface, not as primary copy on the public site.

**Acceptance Scenarios**:

1. **Given** a person who belongs to two accounts, **When** contacts are downloaded, **Then** there are two rows with the same contact identity, the same email, phone, and mailing address (copied, not re-authored per account), and two different accounts.
2. **Given** a person who belongs to one account, **When** contacts are downloaded, **Then** there is exactly one contact row for that person.
3. **Given** the full contact download, **When** a reviewer sums rows, **Then** the count equals the number of person–account membership links (not the number of people).
4. **Given** deal, case, and project downloads, **When** a reviewer inspects columns, **Then** main contact is optional (empty when omitted), and stakeholders are present as an export field (empty when none). Project download also carries optional originating deal and related cases. Project people in export match main ∪ stakeholders.
5. **Given** a target CRM that treats email as unique, **When** a reviewer reads the documented collision policy, **Then** they learn that Turpinverse contact identity is the person key, email is a copied attribute, multi-membership rows intentionally repeat email, and Turpinverse will **not** invent per-account email, phone, or address in canon. Importers who need unique email MUST apply their own rule (composite person+account, skip, or suffix) outside Turpinverse.
6. **Given** the public reference site, **When** a visitor reads person and organisation pages, **Then** they see human-readable affiliations, not a CSV collision essay or per-account fake emails.

---

### Edge Cases

- What if an account has zero members? Allowed by the join rules. This increment MUST NOT author an empty account just to demonstrate the rule.
- What if a deal, case, or project has no main contact? Allowed by the join rules. This increment MUST NOT strip main contacts from demo records just to demonstrate optionality. Public pages omit the named-contact block; download main-contact cells are empty.
- What if the main contact is set but is not an account member? Invalid. Completeness fails. Fix by changing main contact, moving the person to stakeholders, or (out of scope here except as forbidden) adding membership solely to dodge the rule on the four named rows.
- What if a stakeholder is also the main contact? Invalid as a duplicate listing. Main contact occupies the main slot; stakeholders are additional people.
- What if a stakeholder id does not exist? Invalid.
- What if a contact has no account? Invalid. Every person keeps at least one account. Unassigned contacts are out of scope.
- What if someone stores a different email, phone, or mailing address per account on the person record? Invalid. One identity per person in canon; duplication happens only at export.
- What if a CRM keys uniqueness on email? Documented collision: repeated email on membership rows is expected; Turpinverse does not mint alias emails.
- What if a project still has a required undifferentiated people list that disagrees with main ∪ stakeholders? Invalid. Retire or derive that list.
- What if an event lists deals or cases that do not exist? Invalid.
- What if the public site currently assumes a deal or case always has a main contact? Publication MUST tolerate omission; reader copy uses names, never empty technical keys as primary content.
- What if professional-extras “contact” copy is treated as a CRM contact? It is not. No change to extras in this feature.
- What if this work is recorded only by silently rewriting the original universe specification? Invalid. This feature has its own specification folder. Existing universe docs and export contracts that would otherwise **lie** after this change MUST be updated so documentation matches behaviour (constitution VIII), without treating the original universe spec as the only record of the join graph.

## Failure Modes & Error Handling *(required when feature defines external boundaries or user-facing failures)*

This increment extends published canon, completeness checking, and existing export datasets. It does not add a new anonymous bulk surface.

| Failure Mode | Trigger | Expected Handling | User/System Outcome |
|--------------|---------|-------------------|----------------------|
| Unassigned contact | A person has no account | Completeness check fails (existing membership rules remain; people still need ≥1 account) | No CRM “lead-only” people in canon |
| Primary contact not a member | Account primary contact is set but is not in the account’s members | Completeness check fails (**VR-052**) | Primary contact is omitted or changed to a member |
| Missing account on pipeline record | Deal, case, or project lacks a valid account | Completeness check fails (**VR-053**) | Record is not accepted |
| Main contact not an account member | Deal, case, or project main contact is set but is not a member of that account | Completeness check fails (**VR-054**) | Includes the four named rows until they are fixed |
| Unknown or duplicate stakeholder | Stakeholder id missing, unknown, duplicated, or equal to the main contact | Completeness check fails (**VR-055**) | Stakeholders are extra existing people only |
| Unknown project lineage | Project names a deal or cases that do not exist | Completeness check fails (**VR-056**) | Lineage omitted or pointed at real records |
| Unknown event pipeline links | Event names deals or cases that do not exist | Completeness check fails (**VR-057**) | Optional arrays only contain real ids |
| Four story rows still invalid | `deal-008`, `case-001`, `case-017`, or `palmer-identity-vault` still use a non-member as main / undifferentiated project people | Completeness check fails (**VR-054** and/or **VR-058**) | Stories preserved via member main + stakeholder, not fake membership |
| Contact export under-counts memberships | Download emits one row per person instead of one per membership | Export mapping is wrong; tests fail (**VR-059** as the membership-row invariant) | Multi-account people appear once per account |
| Per-account identity in canon | Email, phone, or address stored per membership on the person | Rejected | Copy identity at export only |
| Public site shows join plumbing | Reader pages lead with ids, CSV column names, or collision policy | Channel charter failure | Names and stories on the public site; ids on export |
| Docs disagree with the graph | Original universe spec or export contracts still require deal/case main contact or single contact row per person | Update those contracts **and** keep this feature folder as the join-correction record | Documentation matches behaviour |

Existing deceased-person-must-not-own-**active**-deals handling remains in force when a main contact is set.

## Security & Access *(required when feature handles sensitive data, authentication, or external exposure)*

Canon remains **fictional** demo data. This feature repeats contact identity (including email) across membership rows. No new authentication surface.

| Asset / Surface | Threat | Access Control | Verification |
|-----------------|--------|----------------|--------------|
| Contact identity (email, phone, mailing address) | Readers treating repeated emails as real people or as per-employer mailboxes | In-universe fiction; one authored identity per person; collision policy explains copy-on-export | Review export rows for copied — not invented — identity |
| Public reference site | Join keys or export policy leaking into reader copy | Same access as existing pages; names and stories only | Reviewer can open deal, case, project, person, and organisation pages |
| In-product export download | Technical ids and duplicate emails in importable files | Same access as existing downloads; document collision policy next to export | Reviewer can preview contacts, deals, cases, and projects |

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Contact → account remains many-to-many with a **minimum of one account per contact**. Unassigned contacts are not allowed.
- **FR-002**: Account → contact is zero-or-more members. The members list MAY be empty in the rules (`min` 0). Demo data for this increment NEED NOT include an empty account.
- **FR-003**: An account MAY name at most one **primary contact**. When set, that person MUST be a member of that account (**VR-052**). This is distinct from the contact’s primary account (first account on the contact).
- **FR-004**: Every deal, case, and project MUST have exactly one account (**VR-053**).
- **FR-005**: Deal, case, and project MAY omit a main contact. When a main contact is set, that person MUST exist and MUST be a member of that record’s account (**VR-054**). Deceased people MUST NOT own **active** deals when they are the main contact (existing active-deal rule still applies).
- **FR-006**: Deal, case, and project MAY list **stakeholders** (zero or more). Stakeholders MUST be existing people, MUST NOT duplicate the main contact, and NEED NOT be members of the account (**VR-055**). This increment MUST NOT add stakeholder casts except to repair the four rows in FR-010.
- **FR-007**: A project MAY name at most one originating deal and zero or more related cases; those ids MUST exist when set (**VR-056**). Article “about this project/case” links stay publication links and are unchanged.
- **FR-008**: Project people used for “projects this person is on” (public site and in-product filters) MUST be the main contact (if any) union stakeholders. Any required undifferentiated people list is retired or derived from that union so it cannot disagree with membership.
- **FR-009**: Timeline events MAY name zero or more deals and zero or more cases; those ids MUST exist when set (**VR-057**). Case roll-up to account and main contact is presentation/export only — do not duplicate those ids as required fields on the event.
- **FR-010**: Canon MUST be hand-authored. The four invalid rows MUST be corrected as in User Story 2 (**VR-058**): keep the existing accounts; keep a main contact who **is** a member; keep the previous non-member on the record as a stakeholder; invent no empty accounts, no contact-less pipeline rows, and no extra people. Intended deputies (assumptions): Epping Forest main contact William Hargreaves with Henry Clayton stakeholder; Brazier Legal mains Mary Brazier with Richard Turpin stakeholder on `case-001` and `palmer-identity-vault`; Turpin Enterprises main contact Henry Clayton with Thomas Collier stakeholder on `case-017`.
- **FR-011**: Canon MUST NOT store per-account email, phone, or mailing address. Professional-extras contact copy is not a CRM contact and is unchanged.
- **FR-012**: **In-product export (constitution IX)**: Contact download MUST emit **one row per membership** (**VR-059**): same contact identity on each row, different account, copied email/phone/mailing address. Account download MAY include optional primary contact. Deal, case, and project downloads MUST include optional main contact and a stakeholder field (empty when omitted). Project download MUST include optional originating deal and related cases. Stakeholder export is a field on those datasets (not a requirement to invent a second file).
- **FR-013**: **Collision policy**: Turpinverse unique person key is the contact identity. Email is a copied attribute. Multi-membership rows repeat the same email on purpose. Importers whose CRM unique-keys on email MUST use contact identity (or contact+account) or apply their own suffix/skip rule. Document this on the export surface and exporter documentation — not as primary public-site copy.
- **FR-014**: **Public reference site (constitution IX)**: Show named people and accounts. Tolerate omitted main contact. Do not present technical identifiers or collision policy as primary content. Change public-site publication **only if** current pages would break or mislead under optional main contact or derived project people.
- **FR-015**: Completeness checks for FR-001–FR-010 MUST be automated and MUST have tests written to fail before the new behaviour is implemented (constitution III). This feature is specified in `specs/008-crm-join-graph` — do not treat the original universe specification as the only place this graph is recorded. Where older contracts still require a deal/case main contact or a single contact row per person, update them so documentation matches the target graph (constitution VIII).

### Key Entities

- **Contact (persona)**: One person; at least one account; one email, phone, and mailing address for the person.
- **Account (organisation)**: Zero or more member contacts; optional primary contact (member when set).
- **Membership**: A person–account link recorded on both sides when present. Export projects one contact row per membership.
- **Deal / Case / Project**: Exactly one account; optional main contact (member when set); optional stakeholders (need not be members). Project also optional originating deal and related cases. Project people = main ∪ stakeholders.
- **Timeline event**: Optional people, accounts, deals, and cases.
- **Contact download row**: One membership projection; identity fields copied; account differs per row.
- **Stakeholder**: Extra person on a deal, case, or project who may sit outside the account.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: An automated completeness check reports pass on the full authored dataset, including the four repaired rows, and fail with the offending record identity when a main contact is not an account member.
- **SC-002**: 100% of contacts still have at least one account. A reviewer can pick any person and name that account in under one minute.
- **SC-003**: A reviewer inspecting `deal-008`, `case-001`, `case-017`, and `palmer-identity-vault` confirms each keeps its original account, has a main contact who is a member, and lists the previous non-member as a stakeholder — with no new people or empty accounts added for the demo.
- **SC-004**: Contact download row count equals the number of person–account memberships. A reviewer can find two rows for at least one multi-account person (same identity and email, different accounts) within one download preview.
- **SC-005**: A reviewer mapping downloads into a CRM can place optional main contact and stakeholders on deals, cases, and projects, and can state the email collision policy after reading export documentation once (under five minutes) without finding per-account emails in canon.
- **SC-006**: A reviewer comparing the public reference site and the in-product app confirms names and stories on the public site versus identifiers and duplicate membership rows in download; omitted main contacts do not produce empty-id reader copy.
- **SC-007**: Join rules, completeness outcomes, export behaviour, and [`docs/entity-relationships.md`](../../docs/entity-relationships.md) agree. A later completeness story can assume this graph without re-litigating membership.

## Assumptions

- GitHub issue #41 is the source of scope; [`docs/entity-relationships.md`](../../docs/entity-relationships.md) is the join-graph source of truth for cardinality.
- Intended deputies for FR-010 (existing members only): William Hargreaves + Henry Clayton on `deal-008`; Mary Brazier + Richard Turpin on `case-001` and `palmer-identity-vault`; Henry Clayton + Thomas Collier on `case-017`.
- Schema **allows** empty accounts and omitted main contacts; **demo data in this increment does not add examples** of those optionality shapes.
- Stakeholder export is a single field on deal, case, and project downloads (joined list of contact identities), not a second dataset, unless planning finds a documented reason to split files.
- Account primary contact is optional in data and MAY appear as an export column; demo data NEED NOT populate it on every account.
- Existing Creative Universe Rules and constitution IX / [`docs/product-surfaces.md`](../../docs/product-surfaces.md) apply.
- This is a new feature folder (`specs/008-crm-join-graph`). The original universe specification stays the origin story of the dataset; it is not silently replaced. Contracts that would be false after this change are updated in place as documentation alignment, with traceability back here.
- Career, education, achievements, articles, galleries, professional extras, and postal-address rules are unchanged except where they consume project people or optional foreign keys.
- Leads, activities, products, quotes, and other CRM completeness children (#32–#38) are out of scope except that this feature unblocks them.
- English-only copy; no new authentication; no real people’s data.
- Validation codes **VR-052–VR-059** are allocated here. Older codes that required a deal/case main contact are reinterpreted as “when set, identity must exist” plus **VR-054** for membership.
