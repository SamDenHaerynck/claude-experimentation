# Validation: BE-Peppol-Commerce

Deep Check, day 024 (Phase 1, tournament round 1, shortlist #1). Idea: an Optimizely Configured
Commerce connector/extension that generates and receives Peppol BIS/EN16931 e-invoices for Belgian
B2B buyers, integrating as an API client of an already-accredited Peppol Access Point provider
(not itself accredited).

## H2 question (resolved first, per `STATE.md`'s day-023 next action)

**Question:** does shipping Peppol e-invoicing require the product itself to be an accredited
Peppol Access Point (a licence — would trigger H2), or can it integrate as a client of an existing
accredited AP's API (no licence needed)?

**Answer: it can integrate as a client. H2 does not apply.** Evidence (all fetched and confirmed
on the page unless marked otherwise):

- peppol.nl explicitly lists three paths for a software vendor: connect via an existing certified
  service provider's API, become a service provider yourself (OpenPeppol membership + Peppol
  Authority agreement + ISO 27001 + PASR), or a hybrid. The client-of-an-API-route is listed first,
  as a normal option, not a workaround.
  https://www.peppol.nl/en/connect-peppol/connect-peppol-software-provider
- OpenPeppol's own "For Service Providers" page confirms the four-corner model and that becoming
  an accredited Service Provider (Access Point) is a distinct, separate role from an end-user
  application. https://peppol.org/about/for-service-providers/
- Storecove markets directly to software vendors on this exact basis: "you don't need to go through
  the Peppol certification process since you will be using our Peppol certificates."
  https://www.storecove.com/blog/en/how-to-become-a-peppol-access-point/, and frames the choice as
  build-your-own-accreditation vs. partner-with-an-accredited-AP:
  https://www.storecove.com/blog/en/6-considerations-for-becoming-peppol-access-point/
- Qvalia's partner page states that its "Embedded"/"White-Label" delivery models remove "the need
  for your own Peppol certification, audits, and ongoing Peppol governance"; only its separate
  "Hosted Access Point" product requires the partner to get its own accreditation.
  https://qvalia.com/peppol-infrastructure-white-label-for-partners/
- Recommand's docs describe the four-corner model with the client application at Corner 1 calling
  Recommand's API, while Recommand (the accredited AP) handles Corner 2 transmission.
  https://docs.recommand.eu/docs/peppol-network-basics
- Real examples of software that ships Peppol support this way today, without being an AP itself:
  Woo2Billit (WooCommerce plugin, calls Billit's API) — https://woo2billit.eu/documentation/; the
  Odoo OCA community connector, which requires a separate third-party AP subscription (Basware,
  Pagero, Storecove, etc.) per https://www.invoicenavigator.eu/software/odoo/peppol. (For contrast:
  Odoo SA's own first-party module chose the opposite path and self-accredited as both AP and SMP —
  https://www.odoo.com/documentation/19.0/applications/finance/accounting/customer_invoices/electronic_invoicing.html
  — showing both models coexist and neither is mandatory.)

**Not independently verified:** the OpenPeppol Transport Infrastructure Agreement's own legal text
(accreditation-step detail here rests on Storecove's summary of it, not a direct fetch of the
primary document). Doesn't change the answer — no source found contradicts the client-of-an-AP
path being license-free — but flagged per the evidence rules.

**Conclusion: H2 does not apply.** The connector calls a Peppol AP provider's API (e.g. Storecove,
Qvalia, Recommand, or Billit); it does not itself need OpenPeppol accreditation.

## H3 check: does Optimizely Commerce already ship this free?

**No.** Checked and confirmed empty:
- world.optimizely.com developer-community search for "peppol" returns no hits (fetched).
- The Optimizely apps marketplace (optimizely.com/apps/all, 334 listed connectors) has no
  invoicing/Peppol/tax/EDI category or listing; visible ERP/tax/payment connectors are Infor CSD,
  IFS Aurena, SX.e, Avalara, Spreedly — none e-invoicing (fetched).
- No Optimizely/Episerver-named integration found on any of Storecove, Qvalia, Recommand, Billit,
  Basware, Pagero, or Unifiedpost's sites or in search results.

