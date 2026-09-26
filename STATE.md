# State
Day: 024
Idea: be-peppol-commerce
Phase: 2 Plan
Slice: n/a
Next action: Write `ideas/be-peppol-commerce/PLAN.md` per `OWNER.md` Phase 2: the one core user
flow (a v1 flow is: an Optimizely Configured Commerce admin/developer configures a Peppol AP
provider's API credentials — Storecove, Qvalia, Recommand, or Billit — in the connector; on order
completion/invoice generation, the connector builds a Peppol BIS/EN16931 XML from the order/invoice
data and sends it through the configured AP's API; inbound Peppol invoices/credit notes route back
into Configured Commerce), an explicit out-of-scope list (e.g. v1 should probably NOT include:
building/hosting its own Access Point, supporting every possible ERP/Configured-Commerce
customization path, or a full accounting/reconciliation UI — but confirm this in the Plan session
itself, don't just copy this verbatim), a stack choice justified for an Optimizely Configured
Commerce extension (likely C#/.NET, matching Optimizely Commerce's own stack), a slice list of 5-10
slices each leaving the app runnable (slice 1 = walking skeleton: a minimal .NET project/test
harness that can build a valid EN16931 XML from a sample order payload and validate it against the
schema, one test passing), and done criteria specific to this app. Have a critic subagent attack
the plan before accepting it, per `OWNER.md`. `ideas/be-peppol-commerce/VALIDATION.md` and
`VALIDATION_KIT.md` are already written (day 024) — read them first, do not re-research.
Read first: OWNER.md (Phase 2 section), ideas/be-peppol-commerce/VALIDATION.md,
ideas/be-peppol-commerce/VALIDATION_KIT.md, DECISIONS.md's 2026-09-26 entry.
Notes for owner:
- (Owner) `be-peppol-commerce` won tournament round 1 (day 024, 20/25, no hard disqualifier) — full
  Deep Check evidence in `ideas/be-peppol-commerce/VALIDATION.md`. A validation kit is ready at
  `ideas/be-peppol-commerce/VALIDATION_KIT.md`: outreach messages (EN/NL), a landing-page draft, and
  one-week strong/weak/kill signal criteria, per `OWNER.md`'s escalation rule ("a tournament winner
  has been picked and its validation kit is ready"). The routine cannot contact anyone — running
  this kit (or not) is entirely up to you. Report results in `ideas/be-peppol-commerce/SIGNALS.md`
  when/if you do; write "stop" there to end the idea at any time.
- (Owner) Runners-up for round 2 if this idea's build stalls or you write "stop" in SIGNALS.md:
  `CMS12-UpgradeAssist` (18/25, Deep Checked day 024, `ideas/cms12-upgradeassist/VALIDATION.md`) and
  `ci-migration-rollback-gate` (16/25, not yet Deep Checked). `ADO-MultiOrg` was also Deep Checked
  day 024 but dropped to 15/25 (a second, more mature free competitor was found covering its core
  mechanism) — `ideas/ado-multiorg/VALIDATION.md` has the detail if you want to reconsider it later.
Tournament round: 1 (won)
Kills before 2026-09-23 rewrite: 14
Last session: 2026-09-26, ended clean
