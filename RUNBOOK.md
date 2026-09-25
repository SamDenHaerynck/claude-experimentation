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
- WebSearch's own synthesized summary can blend a fact from one result into a sentence citing a
  different result's URL (day 023: a "€600/day" figure ended up attributed to two pages that don't
  contain it). Only attribute a claim to a specific URL after fetching that exact page (WebFetch)
  and confirming the claim is actually on it — never attribute straight from the search summary
  text. Symmetry: a claim you did fetch and confirm on the page still counts in the idea's favour
  even if the exact wording differs from the search summary's paraphrase.