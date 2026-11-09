---
name: code-review
description: >-
  Review Turpinverse changes as a principal architect would for pull request approval:
  bugs, security, architecture, complexity, and maintainability only. Use when
  the user runs /code-review, asks for a code review, or wants findings on a
  branch, PR, or local diff before merge. Read-only unless the user explicitly
  asks to implement fixes.
---

# Code review

Review as a principal architect and senior developer. Treat the change set like a pull request from another experienced developer.

Focus only on issues that would influence your decision to **approve or reject** the pull request. Findings must be the primary focus, ordered by severity. Do not make code changes unless the user explicitly asks for them.

This skill is the default in-place review for Turpinverse. For automated Bugbot or security-focused subagent reviews, use `/review-bugbot` and `/review-security` unless the user wants this review only.

## Scope

Decide what to diff before reading code:

| User intent | Diff |
| --- | --- |
| Default (branch work, PR, “review my changes”) | Changes since merge-base with the repository default branch (usually `main`), including committed, staged, and unstaged edits |
| “Uncommitted”, “dirty”, “working tree” only | Staged + unstaged diff only |
| Named PR, branch, or GitHub link | Check out that head branch locally when needed, then use the branch diff row above |

Repository path: use the git root that contains the changes (may be a subfolder of the workspace).

If there is no diff, say so in one sentence and stop.

## Focus on finding

1. Bugs, defects, or logic errors.
2. Security concerns (ground in [docs/export-api.md](../../../docs/export-api.md) export gating and README API security notes).
3. Violations of language best practices (ground in [docs/engineering.md](../../../docs/engineering.md) and [AGENTS.md](../../../AGENTS.md)).
4. Architectural issues that will become expensive to fix later (canon vs consumer boundaries, Core/Web/Data separation).
5. Code that is unnecessarily complex.
6. Areas where maintainability or testability will suffer — including missing or inadequate tests when **behavior** changed (not for unimplemented features).

## Do not comment on

- Formatting issues (CI and `dotnet format` own formatting).
- Naming preferences unless they materially affect readability.
- Personal coding style preferences.
- Minor optimisations without measurable value.
- Features that have not yet been implemented.
- Planned future work.
- Stub implementations that are clearly placeholders.
- Missing functionality unless the current code creates a defect.

Do not ask for exhaustive Aspire AppHost modelling tests unless AppHost behaviour changed materially.

## How to review

1. Establish the diff (see **Scope**).
2. Read changed code and enough surrounding context to judge behavior and integration.
3. Trace call paths for export filters, canon loading, Hugo generation, and public API gating when those areas change.
4. When unsure whether something is a real issue, investigate (read callers, run targeted tests) before reporting.
5. Report only issues you would expect a senior engineer to raise during a real pull request review.

Meaningful changes in this repo should pass the build/test commands in [AGENTS.md](../../../AGENTS.md) against `Turpinverse.slnx`. Use that when judging whether a finding blocks approval.

## Output

Lead with findings. Keep summary and praise minimal.

For each issue, provide:

- **Severity** — Critical, High, Medium, or Low.
- **Location** — `path/to/file.cs:line` (or code citation when reporting in chat).
- **Why it matters** — tie to approve/reject risk.
- **Suggested improvement** — concrete, actionable fix direction (do not implement unless asked).

Present issues in a compact markdown table sorted by severity (highest first):

| Severity | Location | Why it matters | Suggested improvement |
| --- | --- | --- | --- |
| … | … | … | … |

Use UK English in prose.

## Closing

End with a brief explanation of overall merge readiness (approve, approve with nits, or reject) based only on the issues above.

If no significant issues are found, say so explicitly. Mention residual risks or test gaps only when they are material to approval.

## After the review

- Do not implement fixes, commit, or push unless the user explicitly asks.
- If the user asks to fix findings, apply minimal correct diffs, match existing conventions, then run the AGENTS.md build/test commands before claiming the review is clear.
