# Owner instructions

**This file is owner-mandated and read-only to sessions.** It changes only when the owner changes
it: by a commit the owner makes directly, a pull request the owner opens or merges, or a live
instruction from the owner in the current session's transcript, quoted verbatim in that day's log.
A scheduled run has no live instruction by definition, so for a scheduled run this file is
immutable. Do not edit, delete, extend, reorder, reinterpret or add sections. If you think
something here is wrong, propose the change under "Notes for owner" in `STATE.md`, leave the rule
in force, and let the owner decide.

These instructions override the routine spec (the "Daily App Factory" prompt each session is run
with) wherever they conflict. In particular, the Phase 0 and Phase 1 rules below **replace** the
spec's Phase 0 sourcing guidance, its Phase 1 scoring and kill thresholds, and its "three kills in
a row, spend a session on sourcing method" trigger. Read this after `STATE.md`, before anything
else.

**Owner availability:** the owner checks the repo from the GitHub app every few days and may edit
`INBOX.md`, `ideas/<slug>/SIGNALS.md` and this file directly. Treat anything the owner writes in
those files as authoritative the moment you read it.

## Why this file was rewritten (2026-09-23)

After 21 sessions and 14 kills with nothing past Phase 1, the owner reviewed the loop. The
sessions followed the rules correctly; the rules were the problem. Four causes:

1. The old rubric penalised competitors under "reason to exist" while requiring proof of demand.
   Paid competitors are usually the strongest proof of demand and willingness to pay there is, so
   the two requirements cancelled out.
2. Demand was scored on what the agent could reach. Reddit, G2, Upwork and most practitioner forums
   block automated access, so "no evidence found" kept producing auto-kills for tooling reasons.
3. Review pressure only pointed one way: toward finding reasons to kill.
4. Generic pain-point search produced US compliance niches with no owner advantage and no
   distribution.

The goal of this factory is to **build the best idea available and put it in front of real people
quickly**, not to find an idea that desk research can prove in advance. Desk research can rank
ideas and rule out duds. Only real users can validate one.

## Phase 0: Sourcing

Pull candidates from these sources, in this order of priority:

1. **`INBOX.md`**. Ideas the owner drops in. Every inbox idea is screened in the next Phase 0
   session, ahead of anything else, then moved to `BACKLOG.md` or marked screened-out with one line
   of reason.
2. **Owner-fit channels.** The owner is a developer at an IT consultancy, working daily with
   Optimizely CMS/DXP, Optimizely Connect Platform (OCP), Optimizely Opal, and around 20 Azure DevOps
   organisations, for clients in Belgium and the wider EU. Ideas in these areas get owner domain
   knowledge, a real network of potential first users, and a realistic sales path. Look at:
   - Optimizely: the Optimizely Marketplace / add-on listings and their gaps, Optimizely World
     forums and community posts, public idea or feedback boards, OCP apps and Opal tools that do not
     exist yet, GitHub issues on Optimizely/EPiServer open-source repos.
   - Azure DevOps and .NET/TypeScript tooling: Visual Studio Marketplace extension reviews,
     Developer Community feature requests, multi-org and multi-client pain (permissions, pipelines,
     reporting across organisations).
   - EU and Belgian gaps: products that US incumbents serve badly in Dutch or French, EU data
     residency, Peppol e-invoicing, local SMB workflows.
3. **Vendor feedback boards and paid-marketplace reviews** (old Channels A and D, see
   `RUNBOOK-archive.md`). Still valid, now scored with the Phase 1 rubric below.
4. **Generic pain-point web search.** Lowest priority. Avoid US-only regulatory niches unless the
   owner asks for one.

One-time allowance: the first screening session after this rewrite re-screens the 14 ideas in
`killed/` under the new rubric, alongside new candidates. This is an explicit owner exception to the
"never rescore a killed idea" rule, for this one pass only. Re-screened ideas that still fail stay
in `killed/`.

## Phase 1: Tournament, not threshold

Ideas no longer have to clear an absolute bar. They compete, and the best one gets built.

A **tournament round** is at most three sessions, each one unit of work:

1. **Screen.** Score 8 to 12 candidates quickly (about 5 minutes each, no deep research). Apply
   hard disqualifiers only. Write the scored list to `BACKLOG.md`, top 3 marked as the shortlist.
2. **Deep check.** For each shortlisted idea, spend up to about 15 minutes: the closest paid
   competitors and their pricing, the wedge, a check for hard disqualifier H3, and 2 to 5 cited
   sources. Write `ideas/<slug>/VALIDATION.md` for each.
3. **Pick.** The highest-scoring shortlisted idea with no hard disqualifier and a total of **12/25
   or more** wins. Ties go to the higher Owner fit score. The winner moves to Phase 2 (Plan) and
   gets a validation kit (below). The two runners-up stay at the top of `BACKLOG.md` for the next
   round.

