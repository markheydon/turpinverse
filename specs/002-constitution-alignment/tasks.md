---
description: "Constitution v2 alignment and code scanning alert #3 remediation"
---

# Tasks: Constitution v2 Alignment

**Input**: [plan.md](./plan.md), constitution v2.0.0, [code scanning alert #3](https://github.com/markheydon/turpinverse/security/code-scanning/3)

**Prerequisites**: Constitution v2.0.0 merged or on branch; `001-turpinverse-universe` feature complete

## Phase 1: Specification & Contract Updates

- [X] T001 Update `specs/001-turpinverse-universe/plan.md` Constitution Check to v2 yes/no table (Principles I–VIII)
- [X] T002 Add Failure Modes & Error Handling section to `specs/001-turpinverse-universe/spec.md`
- [X] T003 Add Security & Access section to `specs/001-turpinverse-universe/spec.md` referencing alert #3 remediation
- [X] T004 Update `specs/001-turpinverse-universe/contracts/export-api.md` with security model and 404-when-disabled behaviour
- [X] T005 Update `specs/001-turpinverse-universe/tasks.md` constitution principle references to v2 numbering

## Phase 2: Security Gating (Alert #3)

- [X] T006 [P] Create `ExportApiOptions` in `src/Turpinverse.Web/Configuration/ExportApiOptions.cs`
- [X] T007 [P] Create `DemoExportAuthorizationHandler` in `src/Turpinverse.Web/Authorization/DemoExportAuthorizationHandler.cs`
- [X] T008 Wire options, authorization, and conditional endpoint mapping in `src/Turpinverse.Web/Program.cs`
- [X] T009 Add `RequireAuthorization("DemoExport")` to `ExportEndpoints.cs` and `CanonEndpoints.cs`
- [X] T010 Add `appsettings.json` / `appsettings.Development.json` defaults for `Export:PublicApiEnabled`

## Phase 3: Explicit Error Handling

- [X] T011 [P] Create `ApiExceptionHandler` in `src/Turpinverse.Web/Infrastructure/ApiExceptionHandler.cs` returning RFC 7807 for `/api/*`
- [X] T012 Register `IExceptionHandler` in `Program.cs`

## Phase 4: Tests & Validation

- [X] T013 Add `ExportApiSecurityTests.cs` — endpoints return 404 when `PublicApiEnabled` is false
- [ ] T014 Verify existing integration tests pass with `PublicApiEnabled` true in test host
- [ ] T015 Run `dotnet test Turpinverse.slnx` and quickstart export scenarios

## Phase 5: Documentation

- [X] T016 Align `README.md` API security note with spec Security & Access table
- [ ] T017 Close or dismiss code scanning alert #3 with link to spec security section
