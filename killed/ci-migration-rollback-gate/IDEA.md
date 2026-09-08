# Idea: CI-embedded migration/rollback safety gate for small teams

## Concept

A GitHub Action that inspects pull requests touching database migration files and blocks merge
unless a corresponding rollback/down migration is present, with a short human-readable diff of
what would break (data loss, irreversible operations) if the rollback were actually run.

## Target user

Small engineering teams (roughly 2-20 engineers) running their own database migrations (Rails,
Django, Alembic, Prisma, Flyway, raw SQL, etc.) in CI/CD without a dedicated platform team or
existing schema-change-management tooling.

## Problem

Migrations get merged without a way to safely undo them. When a migration causes an incident, the
team discovers only then that there is no down migration, or that the down migration is stale/
incorrect, leaving no fast way back to the previous schema state.

## Why now

No specific regulatory or platform trigger identified. This backlog entry was generated on
2026-09-07 specifically to test property (b) from that day's `DECISIONS.md` entry: value from
being embedded in a recurring CI workflow across every future PR, rather than from a single
generated artifact — as a hypothesis for what a one-shot free/DIY/LLM substitute cannot replace.
