# Killed: CI-embedded migration/rollback safety gate for small teams

Killed in Phase 1 (Validate), day 006, 2026-09-08. Score: 15/25 (below the 16 threshold); no
individual demand/willingness-to-pay/buildability dimension scored ≤2 on its own.

Full evidence, competitor research, and scoring are in `VALIDATION.md` in this directory. Short
version: real, recurring pain about migration/rollback correctness exists across five separate
open-source ecosystems (kysely, Supabase, drizzle-orm, EF Core, cookiecutter-django) plus one
direct match at enterprise scale (a GitLab internal issue requesting exactly a rollback-procedure
requirement on migration MRs). But the category is already served: Squawk (free, open-source,
~600K downloads/month) covers general migration-safety linting; Atlas Pro ($9/dev/month) recently
paywalled migration linting/safety checks specifically (Oct 2025), showing the general category is
monetizable; and — the decisive risk — Bytebase's Community tier is free for up to 20 users and
already bundles SQL review (200+ rules) plus rollout-policy gating, giving this idea's exact
stated target audience (small teams) equivalent-category value at $0. No first-person evidence was
found of a small team specifically wanting this narrow check badly enough to pay for a standalone
tool, so "reason to exist" and the total score both came in below bar.

This is the first idea validated against the 2026-09-07 (a)/(b)/(c) lens (see `DECISIONS.md`), and
it produced a real, worth-recording result: property (b) — value from being embedded in a
recurring CI workflow rather than a one-shot artifact — genuinely held for this idea, correctly
distinguishing it from the three prior one-shot kills. It still died, because the lens only rules
out one-shot free/DIY/LLM substitutes, not competing free tools that are *themselves* already
recurring and embedded in CI for the same audience (Squawk, Bytebase Community). See the
`RUNBOOK.md` update this motivated.

Consecutive kills: 4 (dependency-eol-watcher, vendor-security-questionnaire-autofill,
rent-increase-notice-calculator, ci-migration-rollback-gate). See `STATE.md` for the next action.
