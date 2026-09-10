# Killed: State/local sales-tax nexus threshold monitor with change alerts

Killed in Phase 1 (Validate), day 008, 2026-09-10. Score: 14/25 (below the 16 threshold); "evidence
of demand" independently scored 2/5, an automatic kill on its own per the routine's Phase 1 rule
regardless of total.

Full evidence, competitor research, and scoring are in `VALIDATION.md` in this directory. Short
version: the underlying property this idea targeted — thresholds vary by state and a seller's
position relative to them changes continuously, so a one-time calculation goes stale — is real.
But applying both widened `RUNBOOK.md` checks killed it anyway:

- **2026-09-08 check (free-and-recurring incumbent)**: Shopify's own built-in "Tax liability
  insights" dashboard already tracks this for free for any Shopify seller (passively, no push
  alert — a real but narrow gap), and Stripe Tax already sends free, active email/dashboard alerts
  for any seller using Stripe. Both are embedded in tools the target audience already runs, not
  one-shot substitutes.
- Beyond that baseline, a small dedicated competitor, **NexusMonitor**, already ships on the
  Shopify App Store at $19-69/month doing essentially this exact product (multi-platform
  aggregation, colour-coded nexus map, percentage-threshold email alerts) — closer to a direct
  clone than any competitor found in a prior Phase 1 pass. TaxJar, Avalara, NexusFlag, and Numeral
  all also have named nexus-monitoring products or features in the same space.
- **2026-09-09 check (user-voice supports the specific differentiator)**: the three real
  first-person posts found (all Shopify Community) each ask a one-time factual question about how
  the $100k threshold or nexus registration works — none expresses frustration with a one-time
  answer going stale, or asks for ongoing monitoring/alerts. This is the same failure mode that
  killed `contractor-classification-checker` the prior session: a plausible architectural claim
  unsupported by the only real user-voice evidence reachable.

Reddit was unreachable from this environment again this session (third session running with this
exact gap — 2026-09-09 and 2026-09-10) — flagged again under "Notes for owner" as a possible
recurring tooling limitation, not filled with invented evidence.

**This is the third consecutive kill since the 2026-09-07 sourcing-method review**
(`ci-migration-rollback-gate`, `contractor-classification-checker`, and now this idea). Per
`RUNBOOK.md`'s 2026-09-09 entry, the routine's "three in a row" trigger fires without
qualification: the next session must spend its time on sourcing method, not on validating a fourth
backlog candidate. See `STATE.md` for the next action.

Consecutive kills: 6 (dependency-eol-watcher, vendor-security-questionnaire-autofill,
rent-increase-notice-calculator, ci-migration-rollback-gate, contractor-classification-checker,
sales-tax-nexus-monitor).
