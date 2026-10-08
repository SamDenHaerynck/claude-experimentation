# BePeppolCommerce

A .NET 8 library that turns an order record into a Peppol BIS Billing 3.0 UBL invoice. It is being
built as the engine for a future Optimizely Configured Commerce connector for Belgian e-invoicing.
The idea and its evidence are in `../VALIDATION.md`, and the full plan is in `../PLAN.md`.

**Status: all 11 build slices done (1, 2a, 2b and 3 to 10); in review next.** Nothing here has
ever talked to a real Access Point or a real Optimizely Configured Commerce install. Right now it:

- parses an order from JSON (`BePeppolCommerce.Core.Model.OrderJson`), rejecting null or empty
  required values
- builds a UBL 2.1 Invoice XML with the Peppol BIS 3.0 `CustomizationID`/`ProfileID`,
  seller/buyer parties (Belgian `0208` endpoint scheme), per-rate VAT breakdown, totals and lines
  (`BePeppolCommerce.Core.Ubl.PeppolInvoiceBuilder`)
- validates an invoice or credit note (`BePeppolCommerce.Core.Validation.PeppolValidator`) against
  the UBL 2.1 XSD and the official EN16931 (CEN 1.3.15) and Peppol BIS Billing 3.0.20 Schematron
  rules, and returns every failed assert with its rule id (for example `PEPPOL-EN16931-R003`), flag
  (`fatal` blocks, `warning` does not), message and location. String input is parsed with DTDs
  prohibited, so XXE payloads are rejected.
- talks to a Peppol Access Point provider through `BePeppolCommerce.Core.AccessPoint.IPeppolAccessPointClient`
  (send a document, fetch a received document by id). The first implementation is `StorecoveClient`,
  written against Storecove's public OpenAPI spec (https://api.storecove.com/api/v2/openapi.json).
  It is **tested only against a stubbed `HttpMessageHandler`** and has never called the real
  service. Two points still need checking against a real response: the spec does not say how a received
  document's `original` field is encoded (the client accepts raw XML or base64), and it does not
  define the webhook body. Storecove also names identifier schemes its own way (the spec's examples are
  "DE:VAT" and "FR:CTC"). The Belgian name is not confirmed, so `StorecoveOptions.SchemeMap` must
  map `0208` to it. Unmapped schemes are sent unchanged.
  The second implementation is `RecommandClient`, written against Recommand's OpenAPI 3.1 spec
  (https://peppol.recommand.eu/openapi). It sends the UBL as raw XML (`POST /api/v1/{companyId}/send`,
  `documentType: "xml"`) with HTTP Basic auth (API key and secret), and fetches received documents
  with `GET /api/v1/documents/{documentId}`, accepting only `direction: "incoming"` documents of the configured company. A send that Recommand delivered by email instead of Peppol (`sentOverPeppol: false`) is reported as failed. It is also
  **tested only against a stubbed `HttpMessageHandler`**. Recommand's send endpoint has **no
  idempotency key**, so with Recommand a retried send can deliver the same invoice twice.
  `AccessPointClientFactory` picks the client from `AccessPointSettings.Provider` (`storecove` or
  `recommand`).
