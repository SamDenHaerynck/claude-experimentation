using BePeppolCommerce.Core.AccessPoint;

namespace BePeppolCommerce.Api;

/// <summary>
/// Picks the Access Point provider from configuration and registers its client, failing at startup
/// rather than on the first webhook. <c>AccessPoint:Provider</c> is "storecove" (the default, so a
/// configuration with only a <c>Storecove</c> section keeps working) or "recommand". The client is
/// registered only when that provider's section has an API key, so the host still starts locally
/// without one; the webhook then answers 503.
/// </summary>
public static class AccessPointConfig
{
    /// <summary>Registers the configured client and returns the provider name, or null when no key is set.</summary>
    public static string? Register(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["AccessPoint:Provider"];
        provider = string.IsNullOrWhiteSpace(provider) ? "storecove" : provider.Trim().ToLowerInvariant();
        switch (provider)
        {
            case "storecove":
                var storecove = configuration.GetSection("Storecove");
                if (string.IsNullOrWhiteSpace(storecove["ApiKey"])) return null;
                services.AddSingleton(ReadStorecove(storecove));
                services.AddHttpClient<IPeppolAccessPointClient, StorecoveClient>();
                return provider;
            case "recommand":
                var recommand = configuration.GetSection("Recommand");
                if (string.IsNullOrWhiteSpace(recommand["ApiKey"])) return null;
                var options = ReadRecommand(recommand);
                // Construct once now so a missing secret or bad company id fails at startup.
                using (var probe = new HttpClient()) _ = Validate(() => new RecommandClient(probe, options), "Recommand");
                services.AddSingleton(options);
                services.AddHttpClient<IPeppolAccessPointClient, RecommandClient>();
                return provider;
            default:
                throw new InvalidOperationException("AccessPoint:Provider must be 'storecove' or 'recommand'.");
        }
    }

    public static StorecoveOptions ReadStorecove(IConfigurationSection section) =>
        new(section["ApiKey"]!, section.GetValue<int>("LegalEntityId"), ReadBaseUri(section, "Storecove"));

    public static RecommandOptions ReadRecommand(IConfigurationSection section) =>
        new(section["ApiKey"]!, section["ApiSecret"] ?? "", section["CompanyId"] ?? "", ReadBaseUri(section, "Recommand"));

    private static Uri? ReadBaseUri(IConfigurationSection section, string name)
    {
        var raw = section["BaseUri"];
        if (string.IsNullOrWhiteSpace(raw)) return null;
        // A typo must not silently fall back to the production API with the configured key.
        // The value is left out of the messages in case it carries credentials.
        if (!Uri.TryCreate(raw, UriKind.Absolute, out var baseUri))
            throw new InvalidOperationException($"{name}:BaseUri is not an absolute URI.");
        // .NET reports file: URIs (a bare "/path" on Linux) as loopback, so check the scheme explicitly.
        if (!(baseUri.Scheme == Uri.UriSchemeHttps || (baseUri.Scheme == Uri.UriSchemeHttp && baseUri.IsLoopback)))
            throw new InvalidOperationException($"{name}:BaseUri must be https (plain http only for loopback test servers).");
        return baseUri;
    }

    private static T Validate<T>(Func<T> create, string name)
    {
        try { return create(); }
        catch (ArgumentException ex) { throw new InvalidOperationException($"{name} configuration is invalid: {ex.Message}", ex); }
    }
}
