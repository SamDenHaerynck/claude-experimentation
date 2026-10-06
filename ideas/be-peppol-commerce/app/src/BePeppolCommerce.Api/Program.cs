using BePeppolCommerce.Api;

var builder = WebApplication.CreateBuilder(args);

// See AccessPointConfig: AccessPoint:Provider picks Storecove (default) or Recommand.
var accessPoint = AccessPointConfig.Register(builder.Services, builder.Configuration);

builder.Services.Configure<InboundWebhookOptions>(builder.Configuration.GetSection("Webhook"));

var app = builder.Build();

if (string.IsNullOrEmpty(app.Configuration["Webhook:Secret"]))
    app.Logger.LogWarning(app.Environment.IsDevelopment()
        ? "Webhook:Secret is not set; {Route} accepts unauthenticated calls (Development only)."
        : "Webhook:Secret is not set; {Route} answers 503 until it is.", InboundWebhook.Route);

if (!accessPoint.ClientRegistered && AccessPointOptions.Providers.Contains(accessPoint.Provider))
    app.Logger.LogWarning("{Provider} is the selected Access Point provider but its ApiKey is blank; {Route} answers 503 until it is set.",
        accessPoint.Provider, InboundWebhook.Route);

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapPost(InboundWebhook.Route, InboundWebhook.Handle);

app.Run();

/// <summary>Entry point, public so tests can start the host with WebApplicationFactory.</summary>
public partial class Program;