If sessions 2 and 3 fit in one session, combine them. If no shortlisted idea reaches 12/25, run one
more round. If the second round also produces no winner, notify the owner.

### Hard disqualifiers (the only automatic kills)

- **H1 Not buildable:** a usable v1 cannot be built in about 10 sessions within the non-negotiables.
- **H2 Not allowed:** it needs capital, a licence, custody of money, regulated advice (legal,
  medical, financial), or real user data just to function.
- **H3 Already free natively:** the platform it plugs into already ships the core deliverable for
  free, on the plans the target users are actually on, confirmed from that platform's own docs or
  changelog.
- **H4 Contrary evidence:** real users were found explicitly saying they do not need or want this.
  Finding nothing is not contrary evidence.

Nothing else kills an idea on its own. A low score just ranks it lower.

### Scoring (1 to 5 each, total out of 25)

- **Demand signal.** Any real signal counts: posts, feature requests with votes, job postings,
  **paying customers of competitors**, marketplace install counts. Score on what was reachable.
  Record unreachable sources in a separate "Unreachable" line; they are neutral and never lower the
  score.
- **Willingness to pay.** Comparable paid products and their prices. Competitor pricing counts in
  favour, not against.
- **Wedge.** Name one specific thing incumbents serve badly: a segment, region or language, price
  point, integration, or workflow. Back it with at least one data point (a complaint, a documented
  missing feature, a pricing gap, a regional gap). Competitors existing is not a negative on its
  own; a market with competitors and a clear wedge scores well.
- **Owner fit.** 5 = inside the owner's daily work (Optimizely, Azure DevOps, EU/Belgian clients).
  3 = adjacent (general .NET/TS dev tooling, EU SMBs). 1 = no connection and no obvious way for the
  owner to reach 10 potential users.
- **Buildability.** How cleanly a useful v1 fits in about 10 sessions.

### Evidence rules

- Never invent data, prices, user numbers or citations. Every factual claim has a real, retrieved
  URL or is written as "no evidence found".
- **Prefer paraphrase plus URL over exact quotes.** Only put text in quotation marks if this session
  fetched the page and saw it. Do not copy a subagent's quoted phrases into repo files unless you
  re-fetched them. This replaces re-verifying every quote one by one.
- When a cited page mentions other products, list them. Competitors named on a source page are
  useful, not embarrassing.
- Do not spend more than one attempt per session on sources with a record of blocking automated
  access (see the list in `RUNBOOK.md`).

## Validation kit (for every tournament winner)

The routine cannot publish or contact anyone. The owner can. When an idea wins a tournament, write
`ideas/<slug>/VALIDATION_KIT.md` using `templates/VALIDATION_KIT.md`, in the same session as the
pick or the first Plan session. Planning and building continue without waiting for results.

The owner records what happened in `ideas/<slug>/SIGNALS.md`. Read it at the start of every session
working on that idea. Owner-reported results are real evidence: strong positive signals can reshape
the plan, and an owner "kill" or "stop" in that file ends the idea. Only the owner's signals, a hard
disqualifier discovered during Plan/Build, or the 12-session cap can kill an idea once it has won a
tournament.

## Merge every session's end state into `main`

The owner wants `main` to always reflect the end state of the most recently completed session,
without manual merging. At the close of every session, after committing and pushing to the
session's designated branch:

1. **Open a pull request** from the session branch into `main`, unless there is nothing new to
   merge.
2. **Independent review before merging, always.** Run the review as a *separate subagent*, not your
   own reading of the diff. Give it the "Self-correction limits" section below verbatim. The review
   checks at minimum:
   - Correctness of any code changes, and that documented run/test commands actually work.
   - No live credentials, API keys or secrets committed (only `.env.example` placeholders).
   - No invented market, user or revenue data presented as fact.
   - No deployment, domain purchase or third-party account creation attempted.
   - Docs match code, and `STATE.md` matches what actually happened.
   - **Both directions of every verdict.** For a pick or a pass, argue why it might be a bad idea.
     For a kill or a low score, argue why it might be a good idea and what wedge was missed. A kill
     that relied on something other than a hard disqualifier, or treated "no evidence found" as
     negative, is a medium finding.
   - **Whether the diff weakens anything in "Self-correction limits" below. If it does, that is a
     high-severity finding and blocks the merge.**
3. **Review depth depends on the diff.**
   - Code changes: full review as above.
   - Docs, research and planning only: one review round. The reviewer spot-checks up to 3 citations
     rather than re-fetching everything. Medium and low findings can be fixed and merged without a
     second review round. Only high-severity findings require a fresh re-review.
4. **Record the review in the day log** before merging: which subagent ran it, its verdict, and
   every finding with its severity, including ones you chose not to fix and why.
5. **No high-severity issues:** merge the PR in the same session.
6. **High-severity issues:** fix them in the same session if time allows, then re-review with a
   fresh subagent before merging. If they cannot be fixed in time, do NOT merge. Leave the PR open,
   record the blocking findings under "Notes for owner" in `STATE.md`, and resolve them first next
   session.
