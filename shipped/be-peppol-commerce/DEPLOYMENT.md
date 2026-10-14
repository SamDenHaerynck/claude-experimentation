# Deployment

Nothing here has been run by the routine: it never deploys and has no accounts. Every command below
is for the owner. Costs marked "not retrieved" were not fetched, so check the vendor page first.

## What there is to deploy

Two different things, and only the first is deployable today:

1. **`BePeppolCommerce.Api`**, an ASP.NET Core 8 minimal API (`GET /health`, `POST /webhooks/inbound`).
   It only receives: it acknowledges a webhook, fetches the document from the Access Point and checks
   it parses. It stores nothing (no inbound sink, see HANDOFF.md). Deploying it alone gives no
   outbound invoicing and no usable inbound output.
2. **`BePeppolCommerce.Core`**, the library. Outbound sending (`OutboundDispatcher.RunOnceAsync`) runs
   inside a host that implements `IOrderInvoiceSource` and `IAccessPointSettingsProvider`, which is
   the Optimizely Configured Commerce extension. That extension does not exist (no install to build
   it against). See `app/docs/CONFIGURED_COMMERCE_CONTRACT.md`.

So the realistic first deployment is the API host as a webhook receiver plus the walkthrough as a
demo, or a thin custom host that implements the two interfaces for a pilot customer.

## Target platform

Azure App Service (Linux) for the API host, or any Linux machine with the ASP.NET Core 8 runtime.
Vercel does not fit: this is a .NET host that needs IKVM native images, not a Node app.
The Free (F1) App Service plan has 1 GB RAM and 1 GB storage and Microsoft's page says it is not
supported for production (https://azure.microsoft.com/en-us/pricing/details/app-service/linux/,
fetched day 040; the price columns did not load). Plan for Basic B1 (1 core, 1.75 GB RAM, per the
same page) as a minimum: the first validation compiles the Schematron and the whole process holds
the compiled rules.

## Resources needed

| Resource | Why | Notes |
|---|---|---|
| Access Point account | send and receive | Storecove (no public price, sales contact), or Recommand (self-serve) |
| Hosting for the API | webhook receiver | App Service B1 or similar, with HTTPS |
| TLS in front | the webhook secret travels in a header | Azure provides it; on a VM use a reverse proxy |
| A secret store | API key, secrets | App Service settings or Azure Key Vault |
| Public HTTPS URL | the provider must reach the webhook | domain is the owner's to buy |

## Environment variables

From `app/src/BePeppolCommerce.Api/.env.example` (placeholders only; do not source the file as is,
`Storecove__LegalEntityId=0` stops the host). A double underscore separates sections.

| Variable | Required | Meaning |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | yes, set to `Production` | **Never run production as `Development`:** there the webhook check is off when no secret is set (L11 in REVIEW.md) |
| `AccessPoint__Provider` | no | `storecove` (default) or `recommand` |
| `Storecove__ApiKey`, `Storecove__LegalEntityId` | for Storecove | LegalEntityId must be a positive integer |
| `Storecove__SchemeMap__0208` | likely yes for Belgium | Storecove's own name for scheme 0208 is unconfirmed; ask Storecove before the first send |
| `Recommand__ApiKey`, `Recommand__ApiSecret`, `Recommand__CompanyId` | for Recommand | company id: letters, digits, `_`, `-` |
| `Webhook__Secret` | yes outside Development | sent by the caller as `X-Webhook-Secret`; use 32+ random characters (no minimum is enforced, L10) |
| `Webhook__SigningSecret` | Recommand only | HMAC key for `X-Signature`; do not register the endpoint as a real Recommand webhook yet (HANDOFF.md) |

## Steps

1. Install the .NET 8 SDK (`app/README.md`, Prerequisites). Verify locally first:
   `cd ideas/be-peppol-commerce/app && dotnet test` (297 tests) and
   `dotnet run --project samples/BePeppolCommerce.Walkthrough` (exit 0).
2. Publish: `dotnet publish src/BePeppolCommerce.Api -p:PublishProfile=linux-x64`. Output is about
   92 MB, framework-dependent, in `src/BePeppolCommerce.Api/bin/publish/linux-x64/`. The published
   build answered `/health` (day 031) but **the validator has not been exercised from a published
   build**: run an outbound validation on the deployed copy before trusting it.
3. Create an App Service (Linux, .NET 8, B1) and a resource group, then deploy the publish folder
   (for example `az webapp deploy --resource-group <rg> --name <app> --src-path <zip> --type zip`
   after zipping the folder). Use your own names.
4. Set the variables above as App Service application settings (Key Vault references for secrets).
5. Check `GET https://<host>/health`, then `POST /webhooks/inbound` without the secret (expect 401)
   and with it plus a document id from the provider's sandbox.
6. Add rate limiting or an IP restriction at the front door (the host has none, L10).

## Monthly cost

Not retrieved: Azure App Service prices (the page did not load them), Storecove (not public).
Retrieved on day 024 (`VALIDATION.md`): Recommand free up to 25 documents a month, then 29 to 349 EUR
a month; Billit 7.50 to 250 EUR; Qvalia 9 to 899 EUR.

| Load | Hosting | Access Point | Total |
|---|---|---|---|
| Zero users | one App Service B1 (price: check Azure calculator) | Recommand free tier, or a sandbox | about the hosting cost alone |
| About 100 customers | same, possibly B2 | depends on documents per month: each customer's volume sets the tier, and Recommand pools volume across companies | not estimated; no usage data exists |

## What breaks first under load

- **Memory and cold start.** The first validation in a process takes about 4 to 6 s (rules compile,
  measured day 028), then about 25 ms. Scale-out copies each pay it; restarts and slot swaps too.
  Warm up with one validation at startup.
- **Single dispatcher run.** `OutboundDispatcher` must run on one node at a time; two concurrent runs
  can send the same invoice twice. Scaling out the extension needs a lock.
- **Retry without a cap.** A permanently failing retryable invoice is retried every run (HANDOFF.md).
- **The failed-document log is in memory** (newest 1000, lost on restart); production needs a durable one.
- **Access Point rate limits**: unknown, not documented in the provider specs read.
