# BePeppolCommerce

A .NET 8 library that turns an order record into a Peppol BIS Billing 3.0 UBL invoice. It is being
built as the engine for a future Optimizely Configured Commerce connector for Belgian e-invoicing.
The idea and its evidence are in `../VALIDATION.md`, and the full plan is in `../PLAN.md`.

**Status: Slice 1 of 11 (walking skeleton).** Right now it:

- parses an order from JSON (`BePeppolCommerce.Core.Model.OrderJson`)
- builds a well-formed UBL 2.1 Invoice XML with the Peppol BIS 3.0 `CustomizationID`/`ProfileID`,
  seller/buyer parties (Belgian `0208` endpoint scheme), per-rate VAT breakdown, totals and lines
  (`BePeppolCommerce.Core.Ubl.PeppolInvoiceBuilder`)

It does **not** yet do the following:

- validate against the UBL XSD or the EN16931/Peppol Schematron rules (that is Slices 2a/2b). The
  output is well-formed XML only. It has not been confirmed as compliant, so do not send it to a
  real Access Point.
- send or receive anything. There is no Access Point client or API host yet.
- integrate with Optimizely Configured Commerce.

## Prerequisites

- .NET 8 SDK (`dotnet --list-sdks` should show an `8.0.x` entry). `global.json` asks for 8.0.100
  or newer and rolls forward to a newer major SDK if that is all you have installed.

## Run the tests

From this directory (`ideas/be-peppol-commerce/app`):

```
dotnet test
```

The first run restores NuGet packages (xUnit), so it needs network access. All 14 tests should
pass.

## Layout

```
BePeppolCommerce.sln
src/BePeppolCommerce.Core/          library: Model/ (order records, JSON parsing), Ubl/ (invoice builder)
tests/BePeppolCommerce.Core.Tests/  xUnit tests; Fixtures/sample-order.json is the sample order
```

All party data in the fixture is fictitious. There are no secrets or credentials anywhere in this
project, and no `.env` file is needed yet.
