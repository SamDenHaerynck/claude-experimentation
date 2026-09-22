# Automated weekly financial snapshot for solo/micro-business owners on QuickBooks Online

## Concept
Connects to one small business's QuickBooks Online account, auto-flags overdue invoices and new
bank-feed transactions needing categorization/review, and pushes a weekly P&L/balance-sheet/
cash-flow snapshot — aimed at solo/micro-business owners currently paying a human $8-15/hr on
freelance platforms to do this by hand, not at bookkeeping firms managing many clients.

## Target user
A solo or micro-business owner on QuickBooks Online who does not employ a bookkeeper and instead
hires cheap freelance labor (or does it themselves) to keep invoices current, categorize bank-feed
transactions, and produce periodic financial statements.

## Problem
Per the sourcing job posting, the target user wants someone to weekly: "enter new customer
invoices, apply payments, and flag any overdue balances," "record and categorize expenses from
bank feeds, receipts, and credit-card statements," and "generate the P&L, balance sheet, and
cash-flow snapshot."

## Why now
No regulatory or platform trigger. Sourced via Channel B: a Freelancer.com posting
(https://www.freelancer.com/projects/data-entry/weekly-quickbooks-bookkeeping-reporting,
$8-15 USD/hr, recurring weekly, independently re-verified via direct fetch in a prior session).

## Property case (from `BACKLOG.md`)
- (a) plausible: bank feeds/invoices change continuously.
- (b) plausible: tied to a recurring weekly cadence, not a one-shot artifact.
- (c) plausible: a wrong P&L number has real cost.

## Highest-priority risk flagged in `STATE.md`
QuickBooks Online itself already provides live P&L/balance-sheet/cash-flow reports and bank-feed
categorization rules natively, in the base subscription — the classic "already-recurring free
incumbent" failure mode that has killed several prior candidates. See `VALIDATION.md` for the
Phase 1 check of this risk and of the disclosed-but-unverified non-competitors (Dext/Hubdoc,
Chaser, Upflow, Zeni, Puzzle).
