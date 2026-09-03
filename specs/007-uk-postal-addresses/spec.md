# Feature Specification: UK Postal Addresses

**Feature Branch**: `007-uk-postal-addresses`

**Created**: 2026-09-03

**Status**: Draft

**Input**: User description: "Add postal addresses to Turpinverse Organisation and Contact (Persona) canon so demo CRM data looks like a UK system." (full description from GitHub issue #30)

**Source issues**:

- [#30 Registered office and contact postal addresses](https://github.com/markheydon/turpinverse/issues/30)

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Every organisation has a registered office (Priority: P1)

A product demonstrator importing Turpinverse **accounts** into a CRM needs every organisation to look like a UK company: it has a **registered office**, not a missing address and not a generic “headquarters” label unless that is also true. Reviewers browsing the public reference site must see that office on every organisation page as readable premises, town, and postcode.

**Why this priority**: A company without a registered office is not realistic UK demo data. Account records are the primary CRM object this feature must make believable. Contact addresses are optional and can wait.

**Independent Test**: Confirm every organisation has one complete registered office (first line, town, postcode, country; optional second line, third line, and region as authored), that parent and subsidiary organisations do not share the same door even if they share a town, and that the public reference site always shows the office in human-readable form (no export column names or technical identifiers as primary copy).

**Acceptance Scenarios**:

1. **Given** the full organisation set, **When** a reviewer inspects each organisation, **Then** every record has exactly one registered office with required first line, town, postcode, and country filled.
2. **Given** an organisation page on the public reference site, **When** a visitor reads the organisation, **Then** they always see a registered-office block in ordinary postal layout (not labelled only as “HQ” unless the copy also states that is true).
3. **Given** related organisations that share a geography (for example a parent and a subsidiary), **When** a reviewer compares their offices, **Then** they may share a town but MUST NOT share the same premises line and postcode as if they were one door.
4. **Given** the in-product export app, **When** a reviewer downloads accounts, **Then** each account row includes the registered-office field set (first, second, and third lines, town, region, postcode, country), with unused optional lines empty rather than omitted from the column set.

---

### User Story 2 - A minority of contacts have a mailing address (Priority: P2)

A demonstrator importing **contacts** needs the messy pattern of a real UK CRM: most people have **no** personal mailing address (the account address is assumed), while a clear minority do. Richard Turpin MUST have a personal address distinct from Turpin Enterprises. About two other people (intended: Mary Brazier and James Smith) also have addresses. Workplace-is-the-address people (for example Elizabeth Millington at the inn) and the equine/fleet asset Black Bess MUST NOT have a personal address — the stable is company property.

**Why this priority**: Contact addresses make the dataset look like a lived-in CRM, but only after organisations already have offices. Filling every contact would look synthetic.

**Independent Test**: Confirm Richard Turpin has a complete personal mailing address different from his organisation’s registered office; confirm two additional named contacts have complete addresses; confirm most contacts including Black Bess and Elizabeth Millington omit an address entirely; confirm the public reference site shows a persona address only when one exists; confirm the export download uses empty strings in mailing columns when there is no address.

**Acceptance Scenarios**:

1. **Given** Richard Turpin, **When** a reviewer inspects his record, **Then** he has a complete mailing address that is not the same premises as Turpin Enterprises’ registered office.
2. **Given** the contact set, **When** a reviewer counts who has a mailing address, **Then** only a clear minority have one (Richard plus two others; intended Mary Brazier and James Smith), and the rest omit the address object entirely.
3. **Given** Black Bess and Elizabeth Millington, **When** a reviewer inspects those records, **Then** neither has a mailing address.
4. **Given** a persona page on the public reference site, **When** that persona has no mailing address, **Then** no empty address heading or placeholder block is shown.
5. **Given** a persona page on the public reference site, **When** that persona has a mailing address, **Then** the visitor sees a human-readable postal layout (not technical identifiers or export column names as primary copy).
6. **Given** the in-product export app, **When** a reviewer downloads contacts, **Then** mailing columns are always present; contacts without an address have empty strings in those columns, not a pointer to the organisation office.

---

### User Story 3 - UK-shaped address fields on both channels (Priority: P3)

An importer mapping Turpinverse downloads into a UK CRM expects the familiar field names: first, second, and third address lines, town, optional region (county is fine), postcode, and country (default United Kingdom). Some records use a third line because UK systems often have three lines; many will not. The same nested address shape is reused for organisation registered office and contact mailing address. There is one address per entity — no billing/shipping split.

**Why this priority**: Field vocabulary and channel presentation make the data usable in demos; they are independently testable once offices and the contact minority exist.

**Independent Test**: Confirm required fields are present whenever an address exists; confirm at least some addresses populate a third line and some leave it unused; confirm country defaults to United Kingdom when authored that way; confirm organisation and contact downloads flatten to the UK field set below; confirm professional profile contact extras (email/copy/phone) are unchanged; confirm no maps and no new account-detail product page.

**Acceptance Scenarios**:

1. **Given** any stored address, **When** a reviewer inspects it, **Then** it has first line, town, postcode, and country; second line, third line, and region MAY be empty; country is United Kingdom unless a later story says otherwise.
2. **Given** the full authored address set, **When** a reviewer scans third lines, **Then** at least one organisation and at least one contact address use a third line, and many addresses do not.
3. **Given** the public reference site and the in-product export app, **When** a reviewer checks both, **Then** each shows the addresses canon actually has: offices always; contact mailing addresses only when present. Readable site vs importable columns follows the channel charter in [`docs/product-surfaces.md`](../../docs/product-surfaces.md).
4. **Given** professional profile extras contact copy (email, phone, outreach wording), **When** a reviewer looks for postal fields there, **Then** they are not stored or shown as part of that extras block.

---

### Edge Cases

- What if a contact has no mailing address? Omit the address on the persona. The public reference site shows no address block. The download still has mailing columns, filled with empty strings. There is no “same as organisation” flag.
- What if Richard’s personal address matches Turpin Enterprises? Invalid. His mailing address MUST be a distinct premises.
- What if Black Bess or inn staff receive a personal address? Invalid for this feature. The stable and the inn are organisation property / workplace, not a personal mailing address.
- What if two organisations share town and county? Allowed. Sharing the same first line and postcode (one door) is not allowed.
- What if optional second line, third line, or region is unused? Leave them empty. Required fields (first line, town, postcode, country) MUST still be present whenever an address exists.
- What if every address skips the third line? Invalid for this feature’s UK-system flavour: some records MUST use a third line.
- What if copy labels an organisation address only as “HQ”? Prefer “registered office”. “HQ” MAY appear only if it is also true in the narrative.
- What if someone adds billing and shipping addresses? Out of scope. One address per entity.
- What if someone geocodes, maps, or stores coordinates? Out of scope.
- What if someone adds a new account-detail screen in the export app? Out of scope. Account list/preview MAY show town; contact detail shows a mailing address only when present.
- What if postal fields are added to professional-extras contact? Out of scope. Email, copy, and phone stay there; postal addresses live on organisation and persona records.
- What if a postcode or premises matches a real private dwelling? Invalid. Invent premises and postcodes from historical anchors (Hempstead, Epping, York, East Yorkshire, turnpike/forest). Professional at first glance; Turpinverse humour on a second read.

## Failure Modes & Error Handling *(required when feature defines external boundaries or user-facing failures)*

This increment extends published canon and existing organisation, persona, account, and contact surfaces. It does not add maps, a new account-detail product, or a second download format.

| Failure Mode | Trigger | Expected Handling | User/System Outcome |
|--------------|---------|-------------------|----------------------|
| Organisation missing registered office | Any organisation lacks a complete office | Automated completeness check fails and lists the organisation | Delivery is not accepted until every organisation has a complete office |
| Incomplete address | An address is present but missing first line, town, postcode, or country | Automated completeness check fails | Partial addresses are not treated as complete |
| Richard without personal address | Primary persona has no mailing address | Completeness check fails | Richard MUST have a distinct personal address |
| Forbidden personal address | Black Bess or workplace-is-the-address people (e.g. Elizabeth Millington) have a mailing address | Completeness check fails | Those records omit address |
| Contact majority has addresses | Most contacts are given mailing addresses | Completeness check fails against the minority target | Export still looks like a messy CRM (sparse person addresses) |
| Identical org premises | Two organisations share the same first line and postcode | Completeness or review rejects the pair | Towns MAY be shared; doors MUST NOT |
| Real private address | Premises or postcode identify a real private dwelling | Content review rejects the record | Invented premises only |
| Empty address block | Public site shows an address heading when the persona has no address | Publication MUST omit the block | Visitors never see an empty mailing-address section |
| Postal fields on extras contact | Professional-extras contact gains postal lines | Treat as out of shape for this feature | Postal data stays on organisation registered office and persona mailing address |
| “Same as account” pointer | Contact address is stored as a reference to the organisation office | Invalid | Missing contact address *is* the inheritance story |

## Security & Access *(required when feature handles sensitive data, authentication, or external exposure)*

Addresses are **fictional** demo premises published on the same public reference site and export app that already show other Turpinverse records. They MUST NOT be real private addresses. No new authentication surface is introduced.

| Asset / Surface | Threat | Access Control | Verification |
|-----------------|--------|----------------|--------------|
| Registered offices and mailing addresses | Readers treating invented premises as real private dwellings | Clearly in-universe Turpinverse fiction; invent names and postcodes from historical anchors | Review for real residential/PII matches |
| Public reference site | Addresses visible to anyone who can browse organisation and persona pages | Same access as existing canon pages; human-readable layout only | Reviewer can open existing organisation and persona pages |
| In-product export download | Address columns included in importable files | Same access as existing account/contact downloads; no new anonymous bulk surface | Reviewer can preview and download existing account and contact datasets |

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Every organisation MUST have exactly one **registered office**. An organisation without a complete office is invalid. The office is the organisation address; reader-facing copy MUST treat it as a registered office (UK company-law flavour), not a generic headquarters label unless that is also true.
- **FR-002**: A persona MAY have exactly one **mailing address**. When there is none, the address MUST be omitted (not an empty object presented as a block). There is no same-as-organisation pointer; a missing contact address is how inheritance from the account is implied.
- **FR-003**: An address, when present, MUST use this UK CRM field set: first line (`address1`, required), second line (`address2`, optional), third line (`address3`, optional), town (required), region (optional; UK county is fine), postcode (required), country (required; default United Kingdom). The same nested shape is reused for organisation registered office and persona mailing address.
- **FR-004**: One address per entity. The system MUST NOT store a billing/shipping split, coordinates, or map links for this feature.
- **FR-005**: Richard Turpin MUST have a complete mailing address that is a distinct premises from Turpin Enterprises’ registered office.
- **FR-006**: In addition to Richard Turpin, exactly two other contacts MUST have complete mailing addresses. The intended pair is Mary Brazier and James Smith. All other personas MUST omit a mailing address, including Black Bess (equine/fleet asset; the stable is company property) and people whose workplace is the address (including Elizabeth Millington at the inn). The result MUST be a clear minority of contacts with addresses (three of the current contact set).
- **FR-007**: Every organisation MUST have a distinct registered office tied to existing historical geography (Hempstead / Epping corridor, York, East Yorkshire, forest/turnpike). Parent and subsidiary MAY share a town; they MUST NOT share the same door (same first line and postcode). Premises names and postcodes MUST be invented. Copy MUST read as professional at first glance, with Turpinverse humour on a second read, and MUST follow existing creative rules (legend-as-business, euphemism, demo-credible).
- **FR-008**: Across authored addresses, some records MUST populate a third address line (UK systems often have three lines) and most MAY leave it unused.
- **FR-009**: Completeness rules for this feature MUST be enforced by an **automated completeness check** that fails until they hold: every organisation has a complete registered office; any present persona address is complete; Richard Turpin has an address; Black Bess and Elizabeth Millington do not; the contact-with-address count matches the minority target; required fields are non-empty when an address exists. Human review alone is not sufficient for those gates. Invented-premises / no-real-private-address remains a content-review check.
- **FR-010**: **Public reference site (constitution IX)**: organisation pages MUST always show the registered office in human-readable postal layout. Persona pages MUST show a mailing address only when present. Reader-facing pages MUST NOT present technical identifiers, join keys, or export column names as primary content.
- **FR-011**: **In-product export app (constitution IX)**: account and contact downloads MUST flatten address fields for import. Account columns: `registeredOfficeAddress1`, `registeredOfficeAddress2`, `registeredOfficeAddress3`, `registeredOfficeTown`, `registeredOfficeRegion`, `registeredOfficePostcode`, `registeredOfficeCountry`. Contact columns: `mailingAddress1`, `mailingAddress2`, `mailingAddress3`, `mailingTown`, `mailingRegion`, `mailingPostcode`, `mailingCountry`. When a contact has no address, those mailing columns MUST be empty strings. Contact detail MAY show a mailing address when present. Account list/preview MAY show town. This feature MUST NOT add a new account-detail page.
- **FR-012**: Postal addresses MUST NOT be added to professional-extras contact (email, copy, phone). Dual publication uses existing organisation and person/contact surfaces only. Maps, geocoding, and a second public site are out of scope.

### Key Entities

- **Postal address**: Nested value used once per entity. Fields: first line, optional second line, optional third line, town, optional region, postcode, country (default United Kingdom).
- **Organisation registered office**: Required postal address on every organisation. Distinct door per organisation.
- **Persona mailing address**: Optional postal address on a contact. Omitted for most people. Required for Richard Turpin; also present for two additional named contacts.
- **Account download row**: Flattened registered-office columns for CRM import.
- **Contact download row**: Flattened mailing columns for CRM import; empty strings when the persona has no address.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of organisations have a complete registered office (first line, town, postcode, country). A reviewer can open any organisation on the public reference site and see that office in under two minutes per record.
- **SC-002**: Richard Turpin has a mailing address that a reviewer can distinguish from Turpin Enterprises’ registered office in one side-by-side comparison. At least one other contact has no mailing address. Black Bess and Elizabeth Millington have no mailing address.
- **SC-003**: Contacts with a mailing address are a clear minority: exactly three people (Richard plus two others), which is under 20% of the current contact set.
- **SC-004**: A reviewer comparing the public reference site and the in-product download confirms both channels show what canon has: offices always; contact mailing addresses only when present. Persona pages without an address show no empty address section. Download mailing columns for those contacts are empty strings.
- **SC-005**: Account and contact download columns match the UK field set in FR-011 (three address lines, town, region, postcode, country, with registered-office vs mailing prefixes). A reviewer mapping into a UK CRM can complete column matching without inventing extra Turpinverse-only address types (no billing/shipping split).
- **SC-006**: A reviewer scanning addresses finds no real private dwellings, at least one third address line in use, and copy that is demo-credible on first read.
- **SC-007**: An automated completeness check reports pass only when FR-001–FR-006 and FR-008–FR-009 hold, and fail with the offending record identities otherwise.

## Assumptions

- GitHub issue #30 is the source of scope.
- The current organisation count is ten; all ten receive a registered office in this feature. New organisations added later MUST also have a registered office (FR-001 remains the rule).
- The current persona count is twenty-five; the three addressed contacts are Richard Turpin, Mary Brazier, and James Smith unless a later clarification names different deputies. Volume is “clear minority,” not “every person.”
- Existing Creative Universe Rules apply to address lines.
- Country is United Kingdom for all records in this increment (no overseas registered offices).
- Region, when used, is a UK county or ceremonial county.
- Publication follows constitution IX and [`docs/product-surfaces.md`](../../docs/product-surfaces.md): Hugo/public site is the readable showcase; the in-product app is explore/filter/download. Shared canon is the single source of truth.
- This feature is a new specification; it does not rewrite the original universe spec in place.
- English-only copy, consistent with existing canon.
- No new authentication, payment, or personal data of real people.
- Account preview showing town is optional polish; showing registered office on every organisation page and flattening CSV columns is required.