7. **If the review cannot run at all:** do not merge, and do not substitute your own review. Leave
   the PR open, say so in `STATE.md`, and notify the owner if it happens twice running.
8. **Never merge** a PR with a merge conflict against `main`, or where the documented start/test
   commands fail.
9. **Never push, merge or fast-forward anything onto `main` outside a reviewed pull request.** This
   applies to sessions. Commits the owner makes directly, and pull requests the owner opens or
   merges, are owner changes and do not need a session review record.

## Open every session with an integrity check

Keep it short. In order:

1. `git fetch origin main`. Confirm `main` contains the previous session's work. If the last log
   describes work that is not on `main`, see `RUNBOOK.md`. Commits on `main` must come from either
   a session PR with a Review record in its day log, or the owner (Sam Den Haerynck /
   SamDenHaerynck), directly or via a PR the owner opened or merged. Anything else: record it
   under "Notes for owner" and notify.
2. Check for a PR left open by an earlier session, and for unmerged commits on your designated
   branch. If either exists, dealing with it is the first unit of work. Compare by content, not SHA.
3. Once clear, restart the branch: `git fetch origin main && git checkout -B <designated-branch>
   origin/main`. Use `--force-with-lease` only when the branch holds nothing but merged history.
4. Confirm `STATE.md`'s `Day` and `Last session` match the newest file in `log/`. If not, trust the
   log.
5. Read "Notes for owner" in `STATE.md`, `INBOX.md`, and `SIGNALS.md` for the current idea if one
   exists.

**Context budget:** at session start read only `STATE.md`, this file, `RUNBOOK.md`, `INBOX.md`, and
files `STATE.md` names under "Read first". Do not read `RUNBOOK-archive.md`, `DECISIONS.md` or old
logs unless a specific question needs them.

## Escalation

Send the owner a notification (`PushNotification`, when running as a scheduled routine) when, and
only when:

- The loop is blocked by something only a human can fix: revoked GitHub access, a failing push, a
  required account or credential, branch protection preventing the merge.
- You are about to leave `main` in a state where the documented commands do not work.
- A decision exceeds your remit (spending money, signing up for a service, publishing anything,
  handling real user data).
- The same failure has blocked three consecutive sessions.
- The pre-merge review could not run for two sessions running.
- Two tournament rounds in a row produced no winner.
- **A tournament winner has been picked and its validation kit is ready** (so the owner can run it).

Mirror every notification under "Notes for owner" in `STATE.md`. If the blocker is that you cannot
push, the notification is the only durable channel, so make it detailed enough to stand alone.

## Self-correction: keep `RUNBOOK.md` alive

`RUNBOOK.md` is yours to maintain and is the only process file you may rewrite. Keep it **short**:
a checklist of how the loop fails and what to do, under about 150 lines. When an entry is resolved
or superseded, move it to `RUNBOOK-archive.md` instead of growing the main file.

1. When something goes wrong in a session (a command failed, a subagent produced garbage, the
   budget blew out), add or update one short entry: symptom, action, notify or not.
2. When a procedure you wrote in `RUNBOOK.md` proves wrong twice, change it and note the change in
   `DECISIONS.md` in one line.

This applies only to procedures you wrote in `RUNBOOK.md`, never to this file or the routine spec. A
rule here that proves inconvenient stays in force; the response is "Notes for owner".

**Symmetry rule for runbook lessons:** a new lesson may not only add ways to reject ideas. If a
lesson adds a new check that can lower a score, say in the same entry what evidence would count in
the idea's favour on that same check.

## Self-correction limits (do not weaken)

You may rewrite `RUNBOOK.md`, `STATE.md`, `BACKLOG.md`, `DECISIONS.md`, logs, and anything under
`ideas/`. You may **not** use that latitude to change the following, in either direction, no matter
how reasonable it seems mid-session:

- The six non-negotiables in the routine spec, in particular: never deploy, never buy a domain,
  never create third-party accounts, never handle live credentials or real user data; never invent
  market data, pricing, user numbers or citations; never leave `main` unrunnable without saying so.
- **Independent review before every session merge to `main`**, run by a subagent other than the one
  that wrote the diff, given this section verbatim, with its verdict and findings in the day log. If
  the reviewer cannot run, do not merge.
- **No path onto `main` for sessions except a reviewed pull request.**
- The Phase 1 tournament rules in this file: the hard disqualifiers, the scoring dimensions, the
  12/25 floor, and the rule that nothing except a hard disqualifier kills an idea automatically.
  Making them stricter is as much a violation as making them looser.
- The rule against rescoring a killed idea, apart from the one-time re-screen this file grants.
- The session time budget, and one unit of work per session.
- The read-only status of this file.

If you become convinced one of these rules is wrong, propose the change under "Notes for owner",
leave the rule in force, and let the owner decide.
