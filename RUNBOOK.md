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

### Reddit unreachable from this environment, third Validate session running
First seen: 2026-09-09 (day 007) | Status: open (escalated 2026-09-12, still watch)
Symptom: both direct `WebFetch` to reddit.com (and old.reddit.com, and the reddit `.json` search
endpoint) and `WebSearch` with `site:reddit.com` fail to return any reddit.com content, across three
Phase-1 Validate sessions now (2026-09-09, 2026-09-10, 2026-09-12 — day 009 was a sourcing-method
review and did not test it). This looks like a standing environment/tooling restriction rather than
a one-off transient failure, and it is the most likely place for grassroots small-business/seller
discussion to live, which may be systematically understating "evidence of demand" scores across
every Phase 1 pass.
Action: keep treating the gap honestly in `VALIDATION.md` ("no evidence found" from this source, not
filled with invented posts) — this has not changed and should not change. This is the third
occurrence, meeting `OWNER.md`'s escalation bar ("the same failure has now blocked three consecutive
[Validate] sessions"); sent a `PushNotification` on 2026-09-12 (day 010) rather than only a
`STATE.md` note. Keep testing it each Validate session and keep using non-Reddit sources (forums,
industry-specific communities, Substacks) as the fallback, per the day-010 pass which found usable
non-Reddit evidence (Practical Machinist, an independent Substack) even without Reddit access.
Notify owner: done 2026-09-12 (day 010). If it keeps recurring, no further notification is needed
per session — the owner has been told it's a standing limitation — but keep noting each occurrence
here and in `VALIDATION.md`.
Update 2026-09-13 (day 011, `co-owned-vacation-property`): unreachable a fourth consecutive
Validate session. Same failure mode — direct `WebFetch` to reddit.com/old.reddit.com fails outright;
`site:reddit.com` `WebSearch` queries execute but surface no on-topic threads. No new notification
per the prior update's guidance (already told once, standing limitation). Non-Reddit fallback
sources (this session: Bogleheads, via `WebSearch` snippets since direct fetch was Cloudflare-
blocked) again found at least one usable real post, consistent with day 010.
Update 2026-09-14 (day 012, Phase 0 backlog-replenishment sourcing pass, not a Phase 1 Validate
session): unreachable a fifth time, same failure mode (site: search and direct fetch both failed).
A second source also failed this pass: eng-tips.com returned a 403 on direct fetch. The delegated
research subagent substituted other practitioner forums (Mike Holt, ElectricianTalk), vendor/G2/
Capterra pages, and industry blogs/news, and still returned usable real-URL evidence for 3 of 4
candidates — consistent with the standing workaround. No new notification (same standing
limitation already escalated 2026-09-12; this occurrence is Phase 0 sourcing, outside the Phase-1-
Validate context the original escalation was scoped to, but recorded here for the same tracking
purpose).
Update 2026-09-20 (day 018, `vendor-price-list-normalizer` demand re-verification): Mike Holt's
Forum now also returns 403 on direct fetch (previously used successfully as a substitute in day
012). Two more construction-trades forums newly found blocked this session: ElectricianTalk and
ContractorTalk both now redirect every thread URL to a `tollbit.*` gateway returning HTTP 402
Payment Required (a paywall-for-bots service), and JLC Online's old forum thread URLs now 301-
redirect to the plain homepage — the forum content itself appears to no longer be hosted there.
G2 also 403'd on all three products checked (STACK, PlanSwift, Sage Estimating), consistent with
prior sessions. Net effect: for this specific vertical (construction/trades), essentially the
entire practitioner-forum ecosystem this project has relied on as a Reddit substitute (Mike Holt,
ElectricianTalk, ContractorTalk, JLC, plus Reddit itself) was unreachable in the same session,
leaving only vendor review sites (Capterra, Software Advice, GetApp, Trustpilot) as reachable
user-voice sources. The subagent still did a thorough, honestly-reported search of those review
sites (~500+ reviews across six tools) rather than treating the forum blockage as an excuse to
stop early, and correctly reported "no evidence found" for the blocked sources rather than
guessing. Action: for construction/trades verticals specifically, budget for review-site-only
user-voice search (Capterra/G2/Software Advice/GetApp/Trustpilot, accepting G2 will likely 403) and
treat the entire forum layer as probably unreachable rather than attempting it fresh each time;
still worth one attempt per session in case a specific forum's blocking is intermittent, but do not
let its failure alone read as reduced diligence. No new owner notification — this extends an
already-escalated standing limitation (Reddit, 2026-09-12) to more sources in the same vertical,
not a new distinct failure needing its own escalation, and Capterra/Software Advice remained fully
reachable so the search was not blocked outright, only narrowed.

