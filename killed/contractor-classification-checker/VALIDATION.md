# Validation: Contractor-vs-employee classification risk checker with audit-trail export

Research pass run 2026-09-09 via a research subagent tasked with finding real, cited evidence (no
fabricated figures). Reddit was not reachable from this environment (direct fetch and
domain-filtered search both rejected access) — flagged as a real gap below, not filled with
invented substitutes.

## 1. Existing free substitutes

Three free classification quizzes were found and inspected. **All are on-screen-only; none produce
a kept, dated, exportable record**:

- Tax1099 "Worker Classification Analyzer" — https://www.tax1099.com/irs/classify-your-workers/ —
  Typeform quiz; no export; routes to filing IRS Form SS-8 as the "real" next step.
- OnPay "Worker Classification Checker" — https://onpay.com/tools/worker-classification-checker/ —
  6 yes/no questions, on-screen result only, no export or dated record.
- SmallBizHandbook Employee vs Contractor Quiz —
  https://smallbizhandbook.com/employment-law/employee-vs-contractor — 15-question IRS-factor
  quiz, free, no export.

The actual government route, IRS Form SS-8 (https://www.irs.gov/forms-pubs/about-form-ss-8), is
free but slow — commonly cited at 6-8 months for a determination
(https://www.irs.gov/businesses/small-businesses-self-employed/completing-form-ss-8). California's
DIR ABC-test page (https://www.dir.ca.gov/dlse/faq_independentcontractor.htm) is informational only,
no interactive tool.

**Verdict on this sub-question**: the specific gap (an instant tool that also produces a kept,
dated document) genuinely is not filled by the free substitutes found. This part of the thesis
holds.

## 2. Existing paid/recurring competitors

Gusto, Rippling, Deel, and Justworks (payroll/HR/EOR platforms, $8-$59/person/mo per secondary
comparison sources, not independently confirmed on vendor sites) all message general "compliance,"
but none of the sources found describe a **dated, exportable, reasoning-based classification memo
per hire** as a distinct bundled feature — their compliance claims are about payroll-tax execution
and general alerting, not a documented risk analysis for a specific worker. Adjacent-market
precedent: Trade Insight AI sells exactly this "reasoned exportable memo" pattern in
customs/tariff-classification compliance (https://www.tradeinsightai.com/news/how-to-build-a-defensible-classification-audit-trail),
showing commercial precedent for the pattern, just not confirmed in this specific domain.

**Verdict**: no evidence of a free-and-already-recurring tool occupying this exact wedge for this
audience (unlike the `ci-migration-rollback-gate` kill, where Bytebase Community already did). This
sub-check does not kill the idea on its own.

## 3. Evidence of demand, in the target user's own words — the decisive negative finding

Reddit (r/smallbusiness, r/humanresources, r/Entrepreneur, r/tax) was not reachable from this
environment; this is a real, acknowledged gap, not evidence of absence. Three other real forum/Q&A
posts were found and checked for the specific claim the idea depends on — that people want a *kept
record*, not just an answer:

- Avvo — https://www.avvo.com/legal-answers/am-i-an-employee-or-an-independent-contractor--5414736.html
  — asks only "am I an employee or contractor?" No mention of documentation.
- Proformative — https://www.proformative.com/questions/employee-vs-independent-contractor/ — asks
  for arguments to win an internal classification debate. No mention of documentation.
- Quora (search-indexed; direct fetch returned 403) — a live classification dispute, framed as "is
  this OK / should I file as," again the answer, not documentation.

**All three available real-user posts ask only for the classification answer. None spontaneously
asks for documentation or an audit trail.** This directly contradicts the idea's stated thesis
(`BACKLOG.md` #1: "the paid wedge, if any, would be the retained documentation/audit trail rather
than the computation itself") on the only real-user evidence this pass could reach.

Counter-evidence exists, but it is doctrinal, not demand evidence: IRS Section 530 safe-harbor
relief requires an employer to show a "reasonable basis" for a contractor classification
(https://www.irs.gov/pub/lanoa/pmta_2011-15.pdf;
https://www.journalofaccountancy.com/issues/2012/jul/20125528/), which is a real legal reason
documentation *could* matter — but it comes from IRS doctrine and tax-practitioner commentary, not
from any small business owner asking for it. The same source also surfaces an unresolved conflict
in whether documentation must be contemporaneous (one secondary source says after-the-fact reliance
is generally barred; the Journal of Accountancy piece describes a Tax Court case, *McClellan*,
where after-the-fact evidence was accepted) — not resolved against primary case law in this pass.

## 4. Cost of being wrong

Real, if partly secondary-sourced, evidence that penalties are substantial:

- IRC §3509 unintentional misclassification: $50 per unfiled W-2, 1.5-3% of wages, 20-40% of unpaid
  employee FICA (secondary source: https://ablemkr.com/irs-penalties-misclassifying-workers/;
  primary IRC text not independently pulled this session).
- Willful misclassification: 20% of wages + 100% of FICA, criminal fines to $1,000/worker; IRC
  §7202 willful failure to withhold up to $10,000 and 5 years imprisonment (same secondary source).
- California state penalties: $10,000-$25,000/worker willful, $5,000-$15,000/worker unintentional
  (same secondary source; not verified against CA EDD/DIR primary text).
- Real DOL enforcement example: $446,334 recovered from two Louisiana home-care companies for
  misclassifying 88 workers, announced 2025-01-17
  (https://www.dol.gov/newsroom/releases/whd/whd20250117-0 — direct fetch returned HTTP 403 this
  session; confirmed via a search-index snippet of the same page, not a direct fetch).
- A "~30% of employers have misclassified at least one worker" figure surfaced only as an
  AI-generated search summary with no traceable primary source — **not used** as evidence per
  routine non-negotiable #4.

**Verdict**: the cost-of-being-wrong claim is real. But high cost alone does not establish that the
target user distrusts a free/undocumented answer enough to pay for documentation — see #3.

## 5. Explicit gaps

- Reddit entirely unreachable from this environment (biggest gap; likely where the most relevant
  grassroots discussion lives).
- Deel/Rippling/Gusto/Justworks pricing sourced from third-party comparison articles, not
  vendor sites directly.
- Absence of a bundled "classification risk memo" feature in existing paid platforms is absence of
  evidence, not evidence of absence.
- IRC §3509 and California penalty figures sourced from a compliance blog, not primary legal text.
- The DOL press release could not be directly fetched (403); relies on a search-index snippet.
- The "~30% of employers" prevalence figure could not be traced to a primary source and was
  discarded, not used.

## Scoring (1-5 each)

- **Evidence of demand: 2/5.** Real evidence that people want a classification *answer* exists, but
  every real-user post found asks only for the answer — none supports demand for the specific
  documented/audit-trail deliverable this idea is built around. That is the product's actual
  differentiator, and it has no supporting user-voice evidence found, only doctrinal reasoning.
- **Willingness to pay: 2/5.** No product or forum evidence anyone pays, or would pay, specifically
  for a documented record over a free answer. The nearest comparable (Trade Insight AI) is a
  different compliance domain (customs/tariff), not a confirmed analog for worker classification.
- **Buildable to handoff in 10 sessions or fewer: 4/5.** A questionnaire plus a generated,
  dated PDF/document export is straightforward with no unusual technical risk.
- **Reason to exist alongside what already ships: 2/5.** The narrow gap (instant + exportable) is
  real, but the free official alternative (IRS Form SS-8) is slow yet authoritative and free, and no
  evidence surfaced that the target audience treats an undocumented free quiz as insufficient — the
  demand evidence in #3 argues the opposite.
- **Low compliance/operational burden: 2/5.** The product sits close to legal advice (worker
  classification determinations, state-by-state ABC-test variance) for an unlicensed tool; that is
  a real, non-trivial liability/scope-creep risk for the operator, distinct from and additional to
  the demand problem.

**Total: 12/25.** Below the 16 kill threshold, and both "evidence of demand" and "willingness to
pay" independently score ≤2, which is an automatic kill per the routine's Phase 1 rule regardless of
total.

## Verdict: KILL
