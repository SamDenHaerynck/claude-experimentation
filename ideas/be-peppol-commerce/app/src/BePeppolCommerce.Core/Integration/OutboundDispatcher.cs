using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Outbound;

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

    public OutboundDispatcher(IOrderInvoiceSource source, IAccessPointSettingsProvider settings,
        Func<AccessPointSettings, IPeppolAccessPointClient> clientFactory)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    }

    /// <summary>
    /// Sends up to <paramref name="batchSize"/> pending invoices, one at a time. When the settings
    /// provider returns null, nothing is read or sent and the summary says not configured. A failure on
    /// one invoice is recorded on that invoice and the run continues; exceptions from the source, the
    /// settings provider or the client factory escape, as does cancellation.
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
            OutboundResult result;
            try
            {
                result = await sender.SendAsync(item.Order, item.IdempotencyKey, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // A bug or bad data the builder did not anticipate. Retrying would fail the same way and
                // block the queue, so the invoice is parked for a person to look at.
                await _source.MarkFailedAsync(item.SourceId,
                    new DispatchFailure("Unexpected error", false, [$"{ex.GetType().Name}: {ex.Message}"]), cancellationToken);
                permanent++;
                continue;
            }

            if (result.Status == OutboundStatus.Sent)
            {
                await _source.MarkSentAsync(item.SourceId, result.SubmissionId!, cancellationToken);
                sent++;
                continue;
            }

            var failure = ToFailure(result);
            await _source.MarkFailedAsync(item.SourceId, failure, cancellationToken);
            if (failure.Retryable) retryable++; else permanent++;
        }
        return new DispatchSummary(true, sent, retryable, permanent);
    }

    /// <summary>
    /// Validation failures and provider rejections (4xx other than 408 and 429) need a change to the
    /// order, so they are permanent. No HTTP status (transport error), 408, 429 and 5xx are retryable.
    /// </summary>
    public static DispatchFailure ToFailure(OutboundResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Status == OutboundStatus.ValidationFailed)
            return new DispatchFailure("Validation failed", false,
                result.Validation.Errors.Select(f => $"{f.RuleId}: {f.Message}").ToArray());

        var send = result.Send!;
        var retryable = send.HttpStatus is null or 408 or 429 or >= 500;
        return new DispatchFailure(retryable ? "Access Point unavailable" : "Rejected by Access Point", retryable,
            send.Errors.Select(e => $"{e.Source}: {e.Details}").ToArray());
    }
}
