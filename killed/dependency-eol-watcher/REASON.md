# Kill reason: Dependency deprecation/EOL watcher

Killed in Validate (Phase 1), day 002, without building anything.

Score: 14/25 (below the 16 threshold), and Evidence-of-demand scored 2/5 on its own, which is an
independent kill condition per the routine's rules.

Summary: the problem (EOL/deprecated dependencies going unnoticed) is real per industry sources,
but the space is already served by free, widely-installed tools (GitHub Dependabot, Renovate) plus
a free public EOL data API (endoflife.date). The one concrete first-person demand signal found — a
GitLab feature request — asks for EOL data inside an existing tool, which undercuts the case for a
new standalone product. See VALIDATION.md for full scoring and sources.

Not eligible for rescoring later per the routine's non-negotiables.
