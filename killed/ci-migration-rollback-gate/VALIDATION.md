# Validation: CI-embedded migration/rollback safety gate for small teams

Research run directly (web search + fetch), day 006, 2026-09-08. All URLs below were directly
retrieved by web search or WebFetch, not invented.

## Early check (per the 2026-09-07 sharpened method): closest free substitute and property (b)

Before scoring, named the closest free substitute for the *specific* proposed value (block merge
unless a down/rollback migration is present, with a rollback-breakage diff):

- **Squawk** (https://squawkhq.com/) — fully open-source, free, Postgres-specific migration
  linter, ~600K downloads/month, 32 rules. It catches dangerous SQL patterns (locking DDL, unsafe
  column drops, missing `CONCURRENTLY`, etc.) in CI, but does not check whether a down/rollback
  migration exists or is correct — a different, narrower check than what this idea proposes.
- **Alembic Migration Checker** GitHub Action — checks that the DB's applied Alembic version
  matches the latest migration script, not rollback presence/correctness.
- No free tool was found that specifically checks "does a rollback/down migration exist and what
  would it break," so no exact one-shot or free recurring substitute duplicates this idea's
  narrowest claim.

**Does property (b) hold?** Yes, on its own terms: the proposed value only exists by running on
every future PR against the current migration history, so a single LLM prompt or one-time DIY
script cannot substitute for it — this is a different shape from the three prior one-shot kills.
However (see "Reason to exist" below), property (b) turned out to be necessary but not sufficient:
holding it does not by itself rule out *other* free, already-recurring tools occupying adjacent
ground for the same audience.

## Competitors and adjacent products, with pricing

- **Atlas** (ariga.io) — https://atlasgo.io/pricing — Starter tier free but "limited inspection &
  diffing," no formal linting. **Atlas Pro (Team): from $9/developer/month** plus CI/CD and
  database usage, and this is the tier that includes "migration linting & safety checks" — Atlas
  paywalled its migration linter out of the free tier as of v0.38 (Oct 2025), per
  https://dev.to/mickelsamuel/atlas-paywalled-their-migration-linter-here-are-your-free-alternatives-4god.
  This is direct, recent evidence that a real company judged migration-safety linting valuable
  enough to move behind a paid tier.
- **Bytebase** — https://www.bytebase.com/pricing/ — **Community tier is free** for up to 20 users
  / 10 database instances and already includes "Pre-deployment SQL Review (200+ Rules)" and
  "Rollout Policy (progressive, scheduled)" gating — i.e., free CI-style schema-change gating for
  exactly the small-team audience this idea targets. **Pro tier: $20/user/month** adds SSO/groups/
  support, not core gating. This is the strongest free-substitute risk found: the target audience
  can already get equivalent-category gating for $0 by adopting Bytebase Community.
- **Liquibase** (Secure) — https://www.liquibase.com/pricing — free/OSS "Community" edition exists
  but the pricing page centers Liquibase Secure, which gates "automated rollback," "drift
  detection," and "policy enforcement" behind sales-quoted Starter/Growth/Business/Enterprise
  tiers (Starter/Growth restricted to companies under $1B revenue; still enterprise-sales-driven,
  no self-serve small-team price point found).

## First-person / practitioner evidence

1. GitLab (internal engineering) — https://gitlab.com/gitlab-org/gitlab/issues/32255, "Require
   merge requests that include migrations to specify a rollback procedure" — filed after a
   background migration was merged with no rollback procedure defined. This is the closest direct
   match to the exact proposed check found in this research pass, but the source is a
   large-scale engineering org with its own platform team, not the small-team audience this idea
   targets — the evidence does not confirm the same pain is felt, or paid for, at small-team scale.
   (Direct WebFetch of this URL returned HTTP 429 from GitLab both times it was attempted; the
   quote and description above come from the search engine's indexed summary of the page, not a
   direct fetch — flagging this explicitly rather than treating it as fully confirmed.)
2. kysely (SQL query builder) — https://github.com/kysely-org/kysely/issues/1358 — practitioner
   reports the rollback command "will complete successfully [and] rollback *only* the migrations
   which have down migrations," silently leaving the DB in an inconsistent state. This is a request
   that the migration *library* fail loudly, not a request for a third-party CI gate — adjacent,
   not identical, evidence.
3. Supabase — https://github.com/orgs/supabase/discussions/11263 — developer wants rollback
   capability at all, to resolve migration conflicts between two sibling branches before merge.
   Again a feature request aimed at the platform's own tooling, not a CI-gate product.
4. drizzle-orm — https://github.com/drizzle-team/drizzle-orm/discussions/1339 — same shape as #3,
   a request for rollback capability within the ORM itself.
5. dotnet/efcore — https://github.com/dotnet/efcore/issues/26348 — and cookiecutter-django —
   https://github.com/cookiecutter/cookiecutter-django/issues/5321 — both about detecting *missing*
   migrations (model changes without a generated migration file), a related but distinct problem
   from missing/incorrect *down* migrations, and one already solved for free natively in both
   ecosystems (Django's built-in `makemigrations --check`; EF Core has a documented workaround).

No first-person account was found, in any forum, issue tracker, or review site, of a small team
specifically asking for "a CI check that blocks merge without a down migration" — the one direct
match (GitLab) is enterprise-scale and its own internal tooling request, and it could not be
confirmed to have shipped as a reusable gate; everything else found is adjacent (library-level
rollback correctness, or missing-migration detection, both different problems).

## Scoring (1-5 each)

- **Evidence of demand: 3.** Real, recurring pain about migration/rollback correctness and CI
  validation exists across five separate ecosystems (kysely, Supabase, drizzle, EF Core,
  cookiecutter-django) plus one direct match at enterprise scale (GitLab). But the direct match is
  from a large org's internal platform-tooling request, not from the stated small-team target
  audience, and no small-team practitioner was found asking for this specific gate.
- **Willingness to pay: 3.** Atlas Pro paywalling exactly "migration linting & safety checks" at
  $9/dev/month (Oct 2025) is real, recent, direct evidence that this general category is
  monetizable. Liquibase gating "automated rollback" behind enterprise sales is a second data
  point. But Bytebase Community already gives the stated target audience (small teams, up to 20
  users) equivalent-category gating (SQL review + rollout policy) free, which is a direct,
  specific counter-signal for willingness to pay in this exact segment, not just an absence of
  positive evidence.
- **Buildable to handoff in ≤10 sessions: 3.** Feasible only if v1 is scoped to one migration
  convention/framework (e.g., a naming-convention pairing check for one framework, such as
  Django/Rails-style up/down file pairs) rather than attempting broad multi-framework support
  (Rails, Django, Alembic, Prisma, Flyway, raw SQL), which would not fit the session budget.
- **Reason to exist alongside what already ships: 2.** Three adjacent products already occupy this
  category for the same or overlapping audience: Squawk (free, OSS, general migration-safety
  linting), Atlas Pro ($9/dev/mo, migration linting/safety checks), and — most directly — Bytebase
  Community (free, SQL review + rollout-policy gating for small teams). The specific narrow slice
  proposed (down-migration presence/correctness + rollback-breakage diff) is not identically
  replicated by any single one of them, but it is a thin niche squeezed between three overlapping
  free-and-paid products already serving this exact audience, without demand evidence specific
  enough to confirm a standalone tool is worth adopting alongside (or instead of) them.
- **Low compliance/operational burden: 4.** Static analysis of migration files in a repository; no
  PII or regulated data required to perform the check itself.

**Total: 3 + 3 + 3 + 2 + 4 = 15/25.**

## Verdict: KILL

Total is under the 16 threshold (15/25); no individual dimension among demand/willingness-to-
pay/buildability is ≤2 on its own, so the total-score rule is what triggers the kill, not an
independent single-dimension trigger.

**This is the result flagged in advance in `STATE.md`/`RUNBOOK.md` as worth recording plainly: the
(a)/(b)/(c) lens from 2026-09-07 does not fully discriminate.** Property (b) — value from being
embedded in a recurring CI workflow, not a one-shot artifact — genuinely holds for this idea: no
one-shot LLM prompt or DIY script can substitute for a check that has to run on every future PR.
But holding property (b) was not sufficient, because the free substitutes in this category are not
one-shot outputs either — Squawk and, most directly, Bytebase Community are themselves free,
*already-recurring, already-embedded-in-CI* tools serving the same small-team audience. The
(a)/(b)/(c) lens was designed to rule out one-shot free/DIY/LLM substitutes; it was not designed to
rule out competing free *recurring* open-source or platform tools, and this idea shows that gap
concretely. See the `RUNBOOK.md` update for the adjustment this motivates.
