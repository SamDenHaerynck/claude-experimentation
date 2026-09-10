# Runbook

How this loop actually fails, and what to do about it. Maintained by the session, not the owner
(see "Self-correction" in `OWNER.md`). Read the index at the start of every session; read the full
entry when the symptom matches.

Append an entry whenever a session hits a failure not already listed. Update an entry when its
recorded fix turns out not to work. Keep entries short: symptom, action, notify-or-not, status.

Two scope notes. First, this file is the *only* process file a session may rewrite, and the
"wrong twice, change it" rule applies only to procedures recorded here — never to `OWNER.md` or to
the routine spec. Where an entry here conflicts with `OWNER.md`, `OWNER.md` wins and the entry is
what gets fixed; no entry here can authorize a merge to `main` that `OWNER.md` forbids. Second, where an entry below restates a rule from the routine spec (which lives
in the scheduled-task prompt, outside this repo), it is marked *[from the spec]*. Those
restatements cannot be verified from inside the repo and are not the source of truth: if one ever
appears to conflict with the spec you were actually run with, the spec wins — fix the entry.

Format:

```
### <short symptom>
First seen: YYYY-MM-DD (day NNN) | Status: <open | resolved | recurring>
Symptom: what you observe.
Action: exact steps to recover.
Notify owner: yes/no, and why.
```

---

### `git push` rejected with 403 "Claude doesn't have GitHub access"
First seen: 2026-09-02 (day 001) | Status: resolved (owner granted access same day)
Symptom: `git push` fails with `403` and a remote message about the Claude GitHub App not having
access to the repository. GitHub MCP *read* calls (`get_me`, `list_branches`) still succeed, and
MCP write calls (`create_branch`, `push_files`) fail with `403 Resource not accessible by
integration`. This is a permissions failure, not a network failure.
Action: do not burn the session on retries — the exponential-backoff push-retry guidance *[from the
spec]* is for network errors, and one confirming retry is enough here. Distinguish the two by the
message: a 403 naming access or integration permissions will not clear on retry. Commit all work
locally so nothing is lost inside
the session, write `STATE.md` and the day log as usual, then notify the owner. Note that the
container is ephemeral: local commits that never reach `origin` are lost, so the next session may
need to redo the work. Record in `STATE.md` exactly what was done, so redoing it is cheap.
Notify owner: yes, immediately. Only a human can restore access (install/grant the Claude GitHub
App, or reconnect GitHub in claude.ai settings). The loop cannot make progress until they do.

### Previous session's work never reached `main`
First seen: not yet observed | Status: open (preventive)
Symptom: the newest file in `log/` describes work that is not present on `origin/main`, or
`STATE.md` refers to files that do not exist.
Action: check for an open PR from the previous session's branch first — if one exists, review and
merge it per `OWNER.md` before anything else. If the branch exists but no PR does, open one,
review, merge. If the branch itself is gone, the work was lost with the container: re-derive it
from the day log, which is the only surviving record, and treat that as the session's unit of work.
Notify owner: only if this happens twice in a row, or if the cause is a permissions/protection
failure rather than a missed step.

### Designated branch already contains merged history
First seen: 2026-09-02 (day 001) | Status: recurring by design
Symptom: the session's designated branch is the same one whose PR was merged in a previous session,
so it already carries commits now on `main`.
Action: this is the normal case under the merge-every-session workflow, but check before you
rewrite anything. First confirm the branch carries nothing that is not already on `origin/main`
(`git log --oneline origin/main..HEAD`) and that no PR from it is still open. Judge by content, not
by SHA: a squash-merged PR leaves the branch listing commits that look unmerged forever, so check
whether their changes are already on `main` (`git diff origin/main...HEAD` empty, or the PR shows
as merged) before concluding there is unmerged work. If it does carry
unmerged commits, keep them — rebase them onto `origin/main` and open a PR — and do not proceed to
the restart. Only when the branch is purely merged history, restart it:
`git fetch origin main && git checkout -B <designated-branch> origin/main`, pushing with
`--force-with-lease` if the remote branch is now behind. Never add commits on top of already-merged
history, and never try to reuse the merged PR — each session opens a new one. A non-fast-forward
push rejection here means the check above was skipped, not that the loop is blocked: re-check for
unmerged commits rather than notifying the owner.
Notify owner: no.

