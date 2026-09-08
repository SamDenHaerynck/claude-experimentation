# Backlog

Ranked pool of candidate ideas not yet started. Phase 0 picks the top entry. When an idea is
selected it moves out of this list into `ideas/<slug>/`; when generating replacements, add new
candidates to the bottom unless evidence gathered during Validate suggests otherwise.

Re-ranked 2026-09-07 after the three-kills-in-a-row sourcing-method review (see `DECISIONS.md`).
All three prior kills were one-shot output generators (a lookup, an autofill, a letter draft) fully
replaceable by a free calculator, a free adjacent tool, or a generic LLM prompt. Going forward,
Phase 0/1 selects for at least one of: (a) value from data that changes on an ongoing basis and
must be kept current, (b) value from embedding in a recurring workflow or persisted system state
rather than a single generated artifact, or (c) a cost of being wrong high enough that free/DIY
answers aren't trusted. The three new entries below (originally four; #1 was killed 2026-09-08,
see `killed/ci-migration-rollback-gate/`) were ordered first because each is chosen to test one of
those properties; the two carried-over entries are one-shot generators with the same shape as the
three kills and are demoted to the bottom pending evidence that a real wedge exists.

Updated 2026-09-08: killed candidate #1 (CI-embedded migration/rollback safety gate) in Phase 1 —
see `killed/ci-migration-rollback-gate/REASON.md`. Result widens the (a)/(b)/(c) check itself (see
`RUNBOOK.md`): holding property (b) is necessary but not sufficient if a free tool that is *itself*
already recurring/CI-embedded (not just a one-shot substitute) already serves the same audience —
name that risk explicitly too, not just one-shot LLM/DIY substitutes.

1. **Contractor-vs-employee (worker classification) risk checker with an audit-trail export** — a
   short questionnaire against IRS/DOL common-law-factor tests for a specific hire, producing a
   dated, exportable record of the analysis and reasoning (not just a yes/no) that a small business
   can keep on file if audited. Rationale: tests property (c) — free tests and forum answers already
   exist (expect this to surface in Validate, same as the rent-increase idea), but misclassification
   penalties are large enough that the paid wedge, if any, would be the retained documentation/audit
   trail rather than the computation itself; Validate must find evidence anyone treats an
   undocumented free answer as insufficient before this clears "reason to exist."

2. **State/local sales-tax nexus threshold monitor with change alerts** — tracks a small
   e-commerce seller's revenue/transaction count per state against economic-nexus thresholds that
   change periodically by state law, and alerts before a new filing obligation is triggered.
   Rationale: tests property (a) — thresholds change over time across 40+ jurisdictions, which is
   an ongoing maintenance burden a one-time free calculator doesn't take on; likely paid incumbents
   exist here too (TaxJar/Avalara), so Validate must specifically check whether a narrow, cheap
   alternative has room next to them or whether this repeats the EOL-watcher pattern of the
   category being already bundled into tools the audience already buys.

3. **Freelancer SOW/contract generator with e-sign tracking** — a lightweight, freelancer-specific
   alternative to heavyweight contract platforms: generates scoped statements of work from a short
   intake form, tracks e-signature status, and reminds on renewal/expiry. Rationale: solo
   consultants often use generic templates or expensive all-in-one tools (DocuSign, PandaDoc) built
   for larger teams; a narrow, cheap, fast tool may fill a gap. Demoted: the core deliverable (a
   generated SOW document) is a one-shot artifact of the same shape as the three kills; not
   disqualified outright since the e-sign tracking/renewal reminders piece is arguably property
   (b), but Validate must test whether that piece alone (not the document generation) is what a
   buyer would pay for, given free-template and incumbent-freemium substitutes likely exist.

4. **Git-history-to-changelog generator for indie SaaS** — ingests merged PRs/commits and drafts a
   customer-facing changelog entry or release-notes email, matching a configurable tone/template.
   Rationale: solo/indie SaaS founders ship frequently but often skip customer communication because
   writing a polished changelog entry takes more time than the fix itself. Demoted: this is close to
   a pure one-shot text-generation task, which a general-purpose LLM prompt against pasted commits
   already does for free today — the same substitute that helped kill the questionnaire-autofill
   idea. Validate would need to find a real wedge (e.g. persisted per-repo config/workflow
   automation, property (b)) beyond "nicer prompt" before this clears "reason to exist."
