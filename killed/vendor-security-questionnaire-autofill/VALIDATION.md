# Validation: Vendor security questionnaire autofill assistant

## Competitors and pricing (real, retrieved directly from vendor pages)

1. **Conveyor** — questionnaire automation + trust center. Business tier starts at
   $9,600/year (unlimited seats, 20 questionnaire credits, 10 RFP projects); Enterprise is custom.
   Source: https://conveyor.com/pricing (fetched directly)
2. **1up.ai** — AI security-questionnaire/RFP automation. Plans start at $250/month, not
   volume-based, 14-day free trial; case studies (WalkMe, JumpCloud, Continu) indicate a
   mid-market/enterprise customer base. Source: https://1up.ai/automate-security-questionnaires
   (fetched directly)
3. **AutoRFP.ai** — Scale tier $899/month (24 projects/year), Accelerate tier $1,299/month (50
   projects/year), both billed yearly, unlimited users; Enterprise custom. Source:
   https://autorfp.ai/pricing (fetched directly)

## Direct competitive threat to this exact niche (AI-drafted answers from a past-answer library)

- **Stacksi** — YC-backed, Show HN'd directly into this problem: "Our system typically gets 90%+
  of the questionnaire completed with no user involvement," positioned as "software-enabled
  service" with a human in the loop for gap remediation guidance — i.e., already doing the core
  loop this idea proposes, for the same target buyer. Source:
  https://news.ycombinator.com/item?id=26513327 (fetched directly)
- **Loopio / RFPIO (Responsive)** — established RFP-response platforms with AI autofill features
  (Loopio "Magic"), pre-dating this idea, already used as fallback answers in the Stacksi HN
  thread above. Same source.
- **SafeBase** — markets "AI Questionnaire Assistance" explicitly to "eliminate the tedious
  back-and-forth response process." Source: https://www.g2.com/products/safebase/reviews (search
  snippet from G2 product page)
- Also observed in market-scan searches but not individually price-verified: Vanta, Secureframe,
  Whistic, Vendict, Thalamus AI, Arphie.ai, Orbiq — all offering AI-drafted questionnaire response
  products as of 2026. Several vendor blog posts about "how startups should handle security
  questionnaires" that turned up in search are themselves marketing content from these
  competitors, not independent evidence, and were excluded from the demand evidence below.

## First-person evidence of the problem (real, retrieved directly)

1. *"Whenever we get a client with around 100+ head count, they ask to fill their own security
   assessment. It takes a lot of time as it has sometimes 100+ questions... We're too small to
   hire someone for this and as a founder, my time can surely be used better somewhere else."* —
   original poster, Ask HN: "How small startups deal with long security questionnaires from
   clients?", June 2023. https://news.ycombinator.com/item?id=36488436
2. *"These questionnaires are time consuming and redundant"* — original poster describing a
   100-question assessment from an integration partner, two-person team with no SOC 2 or
   third-party pentest despite handling medical/financial data. Ask HN, 2019.
   https://news.ycombinator.com/item?id=20267619
3. *"Despise security questionnaires, so a very important problem you're solving."* — commenter on
   the Stacksi Show HN, responding to a product built to automate exactly this. 2021.
   https://news.ycombinator.com/item?id=26513327
4. Same 2023 HN thread, commenter `iamflimflam1` suggests the practical workaround people already
   use instead of buying a tool: "build a database of the questions and your answers so that you
   already have most of the answers close at hand," and commenter `bennyelv` reports success using
   GPT-4 directly: "answers were as accurate as a human doing it!" — i.e., the DIY substitute
   (a spreadsheet + general-purpose LLM) is already perceived as good enough by at least one
   practitioner in the target segment. https://news.ycombinator.com/item?id=36488436

## Scoring (1-5 each)

- **Evidence of demand: 4.** The pain is real, recurring, and spans multiple years (2019, 2021,
  2023) and multiple independent threads, with founders explicitly naming lost time and no budget
  for a dedicated hire.
- **Willingness to pay: 4.** Verified real pricing from $250/month (1up.ai) to $1,299/month
  (AutoRFP.ai) to $9,600/year (Conveyor) shows a market that already pays for this, and one HN
  commenter cites $35,000 for SOC 2 compliance work as the going alternative cost — the target
  segment clearly has budget for compliance-adjacent pain when the deal is big enough.
- **Buildable to handoff in ≤10 sessions: 3.** The core loop (store past Q&A pairs, match new
  questions via embedding/keyword search, draft a response) is buildable as a scoped web app in
  that budget, but doing the answer-matching and document-import (PDF/XLSX questionnaires in the
  wild come in inconsistent formats) well enough to beat "paste into ChatGPT" is real product work,
  not a weekend feature.
- **Reason to exist alongside what already ships: 1.** This is an already-crowded, well-funded
  category doing exactly this loop for exactly this buyer: Stacksi (YC-backed) claims 90%+
  autofill with a human-in-the-loop model aimed at the same small-vendor gap; Conveyor, 1up.ai,
  AutoRFP.ai, SafeBase, Vanta, Secureframe, Whistic, Vendict, Thalamus AI, Arphie.ai and Loopio/
  RFPIO all sell AI-drafted questionnaire responses as of 2026. On top of the crowded paid
  category, the cheapest viable substitute — a spreadsheet of past answers plus a general-purpose
  LLM — is already reported working "as accurate as a human" by a practitioner in the exact target
  segment (evidence item 4 above). A new narrow entrant has no identified wedge against either the
  incumbents or the free DIY path.
- **Low compliance/operational burden: 3.** The product would store customers' security-posture
  answers (SOC2/InfoSec details) — not regulated PII, but sensitive enough that a breach or leak
  would be reputationally damaging to a company selling trust-related tooling specifically.

**Total: 15/25.**

## Verdict: KILL

Total is under the 16 threshold. Independent of the total, "reason to exist alongside what already
ships" would score 1/5 on its own — this is one of the more saturated categories found so far, with
a YC-backed direct competitor (Stacksi) explicitly targeting the same small-vendor gap this idea
was written for, several more well-funded AI-native entrants (Conveyor, 1up.ai, AutoRFP.ai,
SafeBase, and others), and a credible free substitute (spreadsheet + general LLM) already reported
as "good enough" by a practitioner in the target segment. No identified wedge against either.
