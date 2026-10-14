using System.Collections.Concurrent;
using BePeppolCommerce.Core.AccessPoint;

namespace BePeppolCommerce.Walkthrough;

/// <summary>
/// An in-process stand-in for a Peppol Access Point provider. It never touches the network. Every
/// accepted document is kept under its submission id, and <see cref="GetInboundAsync"/> hands it back,
/// as if the buyer's Access Point had received it and announced it by webhook.
/// </summary>
public sealed class FakeAccessPoint : IPeppolAccessPointClient
{
    private readonly ConcurrentDictionary<string, string> _documents = new();
    private int _next;

    public IReadOnlyDictionary<string, string> Documents => _documents;

    public Task<AccessPointResult<string>> SendAsync(OutboundDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();
        var id = $"fake-{Interlocked.Increment(ref _next):D4}";
        _documents[id] = document.UblXml;
        return Task.FromResult(AccessPointResult<string>.Ok(id, 200));
    }

    public Task<AccessPointResult<InboundDocument>> GetInboundAsync(string providerDocumentId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(providerDocumentId);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_documents.TryGetValue(providerDocumentId, out var xml)
            ? AccessPointResult<InboundDocument>.Ok(new InboundDocument(providerDocumentId, xml), 200)
            : AccessPointResult<InboundDocument>.Fail(404, new AccessPointError("fake", $"No document '{providerDocumentId}'.")));
    }
}
