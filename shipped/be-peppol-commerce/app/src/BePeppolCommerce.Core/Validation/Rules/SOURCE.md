# Where the validation rules come from

Nothing derived from the Peppol rules is committed here. The build downloads the two official
Schematron files into `obj/sch/`, checks each one against a pinned SHA-256, and embeds them in
the assembly. `PeppolValidator` compiles them to XSLT in memory the first time it runs, which takes
a few seconds.

| | |
|---|---|
| Upstream repo | https://github.com/OpenPEPPOL/peppol-bis-invoice-3 |
| Tag | `v3.0.20` (commit `261c458474e27d58a25be629cccac28883171c92`) |
| Files | `rules/sch/CEN-EN16931-UBL.sch` (CEN Schematron 1.3.15) and `rules/sch/PEPPOL-EN16931-UBL.sch` (Peppol BIS Billing 3.0.20), fetched from `raw.githubusercontent.com` at that commit |
| Compiler | ISO Schematron skeleton, https://github.com/Schematron/schematron commit `77dcd36`. The four files needed are committed unmodified in `../Skeleton/`, with its MIT `LICENSE` |
| Processor | Saxon-HE 12.10 (via IKVM, see `../../BePeppolCommerce.Core.csproj`) |

## Why the rules are not committed

- `CEN-EN16931-UBL.sch` states in its header that it is licensed under EUPL 1.2.
- `PEPPOL-EN16931-UBL.sch` states in its header that it uses CEN/EN16931-1 business terms,
  reproduced with permission from CEN. It states no licence of its own, and the upstream repo has
  no LICENSE file.
- The upstream `guide/bis/introduction.adoc` says OpenPeppol AISBL holds the copyright of the
  Peppol BIS, and that the Peppol BIS document may not be modified, redistributed, sold or
  repackaged without OpenPeppol's prior consent. It is not clear whether that sentence covers the
  Schematron files. For a public repo, that is reason enough not to commit them or anything
  generated from them.

For the same reason, the upstream `rules/examples/*.xml` are not vendored as tests. On day 028 all
nine examples, run locally through `PeppolValidator`, passed with zero findings.

## Moving to a new upstream release

Change the commit in `PeppolRulesRaw` and both `Sha256` values in the csproj (`sha256sum` on the
new files), run `dotnet test`, and run the new `rules/examples` through the validator.
