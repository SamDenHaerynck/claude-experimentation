# Validation: State/local sales-tax nexus threshold monitor with change alerts

Research pass run 2026-09-10, applying both widened `RUNBOOK.md` checks named in `STATE.md`: (1)
the 2026-09-08 check — after confirming a claimed (a)/(b)/(c) property holds, name the closest free
tool that is itself already recurring/embedded (not one-shot) for the same audience; (2) the
2026-09-09 check — verify real user-voice posts support the *specific* claimed differentiator, not
just the general problem. Reddit was again unreachable from this environment (direct fetch to
reddit.com rejected; domain-filtered search returned no reddit.com results) — same gap noted in the
2026-09-09 log, flagged again below, not filled with invented evidence.

## 1. Does property (a) hold? Yes, genuinely.

Economic-nexus thresholds do vary by state (commonly $100k revenue and/or 200 transactions, but
California is $500k, and measurement windows differ — current year, prior year, rolling four
quarters) and a seller's proximity to a threshold changes continuously as sales accrue
(https://docs.stripe.com/tax/monitoring — "Time windows" section). This is a real ongoing-tracking
problem, not a one-time lookup. Property (a) holds on its own terms.

## 2. Closest free, already-recurring/embedded tool for the same audience (2026-09-08 check)

Two are found, both free and already embedded in tools the target audience already uses:

- **Shopify's built-in "Tax liability insights" dashboard** (Settings → Taxes and Duties → United
  States → Review Insights) — free (Shopify Tax's calculation fee, 0.35%/0.25% on Shopify
  Plus, only applies after $100k in a state and only to actual tax calculation, not to the
  monitoring dashboard itself), already recurring (syncs with live sales), shows a "Monitoring"
  status once a state reaches 80% of its threshold
  (https://help.shopify.com/en/manual/taxes/us/us-tax-liability, corroborated by
  https://thetaxvalet.com/blog/shopifys-sales-tax-liability-and-nexus-dashboard-explained and
  https://ecomcpa.com/shopifys-new-feature-sales-tax-nexus-alert/). **Real gap found**: this
  dashboard does not push an email/notification alert — a merchant has to check it manually
  (confirmed by direct fetch of the Shopify help page itself, which describes only a dashboard
  status, no alert mechanism). This is a genuine, verified limitation, not assumed.
- **Stripe Tax's built-in threshold monitoring** — sends automatic email and Dashboard
  notifications when a location crosses a threshold, for any merchant who has opted into Stripe Tax
  (https://docs.stripe.com/tax/monitoring, fetched directly). This *is* an active-alert competitor,
  not just a passive dashboard, for any seller already using Stripe for payments. It is bundled at
  no separate charge for monitoring itself, though Stripe Tax's calculation feature carries its own
  per-transaction fee (0.5% no-code / $0.50 per API call) once a merchant actually turns on
  calculation — so "free" here means "no added cost for the monitoring/alert on top of a product
  many sellers already run," not "zero-cost in all cases."

**Verdict on this check**: property (a) holds, but it does not clear the bar the 2026-09-08 widening
requires. The exact "ongoing tracking that goes stale" job is already covered for free (passively)
by Shopify for any Shopify seller, and covered for free (actively, with alerts) by Stripe Tax for
any Stripe-processing seller. This repeats the `ci-migration-rollback-gate` failure mode.

## 3. Existing paid dedicated competitors (beyond TaxJar/Avalara named in BACKLOG.md)

- **TaxJar** — Starter plan raised to $39/month in 2026 (from $19); economic-nexus dashboard with
  automated approach-alerts is Professional-tier only, Starter gets a manual checker only
  (https://taxcloud.com/blog/taxjar-pricing-how-much-does-taxjar-cost/,
  https://www.galvix.com/article/taxjar-pricing/). Confirms BACKLOG.md's flagged incumbent risk.
- **Avalara** — free one-time nexus risk assessment, plus a "Small Business" tier that includes
  ongoing nexus monitoring with threshold alerts as a bundled feature
  (https://www.avalara.com/us/en/learn/nexus/nexus-risk-assessment.html,
  https://www.avalara.com/us/en/products/professional-services/sales-tax-risk-assessment.html).
  Specific price not found on Avalara's own site in this pass (secondary sources only).
- **NexusMonitor (Shopify App Store app)** — a small, dedicated, already-shipping indie SaaS doing
  essentially the exact product this idea proposes: aggregates Shopify/WooCommerce/Square sales
  into one dashboard, colour-coded nexus map, email alerts at custom percentage thresholds across
  46+ states, $19/month Starter, $39/month Growth (all states/platforms), $69/month Pro (adds
  rule-change alerts and PDF compliance reports), 14-day free trial
  (https://apps.shopify.com/nexusmonitor, fetched directly). This is the single most damaging
  finding: it is not an adjacent enterprise suite but a narrow point-solution, at accessible
  small-seller pricing, already live in the exact channel (Shopify App Store) this idea's target
  audience already shops in.
- **NexusFlag** — a comparable dedicated competitor: free nexus-exposure check, 14-day free trial
  (no card) for ongoing monitoring across all 50 states with threshold alerts
  (https://nexusflag.com/states, via search result summary; not independently fetched this pass).
- **Numeral** ("Monitor Sales Tax Nexus with Numeral") and **TaxJar "Nexus Insights"** — both list
  dedicated nexus-tracking as a named product/feature, additional evidence the category is already
  well served (https://www.numeral.com/product/nexus-tracking, https://www.taxjar.com/product/nexus-insights;
  page contents not independently fetched this pass, titles/positioning only).

**Verdict**: the space is crowded at every price point from $0 (Shopify, Stripe Tax, Avalara
one-time check, NexusFlag exposure check) to $19-69/month (NexusMonitor) to $39+/month (TaxJar) to
enterprise (Avalara). NexusMonitor in particular is close to a direct clone of this idea's own
concept, already live, at lower-than-planned pricing.

## 4. Real user-voice evidence, checked against the specific differentiator (2026-09-09 check)

Found and checked three real first-person posts/questions for whether they support the *specific*
claim this idea depends on — that changing thresholds create an ongoing maintenance burden a
one-time calculator doesn't take on (not just "sales tax nexus is confusing"):

- Shopify Community — "Do I need to register for sales tax in every state to sell nationwide?"
  (https://community.shopify.com/t/do-i-need-to-register-for-sales-tax-in-every-state-to-sell-nationwide/145353)
  — a one-time "do I need to register everywhere" question. No mention of tracking over time.
- Shopify Community — "Clarification on Automatic Tax Application and $100,000 Threshold for
  Shopify Tax"
  (https://community.shopify.com/t/clarification-on-automatic-tax-application-and-100-000-threshold-for-shopify-tax/283178)
  — a one-time mechanics question ("does tax turn on automatically at $100k?"), not a request for
  ongoing monitoring or alerts.
- Shopify Community — "Does Shopify track sales in states without physical nexus?"
  (https://community.shopify.com/c/shopify-discussions/does-shopify-track-sales-in-states-without-physical-nexus/m-p/1897259)
  — again a one-time factual question about what Shopify already does, not a complaint about needing
  a better ongoing-tracking tool.

**None of the three real posts found ask for ongoing monitoring, alerts, or express frustration
with a one-time calculator going stale.** All three ask a one-time factual question and would be
satisfied by a correct one-time answer (which free vendor content, and Shopify's/Stripe's own docs,
already provide). This is the same failure mode as the 2026-09-09 kill
(`contractor-classification-checker`): the idea's claimed differentiator is architecturally
plausible but unsupported by the only real user-voice evidence this pass could reach.

## 5. Explicit gaps

- Reddit (r/ecommerce, r/Shopify, r/smallbusiness, r/Etsy) unreachable again from this environment
  (WebFetch to reddit.com explicitly rejected; WebSearch with `site:reddit.com` returned zero
  reddit.com results both times). This is the second session running this gap has been hit
  (2026-09-09 and now 2026-09-10) — worth the owner's attention as a possible tooling/network
  limitation rather than a one-off.
- Avalara Small Business tier pricing and NexusFlag's paid-tier pricing were not found on the
  vendors' own pages in this pass, only via secondary summaries.
- Numeral's and TaxJar's dedicated nexus-tracking product pages were identified by title/URL but not
  independently fetched and read in full this pass.

## Scoring (1-5 each)

- **Evidence of demand: 2/5.** The general problem (multi-state nexus is real and states vary) is
  well documented by vendors, but every real first-person post found asks a one-time factual
  question, not for ongoing monitoring — the same gap that killed `contractor-classification-checker`
  the day before.
- **Willingness to pay: 4/5.** Real, verifiable price points exist for this exact category
  (NexusMonitor $19-69/mo, TaxJar $39+/mo, Avalara Small Business bundle), proving people do pay for
  nexus monitoring generally.
- **Buildable to handoff in 10 sessions or fewer: 4/5.** A threshold-tracking dashboard pulling from
  a sales data source (CSV upload or a platform API) with email alerts is straightforward, no unusual
  technical risk.
- **Reason to exist alongside what already ships: 1/5.** NexusMonitor is a near-identical, already-
  shipping product at accessible small-seller pricing in the same distribution channel (Shopify App
  Store); Shopify's own free dashboard and Stripe Tax's free bundled alerts already cover the core
  ongoing-tracking job for large parts of the audience. This is a more crowded field than any prior
  Phase 1 idea has faced.
- **Low compliance/operational burden: 3/5.** Lower legal-advice risk than the classification
  checker (this is tracking sales against published numeric thresholds, not making a legal
  determination), but a wrong threshold calculation or a missed rate/rule change could cause a real
  user to under-collect tax and face a real penalty — a genuine, non-trivial accuracy/liability risk.

**Total: 14/25.** Below the 16 kill threshold; "evidence of demand" also independently scores 2/5,
an automatic kill per the routine's Phase 1 rule regardless of total.

## Verdict: KILL
