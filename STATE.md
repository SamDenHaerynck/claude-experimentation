# State
Day: 025
Idea: be-peppol-commerce
Phase: 3 Build
Slice: 1 of 11, not started
Next action: Build slice 1 (walking skeleton) per `ideas/be-peppol-commerce/PLAN.md`: create a
.NET 8 solution with `BePeppolCommerce.Core` (class library) and `BePeppolCommerce.Core.Tests`
(xUnit), one sample order JSON fixture, and one passing test that builds a minimal, well-formed
Peppol BIS UBL invoice XML from that fixture (structurally valid XML only — real EN16931 schema
validation is Slice 2a/2b, not this slice). Write `app/README.md` documenting `dotnet test`. Note:
`PLAN.md`'s Slice 2 was split into 2a (validation-approach spike) + 2b (wire-in) after critic
review found .NET has no built-in XSLT 2.0/3.0 processor for real Schematron rules — do not treat
Slice 2 as a single simple session when you get there.
Read first: OWNER.md (Phase 2/3 sections), ideas/be-peppol-commerce/PLAN.md (all of it, including
"Critic review" and the schedule-risk-compression note), ideas/be-peppol-commerce/VALIDATION.md if
provider/wedge context is needed again.
Notes for owner:
- (Owner) `be-peppol-commerce` won tournament round 1 (day 024, 20/25, no hard disqualifier) — full
  Deep Check evidence in `ideas/be-peppol-commerce/VALIDATION.md`. A validation kit is ready at
  `ideas/be-peppol-commerce/VALIDATION_KIT.md`: outreach messages (EN/NL), a landing-page draft, and
  one-week strong/weak/kill signal criteria, per `OWNER.md`'s escalation rule ("a tournament winner
  has been picked and its validation kit is ready"). The routine cannot contact anyone — running
  this kit (or not) is entirely up to you. Report results in `ideas/be-peppol-commerce/SIGNALS.md`
  when/if you do; write "stop" there to end the idea at any time. Still no `SIGNALS.md` as of day
  025 — build proceeds without waiting, per `OWNER.md`.
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
Last session: 2026-09-27, ended clean
