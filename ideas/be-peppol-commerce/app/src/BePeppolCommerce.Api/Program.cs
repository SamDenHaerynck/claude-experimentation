using BePeppolCommerce.Api;
using BePeppolCommerce.Core.AccessPoint;

var builder = WebApplication.CreateBuilder(args);

// The Access Point client is registered only when an API key is configured, so the host still
// starts locally without one; the webhook then answers 503 (see InboundWebhook).
var storecove = builder.Configuration.GetSection("Storecove");
if (!string.IsNullOrWhiteSpace(storecove["ApiKey"]))
{
    var options = new StorecoveOptions(
        storecove["ApiKey"]!,
        storecove.GetValue<int>("LegalEntityId"),
        Uri.TryCreate(storecove["BaseUri"], UriKind.Absolute, out var baseUri) ? baseUri : null);
    builder.Services.AddSingleton(options);
    builder.Services.AddHttpClient<IPeppolAccessPointClient, StorecoveClient>();
}

builder.Services.Configure<InboundWebhookOptions>(builder.Configuration.GetSection("Webhook"));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapPost(InboundWebhook.Route, InboundWebhook.Handle);

app.Run();

/// <summary>Entry point, public so tests can start the host with WebApplicationFactory.</summary>
public partial class Program;
