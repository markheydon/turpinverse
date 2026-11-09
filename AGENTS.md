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
| Why something was decided | [`adr/`](adr/) |
| Repo maintenance rules | [`docs/engineering.md`](docs/engineering.md) |
| Career/article Hugo mapping | [`docs/career-portfolio-mapping.md`](docs/career-portfolio-mapping.md), [`docs/article-gallery-mapping.md`](docs/article-gallery-mapping.md) |

Intent for new work: a GitHub issue (templates under `.github/ISSUE_TEMPLATE/`) plus the docs above. Use the issue as Cursor **Plan** mode input, then **Agent** mode. Do not add a `specs/###-feature/` folder. Labels: `type/*`, `priority/*`, `status/*`, `size/*`.

## Skills

Project skills live in [`.agents/skills/`](.agents/skills/). Read `SKILL.md` when the task matches.

| Skill | Use when |
|-------|----------|
| `create-architectural-decision-record` | Creating or major-updating an ADR |
| `documentation-writer` | Diátaxis-aligned documentation |
| `project-documentation` | Project-aware docs placement |
| `code-review` | PR or branch review before merge (see [`.cursor/commands/loop-code-review.md`](.cursor/commands/loop-code-review.md)) |
| `principal-engineering` | Non-trivial implementation, refactors, build fixes, new abstractions |
| `blazor-blueprint` | BlazorBlueprint UI in `Turpinverse.Web` |
| `aspire` (and related `aspire-*` skills) | AppHost wiring, deploy, or diagnostics |
| `aspire-project-v2-migration` | Assessing or migrating AppHost to DotnetProject resources (13.6+) |

## Task routing

- **Code** (`src/`, `tests/`): [`docs/engineering.md`](docs/engineering.md), [`.cursor/rules/csharp.mdc`](.cursor/rules/csharp.mdc).
- **Documentation** (`**/*.md` except `adr/`): `documentation-writer` + `project-documentation`.
- **ADRs** (`adr/*.md`): `create-architectural-decision-record` only.

## Tests

Follow [`.cursor/rules/testing-standards.mdc`](.cursor/rules/testing-standards.mdc): xUnit v3, NSubstitute, built-in `Assert` only.

When changing canon validation, export, or Hugo generation, add or update boundary tests.

## Central package management

- Add package versions only in [`Directory.Packages.props`](Directory.Packages.props).
- Do **not** add or remove NuGet packages unless the user has explicitly instructed that work.

Before finishing dependency-related work:

```bash
dotnet package list --outdated
```

Bump centrally managed packages to the latest compatible stable versions where appropriate, then run a full test suite after version changes:

```bash
dotnet test Turpinverse.slnx -c Release
```

## Hugo site

The public reference site lives under [`site/`](site/). Content and `site/data/*.json` are **generated** from canon — do not hand-edit generated markdown or data files except by re-running the generator.

**Dev container** installs Go, Hugo Extended, and Node (see `.devcontainer/post-create.sh`) — use `hugo` directly under `site/` after running the content generator, same as CI.

**Bare WSL/Windows (no Hugo install)** — from the repo root:

```bash
./scripts/invoke-hugo-site.sh serve    # dev server + live reload → http://localhost:1313
./scripts/invoke-hugo-site.sh build    # production build to site/public/
./scripts/invoke-hugo-site.sh preview  # build then nginx preview → http://localhost:8080
```

`serve`, `build`, and `preview` all run `dotnet run --project src/Turpinverse.Tools.GenerateHugoContent` first. After changing `HugoContentGenerator`, layouts under `site/layouts/`, or canon entities that publish on Hugo, regenerate and run `serve` or `build` to confirm templates render.

**Layout data files:** Hugo keys match the JSON filename (without `.json`). Hyphenated files must use `index hugo.Data "credit-notes"` (same pattern as `tax-rates`), not `hugo.Data.creditNotes`.

Unit tests: `Category=CanonValidation` for generator parity (e.g. `CommittedSiteInvoicesDataTests`); see [`site/README.md`](site/README.md) for theme and deploy detail.

## Pull request verification

From the repo root before opening a PR:

```bash
dotnet build Turpinverse.slnx -c Release
dotnet test Turpinverse.slnx -c Release
```

When changing Hugo layouts or generated content, also run `./scripts/invoke-hugo-site.sh build`.

## Historical artefacts

Frozen Spec Kit packs: [`archive/specs/`](archive/specs/) (after migration). Do not treat them as source of truth.

## Build

```bash
dotnet build
dotnet test
dotnet run --project src/Turpinverse.AppHost
```

Canon JSON is embedded from `canon/` via `Turpinverse.Data.csproj` and `Turpinverse.Core.csproj` — do not reintroduce a `src/Turpinverse.Data/canon/` tree.

## Not allowed without explicit instruction

- Add or remove NuGet packages (except centrally pinned version maintenance when asked).
- Modify CI/CD behaviour without a clear request.
- Commit secrets or API keys.
