# Owner instructions

These override the routine instructions (the "Daily App Factory" spec each session is run with)
wherever they conflict. Read this after `STATE.md`, before anything else.

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
