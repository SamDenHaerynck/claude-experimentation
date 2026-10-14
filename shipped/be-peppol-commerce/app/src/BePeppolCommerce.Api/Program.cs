using BePeppolCommerce.Api;
using BePeppolCommerce.Core.Integration;

var builder = WebApplication.CreateBuilder(args);

// See AccessPointConfig: AccessPoint:Provider picks Storecove (default) or Recommand.
var accessPoint = AccessPointConfig.Register(builder.Services, builder.Configuration);

// Failed sends and receives are kept here as well as logged (see PeppolLogEvents). In memory only: a
// production host should register its own durable IFailedDocumentLog.
builder.Services.AddSingleton<IFailedDocumentLog, InMemoryFailedDocumentLog>(_ => new InMemoryFailedDocumentLog());

builder.Services.Configure<InboundWebhookOptions>(builder.Configuration.GetSection("Webhook"));

var app = builder.Build();

if (string.IsNullOrEmpty(app.Configuration["Webhook:Secret"]) && string.IsNullOrEmpty(app.Configuration["Webhook:SigningSecret"]))
    app.Logger.LogWarning(app.Environment.IsDevelopment()
        ? "Neither Webhook:Secret nor Webhook:SigningSecret is set; {Route} accepts unauthenticated calls (Development only)."
        : "Neither Webhook:Secret nor Webhook:SigningSecret is set; {Route} answers 503 until one is.", InboundWebhook.Route);

if (!accessPoint.ClientRegistered && AccessPointOptions.Providers.Contains(accessPoint.Provider))
    app.Logger.LogWarning("{Provider} is the selected Access Point provider but its ApiKey is blank; {Route} answers 503 until it is set.",
        accessPoint.Provider, InboundWebhook.Route);

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapPost(InboundWebhook.Route, InboundWebhook.Handle);

app.Run();

/// <summary>Entry point, public so tests can start the host with WebApplicationFactory.</summary>
public partial class Program;
