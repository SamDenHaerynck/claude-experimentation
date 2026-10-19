# Refresh: CMS12-UpgradeAssist (2026-10-19)

Refresh of VALIDATION.md (old score 18/25). Every claim below cites a page fetched this session; otherwise "no evidence found".

## 1. H3 refresh (does the platform ship the deliverable free?)

**Platform (Optimizely / Microsoft / GitHub): no CMS-specific scanner found.**
- Optimizely's current CMS 13 upgrade page says earlier CMS migration tools are deprecated and recommends the GitHub Copilot app modernization chat agent or similar AI tools. It names no Optimizely-specific Copilot instruction file or scanner. It does give a manual audit checklist: check third-party NuGet packages, clear CMS 12 obsolete warnings, find dynamic properties, the legacy plugin system and mirroring, review Graph queries, back up the DB first. It also suggests TreatWarningsAsErrors. https://docs.optimizely.com/cms-13/docs/upgrade-to-cms-13
- episerver/upgrade-assistant-extensions is still archived (2025-11-12, read-only). The README says Upgrade Assistant is deprecated in favor of the GitHub Copilot app modernization chat agent. https://github.com/episerver/upgrade-assistant-extensions
- A web search for community or Optimizely Copilot-instructions, skills or agent files for CMS 12->13 found none. Search summary only, not a page fetch: https://docs.developers.optimizely.com/content-management-system/docs/upgrade-to-cms-13
- Opal: search surfaced a Content Model Agent in community blogs, but no evidence it helps code migration (not fetched; treat as unconfirmed). Opal is relevant to CMS 13 only via Opti ID setup (Blend article below).

**Community, free, open source: NEW and directly relevant.**
- **OptimizelyCms13ReadinessScanner** (adnanzameer, MIT): .NET global/local tool, static analysis of CMS 12 codebases for CMS 13 breaking changes. Scans C#, .csproj, Directory.Build.props, appsettings, web.config. It has 13 rules (OPT13-001..013) with severities Blocker to Info, for example Find package references and ContentArea.FilteredItems. Optional flags: --semantic (MSBuild compile), --check-packages (NuGet compat), --check-api (against real CMS 13 assemblies). Outputs console/Markdown/JSON/SARIF, baseline file for CI, custom JSON rules. Does not mention CMS 11. Repo created 2026-10-08, 5 commits, 0 stars. https://github.com/adnanzameer/OptimizelyCms13ReadinessScanner
- NuGet: version 1.0.0, last updated 10/8/2026, 77 total downloads, MIT, owner adnanzameer. https://www.nuget.org/packages/OptimizelyCms13ReadinessScanner

**Does CMS 13 change the target? Yes.** CMS 13 requires .NET 10 and its breaking-changes doc has 10 sections: framework/platform (package restructuring, DI namespaces, JSON library switch), content types and properties, repository, sites->applications/routing, security/ACL, scheduling/plugins/events (plugin system removed), UI/shell, localization/import-export (mirroring removed), third-party compatibility, and an API replacement map. https://docs.optimizely.com/cms-13/docs/breaking-changes-in-cms-13
- EpiServer.Find is unsupported in CMS 13 (move to Graph or own search): upgrade doc above.
- Practitioner blog (Apr 2026): third-party packages were the biggest time sink; obsolete APIs become build errors; also admin-UI "module not found"/"No policy" errors, DI registration order (AddContentGraph before AddContentManager), SQL compat level 140+. https://world.optimizely.com/blogs/sanjay-katiyar/dates/2026/4/optimizely-migration-from-cms-12-to-cms-13/
- Forum thread (Feb-May 2026): obsolete ContentArea.FilteredItems error, ten partner packages blocking runtime as of 2026-05-01 (Geta Sitemaps, DbLocalizationProvider, LanguageManager and others). It notes an official upgrade guide was published about 2026-05-22. https://world.optimizely.com/forum/developer-forum/Developer-to-developer/Thread-Container/2026/2/upgrade-guide-for-cms-13
- Implication: a scanner now has to cover a package-compatibility matrix and DI/registration checks, not only code patterns.

**H3 VERDICT:**
- Platform-shipped free deliverable: NOT triggered. Optimizely and Microsoft deprecated their tooling and point to generic Copilot.
- Core deliverable already free for CMS 12->13: PARTIALLY TRIGGERED by a community tool. It is MIT, on NuGet, and does the code/package/config pre-flight scan. It is very young (2 weeks, 77 downloads, 13 rules), so it is a thin v1.0 rather than an established standard.
- CMS 11->12 scanning: still no free or commercial public scanner found. But CMS 11 is out of support and the market has moved toward CMS 12->13.
- Not a hard disqualifier, but the free-substitute risk is now concrete rather than hypothetical.

## 2. Competitors (every product found)

