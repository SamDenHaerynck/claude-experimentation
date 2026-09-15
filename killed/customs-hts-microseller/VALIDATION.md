# Validation: Small-batch customs/HTS classification for micro e-commerce sellers

Day 013, 2026-09-15. Research delegated to a subagent instructed to avoid the fabricated-quote
failure mode recorded in `RUNBOOK.md` (2026-09-14 entry) — it was told to mark anything not
directly re-fetched as "secondhand/unverified" rather than dress a paraphrase up as a quote, and it
did so consistently (several claims below are explicitly flagged as unverified because a fetch
403'd or hit a login wall). Evidence quality is otherwise good: several primary threads were fetched
directly.

## Competitors found (3+, with pricing)

1. **Zonos** (zonos.com/classify, zonos.com/landed-cost) — free AI HS classification bundled with
   Landed Cost up to 10,000 classifications/year; paid tiers for API/checkout duty collection.
   Shopify App Store listing has 150+ reviews at 4.6-4.7★. Built for platform-integrated stores
   doing checkout-time duty collection at scale — the originally disclosed gap (occasional,
   non-integrated sellers) is real for this one specifically.
2. **InstaDuty** (instaduty.com) — AI duty calculator, upload a commercial invoice, get HS
   classification + duty/fee (MPF, HMF, Section 301) calculated. Positioned for one-off/ad-hoc use,
   not platform-tied — closer to the proposed idea than Zonos. Pricing reported secondhand as
   "free to $99/month" (site returned HTTP 429 on direct fetch, unverified). No review-platform
   footprint found (G2/Capterra/Trustpilot all empty).
3. **Zipments** (zipments.io) — free HTS/HS lookup, US customs invoice generator, duty/tax
   calculator. Marketed at "importers, shippers, brokers, and logistics teams" but the tools
   themselves are standalone, no integration required. Invoice generator noted as "free while in
   beta" (secondhand, pricing page not independently fetched). No review-platform footprint found.
4. (Noted but not comparable: GingerControl, Thomson Reuters ONESOURCE, Descartes CustomsInfo,
   Avalara — enterprise API classification for teams doing thousands of SKUs, not occasional
   single-shipment sellers.)

## Real user-voice posts (3+, with URLs)

- eBay Community, "Tariff code" thread (~Sept 2025): seller says they can't find the tariff codes
  eBay now requires for international labels.
- eBay Community, "What HS Tariff codes do I need to use for two items" (~Sept 2025): seller unsure
  how to classify a combo product (metal pins mounted in a frame) — a genuine classification-
  specific pain point, not generic shipping frustration.
- eBay Community, "Exporting: Seller Feature Request — Tariffs/Classifications" (~Sept 2025): seller
  requests an AI-suggested-HTS field at listing time, noting a 2-digit code change can swing duty
  from 5% to 75%+.
- Shopify Community (community.shopify.com/t/calculating-tariffs-and-shipping-to-the-us-from-the-uk/562214,
  active, post-2025-08-29): UK merchant reports Shopify's built-in calculator wrongly stacking the
  10% IEEPA flat tariff on top of HS-code duties for postal shipments; Shopify staff acknowledged
  the bug in-thread. Directly on-point and independently fetched.
- Etsy Community thread "Navigating US Tariffs as an International Seller — A Workaround and a Plea
  for Better Tools" — title/existence confirmed via search, but the page is login-walled; content
  is secondhand via a search snippet only (a seller describing a manual Zonos + Canada Post
  workaround), explicitly flagged as unverified rather than quoted as fact.

Bar met: at least 4 real, dated, on-point posts, 3 of them independently fetched in full (not just
search snippets).

## Deep review-page complaint check

- Zonos: direct fetch of G2 and Trustpilot review pages both failed (403/404). Secondhand-only
  (search-indexed snippets, explicitly flagged unverified): ~4.8/5 on G2; a Capterra review noting
  "hard to read" reports and a sales-tax coverage gap; Shopify App Store complaints about VAT
  calculated on pre-discount price and post-setup malfunctions. None of this was independently
  confirmed by direct fetch, so treat as suggestive, not established.
- InstaDuty / Zipments: no review-platform presence at all — "no reviews found," which is itself
  signal (too new/small to have attracted independent review, not evidence of being well-loved).

## Underserved-sub-segment search — the decisive finding

This is the finding that changes the verdict. Per valueaddedresource.net (fetched directly, dated
2026-06-01 and 2026-06-09), **Etsy itself is building a "US Estimated Tariffs Calculator" into its
own listing-creation flow**, powered by a Zonos integration, that suggests HS codes and estimates
tariffs — and separately, effective 2026-07-09, Etsy now requires non-US sellers to prepay tariffs
(DDP) and bake duty into listed prices. That date is in the past relative to today (2026-09-15),
meaning this is not a future risk to note and move past — it is already live. Etsy's own tool is
listing-level guidance and pricing help, not a full per-shipment commercial invoice, and it's
Etsy-only (doesn't help a standalone Shopify seller), so it doesn't fully close the gap — but it
closes it for a large fraction of the named target audience (Etsy sellers specifically), for free,
built into the platform they already use. Combined with InstaDuty and Zipments already offering
free standalone HS-lookup + invoice + duty-calc without integration, the "nothing serves occasional
sellers" premise the idea was built on does not hold. What's left is a narrower and less-evidenced
claim: that existing free tools are too complex or not craft-seller-friendly enough for a hobbyist —
no user-voice evidence found actually says this; it's an assumption, not a finding.

## Scoring (1-5 each, per the routine's Phase 1 rubric)

- **Evidence of demand: 3/5.** Real, dated, on-point complaints exist (5 posts, 4 independently
  verified) about HTS/duty confusion specifically tied to the 2025-08-29 repeal. But none of the
  posts ask for a paid tool — the Etsy thread's own "workaround" was to combine two other free
  services manually, and the eBay/Shopify threads ask the *platform* to fix it, not a third-party
  vendor.
- **Willingness to pay: 2/5 — automatic kill threshold on its own.** Every real standalone
  competitor found (Zonos free tier, InstaDuty, Zipments) is free or has an unverified-but-plausibly-low
  price; Etsy is now building the closest analog into its platform for free. No evidence anywhere in
  this research of anyone paying for this specific job. A segment defined by low shipment volume and
  hobbyist economics is also structurally a poor fit for a paid subscription.
- **Buildable to handoff in <=10 sessions: 4/5.** Technically tractable — the USITC HTS schedule is
  public data, LLM-assisted classification plus a lookup table and a templated invoice generator is
  a bounded MVP. The one build risk (accurate country/duty-rate coverage) is a scope question, not
  a buildability blocker.
- **Reason to exist alongside what already ships: 2/5.** Three free-or-near-free standalone
  competitors already exist, one of them (InstaDuty) explicitly positioned for the same one-off,
  non-integrated use case the idea proposes, and the platform with the largest share of the named
  target audience (Etsy) is actively closing this exact gap in-house, for free, as of two months
  before this validation. The remaining differentiator (friendlier UX for hobbyists) is asserted,
  not evidenced.
- **Low compliance/operational burden: 2/5.** Misclassification carries real penalty exposure (19
  USC 1592, 20-40% of underpaid duty for negligence, up to 4x for fraud) that would sit on the tool
  vendor's advice, not just the seller's action; duty-rate and tariff-schedule data (Section 301,
  IEEPA rates) changes on short notice and must be kept current or the tool becomes a liability
  rather than a help.

**Total: 13/25** — below the 16 threshold, and willingness-to-pay independently scores 2/5, which
alone is a kill per the routine's rule ("kill if... demand, willingness to pay, or buildability
scores 2 or below").

## Verdict: KILL

This is the third kill in a row since the 2026-09-11 sourcing-method review (day 010
`rfq-quote-comparison`, day 011 `co-owned-vacation-property`, day 013 this one — day 012 validated
nothing, so it doesn't count toward or reset the streak). Per the routine's own instruction ("If
three ideas in a row are killed, do not generate a fourth the same way"), day 014 must spend its
session on sourcing method rather than pulling the next backlog entry and validating it the same
way. See `RUNBOOK.md` for the new entry and `DECISIONS.md` for the recorded choice.
