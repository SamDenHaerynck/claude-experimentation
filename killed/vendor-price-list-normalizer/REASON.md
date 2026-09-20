# Killed: Vendor price-list ingestion/normalizer for construction estimating software

Killed in Phase 1 (Validate), day 018, 2026-09-20. Score: 13/25 (below the 16 threshold); evidence
of demand independently scored 1/5 (auto-kill on its own).

## Why

The idea's only demand evidence was a single Capterra review (Gary L., STACK Takeoff, 2025-10-27)
describing the pain of manually importing/converting vendor price lists. This project's own bar
requires 3+ individually-voiced complaints about the same specific pain before demand counts as
evidenced. An exhaustive re-check — every review across STACK Takeoff, PlanSwift, ProEst,
Buildxact, Clear Estimates, and Esticom/Procore Estimating (~500+ reviews), plus several
practitioner forums and G2 — found no second matching complaint. Two related-but-different
complaints turned up (internal price-sync friction on PlanSwift; cost lookup friction on Esticom)
but neither matches the specific "importing an external vendor's price sheet" pain.

Separately, the originally-claimed competitive gap ("no general vendor price-list importer exists
anywhere") does not hold: Buildxact already ships a built-in general vendor price-list importer
(upload Excel, map columns, flag errors) as a free, bundled feature, and Buildern/Handoff advertise
similar bulk-import capability. The idea's differentiator (cross-platform + change-diffing) still
has no exact match, but a contractor with this pain has a materially cheaper existing option
(switch to or already use Buildxact) than paying for a standalone third-party tool.

## Caveat

Several channels most likely to carry trades-specific complaints were technically unreachable to
this session (G2 403, Mike Holt's Forum 403, ElectricianTalk/ContractorTalk paywalled via a
`tollbit.*` gateway returning 402, JLC Online's old forum URLs now redirect to its homepage, Reddit
unreachable — a standing five-session-old limitation). This is recorded honestly as "no evidence
found" from those specific sources, not as a clean negative search; see the updated `RUNBOOK.md`
entry. A human with direct/authenticated access to those sites could still find corroborating
evidence this session could not reach — but based on everything actually verifiable, demand does
not clear the bar.

## Disposition

Moved to `killed/vendor-price-list-normalizer/`. `BACKLOG.md` former #2 (QuickBooks micro-business
weekly financial snapshot) becomes new #1. This is the third kill since the 2026-09-16
sourcing-method review, counting only sessions that ran a Validate (day 015, day 016, day 018 —
day 017 was a replenishment-only session and does not count, per the day-011→012 precedent). The
routine's three-in-a-row trigger fires again: day 019 must spend its session on sourcing/validation
method, not on validating the new #1.
