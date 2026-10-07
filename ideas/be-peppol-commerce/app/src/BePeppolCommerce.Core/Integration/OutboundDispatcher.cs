using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Outbound;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BePeppolCommerce.Core.Integration;

/// <summary>Counts from one <see cref="OutboundDispatcher.RunOnceAsync"/> call.</summary>
public sealed record DispatchSummary(bool Configured, int Sent, int FailedRetryable, int FailedPermanent);

/// <summary>Creates Access Point clients from runtime settings.</summary>
public static class AccessPointClientFactory
{
    /// <summary>
    /// Returns the client named by <see cref="AccessPointSettings.Provider"/>. Throws
    /// <see cref="ArgumentException"/> for an unknown provider or unusable settings (a configuration
    /// error, reported before anything is sent). The caller owns <paramref name="http"/>.
    /// </summary>
    public static IPeppolAccessPointClient Create(AccessPointSettings settings, HttpClient http)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(http);
        return settings.Provider.ToLowerInvariant() switch
        {
            "storecove" => new StorecoveClient(http, new StorecoveOptions(settings.ApiKey,
                int.TryParse(settings.AccountId, out var id) && id > 0
                    ? id
                    : throw new ArgumentException("Storecove AccountId must be a positive integer (the legal entity id).", nameof(settings)),
                settings.BaseUri)),
            "recommand" => new RecommandClient(http, new RecommandOptions(settings.ApiKey, settings.ApiSecret ?? "", settings.AccountId, settings.BaseUri)),
            _ => throw new ArgumentException($"Unknown Access Point provider '{settings.Provider}'.", nameof(settings)),
        };
    }
}

/// <summary>
/// Drains an <see cref="IOrderInvoiceSource"/>: each pending invoice goes through
/// <see cref="OutboundInvoiceSender"/>, and the outcome is written back to the source. Meant to be called
/// from a background job, never from the checkout request itself.
/// </summary>
/// <remarks>
/// Not safe to run concurrently against the same source: two runs could send the same invoice twice
/// (the shared idempotency key should make the Access Point drop the second, which is unverified for
/// Storecove). Schedule one run at a time.
/// </remarks>
public sealed class OutboundDispatcher
{
    private readonly IOrderInvoiceSource _source;
    private readonly IAccessPointSettingsProvider _settings;
    private readonly Func<AccessPointSettings, IPeppolAccessPointClient> _clientFactory;
    private readonly IFailedDocumentLog? _failures;
    private readonly ILogger _logger;
    private readonly TimeProvider _time;

