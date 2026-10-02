using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Model;
using BePeppolCommerce.Core.Outbound;
using BePeppolCommerce.Core.Validation;

namespace BePeppolCommerce.Core.Tests;

/// <summary>
/// End-to-end outbound tests: the real builder, validator and <see cref="StorecoveClient"/>, talking
/// over real HTTP to a fake Access Point on a loopback port.
/// </summary>
public class OutboundInvoiceSenderTests
{
    private const string SubmissionGuid = "5d0c6a1e-2f4b-4e8a-9c3d-7b1a0e6f2d94";

    private static Order LoadSample() =>
        OrderJson.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json")));

    /// <summary>A minimal HTTP server on 127.0.0.1 that records each request and replies with a canned response.</summary>
    private sealed class FakeAccessPoint : IAsyncDisposable
    {
        private readonly HttpListener _listener = new();
        private readonly Task _loop;

        public ConcurrentQueue<(string Method, string Path, string? Authorization, string Body)> Requests { get; } = new();
        public Uri BaseUri { get; }

        public FakeAccessPoint(int status, string responseJson)
        {
            var port = FreePort();
            BaseUri = new Uri($"http://127.0.0.1:{port}/api/v2/");
            _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
            _listener.Start();
            _loop = Task.Run(async () =>
            {
                while (_listener.IsListening)
                {
                    HttpListenerContext ctx;
                    try { ctx = await _listener.GetContextAsync(); }
                    catch (Exception ex) when (ex is HttpListenerException or ObjectDisposedException) { return; }
                    using (var reader = new StreamReader(ctx.Request.InputStream, Encoding.UTF8))
                        Requests.Enqueue((ctx.Request.HttpMethod, ctx.Request.Url!.AbsolutePath, ctx.Request.Headers["Authorization"], await reader.ReadToEndAsync()));
                    var bytes = Encoding.UTF8.GetBytes(responseJson);
                    ctx.Response.StatusCode = status;
                    ctx.Response.ContentType = "application/json";
                    ctx.Response.ContentLength64 = bytes.Length;
                    await ctx.Response.OutputStream.WriteAsync(bytes);
                    ctx.Response.Close();
                }
            });
        }

        public OutboundInvoiceSender CreateSender()
        {
            var client = new StorecoveClient(new HttpClient(), new StorecoveOptions("test-key-placeholder", 42, BaseUri));
            return new OutboundInvoiceSender(client);
        }

        private static int FreePort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public async ValueTask DisposeAsync()
        {
            _listener.Stop();
            _listener.Close();
            await _loop;
        }
    }

    [Fact]
    public async Task SampleOrder_IsValidatedAndReachesFakeAccessPointAsBase64Ubl()
    {
        await using var ap = new FakeAccessPoint(200, $"{{\"guid\":\"{SubmissionGuid}\"}}");
        var order = LoadSample();

        var result = await ap.CreateSender().SendAsync(order);

        Assert.Equal(OutboundStatus.Sent, result.Status);
        Assert.Equal(SubmissionGuid, result.SubmissionId);
        Assert.True(result.Validation.IsValid);
        Assert.Empty(result.Validation.Findings);

        var request = Assert.Single(ap.Requests);
        Assert.Equal("POST", request.Method);
        Assert.Equal("/api/v2/document_submissions", request.Path);
        Assert.Equal("Bearer test-key-placeholder", request.Authorization);

        using var body = JsonDocument.Parse(request.Body);
        var root = body.RootElement;
        Assert.Equal(OutboundInvoiceSender.DeriveIdempotencyKey(order).ToString("D"), root.GetProperty("idempotencyGuid").GetString());
        var id = root.GetProperty("routing").GetProperty("eIdentifiers")[0];
        Assert.Equal("0208", id.GetProperty("scheme").GetString());
        Assert.Equal("0000000196", id.GetProperty("id").GetString()); // the buyer, not the seller
        var sentXml = Encoding.UTF8.GetString(Convert.FromBase64String(
            root.GetProperty("document").GetProperty("rawDocumentData").GetProperty("document").GetString()!));
        Assert.Equal(result.UblXml, sentXml);

        // What arrived at the fake is itself a valid Peppol invoice for this order.
        Assert.True(PeppolValidator.Validate(sentXml).IsValid);
        var sent = PeppolValidator.ParseUntrusted(sentXml);
        Assert.Equal("INV-2026-0001", sent.Root!.Element(Ubl.PeppolInvoiceBuilder.Cbc + "ID")!.Value);
    }

