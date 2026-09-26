# Validation: CMS12-UpgradeAssist

Deep Check, day 024 (Phase 1, tournament round 1, shortlist #2). Idea: a tool that scans an
Optimizely CMS 11 (EPiServer/Alloy) codebase for breaking changes and missing admin features when
upgrading to Optimizely CMS 12.

## Original complaint: still unresolved

world.optimizely.com's 2022 thread (fetched) confirms the original complaint: CMS 11's
Admin > Config > Modules screen (view installed module versions, disable scheduled jobs) has no
CMS 12 equivalent. Optimizely staff member Grzegorz Wiecheć replied (2022-08-08): "You are right,
it's not available. We are now looking into porting Plugin-Manager functionality into CMS 12. It
should be available in near future."
https://world.optimizely.com/forum/developer-forum/cms-12/thread-container/2022/8/how-do-you-see-the-versions-installed-cms-12-/

Checked current (2026) state: docs.developers.optimizely.com's shell-modules config doc and its
breaking-changes doc (both fetched) describe module config as purely file/manifest-based, with no
admin screen; the breaking-changes doc explicitly states "the CMS 11 dashboard was not ported to
CMS 12." The 2026 CMS 12 release notes (support.optimizely.com, fetched) do not mention a module
admin screen or Plugin-Manager anywhere. **No evidence found that the ported feature staff promised
in 2022 was ever delivered.** A later (Feb 2024) forum thread found only discusses an unrelated
module-authorization-policy bug, not the missing screen — search for 2024-2026 complaints specific
to this screen was not exhaustive, so "still actively complained about" is unconfirmed, but "still
missing from the product" is confirmed from current docs.

## Other documented CMS 11→12 pain points (broader demand evidence than the original screen alone)

All Deep Check research points beyond the Modules gap, each independently confirming real migration
friction:
- .NET Framework → .NET Core/5 runtime migration, described as "the largest single bucket" of
  upgrade work. https://umage.ai/insights/cms-11-out-of-support/ (fetched)
- WCF-based features removed entirely (Mirroring, WCF event provider, WCF/Lucene search service).
  https://docs.developers.optimizely.com (breaking-changes doc, fetched)
- XForms removed (must migrate to Optimizely Forms) — same doc.
- WebForms/`GuiPlugInAttribute`/`PagePlugInAttribute` admin extensibility removed; must rewrite as
  `IMenuProvider`/middleware — same doc.
- `BlockController`/`PartialContentController` → view components — same doc.
- `EPiServer.Find` must be fully re-implemented on Optimizely Graph. umage.ai (fetched)
- Real, named upgrade-failure threads from Aug-Nov 2023 (users pappabj0rn, Johan Petersson, Johan
  Book): "Unable to find a module by assembly...", `NullReferenceException` in `RegisterNlsRoute`,
  missing `module.zip` in published packages.
  https://world.optimizely.com/forum/developer-forum/cms-12/thread-container/2023/8/problems-upgrading-to-cms-12/
  (fetched)

## Existing competing tools

- **Microsoft's `dotnet upgrade-assistant` + Optimizely's CMS-specific extension rules**
  (github.com/episerver/upgrade-assistant-extensions, fetched; documented at
  docs.developers.optimizely.com, fetched): handles project retargeting, NuGet updates,
  `web.config` migration — but does not scan for missing admin features, and leaves substantial
  manual work. **This repo was archived 2025-11-12; its README states it is "deprecated in favor of
  GitHub Copilot app modernization chat agent."** Optimizely's own semi-official tooling for this
  exact problem is now dead, replaced with a generic (non-CMS-specific) AI agent.
- **Royal Cyber's "OptiUpgrade Assistant"**: the closest real competing concept — claims automation
  of "up to 31 migration steps" and fixes for "220+ categories of build errors," 60-70% time
  reduction. Posted by its author on world.optimizely.com's blog (2026-04-13, fetched). Critically,
  the same post states: "the tool is exclusively available for internal use within Royal Cyber
  projects. It is not commercially licensed or distributed as a public product." Not a market
  competitor today, but strong evidence a specialized consultancy judged this worth building
  proprietary tooling for.
- **Optimizely CMS SaaS Migration Tool** (community, github.com/chrno1209/optimizely-cms-saas-migration-tool):
  different scope — content-type/template parity checking between environments, not a CMS 11→12
  breaking-change scanner.
- **No public, commercially-available breaking-change/admin-gap scanner for CMS 11→12 was found.**

**Risk to flag, not a hard disqualifier:** Optimizely/Microsoft's own stated direction is toward
generic AI-assisted modernization ("use GitHub Copilot app modernization chat agent") rather than a
domain-specific scanner. A generic AI coding assistant is a plausible free-if-already-paying-for-it
substitute for some of this value, though it would need CMS 11/12-specific knowledge (the exact
breaking-change list above) to match a purpose-built tool, which is not confirmed either way.

## CMS 11 install base / EOL

CMS 13 reached GA 2026-03-31; Optimizely formally announced CMS 11 out-of-support 2026-04-10
(support.optimizely.com, fetched). Policy: only the current version plus one prior major version
receive active security/bug fixes. Optimizely does not force an upgrade or shut off CMS 11 sites
(explicit in the same support article). **No quantitative CMS 11 install-base numbers were found**
anywhere (Optimizely's own pages, or partner blogs checked) — recorded as "no evidence found," not
invented. A general Optimizely-detected-domains count (~43,312, technologychecker.io, fetched) has
no CMS-version breakdown and isn't usable as an install-base figure for CMS 11 specifically.

## H3 check

No hard disqualifier: Optimizely's own free/semi-official tooling for this (the extended
`upgrade-assistant`) is archived/deprecated, not currently shipping. No current Optimizely-native
feature covers admin-gap scanning or breaking-change detection for free.

## Revised scoring

Day-022 quick screen: Demand 3, WTP 2, Wedge 3, Owner fit 5, Buildability 3 = 16/25.

Deep Check revision:
- **Demand → 4** (up from 3): the original 2022 complaint is confirmed with a direct Optimizely
  staff quote, plus multiple independent, named, dated (2023) upgrade-failure forum threads found
  this session — demand evidence is broader and more concrete than the single Modules-screen
  complaint the original screen relied on.
- **WTP → 3** (up from 2): no publicly listed price for a comparable product exists, but a named
  consultancy (Royal Cyber) built proprietary tooling for exactly this problem and markets the time
  savings publicly — a real signal of value, short of a direct price point.
- **Wedge → 3** (unchanged): real, currently-unserved pain confirmed; the closest analog is
  proprietary/internal-only (Royal Cyber) and Optimizely's own official tooling was just
  discontinued. Held at 3, not raised, because of the flagged generic-AI-substitute risk above.
- **Owner fit → 5** (unchanged): squarely the owner's daily work.
- **Buildability → 3** (unchanged): the breaking-changes doc gives a concrete, bounded checklist of
  known patterns to scan for (WebForms attributes, WCF usage, `BlockController`, XForms,
  `EPiServer.Find` usage, shell-module manifests), which bounds scope, but covering all categories
  plus parsing real C# codebases is non-trivial.

**Revised total: 18/25.** No hard disqualifier.

## Unreachable / not attempted

- royalcyber.com's own product page (403 Forbidden on fetch) — Royal Cyber's tool description
  relies on their world.optimizely.com blog post only.
- journey-digital.com's CMS 11 article (404 on fetch, likely moved/removed).
- hiddenfoundry.com's upgrade-milestones article (502, transient, not retried).
- Optimizely Marketplace app store was not checked directly for a scanner listing (no working URL
  found/attempted this session) — worth a follow-up if this idea is picked.
- 2024-2026 forum search for continued Modules-screen complaints was not exhaustive; some results
  relied on search summaries rather than direct fetch.
