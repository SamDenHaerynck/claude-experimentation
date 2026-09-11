# Backlog

Ranked pool of candidate ideas not yet started. Phase 0 picks the top entry. When an idea is
selected it moves out of this list into `ideas/<slug>/`; when generating replacements, add new
candidates to the bottom unless evidence gathered during Validate suggests otherwise.

Re-ranked 2026-09-07 after the three-kills-in-a-row sourcing-method review (see `DECISIONS.md`).
All three prior kills were one-shot output generators (a lookup, an autofill, a letter draft) fully
replaceable by a free calculator, a free adjacent tool, or a generic LLM prompt. Going forward,
Phase 0/1 selects for at least one of: (a) value from data that changes on an ongoing basis and
must be kept current, (b) value from embedding in a recurring workflow or persisted system state
rather than a single generated artifact, or (c) a cost of being wrong high enough that free/DIY
answers aren't trusted. The three new entries below (originally four; #1 was killed 2026-09-08,
see `killed/ci-migration-rollback-gate/`) were ordered first because each is chosen to test one of
those properties; the two carried-over entries are one-shot generators with the same shape as the
three kills and are demoted to the bottom pending evidence that a real wedge exists.

Updated 2026-09-08: killed candidate #1 (CI-embedded migration/rollback safety gate) in Phase 1 —
see `killed/ci-migration-rollback-gate/REASON.md`. Result widens the (a)/(b)/(c) check itself (see
`RUNBOOK.md`): holding property (b) is necessary but not sufficient if a free tool that is *itself*
already recurring/CI-embedded (not just a one-shot substitute) already serves the same audience —
name that risk explicitly too, not just one-shot LLM/DIY substitutes.

Updated 2026-09-09: killed candidate #1 (contractor-vs-employee classification risk checker) in
Phase 1 — see `killed/contractor-classification-checker/REASON.md`. This one did not lose to a
free-recurring competitor (none was found); it failed because the only real user-voice evidence
found contradicted the idea's own property-(c) thesis — every real forum post asked for the
classification answer, none asked for documentation/an audit trail. Widens the check again (see
`RUNBOOK.md`): a claimed (a)/(b)/(c) property needs real user-voice evidence that the specific
differentiator is wanted, not just a plausible-sounding architectural argument for why it should be.

Updated 2026-09-10: killed candidate #1 (state/local sales-tax nexus threshold monitor) in Phase 1
— see `killed/sales-tax-nexus-monitor/REASON.md`. Property (a) held genuinely, but Shopify's own
free built-in dashboard, Stripe Tax's free built-in alerts, and a near-identical $19-69/mo indie
competitor (NexusMonitor, already on the Shopify App Store) all already serve this exact job; the
only real user-voice evidence found also asked one-time factual questions, not for ongoing
monitoring. This is the third consecutive kill since the 2026-09-07 review — the routine's
three-in-a-row trigger has fired; see `RUNBOOK.md` and `STATE.md`. Remaining entries below are
renumbered; day 009 must spend its session on sourcing method, not on validating entry #1 below.

Updated 2026-09-11 (day 009, sourcing-method review, see `RUNBOOK.md` and `DECISIONS.md`):
replenished from 2 entries back up to 4. This review found that the "recurring monitor/tracker/
alert/comparison tool for a defined professional deadline or requirement" idea shape is colonized
regardless of vertical (five unrelated candidates quick-checked, five already-existing
competitors found), which is why both prior kills this cycle died and why no new candidate of that
shape was added. The two new entries below (#1, #2) instead test the shape this review now
prefers — multi-party coordination/trust infrastructure, not a single-user dashboard — and each
discloses the real competitor found for it rather than omitting it; Phase 1 must run the
competitor-specific-complaint check from the new `RUNBOOK.md` entry before scoring "reason to
exist," not treat the disclosed competitor's existence alone as a reason to skip validating.

1. **Small-manufacturer RFQ / subcontractor-quote comparison tool** — lets a small manufacturer or
   contractor send a request-for-quote to multiple suppliers/subs at once and compare responses
   side by side, instead of the current default of separate emails/calls and a manual spreadsheet.
   Rationale: property (b) — value comes from being the shared record both the buyer and multiple
   suppliers touch on every purchasing cycle, not a single generated artifact; multi-party by
   construction (buyer plus several suppliers must all use it), which is the shape this review now
   prefers over a single-user tracker. Disclosed competitor risk: AuraVMS, QuotesFlow, QuoteWerks,
   and Quotable AI all already sell RFQ/quote-comparison tools to this same small-business
   audience, AuraVMS from $4.99/mo — found via quick `WebSearch`, no traction/complaint evidence
   checked yet. Phase 1 must find either a specific underserved sub-segment (an industry, region,
   or supplier-count band these four don't target) or a real documented complaint about one of them
   before this clears "reason to exist"; a search for QuoteWerks complaints this session (G2:
   4.3/5, no 1-star reviews in the summarized results) found nothing usable, so that check may need
   to go deeper than a single `WebSearch` call (raw review pages, not summaries) to be conclusive.

2. **Co-owned vacation property scheduling and expense-splitting** — a shared calendar plus
   expense ledger for a group of relatives or friends who jointly own a cabin, beach house, or
   timeshare-like property, so booking conflicts and shared costs (utilities, repairs, taxes) don't
   have to be tracked over group text and a shared spreadsheet. Rationale: property (b) — the value
   is the persisted shared calendar/ledger state multiple co-owners all rely on continuously, not a
   one-shot artifact; multi-party by construction like #1. Disclosed competitor risk: this is the
   most colonized of the two — OurSharedPlace ($99/property/year, unlimited members, fair-use
   scheduling) and PlumConnect (calendar plus a group bank account integrated with Mercury) both
   already target this exact audience with close-fit features. A targeted search this session for
   complaints about either found nothing usable. Ranked below #1 for this reason; Phase 1 should
   only proceed past a differentiation check if it can name a concrete gap (e.g. a co-ownership
   structure or price point these two don't serve) rather than "ours would be simpler."

3. **Freelancer SOW/contract generator with e-sign tracking** — a lightweight, freelancer-specific
   alternative to heavyweight contract platforms: generates scoped statements of work from a short
   intake form, tracks e-signature status, and reminds on renewal/expiry. Rationale: solo
   consultants often use generic templates or expensive all-in-one tools (DocuSign, PandaDoc) built
   for larger teams; a narrow, cheap, fast tool may fill a gap. Demoted: the core deliverable (a
   generated SOW document) is a one-shot artifact of the same shape as the three kills; not
   disqualified outright since the e-sign tracking/renewal reminders piece is arguably property
   (b), but Validate must test whether that piece alone (not the document generation) is what a
   buyer would pay for, given free-template and incumbent-freemium substitutes likely exist.

4. **Git-history-to-changelog generator for indie SaaS** — ingests merged PRs/commits and drafts a
   customer-facing changelog entry or release-notes email, matching a configurable tone/template.
   Rationale: solo/indie SaaS founders ship frequently but often skip customer communication because
   writing a polished changelog entry takes more time than the fix itself. Demoted: this is close to
   a pure one-shot text-generation task, which a general-purpose LLM prompt against pasted commits
   already does for free today — the same substitute that helped kill the questionnaire-autofill
   idea. Validate would need to find a real wedge (e.g. persisted per-repo config/workflow
   automation, property (b)) beyond "nicer prompt" before this clears "reason to exist."
