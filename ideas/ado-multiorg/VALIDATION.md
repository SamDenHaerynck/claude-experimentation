# Validation: ADO-MultiOrg

Deep Check, day 024 (Phase 1, tournament round 1, shortlist #3). Idea: an Azure DevOps multi-org
MCP/CLI gateway letting a consultancy switch context across ~15-20+ client Azure DevOps
organizations without reconfiguring.

## Core feature request: confirmed real, confirmed Won't Fix

Microsoft's own `azure-devops-mcp` repo issue #812 (filed 2025-12-29 by Johann Ungerer, requesting
`switch_organization`/`get_current_identity`/`test_authentication`/`refresh_authentication` tools,
with a working fork claimed) is confirmed **Closed, "Will Not Fix,"** assigned to danhellem.
https://github.com/microsoft/azure-devops-mcp/issues/812 (fetched; comment text did not render via
fetch, flagged as a possible tooling gap rather than confirmed empty).

Microsoft's own FAQ confirms this is by design, not an oversight: "you can connect to only one
organization at a time... you can switch organizations as needed" — i.e. the documented workaround
is running separate `mcp.json` server entries per organization.
https://raw.githubusercontent.com/microsoft/azure-devops-mcp/main/docs/FAQ.md (fetched)

## H3 check

The platform itself (Microsoft's official `azure-devops-mcp`) does not ship multi-org switching
free or otherwise — confirmed above. H3 requires the *platform* to ship the deliverable free; the
tools below are third-party competitors, not the platform, so H3 does not apply (same precedent as
`ci-migration-rollback-gate`'s Bytebase re-check in `BACKLOG.md`).

## Existing free competitors — two, not one, and one is materially more mature than initially known

1. **nikydobrev/mcp-server-azure-devops-multi** (the competitor named in the original screen):
   fetched its README and repo page directly. Covers org switching via a `ConnectionManager` that
   authenticates per-org from a `config.json` → PAT mapping (no OAuth/Entra flow, no credential
   rotation). 18 tools: Organizations/Projects (2), Pipelines & Builds (13), Repositories/PRs (3).
   **No Boards/work-item tools wrapped.** Ships as stdio or SSE/Docker/Azure Container Apps — a
   bare protocol server, no management UI or governance layer. Maturity: 0 stars, 0 forks, 0 open
   issues, ~5 commits total — an early/essentially-unused project.
2. **wangkanai/devops-enhanced-mcp** (newly surfaced this session, not in the original screen):
   free, npm package `@wangkanai/devops-mcp`, does **directory-based automatic org-switching** via
   a local `.azure-devops.json` — closer to this idea's core mechanism than nikydobrev's manual
   config-mapping approach. Materially more active: 12 stars, 4 forks, 207 commits. Still a bare
   CLI/protocol tool with no management UI or governance layer.

Both are free and open-source; between them they already cover automatic multi-org switching (the
core mechanism this idea proposes), with the more mature of the two (207 commits, actively
maintained) doing so via the more elegant directory-based approach this idea would likely also use.

## Wedge: not evidenced beyond analogy

No direct evidence was found of demand specifically for a management UI, centralized
credential/PAT vault with rotation, cross-org audit logging, or org-governance reporting layered on
top of multi-org MCP switching. What was found, adjacent but not on point:
- Safeguard.sh sells PAT inventory/governance/anomaly-detection across Azure DevOps orgs, but
  framed as single-tenant security hygiene, not consultancy multi-client context switching.
  https://safeguard.sh/resources/blog/azure-devops-personal-access-token-rotation-2026
- The generic "MCP gateway" category (Pomerium, MCP Mesh, Composio, surfaced via search, not
  independently fetched in depth) sells multi-tenant credential vaulting, RBAC, and audit logging
  as a product category in general — validating that *some* market wants this layer in principle,
  but none of these are Azure-DevOps-specific.

**No forum post, issue, or review was found explicitly asking for ADO-specific multi-org
governance/PAT-vault/audit tooling for consultancies.** Per the evidence rules, this is recorded as
"no evidence found" for the wedge, not treated as contrary evidence (H4 does not apply) — but it
means the differentiator this idea depends on to beat two existing free tools is unevidenced,
plausible by analogy only.

## Other competing products checked

- OpsHub "Multi-Instance Sync Tool" (Visual Studio Marketplace) — a paid migration/consolidation
  tool between ADO instances, a different use case (one-time migration, not live multi-org
  gateway access).
- DevOpSmartBoard (Visual Studio Marketplace) — a cross-org reporting dashboard, different use
  case (reporting, not a live MCP/CLI gateway).
- No dedicated paid "ADO multi-org consultancy management" product was found.

## Revised scoring

Day-022 quick screen: Demand 3, WTP 2, Wedge 2, Owner fit 5, Buildability 4 = 16/25.

Deep Check revision:
- **Demand → 3** (unchanged): the underlying need is real (confirmed feature request, confirmed
  Won't Fix, two independent free tools built to fill the gap), but that same evidence — two
  competitors already exist — caps how much this raises the score, since it also means the raw
  need is already being met for free.
- **WTP → 2** (unchanged): no priced comparable product was found in this space at all; only free
  OSS tools exist. "No evidence found" is neutral, not negative, but there is genuinely nothing to
  point to here.
- **Wedge → 1** (down from 2): at the original screen, only one, apparently-unused free competitor
  (nikydobrev's, 0 stars) was known. Deep Check surfaced a second, materially more active free
  competitor (wangkanai's, 207 commits) that already does automatic multi-org switching — the
  idea's core mechanism — for free, and no wedge beyond that mechanism (governance/UI/PAT vault) is
  evidenced beyond analogy. The case for "a real wedge survives next to it" (the exact question
  `STATE.md` asked Deep Check to resolve) did not hold up.
- **Owner fit → 5** (unchanged): the owner runs ~20 ADO orgs daily.
- **Buildability → 4** (unchanged): straightforward CLI/MCP wrapper work regardless of scoring
  changes elsewhere.

**Revised total: 15/25.** No hard disqualifier (H3 does not strictly apply to third-party OSS
competitors, per the `ci-migration-rollback-gate`/Bytebase precedent), but the score drops well
below both other shortlisted ideas and no longer beats `ci-migration-rollback-gate`'s 16/25 either.

## Unreachable / not attempted

- GitHub REST API calls (api.github.com) were blocked (403, "not enabled for this session") for
  both competitor repos, so exact last-commit dates and full issue-comment JSON on #812 could not
  be retrieved; relied on fetched rendered HTML instead, which returned zero visible comments on
  #812 (possibly a rendering gap, not confirmed truly empty).
- dev.azure.com's in-product marketplace was not checked separately from marketplace.visualstudio.com
  (same underlying listing, not expected to differ).
