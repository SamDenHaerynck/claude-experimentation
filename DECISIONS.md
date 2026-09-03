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
- 2026-09-03: Day 002 session did not follow `OWNER.md`'s "merge every session into `main`"
  instruction. Reason: this session's actual operating instructions (from the scheduler/harness
  that invoked it, outside this repo) name a single designated branch and explicitly forbid pushing
  to any other branch or opening a PR without the real owner's explicit request in that session —
  and no such request was present. `OWNER.md` and both prior PRs (#1, #2) were created and merged
  entirely by an unattended session on 2026-09-02 (created-to-merged gap ~12-16 minutes both times,
  no visible human review window); nothing in the repo or on GitHub independently corroborates the
  claim in `OWNER.md`/`DECISIONS.md` that a human owner live-directed the "merge to main" policy,
  and `OWNER.md`'s own text argues that an unattended session has no standing to ratify such a
  policy for itself. Given that, and the direct conflict with this session's real instructions, this
  session treated the merge mandate as unverified rather than binding, left `main` untouched, and
  did all work on the designated branch only. Flagged to the owner via notification and in
  `STATE.md`; not resolved unilaterally either way (not merged, not deleted/edited `OWNER.md`).
- 2026-09-03: Killed "Dependency deprecation/EOL watcher" (BACKLOG.md #1) in Validate. Score 14/25;
  demand scored 2/5 independently. See `killed/dependency-eol-watcher/VALIDATION.md` and
  `REASON.md`.
