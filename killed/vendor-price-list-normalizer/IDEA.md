# Vendor price-list ingestion/normalizer for construction estimating software

## Concept
A companion tool for contractors/estimators who use takeoff/estimating software (STACK,
PlanSwift, Sage Estimating, ProEst, etc.). It ingests messy vendor price lists — PDFs, Excel
sheets, scanned sheets — from a contractor's own suppliers, normalizes them into the estimator's
item/cost-code format, and flags what changed since the last import (price increases, discontinued
SKUs, new items). It is a companion to the existing estimating tool, not a replacement for it.

## Target user
Construction estimators and small/mid-size contractors who maintain their own negotiated vendor
price books inside a takeoff/estimating tool, and who re-import updated price sheets from multiple
suppliers on a recurring (weekly/monthly/per-bid) basis.

## Problem
Vendor price sheets arrive in inconsistent formats (PDF, Excel, sometimes scanned) with the
vendor's own SKU/description/unit conventions. Getting them into the estimator's own cost-code
structure is currently manual: read the sheet, match items, convert units/pricing, and re-enter
values line by line. This is repeated every time a vendor updates prices.

## Why now
No specific new regulatory or platform trigger identified. The driver here is sourced friction
(Channel C: a paid-customer review), not a timing event — see `BACKLOG.md` entry #1 for the
original sourcing and `VALIDATION.md` for the Phase 1 evidence pass.

## Property case (from `BACKLOG.md`)
- (a) Vendor/commodity prices change on an ongoing basis and must be kept current.
- (b) Every estimate an contractor produces depends on the price catalog being current — this is a
  recurring workflow dependency, not a one-time generated artifact.
- (c) A stale or mis-converted price directly costs margin on a job or a lost bid — real money, not
  just inconvenience.

See `VALIDATION.md` for the full Phase 1 evidence pass, including the risk `STATE.md` flagged as
highest-priority before this idea can be scored: whether the demand evidence clears the project's
"3+ individually-voiced complaints" bar (currently n=1), and independent re-verification of the
claimed competitive gap.
