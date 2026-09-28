# State
Day: 026
Idea: be-peppol-commerce
Phase: 3 Build
Slice: 1 of 11, complete (next: 2a)
Next action: Slice 2a (validation spike) per `ideas/be-peppol-commerce/PLAN.md`. Install the
.NET 8 SDK first (`RUNBOOK.md` "Build environment"). Then, time-boxed: (1) check SaxonCS's
licence and NuGet availability from Saxonica's own site. Whether it is free or commercial-only is
unverified, and a paid licence would rule it out as a default dependency. (2) Look for any other
XSLT 2.0/3.0 option usable offline from .NET. (3) Fetch the official Peppol BIS Billing 3.0 /
EN16931 UBL Schematron artefacts (note their source URL and version) and try one on the XML that
`PeppolInvoiceBuilder.Build` produces from `tests/BePeppolCommerce.Core.Tests/Fixtures/sample-order.json`.
Record the decision in `DECISIONS.md`: full Schematron coverage, or a named partial C# rule set.
Also validate against the UBL 2.1 Invoice XSD if that is cheap (.NET's `XmlSchemaSet` handles XSD 1.0).
Read first: OWNER.md, RUNBOOK.md, ideas/be-peppol-commerce/PLAN.md (Stack + Slices 2a/2b),
ideas/be-peppol-commerce/app/README.md, ideas/be-peppol-commerce/app/src/BePeppolCommerce.Core/Ubl/PeppolInvoiceBuilder.cs
Notes for owner:
- (Owner) `be-peppol-commerce` won tournament round 1 (day 024, 20/25, no hard disqualifier) — full
  Deep Check evidence in `ideas/be-peppol-commerce/VALIDATION.md`. A validation kit is ready at
  `ideas/be-peppol-commerce/VALIDATION_KIT.md`: outreach messages (EN/NL), a landing-page draft, and
  one-week strong/weak/kill signal criteria, per `OWNER.md`'s escalation rule ("a tournament winner
  has been picked and its validation kit is ready"). The routine cannot contact anyone — running
  this kit (or not) is entirely up to you. Report results in `ideas/be-peppol-commerce/SIGNALS.md`
  when/if you do; write "stop" there to end the idea at any time. Still no `SIGNALS.md` as of day
  026 — build proceeds without waiting, per `OWNER.md`.
- (Owner) `PLAN.md` (day 025) scopes v1 honestly around a hard constraint: no license/instance
  exists for a real Optimizely Configured Commerce install, and the routine may never deploy or
  create accounts. So v1 builds the real engine (build/validate/send/receive a Peppol invoice via
  a provider abstraction, tested only against fakes) plus a documented, stubbed Configured Commerce
  extension contract — not a live plugin. If you want an actual Configured Commerce install
  attempted, that has to happen on your side; flag it in `SIGNALS.md` or here if so.
- (Owner) Runners-up for round 2 if this idea's build stalls or you write "stop" in SIGNALS.md:
  `CMS12-UpgradeAssist` (18/25, Deep Checked day 024, `ideas/cms12-upgradeassist/VALIDATION.md`) and
  `ci-migration-rollback-gate` (16/25, not yet Deep Checked). `ADO-MultiOrg` was also Deep Checked
  day 024 but dropped to 15/25 (a second, more mature free competitor was found covering its core
  mechanism) — `ideas/ado-multiorg/VALIDATION.md` has the detail if you want to reconsider it later.
Tournament round: 1 (won)
Kills before 2026-09-23 rewrite: 14
Last session: 2026-09-28, ended clean
