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
- 2026-09-06: Killed "Rent increase / notice compliance calculator" (backlog #1) in Phase 1 on real
  evidence: genuine first-person confusion exists (three BiggerPockets threads across MI/CA/ME,
  2016-2021, plus an adjacent PEI tribunal case), but the exact proposed feature set — jurisdiction
  cap calculation, notice-period lookup, and letter generation together — is already shipped free by
  at least eight distinct tools (Rentlane, LeaseBase, and six more single-jurisdiction calculators),
  and every forum confusion instance found was resolved for free by a peer in the same thread. Score
  11/25 with willingness-to-pay at 1/5, an automatic kill either way. Full record in
  `killed/rent-increase-notice-calculator/`.
- 2026-09-06: Three backlog ideas now killed consecutively (dependency EOL watcher, vendor security
  questionnaire autofill, rent-increase notice calculator), triggering the routine's rule to spend
  the next session on sourcing method rather than generating a fourth idea the same way. All three
  kills so far have found genuine demand/pain but died on "reason to exist alongside what already
  ships" and/or willingness to pay, specifically because a free substitute (DIY, free adjacent tool,
  or free peer forum answer) already satisfies the exact moment of need. The pattern to examine next
  session: is the idea-generation step (Phase 0) systematically picking problems narrow and
  well-defined enough that a free tool or a knowledgeable peer can already solve them completely,
  and if so, what kind of idea would not have that property (e.g. one requiring ongoing/updated
  data no free tool maintains, one requiring integration/workflow embedding a static calculator
  can't offer, or one where the cost of being wrong is high enough that free/DIY isn't trusted).
- 2026-09-07: Sourcing-method review (triggered by the three-kills-in-a-row rule). Conclusion: all
  three kills were single-purpose output generators — a value lookup, a form autofill, a letter
  draft — where the entire deliverable is a static artifact a free calculator, a free adjacent
  product, or a generic LLM prompt already produces completely in one shot, so there was never a
  reason for the user to keep paying after the first output. Re-examined the two ideas already in
  `BACKLOG.md` (freelancer SOW generator, changelog generator) against that lens and both have the
  same shape: a SOW/contract is a one-shot document a freelancer already gets from a free template
  or Bonsai-style incumbent, and a changelog entry is exactly the kind of text a general-purpose LLM
  already drafts for free from pasted commits — no distinguishing wedge over that substitute is
  apparent without evidence, so neither is promoted to next automatically; both stay in
  `BACKLOG.md` for Phase 1 to test properly, but re-ranked below three new candidates chosen to
  fail this test differently (see `BACKLOG.md`). Decided to select for one of three properties going
  forward, in Phase 0 idea generation and as an explicit first check early in Phase 1, before
  spending the rest of the validation budget on the full five-dimension score: (a) the value comes
  from data that changes on an ongoing basis and must be kept current, not a one-time computation,
  so a free tool would have to maintain it forever to compete; (b) the value comes from being
  embedded in a recurring workflow or another system's state (a running check, a scheduled
  notification, a persisted record), not from a single generated artifact a free tool or an LLM
  prompt can fully replace; (c) the cost of an individual being wrong is high enough (money owed,
  legal exposure, a filed document) that a free/DIY answer is not trusted even when one exists, and
  the paid product's value is the guarantee, not the words. Recorded as a `RUNBOOK.md` entry
  (`Idea keeps landing on a one-shot free-substitutable output`) so this check runs automatically in
  future Phase 1 sessions, not just after a triggered review.
