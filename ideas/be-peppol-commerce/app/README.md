# BePeppolCommerce

A .NET 8 library that turns an order record into a Peppol BIS Billing 3.0 UBL invoice. It is being
built as the engine for a future Optimizely Configured Commerce connector for Belgian e-invoicing.
The idea and its evidence are in `../VALIDATION.md`, and the full plan is in `../PLAN.md`.

**Status: Slices 1, 2a, 2b and 3 of 11 done.** Right now it:

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
  define the webhook body.

It does **not** yet do the following:

- emit a VAT exemption reason, an order reference or payment terms. Orders that need them (VAT
  category E/Z/O..., no buyer reference, no due date) are built, but the validator then reports
  BR-E-10, PEPPOL-EN16931-R003 or BR-CO-25, so they are caught before sending.
- send or receive anything end to end. The provider client is not yet wired to the builder and
  validator (Slice 4), and there is no API host for inbound webhooks yet (Slice 5).
- integrate with Optimizely Configured Commerce.

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

## Run the tests

From this directory (`ideas/be-peppol-commerce/app`):

```
dotnet test
```

The first run takes about a minute (restore, jar download, IKVM compiling the jars). All 58 tests
should pass. If a download fails (Maven Central sometimes rate-limits with HTTP 429), or a file fails its
SHA-256 check (the file is then deleted), wait a minute and run `dotnet test` again.

## Layout

```
BePeppolCommerce.sln
src/BePeppolCommerce.Core/          library: AccessPoint/ (provider interface, Storecove client), Model/ (order records, JSON parsing), Ubl/ (invoice builder),
                                    Validation/ (validator, Skeleton/ ISO Schematron, Schemas/ UBL 2.1 XSD,
                                    Rules/SOURCE.md provenance of the downloaded rules)
tests/BePeppolCommerce.Core.Tests/  xUnit tests; Fixtures/sample-order.json is the sample order
```

All party data in the fixture is fictitious. There are no secrets or credentials anywhere in this
project, and no `.env` file is needed yet.
