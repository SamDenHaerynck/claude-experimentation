# Runbook

How this loop actually fails, and what to do about it. Maintained by the session, not the owner
(see "Self-correction" in `OWNER.md`). Read the index at the start of every session; read the full
entry when the symptom matches.

Append an entry whenever a session hits a failure not already listed. Update an entry when its
recorded fix turns out not to work. Keep entries short: symptom, action, notify-or-not, status.

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
Action: do not burn the session on retries — the exponential-backoff retry policy is for network
errors, and one confirming retry is enough here. Commit all work locally so nothing is lost inside
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
Action: this is the normal case under the merge-every-session workflow. Restart the branch from the
current default branch before doing any work:
`git fetch origin main && git checkout -B <designated-branch> origin/main`. Never add commits on
top of already-merged history, and never try to reuse the merged PR — each session opens a new one.
If the branch carries unmerged commits beyond the merged history, keep them: rebase them onto the
new base rather than discarding them.
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
idea now exceeds 12 sessions and scope cannot be cut, kill the idea per the routine and move on —
sunk sessions are not a reason to continue.
Notify owner: no.

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
