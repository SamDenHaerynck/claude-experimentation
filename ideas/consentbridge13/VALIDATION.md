# ConsentBridge13 - Deep Check (2026-10-19)

## H3 verdict: PASS (not disqualified)
No evidence found that Optimizely CMS 13, DXP, Web Experimentation or Opti ID ships a consent banner/CMP.
- Optimizely's own consent doc for Web/Feature Experimentation tells customers to handle consent themselves (optOut API triggered by your own cookie banner, or load snippet via tag manager after consent); it mentions no built-in banner. https://docs.optimizely.com/experimentation-strategy/docs/configure-cookie-and-local-storage-consent
- CMS 13 cookie page only lists cookies the platform sets (apt.uid/apt.sid telemetry, antiforgery, Identity, EPi:NumberOfVisits, EpiStateMarker, EPiViewedPages, ImageEditorFileSize, _utm*). It does not say CMS 13 has a consent feature. https://docs.optimizely.com/cms-13/docs/cookie-usage
- Caveat: I did not fetch Opti ID or DXP-specific docs; absence is "no evidence found", not proof. I did not verify CMS 13 GA date.

## 1. Competing packages/products found
| Product | Evidence | Price |
|---|---|---|
| CookieInformation.Optimizely (NuGet) | v2.2.1.8, last updated 10/13/2025, unlisted, "no owners, not actively maintained", depends EPiServer.CMS.UI.Core >= 12.13.2 and < 13.0.0, 44.4K downloads, no source repo listed. https://www.nuget.org/packages/CookieInformation.Optimizely/2.2.1.8 and https://packages.nuget.org/packages/CookieInformation.Optimizely | package free; needs Cookie Information subscription |
| Cookie Information plugin features | banner, 40+ languages, privacy widget, scans, consent stored on EU servers up to 5 yrs, helper methods for consent-based code, HTTP module stripping non-consented cookies, Consent Mode v2. CMS version support not stated on the page. https://cookieinformation.com/?p=140950 | see section 2 |
| CookieTractor (Obvi Use AB) connector | Optimizely connector listing: JS snippet or GTM plus a Dashboard package from the App Directory; gates experiments/personalization on consent; page names no CMS versions and no price. https://www.optimizely.com/connectors/obvi-use-ab . (A search snippet said CMS 12.0.3+; I did not fetch a page confirming that, treat as unverified.) | not shown on listing |
| OneTrust, Osano, Cookiebot | Mentioned by forum posters as script-injection integrations; no Optimizely-specific CMS add-on seen. https://world.optimizely.com/forum/User-Forum/end-user/Thread-Container/2022/12/recommended-solutions-for-cookie-consent-solutions-with-optimizely-12/ | n/a |
| NuGet searches | My nuget.org search page and search API fetches returned 0 hits for "optimizely cookie consent" and "episerver cookie" (the search API result is likely unreliable via the proxy; also the unlisted package would not appear). Other NuGet/Marketplace consent packages: no evidence found. | n/a |
Note: other web results (DataGrail, ConsentStack, FlexyConsent) are generic CMP-to-Optimizely-Experimentation script integrations, not CMS add-ons; I did not fetch them.

## 2. Vendor pricing (all EUR, ex VAT, fetched pages)
- Cookie Information: Essential from 19.50/domain/month (up to 500 URLs; 40 up to 5,000; 61 up to 15,000), 25,000 monthly consents, Consent Mode v2 included; Business from 36/domain/month; custom for multi-site. https://cookieinformation.com/pricing/
- CookieTractor: Small 25/mo (250/yr, 5,000 pages), Medium 40/mo (400/yr, 15,000), Large 60/mo (600/yr, 50,000); per main domain; 25% discount at 4+ domains. Page does not mention Optimizely. https://cookietractor.com/pricing
- Cookiebot: Free (50 subpages, 1 domain); Premium Lite 7; Small 15; Medium 30; Large 50; XLarge 90 per month per domain; daily scans +99/mo. https://www.cookiebot.com/en/pricing/
- Usercentrics Web CMP: Free; Essential 7; Plus 15; Pro 30 (3 domains); Business 50 (10 domains) up to higher tiers 750/mo; Corporate custom. https://usercentrics.com/pricing/
Implication: vendor CMP fees are 7-60 EUR/month/domain, so an add-on's WTP is bounded by that; it would be priced as a one-off or small annual license for agencies, not a CMP replacement.

