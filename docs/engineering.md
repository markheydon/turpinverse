# Engineering rules

How this repository is maintained. These apply to **code and docs in this repo**, not to every consumer of the `canon/` datasets.

## 1. Canon is the product; apps are consumers

The JSON under [`canon/`](../canon/) is the published demo universe. Hugo and the Blazor export app embed and project that data. Other projects may read `canon/` directly without using this solution.

Channel responsibilities for the bundled consumers: [product-surfaces.md](./product-surfaces.md).

## 2. Test boundary changes

When you change canon validation, CSV export, the Hugo content generator, or export HTTP endpoints, add or update automated tests. Prefer contract or integration coverage at those boundaries.

Testing stack: xUnit v3, NSubstitute — see [README](../README.md) and [`.cursor/rules/testing-standards.mdc`](../.cursor/rules/testing-standards.mdc).

## 3. Explicit errors at API boundaries

Export and validation endpoints must not swallow failures. Use RFC 7807 Problem Details for API errors. When `Export:PublicApiEnabled` is `false`, endpoints are not registered (404).

See [export-api.md](./export-api.md) and the README security note.

## 4. Docs and schemas match code

If behaviour changes, update the living doc or schema in the same change (or revert the code). Stale docs are worse than none.

Authoritative artefacts:

| Concern | Location |
|---------|----------|
| Dataset files | `canon/*.json` |
| JSON Schema | `canon/schema/canon-schema.json` |
| CRM CSV columns | `canon/schema/crm-export-schema.json` |
| Join graph | [entity-relationships.md](./entity-relationships.md) |
| Validation codes | [validation-rules.md](./validation-rules.md) + `CanonValidator.cs` |
| HTTP export | [export-api.md](./export-api.md) |
| Past decisions | [decision-log.md](./decision-log.md) |

## 5. Issues and pull requests

File work with the GitHub issue forms under [`.github/ISSUE_TEMPLATE/`](../.github/ISSUE_TEMPLATE/) (story, bug, chore). Blank issues remain allowed. The issue is the input for Cursor Plan mode, then Agent mode.

Labels use the same taxonomy as other solo-dev-board repos: `type/*`, `priority/*`, `status/*`, `size/*`. Forms apply a type, `status/todo`, and a default priority; adjust size and priority on the issue after filing. Dependabot PRs use `type/chore`.

Pull requests use [`.github/pull_request_template.md`](../.github/pull_request_template.md) and should close the issue they implement.

## What we dropped

This repo no longer uses GitHub Spec Kit. There is no requirement for a feature spec, plan, or tasks file before implementation. Use GitHub issues and the docs above for intent.

Historical Spec Kit packs live under [`archive/specs/`](../archive/specs/) (frozen, not maintained).
