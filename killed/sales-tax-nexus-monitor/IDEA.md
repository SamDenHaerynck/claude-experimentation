# Idea: State/local sales-tax nexus threshold monitor with change alerts

Source: `BACKLOG.md` #1 (re-ranked 2026-09-07).

## Concept

Track a small e-commerce seller's revenue/transaction count per US state against each state's
economic-nexus threshold, and alert before a new sales-tax filing obligation is triggered.

## Target user

A solo or small-team e-commerce seller (Shopify/WooCommerce/multi-marketplace) selling into
multiple states, without an in-house tax/finance function, who does not already use a full tax
compliance suite (Avalara/TaxJar).

## Problem claimed

Economic-nexus thresholds vary by state (revenue and/or transaction count, different measurement
windows) and can change by state legislation over time. A one-time calculation of "am I over the
threshold today" goes stale as sales grow and rules change; the seller needs ongoing tracking, not
a single answer.

## Why now / rationale from BACKLOG.md

Tests property (a) from the `RUNBOOK.md` 2026-09-07 framework: value from data that changes on an
ongoing basis and must be kept current. `BACKLOG.md` flagged TaxJar/Avalara as likely incumbents to
check specifically, and asked whether a narrow, cheap alternative has room next to them.
