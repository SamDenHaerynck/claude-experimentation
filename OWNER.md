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
`ideas/<slug