H3 does not apply — no hard disqualifier found.

## Wedge

Peppol-to-ERP/commerce connectors already exist for **every major competing commerce/ERP
platform except Optimizely Commerce**:
- Commerce-Connections' Peppol AP connects to SAP, Microsoft Dynamics 365 Business Central, Oracle
  NetSuite, MuleSoft, Xero, and Sage 50 — https://commerce-connections.com/partner/peppol (fetched).
- e-invoice.be's Peppol REST API lists compatibility with SAP, Oracle, Dynamics, NetSuite, Odoo,
  Sage, Exact, Twinfield, and Yuki — https://e-invoice.be/peppol-api (fetched).
- PeppolCommerce.eu markets a dedicated Peppol plugin for WooCommerce B2B webshops — a direct
  analog of this idea, but for a different e-commerce platform (surfaced in search, not fetched in
  depth — flagged as a follow-up, not treated as confirmed).

Optimizely Configured Commerce, used by B2B sellers across Belgium and the wider EU (the owner's
own client base), currently has no path to Peppol compliance for the mandate that took effect
2026-01-01 — sellers on this platform must either build a custom integration themselves or migrate
off Optimizely for this capability. That gap, on a platform the owner already works in daily for
Belgian/EU clients, is the wedge.

## Willingness to pay / comparable pricing

Three of the four likely backend AP providers this connector would call publish transparent
self-serve pricing, establishing a real price floor for Peppol connectivity itself (this idea would
price on top of, not instead of, one of these):

| Provider | Pricing | Source |
|---|---|---|
| Qvalia | €9-€899/mo tiered by message volume (25 to 30,000 msgs/mo), enterprise by request | https://qvalia.com/pricing (fetched) |
| Recommand | Free up to 25 docs/mo; €29-€349/mo tiered (200-5,000 docs), volume pooling across companies | https://www.recommand.eu/pricing (fetched) |
| Billit | €7.50-€250/mo tiered (≤25 to ≤1,000 docs/mo), enterprise on request; Peppol network access itself free once on a paid tier | https://www.billit.eu/en-int/pricing/ (fetched) |
| Storecove | Not public — custom quote / 30-day test account, developer/OEM sales motion | https://www.storecove.com/us/en/solutions/peppol-access-point/ (fetched, confirmed no pricing shown) |

This is demand/WTP evidence for Peppol connectivity in general, not specifically for an Optimizely
connector (no existing product charges for that specific integration, since none exists — see
Wedge above). The connector's own price would need to cover the Optimizely-specific integration
work on top of one of these providers' base fee; a plausible v1 model is a flat monthly connector
licence (e.g. €50-150/mo) plus the customer's own AP subscription, comparable to how
Commerce-Connections and e-invoice.be price per-platform connectors — but no direct per-connector
price was found for either (their sites quote the AP subscription only, not a separate per-ERP
connector fee); recorded as "no evidence found" for that specific number, not invented.

## Scoring (unchanged from day-022 screen, re-confirmed in Deep Check)

Demand 4, WTP 4, Wedge 4, Owner fit 5, Buildability 3 — **Total 20/25.** No hard disqualifier
(H2 and H3 both checked and cleared above). This is currently the only shortlisted idea confirmed
clear of all four hard disqualifiers with a Deep Check on file; per `OWNER.md`'s Pick rule, if
CMS12-UpgradeAssist and ADO-MultiOrg's Deep Checks (next session) don't beat 20/25, this idea wins
the tournament.

## Unreachable / not attempted this session

- The Optimizely apps marketplace's full catalog (334 items) was only browsed via its featured
  subset, not paginated through exhaustively — a very niche unlisted invoicing connector can't be
  100% ruled out, though none turned up via marketplace, provider-side, or forum search.
- PeppolCommerce.eu (WooCommerce analog) was not fetched in depth.
- Isabel, Tickstar (other named EU Peppol APs) were not fetched directly, only seen in search
  results.
- No direct fetch of a named Optimizely partner or systems-integrator statement about Peppol demand
  from their own client base (would strengthen demand evidence further; not required to clear the
  hard-disqualifier check or the 12/25 floor).
