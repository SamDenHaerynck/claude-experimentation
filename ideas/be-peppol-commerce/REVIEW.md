# Review

## Round 1 (day 037, 2026-10-09)

Wave one: three adversarial `general-purpose` subagents in parallel, each writing only to its own
scratch directory: (a) correctness and tests, (b) security and input handling, (c) product quality
against the core flow and done criteria. Wave two (docs: follow `app/README.md` literally from a
clean clone) has not run yet.

Findings merged and de-duplicated by the main session. Where two reviewers raised the same defect at
different severities, the higher one is kept and both sources are named. Status: **fixed** (day
037, with test), **open** (to fix in a later session), or **accepted** (not fixed, reason given).

### High

| # | Source | Finding | Status |
|---|---|---|---|
| H1 | correctness, product | Only VAT categories S and Z could produce a valid invoice. No exemption reason (BT-120/121), no Delivery (BT-72/BT-80), and `Percent` always written, so K (intra-EU), AE (reverse charge), E and O orders failed BR-IC-10/11/12, BR-AE-10, BR-E-10, BR-O-05/10 and were parked as permanent validation failures. | **Fixed.** `OrderLine.VatExemptionReasonCode`/`VatExemptionReason` written on each VAT breakdown; `Order.DeliveryDate`/`DeliveryCountryCode` written as `cac:Delivery`; `Percent` left out for O; optional `Party.LegalRegistrationId` (BT-30/47) so an O seller without VAT number passes BR-CO-26. Validator-backed tests for K, AE, E and O plus negatives for K without delivery and AE without reason. |
| H2 | product (high), correctness (medium) | No allowances or charges. A discount, promotion or freight charge cannot be expressed; a negative-price discount line fails BR-27 and is parked permanently. | **Open.** A model and totals change (document- and line-level `cac:AllowanceCharge`, `AllowanceTotalAmount`/`ChargeTotalAmount`, per-category tax effect). Next session's first task. |
| H3 | product (high), correctness (medium) | The documented Configured Commerce path (`AccessPointClientFactory` from `AccessPointSettings`) could not pass a Storecove `SchemeMap`, so `0208` would always go to Storecove unmapped and every Belgian send would likely be rejected as permanent. | **Fixed.** `AccessPointSettings.SchemeMap` added and passed to `StorecoveOptions`; factory test proves the mapped scheme is sent and a bad map is rejected. The real Storecove name for 0208/9925 is still unconfirmed (no sandbox); stays a HANDOFF.md gap. |

### Medium

| # | Source | Finding | Status |
|---|---|---|---|
| M1 | security | Lone surrogate escape (`{"documentId":"\ud800"}`) made `JsonElement.GetString()` throw `InvalidOperationException`; webhook answered 500 (stack trace in Development) instead of 400. | **Fixed.** Caught in `DocumentId`; two test cases added. |
| M2 | correctness | An XML-invalid character in an order string (e.g. vertical tab from pasted text) throws at serialisation; the dispatcher treats it as retryable "Unexpected error" and stops every later run. | Open. Strip or reject in `OrderJson.Parse`/builder, report as permanent BEPC-BUILD failure. |
| M3 | correctness, product | `DueDate` and `BuyerReference` are optional in `Order` but missing either makes the invoice invalid (BR-CO-25, PEPPOL-EN16931-R003); no `OrderReference` or `PaymentTerms` fallback. | Open. Add order reference and payment-terms text. |
| M4 | product | No `PaymentMeans`/IBAN or Belgian structured communication (OGM). Valid but unpayable in practice. | Open; HANDOFF.md gap at minimum. |
| M5 | product, correctness | Credit notes unsupported both ways: negative-total "invoice" validates and would go out as type 380; inbound CreditNote (and SBDH-wrapped documents) rejected with 422. | Open. Reject negative totals now; credit note support is a HANDOFF.md gap unless time allows. |
| M6 | product | Received invoices are parsed and returned in the HTTP response only; no sink/store for the host to consume. | Open; HANDOFF.md gap (inbound Configured Commerce contract "not designed yet" already stated). |
| M7 | security | Webhook returns the parsed invoice (party data, amounts) to the caller, making the secret a bearer token for invoice data. | Open. Return an acknowledgement only; ties into M6. |
| M8 | product | Formatted Belgian enterprise numbers (`0403.170.701`) are not normalised and fail PEPPOL-COMMON-R043 permanently. | Open. Strip dots/spaces and check mod-97 in `OrderJson.Parse`. |
| M9 | correctness | Recommand send timeout or 5xx after delivery is retryable; no idempotency key, so a resend can deliver twice. | Open; extends the day-035 retry-cap gap. HANDOFF.md gap. |
| M10 | security | Signed Recommand deliveries can be replayed (no timestamp, nonce or dedup). | Open; Recommand documents no signed timestamp. HANDOFF.md gap plus host-level rate limiting note. |
| M11 | security | Provider error bodies logged verbatim and stored in failure details; may carry invoice PII. | Open. Truncate/redact provider text. |
| M12 | product | Configured Commerce contract lacks a field mapping table (where buyer endpoint id, VAT category, exemption reason live) and a receiver-capability lookup. | Open; HANDOFF.md gap. |

### Low

| # | Source | Finding | Status |
|---|---|---|---|
| L1 | security | `StorecoveOptions` record `ToString` printed the API key. | **Fixed**, with test. |
| L2 | product | Walkthrough ignored its order-path argument. | **Fixed**: `Program.cs` passes `args[0]`; README documents it. |
| L3 | correctness | VAT category not normalised (`"s"` split subtotals and failed BR-CL). | **Fixed**: trimmed and upper-cased in the builder; test. Not yet checked against UNCL5305 at parse time. |
| L4 | correctness | `ReadBodyAsync` throws on an unknown charset, breaking the never-throw contract. | Open. |
| L5 | correctness | Recommand `deliveryFailure.category` lost when `errors` has an unexpected shape. | Open. |
| L6 | correctness | Price/quantity written at 10 decimals while line amount uses full precision (R120 on extreme inputs). | Open. |
| L7 | security | Parser error text (quoting document content) returned in 422 body. | Open. |
| L8 | security | Provider responses buffered without a size cap before the 10M-char parse limit. | Open. Set `MaxResponseContentBufferSize`. |
| L9 | security | Missing `id`/`companyId`/`guid` in provider responses accepted. | Open. |
| L10 | security | No minimum secret length, no rate limiting, TLS left to the host. | Open; DEPLOYMENT.md to require a TLS proxy. |
| L11 | security | Webhook auth is off whenever the environment is `Development`. | Accepted for now: documented local-run behaviour; DEPLOYMENT.md must say never to run Production as Development. Revisit if time allows. |
| L12 | product | Non-EUR invoices omit the EUR VAT total (TaxCurrencyCode). | Accepted: v1 is Belgian EUR; HANDOFF.md gap. |
| L13 | product | `HANDOFF.md` does not exist, so the last done criterion does not hold yet. | Expected: written in Phase 5. |

Security reviewer also confirmed as sound: XXE/DTD handling, embedded-only XSD/Schematron resolvers,
constant-time secret and HMAC comparison, the 16 KB webhook body limit, id regexes, and the
https-or-loopback base URI rule. No committed secrets.
