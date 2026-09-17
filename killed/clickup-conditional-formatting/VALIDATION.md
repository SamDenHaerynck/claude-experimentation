# Validation: ClickUp custom-field conditional-formatting companion extension

## Method note
Research was delegated to a subagent, then every quoted claim and every negative finding
("no evidence found") below was independently re-fetched and confirmed by the acting session
itself via direct `WebFetch` calls, per the standing `RUNBOOK.md` lesson that delegated subagent
quotes must not be trusted without independent re-verification. One minor discrepancy surfaced: a
Jack Rhone comment's date was reported as "April 14, 2025" by the subagent and "February 27, 2025"
on independent re-fetch — the quoted text itself ("...over 4 years and still not a thing?") matches
exactly in both fetches, so this is treated as a non-substantive date-rendering artifact, not a
fabrication, and the exact day is omitted below rather than asserted from either source.

## 1. Demand evidence (real user posts, all independently re-verified)
Four distinct feature-request threads on ClickUp's own official public feedback board
(feedback.clickup.com, ClickUp's Canny instance), all asking for the same underlying capability
(value/state-based coloring of a field) applied to different ClickUp views:

1. [conditional-formatting-for-custom-fields-color-based-on-value](https://feedback.clickup.com/feature-requests/p/conditional-formatting-for-custom-fields-color-based-on-value)
   — 304 votes, opened Nov 23 2020 (~5 years open), by Chris Ray. Verbatim: "Is it possible to
   format the colour of 'Severity' based on the outcome of the formula? i.e. 1-10 = Green, 11-19 =
   Amber and 20-25 = Red." Verbatim comment (Jack Rhone, 2025): "...over 4 years and still not a
   thing?" Verbatim comment (Huw Whyment, Jan 27 2025): "+1 for conditional colouring."
2. [color-coding-in-table-view](https://feedback.clickup.com/feature-requests/p/color-coding-in-table-view)
   — 14 votes, opened Jul 20 2023, by Elaine Sarkisian. Verbatim: "The Task Visibility option in
   the Calendar View is helpful; having this option in the Table View would be helpful as well."
3. [board-view-color-by-custom-field](https://feedback.clickup.com/feature-requests/p/board-view-color-by-custom-field)
   — 16 votes, opened Feb 10 2020. Verbatim: "Please provide the option to Color the cards in Board
   view by 'Custom field', just like there is option to Color cards by 'Custom field' in 'Calendar
   View'."
4. [color-cards-in-board-view-based-on-custom-field-dropdown](https://feedback.clickup.com/feature-requests/p/color-cards-in-board-view-based-on-custom-field-dropdown)
   — 6 votes, opened Jul 9 2025, by Brandon Buscher. Verbatim: "It would be great if there were an
   option to customize the board view to color cards based on the value of a custom field
   dropdown... similar to using different colors of sticky notes on a physical Kanban board."

**Important undisclosed-until-now finding (from threads 2 and 3 above, both independently
verified):** ClickUp **already ships** value-based card coloring natively — but only in **Calendar
View** ("Color cards by 'Custom field' in 'Calendar View'"). The four requests above are all, in
effect, asking ClickUp to extend a feature it already built for one view to the other views (Table,
Board, List). This changes the risk profile materially from the original BACKLOG.md framing
("ClickUp itself has never shipped this natively") — it is more accurate to say ClickUp has shipped
the core mechanism natively for one view and simply hasn't extended it to others.

**Platform-diversity gap, honestly disclosed:** all four posts live on the same venue (ClickUp's own
feedback board). Repeated attempts (WebSearch and direct fetch, with and without domain
restriction) to find a corroborating Reddit r/clickup, Twitter/X, G2, or Capterra post specifically
on this topic found nothing — Reddit is blocked to fetch tools in this environment, and no on-topic
hits surfaced elsewhere. This is "no evidence found," not "evidence of absence," but it means the
three-distinct-user-post bar is met by three distinct posts on one venue, not three independent
venues.

## 2. Competitor / existing-solution search
- **ClickUp's own official App Marketplace/integrations** (https://clickup.com/integrations,
  fetched directly): zero apps mention conditional formatting, coloring, or highlighting of custom
  fields by value. Closest adjacent apps are external reporting/dashboard tools — EasyInsight
  ("Generate grids, pivot tables, charts, and tree reports with your ClickUp data"), Screenful
  ("Instant Analytics and Automated Reports for ClickUp..."), Tableau Web Connector ("Sync ClickUp
  data with Tableau to create dynamic dashboards") — none operate inside ClickUp's native List/Table
  view; all export data out to a separate dashboard.
- **Chrome Web Store** (searched directly for "clickup conditional formatting" and "clickup custom
  field color"): zero results ("It looks like there aren't any search results for your search").
  Broader "clickup" search surfaces only unrelated extensions (time trackers, task capture tools,
  an RTL-mode extension) — none address field coloring.
- **Adjacent-tool precedent**: an open-source project, `trello-colored-custom-fields` (GitHub), does
  the equivalent for Trello — proving the implementation pattern (client-side DOM/API-driven
  formatting overlay) works, but also demonstrating that the closest known analog to this exact
  product is a **free, unmaintained-as-a-business open-source side project**, not a paid product.
  This is a negative signal for willingness to pay specifically on this feature, addressed below.

## 3. Willingness-to-pay comparables
No direct comparable exists (no paid formatting-only ClickUp extension was found anywhere). Real
verified pricing for adjacent-but-different ClickUp add-ons, to establish general category WTP:
- **Screenful** (https://screenful.com/pricing) — Starter $39/mo, Pro $79/mo, Scale $149/mo,
  Enterprise $399/mo (full analytics/reporting dashboard suite).
- **Everhour** (https://everhour.com/pricing) — Team plan $8.50/user/month billed yearly, 5-seat
  minimum (time tracking, native ClickUp integration).
- **TimeCamp** (https://www.timecamp.com/pricing/) — Starter $2.99-3.99/user/mo, Premium
  $4.99-6.99/user/mo, Ultimate $7.99-9.99/user/mo (time tracking).
These establish that ClickUp teams do pay recurring fees for narrow single-purpose add-ons in
general ($3-150/mo range) — but every one of them is a **full analytics or time-tracking product**,
categorically more valuable per seat than a cosmetic coloring overlay. None establishes that anyone
would pay specifically for value-based cell coloring, and the one closest analog found
(`trello-colored-custom-fields`) is free.

## Scoring (1-5 each)

| Dimension | Score | Basis |
|---|---|---|
| Evidence of demand | 4 | Four distinct, independently re-verified posts over ~5.7 years, sustained and recurring, but all on one venue (ClickUp's own board); no independent cross-platform corroboration found despite a genuine attempt. |
| Willingness to pay, plausible price point | **2** | No comparable found for this specific feature category anywhere. The one true analog (`trello-colored-custom-fields`) is free/open-source, a real negative signal. General ClickUp-addon WTP evidence exists but is for categorically richer products (full reporting suites, time tracking), not a coloring-only overlay. This is "no evidence found" treated as a signal per routine non-negotiable #4, plus one concrete negative data point — not an unsupported low score. |
| Buildable to handoff in ≤10 sessions | 4 | ClickUp has a public API for custom fields/tasks; a Trello equivalent already exists proving the DOM/API overlay pattern works. Scope is narrow (List/Table view coloring only). |
| Reason to exist alongside what already ships | 2 | ClickUp already ships the exact underlying mechanism (value-based card coloring) natively in Calendar View. Every one of the four demand posts is, functionally, asking ClickUp to extend an already-built feature to more views — a materially cheaper lift for the vendor than the Webflow architecture-constrained precedent in `RUNBOOK.md`, and one ClickUp could ship for free at any time with no stated technical objection (unlike Webflow's admin reply citing real architectural constraints). No vendor statement of refusal or roadmap exclusion was found for this request, unlike the original Channel-A rationale that specifically prized a "vendor's own stated reason for declining." |
| Low compliance / operational burden | 4 | Client-side browser extension reading data via ClickUp's own API; no server, no PII handling beyond what the user's own ClickUp session already has access to. |

**Total: 16/25.**

## Verdict: KILL

Per the routine's Phase 1 rule, a score of 2 or below on demand, willingness to pay, or buildability
auto-kills regardless of total. Willingness to pay scores 2/5 here, on real, independently verified
negative evidence (no paid comparable found anywhere for this feature category; the closest known
analog is free/open-source) plus explicit "no evidence found" on direct pricing per routine
non-negotiable #4 — not an unsupported or pattern-matched low score. The reason-to-exist finding
(ClickUp already ships the core mechanism natively in one view, undercutting the original
"ClickUp itself has never shipped this natively" framing from `BACKLOG.md`) independently reinforces
the same kill, though it is not itself an auto-kill dimension.

See `killed/clickup-conditional-formatting/REASON.md` for the filed kill record.
