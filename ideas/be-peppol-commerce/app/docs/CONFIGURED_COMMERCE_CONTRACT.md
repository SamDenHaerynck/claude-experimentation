# Configured Commerce extension contract

What a real Optimizely Configured Commerce extension has to implement to use this library. Nothing
here has run inside a Configured Commerce install: there is no licence or instance to test against.
The interfaces, the dispatcher and the in-memory fakes are real and tested; the Configured Commerce
side is a description, with every unverified point marked as such.

Code: `src/BePeppolCommerce.Core/Integration/`. Tests: `tests/BePeppolCommerce.Core.Tests/OutboundDispatcherTests.cs`.

## Target framework (checked day 032)

Optimizely's migration guide (https://docs.optimizely.com/configured-commerce/docs/net7-framework-to-net-migration,
fetched 2026-10-04, page marked updated 02 Oct 2026) says `Extensions.csproj` ships targeting only
`net48`, and that the target can be switched to `net8.0` (or `net48;net8.0`) on releases
5.2.2512 to 5.2.2604, and to `net10.0` (or `net48;net10.0`) from 5.2.2605.

The same page's migration steps start with updating the repository to the 5.2.2604.725-lts build or
newer. Consequence: this library targets `net8.0` only, so it can be referenced from an Extensions
project that has been retargeted to `net8.0` or `net10.0`. That is documented from build
5.2.2604.725-lts on; whether a project retargeted on an earlier 5.2.2512 to 5.2.2604 build works is
unverified. **An install still on `net48` cannot
use it.** Supporting `net48` would mean multi-targeting `BePeppolCommerce.Core`, which uses .NET 8
APIs throughout (`SHA256.HashData`, `Guid(bytes, bigEndian)`, `ArgumentException.ThrowIfNullOrEmpty`),
plus IKVM for `net472`. That is out of scope for v1 (decided day 032, see `PLAN.md` Slice 6 and `DECISIONS.md`), not done.
Market risk that follows: an install that has not migrated cannot use v1, and how many Belgian
Configured Commerce installs have migrated is not known (no evidence found).

## How the pieces fit

```
checkout (Configured Commerce)        background job (Configured Commerce)      this library
------------------------------        -----------------------------------      ------------
order submitted                       every N minutes:
  -> extension maps it to Order          OutboundDispatcher.RunOnceAsync  -->   OutboundInvoiceSender
  -> stores PendingInvoice                 reads IAccessPointSettingsProvider     build, validate, send
     (fixed IdempotencyKey)                reads IOrderInvoiceSource.GetPending   via IPeppolAccessPointClient
                                           writes MarkSent / MarkFailed
```

