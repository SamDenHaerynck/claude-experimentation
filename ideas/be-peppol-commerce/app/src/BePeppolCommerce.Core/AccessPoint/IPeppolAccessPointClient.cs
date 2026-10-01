namespace BePeppolCommerce.Core.AccessPoint;

/// <summary>A Peppol participant id, for example scheme "0208" and id "0123456789" (Belgian KBO/BCE).</summary>
public sealed record PeppolParticipant(string Scheme, string Id)
{
    public override string ToString() => $"{Scheme}:{Id}";
}

/// <summary>An outbound UBL document, already validated by the caller.</summary>
public sealed record OutboundDocument(string UblXml, PeppolParticipant Recipient, Guid? IdempotencyKey = null);

/// <summary>An inbound document as fetched from the Access Point.</summary>
public sealed record InboundDocument(string ProviderDocumentId, string UblXml);

/// <summary>
/// Outcome of a provider call. Transport and protocol failures that the caller can act on (rejected
/// document, bad credentials, provider down) are returned as a failure, never thrown. Only
/// programming errors (null arguments) and cancellation throw.
/// </summary>
public sealed record AccessPointResult<T>(bool Success, T? Value, int? HttpStatus, IReadOnlyList<AccessPointError> Errors)
{
    public static AccessPointResult<T> Ok(T value, int httpStatus) => new(true, value, httpStatus, Array.Empty<AccessPointError>());

    public static AccessPointResult<T> Fail(int? httpStatus, params AccessPointError[] errors) => new(false, default, httpStatus, errors);
}

/// <summary>One error reported by the provider, or by the client when the response was unusable.</summary>
public sealed record AccessPointError(string Source, string Details);

/// <summary>
/// A client of an existing accredited Peppol Access Point provider. This library never acts as an
/// Access Point itself.
/// </summary>
public interface IPeppolAccessPointClient
{
    /// <summary>Submits a document for delivery. On success, the value is the provider's submission id.</summary>
    Task<AccessPointResult<string>> SendAsync(OutboundDocument document, CancellationToken cancellationToken = default);

    /// <summary>Fetches one received document by the provider's id (as announced by its webhook).</summary>
    Task<AccessPointResult<InboundDocument>> GetInboundAsync(string providerDocumentId, CancellationToken cancellationToken = default);
}
