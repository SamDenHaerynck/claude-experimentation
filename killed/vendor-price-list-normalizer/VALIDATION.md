# Validation: vendor price-list ingestion/normalizer for construction estimating software

Phase 1, day 018 (2026-09-20). Research delegated to a `general-purpose` subagent, briefed to
fetch every source itself and report honestly rather than stretch weak signal to a pass; findings
below independently reviewed by this session against the agent's own report (not re-fetched line
by line, but cross-checked for internal consistency and for whether "found" claims are backed by
an actual quoted source and URL).

## Highest-priority risk from `BACKLOG.md` #1: does demand clear the 3+ individually-voiced bar?

**No.** The subagent checked, by direct fetch:
- Capterra reviews (multi-page) for STACK Takeoff, PlanSwift, ProEst, Buildxact, Clear Estimates,
  Esticom/Procore Estimating — effectively every review for these six tools (~500+ reviews read).
- Software Advice reviews for PlanSwift, ProEst, Buildxact, STACK.
- GetApp (PlanSwift), Trustpilot (STACK, all 5 reviews).
- Mike Holt's Forum, ElectricianTalk, ContractorTalk, JLC Online Forums, Reddit, G2 (PlanSwift,
  Sage Estimating, STACK) — all blocked or empty (see below and the new `RUNBOOK.md` entry).

Two related-but-different complaints were found and verified by direct fetch:
- Trevor C., Construction, PlanSwift review, Software Advice, April 2020, 4/5: price changes on
  *internal* PlanSwift parts/assemblies aren't reflected in current jobs without manual edits or a
  paid $200 plugin — about PlanSwift's own internal price sync, not importing an external vendor's
  price sheet.
- Erin L., Owner/electrical contractor, Esticom review, Capterra, 2019-05-17, 5/5: time-consuming
  to *find* specific costs for wire/materials within the tool's own database — about lookup, not
  import/conversion of an external vendor list.

Neither matches the specific pain (converting an external vendor's price sheet into the tool's own
cost-code format). **Total individually-voiced complaints matching the exact same pain, including
the original Gary L./STACK Takeoff review: 1.** This is below this project's own 3+-voice bar for
treating demand as evidenced (see `RUNBOOK.md`'s "aggregate vs. user-voice" and "no evidence found
is a signal" entries), despite covering essentially every reachable review source for the six most
relevant estimating tools.

**Caveat, stated honestly rather than smoothed over**: several channels most likely to carry
trades-specific grassroots complaints were technically inaccessible to this session, not
searched-and-clean: G2 (403 on all three products), Mike Holt's Forum (403), ElectricianTalk and
ContractorTalk (both now paywalled behind a `tollbit.*` gateway returning 402 Payment Required),
JLC Online Forums (old forum URLs 301-redirect to the homepage — content appears gone), and Reddit
(fetch tool refuses reddit.com outright, a standing five-session-old limitation — see
`RUNBOOK.md`). Per routine non-negotiable #4, "no evidence found" from a source that could not be
reached is recorded as exactly that, not treated as a search that came back clean. A human with
authenticated or direct access to those specific sites could still find corroborating (or
disconfirming) evidence this session could not reach.

## Competitor re-verification

`BACKLOG.md`'s claim ("no general vendor price-list importer exists in STACK's marketplace or
comparable tools") does **not** fully hold once checked directly:

- **STACK's own marketplace** (stackct.com/integrations/, fetched directly): confirmed — only
  point integrations (LED Lighting Supply catalog access, BNi cost-data feed), no general importer.
  This part of the original claim is correct.
- **PlanSwift**: no general importer either; only a paid plugin that re-applies already-manually-
  entered prices, plus vendor-specific data feeds (Vision InfoSoft, EPIC Pricing).
- **Buildxact** (verified via its own help center): ships a real, built-in general vendor
  price-list importer today — upload an Excel price file, map columns to Buildxact's own item
  fields via dropdowns, errors flagged in red before import. It does not diff against the prior
  import (no change-flagging), which is this idea's other proposed feature.
- **Buildern**: advertises bulk price-list import from vendors/subcontractors on its own features
  page (buildern.com/features/construction-cost-catalog) — format-mapping/change-tracking detail
  unspecified.
- **Handoff**: advertises importing pricing from Excel/CSV/PDF/Word/images into its own catalogs
  (handoff.ai blog) — closest in spirit to "ingest messy vendor files," but for its own platform's
  catalog, not a cross-tool normalizer, and no change-flagging documented.

Net: the original claim ("no general importer anywhere") is wrong — Buildxact ships one natively
today, and two more tools advertise similar bulk-import capability. The narrower gap (a
cross-platform normalizer that outputs to multiple named estimating tools' formats *and* flags
deltas since the last import) still has no exact match found, but a contractor with this specific
pain already has a lower-friction option: switch to (or already be on) Buildxact, which solves the
core import friction natively, for free, as part of a tool they'd likely already need.

## Pricing survey (context, gathered regardless of verdict)

| Product | Tier | Price | Source |
|---|---|---|---|
| RSMeans Data Online (Gordian) | Core | $387.60/yr | rsmeans.com/products/online/tiers |
| RSMeans Data Online | Complete | $996.55/yr | same |
| RSMeans Data Online | Complete Plus | $5,674.35/yr | same |
| CostOS Estimating | Basic | $2,800/user, one-time | Capterra vendor pricing widget |
| Sigma Estimates | Professional | $1,380/user/yr | Capterra vendor pricing widget |
| Sigma Estimates | Enterprise | $1,980/user/yr | same |
| STACK Takeoff & Estimate | starting | $249/user/mo (annual) | stackct.com/pricing |
| STACK Full Platform | starting | $298/user/mo | stackct.com/pricing |

No direct paid comparable exists for "a standalone vendor price-list normalizer" as its own
product category — the closest analogs are either bundled features of a full estimating platform
(Buildxact, Handoff) or generic cost-data subscriptions (RSMeans, Sigma, CostOS) serving a
different job (regional average pricing, not a contractor's own negotiated vendor quotes).

## Scoring

- **Evidence of demand: 1/5.** One individually-voiced, independently-verified complaint after an
  exhaustive search of six tools' full review histories plus several (partially blocked) forums.
  Below the project's 3+-voice bar.
- **Willingness to pay: 2/5.** Plausible in principle (a stale price costs real margin), but no
  direct paid comparable found for this specific product category, and the closest analog
  (Buildxact's equivalent capability) is a free, bundled feature rather than something priced
  separately.
- **Buildable to handoff in ≤10 sessions: 4/5.** Scoped to CSV/Excel ingestion (skip scanned-PDF
  OCR for v1), column-mapping UI, and a diff view, this is a plausible walking-skeleton-to-MVP
  build within budget.
- **Reason to exist alongside what already ships: 2/5.** Weakened materially by this session's own
  finding: Buildxact already ships a comparable general importer natively, and Buildern/Handoff
  advertise similar bulk-import capability — the "no general importer exists" premise the idea was
  sourced on does not hold.
- **Low compliance/operational burden: 4/5.** Pricing/catalog data only, no PII or payment data
  handled.

**Total: 13/25** — below the 16 kill threshold. Independently, evidence of demand scored 2 or
below, which is an automatic kill per the routine regardless of total. **Verdict: KILL.**
