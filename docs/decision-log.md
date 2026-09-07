# Decision log

Why Turpinverse data and code look the way they do. Newest entries first. For field-level join rules see [entity-relationships.md](./entity-relationships.md).

---

## Commercial join graph (#45, 2026-09)

Child stories #33–#38 share one frozen ERD in [entity-relationships.md](./entity-relationships.md). Key decisions:

**Account roles.** `Organisation.roles` is a unique array of `customer` | `supplier` | `partner`, min 0 — not a single primary `role`. An org can be customer and supplier (and partner). Hats are authored, not inferred. Turpin Enterprises is the implicit home books; no `sellerAccountId` on documents.

**Invoice party.** Sales invoice **must** have exactly one `accountId`. Project, quote, sales order, deal, and case lineage lives on **lines**, not the invoice/bill header — one invoice can cover several orders or projects.

**Optional catalogue on lines.** `productId` is optional on every document line. A whole-project quote or sales order can be one lump line with `projectId` and no product.

**Payments.** One `Payment` targets `invoiceId` **XOR** `billId`.

**case-011.** AP dispute links to a **Bill** (`caseId` on header), not a sales invoice — corrects #35 wording.

**Leads** are pre-contact strings until `Converted` → existing `convertedContactId`. **Activities** are separate from CanonEvent; closed `regardingType` enum on activities.

**SME import.** Line-level `projectId` matches Xero tracking / Sage 50 / FreeAgent item projects better than a header-only project. Sales-order FKs are dropped on Xero/FreeAgent/QBO import (no SO object there). QBO may need a dummy item when `productId` is blank.

Rejected: single `role`; header `projectId` / `salesOrderId` / `quoteId` on invoices; required `productId` on lines; required quote→order→invoice chain; `quoteLineId` / `salesOrderLineId`; supplier credit notes in v1.

---

## CRM join graph and membership export (2026-09)

Contacts export as **one CSV row per account membership**, not one row per person. The unique person key is `contactId` (persona slug); email is copied onto each row and may repeat.

Deals, cases, and projects have an optional main contact who **must** be an account member when set. Other people can be stakeholders without membership. Projects use main contact ∪ stakeholders only — no separate undifferentiated people list.

Four story rows were repaired (`deal-008`, `case-001`, `case-017`, `palmer-identity-vault`) by moving non-member names to stakeholders rather than inventing fake membership.

Rejected: requiring a main contact on every deal/case; a second `personaIds` list on projects; adding people to accounts solely to preserve an invalid main contact.

---

## Nested UK addresses (2026-09)

Every organisation has a required `registeredOffice`. Personas may have an optional mailing `address`. Same nested `Address` object (`address1`…`country`) on both.

Rejected: a third `addresses.json` file; flattened `registeredOfficeAddress1` fields; postal data on professional-extras contact; geocoding or real postcode validation.

---

## Articles and galleries are Hugo-only datasets (2026-08)

Articles and galleries are canon records for the public reference site. They are **not** CRM export datasets — no CSV columns or Blazor dataset pages.

Rejected: treating articles as another export type; collection taxonomy pages on Hugo.

---

## Professional profile extras sandwich (2026-08)

Intro, about, skills, contact, and socials live in `professional-extras.json` and render in sandwich order on Hugo persona pages and Blazor contact detail. Empty sections are omitted; competing title/biography/email lines are suppressed when extras exist.

Rejected: stuffing the same copy into persona root fields; a second showcase product on Blazor.

---

## Career and portfolio as generic canon (2026-08)

Experience, education, projects, and achievements are first-class JSON entities, not a copy of any one personal-site theme. Projection to Hugo/Blazor layout is documented in [career-portfolio-mapping.md](./career-portfolio-mapping.md).

Rejected: Hugo-only markdown hand-maintained beside JSON; per-theme schema forks.

---

## Hugo vs Blazor channel split (2026-08)

Hugo showcases human-readable demo data without export plumbing or technical IDs as primary content. Blazor explores, filters, and downloads importable CSV. Shared canon is the single source of truth.

Filtered export: preview and download share the same query parameters; zero matches on download returns **409**, not an empty CSV.

Rejected: deals/cases only as spreadsheet views on the public site; silent empty downloads.

---

## Public export API gate (2026-07)

`/api/export/*` and `/api/canon/validate` are intentional anonymous demo endpoints when `Export:PublicApiEnabled` is true (default in Development, false in Production). They use the `DemoExport` policy and are not registered when disabled.

Rejected: `AllowAnonymous()` with no documented gate; full OAuth for fictional demo data.

API failures use RFC 7807 Problem Details via `IExceptionHandler`, not the Blazor HTML error page.

---

## Solution shape and tooling (2026-07)

- **.NET Aspire** orchestrates the Blazor app for local dev (dashboard, OTel, future services) instead of plain `dotnet run` on Web alone.
- **Six projects** under `src/` separate canon data loading, domain logic, UI, orchestration, and the Hugo CLI — enables testing validation without the web host.
- **Hugo content generation in Core** keeps canon-driven site generation next to CSV export instead of hand-maintained markdown or a standalone `tools/` script.
- **Chart.js** on the Blazor dashboard for pipeline and summary charts in sales-demo scenarios.

Rejected: monolithic single project; maintaining Hugo markdown separately from JSON canon.
