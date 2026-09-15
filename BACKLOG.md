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

Updated 2026-09-12 (day 010): killed former candidate #1 (small-manufacturer RFQ / subcontractor-
quote comparison tool) in Phase 1 — see `killed/rfq-quote-comparison/REASON.md`. Both corrections
from the 2026-09-11 review were applied properly (a deeper review-page fetch did surface a real
QuoteWerks complaint, and an underserved-sub-segment search was run) but both still came back
negative: the sub-segment search instead surfaced two *more* dedicated competitors (Jiga for
manufacturing, SmartBid/Buildr for construction) beyond the original four, and real user-voice
evidence only weakly supported the idea's specific buyer-side differentiator. This is the first
kill since the day-009 review, not a third-in-a-row, so the sourcing-method trigger has not fired
again. Remaining entries renumbered; still at 3, the routine's floor, so no replenishment needed
this session.

Updated 2026-09-13 (day 011): killed former candidate #1 (co-owned vacation property scheduling and
expense-splitting) in Phase 1 — see `killed/co-owned-vacation-property/REASON.md`. Both corrections
were applied via a delegated research subagent (a deep review-page complaint check, and an explicit
underserved-sub-segment search) and both came back negative: neither incumbent has any real
independent review footprint to find complaints in, and the sub-segment search surfaced five more
dedicated competitors (CabinPals, SharedKey, Shared Holiday Homes, CalDibs, House Matters) beyond
the two disclosed, each already covering the one narrow gap found (price, international). This is
the second kill since the day-009 review — two since the last review, not yet a third-in-a-row.
Notably this shows the sourcing review's own preferred shape (multi-party coordination, not a
single-user monitor/tracker) is *also* colonized; see `RUNBOOK.md`. Remaining entries renumbered;
down to 2, below the routine's three-candidate floor — next session must replenish before or as
part of Phase 0/1.

Updated 2026-09-14 (day 012): replenished from 2 entries back up to 4, per the day-011 note that
the backlog was below the routine's three-candidate floor. Sourced via a delegated research
subagent using no shape-based prior (per the day-011 retirement of both the "monitor/tracker" and
"multi-party coordination" shape preferences) — instructed instead to find genuinely evidenced
friction (real forum/complaint/review URLs, or a regulatory/platform change creating a new need)
across diverse verticals, and to name a found competitor for each rather than skip that check. The
subagent returned 4 candidates; the independent pre-merge review (see `log/2026-09-14.md`) spot-
checked the cited URLs and found that quoted claims attributed to shipbob.com, blog.inymbus.com,
and softwareadvice.com did not actually appear on those pages — invented citations dressed as real
sourcing. Direct re-verification confirmed this. Two of the 4 candidates are added below as new
#1-#2 after removing/replacing the fabricated quotes with claims actually verified against the
source text (see `RUNBOOK.md`'s new entry on this failure mode); a third candidate (freight-claim
packet generator) is dropped entirely rather than repaired, because two of its three supporting
citations were fabricated and there wasn't time this session to re-source it properly; the fourth
(a structural/civil calc-package assembler for permit submittal) was excluded from the start — the
subagent itself flagged that evidence as weak, consistent with the routine's "no evidence found is
a signal" instruction. Reddit was unreachable again for this sourcing pass (site: search and direct
fetch both failed); the subagent substituted other forums (Mike Holt, ElectricianTalk), vendor/G2/
Capterra pages, and industry blogs/news where a source it tried (eng-tips.com, 403 on direct fetch)
also failed. This is a Phase 0 sourcing pass, not Phase 1 Validate — neither surviving candidate has
had the full competitor/sub-segment/user-voice search run; that happens when one is selected.

Updated 2026-09-15 (day 013): killed former candidate #1 (small-batch customs/HTS classification
for micro e-commerce sellers) in Phase 1 — see `killed/customs-hts-microseller/REASON.md`. Scored
13/25; willingness-to-pay independently scored 2/5 (auto-kill). The originally-disclosed competitor
(Zonos) gap held, but two more free standalone competitors (InstaDuty, Zipments) were found, and
decisively, Etsy itself is already building a Zonos-powered tariff calculator into its own listing
flow (live since 2026-06, prepay-tariffs requirement live since 2026-07-09) — closing the gap for
free for the largest share of the idea's own target audience. This is the third kill in a row since
the 2026-09-11 sourcing-method review (day 010, day 011, day 013 — day 012 validated nothing and
doesn't count). Per the routine's own instruction, day 014 must spend its session on sourcing
method rather than pulling the next entry below and validating it the same way. Remaining entries
renumbered (former #2-#4 are now #1-#3); still at 3 entries, exactly at the routine's 3-candidate
floor, not below it — day 014's unit of work is the sourcing-method review the three-in-a-row
trigger requires, not replenishment, but no replenishment is needed regardless since the floor is
still met.

1. **Grant-report normalizer for small nonprofit program staff** — program staff enter
   outcome/spend data once; the tool reformats it into each individual funder's own report
   template. Rationale: "a single organization may juggle 40-60 applications and just as many
   unique reporting requirements from 20-30 funders, each with its own format and timeline," and
   reformatting is called out as "one of the most time-consuming tasks for nonprofits"
   ([exponentphilanthropy.org](https://exponentphilanthropy.org/blog/how-to-simplify-grant-applications-and-reports-for-nonprofits/),
   both quotes independently re-verified against the page).
   No direct named competitor found: Foundant and Submittable are funder-side intake portals for
   *receiving* reports, not grantee-side tools for reformatting the same underlying data across many
   funders' differing templates — genuinely undisclosed-competitor territory, so Phase 1 must run
   the search fresh rather than trust this absence. Property (b) plausibly holds (the same
   underlying data gets re-formatted repeatedly as new funders and reporting cycles arrive), but
   nonprofit willingness-to-pay is untested and budgets are typically tight — the specific risk
   Phase 1 should weigh most.

2. **Freelancer SOW/contract generator with e-sign tracking** — a lightweight, freelancer-specific
   alternative to heavyweight contract platforms: generates scoped statements of work from a short
   intake form, tracks e-signature status, and reminds on renewal/expiry. Rationale: solo
   consultants often use generic templates or expensive all-in-one tools (DocuSign, PandaDoc) built
   for larger teams; a narrow, cheap, fast tool may fill a gap. Demoted: the core deliverable (a
   generated SOW document) is a one-shot artifact of the same shape as the three kills; not
   disqualified outright since the e-sign tracking/renewal reminders piece is arguably property
   (b), but Validate must test whether that piece alone (not the document generation) is what a
   buyer would pay for, given free-template and incumbent-freemium substitutes likely exist.

3. **Git-history-to-changelog generator for indie SaaS** — ingests merged PRs/commits and drafts a
   customer-facing changelog entry or release-notes email, matching a configurable tone/template.
   Rationale: solo/indie SaaS founders ship frequently but often skip customer communication because
   writing a polished changelog entry takes more time than the fix itself. Demoted: this is close to
   a pure one-shot text-generation task, which a general-purpose LLM prompt against pasted commits
   already does for free today — the same substitute that helped kill the questionnaire-autofill
   idea. Validate would need to find a real wedge (e.g. persisted per-repo config/workflow
   automation, property (b)) beyond "nicer prompt" before this clears "reason to exist."
