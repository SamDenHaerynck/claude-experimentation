using System.Collections.Concurrent;
using BePeppolCommerce.Core.AccessPoint;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BePeppolCommerce.Api.Tests;

/// <summary>
/// Startup configuration (Slice 8): the provider settings bind to typed options and every invalid
/// value stops the host with a message that names the setting but never contains its value.
/// </summary>
public class AccessPointConfigTests
{
    private const string Key = "placeholder-not-a-real-key";
    private const string Secret = "placeholder-not-a-real-secret";
    private const string CompanyId = "c_01JQZ8X0M4T7RB6K9V2NDHW3PA";

    private static WebApplicationFactory<Program> Host(Dictionary<string, string> settings, ILoggerProvider? logs = null) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            foreach (var (key, value) in settings) b.UseSetting(key, value);
            if (logs is not null) b.ConfigureLogging(l => l.AddProvider(logs));
        });

    private static Dictionary<string, string> Storecove(params (string Key, string Value)[] overrides)
    {
        var settings = new Dictionary<string, string> { ["Storecove:ApiKey"] = Key, ["Storecove:LegalEntityId"] = "1" };
        foreach (var (key, value) in overrides) settings[key] = value;
        return settings;
    }

    private static Dictionary<string, string> Recommand(params (string Key, string Value)[] overrides)
    {
        var settings = new Dictionary<string, string>
        {
            ["AccessPoint:Provider"] = "recommand",
            ["Recommand:ApiKey"] = Key,
            ["Recommand:ApiSecret"] = Secret,
            ["Recommand:CompanyId"] = CompanyId,
        };
        foreach (var (key, value) in overrides) settings[key] = value;
        return settings;
    }

    // WebApplicationFactory races when host start fails (it sometimes surfaces ObjectDisposedException
    // instead of the validation error), so failures run the same registration and the startup
    // validator that ValidateOnStart hooks into, directly.
    private static OptionsValidationException StartupFailure(Dictionary<string, string> settings)
    {
        var services = new ServiceCollection();
        AccessPointConfig.Register(services, new ConfigurationBuilder().AddInMemoryCollection(settings!).Build());
        using var provider = services.BuildServiceProvider();
        return Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IStartupValidator>().Validate());
    }

    [Fact]
    public async Task InvalidSetting_StopsARealHostAtStart()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection(Storecove(("Storecove:LegalEntityId", "0"))!);
        AccessPointConfig.Register(builder.Services, builder.Configuration);
        using var host = builder.Build();

        var ex = await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());

        Assert.Contains("Storecove:LegalEntityId", ex.Message);
    }

    [Fact]
    public void StorecoveConfigured_RegistersStorecoveClient()
    {
        using var factory = Host(Storecove());
        using var scope = factory.Services.CreateScope();

        Assert.IsType<StorecoveClient>(scope.ServiceProvider.GetRequiredService<IPeppolAccessPointClient>());
    }

    [Theory]
    [InlineData("recommand")]
    [InlineData(" Recommand ")]
    public void RecommandConfigured_RegistersRecommandClient(string provider)
    {
        using var factory = Host(Recommand(("AccessPoint:Provider", provider)));
        using var scope = factory.Services.CreateScope();

        Assert.IsType<RecommandClient>(scope.ServiceProvider.GetRequiredService<IPeppolAccessPointClient>());
    }

    [Fact]
    public void SchemeMap_BindsFromConfiguration()
    {
        using var factory = Host(Storecove(("Storecove:SchemeMap:0208", "BE:EN")));

        var options = factory.Services.GetRequiredService<StorecoveOptions>();

        Assert.Equal("BE:EN", options.SchemeMap!["0208"]);
    }

    [Theory]
    [InlineData("Storecove:ApiKey", "", false)] // blank key: no client, no validation (see SelectedProviderWithoutKey test)
    [InlineData("Storecove:LegalEntityId", "0", true)]
    [InlineData("Storecove:LegalEntityId", "-5", true)]
    [InlineData("Storecove:BaseUri", "not a uri", true)]
    [InlineData("Storecove:BaseUri", "http://api.example.invalid/", true)]
    [InlineData("Storecove:BaseUri", "/api/v2/", true)]
    [InlineData("Storecove:BaseUri", "ftp://localhost/", true)]
    [InlineData("Storecove:SchemeMap:BE", "BE:EN", true)]
    [InlineData("Storecove:SchemeMap:0208", " ", true)]
    public void InvalidStorecoveSetting_FailsAtStartupNamingTheSetting(string setting, string value, bool fails)
    {
        var settings = Storecove((setting, value));
        if (!fails)
        {
            using var factory = Host(settings);
            Assert.Null(factory.Services.GetService<IPeppolAccessPointClient>());
            return;
        }

        var ex = StartupFailure(settings);

        // "Storecove:SchemeMap:BE" is reported as "Storecove:SchemeMap keys ...", so match the prefix.
        var named = setting.StartsWith("Storecove:SchemeMap:0208") ? setting : string.Join(':', setting.Split(':').Take(2));
        Assert.Contains(named, ex.Message);
        Assert.DoesNotContain(Key, ex.Message);
        if (value.Trim().Length > 1) Assert.DoesNotContain(value, ex.Message);
    }

    [Theory]
    [InlineData("Recommand:ApiSecret", "")]
    [InlineData("Recommand:CompanyId", "")]
    [InlineData("Recommand:CompanyId", "c_1/../x")]
    [InlineData("Recommand:ApiKey", "user:pass-placeholder")]
    [InlineData("Recommand:BaseUri", "http://api.example.invalid/")]
    [InlineData("Recommand:BaseUri", "not a uri")]
    public void InvalidRecommandSetting_FailsAtStartupNamingTheSetting(string setting, string value)
    {
        var ex = StartupFailure(Recommand((setting, value)));

        Assert.Contains(setting, ex.Message);
        Assert.DoesNotContain(Secret, ex.Message);
        if (value.Length > 0) Assert.DoesNotContain(value, ex.Message);
    }

    [Fact]
    public void UnknownProvider_FailsAtStartup()
    {
        var ex = StartupFailure(new() { ["AccessPoint:Provider"] = "billit" });

        Assert.Contains("AccessPoint:Provider", ex.Message);
        Assert.DoesNotContain("billit", ex.Message);
    }

    [Fact]
    public void SeveralInvalidSettings_AreAllReported()
    {
        var ex = StartupFailure(Recommand(("Recommand:ApiSecret", ""), ("Recommand:CompanyId", "")));

        Assert.Contains("Recommand:ApiSecret", ex.Message);
        Assert.Contains("Recommand:CompanyId", ex.Message);
    }

    [Fact]
    public void OtherProvidersSection_IsNotValidated()
    {
        // Recommand is selected, so a half-filled Storecove section must not stop the host.
        using var factory = Host(Recommand(("Storecove:ApiKey", Key), ("Storecove:LegalEntityId", "0")));
        using var scope = factory.Services.CreateScope();

        Assert.IsType<RecommandClient>(scope.ServiceProvider.GetRequiredService<IPeppolAccessPointClient>());
    }

    [Theory]
    [InlineData("storecove")]
    [InlineData("recommand")]
    public void SelectedProviderWithoutKey_StartsWithoutClientAndLogsWarning(string provider)
    {
        var logs = new ListLoggerProvider();
        using var factory = Host(new() { ["AccessPoint:Provider"] = provider }, logs);

        Assert.Null(factory.Services.GetService<IPeppolAccessPointClient>());
        Assert.Contains(logs.Messages, m => m.Level == LogLevel.Warning && m.Text.Contains(provider) && m.Text.Contains("ApiKey is blank"));
    }

    [Fact]
    public void ConfiguredProvider_LogsNoKeyWarning()
    {
        var logs = new ListLoggerProvider();
        using var factory = Host(Storecove(), logs);
        _ = factory.Services;

        Assert.DoesNotContain(logs.Messages, m => m.Text.Contains("ApiKey is blank"));
    }

    private sealed class ListLoggerProvider : ILoggerProvider
    {
        public ConcurrentQueue<(LogLevel Level, string Text)> Messages { get; } = new();

        public ILogger CreateLogger(string categoryName) => new ListLogger(Messages);

        public void Dispose() { }

        private sealed class ListLogger(ConcurrentQueue<(LogLevel, string)> messages) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
                messages.Enqueue((logLevel, formatter(state, exception)));
        }
    }
}
