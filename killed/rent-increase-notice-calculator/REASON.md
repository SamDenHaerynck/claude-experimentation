# Killed: Rent increase / notice compliance calculator

Killed in Phase 1 (Validate), day 004, 2026-09-06. Score: 11/25 (below the 16 threshold), and
"willingness to pay" scored 1/5 on its own — an independent auto-kill trigger.

Full evidence, competitor research, and scoring are in `VALIDATION.md` in this directory. Short
version: real user confusion about rent-increase caps and notice periods does exist (three
first-person BiggerPockets threads across three US states, 2016-2021, plus an adjacent Canadian
tribunal case with real financial consequences), but the exact proposed feature set —
jurisdiction-aware cap calculation, notice-period lookup, and compliant letter generation, together
— is already shipped free by at least eight separate tools (Rentlane, LeaseBase, RentGuard CA,
Shuk Rentals, rentcap.netlify.app, lawagreements.com, openigloo, rentlatefee.com), none of which
charge for it; mainstream PM software (TurboTenant, DoorLoop, Buildium, Avail, Zillow, Innago) only
offers static templates, not the automated calculation itself, so it isn't a source of paid
competition either. No willingness-to-pay evidence was found anywhere, and every first-person
confusion instance found was resolved for free by a peer in the same forum thread. No wedge
identified for a paid entrant against either the free calculators or the free peer-forum answers.

This is the third backlog idea killed in a row (after dependency-eol-watcher and
vendor-security-questionnaire-autofill), which triggers the routine's rule: the next session goes
to reviewing the sourcing method rather than generating a fourth idea the same way. See
`DECISIONS.md` and `STATE.md` for the carried-forward next action.
