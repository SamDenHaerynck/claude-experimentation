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
