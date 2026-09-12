# Killed: Small-manufacturer RFQ / subcontractor-quote comparison tool

Killed in Phase 1 (Validate), day 010, 2026-09-12. Score: 15/25 (below the 16 threshold); "evidence
of demand" independently scored 2/5, an automatic kill on its own per the routine's Phase 1 rule
regardless of total.

Full evidence, competitor research, and scoring are in `VALIDATION.md` in this directory. Short
version: this session applied both corrections named by the 2026-09-11 sourcing-method review
(`STATE.md`, `RUNBOOK.md`) — go deeper than a single search summary before concluding a competitor
has no real complaint, and look for a specific underserved sub-segment or documented complaint
before scoring "reason to exist" low on bare competitor existence. Both checks were applied
properly this time, and both still came back negative:

- A deeper pass (actual Capterra review pages, not search summaries) did find a real, specific
  complaint about QuoteWerks (dated UI, workflow friction, slow support) — but QuoteWerks treats
  RFQ comparison as a $20/mo bolt-on to a general CPQ product, so this complaint doesn't open a
  wedge into the competitive field this idea would actually enter.
- The underserved-sub-segment search came back empty, and worse: it surfaced two *additional*
  dedicated competitors beyond the four already known (AuraVMS, QuotesFlow, QuoteWerks, Quotable
  AI) — **Jiga** already serves the manufacturing/job-shop RFQ vertical, and **SmartBid**/**Buildr**
  already serve the construction-subcontractor bid-comparison vertical. The two most obvious
  target verticals for this idea are each already served by a dedicated incumbent.
- Real user-voice evidence was found (non-Reddit: Practical Machinist, an independent construction-
  estimator Substack, Contractor Talk), but checked against the idea's *specific* differentiator
  (a buyer-side, multi-party quote-comparison record), only one of three sources squarely supports
  it — one is the wrong persona (a seller quoting out, not a buyer comparing inbound quotes), and
  one is real but unverified (paywalled). This is the same failure mode that killed
  `contractor-classification-checker` and `sales-tax-nexus-monitor`.

Reddit was unreachable again this session — the **third** Validate session this exact environment
limitation has been hit (2026-09-09, 2026-09-10, 2026-09-12). This meets `OWNER.md`'s escalation bar
("the same failure has now blocked three consecutive sessions"); notified the owner via
`PushNotification` this session rather than only recording it here.

This is the first kill since the 2026-09-11 sourcing-method review (day 009), so the routine's
three-in-a-row trigger has **not** fired again yet — one kill into a new cycle, not three.

Consecutive kills: 7 (dependency-eol-watcher, vendor-security-questionnaire-autofill,
rent-increase-notice-calculator, ci-migration-rollback-gate, contractor-classification-checker,
sales-tax-nexus-monitor, rfq-quote-comparison).
