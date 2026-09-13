# Validation: Co-owned vacation property scheduling and expense-splitting

Research delegated to a subagent this session, applying both 2026-09-12 `RUNBOOK.md` corrections:
(1) a deep review-page complaint check on the two previously-disclosed incumbents (not a
`WebSearch`-summary-depth pass), and (2) an explicit underserved-sub-segment search before scoring
"reason to exist." Reddit reachability was also re-tested as a fourth data point.

## 1. Deep complaint check — OurSharedPlace ($99/property/yr)

No G2, Capterra, or Trustpilot listing exists for this product (`trustpilot.com/review/oursharedplace.com`
returns HTTP 404; no G2/Capterra search hits). iOS App Store listing
(`https://apps.apple.com/app/id6768829444`) shows only 2 total ratings (5.0 avg, no written
reviews). Android listing is a thin "Open Beta" page with no usable review content. The only
substantial online presence is the founder's own promotional blog
(`https://stevemurch.com/managing-a-vacation-home-with-oursharedplace/2025/11`).

**Finding: no independent review coverage exists at all** — not "reviewed well," just too small/new
to have any. This is itself informative: there's no documented complaint trail to build a wedge
from.

## 2. Deep complaint check — PlumConnect

iOS App Store (`https://apps.apple.com/us/app/plumconnect/id6557079052`): 5.0 stars, only 4 ratings,
one visible review is positive ("This is great... wonderful experiences with Plum"). Google Play
listing was unfetchable. Trustpilot has no page for `plumconnect.com`; the parent company **Plum
CoOwnership** has a page (`https://www.trustpilot.com/review/plumcoownership.com`, 4.4/5, 11
reviews, 0 in the last 12 months) but those reviews are about the brokerage/fractional-purchase
service, not the app's calendar/ledger functionality.

**Finding: same pattern as OurSharedPlace** — negligible independent review footprint, no
documented complaints about the actual product being validated against.

## 3. Underserved-sub-segment search

| Avenue | Result |
|---|---|
| Budget-conscious smaller groups | **Already served.** CabinPals (free tier + $89.99/yr paid, `cabinpals.com`) and SharedKey ($49/yr) already undercut OurSharedPlace. |
| PlumConnect standalone pricing | PlumConnect's DIY tier is ~$600/yr (`plumcoownership.com/plumconnect-app`) — a real 6x gap vs. OurSharedPlace, but... |
| International / cross-currency | Mercury (PlumConnect's banking backend) is USD-only with KYC friction for non-US owners (`support.mercury.com`) — a real constraint, but a UK competitor, **Shared Holiday Homes**, already exists and explicitly markets itself against SharedKey (`sharedholidayhomes.com/blog/shared-holiday-homes-vs-sharedkey`), so this isn't a vacant gap either. |
| Multi-generational trusts/LLCs | No software gap or competitor found either way — only law-firm advisory content, not evidence of an unmet product need. |
| Larger groups (10+ co-owners) | No gap found — all named competitors advertise "unlimited members." |

**Overall competitive density**: beyond the original two, the search surfaced **five more direct
competitors already serving this exact niche**: CabinPals, SharedKey, Shared Holiday Homes, CalDibs
(calendar-only, free tier), and House Matters (`housematters.app`, ~$10/mo, explicitly does
expense-splitting + ledger for family-owned property). That's **seven named competitors total**
spanning free to $600/yr, not two.

## 4. Real user-voice evidence for the specific differentiator (persisted ledger/calendar, not a
one-time split)

- **Reddit: unreachable a fourth consecutive Validate session** (2026-09-09, 09-10, 09-12, 09-13).
  Direct `WebFetch` to reddit.com/old.reddit.com failed outright; `site:reddit.com` `WebSearch`
  queries ran but surfaced no on-topic threads. Per `RUNBOOK.md`, this gap is already escalated and
  not re-notified.
- **Bogleheads** (`bogleheads.org` itself blocks direct fetch via Cloudflare challenge; findings are
  from `WebSearch` snippets of real threads, not full-page verification):
  - `viewtopic.php?t=437786` — one poster describes maintaining "a budget table that takes each
    owner's carryover balance from last year minus their share of the budget," calling it "messy."
    This is genuine, specific evidence of someone wanting exactly the persisted-ledger property the
    idea depends on, not a one-time split.
  - `viewtopic.php?t=370050` and `viewtopic.php?t=459799` — general cost-splitting and inherited-
    property coordination discussion; neither explicitly asks for a persisted tracking tool.
- No city-data.com, FlipKey/VRBO, or fractional-ownership forum threads found on this specific
  point; only generic advisory listicles (e.g. cabinlife.com) describing scheduling/expense disputes
  as common categories, without a first-person "I want a kept record" quote.

**This does not clear the routine's three-real-post bar with real specificity**: one strong,
on-point post (Bogleheads t=437786), two tangential, and a fourth major channel (Reddit) still
inaccessible.

## Scoring (1-5 each)

- **Evidence of demand: 2/5.** Only one genuinely on-point real user-voice post found across all
  channels tried; Reddit — the most likely source of first-person small-group financial griping —
  remains unreachable for a fourth session running. Per `RUNBOOK.md`'s "web research returns no
  usable evidence" entry, thin evidence is a real signal, not a search-depth artifact to wave away.
- **Willingness to pay: 3/5.** A real price band exists ($49-99/yr for OurSharedPlace/SharedKey/
  CabinPals, ~$10/mo for House Matters, ~$600/yr for PlumConnect), proving *some* market pays for
  this category — but that's evidence incumbents can charge, not evidence a new entrant would
  capture spend from them.
- **Buildable to handoff in ≤10 sessions: 4/5.** A shared calendar plus expense ledger with no
  required bank integration (unlike PlumConnect) is a straightforward CRUD/auth/calendar-UI build.
- **Reason to exist alongside what already ships: 1/5.** Deeper search found **five additional
  direct competitors** beyond the two originally disclosed — seven total, spanning free to $600/yr,
  covering budget-conscious, unlimited-member, and (for the UK) international segments. The two
  narrow gaps found (PlumConnect's price, international/cross-currency) are each already served by
  a different existing competitor (CabinPals/SharedKey on price, Shared Holiday Homes on the UK
  market). No unserved gap was found.
- **Low compliance/operational burden: 4/5.** No payment processing or bank integration required
  for a calendar+ledger-only v1 (unlike PlumConnect's Mercury integration), so burden is low.

**Total: 14/25.** Also independently triggers the kill floor on "evidence of demand" scoring 2/5
(routine: "Kill the idea if... demand, willingness to pay, or buildability scores 2 or below").

## Verdict: KILL

This is the second idea in a row (after `rfq-quote-comparison`, day 010) killed primarily on
crowded-market grounds rather than a lack of a well-defined problem — but unlike that kill, here the
2026-09-11 sourcing-method review's own preferred idea shape (multi-party coordination / persisted
shared state, not a single-user monitor/tracker) turned out to be colonized too. "Shared calendar +
expense ledger for a co-owned group" is evidently a broadly copyable, already-saturated app pattern
regardless of which multi-party niche it's applied to (vacation homes here; the prior sourcing
review found the same for notary renewals, OSHA logs, CE tracking, and RFQ comparison). See
`RUNBOOK.md` for the widened entry recording this.