| Product | Type | Scope | Price |
|---|---|---|---|
| OptimizelyCms13ReadinessScanner (adnanzameer) | OSS, MIT, NuGet | CMS 12 -> 13 static scan | Free (MIT) |
| Royal Cyber OptiUpgrade Assistant | Proprietary | CMS 11->12 (also lists 12->13) | None; internal use only, not licensed. Blog dated 2026-04-13. https://world.optimizely.com/blogs/vaibhav/dates/2026/4/accelerating-optimizely-cms-upgrades-with-intelligent-automation/ |
| Royal Cyber "CMS 13 Accelerator" | Service marketing (from search snippet, page not fetched; royalcyber.com returned 403) | automated code assessment, readiness analysis | Not shown |
| First Line Software CMS 13 + Commerce 15 upgrade | Service | assessment, Graph/Opti ID, optional AI Commerce Readiness Scan, uses "Optimizely AI Development Studio" (built on Claude Agent SDK, per the page) | No price; request an assessment. https://firstlinesoftware.com/optimizely-cms-13-upgrade/ |
| First Line Software migration/modernization | Service | CMS 11/12/Ektron/Sitecore; "OptiUp" AI tool for assessment/planning | No price. https://firstlinesoftware.com/optimizely-implementation/ |
| First Line readiness assessment | Service | source code and architecture review only | No price. https://firstlinesoftware.com/understand-your-cms-13-migration-before-work-begins/ |
| Diagram CMS 12 technical analysis | Service | impact assessment, requirements audit, DevOps report; free 30-min call | No price. https://info.wearediagram.com/optimizely-cms-12-upgrade-technical-analysis |
| Blend Interactive | Service/content | upgrade readiness article only | No price. https://www.blendinteractive.com/thoughts/are-you-ready-for-optimizely-cms-13/ |
| Oshyn | Partner (search snippet only, not fetched) | general upgrade help | not shown |
| GitHub Copilot app modernization agent | Generic AI, recommended by Optimizely | all .NET | Pricing not checked |
| DrewNull/optimizely-cms12-upgrade, rahmanluminary/Optimizely-Alloy-CMS-13 | Personal repos (search listing only, not fetched) | notes / sample upgrade | Free |

- Fixed-price audit offerings with a published price: none found. All partners gate pricing behind contact.
- Optimizely Marketplace: marketplace.optimizely.com did not resolve (DNS error). No evidence found on listings.
- Earlier-known (VALIDATION.md): chrno1209 CMS SaaS migration tool (different scope), not re-fetched.

## 3. Demand

- 2026 pain is well documented, mostly CMS 12->13 (links in section 1): third-party package blockers, obsolete-API build errors, Find->Graph rewrite, admin module/policy errors, DB migration issues.
- Partner pain framing: Blend says sites on DB-backed auth plus Search & Navigation face the highest effort; also lists deprecated APIs, legacy content types/jobs, Opti ID migration. Neither it nor First Line gave numbers. https://www.blendinteractive.com/thoughts/are-you-ready-for-optimizely-cms-13/
- Demand-side signal for the tool category: three vendors (Royal Cyber, First Line, plus the OSS author) each built or marketed upgrade tooling in 2026. That is supply-side evidence, not buyer evidence.
- Install base: no CMS 11/12 version breakdown found. A search summary says W3Techs puts Optimizely under 0.1% of known-CMS sites and has no version split (summary only, W3Techs page not fetched). Optimizely says CMS 11 and earlier are unsupported as of 2026-04-10 but not forcibly cut off (support.optimizely.com, cited in VALIDATION.md; surfaced in search, not re-fetched).
- Paid-demand evidence (people paying for a scanner): no evidence found. 77 downloads of the free scanner in about 10 days is a weak adoption data point (NuGet page above).

## 4. Re-score (old 18/25)

| Dim | Old | New | Why |
|---|---|---|---|
| Demand | 4 | 4 | Pain is real and fresh for 12->13 (many 2026 threads/blogs). CMS 11->12 demand likely shrinking as EOL passes. |
| WTP | 3 | 2 | A free MIT scanner now exists, the official line is "use Copilot", partners sell services (no listed prices) and bundle their own internal tools. No evidence anyone pays for a scanner. |
| Wedge | 3 | 2 | The 12->13 code-scan wedge is already occupied (13 rules, SARIF, baseline, package/API checks). Remaining gaps: CMS 11->12, deeper package-matrix coverage, admin-feature gaps, fix automation. Unverified whether those matter to buyers. |
| Owner fit | 5 | 5 | Unchanged: owner's daily work. |
| Buildability | 3 | 4 | CMS 13 breaking-changes doc and API replacement map give a concrete rule source, and the existing tool shows it is feasible with Roslyn/MSBuild. |

**Total: 17/25 (down from 18).** The Wedge and WTP drops are real. The Demand/Buildability gains are partly offset by competition.

## Top 3 findings
1. A free MIT CMS 12->13 readiness scanner appeared 2026-10-08 (NuGet 1.0.0, 77 downloads). H3 is partially triggered for 12->13, though not by the platform.
2. Optimizely and Microsoft ship no CMS-specific scanner; the official path is generic Copilot plus a manual audit checklist. The old extension repo remains archived.
3. CMS 13 shifts the targets to package compatibility, Find->Graph, Opti ID, plugin/mirroring removal and DI registration. No published prices exist for any audit or scanner; the only price found is free.

## Unreachable
- royalcyber.com blog (403). docs.developers.optimizely.com URLs redirect (302/301) to docs.optimizely.com; used the redirect target. marketplace.optimizely.com (DNS failure). Reddit/G2/Upwork not attempted (blocked per brief). W3Techs, Oshyn, Optimizely support CMS 11 EOL article, Copilot pricing not fetched this session. Install base: no evidence found.
