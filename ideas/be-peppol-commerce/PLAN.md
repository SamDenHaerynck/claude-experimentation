# Plan: BE-Peppol-Commerce

Day 025, Phase 2 (Plan). Written after `VALIDATION.md` (Deep Check, 20/25, no hard disqualifier)
and `VALIDATION_KIT.md`. No `SIGNALS.md` yet — the owner has not run the validation kit as of this
session; planning and building proceed without waiting for it, per `OWNER.md`.

## Reality check that shapes this plan

Optimizely Configured Commerce is a commercial, licensed .NET platform. This routine has no license,
no instance, and (per the non-negotiables) may never deploy anything or create third-party
accounts. A real install against a live Configured Commerce tenant is therefore impossible from
inside this repo, in this session or any future one, without the owner doing it themselves.

So v1 is scoped as: **a standalone, fully testable .NET class library and thin API that does the
real work** (build a compliant Peppol invoice, validate it, send it through a real AP provider's
documented API shape, receive and parse an inbound one) **plus a documented, code-level extension
contract** showing exactly what a real Configured Commerce extension would call. The Configured
Commerce side itself is a documented interface and a stub adapter, not a live integration — this is
called out explicitly below and in `HANDOFF.md` later, not glossed over.

## Core user flow (v1)

1. An operator configures one Peppol Access Point (AP) provider's API base URL and API key via
   local config (`appsettings.json` / environment variables, `.env.example` only — no live keys ever
   committed).
2. Given an order/invoice record (from a fixture in v1; from Configured Commerce's own order object
   in a real install), the library maps it to a Peppol BIS Billing 3.0 UBL invoice XML.
3. The XML is validated against the EN16931 Schematron/schema rules before anything is sent.
   A document that fails validation is never sent.
4. The validated XML is sent through an `IPeppolAccessPointClient` abstraction to the configured AP
   provider's API. v1 tentatively targets two concrete provider clients behind this interface,
   **Recommand** and **Storecove** — `VALIDATION.md`'s sources confirm both clear H2 (client-of-an-
   AP, no accreditation needed) and give pricing, but do NOT establish that either's public docs
   describe payload/webhook shapes in enough detail to build a fake against; that is unconfirmed
   until Slice 3's checkpoint (see Slice 3) — both exercised only against fakes/mocks in tests, no
   live provider account, no live key.
5. Inbound Peppol documents (as an AP provider would deliver them — webhook payload or poll
   response) are received on a minimal ASP.NET Core endpoint, parsed into a normalized internal
   model, and exposed for a caller (a real Configured Commerce extension, in production) to consume.

## Out of scope for v1

- Building or hosting our own Peppol Access Point (H2 was cleared on the basis that this product is
  always a *client* of an existing accredited AP — that stays true here; no AP logic of our own).
- Any live install, deployment, or third-party account against a real Optimizely Configured Commerce
  instance, or against a real AP provider account. Forbidden by the non-negotiables regardless of
  buildability.
- More than two AP provider client implementations. The interface is designed so a third (Qvalia,
  Billit) is a bounded addition, but only two ship built and tested in v1.
- A full accounting/reconciliation UI, ledger, or invoice archive/search UI.
- Multi-country tax logic. Belgian VAT fields only; the EN16931 model supports more, but v1 does not
  populate or test other countries' rules.
- Credential/secret management UI (key vault integration, per-tenant credential rotation). Config
  file/env vars only.
- Webhook hardening: retries-with-backoff, dead-letter queues, at-least-once delivery guarantees.
  v1 handles the synchronous happy path and records failures to a log; it does not build durable
  retry infrastructure.
- OAuth or any user-facing auth. This is a backend library/service; no UI, no login.

## Stack

