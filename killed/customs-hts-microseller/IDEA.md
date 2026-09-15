# Idea: Small-batch customs/HTS classification and duty paperwork for micro e-commerce sellers

## Concept
A tool that takes a product description (and optionally a photo) and generates: the correct HTS
(Harmonized Tariff Schedule) code, a commercial invoice, and a duty estimate for a single
occasional international shipment.

## Target user
Etsy sellers, small Shopify store owners, and independent craftspeople who ship internationally in
small, irregular volume (a handful of parcels a month, not a platform-integrated high-volume
pipeline). Not aimed at established importers/exporters who already have customs brokers.

## Problem
Executive Order 14324 ("Suspending Duty-Free De Minimis Treatment for All Countries") eliminated
the $800 de minimis exemption worldwide effective 2025-08-29
(https://www.whitehouse.gov/presidential-actions/2025/07/suspending-duty-free-de-minimis-treatment-for-all-countries/).
Before this, most small-value parcels moved without formal customs entry or an HTS code. Now every
parcel needs one, and misclassification penalties under 19 USC 1592 run 20-40% of underpaid duty
for negligence, up to 4x for fraud. This is a brand-new compliance burden for a segment (occasional
small sellers) that never had to think about HTS codes or formal customs entry before.

## Why now
The regulatory change is dated (2025-08-29) and permanent-until-changed, not speculative. It
affects every US-bound international parcel, not a narrow niche.

## Named competitor (disclosed, not to be treated as automatic kill)
Zonos (zonos.com/classify, zonos.com/landed-cost) — offers free HS classification and landed-cost
tools, but built for platform-integrated, higher-volume stores (Shopify/BigCommerce apps, API
integration), not for an occasional seller doing one manual shipment at a time. The disclosed gap
Validate must actually test.

## Validation to run today
- At least 3 existing/adjacent competitors with pricing (Zonos + 2 more).
- At least 3 real first-person user-voice posts with URLs evidencing the specific differentiator
  (occasional/manual small-volume sellers needing HTS+duty help post-de-minimis-repeal).
- Deep review-page complaint check on named competitors.
- Underserved-sub-segment search.
- Score 1-5 on: demand, willingness to pay, buildability (<=10 sessions), reason to exist alongside
  incumbents, compliance/ops burden. Kill if total < 16, or if demand/WTP/buildability <= 2.