- runs the outbound flow in one call (`BePeppolCommerce.Core.Outbound.OutboundInvoiceSender`):
  order, then UBL XML, then validation, then send to the buyer's endpoint (`EndpointSchemeId` and
  `EndpointId`) through any `IPeppolAccessPointClient`. It returns `ValidationFailed` (nothing was
  sent; the findings say why), `SendFailed` (valid, but the provider refused it or was unreachable)
  or `Sent` (with the provider's submission id). Without an explicit idempotency key, it derives
  one from the seller's endpoint, the invoice number, the recipient and the exact XML, so a retry of
  an unchanged invoice reuses the key and a corrected one gets a new key. How Storecove treats a
  reused key (how long it remembers it, and whether rejected submissions count) is not in its spec
  and is unverified. Tested against a fake Access Point on a loopback HTTP port, through the real
  `StorecoveClient`.

- receives invoices (`BePeppolCommerce.Api`, an ASP.NET Core minimal API). `POST /webhooks/inbound`
  takes a small JSON body naming a received document, fetches that document from the Access Point
  with `GetInboundAsync`, and returns a normalized `InboundInvoice` (invoice number, issue date,
  currency, seller and buyer name and Peppol endpoint, line count, payable amount), parsed by
  `BePeppolCommerce.Core.Inbound.InboundInvoiceParser`. **The webhook body is this project's own
  minimal shape**, `{ "guid": "<document id>" }` (or `document_guid`, the property name Storecove's
  spec mentions), because Storecove's public spec does not define its webhook body. For Recommand ids
  (`doc_...`), which are not GUIDs, use `{ "documentId": "<id>" }`. Recommand's real webhook
  deliveries (an event envelope signed with an HMAC-SHA256 `X-Signature` header) are not parsed or
  verified yet. Match the body to a real delivery before production. The parser does not run the Peppol rules on received
  documents; call `PeppolValidator` for that.
- defines the contract a Configured Commerce extension would implement
  (`BePeppolCommerce.Core.Integration`, documented in `docs/CONFIGURED_COMMERCE_CONTRACT.md`):
  `IOrderInvoiceSource` (a queue of orders to invoice, with an idempotency key fixed at queue time)
  and `IAccessPointSettingsProvider` (provider name, API key, account id at runtime).
  `OutboundDispatcher.RunOnceAsync` drains the queue through `OutboundInvoiceSender` and records each
  result as sent, retryable failure (transport error, timeout, 401, 403, 404, 408, 429, 5xx; this
  ends the run) or permanent failure (validation, other 4xx). It is tested only with the in-memory fakes in the same namespace.

It does **not** yet do the following:

- emit a VAT exemption reason, an order reference or payment terms. Orders that need them (VAT
  category E/Z/O..., no buyer reference, no due date) are built, but the validator then reports
  BR-E-10, PEPPOL-EN16931-R003 or BR-CO-25, so they are caught before sending.
- run inside Optimizely Configured Commerce. The contract above has never been built against a real
  install. The library targets `net8.0`, so it needs a Configured Commerce install whose Extensions
  project has been retargeted to `net8.0`/`net10.0` (documented from build 5.2.2604.725-lts on); installs still on `net48` cannot
  use it (see the contract doc).

## How validation works

Saxon-HE 12.10 (Java, MPL-2.0) runs the Schematron rules. IKVM 8.16.1 compiles its jar into a .NET
assembly at build time. The first build downloads the following into `src/BePeppolCommerce.Core/obj/`:

- two jars from Maven Central
- the two official Schematron files from OpenPEPPOL/peppol-bis-invoice-3 v3.0.20

Each file is checked against a SHA-256 pinned in `BePeppolCommerce.Core.csproj`. A mismatch
deletes the file and fails the build; building again retries. None of these files is committed:
the Peppol rules may not be redistributed (see `Validation/Rules/SOURCE.md`). The ISO Schematron
skeleton (MIT) is committed in `Validation/Skeleton/`. The UBL 2.1 schemas are vendored unmodified
in `Validation/Schemas/UBL-2.1/`, with the OASIS notice.

Measured on the session container (day 028): the first validation in a process takes about 4 to 6 s,
because it compiles the rules. After that, about 25 ms per invoice for all three checks. The build
output is large, about 340 MB in `tests/.../bin`, mostly IKVM runtime images for every platform.

Schematron findings carry an XPath in `Location`. XSD findings carry a line and position when
the input was a string, and an empty `Location` for an `XDocument` built in memory.

## Prerequisites

- .NET 8 SDK (`dotnet --list-sdks` should show an `8.0.x` entry). `global.json` asks for 8.0.100
  or newer and rolls forward to a newer major SDK if that is all you have installed.
- Network access on the first build (NuGet, Maven Central, raw.githubusercontent.com).
  No Java install is needed.

## See it work in one command (the walkthrough)

From this directory (`ideas/be-peppol-commerce/app`):

```
dotnet run --project samples/BePeppolCommerce.Walkthrough
```

It runs the whole v1 flow in one process against in-memory fakes, with no network calls after the
first build and no credentials:

1. loads the fictitious sample order (`tests/BePeppolCommerce.Core.Tests/Fixtures/sample-order.json`)
2. builds the UBL invoice and validates it against the XSD and the official Peppol rules (expect
   `Valid: True (0 errors, 0 warnings)`; this step takes a few seconds the first time in a process)
3. queues it, plus a copy without a buyer reference, in `InMemoryOrderInvoiceSource` and runs
   `OutboundDispatcher` once with a fake Access Point: the sample is sent (submission `fake-0001`),
   the copy fails validation with `PEPPOL-EN16931-R003` and lands in the failed-document log
4. fetches the sent document back from the fake Access Point and parses it with
   `InboundInvoiceParser`, as `POST /webhooks/inbound` does (expect invoice `INV-2026-0001`, 3 lines,
   payable 268.63 EUR)

It ends with `Walkthrough finished: every step ended as expected.` and exit code 0, or prints
`UNEXPECTED:` lines and exits with 1. The first run builds everything (about a minute, see below);
after that it takes about 15 s. `WalkthroughTests` runs the same code as part of `dotnet test`.
The fake Access Point is `samples/BePeppolCommerce.Walkthrough/FakeAccessPoint.cs`.

## Run the tests

From this directory (`ideas/be-peppol-commerce/app`):

```
dotnet test
```

The first run takes about a minute (restore, jar download, IKVM compiling the jars). All 265 tests
should pass (185 in `BePeppolCommerce.Core.Tests`, 80 in `BePeppolCommerce.Api.Tests`). If a download fails (Maven Central sometimes rate-limits with HTTP 429), or a file fails its
SHA-256 check (the file is then deleted), wait a minute and run `dotnet test` again.

## Run the API host

```
dotnet run --project src/BePeppolCommerce.Api
```

It listens on http://localhost:5080 in the Development environment (set by
`Properties/launchSettings.json`). `GET /health` answers `{"status":"ok"}`. Without an Access Point
API key, `POST /webhooks/inbound` answers 503 and a startup warning names the selected provider.
To point it at a provider, set configuration (environment variables shown; see
`src/BePeppolCommerce.Api/.env.example`, placeholders only). The settings bind to typed options
(`AccessPointOptions`, `StorecoveSection`, `RecommandSection` in `AccessPointConfig.cs`). Once the
selected provider has an API key, every invalid value in its section stops the host at startup with
an `OptionsValidationException` that names each bad setting (for example
`Storecove:LegalEntityId must be a positive Storecove legal entity id.`) and never prints its value.
The other provider's section is not checked.

