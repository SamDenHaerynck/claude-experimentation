using BePeppolCommerce.Core.Model;

namespace BePeppolCommerce.Core.Integration;

/// <summary>
/// One invoice waiting to be sent. <see cref="SourceId"/> is the host platform's own id for it (for
/// example a Configured Commerce order number), opaque to this library. <see cref="IdempotencyKey"/> is
/// fixed by the source when the invoice is first queued and must be the same on every retry, so the
/// Access Point can recognise a resend of the same invoice.
/// </summary>
public sealed record PendingInvoice(string SourceId, Order Order, Guid IdempotencyKey);

/// <summary>
/// Where a failed invoice stands after one dispatch attempt. <see cref="Retryable"/> is true only when
/// sending the same invoice again later could succeed (provider down, timeout, rate limit); a
/// validation failure or a rejection by the provider is never retryable without a change to the order.
/// </summary>
public sealed record DispatchFailure(string Reason, bool Retryable, IReadOnlyList<string> Details);

/// <summary>
/// The host platform's side of the outbound flow: a queue of invoices to send, and a place to record
/// what happened to each. In a Configured Commerce install this is implemented by the extension (see
/// docs/CONFIGURED_COMMERCE_CONTRACT.md); <see cref="InMemoryOrderInvoiceSource"/> is the test fake.
/// </summary>
/// <remarks>
/// Error contract: implementations throw only for programming errors (null or empty arguments, an
/// unknown <c>sourceId</c>) and for storage failures they cannot recover from; the dispatcher lets those
/// escape. Cancellation throws <see cref="OperationCanceledException"/>. An invoice that cannot be sent
/// is never an exception here: it is reported through <see cref="MarkFailedAsync"/>.
/// </remarks>
public interface IOrderInvoiceSource
{
    /// <summary>
    /// Returns up to <paramref name="maxCount"/> invoices that are queued and not yet sent or
    /// permanently failed: never-attempted invoices oldest first, then retryable failures least
    /// recently attempted first, so a failing invoice cannot hold up newer ones. Returns an empty list when there is nothing to do, never null.
    /// </summary>
    Task<IReadOnlyList<PendingInvoice>> GetPendingAsync(int maxCount, CancellationToken cancellationToken = default);

    /// <summary>Records that the Access Point accepted the invoice under <paramref name="submissionId"/>. The invoice leaves the queue.</summary>
    Task MarkSentAsync(string sourceId, string submissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a failed attempt. A retryable failure keeps the invoice queued for a later
    /// <see cref="GetPendingAsync"/>; a non-retryable one takes it out of the queue until someone fixes
    /// the order and queues it again.
    /// </summary>
    Task MarkFailedAsync(string sourceId, DispatchFailure failure, CancellationToken cancellationToken = default);
}
