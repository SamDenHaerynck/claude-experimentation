using BePeppolCommerce.Core.AccessPoint;
using Microsoft.Extensions.Options;

namespace BePeppolCommerce.Api;

/// <summary><c>AccessPoint</c> section: which provider the host sends and fetches through.</summary>
public sealed class AccessPointOptions
{
    public const string SectionName = "AccessPoint";
    public static readonly string[] Providers = ["storecove", "recommand"];

    /// <summary>"storecove" (the default when blank) or "recommand", case-insensitive.</summary>
    public string? Provider { get; set; }

    /// <summary>The trimmed, lower-case provider name, "storecove" when none is set.</summary>
    public string ProviderName => string.IsNullOrWhiteSpace(Provider) ? "storecove" : Provider.Trim().ToLowerInvariant();

    public IEnumerable<string> Problems()
    {
        if (!Providers.Contains(ProviderName)) yield return "Provider must be 'storecove' or 'recommand'.";
    }
}

/// <summary><c>Storecove</c> section, bound from appsettings.json or <c>Storecove__*</c> environment variables.</summary>
public sealed class StorecoveSection
{
    public const string SectionName = "Storecove";

    public string? ApiKey { get; set; }
    public int LegalEntityId { get; set; }
    public string? BaseUri { get; set; }

    /// <summary>Peppol ICD scheme to Storecove scheme name, e.g. <c>Storecove:SchemeMap:0208</c>.</summary>
    public Dictionary<string, string> SchemeMap { get; set; } = new(StringComparer.Ordinal);

    public StorecoveOptions ToOptions() =>
        new(ApiKey ?? "", LegalEntityId, AccessPointConfig.ParseBaseUri(BaseUri), SchemeMap.Count == 0 ? null : SchemeMap);

    public IEnumerable<string> Problems() =>
        AccessPointConfig.BaseUriProblem(BaseUri) is { } problem
            ? [problem]
            : StorecoveOptions.Validate(ToOptions());
}

/// <summary><c>Recommand</c> section, bound from appsettings.json or <c>Recommand__*</c> environment variables.</summary>
public sealed class RecommandSection
{
    public const string SectionName = "Recommand";

    public string? ApiKey { get; set; }
    public string? ApiSecret { get; set; }
    public string? CompanyId { get; set; }
    public string? BaseUri { get; set; }

    public RecommandOptions ToOptions() =>
        new(ApiKey ?? "", ApiSecret ?? "", CompanyId ?? "", AccessPointConfig.ParseBaseUri(BaseUri));

    public IEnumerable<string> Problems() =>
        AccessPointConfig.BaseUriProblem(BaseUri) is { } problem
            ? [problem]
            : RecommandOptions.Validate(ToOptions());
}

/// <summary>What <see cref="AccessPointConfig.Register"/> chose: the provider, and whether its client was registered.</summary>
public sealed record AccessPointRegistration(string Provider, bool ClientRegistered);

/// <summary>
/// Binds the provider settings to typed options and registers the selected provider's client. Every
/// invalid value fails host startup (<c>ValidateOnStart</c>) with an <see cref="OptionsValidationException"/>
/// naming the setting but never its value. The client is registered only when the selected
/// provider's section has an API key, so the host still starts locally without one; the webhook then
/// answers 503 and Program logs a warning.
/// </summary>
public static class AccessPointConfig
{
    public static AccessPointRegistration Register(IServiceCollection services, IConfiguration configuration)
    {
        AddValidated<AccessPointOptions>(services, configuration, AccessPointOptions.SectionName, o => o.Problems());

        // The provider decides which typed client to register, so it is read now rather than at startup.
        var provider = (configuration.GetSection(AccessPointOptions.SectionName).Get<AccessPointOptions>() ?? new()).ProviderName;
        switch (provider)
        {
            case "storecove" when HasApiKey(configuration, StorecoveSection.SectionName):
                AddValidated<StorecoveSection>(services, configuration, StorecoveSection.SectionName, s => s.Problems());
                services.AddSingleton(sp => sp.GetRequiredService<IOptions<StorecoveSection>>().Value.ToOptions());
                services.AddHttpClient<IPeppolAccessPointClient, StorecoveClient>();
                return new(provider, true);
            case "recommand" when HasApiKey(configuration, RecommandSection.SectionName):
                AddValidated<RecommandSection>(services, configuration, RecommandSection.SectionName, s => s.Problems());
                services.AddSingleton(sp => sp.GetRequiredService<IOptions<RecommandSection>>().Value.ToOptions());
                services.AddHttpClient<IPeppolAccessPointClient, RecommandClient>();
                return new(provider, true);
            default:
                // A blank key, or an unknown provider that AccessPointOptions validation rejects at startup.
                return new(provider, false);
        }
    }

    internal static Uri? ParseBaseUri(string? raw) =>
        string.IsNullOrWhiteSpace(raw) ? null : Uri.TryCreate(raw, UriKind.Absolute, out var uri) ? uri : null;

    /// <summary>A typo must not silently fall back to the production API with the configured key.</summary>
    internal static string? BaseUriProblem(string? raw) =>
        string.IsNullOrWhiteSpace(raw) || Uri.TryCreate(raw, UriKind.Absolute, out _) ? null : "BaseUri is not an absolute URI.";

    private static bool HasApiKey(IConfiguration configuration, string section) =>
        !string.IsNullOrWhiteSpace(configuration[$"{section}:ApiKey"]);

    private static void AddValidated<T>(IServiceCollection services, IConfiguration configuration, string section, Func<T, IEnumerable<string>> problems)
        where T : class
    {
        services.AddOptions<T>().Bind(configuration.GetSection(section)).ValidateOnStart();
        services.AddSingleton<IValidateOptions<T>>(new SectionValidator<T>(section, problems));
    }

    /// <summary>Prefixes each problem with its section, so the message reads "Storecove:LegalEntityId ...".</summary>
    private sealed class SectionValidator<T>(string section, Func<T, IEnumerable<string>> problems) : IValidateOptions<T>
        where T : class
    {
        public ValidateOptionsResult Validate(string? name, T options)
        {
            var failures = problems(options).Select(p => $"{section}:{p}").ToList();
            return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
        }
    }
}
