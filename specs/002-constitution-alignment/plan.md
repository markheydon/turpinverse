# Implementation Plan: Constitution v2 Alignment

**Branch**: `002-constitution-alignment` | **Date**: 2026-07-22 | **Spec**: N/A (meta-alignment)

**Input**: Constitution v2.0.0 ratified 2026-07-22; code scanning alert
[#3](https://github.com/markheydon/turpinverse/security/code-scanning/3); retrospective
gap analysis of `001-turpinverse-universe`.

## Summary

Retrofit specifications, contracts, and code so they satisfy constitution v2
principles as if those rules had been plan restrictions from day one. Primary
gaps: missing Security & Access and Failure Modes artifacts in `001` spec,
stale constitution check in `001` plan, unauthenticated export API without an
explicit environment gate (CodeQL alert #3), and domain-layer exceptions that
bypass the documented RFC 7807 error contract.

## Code Scanning Alert #3

| Field | Value |
|-------|-------|
| URL | https://github.com/markheydon/turpinverse/security/code-scanning/3 |
| Inferred rule | `cs/web/missing-function-level-access-control` |
| Location | `src/Turpinverse.Web/Endpoints/ExportEndpoints.cs`, `CanonEndpoints.cs` |
| Issue | `/api/export/*` and `/api/canon/validate` are mapped without authorization |
| Risk | Public CSV download and validation introspection if app is deployed to an open network |
| Accepted in dev | Yes — fictional demo data, no PII, local Aspire demos |
| Not accepted in prod | Endpoints MUST be disabled or require auth when `PublicApiEnabled` is false |

**Remediation** (constitution Principle VI — Security by Design):

1. Document threats and controls in `001` spec Security & Access table.
2. Add `Export:PublicApiEnabled` configuration (default `true` in Development, `false` otherwise).
3. Register endpoints only when enabled; attach `RequireAuthorization("DemoExport")` policy that succeeds only when the flag is on.
4. Add integration test proving endpoints return 404 when disabled.
5. Dismiss or auto-close alert #3 after merge with link to spec security table.

## Day-One Architecture (retrospective)

If constitution v2 had governed the original `001` plan, artifacts and code would
have looked like this:

### Traceability (Principle I)

| Artifact | Day-one state | Current gap |
|----------|---------------|-------------|
| `spec.md` | FR-001–FR-014 with Security & Failure Modes sections | Missing security/error sections |
| `plan.md` | Yes/no Constitution Check for all 8 principles | Old principle names (I–V) |
| `tasks.md` | Tasks cite spec FR + constitution principle | Principle III reference only |
| `contracts/export-api.md` | Security model + error matrix | Security implicit in README only |

### Independence (Principle II)

No structural change required. User stories P1→P2→P3 remain valid.

### Testability (Principle III)

| Area | Day-one state | Current gap |
|------|---------------|-------------|
| Export API | Contract tests for 200/400/404/422 | No test for disabled API (404) |
| Canon validate | 200 vs 422 paths covered | ✅ |
| Boundary coverage | Integration tests for cross-refs | ✅ |

### Separation of Concerns (Principle IV)

Current layout already matches day-one intent:

```text
Turpinverse.Data/     → persistence (embedded JSON)
Turpinverse.Core/     → domain, export mapping, validation
Turpinverse.Web/      → HTTP adapters + Blazor UI
```

Endpoints stay thin; no business logic moves into Razor components.

### Explicit Error Handling (Principle V)

| Failure mode | Day-one handling | Current gap |
|--------------|------------------|-------------|
| Invalid dataset | 400 Problem Details at HTTP boundary | ✅ in endpoints |
| Canon load failure | 500 Problem Details | ⚠️ unhandled exception may leak stack |
| Invalid dataset in Core | Typed `ExportResult` / no throw | ⚠️ `CsvExportService` throws `ArgumentException` |
| Validation violations | 422 with violation list | ✅ |

**Day-one code pattern**: HTTP layer validates input; Core returns `Result<T>`;
unhandled exceptions mapped to RFC 7807 via `IExceptionHandler` for `/api/*`.

### Security by Design (Principle VI)

| Surface | Day-one control | Current gap |
|---------|-----------------|-------------|
| `/api/export/*` | Gated by `Export:PublicApiEnabled`; documented threat model | Always mapped; CodeQL alert #3 |
| `/api/canon/validate` | Same gate | Always mapped |
| Health checks | Development only | ✅ (ServiceDefaults) |
| Blazor UI | Same-origin; no sensitive actions | ✅ |

**Day-one code pattern**:

```text
Program.cs
  → read Export:PublicApiEnabled
  → if enabled: MapExportEndpoints().RequireAuthorization("DemoExport")
  → else: endpoints not registered (404)

DemoExportAuthorizationHandler
  → succeeds only when PublicApiEnabled == true
```

### Justified Complexity (Principle VII)

Existing Complexity Tracking in `001` plan remains valid. No new violations.

### Documentation Contract (Principle VIII)

Update `001` plan Constitution Check, spec security/error sections, and
`export-api.md` to match implemented gating behaviour.

## Constitution Check

*GATE: Must pass before implementation. Re-check after code changes.*

| Principle | Status | Evidence |
|-----------|--------|----------|
| I. Spec-Driven Traceability | Yes | This plan traces to constitution v2 + alert #3 |
| II. Incremental Independence | N/A | Alignment work; no new user stories |
| III. Verifiable Testability | Yes | New 404-when-disabled integration test |
| IV. Separation of Concerns | Yes | Policy handler in Web; options in Web; Core unchanged for gating |
| V. Explicit Error Handling | Yes | API exception handler + spec failure-mode table |
| VI. Security by Design | Yes | Config gate + spec threat table + alert remediation |
| VII. Justified Complexity | Yes | Policy handler simpler than full auth stack for demo app |
| VIII. Documentation Contract | Yes | Spec, plan, contract updated in same PR |

**Gate result**: PASS

## Project Structure

No new projects. Files touched:

```text
specs/001-turpinverse-universe/
├── spec.md              # + Security & Access, Failure Modes
├── plan.md              # Constitution Check v2 table
└── contracts/export-api.md  # + Security model, disabled API behaviour

specs/002-constitution-alignment/
├── plan.md              # This file
└── tasks.md             # Actionable remediation tasks

src/Turpinverse.Web/
├── Configuration/ExportApiOptions.cs
├── Authorization/DemoExportAuthorizationHandler.cs
├── Infrastructure/ApiExceptionHandler.cs
├── Endpoints/ExportEndpoints.cs   # RequireAuthorization
├── Endpoints/CanonEndpoints.cs    # RequireAuthorization
└── Program.cs                       # conditional mapping + auth

tests/Turpinverse.IntegrationTests/
└── Export/ExportApiSecurityTests.cs
```

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Custom `DemoExport` authorization policy | Satisfies CodeQL access-control query while preserving intentional anonymous demo access when explicitly enabled | `AllowAnonymous()` alone does not document or gate access; full OAuth is out of scope for v1 demo app |
| `IExceptionHandler` for API routes | Unhandled canon/load failures must return RFC 7807 per contract | Relying on Blazor `/Error` page returns HTML, not Problem Details |
