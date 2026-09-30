# Where these rules come from

`CEN-EN16931-UBL.xslt` and `PEPPOL-EN16931-UBL.xslt` are generated, not hand-written. Do not edit
them; regenerate them.

| | |
|---|---|
| Upstream repo | https://github.com/OpenPEPPOL/peppol-bis-invoice-3 |
| Tag | `v3.0.20` (commit `261c458474e27d58a25be629cccac28883171c92`) |
| Inputs | `rules/sch/CEN-EN16931-UBL.sch` (CEN Schematron 1.3.15) and `rules/sch/PEPPOL-EN16931-UBL.sch` (Peppol BIS Billing 3.0.20) |
| Compiler | ISO Schematron skeleton, https://github.com/Schematron/schematron commit `77dcd36`, `trunk/schematron/code/` |
| Processor | Saxon-HE 12.10 (via IKVM, see `../../BePeppolCommerce.Core.csproj`) |
| Generated | 2026-09-30 (day 028) |

## Licences

- `CEN-EN16931-UBL.sch` states in its header that it is licensed under the European Union Public
  Licence (EUPL) version 1.2. The generated XSLT is a derived work of it.
- `PEPPOL-EN16931-UBL.sch` states in its header that it uses CEN/EN16931-1 business terms,
  reproduced with permission from CEN, and that CEN bears no liability and gives no warranties. It
  states no licence of its own, and the upstream repo has no LICENSE file. Its redistribution terms
  are therefore **not confirmed**; see "Notes for owner" in `STATE.md`.
- The ISO Schematron skeleton is MIT-licensed per its own file headers.

## Regenerate (Linux, .NET 8 SDK on PATH, from `ideas/be-peppol-commerce/app`)

```
git clone https://github.com/Schematron/schematron.git /tmp/schematron && git -C /tmp/schematron checkout -q 77dcd36
git clone --depth 1 --branch v3.0.20 https://github.com/OpenPEPPOL/peppol-bis-invoice-3.git /tmp/peppol-bis-invoice-3
R=src/BePeppolCommerce.Core/Validation/Rules
for s in CEN-EN16931-UBL PEPPOL-EN16931-UBL; do
  dotnet run --project tools/RulesGen -- /tmp/schematron/trunk/schematron/code /tmp/peppol-bis-invoice-3/rules/sch/$s.sch $R/$s.xslt
done
dotnet test
```

For a new upstream release, change the tag, regenerate, update this file, and run the upstream
`rules/examples/*.xml` through `PeppolValidator` (on day 028 all nine passed with zero findings).
