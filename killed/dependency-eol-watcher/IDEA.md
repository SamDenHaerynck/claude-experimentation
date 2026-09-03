# Idea: Dependency deprecation/EOL watcher

Source: BACKLOG.md item 1.

## Concept
A CLI/CI tool that scans a repo's manifest files (package.json, requirements.txt, etc.),
cross-references each dependency's version against deprecation/EOL/security-advisory feeds, and
produces a prioritized migration brief (which dependencies are EOL or approaching EOL, why it
matters, suggested next version).

## Target user
Engineering teams / tech leads at small-to-mid companies without a dedicated platform or security
team, who currently notice EOL dependencies only after something breaks.

## Problem
Package deprecation and end-of-life dates are scattered across many upstream sources. Vulnerability
scanners (Dependabot, Snyk, Renovate) flag known CVEs, but EOL itself (upstream stops patching,
"quiet abandonment") is a distinct signal that is easy to miss until a CVE lands on a version nobody
is patching anymore.

## Why now
No specific trigger found beyond the general, ongoing growth of dependency trees and increasing CVE
volume. See VALIDATION.md — this claim is weak and did not move the demand score.
