# Validation: Dependency deprecation/EOL watcher

## Competitors and pricing (real, retrieved)

1. **Snyk** — SCA/vuln scanning that already surfaces deprecated/unmaintained packages as part of
   its scan. Team plan $25/contributing-developer/month; Enterprise custom (median reported
   contract ~$45k/year). Source:
   https://www.trustradius.com/products/snyk/pricing
2. **Mend (Renovate)** — free hosted Community edition (GitHub/Bitbucket Cloud) automates version
   bumps; paid Enterprise tier is $250/contributing-developer/year, and Mend AppSec (bundles SCA +
   SAST + Renovate + component inventory) is $1,000/year. Source:
   https://www.mend.io/pricing/ and https://github.com/renovatebot/renovate/discussions/29598
3. **FOSSA** — open-source dependency/license/vuln management. Business plan $20/project/month
   (billed annually), scales with contributing developers. Source: https://fossa.com/pricing/

## Direct competitive threat to the specific niche (EOL/deprecation data itself)

- **endoflife.date** — a free, community-maintained, open API that already answers "is this
  version EOL" for hundreds of products, purpose-built for exactly this problem. Its own creator
  states the motivation in their own words (see evidence below). Source:
  https://news.ycombinator.com/item?id=34550325
- **GitLab** has an open internal work item to pull `endoflife.date` data directly into its
  existing (already-paid-for) dependency dashboard — i.e., a major platform is planning to ship
  this as a feature bundled into a product teams already pay for, not sell it standalone. Source:
  https://gitlab.com/gitlab-org/gitlab/-/work_items/402742
- **Socket.dev** already publishes on and flags deprecated/abandoned npm packages as part of its
  supply-chain security product. Source: https://socket.dev/blog/the-risks-of-deprecated-npm-packages

## First-person evidence of the problem (real, retrieved)

1. Creator of endoflife.date, explaining why he built it: *"I was frustrated having to lookup
   information on multiple sites, and having to dig deep and read through terribly written support
   policies."* — captn3m0, Hacker News, https://news.ycombinator.com/item?id=34550325
2. *"how to keep track of dependencies and plugin versions in all layers of our apps and, say, not
   miss a lot of versions"* — Cécile Fécherolle, asking the community how they cope, DEV Community,
   https://dev.to/cfecherolle/what-are-your-tools-for-keeping-dependencies-and-plugins-versions-up-to-date-126g
3. *"your team has to remember to make the updates and yes.....they will forget without
   reminders"* — Bradston Henry, reply on the same thread, same URL as above, describing manual
   wiki-based tracking failing without dedicated ownership.

## Scoring (1-5 each)

- **Evidence of demand: 3.** The pain is real and recurring (three independent first-person
  accounts above), but it reads as a low-grade chronic annoyance, not an acute, budget-unlocking
  problem — nobody in the evidence describes an incident that made them go looking for a paid
  tool, only for a better way to keep a personal/team list current.
- **Willingness to pay: 2.** The specific data this idea is built on (EOL dates) is already free
  and public via `endoflife.date`'s open API — the exact resource the strongest evidence quote is
  about. The buyer segment that does pay for dependency risk (Snyk/Mend/FOSSA customers, $20-25 per
  dev/month) already gets deprecated-package flagging bundled into tools they're already paying
  for, and the platform most likely to own this natively (GitLab) is building it as a bundled
  dashboard feature, not a paid add-on. No evidence anyone would pay separately for this exact
  slice.
- **Buildable to handoff in ≤10 sessions: 4.** Technically straightforward — manifest parsing per
  ecosystem, calls to `endoflife.date` and OSV.dev APIs, a ranked report. No novel engineering risk.
- **Reason to exist alongside what already ships: 2.** `endoflife.date` covers the core data for
  free; Renovate/Dependabot already automate the fix once you know a version is outdated; Snyk/
  Socket already fold deprecated-package detection into tools teams already buy. The remaining gap
  — a standalone prioritized migration brief — is thin and easily absorbed as a feature by any of
  the above, several of which are already moving that direction.
- **Low compliance/operational burden: 4.** Static analysis over manifests and public APIs, no PII,
  no live infra dependency for the core function.

**Total: 15/25.**

## Verdict: KILL

Total is under the 16 threshold, and independent of that, willingness to pay scores 2, which is an
automatic kill per the routine's rule ("Kill the idea if... demand, willingness to pay, or
buildability scores 2 or below"). The core data this idea would resell is already a free public
good, the paying audience already has this bundled into tools they use, and the platform best
positioned to own it natively is already building toward that. No evidence found of anyone willing
to pay specifically for this slice, standalone.
