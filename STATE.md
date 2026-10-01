# State
Day: 029
Idea: be-peppol-commerce
Phase: 3 Build
Slice: 3 of 11, complete (next: 4)
Next action: Slice 4 (outbound flow end to end) in `ideas/be-peppol-commerce/PLAN.md`. Install the
.NET 8 SDK (`RUNBOOK.md` "Build environment") and run `dotnet test` in `ideas/be-peppol-commerce/app`
(63 should pass; on a Maven Central 429, wait a minute and rerun). Then add an orchestrator in
`src/BePeppolCommerce.Core/` (for example `Outbound/OutboundInvoiceSender.cs`) that takes an `Order`,
builds the XML with `PeppolInvoiceBuilder`, validates it with `PeppolValidator`, and only if
`IsValid` calls `IPeppolAccessPointClient.SendAsync` with the buyer's endpoint as
`PeppolParticipant`. It returns one result that says whether the invoice failed validation, failed
to send, or was sent. Test it against an in-memory fake HTTP server running `StorecoveClient`
(`StubHandler` style, or Kestrel on a loopback port). Assert that the fake receives the base64 UBL
for the sample fixture, and that an order without a buyer reference never reaches the fake.
Read first: OWNER.md, RUNBOOK.md, ideas/be-peppol-commerce/PLAN.md (Slices 3-4),
ideas/be-peppol-commerce/app/README.md,
ideas/be-peppol-commerce/app/src/BePeppolCommerce.Core/AccessPoint/IPeppolAccessPointClient.cs,
ideas/be-peppol-commerce/app/src/BePeppolCommerce.Core/AccessPoint/StorecoveClient.cs,
ideas/be-peppol-commerce/app/tests/BePeppolCommerce.Core.Tests/StorecoveClientTests.cs
Notes for owner:
- (Owner) Licensing of the Peppol rules (day 028). OpenPEPPOL's BIS guide
  (`guide/bis/introduction.adoc` in peppol-bis-invoice-3) says OpenPeppol AISBL holds the copyright,
  and that the Peppol BIS document may not be redistributed without its consent. So the repo
  commits nothing derived from the Peppol Schematron. The build downloads the pinned files and
  compiles them at runtime. If you ship a product, check whether bundling the rules in a
  distributed package needs OpenPeppol's consent. That question also decides Slice 5 and later
  packaging. Two related points:
  - The built DLL embeds the two `.sch` files, so any published package redistributes them.
  - History exposure. The first day-028 commit, `2428761`, pushed compiled Peppol/CEN XSLT to the
    public branch `claude/blissful-maxwell-612ud9`. PR #31 was squash-merged, so those files are
    not on `main`. They remain reachable through that branch until the next session resets it, and
    through `refs/pull/31/head` permanently, unless GitHub Support purges it. The routine cannot
    delete them. Whether that matters is your call.
- (Owner) `be-peppol-commerce` won tournament round 1 (day 024, 20/25, no hard disqualifier) — full
  Deep Check evidence in `ideas/be-peppol-commerce/VALIDATION.md`. A validation kit is ready at
  `ideas/be-peppol-commerce/VALIDATION_KIT.md`: outreach messages (EN/NL), a landing-page draft, and
  one-week strong/weak/kill signal criteria, per `OWNER.md`'s escalation rule ("a tournament winner
  has been picked and its validation kit is ready"). The routine cannot contact anyone — running
  this kit (or not) is entirely up to you. Report results in `ideas/be-peppol-commerce/SIGNALS.md`
  when/if you do; write "stop" there to end the idea at any time. Still no `SIGNALS.md` as of day
  028 — build proceeds without waiting, per `OWNER.md`.
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
Last session: 2026-10-01, ended clean