## 3. Demand evidence (weak to moderate, all older than CMS 13)
- Forum 2020 thread: asks if a Marketplace add-on exists for opt-in/opt-out consent; a 2022 reply from Andreas Lindholm calls consent a basic core feature and says he is working on a CMS 12 solution; 2024 reply recommends Cookie Information's ready-made integration. https://world.optimizely.com/forum/developers-add-ons-forum/Addons/Thread-Container/2020/3/cookie-consent-opt-inopt-out-feature-on-episerver-platform
- Forum Dec 2022 - Feb 2023: Andreas Lindholm asks which consent solutions integrate well with Optimizely 12; four different answers (OneTrust, Cookiebot, Cookie Information, Osano), no consensus. https://world.optimizely.com/forum/User-Forum/end-user/Thread-Container/2022/12/recommended-solutions-for-cookie-consent-solutions-with-optimizely-12/
- Forms forum Jun 2021: dev needs EPiForm cookies (.EPiForm_VisitorIdentifier, .EPiForm_BID, EPiForm_{FormGuid}) stored only on consent; answers are hand-rolled workarounds (override FormContainer; custom IVisitorIdentifyProvider relying on internal API). Direct evidence for the consent-gated Forms wedge. https://world.optimizely.com/forum/developers-add-ons-forum/Forms/thread-container/2021/6/how-to-store-cookies-only-if-user-consent-to-them
- Nuget download signal: 44.4K total, about 33/day for the legacy package (a proxy for installed base on CMS 12). https://packages.nuget.org/packages/CookieInformation.Optimizely
- CMS 13-specific demand (forum, GitHub issues): no evidence found. Reddit/G2/Upwork not attempted.

## 4. Wedge, buildability, owner fit
- Wedge: consent-gating of Optimizely's own server-side cookies (Forms visitor cookies, personalization cookies like EPi:NumberOfVisits/EPiViewedPages) and CMS 13 compatibility. Data point: the only Forms-consent answers on the forum are internal-API hacks (above), and the incumbent package hard-caps below CMS 13 and is unlisted/ownerless (NuGet page above). Cookie Information's own plugin has an HTTP module and helper methods, so the gap is mostly the CMS 13 port plus Forms, not a new concept; the vendor could republish.
- Risk: a vendor-agnostic wrapper has thin differentiation if Cookie Information ships a CMS 13 update (no evidence found either way). Most forum integrations are plain script injection, so many teams may not need an add-on.
- Buildability in ~10 sessions: plausible for a CMS 13 add-on with admin settings, a script-injection/consent-gated block, a Forms container gate, a cookie-stripping middleware and Consent Mode v2 default-denied snippet, targeting 1-2 vendors first. CMS 13 API specifics not researched here.
- Owner fit: 5 (Optimizely dev at Belgian consultancy; the target market is his own clients; CookieTractor is Swedish and Cookie Information Danish so the Benelux angle is open but unproven).

## 5. Scores (1-5)
- Demand 2 (real but old, CMS 12-era, small threads; nothing for 13)
- WTP 2 (vendor CMPs cost 7-60 EUR/mo; add-on value is agency time saved; no price evidence for add-ons)
- Wedge 3 (Forms/server-cookie gating and CMS 13 gap are evidenced; vendor could close it)
- Owner fit 5
- Buildability 4
- Total 16/25

## Unreachable / not done
Reddit/G2/Upwork not attempted. nuget.org search returned 0 results (unreliable). Cookie Information's support article for the Optimizely integration returned 404. Opti ID/DXP docs, Optimizely App Marketplace listing, GitHub issues, Cookie Information pricing beyond page start (page text truncated) not checked.
