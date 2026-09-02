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
- 2026-09-02: Bounded that self-correction with an owner-authored "Self-correction limits" section
  (non-negotiables, review-before-merge, kill thresholds, time budget are not self-editable) and
  made weakening it a high-severity review finding, since no human reviews any merge until approx
  2026-09-16.
