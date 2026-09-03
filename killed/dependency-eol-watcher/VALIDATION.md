# Validation: Dependency deprecation/EOL watcher

## Competitors and pricing (real, retrieved)
- **GitHub Dependabot** — free, built into GitHub, no separate license. Covers version updates and
  security-advisory-based alerts (GitHub Advisory Database, 20,000+ advisories).
  https://appsecsanta.com/sca-tools/snyk-vs-dependabot
- **Renovate** (open source / Mend-hosted) — core dependency-update functionality free via the
  Community tier; free hosted app for most use cases; paid tiers only for larger orgs wanting
  support/extra platform features. https://www.turbostarter.dev/blog/renovate-vs-dependabot-whats-the-best-tool-to-automate-your-dependency-updates
- **Snyk** — free tier capped at 200 tests/month, then per-developer paid plans (Team/Enterprise).
  Proprietary vuln database, not EOL-focused. https://appsecsanta.com/sca-tools/snyk-vs-dependabot
- **endoflife.date** — free, open-source, community-maintained database of EOL dates for 380+
  products, with a public API already built for exactly this cross-referencing use case.
  https://endoflife.date/ , https://github.com/endoflife-date/endoflife.date
- **HeroDevs "Never-Ending Support"** — a real paid product in the adjacent space, but sells
  continued security patching *for already-EOL* open source (AngularJS, Vue 2, Drupal 7, Spring),
  not detection/migration-brief tooling. No public price list found (custom quote only).
  https://www.herodevs.com/support

## Evidence of demand (first-person / practitioner sources)
- GitLab work item, filed by a practitioner against GitLab itself: "Add end of life information to
  dependency view" — direct evidence someone wants EOL data surfaced in their existing
  dependency-scanning workflow, not as a separate product.
  https://gitlab.com/gitlab-org/gitlab/-/work_items/402742
- Industry commentary (not first-person, but corroborating): "quiet abandonment — where the
  maintainer stops responding to issues... without formal announcement — is the most common form of
  EOL... and the hardest to detect." https://tuxcare.com/blog/junior-devs-eol-software/
- Could not find a second or third genuine first-person forum/issue-tracker complaint (Reddit
  r/devops, r/sysadmin, Hacker News) describing an EOL-dependency incident in the poster's own
  words within the time available. Searches returned SEO roundups, general dependency-hell
  articles, or unrelated results. Recording "no further evidence found" per RUNBOOK.md rather than
  substituting listicles.

## Scoring (1-5 each)

| Dimension | Score | Why |
|---|---|---|
| Evidence of demand | 2 | One real practitioner request (GitLab issue), but it asks for the feature *inside* an existing tool, not for a new product. No second/third independent first-person complaint found. |
| Willingness to pay | 2 | The only paid comparable (HeroDevs) sells a different thing (ongoing patch support, not scanning/briefing). Dependabot/Renovate already do free, automated version-bump PRs that route around most EOL risk without anyone reading a "brief". No evidence anyone pays specifically for an EOL migration-brief tool. |
| Buildable to handoff in <=10 sessions | 4 | endoflife.date's public API plus manifest parsing (package.json, requirements.txt, etc.) is a small, well-scoped build. |
| Reason to exist alongside what ships | 2 | Dependabot/Renovate (free, already installed in most repos) and endoflife.date (free, public API) cover the data and remediation-PR flow already. The one real feature request found (above) asks incumbents to add this, which is the more likely path than a new standalone tool winning adoption. |
| Low compliance/operational burden | 4 | Reads manifest files only, no PII, no live credentials needed to demo. |

**Total: 14 / 25.**

## Verdict: KILL

Total is under the 16 threshold, and demand independently scores 2 (at the kill line on its own).
The core problem is real (industry sources agree EOL/quiet-abandonment is hard to detect) but the
market already has free tooling (Dependabot, Renovate, endoflife.date) covering the same ground,
and the one concrete piece of demand evidence found asks for this as a feature of an existing tool,
not a reason to build a new one. Not rescoring this if it resurfaces later per the routine's rules.
