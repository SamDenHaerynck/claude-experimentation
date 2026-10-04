using System.Collections.Concurrent;
using BePeppolCommerce.Core.Model;

namespace BePeppolCommerce.Core.Integration;

/// <summary>
/// Test and demo implementation of <see cref="IOrderInvoiceSource"/>. Thread-safe. Not durable: the
/// queue is lost when the process ends, so it must never stand in for a real host's storage.
/// The queue holds never-attempted invoices first, then retryable failures in the order they failed,
/// which is the ordering rule in <see cref="IOrderInvoiceSource.GetPendingAsync"/>.
/// </summary>
public sealed class InMemoryOrderInvoiceSource : IOrderInvoiceSource
{
    private readonly object _gate = new();
    private readonly List<PendingInvoice> _queue = new();
    private readonly HashSet<string> _retrying = new();
    private readonly ConcurrentDictionary<string, string> _sent = new();
    private readonly ConcurrentDictionary<string, DispatchFailure> _failed = new();

    /// <summary>Submission id per sent invoice.</summary>
    public IReadOnlyDictionary<string, string> Sent { get { lock (_gate) return new Dictionary<string, string>(_sent); } }

    /// <summary>Latest failure per invoice; cleared when the invoice is sent or queued again.</summary>
    public IReadOnlyDictionary<string, DispatchFailure> Failed { get { lock (_gate) return new Dictionary<string, DispatchFailure>(_failed); } }

    /// <summary>Queues an order. The idempotency key is fixed here, once, as a real host must do.</summary>
    public PendingInvoice Enqueue(string sourceId, Order order, Guid? idempotencyKey = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(sourceId);
        ArgumentNullException.ThrowIfNull(order);
        var item = new PendingInvoice(sourceId, order, idempotencyKey ?? Guid.NewGuid());
        lock (_gate)
        {
            if (_queue.Exists(p => p.SourceId == sourceId))
                throw new InvalidOperationException($"'{sourceId}' is already queued.");
            if (_sent.ContainsKey(sourceId))
                throw new InvalidOperationException($"'{sourceId}' was already sent.");
            // Ahead of the retrying invoices, which sit at the back.
            _queue.Insert(_queue.Count - _retrying.Count, item);
            _failed.TryRemove(sourceId, out _);
        }
        return item;
    }

    public Task<IReadOnlyList<PendingInvoice>> GetPendingAsync(int maxCount, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxCount);
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
            return Task.FromResult<IReadOnlyList<PendingInvoice>>(_queue.Take(maxCount).ToArray());
    }

    public Task MarkSentAsync(string sourceId, string submissionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(submissionId);
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            _queue.RemoveAt(IndexOf(sourceId));
            _retrying.Remove(sourceId);
            _sent[sourceId] = submissionId;
            _failed.TryRemove(sourceId, out _);
        }
        return Task.CompletedTask;
    }

    public Task MarkFailedAsync(string sourceId, DispatchFailure failure, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(failure);
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            var index = IndexOf(sourceId);
            var item = _queue[index];
            _queue.RemoveAt(index);
            _retrying.Remove(sourceId);
            if (failure.Retryable)
            {
                _queue.Add(item);
                _retrying.Add(sourceId);
            }
            _failed[sourceId] = failure;
        }
        return Task.CompletedTask;
    }

    private int IndexOf(string sourceId)
    {
        ArgumentException.ThrowIfNullOrEmpty(sourceId);
        var index = _queue.FindIndex(p => p.SourceId == sourceId);
        return index >= 0 ? index : throw new InvalidOperationException($"'{sourceId}' is not queued.");
    }
}

/// <summary>Test and demo implementation of <see cref="IAccessPointSettingsProvider"/>: returns whatever was set.</summary>
public sealed class InMemoryAccessPointSettingsProvider(AccessPointSettings? settings = null) : IAccessPointSettingsProvider
{
    public AccessPointSettings? Settings { get; set; } = settings;

    public Task<AccessPointSettings?> GetAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Settings);
    }
}
