# Killed: Small-batch customs/HTS classification for micro e-commerce sellers

Killed in Phase 1 (Validate), day 013, 2026-09-15. Score: 13/25 (below the 16 threshold);
willingness-to-pay independently scored 2/5, an automatic kill on its own per the routine's Phase 1
rule regardless of total.

Full evidence, competitor research, and scoring are in `VALIDATION.md` in this directory. Short
version: the regulatory trigger (Executive Order 14324 ending de minimis, effective 2025-08-29) is
real and the resulting seller confusion is real and well-evidenced (5 dated forum posts, 4
independently fetched, including a Shopify Community thread where Shopify staff acknowledged a
related calculator bug). But the "nothing serves occasional sellers" premise the idea depended on
did not hold up:

- Two more free standalone competitors were found beyond the originally-disclosed Zonos —
  InstaDuty and Zipments — both offering HS classification, invoice generation, and duty
  calculation without requiring platform integration, i.e. exactly the gap the idea targeted.
- Decisively, Etsy itself is already building a Zonos-powered "US Estimated Tariffs Calculator"
  into its own listing flow (per valueaddedresource.net, dated June 2026) and has required non-US
  sellers to prepay tariffs since 2026-07-09 — both already live as of this validation, not a
  future risk. This closes the gap for free, in-platform, for the largest share of the idea's named
  target audience (Etsy sellers) specifically.
- No evidence was found anywhere that anyone is willing to pay for this job — every real
  competitor is free or unverified-low-price, and the hobbyist/occasional-shipper economics this
  idea targets are a structurally weak fit for a subscription.
- Compliance burden is real and asymmetric: misclassification penalty exposure (19 USC 1592) would
  sit partly on the tool vendor's advice, and duty-rate data requires active maintenance against
  short-notice tariff changes.

This is the **third kill in a row** since the 2026-09-11 sourcing-method review (day 010
`rfq-quote-comparison`, day 011 `co-owned-vacation-property`, day 013 this one; day 012 validated
nothing and does not count toward the streak). Per the routine's instruction, day 014 must spend
its session on sourcing method rather than validating the next backlog entry the same way. See
`RUNBOOK.md` for the new entry and `DECISIONS.md` for the recorded choice.

Consecutive kills: 9 (dependency-eol-watcher, vendor-security-questionnaire-autofill,
rent-increase-notice-calculator, ci-migration-rollback-gate, contractor-classification-checker,
sales-tax-nexus-monitor, rfq-quote-comparison, co-owned-vacation-property,
customs-hts-microseller).
