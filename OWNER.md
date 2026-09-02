# Owner instructions

These override the routine instructions (the "Daily App Factory" spec each session is run with)
wherever they conflict. Read this after `STATE.md`, before anything else.

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
2. **Review before merging — always, no exceptions.** Run an independent review pass on the PR
   diff before merging it, using a reviewer subagent (or the `code-review` skill if available in
   the session). The review should check at minimum:
   - Correctness of any code changes, and that documented run/test commands actually work.
   - No live credentials, API keys, or secrets committed (only `.env.example` placeholders).
   - No invented market/user/revenue data presented as fact (routine non-negotiable #4).
   - No deployment, domain purchase, or third-party account creation attempted (non-negotiable #3).
   - General coherence: docs match code, `STATE.md` matches what actually happened.
   - **Whether the diff weakens anything in "Self-correction limits" below. If it does, that is a
     high-severity finding and blocks the merge.**
3. **If the review finds no high-severity issues:** merge the PR into `main` in the same session.
   A brief PR description summarizing the session's work is enough; no need to wait for a human.
4. **If the review finds high-severity issues:** fix them in the same session if time allows, then
   re-review before merging. If they can't be fixed in the time remaining, do NOT merge — leave the
   PR open, record the specific blocking findings under "Notes for owner" in `STATE.md`, and
   resolve them at the start of the next session before doing anything else.
5. **Never merge** a PR that has a merge conflict against `main`, or where the documented
   start/test commands fail. Resolve first, or leave open and flag per point 4.
6. This applies to every session, not only Phase 5 Handoff — small daily doc/plan-only sessions get
   the same merge-and-review treatment as build sessions.

Rationale: this is a single-agent-maintained repository with no other reviewers, so an automated,
adversarial review pass in place of a human reviewer is the safeguard against silently merging a
mistake into `main`. Skipping the review step to save time is not an acceptable tradeoff.

## Open every session with an integrity check

Because each session merges its own work and no human verifies it, start every session by checking
that the repo is actually in the state the last session claimed. Before the normal Open steps:

1. `git fetch origin main`. Confirm `main` contains the previous session's commit(s). If the last
   log file describes work that is not on `main`, that session's merge silently failed — see
   `RUNBOOK.md` entry "Previous session's work never reached main".
2. **Restart your designated branch from `main`** whenever the previous session's PR was merged
   (which is the normal case under this workflow):
   `git fetch origin main && git checkout -B <designated-branch> origin/main`.
   Never stack a new session's commits on already-merged history, and never reuse a merged PR.
3. Check for PRs left open by an earlier session. If one exists, resolving it is the first unit of
   work, ahead of whatever `STATE.md` names as next action.
4. Confirm `STATE.md`'s `Day` and `Last session` match the newest file in `log/`. If they disagree,
   trust the log files (they are written last) and correct `STATE.md`.
5. Read the "Notes for owner" block in `STATE.md` and any unresolved entry in `RUNBOOK.md`. An
   unresolved blocker outranks the recorded next action.

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

Notify with what you have, immediately, rather than after a full diagnosis — then keep working if
work is still possible. Do not notify for routine progress, a normal kill verdict, or a session
that went fine. Also record the same thing under "Notes for owner" in `STATE.md`, because a
notification may go unread for days.

## Self-correction: keep `RUNBOOK.md` alive

`RUNBOOK.md` is yours to maintain. It is the memory of how this loop actually fails and what to do
about it. Two obligations:

1. **Before closing any session in which something went wrong** — a command failed, a step in these
   instructions turned out to be impossible, a subagent produced garbage, the budget blew out, an
   assumption in `PLAN.md` proved false — append or update an entry in `RUNBOOK.md`: symptom, what
   you did, whether it worked, whether to notify the owner next time. One short entry, not an essay.
2. **When a recorded process turns out to be wrong twice**, change it: fix the procedure in
   `RUNBOOK.md`, note the change in `DECISIONS.md` in one line, and use the new version from then
   on. Do not keep following a step you have watched fail. Repeating a known-broken procedure
   because it is written down is a failure, not obedience.

This is the intended self-improvement loop: the process spec you are run with cannot be edited from
inside a session, but the runbook layered on top of it can, and it is read every session.

## Self-correction limits (owner-authored — do not weaken)

You may rewrite `RUNBOOK.md`, `STATE.md`, `BACKLOG.md`, `DECISIONS.md`, logs, and anything under
`ideas/`. You may **not** use that latitude to loosen the following, no matter how much time it
would save or how reasonable it seems mid-session:

- The six non-negotiables in the routine spec, in particular: never deploy, never buy a domain,
  never create third-party accounts, never handle live credentials or real user data; never invent
  market data, pricing, user numbers or citations; never leave `main` unrunnable without saying so.
- Review before every merge to `main` (the section above). Not "when the diff looks small."
- The kill thresholds and scoring bar in Phase 1, and the rule against rescoring an already-killed
  idea.
- The session time budget, and one unit of work per session.
- This section, and the owner-authored sections above it.

If you become convinced one of these rules is wrong, that is exactly the case for "Notes for owner"
plus a notification — propose the change, leave the rule in force, and let the owner decide when
they are back. Do not grant yourself the exception and do not delete the rule you find
inconvenient. An unreviewed agent quietly relaxing its own constraints over two weeks is the single
worst outcome available here, worse than a stalled loop.