    /// <summary>
    /// Every failure is marked on the source, written to <paramref name="failures"/> when given, and logged
    /// to <paramref name="logger"/> with a <see cref="PeppolLogEvents"/> id. Neither carries the UBL XML or
    /// a credential.
    /// </summary>
    public OutboundDispatcher(IOrderInvoiceSource source, IAccessPointSettingsProvider settings,
        Func<AccessPointSettings, IPeppolAccessPointClient> clientFactory,
        IFailedDocumentLog? failures = null, ILogger<OutboundDispatcher>? logger = null, TimeProvider? time = null)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _failures = failures;
        _logger = (ILogger?)logger ?? NullLogger.Instance;
        _time = time ?? TimeProvider.System;
    }

    /// <summary>
    /// Sends up to <paramref name="batchSize"/> pending invoices, one at a time. When the settings
    /// provider returns null, nothing is read or sent and the summary says not configured. A permanent
    /// failure caused by the invoice itself is recorded and the run continues. The first retryable
    /// failure (provider unreachable, client timeout, 401, 403, 404, 408, 429, 5xx) is recorded and ends
    /// the run, so an outage or a bad API key is not hit with the whole batch. An unexpected exception
    /// is recorded the same way (retryable) and also ends the run. Once a send has been attempted, its result is recorded
    /// with <see cref="CancellationToken.None"/>. Exceptions from the source, the settings provider or the client factory
    /// escape, as does cancellation of <paramref name="cancellationToken"/>.
    /// </summary>
    public async Task<DispatchSummary> RunOnceAsync(int batchSize = 20, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(batchSize);
        var settings = await _settings.GetAsync(cancellationToken);
        if (settings is null)
            return new DispatchSummary(false, 0, 0, 0);

        var sender = new OutboundInvoiceSender(_clientFactory(settings));
        int sent = 0, retryable = 0, permanent = 0;
        foreach (var item in await _source.GetPendingAsync(batchSize, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            OutboundResult result;
            try
            {
                result = await sender.SendAsync(item.Order, item.IdempotencyKey, cancellationToken);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                // Not the caller's cancellation: a client-side timeout. Same as an unreachable provider.
                await FailAsync(item, new DispatchFailure("Access Point unavailable", true, ["Timed out."]),
                    PeppolLogEvents.OutboundSendFailedRetryable);
                retryable++;
                break;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // A bug, bad data the builder did not anticipate, or a broken environment (for example a
                // disposed HttpClient). Kept queued as retryable, behind newer invoices, and the run stops, so
                // a lasting environment fault does not park the queue one invoice per run.
                await FailAsync(item, new DispatchFailure("Unexpected error", true, [$"{ex.GetType().Name}: {ex.Message}"]),
                    PeppolLogEvents.OutboundUnexpectedError, ex);
                retryable++;
                break;
            }

            if (result.Status == OutboundStatus.Sent)
            {
                // The provider has accepted the invoice, so this is recorded even if the caller cancels. If
                // the call fails anyway, the invoice stays queued and is sent again next run; only the
                // idempotency key prevents a duplicate (unverified for Storecove).
                await _source.MarkSentAsync(item.SourceId, result.SubmissionId!, CancellationToken.None);
                _logger.LogInformation(PeppolLogEvents.OutboundSent, "Outbound invoice {SourceId} sent as {SubmissionId}.",
                    item.SourceId, result.SubmissionId);
                sent++;
                continue;
            }

            var failure = ToFailure(result, settings.Provider);
            await FailAsync(item, failure,
                result.Status == OutboundStatus.ValidationFailed ? PeppolLogEvents.OutboundValidationFailed
                : failure.Retryable ? PeppolLogEvents.OutboundSendFailedRetryable
                : PeppolLogEvents.OutboundSendFailedPermanent);
            if (!failure.Retryable)
            {
                permanent++;
                continue;
            }
            // The provider is down, throttling or refusing our credentials: stop, rather than send the rest
            // of the batch into the same failure.
            retryable++;
            break;
        }
        return new DispatchSummary(true, sent, retryable, permanent);
    }

    // Marks the source first (the queue state matters most), then the failure log, then the log event.
    // Runs with CancellationToken.None: the attempt has happened and must be recorded.
    private async Task FailAsync(PendingInvoice item, DispatchFailure failure, EventId eventId, Exception? exception = null)
    {
        await _source.MarkFailedAsync(item.SourceId, failure, CancellationToken.None);
        await PeppolLogEvents.TryRecordAsync(_failures,
            new FailedDocument(_time.GetUtcNow(), DocumentDirection.Outbound, item.SourceId, failure.Reason, failure.Retryable, failure.Details),
            _logger, CancellationToken.None);
        _logger.Log(failure.Retryable && eventId != PeppolLogEvents.OutboundUnexpectedError ? LogLevel.Warning : LogLevel.Error,
            eventId, exception, "Outbound invoice {SourceId} failed: {Reason} (retryable: {Retryable}). {Details}",
            item.SourceId, failure.Reason, failure.Retryable, string.Join("; ", failure.Details));
    }

    /// <summary>
    /// Validation failures and provider rejections of the document (4xx other than 401, 403, 404, 408
    /// and 429) need a change to the order, so they are permanent. No HTTP status (transport error),
    /// 408, 429 and 5xx are retryable. 401, 403 and 404 point at the API key or account id, not the
    /// order, so they are retryable too, with their own reason, and the invoice waits until the
    /// settings are fixed (whether a provider also answers 404 for an unknown receiver is unverified).
    /// A 2xx status on a failed send means the provider accepted the invoice but its response could not
    /// be read: permanent, with its own reason, because re-queuing it with a new key could deliver it twice.
    /// A 409 is treated as a rejection; whether a provider answers a reused idempotency key with 409
    /// (meaning the invoice was in fact delivered) is unverified.
    /// With <paramref name="provider"/> "recommand", a 422 means the recipient could not be reached over
    /// Peppol (Recommand's OpenAPI spec): still permanent, because retrying the same invoice does not change
    /// the receiver's registration, but with its own reason so nobody edits a correct order to fix it.
    /// </summary>
    public static DispatchFailure ToFailure(OutboundResult result, string? provider = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Status == OutboundStatus.ValidationFailed)
            return new DispatchFailure("Validation failed", false,
                result.Validation.Errors.Select(f => $"{f.RuleId}: {f.Message}").ToArray());

        var send = result.Send!;
        var details = send.Errors.Select(e => $"{e.Source}: {e.Details}").ToArray();
        return send.HttpStatus switch
        {
            >= 200 and < 300 => new DispatchFailure("Accepted by Access Point, response unreadable", false, details),
            422 when string.Equals(provider, "recommand", StringComparison.OrdinalIgnoreCase)
                => new DispatchFailure("Recipient not reachable on Peppol", false, details),
            401 or 403 or 404 => new DispatchFailure("Access Point configuration error", true, details),
            null or 408 or 429 or >= 500 => new DispatchFailure("Access Point unavailable", true, details),
            _ => new DispatchFailure("Rejected by Access Point", false, details),
        };
    }
}