### Sourcing method: the "monitor/track/alert" idea shape is structurally oversaturated
First seen: 2026-09-11 (day 009, sourcing-method review triggered by the second three-in-a-row
kill) | Status: open (apply and watch)
Symptom: this session tested five candidate ideas across five unrelated verticals — notary
commission/journal renewal tracking, OSHA 300 injury-log recordkeeping, professional continuing-
education credit tracking, co-owned vacation-property scheduling/expense-splitting, and small-
manufacturer RFQ quote comparison — using only a handful of quick `WebSearch` calls each. Every
single one already has at least one named, findable, free-or-cheap dedicated competitor: Remindax
(notary renewal/bond/E&O alerts), a free SmarterRisk PWA plus OSHA's own free forms (OSHA 300 log),
cetracker.app (free, 50-state CE tracking), OurSharedPlace ($99/property/yr) and PlumConnect
(co-owned property calendar + bank integration), and AuraVMS/QuotesFlow/QuoteWerks/Quotable AI
(RFQ comparison, one from $4.99/mo). Combined with the two ideas actually killed in Phase 1 this
same three-kill cycle for the identical reason (`ci-migration-rollback-gate` vs. Bytebase
Community; `sales-tax-nexus-monitor` vs. NexusMonitor/Shopify/Stripe), that is 7 for 7: every
"recurring monitor, tracker, deadline-alert, or quote-comparison tool for a well-defined
professional requirement" idea checked so far, in any vertical, already has a close incumbent
findable within minutes. This is a stronger and more general claim than the existing "one-shot
free-substitutable output" entry above (that one is about single-artifact generation; this one is
about an entire *recurring-tool* archetype being colonized regardless of audience).
Action: in Phase 0, deprioritize the "track/monitor/alert/compare for a defined deadline or
requirement" idea shape specifically for the next several candidates — not a permanent ban (this
is one review's worth of evidence, not proof no exception exists), but a specific, falsifiable
claim to keep testing. Prefer shapes that need either (i) multi-party coordination/trust
infrastructure the incumbents above don't provide (a single-user dashboard is easy to clone; a
system two or more real-world parties must both trust and use is harder), or (ii) genuine
domain-specific data-processing or calculation complexity, not a threshold check. Separately: per
the routine's own Phase 1 wording, the bar is "a reason to exist *alongside* what already ships,"
not "zero competitors exist" — so a found competitor should trigger one more targeted search for
real, specific complaints about *that named competitor* (missing feature, price, support) before
scoring "reason to exist" low, rather than treating bare existence as an automatic kill. Tried this
refinement twice this session (searching for complaints about OurSharedPlace/PlumConnect and about
notary journal apps, and separately checking QuoteWerks' G2 review breakdown) and got almost
nothing usable back — mostly unrelated noise or, in QuoteWerks' case, a well-reviewed product with
no 1-star complaints at all in the summarized results. Treat that outcome the same way the
existing "web research returns no usable evidence" entry treats a demand-evidence dead end: as
inconclusive, not as proof the incumbent has no gap — a deeper pass (fetching actual review pages
rather than summarized search results) may be needed before concluding either way, and Phase 1
should budget time for that if it wants this refinement to actually work rather than rubber-stamp
"no gap found."
Notify owner: no. One session's evidence across five quick checks; revisit after Phase 1 actually
applies the multi-party/domain-complexity preference and the competitor-complaint-search
refinement to a real candidate, and record whether either changes the outcome.
Update 2026-09-12 (day 010, first real Phase 1 application, `rfq-quote-comparison`): mixed result.
The multi-party-shape preference did not by itself prevent a kill — this idea was multi-party by
construction (buyer plus several suppliers) and still died, because the competitor-complaint-search
refinement, run properly this time (an actual Capterra fetch, not a search summary), did find a real
complaint about QuoteWerks, but the underserved-sub-segment search came back negative and instead
surfaced two *more* dedicated competitors (Jiga, SmartBid/Buildr) than the four already known — one
in each of the idea's two most obvious target verticals. Takeaway: multi-party shape and a genuine
competitor complaint are necessary but not sufficient; a sub-segment search can still fail to find a
gap and instead reveal the field is denser than last known. Keep running the full search (both the
review-page-complaint check and the sub-segment search) rather than stopping once one comes back
positive.
Notify owner: no.
Update 2026-09-13 (day 011, second real Phase 1 application, `co-owned-vacation-property`): the
multi-party-shape preference failed to prevent a kill again, more decisively this time. This idea
was multi-party by construction (a persisted shared calendar/ledger multiple co-owners continuously
rely on) and had already survived one shallow pass without being killed on bare competitor
existence. The deep review-page complaint check and the sub-segment search (delegated to a research
subagent this session) both came back negative: the two disclosed incumbents (OurSharedPlace,
PlumConnect) turned out to have essentially no independent review footprint at all (too small/new
to have G2/Capterra listings or meaningful App Store review volume), so there was no complaint
trail to find a wedge in either way — and the sub-segment search surfaced five *more* dedicated
competitors (CabinPals, SharedKey, Shared Holiday Homes, CalDibs, House Matters), each already
covering the one narrow gap found (price, international/cross-currency). Two-for-two now: in both
real applications, "multi-party by construction" has not stopped the sub-segment search from
surfacing a denser field than initially known. Widened takeaway: "multi-party coordination" is not
by itself a defensible moat — a shared-state app pattern (calendar + ledger, buyer + suppliers,
etc.) is exactly as copyable as a single-user dashboard once the pattern is well-understood; what
actually needs checking is whether *this specific* multi-party niche already has a copy, not
whether the general shape is multi-party.
Action: stop treating "multi-party coordination/persisted shared state" as a preferred shape that
lowers the bar in Phase 0 — it does not appear to correlate with an open gap any better than any
other shape did. Revert to no shape-based prior at all; run the full competitor/sub-segment/user-
voice search on whatever idea is selected regardless of its shape, and let the evidence (not the
shape) decide. Do not add a new preferred shape without at least one candidate that actually
survives Phase 1 under it — two colonized shapes in a row (monitor/tracker/alert, and now
multi-party coordination) with zero survivals is enough to stop trusting shape-based priors
generally, not just this one.
Notify owner: no — consistent with the routine's own framing, a kill (even a second one testing the
same shape preference) is a normal Phase 1 outcome to record here, not a blocked loop.
Update 2026-09-15 (day 013, third real Phase 1 application under "no shape-based prior",
`customs-hts-microseller`): third kill in a row since this review (day 010, day 011, day 013 — day
012 replenished only, no Validate, so it doesn't count toward or reset the streak). This trips the
routine's own three-in-a-row rule: "if three ideas in a row are killed, do not generate a fourth the
same way... spend the next session on your sourcing method instead." Unlike the prior two kills,
this one didn't die on competitor density alone — see the new entry below on platform-native
closure, which is the more novel and more generalizable finding from this session.
Notify owner: no — a third kill is a normal (if costly) Phase 1 outcome the routine explicitly
anticipates and has a built-in response for (spend day 014 on sourcing method). Escalation is for a
blocked loop, and this loop has a defined next action, not a stall.

### Regulatory-trigger ideas: check whether the dominant platform is closing the gap natively
First seen: 2026-09-15 (day 013, `customs-hts-microseller`) | Status: open (apply and watch)
Symptom: this idea's whole premise was a regulatory change (the 2025-08-29 US de minimis repeal)
creating new demand a third-party tool could serve. The regulatory trigger and the resulting user
pain were both real and well-evidenced (5 dated forum posts, 4 independently fetched). But the
underserved-sub-segment search found that Etsy — the platform hosting the largest share of the
idea's named target audience — is itself already building the fix natively: a Zonos-powered tariff
calculator in its listing flow (live since 2026-06) and a mandatory seller-side duty-prepay
requirement (live since 2026-07-09), both predating this validation session. A regulatory change
creates a genuine, evidenced need, but it also creates the same incentive for the dominant platform
in the space to close the gap in-house for free, since platforms already have the checkout/listing
surface and a direct incentive to reduce buyer-side friction (unexpected customs fees suppress
conversion). The generic-monitor/tracker colonization finding (see the entry above) was about
third-party competitors already existing; this is a distinct and probably stronger risk specific to
regulatory-trigger ideas — the most dangerous competitor may not be another startup but the
platform itself.
Action: when a candidate's core rationale is "a regulatory/platform change created new demand,"
add an explicit check to Phase 1 before scoring "reason to exist": search for the dominant
platform(s) serving the target audience's own product announcements/blog/changelog/community-forum
staff replies from the period after the regulatory change, specifically for signs they are building
or have already shipped a native fix. A platform closing the gap for its own users for free is a
harder kill signal than a third-party competitor, because it comes bundled with a tool the user
already has open and pays nothing extra for.
Notify owner: no — one occurrence; revisit if this recurs on a future regulatory-trigger candidate
to see whether the check generalizes or was specific to this case (Etsy's checkout-integration
incentive may not apply to every platform/regulation combination).

### Delegated research subagents can fabricate exact-quote citations attached to real URLs
First seen: 2026-09-14 (day 012, Phase 0 backlog-replenishment sourcing pass) | Status: open (apply
and watch)
Symptom: a research subagent sourcing new backlog candidates reported several claims as exact
quotes from real, retrievable URLs (shipbob.com, blog.inymbus.com, softwareadvice.com). The URLs
themselves were real and topically relevant, but the independent pre-merge review subagent
re-fetched them directly and found the specific quoted phrases ("may not have a US 10-digit HTS
code documented for your products"; "cargo losses exceed $50 billion annually"; "over-complicated
and extremely frustrating"; "enterprise shippers processing thousands of repetitive claims") did
not actually appear on those pages. This session independently re-fetched the same three URLs and
confirmed the review's finding — the quotes were invented, not paraphrased or slightly misquoted.
This is a more dangerous failure mode than a dead/unreachable source (which is self-evident and
gets flagged as "no evidence found"): a fabricated quote attached to a real, live URL looks exactly
like real sourcing until someone re-fetches the page, and it directly violates the routine's
non-negotiable #4 ("never invent... citations").
Action: treat any *exact quoted phrase* a research subagent attributes to a URL as unverified until
independently re-fetched — do not copy a subagent's quoted claims into `BACKLOG.md`/`VALIDATION.md`
verbatim on trust, even when the URL itself is real and topically on point. Either re-fetch the
specific cited pages yourself before writing them into a repo file as sourced evidence, or rely on
the independent pre-merge review to catch it before merge (as happened here) and fix before merging
rather than after. In this occurrence: two of four sourced candidates had fabricated quotes; one
(freight-claim packet generator) was dropped entirely rather than repaired, since 2 of its 3
citations were fabricated and there wasn't budget left to re-source it properly this session; the
other (customs/HTS classification) was fixed by re-verifying the real citations, replacing the
fabricated shipbob.com quote with an accurate paraphrase of what the page actually says, and adding
a second, primary-source citation (the White House executive order itself) for the regulatory-
change claim rather than relying on one blog's paraphrase of it.
Notify owner: no — caught before merge by the existing review gate working exactly as designed; no
loop-blocking occurred. Revisit if this recurs on a future sourcing pass (a second occurrence would
suggest the fix above — manual re-fetch discipline — isn't sufficient on its own and something
stronger, like requiring the sourcing subagent to self-verify its own quotes before reporting back,
is needed).
Update 2026-09-14 (same session, round 2 review): the round-1 fix was itself incomplete. Only the
one candidate flagged by round 1 (freight-claim) was dropped and the other flagged candidate
(customs/HTS) was fixed, but the *third* candidate (grant-report normalizer) had been marked
"checked out clean on independent re-verification" without actually re-checking each of its two
quotes individually — one ("every funder has their own reporting guidelines... which all require
customization") was itself fabricated and survived into the round-2 diff, caught only because the
round-2 reviewer re-fetched it again from scratch rather than trusting the round-1 "clean" label.
Sharper action: when a fabricated quote is found anywhere in a sourcing subagent's output, do not
trust *any* of that subagent's quotes as "clean" based on spot-checking only the ones a reviewer
happened to flag — independently re-fetch and verify every single quoted phrase in every surviving
candidate, one at a time, before calling any of them verified. A "some checked out" review result
is not the same as "all checked out"; treat every remaining quote as guilty until re-fetched.
Notify owner: no — still caught before merge, this time by round 2 rather than round 1. If a third
round finds yet another fabricated quote, that meets a different bar (the review process itself
repeatedly failing to fully catch this) and should be flagged to the owner rather than just fixed
again.

### Sourcing method review after 9 consecutive real-research kills: scoring bar is not the problem, sourcing channel is
First seen: 2026-09-16 (day 014, third three-in-a-row trigger, and a direct owner instruction added
to `OWNER.md`'s new "Self-optimizing" section) | Status: open (apply and watch)
Symptom: 9 ideas killed in a row across 13 build days, every one sourced via generic web search
(searching for a profession's documented pain point, a regulatory change, or a workflow people do
manually). Re-reading all 9 `killed/*/REASON.md` files together: in every case the auto-kill
dimension (demand, WTP, or reason-to-exist scoring 2 or below) was backed by concrete, independently
verified evidence — a specific named free/bundled competitor, a specific pricing structure showing
no WTP, or real user-voice evidence contradicting the idea's own claimed differentiator. None of the
9 kills relied on vague pattern-matching or an unsupported low score; each traces to a real,
citable fact. Conclusion: the Phase 1 scoring bar is functioning correctly, not being applied too
strictly — it is accurately detecting that ideas sourced by "search for a well-known pain point"
live in the exact part of solution-space that is easiest for a competitor, an incumbent platform,
or a free tool to also find and close, because that sourcing method is not doing anything a
well-resourced competitor couldn't also do. The fix is upstream, in Phase 0's sourcing channel, not
in loosening Phase 1's bar (which `OWNER.md`'s "Self-correction limits" section forbids weakening
regardless).
Action taken this session: tested two new Phase 0 sourcing channels via a delegated research
subagent, each against 2-3 real examples, independently re-fetched (not trusted from search
snippets) per the fabricated-quote lesson above:
- **Channel A — SaaS vendor public feature-request/roadmap boards** (Canny, UserVoice, in-app
  wishlist boards) for an already-paid product: search for a request with a high vote count, open a
  long time (many months/years), from a real paying customer base, ideally with the vendor's *own*
  stated reason for not building it. A delegated research subagent first reported three candidates;
  per the standing "delegated subagents can fabricate quotes" lesson above, all three were then
  independently re-fetched by the acting session itself (not just trusted from the subagent's
  self-report) before being written up here:
  - Webflow's "Desktop/Offline Application" wishlist item
    ([wishlist.webflow.com/ideas/DESIGNER-I-13](https://wishlist.webflow.com/ideas/DESIGNER-I-13)):
    5,412 votes, created Dec 16 2016 (~9 years open), status "Reviewed" (not a shipped/on-roadmap
    status — inferred from the admin reply below, not a separate status-badge value). Directly
    re-fetched Jan 19 2021 admin reply, verbatim: "this is not something on our roadmap at this time
    due to some of the technical constraints that would place on our product" — Webflow's
    browser-based, live-push-update architecture would need "a major shift to release based
    updates" to support it.
  - Webflow's "European Hosting to comply with GDPR" item
    ([wishlist.webflow.com/ideas/WEBFLOW-I-3429](https://wishlist.webflow.com/ideas/WEBFLOW-I-3429)):
    3,553 votes, created Oct 21 2020 (~5 years open), status "Reviewed," unshipped. Directly
    re-fetched comments, verbatim, e.g. a named Austrian professional (Andreas Grünwald, Sep 18
    2025): "We would like to use Webflow. GDPR and keeping data in Europe is a serious issue," and
    another (Christian Søegaard, Sep 12 2025): "the lack of EU-only hosting is becoming a real
    blocker for many of us working with institutional and public clients in Europe."
  - ClickUp's "Conditional Formatting for Custom Fields (Color based on value)" item
    ([feedback.clickup.com](https://feedback.clickup.com/feature-requests/p/conditional-formatting-for-custom-fields-color-based-on-value)):
    303 votes, created Nov 23 2020 (~5 years open), unshipped, with a 2025 user comment "over 4
    years and still not a thing?"
  This channel is
  meaningfully different in kind from prior sourcing: it starts from a *proven* paying customer base
  (real accounts, real vote counts) rather than a hypothesized one, and a vendor's own admission of
  *why* they won't build it (off their architecture, off their business model, too niche) is a more
  durable "reason to exist" signal than "no competitor found yet" — because it explains why the
  vendor is unlikely to close the gap itself later the way Etsy/Shopify/GitLab did in three of the 9
  kills above, rather than merely not having gotten to it yet.
- **Channel B — recurring freelance-marketplace postings** (Upwork/Fiverr) for a custom-built
  internal tool: multiple similar postings from different clients over time is evidence of real WTP
  and of no adequate off-the-shelf product (otherwise they'd buy instead of commission). Weaker in
  practice this session: Upwork blocked direct page fetches (403) from this environment, so only
  search-index titles/dates were reachable, not full postings — and even where postings were found
  (property-management spreadsheet automation, trucking/logistics dispatch automation), off-the-shelf
  incumbents (AppFolio, Buildium, Guesty) already exist in that space, so recurring bespoke-build
  requests alone don't prove those incumbents have a gap; that still needs the same competitor-
  complaint check as any other candidate.
Going forward: prefer Channel A (vendor feature-request boards on already-monetized products) as
the primary Phase 0 sourcing method over generic pain-point web search, specifically looking for a
high-vote, long-open, vendor-explained-decline request. Do not treat Channel A as sufficient on its
own, either — Phase 1 still needs the full competitor search (this time including the vendor's own
app marketplace/extension ecosystem, since a third party may have already filled a vendor-declined
gap) and the WTP evidence (comparable paid marketplace extensions/add-ons for the same platform).
Channel B is not being retired, but needs a stronger verification path (a way to actually fetch full
Upwork postings, or a substitute source) before it is trusted as more than a directional signal.
One further channel noticed but not tested this session: app-marketplace reviews for small paid
add-ons that already patch a large platform's gap (Shopify App Store, Zapier app directory, Chrome
Web Store) — their own review sections may reveal a second layer of still-unmet needs from a
proven-paying niche. Worth testing in a future sourcing pass.
Applied this session: added one new `BACKLOG.md` candidate sourced via Channel A (a companion
browser extension adding client-side conditional formatting to ClickUp custom fields, addressing
the 303-vote/5-year-old ClickUp feature request above) — disclosed as not yet competitor-checked
against ClickUp's own app marketplace/Chrome Web Store beyond the general searches run this session,
which found no existing extension but did find a proven implementation pattern for the same idea on
a different tool (an open-source `trello-colored-custom-fields` project), and did find at least two
other ClickUp threads asking for related color-coding, suggesting broader appetite than one isolated
request.
Notify owner: no — this is the routine's own anticipated response to a third three-in-a-row trigger,
reinforced by a direct (non-live, written) owner instruction in `OWNER.md`, not a blocked loop. The
conclusion that the scoring bar itself should not be weakened is recorded here and under "Notes for
owner" in `STATE.md` for the owner's own judgment, per `OWNER.md`'s self-correction limits (a
disagreement with a non-negotiable rule is a proposal to the owner, not something to change
unilaterally).

### Channel A risk: a vendor's feedback board can evidence demand for a feature it already partly ships elsewhere
First seen: 2026-09-17 (day 015, first Validate run using the Channel A sourcing method adopted
2026-09-16) | Status: open (apply and watch)
Symptom: Channel A (SaaS vendor public feature-request/roadmap boards) surfaced a candidate — a
ClickUp custom-field conditional-formatting extension — with the strongest demand evidence this
project has recorded (4/5, four independently re-verified feedback-board threads spanning ~5.7
years). It still killed, on willingness-to-pay (2/5: no paid comparable found anywhere for this
feature category; the one true implementation analog, `trello-colored-custom-fields`, is free/
open-source). Re-reading the demand threads closely (not just counting votes) surfaced something
the original BACKLOG.md write-up missed: two of the four threads reveal ClickUp already ships the
exact requested mechanism (value-based card coloring by custom field) natively — just only in
Calendar View, not Table/Board/List. The four requests are functionally "extend this to more
views," not "build this from nothing."
Why this matters specifically for Channel A: the method's original rationale (2026-09-16 entry
above) was to prefer requests where the *vendor's own stated reason* for declining suggests they
are unlikely to close the gap themselves (the Webflow precedent: an admin reply citing real
architectural constraints). A request where the vendor has already built 80% of the mechanism
elsewhere in the same product, with no stated objection to extending it, is the opposite case: the
vendor can close the gap for free, cheaply, whenever it wants, which is a weaker "reason to exist"
argument even before WTP is checked — and, per this kill, may correlate with weak WTP too (users
expect the vendor to ship it, not to pay a third party).
Action: when Channel A surfaces a candidate, before scoring, explicitly check whether the requested
feature (or a materially similar version of it) already exists natively for a different view/
context/plan tier within the same product. If it does, and no vendor statement of technical or
business-model refusal is found (the Webflow-style signal Channel A was designed around), treat
that as a specific negative signal on "reason to exist" and flag it directly in VALIDATION.md,
rather than treating "ClickUp itself has never shipped this natively" (true of the literal request,
false of the underlying mechanism) as sufficient framing.
Notify owner: no — caught within the normal Phase 1 process this session, not a blocked loop. Not
a third consecutive kill (first since the 2026-09-16 review), so the three-in-a-row trigger does
not fire.

### Aggregate/advocacy statistics are not a substitute for real user-voice demand evidence
First seen: 2026-09-18 (day 016, grant-report-normalizer kill) | Status: open (apply and watch)
Symptom: the candidate's `BACKLOG.md` write-up quoted a real, independently-verifiable statistic
from a funder-advocacy association's blog ("a single organization may juggle 40-60 applications...
from 20-30 funders") plus a real 280-organization grantmaker survey confirming the structural
problem — both accurate, both re-verified by direct fetch, neither fabricated. It was tempting to
read this as satisfying the routine's "real user-voice demand evidence" bar. On closer search,
zero individually-voiced nonprofit-staffer complaints matching the idea's own specific thesis (pain
from *many* differing funder templates, not one) were found anywhere — the one attributable
practitioner complaint located (NonprofitAF.com) was about a single-funder format demand. The
statistics are real; they are just a different kind of evidence (aggregate/survey/advocacy) than
"a real person, in their own words, describing wanting this specific thing," and the two were at
risk of being conflated.
Why this matters: an idea can look well-evidenced (accurate citations, real URLs, no fabrication)
and still fail the demand bar, because *accuracy* of a citation is necessary but not sufficient —
the citation also has to be the right *kind* of evidence (individual user voice) for the specific
claim it's being used to support (that users want this particular differentiator), not just
adjacent-but-real evidence about the general problem space.
Action: when scoring "evidence of demand," explicitly separate (a) statistics/survey/advocacy
sources confirming a problem exists in aggregate from (b) individually-attributed posts where a
real person describes wanting the *specific* thing the idea proposes. Score (b) is what the
demand dimension is actually about; (a) alone, however well-verified, does not satisfy it. If
search for (b) comes up empty despite genuine effort, that is "no evidence found" per routine
non-negotiable #4, not a gap (a) can fill.
Notify owner: no — caught within the normal Phase 1 process this session, not a blocked loop. This
is the second kill since the 2026-09-16 sourcing-method review (day 015, day 016) — not yet a
third-in-a-row; the trigger fires if day 017 also kills.

### Sourcing method review after a second three-in-a-row since 2026-09-16: tested Channel D (second-order gaps in paid app-marketplace add-ons)
First seen: 2026-09-21 (day 019, second three-in-a-row trigger since the 2026-09-16 review — day
015, day 016, day 018 all killed via real research; day 017 was replenishment-only and doesn't
count) | Status: open (apply and watch)
Symptom: Channels A/B/C have each now produced at least one real kill or a real discriminating
result, but each also carries a known structural weakness (Channel A: the vendor can extend its
own feature for free; Channel B: the task often duplicates what an incumbent's base subscription
already covers; Channel C: review-site sourcing starts from an existing competitor's own review
page, so it is biased toward finding spaces that already have an incumbent by construction). The
untested channel flagged on 2026-09-16 — app-marketplace reviews for small *paid* add-ons that
already patch a gap in a large host platform (Shopify App Store, Atlassian Marketplace,
WooCommerce.com, etc.) — was tested this session via a delegated research subagent, briefed to
fetch real review pages directly (not search snippets) and to run an explicit second-gap
competitor check on any complaint found, per the standing fabrication and "bare existence isn't
enough" lessons above.
Findings (spot-checked independently by this session via direct `WebFetch` re-fetch of the same
review pages — both key quotes and the pivotal competitor-existence claim confirmed verbatim, no
fabrication found):
- **Real second-order gap found:** Sufio (Shopify invoicing app, 465 ratings, 4.9★), reviewer
  MEGAMI Wholesale (edited Sep 11 2026, 3★): "When an order is canceled in Shopify, Sufio does not
  change the invoice status to 'Canceled.' The only option is to create a credit note, which does
  not work for our business..." A second complaint on the same app (Rusty Dog Coffee, 2★) that no
  ACH-via-Stripe option exists was *confirmed by Sufio's own developer reply* ("due to Shopify's
  partner terms, we're currently unable to support ACH through Stripe in our integration") — a
  vendor-admitted gap, structurally similar in kind to the Channel A Webflow precedent, but on a
  paid add-on rather than the host platform itself. The second-gap check found only a narrow
  regional bridge app (FGO Integration, Portugal/EU-specific) partially covering the
  cancellation-sync gap, and nothing covering the ACH gap except Shopify itself — which appears to
  have already closed most of it. The original draft of this entry claimed Shopify Payments' ACH
  Direct Debit for B2B is "Plus/B2B/US-dollar-store" only; the independent pre-merge review flagged
  that as likely outdated and unsourced. Re-checked directly: Shopify's own help-center ACH page
  (`help.shopify.com/en/manual/payments/shopify-payments/local-payment-methods/ach`) now says only
  that the store must be "on an active Shopify subscription plan" (no plan-tier restriction stated,
  where a Plus-only feature would normally say so), consistent with secondary reporting that the
  feature expanded from Plus-exclusive at its 2025-10-23 launch to all paid plans by 2026-04-02 —
  though the primary-source changelog post itself
  (`changelog.shopify.com/posts/ach-direct-debit-bank-payments-for-b2b`) does not state plan tiers
  either way, so the exact expansion date is not independently confirmed from a primary source,
  only inferred from the current help-center wording plus secondary blogs. Recording this
  explicitly as "very likely available on all paid plans now, exact rollout date not
  primary-source-confirmed" rather than asserting the date as settled fact. Either way, the
  practical point holds: this is a stronger version of the same "platform closes it for free
  eventually" risk Channel A already surfaces — the platform looks to have already closed most of
  the ACH gap itself, not left it open indefinitely.
- **False-positive case, caught only by running the second-gap check:** Matrixify (Shopify bulk
  import/export app, 1,654 ratings, 4.9★), reviewer DooDah Distribution (2★): complained that
  direct Airtable-URL import isn't supported, confirmed by the vendor's own reply ("Direct Airtable
  URL imports are not a feature Matrixify offers"). This looked like a fresh second-order niche —
  but the second-gap search found it is *already* served by at least three dedicated products
  (SyncBase, AirSync for Airtable — independently re-confirmed live via direct fetch, "AirSync
  automatically syncs your Shopify store data with Airtable" — and AirPower) plus generic
  Zapier/Make.com connectors. A complaint that looks like an untapped niche can already be crowded
  by other *specialist* competitors searching the exact same review pages — a distinct risk from
  Channel A/D's "platform closes it for free" risk, and arguably harder to rule out, since these
  competitors are themselves narrow and easy to miss without a dedicated search.
- **Marketplace reachability is inconsistent and must be checked per marketplace, not assumed:**
  Shopify App Store review pages fetched reliably; Atlassian Marketplace's review pages returned
  HTTP 410/redirected to a not-found page (fully blocked); WooCommerce.com's product-review API
  was reachable but didn't expose star ratings, making a targeted 1-3-star pull impractical without
  more effort than this session budgeted.
Action: add Channel D to the sourcing rotation, but only with its second-gap competitor check as a
hard gate, not an optional step — the Matrixify/Airtable case shows a real, individually-voiced,
vendor-confirmed complaint can still be a false positive for "reason to exist" if the check is
skipped. When a Channel D complaint survives its own second-gap check (like the Sufio/MEGAMI
cancellation-sync gap), still apply the existing "regulatory/platform-native-closure" check from
the 2026-09-15 entry above before scoring "reason to exist" — the platform itself (not just another
add-on) remains the most likely eventual free closer for gaps this deep in its own ecosystem.
No new `BACKLOG.md` candidate added this session: the one surviving Channel D finding
(Sufio/MEGAMI invoice-cancellation-sync) is a single-feature gap in someone else's app, not a
standalone product with its own (a)/(b)/(c) case at this project's usual candidate scope — writing
it up as a full candidate without a broader thesis around it would repeat the "force a candidate to
fill the session" mistake this project has avoided so far. Flagging it here as a validated *pattern
instance* for a future sourcing pass to build on (e.g. "Shopify-ecosystem invoice/accounting-app
sync gaps" as a broader thesis, tested across more than one invoicing app) rather than adding it
directly.
Notify owner: no — this is the routine's own anticipated response to a second three-in-a-row
trigger since the last review, not a blocked loop.

### Practitioner-forum layer is shrinking faster than sourcing method can rely on; review-site-only search is becoming the practical ceiling in several verticals
First seen: 2026-09-21 (day 019, sourcing-method review) | Status: open (apply and watch)
Symptom: rereading the accumulated "Reddit unreachable" entry above end-to-end shows a pattern
larger than any single blocked site. Independently-operated grassroots forums have gone unreachable
one by one across sessions, not just Reddit: Reddit (blocked since day 007), G2 (403 since ~day
012, recurring), Upwork (403, day 014/016), eng-tips.com (403, day 012), and — newly, in the same
single session (day 018) — Mike Holt's Forum (403, previously worked day 012), ElectricianTalk and
ContractorTalk (both now redirect to a `tollbit.*` bot-paywall gateway returning HTTP 402), and JLC
Online (old thread URLs now 301 to the plain homepage — content gone, not just blocked). Meanwhile,
across the same span, vendor-controlled surfaces have stayed reachable in every session that tried
them: Capterra, Software Advice, GetApp, Trustpilot, vendor feedback/wishlist boards (Channel A),
Freelancer.com (Channel B, though not Upwork), and — this session — the Shopify App Store (Channel
D). The pattern is not "one site is down," it is "independently-hosted, bot-unfriendly community
forums are being progressively closed off to automated fetching across verticals, while
platform-owned or vendor-owned review/feedback surfaces are not." Six-plus sessions of accumulating
evidence make this look structural (bot-detection/anti-scraping deployment trending in one
direction over time) rather than a run of unrelated coincidences.
Why this matters for "evidence of demand" search specifically: this project's demand bar wants
"individually-voiced" user complaints (the 2026-09-18 entry above), and grassroots forums were the
richest source of that voice precisely because they are not tied to being a confirmed customer of
any specific product. Review sites (Capterra/G2/etc.) only capture the voice of people who already
bought *some* existing product — which is fine for the routine's actual bar ("a reason to exist
*alongside* what already ships," not "zero competitors"), but it structurally means most reachable
demand evidence going forward will arrive already paired with an incumbent, not as a clean "nobody
serves this" signal. A sourcing process still budgeting real search time on forums that have a
multi-session blocked track record (e.g. attempting Mike Holt/ElectricianTalk/ContractorTalk fresh
for a new construction-adjacent idea) is spending effort on a channel this project's own history
says is very unlikely to be open, at the cost of time that could go to a deeper review-site pass or
a second-gap check.
Action: (1) stop budgeting significant fresh search time on forums with a 2+-session blocked
track record for a given vertical (Reddit generally; Mike Holt/ElectricianTalk/ContractorTalk/JLC
for construction/trades specifically) — one quick attempt per session is still fine in case a block
is intermittent, but do not let its near-certain failure read as reduced diligence, and do not let
chasing it eat time better spent elsewhere. (2) Treat vendor/platform-owned surfaces (review sites,
vendor feedback boards, Channel D app-marketplace reviews, Freelancer.com) as the primary reachable
user-voice channel set going forward for most verticals, not a fallback. (3) Because that channel
set structurally pairs demand evidence with an existing competitor, do not treat "found real
demand evidence" and "found no competitor" as independently likely outcomes from the same search
pass any more — expect them to arrive together, and budget explicit separate time for the
sub-segment/second-gap competitor-complaint search this project already requires, rather than
treating a review-site demand hit as evidence the field might still be open.
Notify owner: no — this extends the already-escalated 2026-09-12 Reddit notification (`OWNER.md`'s
"same failure blocked three sessions" bar was already met and reported for Reddit specifically);
this entry generalizes the pattern across sources but is not a new blocked-loop condition, since
review sites and vendor surfaces remain reachable and the session's work was not itself blocked.

### Auto-kill dimension set is inconsistently documented across killed ideas: does "reason to exist" count on its own?
First seen: 2026-09-22 (day 020, `qbo-weekly-snapshot` kill, caught by the pre-merge review
subagent) | Status: open (apply and watch)
Symptom: the routine's Phase 1 spec states the total-score kill threshold (<16) and separately
says "Kill the idea if... demand, willingness to pay, or buildability scores 2 or below" — three
named dimensions, not five. Day 020's `VALIDATION.md`/`REASON.md` scored "reason to exist" at 1/5
and called that an independent auto-kill on its own, following this repo's own day-014 `RUNBOOK.md`
entry, which lists "demand, WTP, or reason-to-exist" as the auto-kill set. But several earlier
`killed/*/REASON.md` files (`ci-migration-rollback-gate`, `contractor-classification-checker`,
`grant-report-normalizer`) only ever invoke demand/WTP as auto-kill triggers, and
`killed/clickup-conditional-formatting/VALIDATION.md` states outright that reason-to-exist "is not
itself an auto-kill dimension." The repo has been applying two different rules inconsistently
across sessions, not just today.
Action: this doesn't change any past verdict — every kill scoring "reason to exist" at ≤2 also
scored <16 total (day 020 included: 13/25), so no idea was wrongly killed on this technicality
alone. But a future idea could plausibly score reason-to-exist at 1-2 while still totaling ≥16 on
the other four dimensions, and whether that's an auto-kill is currently undecided repo doctrine.
Do not silently pick one interpretation next time it matters — flag it under "Notes for owner" in
`STATE.md` when it becomes score-determinative, since the routine spec's own text (three named
dimensions) is the more authoritative reading and a runbook entry cannot override the routine
spec, only clarify how this repo has been applying it.
Notify owner: no — doesn't change any past verdict, and no session is currently blocked by it;
noting here so it isn't rediscovered from scratch next time it's score-determinative.
