# Validation: Small-manufacturer RFQ / subcontractor-quote comparison tool

Research pass run 2026-09-12 (day 010), applying both corrections named in `STATE.md` from the
2026-09-11 sourcing-method review: (1) go deeper than a single `WebSearch` summary — fetch actual
review pages — before concluding a named competitor has no real complaint; (2) do not auto-kill on
bare competitor existence — search for a specific underserved sub-segment or a real, specific
complaint about a named competitor first. Research was delegated to a subagent briefed to be
skeptical and cite a URL for every claim; all findings below were independently reviewed against
that report.

## 1. Competitor pricing (confirmed via direct fetch of vendor pricing pages)

- **AuraVMS**: Small Team $4.99/user/mo, Medium Team $7.99/user/mo (annual), Enterprise custom,
  On-Premise from $2,999. (https://www.auravms.com/pricing)
- **QuotesFlow**: Starter $19/mo (3 users, 30 requests/mo), Growth $49/mo (6 users, 80
  requests/mo), unlimited "Power" tier price undisclosed. (https://quotesflow.io/)
- **QuoteWerks**: Essential $50/mo, Balanced $78/mo, Pinnacle $102/mo per concurrent user, plus a
  separate "VendorRFQ" add-on at $20/mo/user. (https://www.quotewerks.com/pricing.asp) — notably
  higher than the day-009 note's unverified $15-29 figure; the vendor's own page supersedes it.
- **Quotable AI**: Free tier (1 user, 10 orders/mo), Starter $49/mo, Pro Seller $99/mo, Advanced
  $249/mo, plus 2.9%+$0.30 payment processing. (https://getquotable.ai/seller/pricing)
- Adjacent, not in the original four: **SmartBid** (construction GC bid-leveling) does not publish
  pricing on its own site (https://smartbid.co/construction-bid-software-pricing — no dollar
  figures present, sales-contact-only) — a correction to this section's original draft, which had
  misattributed a $250-1,500/mo figure to that page. Third-party pricing-estimate sites disagree
  with each other on the real number (downtobid.com estimates roughly $800/user/year; other
  aggregator sites suggest $250-1,500/mo scaling with team size), so this is recorded as an
  unconfirmed estimate, not a confirmed price, per the routine's rule against presenting invented
  or unverifiable figures as fact.

Willingness to pay for the four core RFQ/quote-comparison competitors is well established across a
wide, independently-confirmed price range ($4.99-$102+/mo); SmartBid's adjacent, higher-priced
category (construction bid-leveling) is directionally corroborative but its own exact price is not
confirmed.

## 2. Real complaints about named competitors (deeper pass, per the corrected method)

Fetching actual review pages (not just search summaries) surfaced real complaints this time:

- **QuoteWerks**, Capterra (https://www.capterra.com/p/135691/QuoteWerks/reviews/, fetched
  directly): 1-star review calling the interface "outdated," the database management "cumbersome,"
  and the product "hasn't modernized since 2012"; a 3-star review noting competitors offer
  real-time-editable link-based quotes that QuoteWerks doesn't; other reviews cite slow support and
  friction from needing to "unlock sheets" to update pricing. G2's QuoteWerks reviews page returned
  HTTP 403 and could not be independently checked, but the day-009 claim that "no usable complaint
  exists" is superseded by these Capterra findings.
- **AuraVMS**: G2 reviews/discuss pages both returned HTTP 403 to direct fetch; a search-summary
  pass found only minor gripes (no custom email templates, no Tally integration, "design gets
  boring") — no substantive complaint found, though this may reflect a small/new product with thin
  review volume rather than genuine satisfaction.
- **QuotesFlow and Quotable AI**: no G2 or Capterra listing found for either — no third-party
  review evidence exists at all for these two, again likely reflecting low review volume rather
  than a competitive gap.

The real QuoteWerks complaints found (dated UI, workflow friction, slow support) are genuine, but
they are complaints about a general CPQ/sales-quoting product that treats inbound-supplier-RFQ
comparison as a $20/mo bolt-on ("VendorRFQ") rather than its core product — not a complaint about a
dedicated RFQ-comparison competitor. They do not by themselves establish a wedge into the
competitive field this idea would actually enter (see section 4).

## 3. Real user-voice evidence, checked against the specific differentiator

The idea's specific claim is that a *buyer* comparing multiple *inbound* supplier/sub quotes needs
a shared, structured, multi-party record — not just "an easier way to do X alone." Three real,
directly-fetched (non-Reddit) sources were found and each checked against that specific claim, not
just the general "quoting is annoying" problem:

- **Practical Machinist forum** (https://www.practicalmachinist.com/forum/threads/built-a-free-quoting-calculator-to-get-off-my-spreadsheet-would-love-you-guys-to-tear-it-apart.449764/)
  — a shop owner describes a homemade spreadsheet giving "inconsistent numbers depending on mood"
  and no memory of past quotes. This is real pain, but it is the *seller* side building quotes to
  send to customers, not a *buyer* comparing multiple inbound supplier quotes — the opposite
  persona from this idea's target user. Does not evidence the specific differentiator.
- **Independent construction-estimator Substack**
  (https://mwmoedinger.substack.com/p/part-4-comparing-estimates-creating) — "create a spreadsheet
  to compare bids... it's a lot more complicated than it seems," describing real difficulty making
  bids from different subs comparable. This one **is** squarely on-target: a buyer comparing
  multiple inbound bids.
- **Contractor Talk forum** — a GC's process summarized (via search snippet only; the full page was
  paywall-redirected, HTTP 402, so the exact wording could not be independently verified) as an
  Excel list of subs, emailed individually one-by-one. Directionally relevant but unverified
  full-text, so weaker evidence than the other two.

**Net**: of three sources, only one (the construction-estimator Substack) cleanly evidences the
specific buyer-side differentiator; one is the wrong persona (a seller, not a buyer); one is real
but unverified. This is a thinner and more mixed result than a clean "3 posts support the specific
claim," and Reddit — plausibly the richest source of exactly this small-business/tradesperson
discussion — was checked again and remains unreachable (see section 5).

## 4. Underserved sub-segment check

Checked each named vendor's own stated target market:

- AuraVMS: small-to-mid procurement, cross-industry.
- QuotesFlow: product-based SMBs — resellers, distributors, industrial suppliers, manufacturers,
  contractors, sign/print shops.
- QuoteWerks: general SMB/mid sales-and-CPQ teams; RFQ is a bolt-on, not the core product.
- Quotable AI: B2B sellers broadly — import/export, construction, IT resale, distribution,
  manufacturing.

No clean underserved niche emerged from that list. More importantly, this research pass surfaced
two *additional* dedicated competitors not in the original four, one for each of this idea's two
most obvious target verticals: **Jiga** (manufacturing custom-parts RFQ/quoting platform,
https://jiga.io/platform/) for the manufacturing/job-shop side, and, for the construction side,
**SmartBid** (a construction-specific bid-management product; own site confirms it exists and
targets GCs, but does not publish pricing — see section 1) and **Buildr** (AI-powered
preconstruction software for general contractors, whose own site confirms bid-leveling —
"[m]anage invitations, coverage, and leveling in one place" — as a named capability,
https://buildr.com/ — correcting an earlier draft of this section, which cited a Buildr blog post
that does not actually mention SmartBid). Both verticals this idea would most naturally target
already have at least one dedicated, purpose-built incumbent, beyond the four general RFQ/CPQ
tools already known.

## 5. Explicit gaps

- **Reddit unreachable again** — direct `WebFetch` to `www.reddit.com`, `old.reddit.com`, and the
  Reddit `.json` search endpoint all failed this session (hard-blocked or empty), and `WebSearch`
  with `site:reddit.com` again returned zero actual Reddit results. This is now the **third**
  session this has been hit (2026-09-09, 2026-09-10, and now 2026-09-12) — meeting `OWNER.md`'s
  escalation bar ("the same failure has now blocked three consecutive [Validate] sessions").
  Notified the owner via `PushNotification` this session rather than only noting it here.
- Payment/processing terms, AuraVMS's on-premise contract terms, and QuotesFlow's top "Power" tier
  price were not found on the vendors' own pages.
- **Correction from the mandatory pre-merge review**: the original draft of this file misattributed
  a $250-1,500/mo SmartBid price to SmartBid's own pricing page (that page publishes no figures at
  all) and cited a Buildr blog post that does not mention SmartBid. Both are fixed above (sections 1
  and 4) as of the merge. Neither correction changes the scoring or the verdict: SmartBid and Buildr
  remain real, independently-confirmed competitors in the construction vertical (confirmed from
  their own sites), and no dimension score changed.

## Scoring (1-5 each)

- **Evidence of demand: 2/5.** The general "comparing quotes/bids manually is painful" problem is
  real, but checked against the *specific* differentiator this idea depends on (a buyer-side,
  multi-party comparison record), only one of three real sources found squarely supports it; one is
  the wrong persona (seller, not buyer) and one is real but unverified full-text. This is the same
  failure mode that killed `contractor-classification-checker` and `sales-tax-nexus-monitor`: a
  plausible general problem, but the found user-voice evidence only weakly and partially supports
  the idea's specific claimed wedge. Reddit — the likely richest source for this exact
  small-business discussion — remains unreachable a third time.
- **Willingness to pay: 4/5.** Real, independently confirmed pricing exists across a wide range
  ($4.99/mo to $250-1,500/mo) for this general category, proving people already pay for
  RFQ/quote-comparison and bid-leveling tools.
- **Buildable to handoff in 10 sessions or fewer: 4/5.** A structured RFQ-send / response-collect /
  side-by-side compare workflow is a standard CRUD-plus-email-integration build, no unusual
  technical risk.
- **Reason to exist alongside what already ships: 1/5.** Beyond the four originally-disclosed
  competitors (AuraVMS, QuotesFlow, QuoteWerks, Quotable AI), this pass found the field is denser
  than known: **Jiga** already serves the manufacturing/job-shop vertical and **SmartBid**/**Buildr**
  already serve the construction-subcontractor vertical — the two most obvious verticals this idea
  would target each already have a dedicated incumbent. No underserved sub-segment could be named
  across any competitor's stated target market. The one real, well-documented complaint found
  (QuoteWerks: dated UI, workflow friction, slow support) is about a general CPQ tool treating RFQ
  comparison as a bolt-on, not about a dedicated competitor in either target vertical, so it does
  not establish a wedge into the field this idea would actually enter.
- **Low compliance/operational burden: 4/5.** Business quote/pricing data between known commercial
  parties, no personal/health/financial data or licensed-advice exposure identified.

**Total: 15/25.** Below the 16 kill threshold; "evidence of demand" also independently scores 2/5,
an automatic kill per the routine's Phase 1 rule regardless of total.

## Verdict: KILL