- `AccessPoint__Provider`: `storecove` (the default when unset) or `recommand`. Any other value
  stops the host at startup.
- `Storecove__ApiKey`, `Storecove__LegalEntityId` (a whole number above 0), `Storecove__BaseUri` (defaults to
  `https://api.storecove.com/api/v2/`). A `BaseUri` that is not an absolute https URI (plain http is allowed
  only to loopback) stops the host at startup instead of falling back to the live API.
  `Storecove__SchemeMap__0208=<Storecove scheme name>` maps a Peppol ICD scheme to Storecove's own
  name (keys must be 4-digit ICD codes, values non-blank). The Belgian name is not confirmed, so none
  is set by default and unmapped schemes are sent unchanged.
- With `recommand`: `Recommand__ApiKey`, `Recommand__ApiSecret`, `Recommand__CompanyId`,
  `Recommand__BaseUri` (defaults to `https://app.recommand.eu/`). A missing secret, a company id
  with characters other than letters, digits, `_` and `-`, or a bad `BaseUri` stops the host at
  startup.
- `Webhook__Secret`: requests must send the same value in the `X-Webhook-Secret` header. **Outside
  Development the webhook answers 503 until this is set**, because its response exposes the received
  invoice's parties and amounts. In Development an unset secret disables the check (a warning is
  logged at startup). This header is this project's convention; how Storecove authenticates its
  webhooks is not in its public spec, so check before relying on it.
- `Webhook__SigningSecret` (Recommand): when set, a request with a valid `X-Signature: sha256=<hex>`
  header (HMAC-SHA256 of the raw body under this secret, the format Recommand documents for signed
  deliveries) is accepted without `X-Webhook-Secret`. Either secret satisfies the 503 rule outside
  Development. Recommand's real delivery body (its event envelope) is still not parsed: the body must
  carry a document id as below. **Do not register this endpoint as a real Recommand webhook yet.**
  Recommand sends every team event (sent, delivery status, labels, received) to one URL and the
  handler does not filter on `eventType`, so non-received events would be rejected or fetched and
  fail. There is no replay protection: a captured signed request can be resent (Recommand documents
  no timestamp header).

This host has never been connected to a real Storecove or Recommand account. Example call:

```
curl -X POST http://localhost:5080/webhooks/inbound -H "Content-Type: application/json" \
  -d '{"guid":"0b6f2a3c-1d4e-4f5a-8b9c-0d1e2f3a4b5c"}'
```

