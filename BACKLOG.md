# Backlog

Ranked pool of candidate ideas not yet started, scored under the tournament rubric in `OWNER.md`
(rewritten 2026-09-23). Phase 1 Deep Check runs on the top 3 (the shortlist below); the highest
scorer with no hard disqualifier and 12/25+ wins per `OWNER.md`'s Pick rule.

## 2026-09-24 (day 022) full re-screen

Per `STATE.md`'s day-022 next action: screened `INBOX.md` (empty, nothing to screen), sourced 8 new
candidates from the owner-fit channels via a research subagent, and re-screened the 14 ideas in
`killed/` plus the 2 prior backlog entries under the new hard-disqualifier-only rubric — the
one-time re-screen `OWNER.md` grants for this rewrite. Full source evidence for the 14 killed ideas
is unchanged and still lives in `killed/<slug>/VALIDATION.md` and `REASON.md`; this session did not
re-fetch new evidence for them, only re-applied the new scoring rules to the evidence already on
file. The 8 new candidates' evidence (URLs, paraphrased findings) is recorded per-idea below since
they have no directory yet.

**Judgment call, recorded per `DECISIONS.md`:** re-screened `killed/` ideas that no longer carry a
hard disqualifier are ranked in the table below and are eligible for Deep Check / Pick, but their
directories were left in place under `killed/<slug>/` rather than physically relocated — moving a
directory has no bearing on tournament eligibility (that's determined by this table), and a
physical `git mv` is deferred to the session that actually selects one, to avoid mid-review file
churn. Ideas that still carry a hard disqualifier (H1-H4) stay in `killed/` and are excluded from
shortlist eligibility regardless of score, per `OWNER.md`.

### Ranked table (24 candidates, highest score first)

| # | Idea | Demand | WTP | Wedge | Owner fit | Build | Total | Hard disq. | Status |
|---|---|---|---|---|---|---|---|---|---|
| 1 | **BE-Peppol-Commerce** (new) | 4 | 4 | 4 | 5 | 3 | **20** | none — H2 to confirm in Deep Check | Shortlist #1 |
| 2 | **CMS12-UpgradeAssist** (new) | 3 | 2 | 3 | 5 | 3 | **16** | none | Shortlist #2 |
| 3 | **ADO-MultiOrg** (new) | 3 | 2 | 2 | 5 | 4 | **16** | none | Shortlist #3 |
| 4 | ci-migration-rollback-gate (revived, was killed/) | 4 | 3 | 2 | 3 | 4 | 16 | none | Close 4th, not shortlisted (lower owner fit on tiebreak) |
| 5 | ADO-GovAudit (new) | 2 | 2 | 3 | 5 | 3 | 15 | none | Backlog |
| 5 | Opal-ToolPack (new) | 2 | 2 | 3 | 5 | 3 | 15 | none | Backlog |
| 5 | Opal-Insights (new) | 2 | 2 | 3 | 5 | 3 | 15 | none | Backlog |
| 5 | ADO-BoardsMultiOrg (new) | 2 | 1 | 3 | 5 | 4 | 15 | none | Backlog |
| 5 | AdvCMS-ReviewPersist (new) | 2 | 2 | 2 | 5 | 4 | 15 | none | Backlog |
| 10 | vendor-security-questionnaire-autofill (revived) | 4 | 4 | 2 | 1 | 3 | 14 | none | Backlog — no owner network |
| 10 | freelancer-sow-contract-generator (unvalidated carry-over) | 3 | 3 | 2 | 2 | 4 | 14 | none (unvalidated) | Backlog |
| 10 | dependency-eol-watcher (revived) | 3 | 2 | 2 | 3 | 4 | 14 | none | Backlog |
| 13 | grant-report-normalizer (revived) | 2 | 4 | 2 | 1 | 4 | 13 | none | Backlog |
| 13 | clickup-conditional-formatting (revived) | 4 | 2 | 2 | 1 | 4 | 13 | none | Backlog |
| 13 | git-changelog-generator (unvalidated carry-over) | 2 | 2 | 2 | 3 | 4 | 13 | none (unvalidated) | Backlog |
| 16 | rfq-quote-comparison (revived) | 2 | 3 | 2 | 1 | 4 | 12 | none | Backlog |
| 16 | qbo-weekly-snapshot | 3 | 3 | 1 | 1 | 4 | 12 | **H3** | Stays in killed/ |
| 18 | co-owned-vacation-property (revived) | 2 | 3 | 1 | 1 | 4 | 11 | none | Backlog |
| 18 | vendor-price-list-normalizer (revived) | 1 | 2 | 2 | 2 | 4 | 11 | none | Backlog |
| 18 | sales-tax-nexus-monitor | 2 | 3 | 1 | 1 | 4 | 11 | **H3** | Stays in killed/ |
| 21 | rent-increase-notice-calculator | 3 | 1 | 1 | 1 | 4 | 10 | **H3** | Stays in killed/ |
| 22 | contractor-classification-checker (revived) | 2 | 2 | 1 | 1 | 3 | 9 | none* | Backlog (bottom) |
| 22 | customs-hts-microseller | 3 | 1 | 1 | 1 | 3 | 9 | **H3** | Stays in killed/ |
| 22 | cam-reconciliation-small-cre | 1 | 2 | 1 | 1 | 4 | 9 | **H4** | Stays in killed/ |

\* `contractor-classification-checker`'s original kill cited "real users only asked for the
classification answer, never the audit trail" — on strict re-reading this is an *absence* of demand
for the differentiator, not a user *explicitly saying they don't want it*, so it does not meet H4's
letter ("Finding nothing is not contrary evidence"). Its `REASON.md` also flagged a second signal
this table's "none" call did not originally address: the idea "sits close to unlicensed legal
advice," H2-adjacent ("regulated advice (legal...)"). Judgment call: several free comparable tools
(Tax1099, OnPay, SmallBizHandbook quizzes) already do this exact classification-risk assessment as
plain software, without apparent professional licensure, so H2 likely does not strictly apply — but
this is a real liability-risk factor to re-check explicitly in any future Deep Check of this idea,
not one to wave past silently. Kept out of `killed/` for scoring purposes; its score (9/25, owner
fit 1) keeps it at the bottom regardless.

## Top-3 shortlist for Deep Check

1. **BE-Peppol-Commerce** — Optimizely Configured Commerce connector generating/receiving Peppol
   BIS/EN16931 e-invoices for Belgian B2B buyers. Belgium's B2B e-invoicing mandate took effect
   2026-01-01 (already live, not a future risk), affecting roughly 1.2M VAT entities per
   combell.com and blog.seeburger.com. No Optimizely-Commerce-specific Peppol integration was found
   in this session's search; named generic Peppol access points (Smart PEPPOL, Babelway, Qvalia,
   Banqup, Taxilla) don't address the Commerce/OCP integration layer. **Deep Check must resolve
   before anything else: does shipping this require the product itself to be an accredited Peppol
   Access Point (an H2-relevant licensing question), or can it integrate as a client of an existing
   accredited AP's API (no license needed)?** If it requires AP accreditation, this may hit H2 and
   should be re-scored or killed on that basis alone.
