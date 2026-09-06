# Universe voice

Conventions for all Turpinverse content. They take precedence over historical accuracy.

1. **Legend first, ledger second** — The smirk comes before the footnote. Biographies lead with the modern reframe or witty hook; period detail is optional colour.
2. **Modern names** — Display names should feel like 2026 LinkedIn, not 1739 parish records. Use contemporary-sounding names (`Richard Turpin`, not `Dick Turpin`). Stable `id` slugs are preserved for data integrity; `displayName`, emails, and user-facing titles reflect the modern persona.
3. **Alias humour** — Alternate identities are separate CRM contacts or flagged aliases. Doubles and collisions (e.g. John Palmer vs Ned Palmer) are a feature, not a bug.
4. **Title wordplay** — Job titles do the heavy lifting. Pattern: corporate euphemism + period nod (e.g. "Identity Management Consultant", "Chief Transport Asset").
5. **Organisation personality** — Every organisation has a tagline-quality `legalName` or description punchline, not just "Premium X Services".
6. **Deal and case voice** — Ticket subjects sound like real helpdesk entries that only make sense if you know the legend.
7. **Victorian myth is fair game** — Romanticised additions (Black Bess overnight ride, Ainsworth romance) are in-universe marketing lore, not errors.
8. **Euphemism over explicitness** — Prefer corporate jargon ("rapid asset redistribution", "corridor optimisation") over overt criminal language.

Tone guidelines in [`canon/tone-guidelines.json`](../canon/tone-guidelines.json) extend these rules with examples and forbidden patterns enforced by `ToneValidator` (`TONE-001`).

## Related docs

- [canon/README.md](../canon/README.md) — dataset files
- [product-surfaces.md](./product-surfaces.md) — how Hugo and Blazor present copy
- [decision-log.md](./decision-log.md) — why the data model looks the way it does
