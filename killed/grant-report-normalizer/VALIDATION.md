# Validation: Grant-report normalizer for small nonprofit program staff

## Method note
Research was delegated to a subagent, then every quoted claim and every negative finding ("no
evidence found") below was independently re-fetched and confirmed by the acting session itself via
direct `WebFetch` calls, per the standing `RUNBOOK.md` lesson that delegated subagent quotes must
not be trusted without independent re-verification. The subagent's own report flagged one integrity
issue in its own process (a WebSearch summary that fabricated statistics attributed to
fundeasy.com, caught by the subagent itself against a direct fetch) — that fabrication is not used
below. Separately, the two quotes originally carried over from `BACKLOG.md` and attributed to
exponentphilanthropy.org were independently re-fetched by the acting session directly (not just
re-trusted from the subagent or from the prior day's carry-over) and both confirmed verbatim.

## 1. Demand evidence
No individually-attributed nonprofit staffer post was found, despite genuine multi-angle search
(WebSearch site-restricted and unrestricted; Reddit blocked to fetch tools in this environment, as
in prior sessions), stating in their own words that they have to reformat the same underlying data
into *multiple different* funders' report templates. What was found instead, all independently
re-verified:

- **Exponent Philanthropy** (funder-serving association blog, written to persuade funders, not a
  grantee's own voice) —
  [exponentphilanthropy.org](https://exponentphilanthropy.org/blog/how-to-simplify-grant-applications-and-reports-for-nonprofits/),
  re-fetched directly. Verbatim: "A single organization may juggle 40–60 applications and just as
  many unique reporting requirements from 20–30 funders, each with its own format and timeline."
  Verbatim: "Reduces burden – Reformatting is one of the most time-consuming tasks for
  nonprofits." (both confirmed present on direct fetch, correcting the possibility raised by the
  subagent's own fabrication warning about a *different* page).
- **PEAK Grantmaking** (grantmaker-side membership association, survey of 280 grantmaking
  organizations — the grantmakers' own reported policy, not a grantee complaint) —
  [peakgrantmaking.org](https://www.peakgrantmaking.org/insights/grant-reporting-the-current-state-of-practice/),
  re-fetched directly. Verbatim: "most grantmakers require reports in their own format and will not
  accept a common report submitted to multiple funders." About a third of respondents already
  accept a common report; another third are willing to consider it — i.e. roughly two-thirds do
  **not** currently accept one, confirming the structural problem (differing per-funder formats) is
  real and widespread, but this is grantmakers describing their own policy, not grantee-voiced pain.
- **NonprofitAF.com** (Vu Le, a real, named, widely-read nonprofit-sector practitioner blog) —
  [nonprofitaf.com](https://www.nonprofitaf.com/report-crappy-funders/), re-fetched directly.
  Verbatim: "Making nonprofits translate their budget into a funder's budget format: It is
  ridiculous and self-centered for any funder to expect anyone to convert their budgets into the
  funder's format, especially if the funder's format is in—gasp!—Microsoft Word!" This is genuine,
  attributable practitioner-voiced pain about the exact reformatting task, from a listicle of
  complaints about "a funder who does any of the crappy practices listed below" (i.e. applicable to
  any funder exhibiting the practice, not framed as literally about a single specific funder) — but
  it still describes the pain of one instance of reformatting ("a funder's format"), not explicitly
  the compounding, many-simultaneous-templates pain the idea's own thesis rests on.
- **FundEasy** vendor blog anecdote (an unnamed "development director at a mid-sized rescue
  mission") — explicitly disclosed by the research subagent as illustrative vendor marketing copy,
  not a verified real-user testimonial. Excluded as evidence per routine non-negotiable #4.
- **TechSoup** (Dalya Massachi, Dec 7 2020) confirms the structural problem from a nonprofit
  resource org's perspective: "Each funder has a specific grant reporting format in mind...
  Organizations will need to develop funder-specific formats" — but is guidance/advocacy content,
  not a user complaint.

**Assessment**: the underlying structural problem (differing per-funder report formats, confirmed
by an accurately-quoted advocacy statistic and an independent 280-organization grantmaker survey)
is real and well-documented. But the specific bar this project otherwise applies — real,
individually-voiced user complaints describing the pain of the *specific* differentiator the idea
is built around (here: doing this across *many* funders, not one) — is not met. One attributable
practitioner complaint exists, but about a single-funder format demand, which a generic
letter-writing/formatting task (arguably already served by a general LLM prompt) could address
without needing a multi-funder-template product. This is "no evidence found" on the idea's specific
thesis, which routine non-negotiable #4 says to treat as a signal, not fill with the (accurate, but
not user-voiced) aggregate statistics.

## 2. Competitor / existing-solution search (run fresh, per BACKLOG's own caveat)
`BACKLOG.md` disclosed that Foundant and Submittable were checked and found to be funder-side
intake portals, not a grantee-side reformatting competitor, and flagged that the search needed to
be re-run fresh rather than trusted. Re-run fresh, it found a **materially closer direct
competitor that the original sourcing pass missed**:

- **Sopact** — [sopact.com](https://www.sopact.com/use-case/nonprofit-grant-management-software),
  re-fetched directly. Explicitly grantee-side: "This guide is for grant recipients. Foundations
  and agencies selecting grantees need a different workflow." Verbatim, and a near-exact match to
  this idea's core concept: "Store the evidence separately from each report's presentation. Funders
  can receive different explanations and views while the underlying sources, definitions and
  calculations remain consistent." Pricing
  ([sopact.com/pricing](https://www.sopact.com/pricing), re-fetched directly): Power "$299 per
  month, billed annually" (~$3,588/yr), Growth "$799 per month, billed annually" (~$9,588/yr),
  Enterprise custom, plus a mandatory "Setup engagement: $200 per hour, 10-hour minimum" (**$2,000
  minimum upfront**).
- **Tahua** — serves both funder and grantee sides; verbatim: "funders design proportionate
  reporting requirements and provides grantees with modern portals that make multi-report
  management more efficient," and separately quotes "organisations receiving multiple grants spend
  15-30% of staff time on grant reporting and compliance" (secondary source, not independently
  re-fetched by the acting session; flagged as such). No pricing disclosed.
- **Foundant GrantHub** (Foundant's grantee-side product, distinct from their funder-side GLM):
  reported by the subagent via secondary aggregator pages (TrustRadius/ITQlick), not independently
  re-fetched by the acting session — historical pricing "starting at $995 per year," with some
  secondary sources suggesting the "Pro" tier has been discontinued. Treated as directional, not
  confirmed.
- Knack, FundEasy, and Fluxx are adjacent (internal dashboards, funder-side portals, or a single
  organization's own template standardization) but none was found to reformat one dataset into many
  *other* funders' own distinct templates the way Sopact explicitly does.

**This corrects `BACKLOG.md`'s framing.** The claim "no direct named competitor was found" does not
survive a fresh search: Sopact is a real, currently-sold, near-exact match to the idea's core
mechanism (store data once, present differently per funder), not merely an adjacent tool.

## 3. Free/DIY substitute check (per the 2026-09-17 `RUNBOOK.md` lesson)
- Some individual funders now require **direct online-form entry into the funder's own portal**,
  removing the "upload a mismatched document" version of the pain for that one funder — e.g.
  **FFAR** ([foundationfar.org](https://foundationfar.org/grants-funding/required-forms-reporting/),
  re-fetched directly): "Narrative Report: To be filled out as an online form in FFAR's Grants
  Management System (BBGM). Uploads of the form are not accepted." This does **not** remove the
  core multi-funder pain: a grantee with 20-30 funders would still re-enter the same underlying
  numbers into 20-30 differently-structured portals (confirmed structurally by the PEAK Grantmaking
  finding above that ~two-thirds of funders do not accept a shared/common format).
- A free **regional "common grant report" template** exists for a subset of funders who opt into a
  consortium format (e.g. Council of Michigan Foundations), per secondary search-result summaries
  only (the primary page 403'd on fetch, so not independently confirmed) — but per PEAK
  Grantmaking's own survey, only about a third of funders currently accept a common report, so this
  free option covers a minority of a typical nonprofit's 20-30 funder relationships, not 80% of the
  job.
- No evidence found of a widely-used free spreadsheet/Word template on TechSoup or
  councilofnonprofits.org that solves the cross-funder reformatting problem generally; TechSoup's
  own guidance instead tells nonprofits they must build funder-specific formats themselves.

**Assessment**: no free/DIY substitute closes 80% of the gap; this check does not kill the idea on
its own.

## 4. Willingness-to-pay comparables
Real, verified paid comparables establish that small nonprofits do pay for adjacent grants-tracking
software, and — critically — that a near-exact competitor (Sopact, above) is already being sold for
this specific workflow:
- **Sopact**: $3,588-$9,588/yr + $2,000 minimum setup (re-fetched directly, see above).
- **Instrumentl**: reported via pricing-aggregator pages (not the vendor's own page directly, not
  independently re-fetched) — Discover plan ~$299-349/mo, up to Full Lifecycle ~$999-1,159/mo;
  lowest annual commitment ≈$3,588/yr; requires 501(c)(3) status.
- **Foundant GrantHub**: ~$995/yr (secondary source, not independently re-fetched).
- **Sumac**: base plan "$109/month" (grant-management is an add-on with cost not publicly listed).
- **Bloomerang**: Starter "$79/month," Standard "$125/month" (secondary aggregator pages, not
  independently re-fetched) — general nonprofit CRM with a grant-tracking dashboard feature, not a
  reformatting tool.
No nonprofit-specific discount or free tier was found for any of these named tools; TechSoup offers
general sector-wide software discounts but no evidence found of one specifically covering a
grant-report-reformatting product.

**Assessment**: WTP for the general category is real and verified — nonprofits already pay
$1,000-$9,500+/yr for adjacent or directly-overlapping tools. The risk this dimension does not carry
is "no one would pay for this"; the risk it does carry is captured under "reason to exist" below.

## Scoring (1-5 each)

| Dimension | Score | Basis |
|---|---|---|
| Evidence of demand | **2** | The underlying structural problem (differing per-funder formats) is real, confirmed by an accurately-quoted advocacy statistic and an independent 280-org grantmaker survey — but zero individually-voiced grantee complaints specifically describing the idea's own multi-funder-template thesis were found despite genuine multi-angle search, and the one attributable practitioner complaint found (NonprofitAF) is framed as single-funder, not multi-funder. Aggregate/advocacy statistics are not a substitute for user-voiced demand for *this specific* differentiator, per routine non-negotiable #4 ("no evidence found" is a signal, not filled with adjacent-but-different evidence). |
| Willingness to pay, plausible price point | 4 | Real, independently re-verified paid comparables exist, including a near-exact direct competitor (Sopact) already charging $3,588-$9,588/yr plus a $2,000 setup minimum for this specific workflow. WTP for the category is well-established. |
| Buildable to handoff in ≤10 sessions | 3 | No single public API to build against (unlike prior candidates with one platform's API); requires a data model for program/budget/outcome fields plus a per-funder template-mapping mechanism and document export. Feasible as a narrowly-scoped MVP but more design-heavy than most candidates validated so far. |
| Reason to exist alongside what already ships | 2 | Sopact is a real, currently-sold, near-exact match to the core mechanism (store data once, present differently per funder) — contradicting `BACKLOG.md`'s "no direct competitor found" once the search was actually re-run fresh, as it flagged was necessary. The only differentiation identified (a cheaper, narrower tool for the smallest, tightest-budget nonprofits that Sopact's $3.6-9.6k/yr+setup pricing prices out) is an inferred architectural argument with no user-voice evidence found that small nonprofits specifically want or are asking for a cheaper alternative — the same "plausible-sounding argument without user-voice evidence" failure mode the 2026-09-09 `RUNBOOK.md` lesson warns against. |
| Low compliance / operational burden | 4 | Organizational financial/program data, not consumer PII; no regulatory trigger; standard SaaS data-handling burden. |

**Total: 15/25.**

## Verdict: KILL

Two independent grounds both point to kill, per the routine's own rule (total <16, or an auto-kill
dimension at ≤2):
1. **Total score is 15/25**, below the 16 floor.
2. **Evidence of demand scores 2/5**, an explicit auto-kill dimension, on real "no evidence found"
   grounds (no individually-voiced grantee complaint matching the idea's specific multi-funder
   thesis), not an unsupported or pattern-matched low score.

The willingness-to-pay risk `BACKLOG.md` flagged as the biggest concern actually resolved
favorably (WTP scores 4/5, backed by a real paid near-exact competitor) — but the fresh competitor
search `BACKLOG.md` itself said was necessary surfaced a different, unanticipated problem: a direct
competitor (Sopact) already exists and already charges for this exact mechanism, and the demand
evidence for the idea's own specific angle (many funders, not one) turned out to be aggregate
statistics and advocacy commentary rather than real user voice.

See `killed/grant-report-normalizer/REASON.md` for the filed kill record.