Sending is kept out of the checkout request on purpose. Validation takes about 25 ms warm but
compiles the Schematron for several seconds on first use, and a provider outage must not fail a
checkout. Configured Commerce handlers are synchronous (`Execute(IUnitOfWork, parameter, result)`,
per https://docs.optimizely.com/configured-commerce/docs/working-with-handlers, fetched 2026-10-04),
so calling this async library from inside one would also mean blocking on a task.

**Unverified:** which handler chain fires on order submission, which Configured Commerce API maps
an order to `Order` (customer, lines, VAT), what storage the extension should use for the queue
(a custom table via the platform's Entity Framework model is the likely route), and how a recurring
background job is scheduled in Configured Commerce. Each needs a real install or a partner's
answer.

## `IOrderInvoiceSource`

```csharp
public sealed record PendingInvoice(string SourceId, Order Order, Guid IdempotencyKey);
public sealed record DispatchFailure(string Reason, bool Retryable, IReadOnlyList<string> Details);

public interface IOrderInvoiceSource
{
    Task<IReadOnlyList<PendingInvoice>> GetPendingAsync(int maxCount, CancellationToken cancellationToken = default);
    Task MarkSentAsync(string sourceId, string submissionId, CancellationToken cancellationToken = default);
    Task MarkFailedAsync(string sourceId, DispatchFailure failure, CancellationToken cancellationToken = default);
}
```

| Element | Contract |
|---|---|
| Signatures | As above. `GetPendingAsync` returns at most `maxCount` items, never null (empty when idle). `maxCount` must be positive. Order: never-attempted invoices oldest first, then retryable failures least recently attempted first, so one failing invoice cannot hold up newer ones. |
| Idempotency | `IdempotencyKey` is chosen once, when the order is queued, and stored with it. Every retry passes the same key. Do not derive it again at send time: a library upgrade between attempts would change a derived key and could deliver the invoice twice (see `OutboundInvoiceSender.SendAsync`). |
| Lifetime and threading | The dispatcher calls the source sequentially, never in parallel, within one run. Register the implementation transient, or per request if the job runs inside a request scope (Configured Commerce's default lifetime is per request, per https://docs.optimizely.com/configured-commerce/docs/dependency-injection, fetched 2026-10-04: classes implementing `IDependency` and `IExtension` are registered automatically, and `ISingletonLifetime` or `ITransientLifetime` change the lifetime; whether a Configured Commerce background job has a request scope is unverified). It does not need to be thread-safe, but **only one dispatcher run may be active at a time**, across all web nodes: two concurrent runs can both read the same pending invoice. Use a job scheduler that guarantees a single runner, or a database lock. |
| Error contract | Throws `ArgumentException`/`ArgumentOutOfRangeException` for bad arguments, `InvalidOperationException` for an unknown `sourceId`, `OperationCanceledException` on cancellation, and lets storage failures escape; the dispatcher does not catch any of these, so the run stops. An invoice that cannot be sent is never an exception: it arrives through `MarkFailedAsync`. |
| State after each call | `MarkSentAsync`: removed from the queue, submission id stored, any earlier failure cleared. If it throws after the Access Point accepted the invoice, the invoice stays queued and is sent again on the next run; only the idempotency key prevents a duplicate delivery, and Storecove's handling of a reused key is unverified. `MarkFailedAsync` with `Retryable = true`: stays queued behind never-attempted invoices, failure stored for display. With `Retryable = false`: removed from the queue until someone fixes the order and queues it again. |
| Credentials | None. The source never sees Access Point credentials. |

Fake: `InMemoryOrderInvoiceSource` (thread-safe, not durable, exposes `Sent` and `Failed` for tests).

## `IAccessPointSettingsProvider`

```csharp
public sealed record AccessPointSettings(string Provider, string ApiKey, string AccountId, Uri? BaseUri = null);

public interface IAccessPointSettingsProvider
{
    Task<AccessPointSettings?> GetAsync(CancellationToken cancellationToken = default);
}
```

| Element | Contract |
|---|---|
| Signatures | As above. `Provider` is `"storecove"` today (case-insensitive); Slice 7 adds a second. `AccountId` is the provider's id for the sending company (Storecove: the legal entity id, a positive integer). `BaseUri` null means the provider's production API; anything else must be https, or http to loopback for tests. |
| Lifetime and threading | Called once at the start of each dispatcher run, so changed settings apply on the next run without a restart. Any lifetime works; a singleton must be thread-safe. |
| Error contract | Returns null when Peppol sending is off or not configured; the run then sends nothing and reports `Configured = false`. Throws `OperationCanceledException` on cancellation, and otherwise only if the settings store itself fails. Settings that are present but unusable (unknown provider, bad `AccountId`, blank key, non-https `BaseUri`) make `AccessPointClientFactory.Create` throw `ArgumentException` before anything is read from the queue. |
| Credentials | The API key comes from the host at runtime, never from this repo or a committed file. In Configured Commerce the expected home is the platform's settings storage or a secret store such as Azure Key Vault; **which Configured Commerce settings API to use is unverified.** `AccessPointSettings.ToString()` leaves the key out so the record can be logged. |

Fake: `InMemoryAccessPointSettingsProvider` (returns whatever `Settings` holds).

## `OutboundDispatcher` (provided by this library)

```csharp
public OutboundDispatcher(IOrderInvoiceSource source, IAccessPointSettingsProvider settings,
    Func<AccessPointSettings, IPeppolAccessPointClient> clientFactory);
public Task<DispatchSummary> RunOnceAsync(int batchSize = 20, CancellationToken cancellationToken = default);
public static DispatchFailure ToFailure(OutboundResult result);
// DispatchSummary(bool Configured, int Sent, int FailedRetryable, int FailedPermanent)
```

- Normal wiring: `clientFactory = s => AccessPointClientFactory.Create(s, httpClient)`, with an
  `HttpClient` from `IHttpClientFactory` or a long-lived instance; the caller owns it.
- Failure classification: validation failure → permanent. Send failure with no HTTP status
  (transport error), a client-side timeout, 408, 429 or 5xx → retryable (`"Access Point unavailable"`).
  401, 403 or 404 → retryable (`"Access Point configuration error"`): they point at the API key or
  account id, not the order, so the invoice waits until the settings are fixed. Whether a provider
  also answers 404 for an unknown receiver (which would be an order problem) is unverified. A failed
  send with a 2xx status → permanent (`"Accepted by Access Point, response unreadable"`): the
  provider took the invoice, so a person must check it there; re-queuing it with a new key could
  deliver it twice. Any other 4xx →
  permanent (`"Rejected by Access Point"`), including 409; whether a provider answers a reused idempotency key with 409 (which would mean the
  invoice was in fact delivered) is unverified.
- A permanent failure caused by the invoice is recorded and the run continues with the next invoice.
  The first retryable failure is recorded and **ends the run**, so a provider outage or a bad key
  gets one request per run, not the whole batch.
- An unexpected exception while sending one invoice is recorded as retryable
  (`Reason = "Unexpected error"`) and ends the run. The invoice stays queued behind newer ones, so a
  lasting environment fault (for example a disposed `HttpClient`) does not park the queue one invoice
  per run. A host should alert on repeated `"Unexpected error"` or `"Access Point configuration error"`
  failures, because the summary only counts them as retryable.
- Cancellation of the caller's token is checked before each invoice and stops the run. If it
  happens during a send, the run throws; an outcome the provider already returned is still recorded
  (the source is called with `CancellationToken.None` after a send). An `OperationCanceledException`
  with the caller's token not cancelled (a client timeout) is a retryable failure.
- There is no retry limit or back-off beyond that: a retryable invoice is tried again on every run,
  behind newer invoices. The host decides when to give up, for example by marking it non-retryable
  after N attempts. **Gap**, noted for review.

## Inbound

This slice defines no Configured Commerce interface for inbound. `PLAN.md`'s core flow (step 5) ends
with received invoices parsed and exposed for a caller to consume: the webhook host
(`BePeppolCommerce.Api`) fetches, parses and returns them. What a Configured Commerce extension
would do with a received invoice (store it, notify someone) is not designed yet.