Responses: 200 with `{ providerDocumentId, invoice }`; 400 if the body is not a JSON object whose
`guid`/`document_guid` is a GUID string or whose `documentId` is a GUID or a Recommand-style id (with Storecove configured, a non-GUID `documentId` passes this check but the client refuses it, so the answer is 502); 401 on a wrong or missing secret or signature (when configured); 413
above 16 KB; 502 if the Access Point fetch failed (details are logged, not returned); 422 if the
fetched document is not a parseable UBL Invoice (credit notes are not handled yet); 503 if no
provider is configured, or outside Development if neither secret is configured.

Received documents are parsed with DTDs prohibited and a cap of 10 million characters
(`PeppolValidator.MaxDocumentCharacters`, a defensive limit chosen here, not a Peppol rule).

## Failures and logging

Every failed send or receive is recorded in two places (outbound, after the source's `MarkFailedAsync`;
if that call throws, the exception escapes and neither record is written). Neither contains the invoice XML or a
credential, though details can quote short fragments: an XML parser message names elements, and an
unexpected exception's message is recorded as is.

- An `IFailedDocumentLog` (`Core/Integration/FailedDocumentLog.cs`): time, direction, document id
  (the host's source id outbound, the Access Point's id inbound), reason, retryable, details. The
  API host registers `InMemoryFailedDocumentLog` (newest 1000 entries, lost on restart, not exposed
  over HTTP); a production host should register its own durable implementation. The dispatcher takes
  one as an optional constructor argument. If writing to it throws, that is logged as event 9001
  and the run continues.
- A log event with a stable `EventId` (`PeppolLogEvents`):

| Id | Name | Level | When |
| --- | --- | --- | --- |
| 1000 | OutboundSent | Information | the Access Point accepted an invoice |
| 1001 | OutboundValidationFailed | Error | the invoice failed Peppol validation and was not sent |
| 1002 | OutboundSendFailedRetryable | Warning | provider down, timeout, 401/403/404, 408, 429, 5xx, Recommand 422 with category `transport` |
| 1003 | OutboundSendFailedPermanent | Error | provider rejected the invoice; a Recommand 422 with category `recipient_not_found` or `document_not_supported` has reason "Recipient not reachable on Peppol" |
| 1004 | OutboundUnexpectedError | Error | an exception from the send path; logged with the exception |
| 2001 | InboundFetchFailed | Warning | fetching a received document from the Access Point failed |
| 2002 | InboundParseFailed | Warning | the fetched document is not a parseable UBL Invoice |
| 2003 | InboundPayloadRejected | Warning | webhook body too large or without a usable document id (logged, not recorded) |
| 2004 | InboundUnauthorized | Warning | wrong or missing webhook secret or signature (logged, not recorded) |
| 9001 | FailureRecordFailed | Error | the failed-document log threw |

## Publish

```
dotnet publish src/BePeppolCommerce.Api -p:PublishProfile=linux-x64
```

Output goes to `src/BePeppolCommerce.Api/bin/publish/linux-x64/` and is framework-dependent (the
target machine needs the ASP.NET Core 8 runtime). Measured day 031: about 92 MB, against about
340 MB for a publish without a RuntimeIdentifier (IKVM ships images for every platform). The
published host started and answered `/health`; the validator has not yet been exercised from a
published build. For Windows, copy the profile and set `win-x64`.

## Layout

```
BePeppolCommerce.sln
src/BePeppolCommerce.Core/          library: AccessPoint/ (provider interface, Storecove and Recommand clients), Integration/ (Configured Commerce contract, dispatcher, fakes), Outbound/ (build, validate, send), Model/ (order records, JSON parsing), Ubl/ (invoice builder),
                                    Validation/ (validator, Skeleton/ ISO Schematron, Schemas/ UBL 2.1 XSD,
                                    Rules/SOURCE.md provenance of the downloaded rules)
src/BePeppolCommerce.Core/Inbound/  received-invoice parser and normalized model
src/BePeppolCommerce.Api/           ASP.NET Core host: /health and POST /webhooks/inbound
samples/BePeppolCommerce.Walkthrough/  console walkthrough of the whole flow against fakes
tests/BePeppolCommerce.Core.Tests/  xUnit tests; Fixtures/sample-order.json is the sample order
tests/BePeppolCommerce.Api.Tests/   host tests through WebApplicationFactory with a fake Access Point,
                                    and startup configuration tests (AccessPointConfigTests)
docs/CONFIGURED_COMMERCE_CONTRACT.md  what a Configured Commerce extension implements
```

All party data in the fixture is fictitious. There are no secrets or credentials anywhere in this
project. `src/BePeppolCommerce.Api/.env.example` lists the configuration keys with placeholder values.