**C#/.NET 8**, class library + xUnit test project + a thin ASP.NET Core minimal-API host for the
inbound endpoint. Justification: Optimizely Configured Commerce is itself a .NET/ASP.NET Core
platform, so a connector meant to eventually run as (or alongside) a Configured Commerce extension
must be .NET to have any real installation path for the owner. .NET 8 is Microsoft's current LTS,
runs locally with a single `dotnet test` / `dotnet run` command, and needs no exotic dependencies
for XML/HTTP handling (BCL / `System.Net.Http`). **EN16931 Schematron validation (settled day 027, Slice 2a):** .NET's built-in
`XslCompiledTransform` is XSLT 1.0 only, and the official rules need XSLT 2.0. v1 uses Saxon-HE 12.5
(the Java build, MPL-2.0) compiled to a .NET assembly by IKVM (NuGet `IKVM`, zlib licence plus
OpenJDK's GPLv2 with Classpath exception). This gives full coverage of the official CEN and Peppol
rule sets, checked on day 027 in `spikes/2a-schematron/`. The cost is a heavy dependency. The spike's `bin/` is 337 MB, mostly
IKVM's `ikvm/` images and per-platform `runtimes/`, and the IKVM packages take about 3.3 GB of NuGet
cache. The first build also takes about a minute longer. Trimming this with a `RuntimeIdentifier`
is a 2b task. SaxonCS was rejected because NuGet only has 12.x, which
needs a paid licence key. SaxonCS-HE 13.0 is a free download from Saxonica. It was checked on day 027
and ships only as a self-contained command-line executable (131 MB for Linux), with no library DLL
and no NuGet package. It lost to in-process Saxon because it would mean one process per invoice and
one binary per OS. Its licence was not checked. Reconsider if a SaxonCS-HE library package appears. Details are in `DECISIONS.md` (day 027). No frontend
framework is needed for v1 — there is no UI.

## Slices

Status: Slices 1 (day 026), 2a (day 027), 2b (day 028), 3 (day 029), 4 (day 030), 5 (day 031) and 6 (day 032) complete. Next: Slice 7.

Each slice leaves `dotnet test` (and, from Slice 5 on, `dotnet run` for the API host) green.

1. **Walking skeleton.** *(Done, day 026: 14 xUnit tests, `dotnet test` green.)* Solution with `BePeppolCommerce.Core` (class library) and
   `BePeppolCommerce.Core.Tests` (xUnit). One sample order JSON fixture. One test: build a minimal,
   well-formed Peppol BIS UBL invoice XML from the fixture (structurally valid XML, not yet schema
   validated). `app/README.md` documents `dotnet test`.
2a. **Validation spike.** Time-boxed investigation (this is the whole slice if it runs long): try
   SaxonCS (or another XSLT 2.0/3.0-capable .NET option) against the official EN16931/Peppol BIS3
   Schematron rules on the Slice-1 fixture. Decide and record in `DECISIONS.md`: full Schematron
   coverage via a real XSLT 2.0/3.0 processor, or a documented fallback of partial rule coverage
   hand-implemented in C# (explicitly named as partial, never presented as full compliance). Either
   answer is an acceptable slice outcome; "no viable offline .NET path found, falling back to
   partial coverage" is a valid, honest result — do not let this slice balloon chasing full coverage
   if the spike says otherwise.
   *(Done, day 027: full Schematron via IKVM + Saxon-HE 12.5 works on .NET 8. Fixture passes CEN
   1.3.15, Peppol 3.0.20 and the UBL 2.1 XSD, and a broken copy fails with the expected rule IDs. See
   `spikes/2a-schematron/README.md`.)*
2b. **Wire in validation.** Wire the Slice-2a decision into the library so Slice 1's generated XML
   is checked before anything downstream trusts it. Slice 1's fixture must pass; add a fixture that
   is deliberately invalid and assert it fails validation with a clear error.
   **Approach, decided in 2a:** (1) Add `IKVM` to `BePeppolCommerce.Core` and get the Saxon-HE 12.5
   jar (plus `xmlresolver` 5.2.2) in this order. First try `IKVM.Maven.Sdk` `MavenReference`, but only
   for about 5 minutes: in the sandbox its Java trust store rejects the proxy CA. Otherwise use an
   MSBuild `DownloadFile` target, placed before `ResolveReferences`, that downloads the pinned jars
   from Maven Central into `obj/` and feeds them to `IkvmReference`. That keeps `dotnet test` a single
   command. Never commit the jars. (2) Commit the XSLT pre-compiled from the v3.0.20 `.sch` files
   under `Validation/Rules/`, with a `SOURCE.md` giving the upstream repo, tag, commit `261c458`, the
   ISO skeleton commit `77dcd36`, and the EUPL-1.2 notice for the CEN rules. Also commit the
   generator (a script or test-only helper), so the next upstream release can be regenerated. (3)
   Compile each XSLT once per process into a cached `XsltExecutable` and create a new transformer per
   call. (4) Return a result type with the failed asserts (id, flag, text, location), treating
   `flag="fatal"` as blocking and `warning` as non-blocking. Validate against the UBL 2.1 XSD first,
   using vendored OASIS XSDs (the whole `xsd/` tree of UBL-2.1.zip is about 4.9 MB; vendor only the files
   `UBL-Invoice-2.1.xsd` transitively imports), only if that is reasonable; otherwise skip the XSD step and
   record that. (5) Carry over the day-026 lows: JSON nulls in required strings pass
   `OrderJson.Parse`; there is no BuyerReference/OrderReference or PaymentTerms fallback; non-S VAT
   categories have no exemption reason. Test (5) against the real Schematron now: for example, an
   order without a buyer reference must fail PEPPOL-EN16931-R003.
   Also from the day-027 review: (6) Harden parsing against XXE. Parse input with a .NET `XmlReader`
   (`DtdProcessing.Prohibit`, `XmlResolver = null`) and pass Saxon the parsed tree, or set the
   parser's security features. Add a test with an external-entity payload. (7) Pin a SHA-256 for
   each downloaded jar and fail on mismatch, and document that the first build needs network access.
   (8) Pin the latest Saxon-HE 12.x (12.5 was an arbitrary choice). Saxon 13 on IKVM (Java SE 8)
   is unverified and may be a long-term ceiling. (9) Add the upstream `rules/examples` files as
   conformance tests, because the ISO skeleton is not Peppol's own build tooling. (10) Measure warm
   per-invoice latency, add a parallel-validation test, and never share `XsltCompiler` across
   threads. (11) Trim `runtimes/` output with a `RuntimeIdentifier`.
   *(Done, day 028: `PeppolValidator` runs a root-element guard, then the vendored UBL 2.1 XSD, then
   CEN 1.3.15 and Peppol 3.0.20, with Saxon-HE 12.10 via IKVM. The jars and the two upstream `.sch`
   files are downloaded at build time with pinned SHA-256; the `.sch` files are compiled in memory
   on first use and never committed, because OpenPeppol restricts redistribution (see
   `app/src/BePeppolCommerce.Core/Validation/Rules/SOURCE.md`). 40 tests pass. They cover R003,
   BR-CO-25, BR-E-10, the XSD, XXE (string input and an `XDocument` carrying a DOCTYPE), parallel
   runs, and the required-string null checks. All nine upstream `rules/examples` files pass locally
   with zero findings; they are not vendored, for the same licence reason. (11) is deferred to
   Slice 5. Warm latency is about 25 ms per invoice.)*
3. **Provider abstraction.** Define `IPeppolAccessPointClient` (send outbound document, receive
   inbound document/list). **First checkpoint:** confirm Recommand's and Storecove's public docs
   (no signup) actually describe request/response/webhook payload shapes in enough detail to build
   a believable fake against — `VALIDATION.md`'s evidence for both is about accreditation, not API
   payload shape, so this is not yet confirmed. Recommand and Storecove are the default pick over
   Billit (also cleared in `VALIDATION.md`, and with a real shipped precedent via Woo2Billit) on an
   **unverified working assumption, not a cited fact**: that a smaller, developer-tooling-branded
   provider is more likely to publish a self-serve API reference than Billit, whose `VALIDATION.md`
   citations are pricing/marketing pages, not developer docs. This slice's checkpoint is exactly
   where that assumption gets tested — confirm or refute it first, with a real fetched URL either
   way, before writing any client code; if the checkpoint finds either provider's docs too thin,
   fall back to Billit or the other uninvestigated provider before losing session time
   reverse-engineering an undocumented shape.
   Implement the first concrete client against a mocked `HttpMessageHandler` — no live endpoint, no
   live key. Tests cover a success response and an error response.
   *(Done, day 029. Checkpoint: Storecove publishes a machine-readable Swagger 2.0 spec at
   https://api.storecove.com/api/v2/openapi.json, fetched with curl and read directly. It defines
   `POST /document_submissions` (body `DocumentSubmission` with `legalEntityId`, `idempotencyGuid`,
   `routing.eIdentifiers[{scheme,id}]`, `document.rawDocumentData{document (base64), parseStrategy:
   "ubl"}`; 200 returns `{guid}`, 422 returns an array of `ErrorModel{source,details}`), Bearer auth,
   and `GET /received_documents/{guid}/{format}` returning `Transportable{guid, direction, original}`.
   The spec does not define the `received_document` webhook body beyond naming its `document_guid`
   property, and does not say how `original` is encoded. The client accepts raw XML or base64 and
   this must be checked against a real response. Recommand: https://docs.recommand.eu/ links an API
   reference generated from an OpenAPI spec. A WebFetch summary of
   https://docs.recommand.eu/docs/sending-invoices described `POST /api/v1/{companyId}/send` with Basic
   auth, but that is a model summary and was not read raw. Recommand is the second provider (Slice 7),
   and its docs must be re-read raw first. Storecove was built first because its spec could be read
   directly. Built: `AccessPoint/IPeppolAccessPointClient.cs` (interface, result and error types) and
   `AccessPoint/StorecoveClient.cs`, with 23 tests in `StorecoveClientTests.cs` against a stub
   `HttpMessageHandler`. Provider errors and transport errors come back as failed results; null
   arguments and caller cancellation throw. 63 tests pass. Also unverified: Storecove uses its own
   identifier scheme names (the spec's examples are "DE:VAT" and "FR:CTC"), and it points to an
   external list for the rest, so the name for Belgian `0208` is unknown. `StorecoveOptions.SchemeMap`
   carries the mapping, and Slice 8 must fill it from Storecove's receiver identifiers list.)*
4. **Outbound flow end to end.** Wire fixture order → XML → validate → send through the Slice-3
   client, against an in-memory fake AP server (e.g. `WebApplicationFactory`/`HttpListener` test
   double), asserting the correct request shape reaches the fake server and a failure short-circuits
   before sending.
5. **Inbound flow.** Minimal ASP.NET Core endpoint that accepts an inbound Peppol document payload
   (shape modeled on the chosen provider's documented webhook/poll format), parses it into a
   normalized `InboundInvoice` model, with tests against fixture payloads (valid and malformed).
   Also (carried from 2b item 11): set a `RuntimeIdentifier` on the host's publish profile so
   IKVM's per-platform `runtimes/` images are trimmed, and record the published size. Cap the
   inbound payload size (for example `MaxCharactersInDocument` in `PeppolValidator.ParseUntrusted`
   plus a request size limit), because validation builds two full trees (day-028 review).
   *(Done, day 031: `Core/Inbound/InboundInvoiceParser` returns a normalized `InboundInvoice` or a
   failure result (malformed, DTD, over `PeppolValidator.MaxDocumentCharacters` = 10M chars, non-Invoice
   root, missing fields). `src/BePeppolCommerce.Api` serves `GET /health` and `POST /webhooks/inbound`
   (body `{ "guid" | "document_guid" }`, a minimal shape of our own because Storecove's spec does not
   define the webhook body; value must be a GUID; 16 KB bounded read; shared-secret header, our own
   convention, required outside Development). 27 host tests via `WebApplicationFactory` with a fake
   Access Point, 14 parser tests; 114 total pass.
   `Properties/PublishProfiles/linux-x64.pubxml` cuts the publish from about 340 MB to 92 MB. Not done:
   credit notes are rejected (422) by the inbound parser; inbound documents are parsed, not validated;
   the validator has not been run from a published build.)*
6. **Configured Commerce extension contract.** First, check Optimizely's public docs for the target framework of Configured Commerce extensions.
   The day-027 reviewer said, unverified, that it has historically been .NET Framework 4.8. If so,
   multi-target `BePeppolCommerce.Core` (IKVM supports net472). Document and stub (interfaces + a fake in-memory
   implementation, not a real plugin) the two integration points a real Configured Commerce
   extension would implement: an order/invoice-completion source (`IOrderInvoiceSource`) and a
   credential/config provider. This slice is done only when the doc covers, concretely, for each
   interface: full method signatures with parameter/return types; lifetime and threading
   expectations (singleton vs. per-request, thread-safety); the error contract (what throws vs. what
   returns a failure result, and for which conditions); and where credentials are expected to come
   from at runtime. A description in prose without these four elements does not satisfy this slice.
   *(Done, day 032. Target framework: Optimizely's migration guide
   (https://docs.optimizely.com/configured-commerce/docs/net7-framework-to-net-migration, fetched raw)
   says `Extensions.csproj` ships `net48` only and can be retargeted to `net8.0` (or `net48;net8.0`)
   on releases 5.2.2512 to 5.2.2604 and to `net10.0` (or `net48;net10.0`) from 5.2.2605; its steps
   start from build 5.2.2604.725-lts or newer. Market risk: how many installs have migrated is
   unknown (no evidence found). So the
   library stays `net8.0`; `net48` multi-targeting is out of scope for v1 and must be stated in
   `HANDOFF.md` (installs on `net48` cannot use it). Built `Integration/`: `IOrderInvoiceSource`
   (queue with an idempotency key fixed at queue time; `MarkSentAsync`/`MarkFailedAsync`),
   `IAccessPointSettingsProvider`, `AccessPointClientFactory`, `OutboundDispatcher` (drains the queue,
   classifies failures as retryable or permanent), and in-memory fakes. Contract doc:
   `app/docs/CONFIGURED_COMMERCE_CONTRACT.md`, with the four elements per interface and the
   Configured Commerce side's unverified points listed (order-submit handler chain, order mapping
   API, queue storage, job scheduling, settings API). 21 new tests, 135 total. The first review's findings (timeout handling, outage stopping the run,
   retry ordering) are in the day-032 log.)*
7. **Second provider.** Implement the second client (whichever of Recommand/Storecove wasn't built
   in Slice 3) behind the same `IPeppolAccessPointClient` interface, proving the abstraction holds
   without changes to Slices 1-2 and 4-6. Config-driven provider selection (e.g. an enum/string in
   `appsettings.json`).
8. **Config and credential handling.** Strongly-typed settings model bound from
   `appsettings.json`/env vars, `.env.example` with placeholder values only, and validation that
   surfaces a clear error for missing/invalid provider config at startup rather than failing deep in
   a send call.
9. **Error handling and observability.** Structured logging for validation failures, send failures,
   and malformed inbound payloads; a simple failed-document record (in-memory/log-based, not a
   database) so a failure is never silently dropped. Tests assert the failure path is recorded.
10. **Polish and walkthrough.** A documented end-to-end sample (script or `dotnet run` command) that
    generates, validates, and mock-sends one sample invoice, then mock-receives one inbound
    document, entirely against fakes — runnable by a stranger in under five minutes per
    `app/README.md`. Fix any drift between docs and code found while writing it.

Slice 2 is already split into 2a/2b for exactly this reason (11 slices total, 1 under the 12-session
cut-scope trigger). If Slice 2a's spike runs long or another slice proves to be two sessions of
work, split it in `PLAN.md` rather than overrunning a session. If schedule risk has to be absorbed
without adding sessions, compress in this order first: Slice 7 (second provider — one provider still
meets "at least two" if cut to one and the done criteria bullet is revised down explicitly, not
silently) and Slice 9 (error handling — reduce to logging only, drop the failed-document record)
before touching Slices 1-6, which are load-bearing for the done criteria.

## Done criteria (v1 handoff bar for this app specifically)

- Given the sample order fixture, the library builds a Peppol BIS Billing 3.0 UBL invoice XML that
  passes EN16931 validation per whatever coverage Slice 2a's spike settles on (full Schematron via
  a real XSLT 2.0/3.0 processor, or a documented partial rule set — either is acceptable if named
  honestly), automated and passing in CI-equivalent (`dotnet test`).
- That XML is sent through the provider abstraction to a fake AP endpoint and the success path is
  exercised by an automated test; a validation failure demonstrably never reaches the send step.
- A mock inbound Peppol document is received and parsed into the normalized model, exercised by an
  automated test, including a malformed-payload case that fails cleanly.
- At least two AP provider client implementations exist behind `IPeppolAccessPointClient`, each
  tested only against fakes/mocks — no live provider account or key is created or used at any point.
- The Configured Commerce extension contract (Slice 6) satisfies the four-element checklist in
  Slice 6 (signatures, lifetime/threading, error contract, credential source) — not a vibe-based
  "clear enough" judgment call.
- `app/README.md` lets a stranger run `dotnet test` and the Slice 10 walkthrough in under five
  minutes, with no live credentials anywhere in the repo.
- Every gap above the "real Configured Commerce install" line (stubbed vs. real) is stated plainly
  in `HANDOFF.md` when this ships — never implied to be more finished than it is.

## Critic review

Run day 025, `general-purpose` subagent, adversarial brief (buildability, honesty of
real-vs-stubbed, provider doc depth, slice sequencing, falsifiability of done criteria). Findings
and what changed, most severe first (full findings in the day log):

1. **High — accepted, changed.** Slice 2 (EN16931 Schematron validation) was underscoped and the
   Stack section stated a settled answer that wasn't settled: .NET's `XslCompiledTransform` is
   XSLT 1.0 only and real Schematron rules need XSLT 2.0/3.0. Split into Slice 2a (spike, decide and
   record the approach, partial coverage is an acceptable honest outcome) and 2b (wire it in). Stack
   section rewritten to state this as open, not settled.
2. **Medium — accepted, changed.** Recommand/Storecove were asserted as good picks for fakeable
   docs on the strength of evidence that was actually about accreditation, not payload shape. Added
   an explicit first checkpoint in Slice 3 to confirm doc depth, a stated reason for picking them
   over Billit, and a fallback if the checkpoint fails.
3. **Medium — accepted, changed.** Slice 6's "documented clearly enough" bar was unfalsifiable.
   Replaced with a concrete four-element checklist (signatures, lifetime/threading, error contract,
   credential source), echoed in the Done criteria bullet.
4. **Low/medium — accepted, changed.** No stated order for absorbing schedule risk if Slices 2-3
   overrun. Added an explicit compression order (Slice 7 then Slice 9 before Slices 1-6).
5. **Low — accepted, changed.** The "at least two providers" done-criteria bullet didn't repeat the
   fakes-only qualifier stated elsewhere. Appended it directly to that bullet.
6. **Low — accepted, changed.** Recommand/Storecove over Billit was an unstated judgment call.
   Addressed by finding 2's fix (reason stated in Slice 3).

The critic also confirmed: no fabricated market/revenue data in this file; Slice 1 is a genuine
walking skeleton; validation-before-provider-abstraction sequencing (2 before 3) is correct as
written — the real risk is Slice 2 overrunning and dragging Slice 3 down with it, not misordering,
which the schedule-risk note above now addresses.