    [Fact]
    public async Task OrderWithoutBuyerReference_FailsValidationAndNeverReachesFake()
    {
        await using var ap = new FakeAccessPoint(200, $"{{\"guid\":\"{SubmissionGuid}\"}}");
        var order = LoadSample() with { BuyerReference = null };

        var result = await ap.CreateSender().SendAsync(order);

        Assert.Equal(OutboundStatus.ValidationFailed, result.Status);
        Assert.Null(result.Send);
        Assert.Null(result.SubmissionId);
        Assert.NotNull(result.UblXml);
        Assert.Contains("PEPPOL-EN16931-R003", result.Validation.Errors.Select(e => e.RuleId));
        Assert.Empty(ap.Requests);
    }

    [Fact]
    public async Task OrderWithoutLines_FailsWithBuildFindingAndNeverReachesFake()
    {
        await using var ap = new FakeAccessPoint(200, $"{{\"guid\":\"{SubmissionGuid}\"}}");
        var order = LoadSample() with { Lines = [] };

        var result = await ap.CreateSender().SendAsync(order);

        Assert.Equal(OutboundStatus.ValidationFailed, result.Status);
        Assert.Null(result.UblXml);
        Assert.Equal(OutboundInvoiceSender.BuildRuleId, Assert.Single(result.Validation.Errors).RuleId);
        Assert.Empty(ap.Requests);
    }

    [Fact]
    public async Task ProviderRejection_IsSendFailedWithProviderErrors()
    {
        await using var ap = new FakeAccessPoint(422, "[{\"source\":\"routing\",\"details\":\"Unknown receiver\"}]");

        var result = await ap.CreateSender().SendAsync(LoadSample());

        Assert.Equal(OutboundStatus.SendFailed, result.Status);
        Assert.Null(result.SubmissionId);
        Assert.True(result.Validation.IsValid);
        Assert.Equal(422, result.Send!.HttpStatus);
        var error = Assert.Single(result.Send.Errors);
        Assert.Equal("routing", error.Source);
        Assert.Equal("Unknown receiver", error.Details);
        Assert.Single(ap.Requests);
    }

    [Fact]
    public async Task UnreachableAccessPoint_IsSendFailedNotException()
    {
        FakeAccessPoint ap = new(200, "{}");
        var sender = ap.CreateSender();
        await ap.DisposeAsync(); // nothing listens on the port any more

        var result = await sender.SendAsync(LoadSample());

        Assert.Equal(OutboundStatus.SendFailed, result.Status);
        Assert.Null(result.Send!.HttpStatus);
        Assert.Equal("transport", Assert.Single(result.Send.Errors).Source);
    }

    [Fact]
    public async Task ExplicitIdempotencyKey_IsPassedThrough()
    {
        await using var ap = new FakeAccessPoint(200, $"{{\"guid\":\"{SubmissionGuid}\"}}");
        var key = Guid.Parse("0f8e2d4c-6b1a-4c3e-9d7f-1a2b3c4d5e6f");

        await ap.CreateSender().SendAsync(LoadSample(), key);

        using var body = JsonDocument.Parse(Assert.Single(ap.Requests).Body);
        Assert.Equal(key.ToString("D"), body.RootElement.GetProperty("idempotencyGuid").GetString());
    }

    [Fact]
    public void DerivedIdempotencyKey_IsStablePerInvoiceAndDiffersAcrossInvoices()
    {
        var order = LoadSample();

        Assert.Equal(OutboundInvoiceSender.DeriveIdempotencyKey(order), OutboundInvoiceSender.DeriveIdempotencyKey(LoadSample()));
        Assert.NotEqual(OutboundInvoiceSender.DeriveIdempotencyKey(order),
            OutboundInvoiceSender.DeriveIdempotencyKey(order with { InvoiceNumber = "INV-2026-0002" }));
    }

    [Fact]
    public async Task NullOrder_Throws()
    {
        var sender = new OutboundInvoiceSender(new StorecoveClient(new HttpClient(), new StorecoveOptions("test-key-placeholder", 42)));
        await Assert.ThrowsAsync<ArgumentNullException>(() => sender.SendAsync(null!));
    }
}
