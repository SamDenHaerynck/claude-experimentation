# State
Day: 027
Idea: be-peppol-commerce
Phase: 3 Build
Slice: 2a of 11, complete (next: 2b)
Next action: Slice 2b (wire in validation), following the "Approach, decided in 2a" block under
Slice 2b in `ideas/be-peppol-commerce/PLAN.md`. First install the .NET 8 SDK (`RUNBOOK.md`
"Build environment"). Then: (1) add NuGet `IKVM` 8.16.1 to `BePeppolCommerce.Core`, and get
Saxon-HE 12.5 plus xmlresolver 5.2.2. Try `MavenReference` for 5 minutes at most; if it fails, use an
MSBuild `DownloadFile` target and `IkvmReference`. Never commit the jars. (2) Regenerate the XSLT
from OpenPEPPOL/peppol-bis-invoice-3 tag v3.0.20 with the ISO skeleton, the way
`spikes/2a-schematron/Program.cs` does, and commit it under `Validation/Rules/` together with
`SOURCE.md`. (3) Add a `PeppolValidator` that returns the failed asserts. (4) Add tests: the fixture
passes; a broken fixture fails with the rule IDs; a missing buyer reference fails
PEPPOL-EN16931-R003. Also fix the day-026 lows and apply the review carry-overs (6)-(11) in the Slice 2b block (XXE, jar hashes, conformance examples). If the slice runs long, split it into 2b (Schematron)
and 2c (XSD + lows) in PLAN.md rather than overrunning.
Read first: OWNER.md, RUNBOOK.md, ideas/be-peppol-commerce/PLAN.md (Stack + Slice 2b),
ideas/be-peppol-commerce/spikes/2a-schematron/README.md and Program.cs,
ideas/be-peppol-commerce/app/README.md, ideas/be-peppol-commerce/app/src/BePeppolCommerce.Core/Ubl/PeppolInvoiceBuilder.cs
Notes for owner:
- (Owner) `be-peppol-commerce` won tournament round 1 (day 024, 20/25, no hard disqualifier) — full
  Deep Check evidence in `ideas/be-peppol-commerce/VALIDATION.md`. A validation kit is ready at
  `ideas/be-peppol-commerce/VALIDATION_KIT.md`: outreach messages (EN/NL), a landing-page draft, and
  one-week strong/weak/kill signal criteria, per `OWNER.md`'s escalation rule ("a tournament winner
  has been picked and its validation kit is ready"). The routine cannot contact anyone — running
  this kit (or not) is entirely up to you. Report results in `ideas/be-peppol-commerce/SIGNALS.md`
  when/if you do; write "stop" there to end the idea at any time. Still no `SIGNALS.md` as of day
  027 — build proceeds without waiting, per `OWNER.md`.
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
- (Owner) Day 027 decided that Peppol validation runs Saxon-HE (Java) inside .NET through IKVM. It
  gives full official rule coverage but is heavy: 337 MB of build output before trimming. The
  review also raised an unverified point: Configured Commerce extensions may have to target .NET
  Framework 4.8, not .NET 8. Slice 6 checks this first. If you already know the answer, a line in
  `SIGNALS.md` would save a session.
Tournament round: 1 (won)
Kills before 2026-09-23 rewrite: 14
Last session: 2026-09-29, ended clean
