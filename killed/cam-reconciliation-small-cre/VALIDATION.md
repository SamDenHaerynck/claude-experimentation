# Validation: CAM reconciliation tool for small independent commercial property managers

Phase 1, day 021 (2026-09-23). Sourced via a delegated research subagent (general web search
across ~17 verticals, not one of this project's four named vendor-surface channels — nearly every
other vertical it checked was already colonized by 2+ named competitors within minutes; this was
the one candidate it judged to have genuinely solid evidence). Every quote and pricing figure below
was independently re-verified by this session via direct `WebFetch`/`WebSearch` against the cited
URL, not just trusted from the subagent, per this project's standing fabricated-citation lesson
(day 012, day 019).

## User-voice evidence (independently re-verified)

- BiggerPockets forum thread, "Best PM software for a small commercial portfolio (under 20 units)
  — nobody has answe[red]" (https://www.biggerpockets.com/forums/32/topics/1281484-best-pm-
  software-for-a-small-commercial-portfolio-under-20-units-nobody-has-answe). Original poster Ryan
  Stomel: "I've been on a spreadsheet + QuickBooks combo and it works until CAM reconciliation
  season, at which point it turns into a nightmare." He names and rejects Yardi/MRI ("massive
  overkill for my size, priced for institutional operators"), AppFolio ("primarily residential...
  commercial module is an afterthought"), Buildium/DoorLoop/Rent Manager ("built for units-based
  residential PM... not designed for NNN reimbursements"), and RealPage ("Enterprise only, pricing
  not publicly available").
- Capterra, STRATAFOLIO reviews page (https://www.capterra.com/p/196759/STRATAFOLIO/reviews/),
  Scott L. (President, Real Estate, Feb 12 2025), on switching away from Buildium: "Buildium seems
  to be designed for owners of multi-family properties. Very cumbersome for regular commercial
  property." This is a real, individually-voiced complaint about commercial/residential software
  fit generally, not specifically about CAM reconciliation — an adjacent but not identical pain
  point to Ryan Stomel's.
- A reply in the same BiggerPockets thread, Nicholas Cokas, directly rebuts the premise that this
  is a software problem: "at under 20 commercial units, the problem is almost never software. It's
  workflow." He recommends per-tenant lease abstracts and quarterly (not annual) mini-
  reconciliations instead, and separately names Yardi Breeze Premier (~$400/month) as already
  offering native commercial lease support at this portfolio size. This is real, individually-
  voiced evidence contradicting the idea's own thesis, the same failure mode that killed the
  contractor-classification idea (day 009).

No third independently-voiced complaint specifically about CAM reconciliation pain (as opposed to
general commercial-property-software fit) was found; further general web searches for this exact
pain point returned only vendor blog content (STRATAFOLIO, Pickspace, Kardin, etc.) describing the
problem in the abstract, not additional real individually-voiced complaints with a fetchable,
quotable source. Demand stays at effectively n=1-2 for the specific claim, short of this project's
standing 3+-voice bar (see day-018 kill).

## Competitor check

- **STRATAFOLIO** (stratafolio.com/pricing, directly fetched): Essential $160/mo, Professional
  $190/mo, Enterprise $230/mo, each including the first 5 units (+$2-6/additional unit), plus an
  unspecified one-time onboarding fee; requires QuickBooks Online or Desktop. The original
  complainant researched six other named platforms but never mentioned STRATAFOLIO by name — an
  ambiguous discoverability-vs-fit signal, not conclusive either way on its own.
- **PigJet** (pigjet.com, softwareadvice.com/product/538154-PigJet, both directly fetched) — the
  decisive finding, and one the sourcing subagent's own report did not surface despite it appearing
  by name in the same forum thread used as evidence. PigJet's own site states it is built for
  "portfolios managing between 3 and 25 NNN properties" — almost an exact match to the target
  segment ("under 20 units") — doing AI-powered lease abstraction from PDF leases, automated NNN/
  CAM calculation, and "detailed annual reconciliation statements." This is a near-exact match to
  this idea's core deliverable, for the same target segment size, from a currently-marketed,
  purpose-built product (not a repurposed enterprise tool). Pricing is not published ("available
  upon request") and SoftwareAdvice shows zero reviews yet, suggesting an early-stage product with
  unproven traction — but its existence and exact positioning are independently confirmed from its
  own site and a third-party listing, not inferred.
- **Yardi Breeze Premier** (~$400/month per the same thread's own reply) is also positioned as
  covering this segment natively, though at a materially higher price point aimed at operators
  willing to pay for a fuller platform.

## Native-platform check

The target user's base accounting platform is QuickBooks Online. Multiple `WebSearch` results
(including QuickBooks' own community page, which returned HTTP 502 on repeated direct `WebFetch`
attempts this session and could not be independently re-verified by quote) corroborate that CAM
reconciliation is not a native QBO feature and generally requires manual workarounds or a
third-party tool — unlike the last two kills, the dominant platform this idea sits next to does not
appear to already give the deliverable away free. This is the one point in this idea's favor, but
it is moot given the PigJet finding below.

## Scoring

- **Evidence of demand: 2/5.** One clear, individually-voiced complaint specifically about CAM
  reconciliation pain (Ryan Stomel), one adjacent-but-not-identical complaint (Scott L), and one
  individually-voiced counter-argument in the very same thread disputing that software is the right
  fix at all. Short of the project's 3+-voice bar, and partially contradicted rather than merely
  unconfirmed.
- **Willingness to pay: 3/5.** Real, non-trivial willingness to pay for CRE-specific software
  exists in this market (STRATAFOLIO $160-230+/mo, Yardi Breeze Premier ~$400/mo), but the primary
  complainant explicitly rejected every named option as overkill/mispriced for his exact segment,
  and no evidence was found of anyone paying specifically for a cheaper CAM-reconciliation-only
  point solution.
- **Buildable to handoff in ≤10 sessions: 3/5.** Core allocation/cap/statement-generation logic is
  buildable, but lease-term variability (no two leases treat CAM identically, per multiple sources)
  means a competitive MVP needs either careful structured lease-data entry or lease-abstraction
  functionality that starts to overlap with PigJet's own core feature.
- **Reason to exist alongside what already ships: 1/5.** PigJet is a currently-marketed, purpose-
  built platform explicitly sized for this exact portfolio segment, doing lease abstraction, CAM/
  NNN reconciliation, and tenant statement generation — essentially this idea's own core deliverable
  already shipping, recommended in the very same source thread used as evidence. (Per the open
  `RUNBOOK.md` doctrine question from day 020 on whether this dimension is independently
  auto-kill-worthy, this write-up does not need to resolve that question either way: the verdict
  below is independently supported by the total score and by the demand dimension separately
  scoring ≤2, both of which are unambiguous kill criteria in the routine's own spec.)
- **Low compliance/operational burden: 4/5.** Tenant billing/lease financial data has real stakes
  but no special regulatory regime (no PCI/HIPAA-equivalent burden).

**Total: 13/25** — below the 16 kill threshold. Independently, "evidence of demand" scored 2/5
(≤2), an automatic kill per the routine's own spec regardless of total or of the reason-to-exist
doctrine question. **Verdict: KILL.**
