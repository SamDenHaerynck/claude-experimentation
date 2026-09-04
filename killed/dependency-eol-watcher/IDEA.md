# Idea: Dependency deprecation/EOL watcher

## Concept

A tool that scans a repo's manifest files (`package.json`, `requirements.txt`, `pom.xml`, etc.),
cross-references every dependency against deprecation/EOL/security-advisory feeds, and produces a
prioritized migration brief: which dependencies are already end-of-life, which are approaching
EOL, and what to do about each, ranked by risk.

## Target user

Engineering teams (5-50 devs) without a dedicated platform/security engineer — the gap between
"too small to have someone whose job this is" and "too exposed to ignore it."

## Problem

Dependency EOL/deprecation status is scattered across per-project changelogs, vendor support
pages, and community trackers. Nobody owns watching it continuously, so teams find out a
dependency is unsupported only when it breaks, gets flagged in an audit, or is exploited.

## Why now

No specific regulatory or platform trigger identified — this is a persistent, not a newly created,
pain point. Framed as "narrow tool for a professional audience" per the backlog rationale.
