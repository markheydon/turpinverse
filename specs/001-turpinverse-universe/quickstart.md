# Quickstart: Turpinverse Universe & Demo Data

**Date**: 2026-07-18 | **Plan**: [plan.md](./plan.md)

Validation guide for proving the feature works end-to-end. Implementation details belong
in `tasks.md`; this document defines runnable scenarios and expected outcomes.

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.x | Blazor app, Aspire, tests |
| Node.js | 20+ | Tailwind CSS build |
| Hugo Extended | latest | Documentation site |
| Git | 2.x | Repository operations |

## Local Development Setup

```powershell
# Clone repository
git clone https://github.com/<org>/turpinverse.git
cd turpinverse

# Restore .NET solution
dotnet restore

# Install frontend dependencies (Tailwind)
cd src/Turpinverse.Web
npm install
cd ../../..

# Run via Aspire (starts Blazor Server + dashboard)
dotnet run --project src/Turpinverse.AppHost
```

**Expected**: Aspire dashboard opens; `Turpinverse.Web` shows as a running resource with
a clickable URL (typically `https://localhost:7xxx`).

## Validation Scenario 1: Canon Integrity (P1)

Proves User Story 1 — canon is complete and internally consistent.

```powershell
# Run canon validation tests
dotnet test tests/Turpinverse.Core.UnitTests --filter "Category=CanonValidation"
```

**Expected outcomes**:
- ✅ At least 15 personas loaded
- ✅ At least 8 organisations loaded
- ✅ Zero cross-reference violations (VR-001 through VR-010)
- ✅ Alias map includes `John Palmer` → `dick-turpin`

**Manual check**: Open `src/Turpinverse.Data/canon/personas.json` and confirm entries for
Dick Turpin, Elizabeth Millington, Samuel Gregory, Mary Brazier, Matthew King, Richard
Bayes, and others from the Wikipedia record.

## Validation Scenario 2: Hugo Site Build (P1)

Proves the documentation site builds and contains hyperlinked canon content.

```powershell
# Generate Hugo content from canon JSON
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent

# Build Hugo site to docs/ (local Hugo install)
cd site
hugo --minify
cd ..
```

**Without Hugo installed** (Docker / Podman — recommended on Windows):

```powershell
.\scripts\Invoke-HugoSite.ps1 build
# or live preview: .\scripts\Invoke-HugoSite.ps1 serve
```

See [site/README.md](../../../site/README.md) for container commands and troubleshooting.

**Expected outcomes**:
- ✅ `docs/index.html` exists
- ✅ `docs/personas/` contains a page per persona with cross-links
- ✅ `docs/organisations/` contains org pages with member links
- ✅ `docs/timeline/` renders chronologically sorted events
- ✅ Org chart page renders hierarchy from organisation parent/child relationships

**Manual check**: Open `docs/index.html` in a browser; navigate from Dick Turpin's page
to Essex Gang organisation and back via hyperlinks.

## Validation Scenario 3: CSV Export (P2)

Proves User Story 2 — CRM-ready datasets export with intact cross-references.

```powershell
# Start the app (if not already running)
dotnet run --project src/Turpinverse.AppHost
```

1. Open the Blazor app home page (`/`).
2. Navigate to **Contacts** (`/contacts`) and click **Download CSV**.
3. Repeat for **Accounts** (`/accounts`), **Deals** (`/deals`), and **Cases** (`/cases`).

**Expected outcomes**:
- ✅ Each CSV downloads with UTF-8 BOM header row
- ✅ Contacts CSV: ≥ 25 rows with `contactId`, `email`, `accountId` columns
- ✅ Accounts CSV: ≥ 10 rows with `accountName`, `industry`, `parentAccountId`
- ✅ Deals CSV: ≥ 20 rows; ≥ 80% reference known persona/account IDs (SC-004)
- ✅ Cases CSV: ≥ 15 rows with `subject`, `status`, `priority`

**Automated check**:

```powershell
dotnet test tests/Turpinverse.IntegrationTests --filter "Category=CsvExport"
```

## Validation Scenario 4: Export Consistency (P2)

Proves SC-002 — zero orphan references in generated data.

```powershell
dotnet test tests/Turpinverse.IntegrationTests --filter "Category=CrossReference"
```

**Expected**: All tests pass; every `accountId`/`contactId` in deals and cases resolves to
a record in accounts/contacts exports.

## Validation Scenario 5: Dashboard Visualisation (P3)

Proves Chart.js integration and demo-presenter UX.

1. Open the Blazor home page (`/`) and confirm the dataset summary doughnut chart.
2. Navigate to **Deals** (`/deals`) and view the deal pipeline bar chart.

**Expected outcomes**:
- ✅ Summary chart on home shows entity counts (accounts, contacts, deals, cases)
- ✅ Pipeline chart on deals page shows deal counts per stage
- ✅ Lucide icons render on navigation and action buttons
- ✅ Tailwind styling applied (no unstyled HTML)

## Validation Scenario 6: Tone & Professionalism (P3)

Proves User Story 3 — humour lands without breaking demo credibility.

**Manual review checklist** (3 reviewers):
1. Export all 4 CSVs and open in Excel/LibreOffice.
2. Scan 10 random records per file.
3. Rate each record 1–5 on "suitable for client-facing CRM demo".
4. Note whether Turpinverse theme is recognisable.

**Pass criteria** (SC-003, SC-006):
- ≥ 80% of reviewers rate ≥ 3/5 on professionalism
- ≥ 2 of 3 reviewers identify Turpinverse theme within first 5 records
- ≥ 90% of names read as plausible business entries at first glance

## CI Validation

```powershell
# Simulate CI pipeline locally
dotnet build --configuration Release
dotnet test --configuration Release --no-build
cd site && hugo --minify && cd ..
```

**Expected**: Build succeeds; all tests pass; Hugo builds without errors.

## GitHub Pages Deployment

**Hugo CI** (`hugo-ci.yml`) validates the site on pull requests to `main`:
canon validation, content generation, and Hugo build — no deploy.

**Hugo Deploy** (`hugo-deploy.yml`) publishes to `docs/` on push to `main` or via manual
**workflow_dispatch** in the Actions tab:
1. Runs canon validation tests
2. Generates Hugo content from canon JSON
3. Builds Hugo to `docs/`
4. Commits and pushes `docs/` for GitHub Pages

**Verify**: Visit `https://turpinverse.uk` and confirm the site loads with canon content.

## Troubleshooting

| Issue | Resolution |
|-------|------------|
| Canon validation fails | Check `personas.json` ↔ `organisations.json` bidirectional membership |
| Hugo build empty | Run content generator tool before `hugo` |
| CSV missing BOM | Ensure CsvHelper UTF-8 BOM encoding configured |
| Tailwind styles missing | Run `npm run build:css` in `Turpinverse.Web` before `dotnet run` |
| Aspire dashboard empty | Ensure `Turpinverse.AppHost` references `Turpinverse.Web` project |

## Feature Complete

All validation scenarios above pass. The `001-turpinverse-universe` feature is implemented
and documented. For ongoing changes, follow the repository contribution guidelines in
[README.md](../../../README.md).
