# Decisions

Append-only log of significant choices and why they were made. One line each, newest last.

- 2026-09-02: Bootstrapped repo layout per routine instructions on day 1 instead of starting
  validation, per the explicit bootstrapping rule (first session creates structure only).
- 2026-09-02: Seeded BACKLOG.md with five candidate ideas skewed toward narrow professional tools
  (dev tooling, small-landlord compliance, freelancer ops, indie-SaaS ops) rather than broad
  consumer apps, per the Phase 0 preference stated in the routine instructions.
- 2026-09-02: Owner asked for every session's end state to land on `main`; added OWNER.md rule to
  open a PR each session, gate it on an automated adversarial review, and merge only with no
  high-severity findings — chosen over pushing directly to `main` so there is always one review gate.
- 2026-09-02: Added RUNBOOK.md as the self-correction layer, because the routine spec itself lives
  outside the repo and cannot be edited from inside a session; the runbook can, and OWNER.md makes
  reading and updating it mandatory.
- 2026-09-02: Bounded that self-correction with an owner-mandated "Self-correction limits" section
  (non-negotiables, review-before-merge, kill thresholds, time budget are not self-editable) and
  made weakening it a high-severity review finding, since no human reviews any merge until approx
  2026-09-16.
- 2026-09-04: Killed "Dependency deprecation/EOL watcher" (backlog #1) in Phase 1 on real evidence:
  the core EOL data is already a free public good (`endoflife.date`), the paying SCA-tool audience
  already gets deprecated-package flagging bundled in (Snyk, Mend/Renovate, FOSSA, Socket.dev), and
  GitLab is already building EOL data into its own paid dependency dashboard as a bundled feature.
  Score 15/25 with willingness-to-pay at 2/5, an automatic kill either way. Full record in
  `killed/dependency-eol-watcher/`.
- 2026-09-05: Killed "Vendor security questionnaire autofill assistant" (backlog #1) in Phase 1 on
  real evidence: demand and willingness to pay are both genuine (verified pricing $250/mo-$9,600/yr
  across Conveyor, 1up.ai, AutoRFP.ai), but the category is already saturated — a YC-backed
  competitor (Stacksi) targets the exact same small-vendor gap claiming 90%+ autofill, several
  more well-funded AI-native entrants already sell this, and a free DIY substitute (spreadsheet +
  general-purpose LLM) is already reported as good enough by a practitioner in the target segment.
  Score 15/25 with "reason to exist" at 1/5. Full record in
  `killed/vendor-security-questionnaire-autofill/`.
- 2026-09-05: Two backlog ideas now killed consecutively (dependency EOL watcher, vendor security
  questionnaire autofill), both partly on differentiation grounds (crowded incumbent category) more
  than pure lack of demand. Noting the pattern rather than acting on it yet — the routine's
  "sourcing method" review only triggers at three kills in a row, and three candidates remain in
  `BACKLOG.md`, so no new idea generation was needed this session.
