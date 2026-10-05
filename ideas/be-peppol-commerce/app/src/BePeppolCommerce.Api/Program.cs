using BePeppolCommerce.Api;

var builder = WebApplication.CreateBuilder(args);

// See AccessPointConfig: AccessPoint:Provider picks Storecove (default) or Recommand.
AccessPointConfig.Register(builder.Services, builder.Configuration);

builder.Services.Configure<InboundWebhookOptions>(builder.Configuration.GetSection("Webhook"));

var app = builder.Build();

if (string.IsNullOrEmpty(app.Configuration["Webhook:Secret"]))
    app.Logger.LogWarning(app.Environment.IsDevelopment()
        ? "Webhook:Secret is not set; {Route} accepts unauthenticated calls (Development only)."
        : "Webhook:Secret is not set; {Route} answers 503 until it is.", InboundWebhook.Route);

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapPost(InboundWebhook.Route, InboundWebhook.Handle);

app.Run();

/// <summary>Entry point, public so tests can start the host with WebApplicationFactory.</summary>
public partial class Program;
