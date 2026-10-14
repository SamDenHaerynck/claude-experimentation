# Handoff

Status at day 040: 11 of 11 build slices done, review round 1 done, documents written. Never run
against a real Access Point or a real Optimizely Configured Commerce install.

## What works (tested against fakes and the official rules)

- Order JSON to a Peppol BIS Billing 3.0 UBL invoice, validated against UBL 2.1 XSD, EN16931 and
  Peppol Schematron with rule ids. VAT categories S, Z, E, AE, K, O; document-level allowances and
  charges; IBAN and Belgian structured communication; order reference and payment terms.
- Outbound flow: build, validate, send through Storecove or Recommand clients, with idempotency key,
  classified retryable/permanent failures, failed-document log and stable log event ids.
- `OutboundDispatcher` over the `IOrderInvoiceSource` and `IAccessPointSettingsProvider` contract.
- Inbound webhook host that fetches and parses a received invoice and answers with an acknowledgement.
- Walkthrough (`dotnet run --project samples/BePeppolCommerce.Walkthrough`) of the whole flow.
- 297 tests (215 core, 82 API).

## What does not work, or is unproven

- **Not a Configured Commerce extension.** The contract is documented (`app/docs/CONFIGURED_COMMERCE_CONTRACT.md`)
  but never built. Installs still on `net48` cannot use the `net8.0` library; how many Belgian installs
  have migrated is unknown.
- **No real provider has ever been called.** Storecove: the name for Belgian scheme 0208/9925 is
  unconfirmed (H3 in REVIEW.md, set `Storecove__SchemeMap__0208`), the received document's `original`
  encoding, the webhook body and webhook authentication, and idempotency-key behaviour are unknown.
  Recommand: real webhook deliveries are not parsed (event envelope, `eventType`).
- **No credit notes**, outbound or inbound (M5). A negative total is refused with `BEPC-BUILD`.
- **No line-level allowances or charges** (H2 remainder). Send discounts as document allowances; a
  negative-price discount line fails BR-27.
- **Received invoices go nowhere** (M6): parsed, acknowledged, not stored.
- **Recommand can double-deliver** (M9): its send has no idempotency key, and there is no retry cap, so
  a `transport` 422 or timeout is retried every run. Storecove is the safer default.
- **Webhook replay** (M10) and no rate limiting, no minimum secret length (L10).
- **No field mapping table** for Configured Commerce (M12): where buyer endpoint id, VAT category and
  exemption reason live in an order, plus no receiver-capability lookup.
- Open lows: L4 charset throw, L5, L6 decimals, L7 parser text in 422, L8 response size cap, L9 missing ids.
- Non-EUR invoices omit the EUR VAT total (L12). Conflicting exemption reasons in one VAT group are not rejected at parse.
- IBAN/OGM checksums run only on the `OrderJson.Parse` path.
- The validator has not been run from a published build; the build output is large (about 340 MB, 92 MB published).
- Licensing: the repo downloads the Peppol Schematron at build time and the built DLL embeds it; see
  the licensing note in `STATE.md` before distributing anything. Early commits remain reachable on the
  old branch and `refs/pull/31/head`.
- The 12-session cap: build used 11; whether fixing review highs counts as build is yours to confirm
  (STATE.md, day 037 note).

## First three things to do

1. **Run the validation kit** (`VALIDATION_KIT.md`) with two or three Belgian Optimizely clients, and
   record results in `SIGNALS.md` (write "stop" to end the idea). Ask their Configured Commerce version.
   Everything else depends on whether anyone wants this.
2. **If there is interest, open a Storecove or Recommand sandbox account** and send one invoice from the
   walkthrough order; settle the scheme name, document encoding, webhook shape and idempotency
   behaviour, and fix what differs. Resolve the OpenPeppol redistribution question before any package is published.
3. **Get a Configured Commerce dev instance (or a partner's help)** and build the extension against the
   contract, starting with the order-to-`Order` mapping table (M12) and a durable queue. Then close
   the open items: credit notes, an inbound sink, retry cap.
