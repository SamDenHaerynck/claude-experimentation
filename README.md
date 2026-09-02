# claude-experimentation

This repository is an autonomous "app factory." One AI work session runs per day, with no memory
between sessions other than this repository. Each session picks up exactly where the last one left
off by reading `STATE.md`, does one small unit of work, and commits before stopping.

The mission: invent, validate, plan, build, and hand off small software products with a credible
path to revenue. Work proceeds on exactly one idea at a time through a fixed phase machine (Select
→ Validate → Plan → Build → Review → Handoff). Ideas that don't clear the validation bar are killed
early and recorded in `killed/`, with the reasoning kept for future reference. Ideas that reach the
handoff bar move to `shipped/`, complete with deployment, monetization, and handoff docs — the repo
owner takes it from there. Nothing is ever deployed, purchased, or connected to a live account by
the session itself; it only produces code and plans for a human to execute.

## Layout

- `STATE.md` — current idea, current phase, exact next action. Read this first, always.
- `OWNER.md` — owner-authored instructions that override the routine. Read-only to sessions; covers
  the merge-and-review workflow, the start-of-session integrity check, escalation, and the limits
  on what a session may change about its own process.
- `RUNBOOK.md` — session-maintained record of how this loop fails and how to recover. This is the
  self-correction layer: sessions append what went wrong and revise procedures that failed twice.
- `BACKLOG.md` — ranked pool of candidate ideas not yet started.
- `DECISIONS.md` — append-only log of significant choices and why they were made.
- `log/YYYY-MM-DD.md` — one file per session: timing, work done, summary.
- `ideas/<slug>/` — the idea currently in progress (concept, validation, plan, review, code).
- `shipped/<slug>/` — ideas that reached the handoff bar, ready for the owner to deploy.
- `killed/<slug>/` — ideas rejected at any phase, each with a `REASON.md`.

## Ground rules

- No deployments, no purchased domains, no third-party accounts, no live credentials, no real user
  data. The session produces code and plans only.
- No invented market data. Every demand or pricing claim needs a real, retrieved URL, or it's
  recorded as "no evidence found."
- Small daily increments. One phase transition or one build slice per session, committed and
  pushed before stopping.
- Every session's end state lands on `main` through a pull request that an independent review
  subagent has attacked first; the verdict and findings go in that day's log. No high-severity
  finding, no merge.
