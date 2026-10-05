# State
Day: 033
Idea: be-peppol-commerce
Phase: 3 Build
Slice: 7 of 11, complete (next: 8)
Next action: Slice 8 (config and credential handling) in `ideas/be-peppol-commerce/PLAN.md`.
Install the .NET 8 SDK (`RUNBOOK.md` "Build environment") and run `dotnet test` in
`ideas/be-peppol-commerce/app` (193 should pass). Then replace the ad hoc reads in
`src/BePeppolCommerce.Api/AccessPointConfig.cs` with strongly typed options classes
(`AccessPointOptions`, `StorecoveSection`, `RecommandSection`) bound with
`services.AddOptions<T>().Bind(...).Validate(...).ValidateOnStart()`, so every missing or invalid
value (no key, LegalEntityId <= 0, missing Recommand secret or company id, bad BaseUri) fails at
startup with a message naming the setting but never its value. Add `StorecoveOptions.SchemeMap`
binding from config (`Storecove:SchemeMap:0208=<name>`). Keep `.env.example` placeholders in step,
and add tests in `tests/BePeppolCommerce.Api.Tests` for each failure. Also log a startup warning naming
the selected provider when its key is blank (day-033 review finding 6), and move Recommand's option
checks into a static `RecommandOptions.Validate()` (finding 7). If time is left: decide in
`DECISIONS.md` whether Recommand webhook support (HMAC-SHA256 `X-Signature` over the raw body,
event envelope shape from https://github.com/brbxai/recommand-peppol) belongs in Slice 8 or 9.
Read first: OWNER.md, RUNBOOK.md, ideas/be-peppol-commerce/PLAN.md (Slices 7 to 9),
ideas/be-peppol-commerce/app/README.md,
ideas/be-peppol-commerce/app/src/BePeppolCommerce.Api/AccessPointConfig.cs,
ideas/be-peppol-commerce/app/src/BePeppolCommerce.Api/Program.cs,
ideas/be-peppol-commerce/app/src/BePeppolCommerce.Core/AccessPoint/StorecoveClient.cs,
ideas/be-peppol-commerce/app/src/BePeppolCommerce.Core/AccessPoint/RecommandClient.cs,
ideas/be-peppol-commerce/app/tests/BePeppolCommerce.Api.Tests/InboundWebhookTests.cs
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
  `SIGNALS.md` would save a session. **Answered day 032** from Optimizely's migration guide:
  Extensions ship `net48` and can be retargeted to `net8.0`/`net10.0` from release 5.2.2512 (steps
  start at build 5.2.2604.725-lts). The library stays `net8.0`, so installs that have not migrated
  cannot use v1. How many Belgian installs have migrated is unknown; if you know, note it in
  `SIGNALS.md`, because it decides whether `net48` support is worth a later slice.
- (Owner) Day 030: the outbound flow derives a Storecove idempotency key from the invoice content
  when the caller passes none. Storecove's spec does not say how long it remembers a key or whether
  rejected submissions count. If you have a Storecove sandbox, that is worth one check before Slice 8.
- (Owner) Day 031: the inbound webhook (`POST /webhooks/inbound`) uses a body shape and an
  `X-Webhook-Secret` header that are this project's own conventions, because Storecove's public spec
  defines neither its webhook body nor its webhook authentication. With a Storecove sandbox, one real
  webhook delivery would settle both.
- (Owner) Day 033: Recommand's send endpoint takes no idempotency key (its OpenAPI spec at
  https://peppol.recommand.eu/openapi has idempotency keys only on e-reporting submissions and webhook deliveries), so with
  Recommand as provider a retried send can deliver an invoice twice. Storecove remains the safer
  default for the dispatcher. If you talk to Recommand, asking whether a send idempotency key is
  planned would settle it.
Tournament round: 1 (won)
Kills before 2026-09-23 rewrite: 14
Last session: 2026-10-05, ended clean