- 2026-09-08: Killed "CI-embedded migration/rollback safety gate for small teams" (backlog #1) in
  Phase 1 on real evidence: recurring migration/rollback pain exists across five ecosystems (kysely,
  Supabase, drizzle-orm, EF Core, cookiecutter-django) plus one direct match at enterprise scale
  (a GitLab internal issue requesting exactly a rollback-procedure requirement), but Bytebase's free
  Community tier (free for up to 20 users) already bundles equivalent-category SQL review + rollout
  gating for this idea's exact target audience, and Atlas Pro ($9/dev/mo) shows the general category
  is monetizable but already claimed. Score 15/25, no single dimension ≤2. Full record in
  `killed/ci-migration-rollback-gate/`.
- 2026-09-08: This is the first idea actually scored against the 2026-09-07 (a)/(b)/(c) lens, and it
  produced a real, worth-recording partial-failure result: property (b) (value from being embedded
  in a recurring CI workflow, not a one-shot artifact) genuinely held, correctly distinguishing this
  idea from the three prior one-shot kills — but it still died, because the free substitutes here
  (Squawk, and decisively Bytebase Community) are not one-shot outputs either; they are themselves
  free, already-recurring, already-CI-embedded tools serving the same audience. Widened the check:
  in Phase 1, after confirming a proposed property (a)/(b)/(c) holds, also explicitly name the
  closest free tool that is *itself* already recurring/embedded (not just a one-shot substitute) for
  the same audience, and treat that as a distinct risk to weigh in "reason to exist," not something
  the (a)/(b)/(c) check alone rules out. Updated the `RUNBOOK.md` entry accordingly rather than
  waiting for a second failure, per that entry's own instruction to revisit after the first real
  Phase 1 application.
- 2026-09-08: Four backlog ideas now killed consecutively. The routine's explicit "three in a row"
  rule already fired and was acted on (2026-09-07 sourcing-method review); no separate rule exists
  for a fourth, so proceeding directly to validating the next backlog entry next session, using the
  widened check above, rather than pausing for another full method review. Flagged under "Notes for
  owner" in `STATE.md` for visibility given the streak length, not as a blocker.
- 2026-09-09: Killed "Contractor-vs-employee classification risk checker with audit-trail export"
  (backlog #1) in Phase 1. Real evidence: the specific free-and-instant-and-exportable gap the idea
  targeted does genuinely not exist (three free quizzes checked are on-screen-only; IRS Form SS-8 is
  free but takes 6-8 months), and no paid platform (Gusto/Rippling/Deel/Justworks) was found bundling
  the exact documented-memo feature either — so this did not repeat the `ci-migration-rollback-gate`
  failure mode of a competing free-recurring tool. It died instead because the only three real
  user-voice posts found (Avvo, Proformative, a Quora thread) each asked only for the classification
  answer, never for documentation — directly contradicting the idea's own property-(c) thesis. Score
  12/25, with both demand and willingness-to-pay independently at 2/5 (automatic kill). Reddit, the
  likely source of the most relevant grassroots discussion, was unreachable from this environment and
  is recorded as a gap, not treated as supporting evidence. Full record in
  `killed/contractor-classification-checker/`.
- 2026-09-09: Widened the Phase 1 check again, in a new `RUNBOOK.md` entry distinct from the
  2026-09-08 one: a claimed (a)/(b)/(c) property must be checked against real user-voice evidence for
  the *specific* differentiator claimed, not accepted on plausible architectural or doctrinal
  reasoning alone (e.g. "penalties are high" is not the same claim as "people want documentation, not
  just an answer," and only the latter needed checking against real posts). This is a different
  failure mode from the 2026-09-08 widening (competing free-recurring tool) and is being tracked
  separately in `RUNBOOK.md` pending a second application.
- 2026-09-09: Five ideas now killed since Phase 1 began; the count of consecutive kills since the
  last sourcing-method review (2026-09-07) is 2, not yet 3, so the routine's "three in a row"
  trigger has not fired. This session's own `RUNBOOK.md` entry initially claimed the trigger
  requires three consecutive kills *without an intervening `RUNBOOK.md` widening* — that qualifier
  does not exist in the routine's rule and was invented; the independent pre-merge review (see day
  007 log) correctly flagged it as an unnecessary and inconsistently-applied gloss that could, if
  reused, let every future kill "reset the clock" and erode the safeguard. Corrected the
  `RUNBOOK.md` entry to the plain reading before merging: the count is simply 2 of 3, and the next
  kill (backlog #1, sales-tax nexus monitor) would make it 3 and fire the trigger without
  qualification.
