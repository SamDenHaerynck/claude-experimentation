# Runbook

Short checklist, under 150 lines. Move resolved entries to RUNBOOK-archive.md (reference only, do
not read at session start). OWNER.md wins on conflicts.

## Git
- Push 403 about GitHub access: one retry, commit locally, notify immediately (stand-alone message).
- Branch holds only merged history: git checkout -B <branch> origin/main, --force-with-lease if needed.
- Previous work not on main: merge its open PR, or open one, or re-derive from the day log.
- wip: commit or dirty tree: finish if it clearly fits, else revert and re-plan smaller.
- Review has high findings or can't run: don't merge, leave PR open, note for owner.
- Past minute 50: stop, commit wip:, record next step.

## Research
- Blocked (one attempt max): Reddit, G2, Upwork, eng-tips, Mike Holt, ElectricianTalk,
  ContractorTalk, JLC, Atlassian Marketplace reviews. Reachable: Capterra, Software Advice, GetApp,
  Trustpilot, vendor feedback boards, Shopify App Store, Freelancer.com. Blocked = neutral.
- Subagents fabricate quotes on real URLs: brief them for paraphrase + URL only.
- Subagents miss competitors on cited pages: ask for every product named. A paid competitor also
  counts as demand and WTP evidence.
- H3 check: platform's own docs, and which plans include the feature.
- Symmetry rule: every new check that can lower a score must say what evidence counts in favour.
- A research note can end up attributing a figure to a URL that doesn't actually contain it (day
  023: a "€600/day" figure was attributed to two pages that, on direct refetch, don't contain it
  and couldn't be traced to any source). Only attribute a claim to a specific URL after fetching
  that exact page (WebFetch) and confirming the claim is actually on it — never attribute straight
  from a search tool's synthesized summary text. Symmetry: a claim you did fetch and confirm on the
  page still counts in the idea's favour even if the exact wording differs from the search
  summary's paraphrase.

- API docs site is a JS app with no visible spec link (day 033, Recommand): check whether the API
  source is public on GitHub, `git clone` it and grep the server for its `/openapi` route, then curl
  that route on the API host. Got the raw spec in about 5 minutes.

## Build environment
- `dotnet` is not preinstalled in the session container (day 026). Install the .NET 8 SDK first:
  `curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh && bash
  /tmp/dotnet-install.sh --channel 8.0 --install-dir $HOME/.dotnet`, then
  `export PATH=$HOME/.dotnet:$PATH`. Takes about a minute; NuGet restore works through the proxy.
- Maven Central through the proxy: plain `curl` works, but repeated fetches can return HTTP 429 as a
  small text file saved under the `.jar` name, so check the result with `file`. Java-based tools
  such as `IKVM.Maven.Sdk` fail with PKIX errors because their trust store lacks the proxy CA (day 027).
  GitHub REST API calls to other repos are blocked (403), but `git clone` of public repos works.
