# BePeppolCommerce

A .NET 8 library that turns an order record into a Peppol BIS Billing 3.0 UBL invoice. It is being
built as the engine for a future Optimizely Configured Commerce connector for Belgian e-invoicing.
The idea and its evidence are in `../VALIDATION.md`, and the full plan is in `../PLAN.md`.

**Status: Slices 1, 2a and 2b of 11 done.** Right now it:

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

It does **not** yet do the following:

- emit a VAT exemption reason, an order reference or payment terms. Orders that need them (VAT
  category E/Z/O..., no buyer reference, no due date) are built, but the validator then reports
  BR-E-10, PEPPOL-EN16931-R003 or BR-CO-25, so they are caught before sending.
- send or receive anything. There is no Access Point client or API host yet.
- integrate with Optimizely Configured Commerce.

## How validation works

Saxon-HE 12.10 (Java, MPL-2.0) runs the Schematron rules. IKVM 8.16.1 compiles its jar into a .NET
assembly at build time. The first build downloads two jars from Maven Central into
`src/BePeppolCommerce.Core/obj/jars/` and checks their SHA-256 against the values pinned in
`BePeppolCommerce.Core.csproj`. A mismatch deletes the file and fails the build; building again
retries. The jars are never committed.

The Schematron is committed pre-compiled as XSLT in `src/BePeppolCommerce.Core/Validation/Rules/`
(see `SOURCE.md` there for the upstream tag, licences and how to regenerate it with
`tools/RulesGen`). The UBL 2.1 schemas are vendored unmodified in `Validation/Schemas/UBL-2.1/`
with the OASIS notice.

Measured on the session container (day 028): the first validation in a process takes about 3 to 4 s
(stylesheet compilation); after that about 25 ms per invoice for all three checks. The build output
is large (about 340 MB in `tests/.../bin`, mostly IKVM runtime images for every platform).

## Prerequisites

- .NET 8 SDK (`dotnet --list-sdks` should show an `8.0.x` entry). `global.json` asks for 8.0.100
  or newer and rolls forward to a newer major SDK if that is all you have installed.
- Network access on the first build (NuGet packages, and the two Saxon jars from Maven Central).
  No Java install is needed.

## Run the tests

From this directory (`ideas/be-peppol-commerce/app`):

```
dotnet test
```

The first run takes about a minute (restore, jar download, IKVM compiling the jars). All 38 tests
should pass. If the build fails with a SHA-256 mismatch, Maven Central probably rate-limited the
download (it answers HTTP 429 with a short text body); run `dotnet test` again.

## Layout

```
BePeppolCommerce.sln
src/BePeppolCommerce.Core/          library: Model/ (order records, JSON parsing), Ubl/ (invoice builder),
                                    Validation/ (validator, Rules/ XSLT, Schemas/ UBL 2.1 XSD)
tests/BePeppolCommerce.Core.Tests/  xUnit tests; Fixtures/sample-order.json is the sample order
tools/RulesGen/                     maintainer tool to regenerate Validation/Rules (not in the solution)
```

All party data in the fixture is fictitious. There are no secrets or credentials anywhere in this
project, and no `.env` file is needed yet.
