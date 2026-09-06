# Validation: Rent increase / notice compliance calculator

Research run via two independent subagents (one on competitor/pricing landscape, one on
first-person problem evidence), day 004, 2026-09-06. All URLs below were directly fetched or
returned by web search by those subagents, not invented.

## Mainstream property-management software — does it already bundle this?

Checked TurboTenant, DoorLoop, Buildium, Avail, Zillow Rental Manager, Innago, Rentec Direct,
Stessa, AppFolio, Landlordy. Consistent pattern: every one of these that touches rent-increase
notices at all (TurboTenant, DoorLoop, Avail, Zillow, Innago) offers only a **free static
letter template or blog article**, not an automated jurisdiction-aware cap/notice-period
calculation. None found with a paid tier specifically gating this calculation feature.
- TurboTenant: https://www.turbotenant.com/property-management/rent-increase-letter/ — template
  content, Premium account gates a 32-form pack (price not disclosed on
  https://www.turbotenant.com/property-management/pricing-payment-management/).
- DoorLoop: https://support.doorloop.com/en/articles/6184925-schedule-a-rent-increase — only
  schedules a rent-charge change and sends a generic notice; no legal cap/notice-period check.
  Pricing (general PM suite, not this feature): Starter ~$69-79/unit/mo, Pro ~$139-169/unit/mo,
  Premium ~$199-229/unit/mo (https://www.doorloop.com).
- Buildium: https://www.buildium.com/pricing/ ($62/$192/$400 per month tiers) — no rent-increase
  compliance feature found anywhere on the pricing page.
- Avail: https://www.avail.com/education/articles/what-to-include-in-a-rent-increase-notice —
  free article + template only.
- Zillow Rental Manager: https://www.zillow.com/rentals-network/rent-increase-notice/ — same,
  free static template.
- Innago: https://innago.com/rent-increase-notice/ — free static template.
- Rentec Direct, Stessa, Landlordy, AppFolio: no evidence found of a dedicated feature here at all.

## Standalone tools that already do exactly this, for free

- **Rentlane** — https://getrentlane.com/tools/rent-increase-calculator — free calculator + state
  notice-period lookup + notice-letter generator, i.e. all three components of this idea, at $0.
  Their adjacent mobile PM app is $8.99/month, but the calculator/letter tool itself is free.
- **LeaseBase** — https://leasebase.ai/ab1482-calculator/ (California) and
  https://leasebase.ai/nyc-rent-stabilization-calculator/ (NYC) — free, no-signup calculators that
  output a pre-filled notice letter.
- At least six more free single-jurisdiction calculators found in search: RentGuard CA
  (rentguardca.com), Shuk Rentals (shukrentals.com/tools/calculators/rent-increase-notice-california),
  rentcap.netlify.app, rentlatefee.com, lawagreements.com/tools/rent-increase-calculator, openigloo
  (openigloo.com/nyc-rent-stablized-calculator).
- UtilityProfit (https://www.utilityprofit.com/rent-increase-letter-generator-pdf) generates a
  free letter but explicitly does not calculate the cap/notice period itself — a weaker match, but
  still free.

No standalone competitor was found with a **paid** price point for this specific sub-feature; the
two closest primary-purpose matches (Rentlane, LeaseBase) both give the core functionality away
free and monetize only via an unrelated adjacent PM app subscription.

## Government/nonprofit free substitutes

State/city bodies (Oregon DAS, Washington Dept. of Commerce, NYC Rent Guidelines Board) publish
the raw statutory cap percentage or notice-form requirement, but none were found offering an
interactive calculator themselves — that gap is filled by the private free tools above, not by
government. California has no official state calculator; the California Apartment Association (a
landlord trade group, not government) publishes a free CPI calculator for AB 1482
(https://caanet.org/caa-updates-cpi-calculator-for-rent-increases-under-ab-1482-2026/).

## First-person evidence of the problem

Reddit (r/landlord, r/PropertyManagement) and G2/Capterra reviews were largely inaccessible to the
research tooling (no indexed results, 403s on direct fetch), so evidence below skews toward
BiggerPockets forums and one news case rather than the full source list attempted. Three solid
first-person instances found, plus one adjacent non-US case:

1. Michigan landlord, BiggerPockets forum, July 2016: *"It's my understanding that, in Michigan, a
   landlord can raise rent a 'reasonable' amount as specified in the lease. It's usually 5% a
   year, but is it legal to go more, say 10%?"* —
   https://www.biggerpockets.com/forums/52/topics/329534-whats-the-highest-a-landlord-can-raise-rent-in-michigan
   Genuine uncertainty about whether a percentage cap exists at all (it does not, in Michigan).
2. California landlord, BiggerPockets forum, Jan 2021: asks whether he's restricted to a 5%/year
   increase on a newly-acquired below-market property, whether he can raise every 3 months instead,
   or whether hitting tenants with a $300 jump in month one is workable —
   https://www.biggerpockets.com/forums/52/topics/910881-rent-control-in-california
   Confusion about how AB 1482-style caps interact with under-market rent on acquisition.
3. Maine landlord, BiggerPockets forum: *"I just saw that Maine requires a 45-day notice before a
   rent increase can go into effect"*, then asks how to operationalize that against her renewal
   calendar — https://www.biggerpockets.com/forums/52/topics/805134-rent-increase-in-maine
4. Adjacent, non-US: CBC News, Charlottetown PEI, May 30 2019 — a landlord raised rent from $800 to
   $1,500 without authorization, pleading unfamiliarity with the Residential Property Act; tribunal
   ordered a $7,800 refund and capped rent at $800 until a legal increase was approved —
   https://www.cbc.ca/news/canada/prince-edward-island/pei-charlottetown-rent-increase-illegal-1.5164764
   Real financial consequence from not knowing the legal limit, though directional (Canadian
   jurisdiction) rather than direct US market evidence.

**Willingness to pay: no evidence found.** No first-person account surfaced of a landlord paying
for a rent-increase-compliance check specifically (e.g. hiring a lawyer for this alone, or asking
what such a service should cost). All three BiggerPockets threads were resolved by other forum
members answering for free, in the thread, at no cost to the asker — which is itself a working
free substitute for the exact moment of need this idea targets.

## Scoring (1-5 each)

- **Evidence of demand: 3.** Real, recurring confusion exists (three distinct US states, three
  separate posters, over a 5-year span), but every instance was resolved by a peer answering for
  free in the same forum thread within the same day. That is a working, zero-cost substitute for
  the exact use case, not just adjacent competition.
- **Willingness to pay: 1.** No evidence found of anyone paying, or expressing willingness to pay,
  for this specifically. Combined with the standalone-competitor research (both close primary-purpose
  matches, Rentlane and LeaseBase, give the exact functionality away free and monetize only an
  unrelated adjacent app), this is a strong negative signal, not just an absence of positive
  evidence.
- **Buildable to handoff in ≤10 sessions: 4.** The core loop — jurisdiction lookup, cap/notice-period
  calculation, templated letter output — is straightforward CRUD/rules-engine work for a small,
  curated set of jurisdictions. Broad multi-jurisdiction legal accuracy at scale would be a much
  bigger, ongoing maintenance burden, but an MVP scoped to a handful of states is buildable in
  budget.
- **Reason to exist alongside what already ships: 1.** This is a more direct hit than either prior
  kill: the exact proposed feature set (cap calculation + notice-period lookup + letter generation)
  already exists, free, in at least eight separate tools found in a single research pass (Rentlane,
  LeaseBase, RentGuard CA, Shuk Rentals, rentcap.netlify.app, lawagreements.com, openigloo,
  rentlatefee.com), none of which charge for it. A new entrant is not filling a gap; it is
  competing with an already-commoditized free category.
- **Low compliance/operational burden: 2.** Rent-control law changes yearly in several
  jurisdictions (Oregon and Washington caps are CPI-recalculated annually) and a wrong calculation
  carries real legal/financial consequences for the end user (see the PEI tribunal case above:
  refund ordered, rent rolled back) — this is a genuine ongoing legal-accuracy maintenance
  liability, not a low-burden category.

**Total: 3 + 1 + 4 + 1 + 2 = 11/25.**

## Verdict: KILL

Total is well under the 16 threshold, and "willingness to pay" independently scores ≤2, which is
itself an auto-kill condition per the routine's Phase 1 rule. The exact feature set proposed
(jurisdiction-aware cap calculation, notice-period lookup, and letter generation, together) is
already shipped free by at least eight distinct tools, monetizing only through unrelated adjacent
products. Real user confusion does exist (three genuine first-person instances across three US
states), but it is currently being resolved for free — either by peers in a forum thread or by one
of the free calculator tools above — leaving no identified wedge for a paid entrant.
