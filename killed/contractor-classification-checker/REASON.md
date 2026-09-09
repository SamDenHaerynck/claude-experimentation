# Killed: Contractor-vs-employee classification risk checker with audit-trail export

Killed in Phase 1 (Validate), day 007, 2026-09-09. Score: 12/25 (below the 16 threshold); both
"evidence of demand" and "willingness to pay" independently scored 2/5, each an automatic kill on
its own per the routine's Phase 1 rule regardless of total.

Full evidence, competitor research, and scoring are in `VALIDATION.md` in this directory. Short
version: the narrow gap this idea targeted — a free tool that is both instant and produces a kept,
dated, exportable record — genuinely does not exist; the free quizzes found (Tax1099, OnPay,
SmallBizHandbook) are all on-screen-only, and the actual free government route (IRS Form SS-8) is
authoritative but slow (6-8 months). No existing paid platform (Gusto, Rippling, Deel, Justworks)
was found bundling this exact documented-memo feature for free either, so this did not repeat the
`ci-migration-rollback-gate` failure mode of losing to an already-recurring free competitor.

It died instead on the idea's own core thesis: that the paid wedge is the retained documentation,
not the classification answer itself. The only three real user posts reachable this session (Avvo,
Proformative, a Quora thread) each asked only "am I classified correctly" — none asked for
documentation or an audit trail. That is the opposite of what the idea needed to clear "reason to
exist," and it came from the only real-user evidence this pass could reach (Reddit, the likely
source of the most relevant grassroots discussion, was not reachable from this environment and is
flagged as a gap, not treated as supporting evidence either way). Doctrinal support exists for
documentation mattering (IRS Section 530 safe-harbor reasonable-basis relief) but that is legal
reasoning, not evidence anyone in the target audience wants or would pay for it. The idea also
scored 2/5 on compliance/operational burden independent of the demand problem: a self-serve worker-
classification tool sits close to unlicensed legal advice, a real liability risk for the operator.

This is the first application of the widened `RUNBOOK.md` check (2026-09-08 update, naming a
free-and-already-recurring competitor as a distinct risk) to an idea built on property (c) rather
than (b). The widened check did not need to fire here — no such competitor was found — but the kill
still happened, on a different failure mode: a plausible-sounding (a)/(b)/(c) property claim that
turned out to be contradicted by the only real user-voice evidence found. See the `RUNBOOK.md`
update this motivated.

Consecutive kills: 5 (dependency-eol-watcher, vendor-security-questionnaire-autofill,
rent-increase-notice-calculator, ci-migration-rollback-gate, contractor-classification-checker).
See `STATE.md` for the next action.
