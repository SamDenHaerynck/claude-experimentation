using BePeppolCommerce.Core.AccessPoint;

namespace BePeppolCommerce.Api;

/// <summary>Reads the "Storecove" configuration section, failing at startup rather than on the first webhook.</summary>
public static class StorecoveConfig
{
    public static StorecoveOptions Read(IConfigurationSection section)
    {
        Uri? baseUri = null;
        var rawBaseUri = section["BaseUri"];
        if (!string.IsNullOrWhiteSpace(rawBaseUri))
        {
            // A typo must not silently fall back to the production API with the configured key.
            if (!Uri.TryCreate(rawBaseUri, UriKind.Absolute, out baseUri))
                throw new InvalidOperationException($"Storecove:BaseUri '{rawBaseUri}' is not an absolute URI.");
            if (baseUri.Scheme != Uri.UriSchemeHttps && !baseUri.IsLoopback)
                throw new InvalidOperationException("Storecove:BaseUri must be https (plain http only for loopback test servers).");
        }
        return new StorecoveOptions(section["ApiKey"]!, section.GetValue<int>("LegalEntityId"), baseUri);
    }
}
