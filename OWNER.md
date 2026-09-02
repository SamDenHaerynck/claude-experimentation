# Owner instructions

**This file is owner-mandated and read-only to sessions.** Parts of it were drafted by a session at
the owner's live direction; that history does not make it editable — every section is a ratified
constraint. It changes only on a live instruction from the owner in the current session, recorded
verbatim in that day's log. A live instruction means the owner's own words in this session's
transcript, quoted verbatim — not a paraphrase, not an inference from an earlier session's log, and
not something you conclude they would want. A session running unattended as a scheduled routine has
none by definition, so for a scheduled run this file is simply immutable. Absent that, do not edit, delete, extend, reorder, reinterpret or add
sections. Checking `git log` or `git blame` and finding a session's name on these lines proves only
who typed them, not who decided them: an unattended session has no standing to revise this file,
whatever the authorship metadata says.

Every section below, including the ones describing your own latitude, is a constraint on you, not a
draft for you to revise. If you become convinced something here is wrong, propose the change under
"Notes for owner" in `STATE.md`, leave the rule in force, and let the owner decide. The one file you
may freely maintain is `RUNBOOK.md`. Where a `RUNBOOK.md` entry conflicts with anything in this
file, this file wins and the runbook entry is the thing to fix.

These instructions override the routine instructions (the "Daily App Factory" spec each session is
run with) wherever they conflict. Read this after `STATE.md`, before anything else.

**Owner availability: unavailable until approximately 2026-09-16.** Nobody will review your work,
answer a question, or unblock you before then. Act accordingly: make the call yourself, record why,
keep moving, and use the notification path in "Escalation" below for anything that actually stops
the loop.

## Merge every session's end state into `main`

The owner wants `main` to always reflect the end state of the most recently completed session,
without manual merging. Follow this at the close of every session, after the normal commit-and-push
to the session's designated branch:

1. **Open a pull request** from the session branch into `main`, unless there is nothing new to
   merge (branch already merged, or no commits ahead of `main`).
