using Microsoft.Extensions.Logging;

namespace BePeppolCommerce.Core.Integration;

public enum DocumentDirection { Outbound, Inbound }

/// <summary>
/// One document that could not be sent or received. <see cref="DocumentId"/> is the host's source id for
/// an outbound invoice, or the Access Point's document id for an inbound one. <see cref="Details"/> holds
/// rule ids, provider error messages or an exception type and message: never the UBL XML and never a
/// credential.
/// </summary>
public sealed record FailedDocument(
    DateTimeOffset At, DocumentDirection Direction, string DocumentId, string Reason, bool Retryable, IReadOnlyList<string> Details);

/// <summary>
/// Where failed documents are written so a failure is never only a log line. A host can implement this
/// over its own storage; <see cref="InMemoryFailedDocumentLog"/> is the v1 implementation.
/// </summary>
/// <remarks>
/// Callers in this library catch and log (event <see cref="PeppolLogEvents.FailureRecordFailed"/>) any
/// exception other than cancellation, so a broken log never stops a dispatch run or a webhook.
/// </remarks>
public interface IFailedDocumentLog
{
    Task RecordAsync(FailedDocument failure, CancellationToken cancellationToken = default);
}

/// <summary>
/// Thread-safe, bounded, not durable: keeps the newest <see cref="Capacity"/> entries and loses them all
/// when the process ends. Good enough for v1 and tests, together with the structured log events; a
/// production host should implement <see cref="IFailedDocumentLog"/> over durable storage.
/// </summary>
public sealed class InMemoryFailedDocumentLog(int capacity = 1000) : IFailedDocumentLog
{
    private readonly object _gate = new();
    private readonly Queue<FailedDocument> _entries = new();

    public int Capacity { get; } = capacity > 0 ? capacity : throw new ArgumentOutOfRangeException(nameof(capacity));

    /// <summary>Snapshot, oldest first.</summary>
    public IReadOnlyList<FailedDocument> Entries { get { lock (_gate) return _entries.ToArray(); } }

    public Task RecordAsync(FailedDocument failure, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(failure);
        lock (_gate)
        {
            if (_entries.Count == Capacity) _entries.Dequeue();
            _entries.Enqueue(failure);
        }
        return Task.CompletedTask;
    }
}

/// <summary>Stable event ids for log filtering and alerting. Never renumber; add new ids only.</summary>
public static class PeppolLogEvents
{
    public static readonly EventId OutboundSent = new(1000, nameof(OutboundSent));
    public static readonly EventId OutboundValidationFailed = new(1001, nameof(OutboundValidationFailed));
    public static readonly EventId OutboundSendFailedRetryable = new(1002, nameof(OutboundSendFailedRetryable));
    public static readonly EventId OutboundSendFailedPermanent = new(1003, nameof(OutboundSendFailedPermanent));
    public static readonly EventId OutboundUnexpectedError = new(1004, nameof(OutboundUnexpectedError));

    public static readonly EventId InboundFetchFailed = new(2001, nameof(InboundFetchFailed));
    public static readonly EventId InboundParseFailed = new(2002, nameof(InboundParseFailed));
    public static readonly EventId InboundPayloadRejected = new(2003, nameof(InboundPayloadRejected));
    public static readonly EventId InboundUnauthorized = new(2004, nameof(InboundUnauthorized));

    public static readonly EventId FailureRecordFailed = new(9001, nameof(FailureRecordFailed));

    /// <summary>
    /// Writes <paramref name="failure"/> to <paramref name="log"/> (if any). An exception from the log is
    /// logged as <see cref="FailureRecordFailed"/> and swallowed; cancellation still escapes.
    /// </summary>
    public static async Task TryRecordAsync(IFailedDocumentLog? log, FailedDocument failure, ILogger logger, CancellationToken cancellationToken)
    {
        if (log is null) return;
        try
        {
            await log.RecordAsync(failure, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(FailureRecordFailed, ex, "Could not record the failure of {Direction} document {DocumentId} ({Reason}).",
                failure.Direction, failure.DocumentId, failure.Reason);
        }
    }
}