### Review finds high-severity issues and time has run out
First seen: not yet observed | Status: open (preventive)
Symptom: the pre-merge review returns high-severity findings past minute 50.
Action: do not merge and do not skip the review. Leave the PR open, push what you have, list each
blocking finding verbatim under "Notes for owner" in `STATE.md`, and make resolving them the next
session's first unit of work. `main` stays at the last good state, which is the point.
Notify owner: no, unless the same findings survive two sessions.

### Session ended `wip:` or the tree is dirty on arrival
First seen: not yet observed | Status: open (preventive)
Symptom: last commit is prefixed `wip:`, or `git status` is not clean at session start.
Action: reconcile before starting anything new, per the routine's Open step: either finish the
small remainder or revert it cleanly. Decide by whether the remainder fits in well under the
session budget; when in doubt, revert and re-plan the slice smaller in `PLAN.md`. Record which you
chose in the day log.
Notify owner: no.

### The same slice fails to complete two sessions running
First seen: not yet observed | Status: open (preventive)
Symptom: a slice is still incomplete after two sessions of work on it.
Action: the slice is mis-sized, not unlucky. Split it in `PLAN.md` into pieces that each finish in
one session, and record the split in `DECISIONS.md`. If splitting does not help because the
underlying approach is wrong, re-scope the slice or cut the feature. If the projected total for the
idea now exceeds the spec's session cap (12 sessions *[from the spec]*) and scope cannot be cut,
kill the idea and move on — sunk sessions are not a reason to continue.
Notify owner: no.

### Pre-merge review cannot run
First seen: not yet observed | Status: open (preventive)
Symptom: the reviewer subagent errors out, times out, or returns nothing usable, so the PR has no
independent review.
Action: do not merge, and do not substitute your own reading of the diff — `OWNER.md` merge rule 6
makes an unreviewed merge unavailable, and this entry may not be edited to change that. Retry once
with a fresh subagent and a shorter brief. If that also fails, leave the PR open, push the work,
record the situation under "Notes for owner" in `STATE.md`, and make the review the first unit of
work next session. `main` staying one session stale is the acceptable outcome here.
Notify owner: only if it happens two sessions running.

### Time budget blown
First seen: not yet observed | Status: open (preventive)
Symptom: past minute 50 with the unit unfinished, or past minute 60 entirely.
Action: stop working immediately and go to Close. Commit `wip:`, mark the slice incomplete in
`STATE.md`, record exactly what is broken and what the next concrete step is. Overrunning to finish
"just one more thing" is a failure of the routine, not dedication. If the budget blows twice in a
row on the same phase, the units are too big: shrink them in `PLAN.md`.
Notify owner: no.