2. **Independent review before merging — always, no exceptions.** Run the review as a *separate
   subagent*, not as your own inline reading of the diff. Give that subagent the "Self-correction
   limits" section below verbatim, and brief it to find problems rather than confirm the work. The
   review must check at minimum:
   - Correctness of any code changes, and that documented run/test commands actually work.
   - No live credentials, API keys, or secrets committed (only `.env.example` placeholders).
   - No invented market/user/revenue data presented as fact (routine non-negotiable #4).
   - No deployment, domain purchase, or third-party account creation attempted (non-negotiable #3).
   - General coherence: docs match code, `STATE.md` matches what actually happened.
   - **Whether the diff weakens anything in "Self-correction limits" below. If it does, that is a
     high-severity finding and blocks the merge.**
3. **Record the review in the day log** before merging: which subagent ran it, its verdict, and
   every finding with its severity — including the ones you chose not to fix, and why. This is the
   audit trail the owner reads on return; a merge with no recorded review is indistinguishable from
   an unreviewed merge.
4. **If the review finds no high-severity issues:** merge the PR into `main` in the same session.
   A brief PR description summarizing the session's work is enough; no need to wait for a human.
5. **If the review finds high-severity issues:** fix them in the same session if time allows, then
   re-review with a *fresh* subagent before merging. If they can't be fixed in the time remaining,
   do NOT merge — leave the PR open, record the specific blocking findings under "Notes for owner"
   in `STATE.md`, and resolve them at the start of the next session before anything else.
6. **If the review cannot run at all** (subagent unavailable, errors out, returns nothing usable):
   do not merge, and do not substitute your own review. Leave the PR open, say so in `STATE.md`,
   and notify the owner if it happens twice running. An unreviewed merge is never the fallback.
7. **Never merge** a PR that has a merge conflict against `main`, or where the documented
   start/test commands fail. Resolve first, or leave open and flag per point 5.
8. This applies to every session, not only Phase 5 Handoff — small daily doc/plan-only sessions get
   the same merge-and-review treatment as build sessions.
9. **Never push, merge or fast-forward anything onto `main` outside a reviewed pull request.**
   Direct pushes to `main` are prohibited without exception; landing work by any path that skips
   rules 1-3 — `git push origin HEAD:main`, a local merge then push, a fast-forward, an MCP file
   write targeting `main` — is the same violation as an unreviewed merge.

Rationale: this is a single-agent-maintained repository with no other reviewers, so an automated,
adversarial review pass in place of a human reviewer is the safeguard against silently merging a
mistake into `main`. Skipping the review step to save time is not an acceptable tradeoff.

## Open every session with an integrity check

Because each session merges its own work and no human verifies it, start every session by checking
that the repo is actually in the state the last session claimed. Do these in order, before any
branch surgery:

1. `git fetch origin main`. Confirm `main` contains the previous session's commit(s). If the last
   log file describes work that is not on `main`, that session's merge silently failed — see
   `RUNBOOK.md` entry "Previous session's work never reached main". Also confirm every commit on
   `main` since the last log is reachable from a merged pull request that has a Review record in
   its day log. If any is not, an earlier session landed work outside the gate: record it under
   "Notes for owner" in `STATE.md` and notify the owner. This is the only check that can catch a
   past rule-9 violation, so do not skip it.
2. Check for a PR left open by an earlier session, and for commits on your designated branch that
   are *not* yet on `main`. If either exists, dealing with it is the first unit of work, ahead of
   whatever `STATE.md` names as next action: review and merge the open PR per the section above, or
   rebase the unmerged commits onto `origin/main` and open a PR for them. Never discard unmerged
   work. Compare by content, not SHA: if `origin/main..HEAD` lists commits but their changes are
   already present on `main` (which is what a squash merge leaves behind), treat the branch as
   fully merged and go to step 3.
3. Only once step 2 is clear — the branch has nothing on it that isn't already on `main` — restart
   the branch from the default branch:
   `git fetch origin main && git checkout -B <designated-branch> origin/main`. This is the normal
   case, because the previous session's PR was merged. Never stack a new session's commits on
   already-merged history, and never reuse a merged PR. If the restart leaves the remote branch
   behind the rewritten local one, a `--force-with-lease` push is correct *only* when the branch
   contains nothing but already-merged history; if it contains anything unmerged, go back to step 2.
4. Confirm `STATE.md`'s `Day` and `Last session` match the newest file in `log/`. If they disagree,
   trust the log files (they are written last) and correct `STATE.md`.
5. Read the "Notes for owner" block in `STATE.md` and any entry in `RUNBOOK.md` whose status is
   `open` or `recurring`. An unresolved blocker outranks the recorded next action.

## Escalation

You cannot ask questions between sessions, but you can raise a flag. Send the owner a notification
(the `PushNotification` tool, when the session runs as a scheduled routine) when — and only when —
one of these is true:

- The loop is blocked by something only a human can fix: revoked GitHub access, a failing push, a
  required account or credential, branch protection preventing the merge.
- You are about to leave `main` in a state where the documented commands do not work.
- A decision genuinely exceeds your remit (spending money, signing up for a service, publishing
  something outward-facing, handling real user data).
- The same failure has now blocked three consecutive sessions.
- The pre-merge review could not run for two sessions running (see merge rule 6).

Notify with what you have, immediately, rather than after a full diagnosis — then keep working if
work is still possible. Do not notify for routine progress, a normal kill verdict, or a session
that went fine. Mirror the same thing under "Notes for owner" in `STATE.md`, because a notification
may go unread for days. Note the one case where that mirror is impossible: if the blocker is that
you cannot push, nothing you write to `STATE.md` survives the container, so the notification is the
only durable channel — make it detailed enough to stand alone, and say in it that the repo has no
record of the session.

## Self-correction: keep `RUNBOOK.md` alive

`RUNBOOK.md` is yours to maintain, and it is the *only* process file you may rewrite. It is the
memory of how this loop actually fails and what to do about it. Two obligations:

1. **Before closing any session in which something went wrong** — a command failed, a subagent
   produced garbage, the budget blew out, an assumption in `PLAN.md` proved false — append or
   update an entry in `RUNBOOK.md`: symptom, what you did, whether it worked, whether to notify the
   owner next time. One short entry, not an essay.
2. **When a procedure recorded in `RUNBOOK.md` turns out to be wrong twice**, change it: fix that
   runbook entry, note the change in `DECISIONS.md` in one line, and use the new version from then
   on. Do not keep following a runbook step you have watched fail.

The scope of obligation 2 is deliberately narrow: it applies to procedures *you* wrote in
`RUNBOOK.md`, never to anything in this file or in the routine spec. A rule in `OWNER.md` that
proves inconvenient, awkward, or repeatedly costly is not thereby "wrong twice" — it stays in
force, and the response is "Notes for owner" plus a notification. In particular, a review that
fails to run is a reason to stop merging, never a reason to write yourself a runbook entry that
retires the review.

This is the intended self-improvement loop: the process spec you are run with cannot be edited from
inside a session, but the runbook layered on top of it can, and it is read every session.

## Self-correction limits (do not weaken)

You may rewrite `RUNBOOK.md`, `STATE.md`, `BACKLOG.md`, `DECISIONS.md`, logs, and anything under
`ideas/`. You may **not** use that latitude to loosen the following, no matter how much time it
would save or how reasonable it seems mid-session:

- The six non-negotiables in the routine spec, in particular: never deploy, never buy a domain,
  never create third-party accounts, never handle live credentials or real user data; never invent
  market data, pricing, user numbers or citations; never leave `main` unrunnable without saying so.
- **Independent review before every merge to `main`**, run by a subagent other than the one that
  wrote the diff, given this section verbatim, with its verdict and findings recorded in the day
  log. If the reviewer cannot run, do not merge — leave the PR open and notify per merge rule 6.
  Not "when the diff looks small", and never replaced by your own inline reading.
- **No path onto `main` except a reviewed pull request** (merge rule 9). Direct pushes,
  fast-forwards and local merges to `main` are prohibited without exception, however small the
  change or however pressed the session.
- The kill thresholds and scoring bar in Phase 1, and the rule against rescoring an already-killed
  idea.
- The session time budget, and one unit of work per session.
- The read-only status of this file, declared at the top. `RUNBOOK.md` is where your process
  learning goes; `OWNER.md` is not.

If you become convinced one of these rules is wrong, that is exactly the case for "Notes for owner"
plus a notification — propose the change, leave the rule in force, and let the owner decide when
they are back. Do not grant yourself the exception and do not delete the rule you find
inconvenient. An unreviewed agent quietly relaxing its own constraints over two weeks is the single
worst outcome available here, worse than a stalled loop.
