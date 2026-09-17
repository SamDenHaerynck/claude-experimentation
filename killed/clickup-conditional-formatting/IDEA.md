# Idea: ClickUp custom-field conditional-formatting companion extension

## Concept
A browser extension (Chrome/Edge, using ClickUp's public API and/or client-side DOM injection) that
adds conditional formatting/coloring to ClickUp custom fields in List and Table views, based on the
field's value — e.g. a Priority-score field turns red above a threshold, a Budget field turns green
under budget. ClickUp has never shipped this natively.

## Target user
ClickUp power users running List/Table views with numeric or dropdown custom fields who want an
at-a-glance visual signal without opening each row — agencies, PMO teams, ops teams using ClickUp as
a lightweight database/CRM substitute.

## Problem
ClickUp's own public feedback board has an open, unshipped request for exactly this capability,
open for roughly five years with hundreds of votes (see VALIDATION.md for exact figures/URLs). Users
report doing this today with manual workarounds (renaming values with emoji, duplicating fields) or
going without.

## Why now
No specific regulatory or platform trigger — this is a persistent, long-unaddressed gap in an
existing paid platform (ClickUp), sourced from ClickUp's own public feedback board rather than
generic pain-point search (see RUNBOOK.md "Sourcing method review" entry, 2026-09-16, for why this
sourcing channel was adopted). The "why now" case rests on: (a) the request being old enough and
high-vote enough to demonstrate durable, not fad, demand, and (b) ClickUp's own architecture/roadmap
reasons for not building it natively (if any are found — see VALIDATION.md) suggesting the vendor is
unlikely to close the gap itself soon, unlike cases in this repo's kill history where the incumbent
platform later shipped the fix for free.

## Full evidence, scoring and verdict
See VALIDATION.md.