### Idea keeps landing on a one-shot free-substitutable output
First seen: 2026-09-07 (day 005, triggered by three kills in a row) | Status: open (apply and watch)
Symptom: an idea's entire deliverable is a single generated artifact (a value lookup, a filled
form, a drafted letter/text) — the kind of thing a free calculator, a free adjacent product, or one
generic LLM prompt already produces completely, with nothing left for a paid product to keep
selling after the first output. All three kills so far (`dependency-eol-watcher`,
`vendor-security-questionnaire-autofill`, `rent-increase-notice-calculator`) died this way, scoring
low on "reason to exist" and/or "willingness to pay" specifically because of this.
Action: in Phase 0, prefer ideas with at least one of: (a) value from data that changes on an
ongoing basis and must be kept current; (b) value from being embedded in a recurring workflow or
persisted system state (a running check, a scheduled alert, a kept record), not a single generated
artifact; (c) a cost of being wrong high enough (money owed, legal exposure, a filed document) that
a free/DIY answer isn't trusted even when one exists. In Phase 1, run this as an explicit early
check — name the closest free substitute (a specific free tool, or "a generic LLM prompt would
produce the same output") before scoring the five dimensions — rather than discovering it only
after a full research pass. See `DECISIONS.md` 2026-09-07 for the full analysis and the re-ranked
`BACKLOG.md`.
Update 2026-09-08 (day 006, first real Phase 1 application, `ci-migration-rollback-gate`): the
check partially failed to discriminate, as this entry asked to watch for. Property (b) held
genuinely — the idea's value only exists by running on every future PR, which no one-shot LLM
prompt or DIY script can substitute for — but the idea still died (15/25), because the closest free
substitutes (Squawk, and decisively Bytebase Community's free small-team tier) are not one-shot
outputs either: they are themselves free, already-recurring, already-CI-embedded tools serving the
same audience. The (a)/(b)/(c) check rules out one-shot free/DIY/LLM substitutes; it does not by
itself rule out a competing free tool that is already recurring/embedded for the same audience.
Widened action: after confirming a proposed (a)/(b)/(c) property holds, also explicitly name the
closest free tool that is *itself* already recurring/embedded (not a one-shot substitute) serving
the same audience, and weigh that as a distinct "reason to exist" risk rather than treating a held
(a)/(b)/(c) property as sufficient on its own.
Notify owner: no. This is one data point on a widened check, not yet re-tested. Revisit again once
the widened version has been applied to a second new idea — if it still doesn't discriminate, or a
demoted idea turns out fine, record that and adjust again rather than trusting this version
untested either.
Update 2026-09-10 (day 008, second application, `sales-tax-nexus-monitor`): the check discriminated
correctly this time. Property (a) held genuinely (thresholds vary by state and change over time),
but two free-and-already-embedded incumbents were found and named before scoring: Shopify's
built-in "Tax liability insights" dashboard (free, passive — a real nuance: no push alert, dashboard
only) and Stripe Tax's built-in threshold monitoring (free active email/dashboard alerts for any
Stripe-processing seller). A third, closer-still finding beyond the check's original scope: a
dedicated small-SaaS competitor, NexusMonitor, already ships on the Shopify App Store at $19-69/mo
doing essentially the same product. This version of the check is now confirmed useful on two
different ideas (one where it found a genuine near-clone, one where it didn't need to fire because
no such competitor existed) — keep using it as written, no further widening needed from this data
point.
Notify owner: no.

### Web research returns no usable evidence
First seen: not yet observed | Status: open (preventive)
Symptom: during Validate, no forum posts, issue threads, review-site complaints or practitioner
writeups describing the problem in the target user's own words; or no competitor with a published
price.
Action: write "no evidence found" in `VALIDATION.md` and treat it as a real signal — it scores
demand low, and a low demand score is a kill. Do not substitute SEO listicles, roundup pages or
vendor marketing copy for evidence of demand, and never fabricate a citation or a number to fill
the gap. If three ideas in a row die this way, the next session goes to sourcing method rather than
a fourth idea, per the routine.
Notify owner: no.

### A claimed (a)/(b)/(c) property is plausible but contradicted by real user-voice evidence
First seen: 2026-09-09 (day 007, `contractor-classification-checker`) | Status: open (apply and
watch)
Symptom: the backlog entry's rationale argues a property ((a), (b), or (c)) holds using plausible
architectural or doctrinal reasoning, but when Validate actually finds real first-person posts from
the target user, they contradict the specific differentiator the idea depends on. Here: the
rationale claimed the paid wedge would be "retained documentation/audit trail, not the computation,"
justified by high misclassification penalties (property (c)) and IRS Section 530 safe-harbor
doctrine — real and high, but doctrinal, not a demand signal. The only three real user posts found
(Avvo, Proformative, a Quora thread) each asked only "am I classified correctly," never for
documentation. Unlike the `ci-migration-rollback-gate` kill (property held, lost to a competing free
tool), this idea's claimed property itself was not supported by the user-voice evidence found.
Action: in Phase 1, do not treat a property (a)/(b)/(c) claim as confirmed on architectural or
doctrinal reasoning alone. After finding real user posts (the three-posts-with-URLs requirement
already in Phase 1), explicitly check whether any of them evidence the *specific* differentiator the
idea depends on (e.g. "wants a kept record," not just "wants an answer") — not merely the general
problem. If the found posts are silent or point the other way, score demand low even if the
property's architectural logic is sound. If real user-voice evidence for the specific
differentiator is unreachable (e.g. a platform is blocked from this environment, as Reddit was this
session), say so explicitly and do not treat that gap as neutral — a claimed differentiator with no
supporting user-voice evidence found does not clear "reason to exist" on doctrine alone.
Notify owner: no. One data point; revisit after a second idea is validated against this addition —
if it still doesn't discriminate well, or unfairly demotes an idea that turns out to have real
demand via a channel this pass couldn't reach (e.g. Reddit), record that and adjust again.
Update 2026-09-10 (day 008, second application, `sales-tax-nexus-monitor`): confirmed useful again.
Three real first-person posts were found (all Shopify Community); each asked a one-time factual
question about how a threshold or registration rule works, none asked for ongoing monitoring or
alerts. Scored demand low on that basis, consistent with the check as written. Two-for-two now —
keep using this check as written.

### Six ideas killed total; second three-in-a-row trigger has now fired
First seen: 2026-09-09 (day 007) | Status: closed 2026-09-10 (day 008) — action taken, see below
Symptom: after the 2026-09-07 sourcing-method review (triggered by three consecutive kills:
`dependency-eol-watcher`, `vendor-security-questionnaire-autofill`, `rent-increase-notice-calculator`),
three more ideas were killed in a row (`ci-migration-rollback-gate`, `contractor-classification-checker`,
`sales-tax-nexus-monitor`), bringing the running total to six and the count since the last review to
three. The routine's "three in a row" trigger is a plain consecutive-kill count with no exception,
and it has now fired a second time.
Action taken: per the routine ("if three ideas in a row are killed, do not generate a fourth the
same way; spend the next session on your sourcing method instead"), day 009 must not validate a new
backlog candidate. It must instead review sourcing method: which evidence sources were used across
this second three-kill run, which score dimension keeps failing (this run: "evidence of demand" and
"reason to exist" both failed on all three — the last two are architecturally-plausible ideas
undercut by either a real near-clone competitor or user-voice evidence pointing the other way, not
lack of a well-defined problem), and what kind of idea would actually pass. See `STATE.md` for the
specific next action.
Notify owner: no — this is a normal-if-notable kill streak, not a blocked loop, per `OWNER.md`
escalation criteria.

### Reddit unreachable from this environment, second session running
First seen: 2026-09-09 (day 007) | Status: open (recurring, watch)
Symptom: both direct `WebFetch` to reddit.com and `WebSearch` with `site:reddit.com` fail to return
any reddit.com content, across two separate sessions now (2026-09-09, and again 2026-09-10, tried
twice with different query patterns within the same session). This looks like a standing
environment/tooling restriction rather than a one-off transient failure, and it is the most likely
place for grassroots small-business/seller discussion to live, which may be systematically
understating "evidence of demand" scores across every Phase 1 pass.
Action: keep treating the gap honestly in `VALIDATION.md` ("no evidence found" from this source, not
filled with invented posts) — this has not changed and should not change. Flagged under "Notes for
owner" again this session. If a third Phase 1 session hits the identical wall, that meets
`OWNER.md`'s escalation bar ("the same failure has now blocked three consecutive sessions") and
should trigger a `PushNotification`, not just a `STATE.md` note.
Notify owner: not yet (this is the second occurrence, escalation bar is a third) — flagged under
"Notes for owner" per the recurring-symptom pattern.
