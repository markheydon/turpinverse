# Agent guide

Turpinverse is **not** a Spec Kit project. Do not run `/speckit-*` commands or expect `specs/` feature packs for new work.

## What this repo is

1. **Public datasets** — JSON under [`canon/`](canon/) (the product). Schema: [`canon/schema/canon-schema.json`](canon/schema/canon-schema.json).
2. **Consumers** — Hugo site and Blazor export app embed that canon. Other repos may read `canon/` directly.
3. **Validation** — `CanonValidator` in `src/Turpinverse.Core/Validation/`; rules summarised in [`docs/validation-rules.md`](docs/validation-rules.md).

## Where to look

| Task | Read first |
|------|------------|
| Edit demo data | `canon/*.json`, then run `Category=CanonValidation` tests |
| Join / export rules | [`docs/entity-relationships.md`](docs/entity-relationships.md) |
| Hugo vs Blazor behaviour | [`docs/product-surfaces.md`](docs/product-surfaces.md) |
| Copy / humour | [`docs/universe-voice.md`](docs/universe-voice.md) |
| HTTP export API | [`docs/export-api.md`](docs/export-api.md) |
| Why something was decided | [`docs/decision-log.md`](docs/decision-log.md) |
| Repo maintenance rules | [`docs/engineering.md`](docs/engineering.md) |
| Career/article Hugo mapping | [`docs/career-portfolio-mapping.md`](docs/career-portfolio-mapping.md), [`docs/article-gallery-mapping.md`](docs/article-gallery-mapping.md) |

Intent for new work: a GitHub issue (templates under `.github/ISSUE_TEMPLATE/`) plus the docs above. Use the issue as Cursor **Plan** mode input, then **Agent** mode. Do not add a `specs/###-feature/` folder. Labels: `type/*`, `priority/*`, `status/*`, `size/*`.

## Tests

Follow [`.cursor/rules/testing-standards.mdc`](.cursor/rules/testing-standards.mdc): xUnit v3, NSubstitute, built-in `Assert` only.

When changing canon validation, export, or Hugo generation, add or update boundary tests.

## Historical artefacts

Frozen Spec Kit packs: [`archive/specs/`](archive/specs/) (after migration). Do not treat them as source of truth.

## Build

```bash
dotnet build
dotnet test
dotnet run --project src/Turpinverse.AppHost
```

Canon JSON is embedded from `canon/` via `Turpinverse.Data.csproj` and `Turpinverse.Core.csproj` — do not reintroduce a `src/Turpinverse.Data/canon/` tree.
