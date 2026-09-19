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

Updated 2026-09-16 (day 014, sourcing-method review after the third three-in-a-row kill and a
direct `OWNER.md` instruction to improve idea sourcing — see `RUNBOOK.md` and `DECISIONS.md`): all
9 kills to date died on concrete, verified evidence (a real free/bundled competitor, a real pricing
gap, or contradicting user-voice evidence), not on an overly strict score, so the fix this session
targets Phase 0 sourcing, not the Phase 1 bar. Tested a new sourcing channel — SaaS vendor public
feature-request/roadmap boards on already-paid products, looking for a high-vote, long-open request
where the vendor has stated its own reason for not building it — via a delegated research subagent.
Added one new candidate below (#1) sourced this way and ranked it ahead of the three carried-over
entries, which are otherwise unchanged and keep their prior demotions/caveats (now #2-#4).

Updated 2026-09-17 (day 015): killed former candidate #1 (ClickUp custom-field
conditional-formatting companion extension) in Phase 1 — see
`killed/clickup-conditional-formatting/REASON.md`. Scored 16/25; willingness-to-pay independently
scored 2/5 (auto-kill) — no paid comparable was found anywhere for this specific feature category,
and the one true implementation analog (`trello-colored-custom-fields`) is free/open-source, a real
negative signal, not just an absence of proof. Demand evidence was the strongest this project has
recorded (4/5, four independently re-verified feedback-board threads over ~5.7 years), validating
Channel A (vendor feature-request boards) as a genuinely stronger demand-sourcing method — but this
kill also surfaces a new risk specific to that channel: two of the four threads revealed ClickUp
already ships the requested mechanism natively in Calendar View, so the "reason to exist" case
(2/5) was undercut by the vendor being able to extend an already-built feature for free, with no
stated architectural objection (unlike the Webflow precedent Channel A was originally validated
on). This is the first kill since the 2026-09-16 sourcing-method review, not a third-in-a-row —
the routine's trigger does not fire again this session. Remaining entries renumbered (former #2-#4
are now #1-#3); back down to 3 entries, exactly at the routine's floor.

Updated 2026-09-18 (day 016): killed former candidate #1 (grant-report normalizer for small
nonprofit program staff) in Phase 1 — see `killed/grant-report-normalizer/REASON.md`. Scored
15/25; evidence of demand independently scored 2/5 (auto-kill). Willingness to pay, the risk this
entry's own write-up flagged as biggest, actually resolved favorably (4/5) — but the fresh
competitor search this entry's own write-up said was necessary surfaced a direct, near-exact
competitor (Sopact, already charging $3,588-$9,588/yr + a $2,000 setup fee for "store data once,
present differently per funder") that the original sourcing pass missed, and the demand evidence
for the idea's specific multi-funder thesis turned out to be accurately-quoted aggregate/advocacy
statistics rather than any real individually-voiced grantee complaint matching that thesis. This is
the second kill since the 2026-09-16 sourcing-method review (day 015, day 016) — not yet a
third-in-a-row; the trigger fires if day 017 also kills. Remaining entries renumbered (former #2-#3
are now #1-#2); down to 2 entries, below the routine's 3-candidate floor — day 017 must replenish
before or as part of Phase 0/1, per the same pattern as day 011/012.

Updated 2026-09-19 (day 017): replenished from 2 entries back up to 4, per the day-016 note that
the backlog was below the routine's three-candidate floor. This session's own unit of work was
replenishment only, not Validate — per the routine's "one unit of work" rule and the day-011→012
precedent (a kill leaving the backlog below floor gets a dedicated replenishment session; Validate
on the resulting top entry is deferred to the next session). Sourced via two research subagents
running two channels not used in the last two Validate cycles (which both used Channel A, SaaS
vendor feedback boards): Channel B (recurring paid freelance/contractor job postings, as *direct*
demand+willingness-to-pay evidence — a business already paying a human for the task) and Channel C
(1-3 star G2/Capterra/TrustRadius reviews on paid mid-market B2B SaaS, from reviewers who are
already paying customers of the base product, complaining about a specific missing
feature/workaround). G2 and Upwork both returned HTTP 403 on every direct-fetch attempt; Capterra
and Freelancer.com fetched successfully and are the actual evidence base below. Every quote below
was independently re-verified via direct `WebFetch` against the cited URL by this session (not
just trusted from the subagent), per the day-012 fabricated-citation lesson. Both subagents also
surfaced a candidate each that failed their own honest gut-check and are **not** added: a Google
Business Profile review-management/freshness tool (already covered by a crowded existing market —
Podium, Birdeye, NiceJob, ReviewTrackers, EmbedSocial, all doing this today per the subagent's
pricing-page checks) and a veterinary cross-department patient-scheduling tool (real, strong
evidence — property (a)/(b)/(c) all plausibly hold — but Instinct Science is an established
incumbent integrating with the exact named practice-management systems for this exact problem).
The two surviving candidates below are added ahead of the two carried-over entries, which are
otherwise unchanged and keep their prior demotions (now #3-#4). Neither new candidate has had a
full Phase 1 competitor/sub-segment/user-voice pass yet — that happens when one is selected next
session — but each names its single highest-priority risk explicitly so Phase 1 starts there
rather than discovering it late.

1. **Vendor price-list ingestion/normalizer for construction estimating software** — for
   contractors/estimators using takeoff/estimating tools (STACK, PlanSwift, etc.), ingests messy
   PDF/Excel/CSV price sheets from a contractor's own suppliers, normalizes them into the
   estimator's item/cost-code format, and flags what changed since the last import — a companion
   to the existing tool, not a replacement. Sourced via Channel C: Gary L., a Construction
   Estimator, 1-star review of STACK Takeoff on Capterra, 2025-10-27
   (https://www.capterra.com/p/147181/STACK-Takeoff/reviews/) — independently re-verified
   verbatim: "Importing material prices is VERY cumbersome. You have to manually import prices
   which is time consuming," and "Every item has to be manually entered and often the user has to
   convert vendor pricing to user pricing, again very time consuming." Property case: (a) material/
   commodity prices change week to week and must stay current; (b) every estimate depends on the
   price catalog, a recurring refresh tied to workflow, not a one-time artifact; (c) a stale price
   directly costs margin or a lost bid — real money. Competitors named by the research subagent
   (not yet independently re-verified by this session — Phase 1 must do so): generic
   regional-average cost databases (RSMeans/Gordian, CostOS, Sigma Estimates, Craftsman National
   Estimator, Trade Service/NetPricer) sell quarterly-updated averages, not a tool for a
   contractor's own negotiated vendor quotes; STACK's own marketplace has a few named point
   integrations (e.g. LED Lighting Supply) but reportedly no general importer. Highest-priority
   risk for Phase 1: this is a single reviewer's complaint (n=1) — no second corroborating quote
   was found in PlanSwift's reviews (which complained about licensing/rendering instead) — so
   demand does not yet clear the "3+ individually-voiced" bar this project's own RUNBOOK.md
   requires; a broader user-voice search (other estimating tools' reviews, contractor forums) must
   run before scoring, and the "no general importer exists" competitor claim needs independent
   re-verification, not trust.

2. **Automated weekly financial snapshot for solo/micro-business owners on QuickBooks Online** —
   connects to one small business's QuickBooks Online, auto-flags overdue invoices and surfaces new
   bank-feed transactions needing categorization/review, and pushes a weekly P&L/balance-sheet/
   cash-flow snapshot — aimed at the segment currently paying a human $8-15/hr on freelance
   platforms to do this by hand, not at bookkeeping firms managing many clients. Sourced via Channel
   B: a Freelancer.com posting (https://www.freelancer.com/projects/data-entry/
   weekly-quickbooks-bookkeeping-reporting, $8-15 USD/hr, recurring weekly, independently
   re-verified via direct WebFetch) asks for weekly work to "enter new customer invoices, apply
   payments, and flag any overdue balances," "record and categorize expenses from bank feeds,
   receipts, and credit-card statements," and "generate the P&L, balance sheet, and cash-flow
   snapshot." Property case: (a)/(b) both plausibly hold (bank feeds/invoices change continuously,
   tied to a recurring weekly cadence); (c) plausibly holds (a wrong P&L number has real cost).
   Highest-priority risk for Phase 1, ahead of everything else: QuickBooks Online itself already
   provides live P&L/balance-sheet/cash-flow reports and bank-feed categorization rules natively,
   included in the base subscription — this looks exactly like the "already-recurring free
   incumbent" failure mode in `RUNBOOK.md` that has killed prior candidates, and the sourcing
   research did not address it at all. Phase 1 must determine whether the job posting is really
   asking for something QBO doesn't already do (the judgment/exception-handling layer on top, not
   the reports themselves) before treating this as evidenced demand for a paid tool. Named
   competitors from the research subagent (not yet independently re-verified — Dext/Hubdoc, Chaser,
   Upflow, Zeni, Puzzle) were judged to target bookkeeping firms or funded startups rather than this
   solo-owner segment; re-verify before trusting "no incumbent found" for this niche.

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
