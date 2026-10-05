namespace BePeppolCommerce.Core.Integration;

/// <summary>
/// Access Point connection settings for one seller. <see cref="Provider"/> names the client ("storecove"
/// or "recommand"). <see cref="AccountId"/> is the provider's id for the sending company (Storecove: the
/// legal entity id; Recommand: the company id). <see cref="BaseUri"/> is null for the provider's
/// production API. <see cref="ApiSecret"/> is required by Recommand (Basic auth key and secret) and
/// ignored by Storecove.
/// </summary>
public sealed record AccessPointSettings(string Provider, string ApiKey, string AccountId, Uri? BaseUri = null, string? ApiSecret = null)
{
    /// <summary>Hides the API key and secret, so settings can be logged.</summary>
    public override string ToString() => $"AccessPointSettings {{ Provider = {Provider}, AccountId = {AccountId}, BaseUri = {BaseUri} }}";
}

/// <summary>
/// Supplies <see cref="AccessPointSettings"/> at runtime. In a Configured Commerce install the extension
/// implements it over the platform's own settings and secret storage (see
/// docs/CONFIGURED_COMMERCE_CONTRACT.md); <see cref="InMemoryAccessPointSettingsProvider"/> is the test fake.
/// </summary>
/// <remarks>
/// Returns null when Peppol sending is not configured or switched off; that is a normal state, not an
/// error. Throws only when the settings store itself fails. Called once per dispatch run, so changed
/// settings take effect on the next run without a restart.
/// </remarks>
public interface IAccessPointSettingsProvider
{
    Task<AccessPointSettings?> GetAsync(CancellationToken cancellationToken = default);
}
