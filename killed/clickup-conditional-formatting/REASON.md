# Killed: ClickUp custom-field conditional-formatting companion extension

Killed in Phase 1 (Validate), day 015, 2026-09-17. Score: 16/25; "willingness to pay" independently
scored 2/5, an automatic kill on its own per the routine's Phase 1 rule regardless of total.

Full evidence, competitor research, and scoring are in `VALIDATION.md` in this directory. Short
version: demand evidence was genuinely strong and independently re-verified — four distinct,
sustained (up to ~5.7 years old) feature-request threads on ClickUp's own official feedback board
asking for value-based coloring of custom fields across List/Table/Board views, all quotes and vote
counts personally re-fetched and confirmed by this session, not just trusted from the research
subagent. But two things killed it:

- **No willingness-to-pay evidence for this specific feature category anywhere.** No paid
  conditional-formatting/coloring extension for ClickUp or any adjacent tool was found. The one true
  implementation analog that exists, `trello-colored-custom-fields` (an open-source GitHub project
  doing the equivalent for Trello), is free — a real negative signal, not just an absence of proof.
  General ClickUp-addon WTP comparables (Screenful $39-399/mo, Everhour $8.50/user/mo, TimeCamp
  $2.99-9.99/user/mo) exist but are all full analytics or time-tracking suites, categorically richer
  products than a cosmetic coloring overlay — they establish people pay for ClickUp add-ons in
  general, not for this one.
- **Reason-to-exist undercut by a finding this session's re-verification surfaced**: two of the four
  demand posts reveal that ClickUp already ships the underlying mechanism (value-based card coloring
  by custom field) natively — in Calendar View only. The four requests are functionally all asking
  ClickUp to extend an already-built feature to more views, a materially cheaper lift for the vendor
  than the original Channel-A rationale assumed (which specifically prized cases with a vendor's own
  stated architectural reason for declining, as found for Webflow on 2026-09-16). No such statement
  of refusal was found here — ClickUp could ship this for free at any time with no disclosed
  technical objection. This is not an auto-kill dimension on its own but reinforces the same
  verdict.

This is the first Phase 1 Validate run using the new Channel A sourcing method (SaaS vendor public
feature-request boards) adopted 2026-09-16 after the prior 9-kill sourcing-method review. Channel A
produced better-than-average demand evidence (score 4/5, the highest demand score of any kill this
project has recorded) but the same review that adopted it also warned Channel A is "not sufficient
on its own" and still requires the full WTP/competitor search — which is exactly what caught this
kill. See `RUNBOOK.md` for a new entry on the specific new failure mode found here: a vendor's own
feedback board can produce strong demand signal for a feature the vendor has *already partially
built* elsewhere in its own product, which is a distinct and arguably higher risk than a third-party
competitor, since the vendor can close the gap for free with no re-architecture.

Consecutive kills: 10 (dependency-eol-watcher, vendor-security-questionnaire-autofill,
rent-increase-notice-calculator, ci-migration-rollback-gate, contractor-classification-checker,
sales-tax-nexus-monitor, rfq-quote-comparison, co-owned-vacation-property,
customs-hts-microseller, and now this idea). This is the fourth kill since the 2026-09-16
sourcing-method review — but that review's own unit of work (session-only, no Validate run) means
only day 015 counts as a fresh three-in-a-row check post-review, and it is the first kill since
then, not a third-in-a-row. The routine's three-in-a-row trigger does **not** fire again this
session. See `STATE.md` for the next action.
