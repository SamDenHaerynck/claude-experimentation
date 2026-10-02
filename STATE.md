# State
Day: 030
Idea: be-peppol-commerce
Phase: 3 Build
Slice: 4 of 11, complete (next: 5)
Next action: Slice 5 (inbound flow) in `ideas/be-peppol-commerce/PLAN.md`. Install the .NET 8 SDK
(`RUNBOOK.md` "Build environment") and run `dotnet test` in `ideas/be-peppol-commerce/app` (73 should
pass; on a Maven Central 429, wait a minute and rerun). Then: (1) add a normalized `InboundInvoice`
record and a parser in `src/BePeppolCommerce.Core/Inbound/` that takes UBL XML via
`PeppolValidator.ParseUntrusted` (add a `MaxCharactersInDocument` cap there) and extracts invoice
number, issue date, currency, seller/buyer name and endpoint, line count and payable amount, returning
a failure result (not an exception) for malformed XML or a non-Invoice root; (2) add
`src/BePeppolCommerce.Api/` (ASP.NET Core minimal API, add to `BePeppolCommerce.sln`) with one POST
route that accepts a webhook body carrying a provider document id, fetches the document with
`IPeppolAccessPointClient.GetInboundAsync`, parses it and returns the normalized model, with a request
size limit. Storecove's webhook body shape is NOT defined in its spec (day 029), so model a minimal
`{ "guid": "..." }` body and say so in the README. Test with `WebApplicationFactory` (or fakes) using
a fixture built from the sample order, plus malformed and oversized cases. If time is short, split
off the `RuntimeIdentifier` publish-size trim to Slice 5b in `PLAN.md` rather than overrunning.
Read first: OWNER.md, RUNBOOK.md, ideas/be-peppol-commerce/PLAN.md (Slice 5),
ideas/be-peppol-commerce/app/README.md,
ideas/be-peppol-commerce/app/src/BePeppolCommerce.Core/AccessPoint/IPeppolAccessPointClient.cs,
ideas/be-peppol-commerce/app/src/BePeppolCommerce.Core/Validation/PeppolValidator.cs,
ideas/be-peppol-commerce/app/tests/BePeppolCommerce.Core.Tests/OutboundInvoiceSenderTests.cs (fake AP pattern)
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
- (Owner) Day 030: the outbound flow derives a Storecove idempotency key from the invoice content
  when the caller passes none. Storecove's spec does not say how long it remembers a key or whether
  rejected submissions count. If you have a Storecove sandbox, that is worth one check before Slice 8.
Tournament round: 1 (won)
Kills before 2026-09-23 rewrite: 14
Last session: 2026-10-02, ended clean