2. **CMS12-UpgradeAssist** — scans an Optimizely CMS 11/Alloy codebase for breaking changes and
   missing admin features when upgrading to CMS 12. Real recurring forum pain (2022-2023 threads on
   world.optimizely.com) about a missing Admin > Config > Modules equivalent in CMS 12; Optimizely
   staff said a port was being considered but Deep Check should confirm current CMS 12 state before
   assuming the gap still exists.
3. **ADO-MultiOrg** — an Azure DevOps multi-org MCP/CLI gateway letting a consultancy switch
   context across ~15-20+ client orgs without reconfiguring. Real: Microsoft's own
   `azure-devops-mcp` repo has an open (`Won't Fix`) issue (#812, filed 2025-12-29) requesting this
   exact capability. Risk to resolve in Deep Check: an independent open-source alternative
   ("Azure DevOps Multi-Organization MCP Server" by nikydobrev on Glama.ai) already exists and is
   free — Deep Check must check what it actually covers and whether a real wedge (packaging,
   support, a management UI, org-governance features) survives next to it.

`ci-migration-rollback-gate` (16/25, owner fit 3) is a close 4th, revived from `killed/` under the
new rubric. Correction on re-check: this idea (and `vendor-security-questionnaire-autofill`,
`dependency-eol-watcher`) predates the hard-disqualifier framework entirely — all three were killed
under the old numeric ≥16 threshold, not under H3 or any hard disqualifier, so "no longer meets H3"
is not quite accurate; more precisely, no hard disqualifier ever applied to these three, under
either the old or new system. `ci-migration-rollback-gate`'s original kill leaned on Bytebase's
free Community tier, but Bytebase is a third-party competitor, not "the platform this idea plugs
into" shipping the feature free, so it would not meet H3 even if re-tested against it directly. It
was not shortlisted only because its owner fit (3, general CI/DevOps) is lower than the other three
16/25-or-above candidates on the `OWNER.md` tiebreak rule. Worth a fresh look, specifically an Azure
DevOps Pipelines-native migration/rollback gate extension (owner runs ~20 ADO orgs daily), if the
top 3 don't pan out in Deep Check.

## Sourcing notes for the 8 new candidates (research subagent, all URLs fetched or returned directly)

- **ADO-MultiOrg**: github.com/microsoft/azure-devops-mcp/issues/812.
- **ADO-GovAudit**: learn.microsoft.com Azure DevOps auditing docs (90-day per-org retention,
  no cross-org rollup); github.com/ZanattaMichael/AzureDevOpsDsc/issues/84.
- **Opal-ToolPack** / **Opal-Insights**: feedback.optimizely.com/forums/966084-optimizely-opal-ai
  (Excel/Outlook-Teams/Canva/Semrush tool requests, 1-3 votes each; agent usage analytics request,
  3 votes; activity-logging/CSV-export request, 1 vote).
- **CMS12-UpgradeAssist**: world.optimizely.com forum threads (2022, 2023) on CMS 11→12 upgrade
  pain and a missing Admin > Config > Modules equivalent.
- **BE-Peppol-Commerce**: blog.seeburger.com and combell.com on Belgium's 2026-01-01 mandatory
  B2B e-invoicing mandate.
- **ADO-BoardsMultiOrg**: josh-ops.com/posts/github-connecting-to-azure-boards-multiple-orgs
  (documents the "not recommended nor possible" single-GitHub-org-to-multi-ADO-org limitation).
- **AdvCMS-ReviewPersist**: world.optimizely.com/blogs/advanced-cms/dates/2019/6 (reviewer-comment
  persistence question on the open-source Advanced & External Reviews add-on, unresolved).

No solid evidence was found for general Optimizely Marketplace add-on gaps or OCP-app-specific
complaints beyond the above — recorded as "no evidence found," not filled in.

## Carry-over entries (unvalidated, quick-screen estimate only, no fresh research this session)

1. **Freelancer SOW/contract generator with e-sign tracking** — lightweight freelancer-specific
   alternative to DocuSign/PandaDoc: generates scoped SOWs from an intake form, tracks e-signature
   status, reminds on renewal/expiry. Owner fit is weak (2/5, general freelance tooling, no
   specific network) — kept in the pool but unlikely to beat the shortlist above.
2. **Git-history-to-changelog generator for indie SaaS** — ingests merged PRs/commits and drafts a
   customer-facing changelog/release-notes email. Close to a one-shot text-generation task a
   general LLM prompt already does free; an Azure DevOps release-notes framing (owner fit 3) was
   not evidenced this session and would need Deep Check to establish a real wedge.
