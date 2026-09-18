# Killed: Grant-report normalizer for small nonprofit program staff

Killed in Phase 1 (Validate), day 016, 2026-09-18. Score: 15/25, below the routine's 16 floor;
"evidence of demand" independently scored 2/5, an automatic kill on its own per the routine's
Phase 1 rule regardless of total.

Full evidence, competitor research, and scoring are in `VALIDATION.md` in this directory. Short
version: `BACKLOG.md`'s biggest flagged risk (willingness to pay, given typically tight nonprofit
budgets) actually resolved favorably — WTP scored 4/5, backed by a real, currently-sold, near-exact
competitor already charging for this. But two other things killed it, both surfaced only once the
checks `BACKLOG.md` itself flagged as necessary were actually run fresh:

- **A direct competitor `BACKLOG.md` said wasn't found, was found on a fresh search.** Sopact
  ([sopact.com](https://www.sopact.com/use-case/nonprofit-grant-management-software), re-fetched
  directly) is grantee-side software whose own marketing describes almost exactly this idea's core
  mechanism verbatim: "Store the evidence separately from each report's presentation. Funders can
  receive different explanations and views while the underlying sources, definitions and
  calculations remain consistent." It is already priced at $3,588-$9,588/yr plus a $2,000 minimum
  setup fee. `BACKLOG.md`'s "no direct named competitor was found" claim (checking only Foundant and
  Submittable, both funder-side intake portals) did not survive the fresh competitor search it
  itself said was needed.
- **Demand evidence for the idea's own specific thesis (many funders, not one) turned out to be
  aggregate statistics and advocacy/survey commentary, not real user voice.** An accurately-quoted
  Exponent Philanthropy statistic ("a single organization may juggle 40-60 applications... from
  20-30 funders") and an independent 280-organization PEAK Grantmaking survey both confirm the
  underlying structural problem (differing per-funder formats) is real. But despite genuine
  multi-angle search (WebSearch, targeted forum/practitioner-blog search; Reddit blocked to fetch
  tools as in prior sessions), zero individually-voiced nonprofit-staffer complaints specifically
  describing the multi-funder reformatting pain were found. The one attributable practitioner
  complaint found (NonprofitAF.com) is framed as a single-funder demand, not the multi-funder
  framing this idea's differentiation rests on. Per routine non-negotiable #4, this "no evidence
  found" on the idea's specific angle is treated as a signal, not papered over with the (real, but
  differently-shaped) aggregate statistics.

The "reason to exist" dimension (2/5, not itself an auto-kill) reinforces the same verdict: the only
identified differentiation from Sopact — a cheaper, narrower tool for the smallest, tightest-budget
nonprofits Sopact's pricing prices out — is an inferred architectural argument with no user-voice
evidence found that small nonprofits are specifically asking for a cheaper alternative. This is the
same "plausible-sounding argument without user-voice evidence" failure mode the 2026-09-09
`RUNBOOK.md` lesson warns against, now recurring on a fresh idea sourced through a different channel
than that original lesson.

This candidate was not sourced via the Channel A method adopted 2026-09-16 (it was carried over from
`BACKLOG.md`'s pre-existing entries, demoted below the ClickUp idea at that time). It was selected
because the ClickUp idea (Channel A's first test) was killed day 015 and this was next in rank. New
`RUNBOOK.md` entry records the general lesson: an idea can carry compelling, accurately-quoted
*aggregate* statistics from an advocacy or industry-survey source while having zero real
individually-voiced user complaints matching its specific thesis — the two are not substitutes for
each other, and the demand-evidence check needs to distinguish them explicitly rather than accept
an accurate aggregate statistic as satisfying the "real user voice" bar.

Consecutive kills: 11 (dependency-eol-watcher, vendor-security-questionnaire-autofill,
rent-increase-notice-calculator, ci-migration-rollback-gate, contractor-classification-checker,
sales-tax-nexus-monitor, rfq-quote-comparison, co-owned-vacation-property,
customs-hts-microseller, clickup-conditional-formatting, and now this idea). This is the second
kill since the 2026-09-16 sourcing-method review (day 015 killed clickup-conditional-formatting;
day 016, this session, is the second) — not yet a third-in-a-row since that review, so the
routine's three-in-a-row trigger does not fire this session. It will fire if day 017 also kills.
See `STATE.md` for the next action.
