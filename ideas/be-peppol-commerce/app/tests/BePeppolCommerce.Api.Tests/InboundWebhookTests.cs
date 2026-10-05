using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BePeppolCommerce.Api;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Model;
using BePeppolCommerce.Core.Ubl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BePeppolCommerce.Api.Tests;

/// <summary>
/// The real host (routing, configuration, handler) through WebApplicationFactory, with the Access
/// Point client replaced by an in-memory fake.
/// </summary>
public class InboundWebhookTests
{
    private const string DocumentGuid = "0b6f2a3c-1d4e-4f5a-8b9c-0d1e2f3a4b5c";

    private static string SampleInvoiceXml() =>
        PeppolInvoiceBuilder.Build(OrderJson.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json")))).ToString();

    private sealed class FakeAccessPoint(Func<string, AccessPointResult<InboundDocument>> onGet) : IPeppolAccessPointClient
    {
        public ConcurrentQueue<string> Fetched { get; } = new();

        public Task<AccessPointResult<string>> SendAsync(OutboundDocument document, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<AccessPointResult<InboundDocument>> GetInboundAsync(string providerDocumentId, CancellationToken cancellationToken = default)
        {
            Fetched.Enqueue(providerDocumentId);
            return Task.FromResult(onGet(providerDocumentId));
        }
    }

    private static FakeAccessPoint Returning(string xml) =>
        new(id => AccessPointResult<InboundDocument>.Ok(new InboundDocument(id, xml), 200));

    private static HttpClient Client(IPeppolAccessPointClient? accessPoint, string? secret = null, string environment = "Development")
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment(environment);
            if (secret is not null) b.UseSetting("Webhook:Secret", secret);
            b.ConfigureTestServices(services =>
            {
                services.RemoveAll<IPeppolAccessPointClient>();
                if (accessPoint is not null) services.AddSingleton(accessPoint);
            });
        });
        return factory.CreateClient();
    }

    private static StringContent Json(string json) => new(json, Encoding.UTF8, "application/json");

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await Client(null).GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("guid")]
    [InlineData("document_guid")]
    public async Task Webhook_FetchesParsesAndReturnsInvoice(string property)
    {
        var fake = Returning(SampleInvoiceXml());

        var response = await Client(fake).PostAsync(InboundWebhook.Route, Json($$"""{ "{{property}}": "{{DocumentGuid}}" }"""));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal([DocumentGuid], fake.Fetched);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = body.RootElement;
        Assert.Equal(DocumentGuid, root.GetProperty("providerDocumentId").GetString());
        var invoice = root.GetProperty("invoice");
        Assert.Equal("INV-2026-0001", invoice.GetProperty("invoiceNumber").GetString());
        Assert.Equal("2026-09-28", invoice.GetProperty("issueDate").GetString());
        Assert.Equal("EUR", invoice.GetProperty("currency").GetString());
        Assert.Equal("0000000097", invoice.GetProperty("seller").GetProperty("endpoint").GetProperty("id").GetString());
        Assert.Equal(3, invoice.GetProperty("lineCount").GetInt32());
        Assert.Equal(268.63m, invoice.GetProperty("payableAmount").GetDecimal());
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("""{ "guid": "" }""")]
    [InlineData("""{ "guid": 42 }""")]
    [InlineData("""{ "guid": null }""")]
    [InlineData("""{ "guid": "not-a-guid" }""")]
    [InlineData("""{ "guid": "0b6f2a3c-1d4e-4f5a-8b9c-0d1e2f3a4b5c\nforged log line" }""")]
    [InlineData("""{ "guid": "doc_01JQZ8X0M4T7RB6K9V2NDHW3PA" }""")]
    [InlineData("""{ "documentId": "" }""")]
    [InlineData("""{ "documentId": 42 }""")]
    [InlineData("""{ "documentId": ".." }""")]
    [InlineData("""{ "documentId": "doc_1\nforged log line" }""")]
    [InlineData("""{ "documentId": "doc_1\n" }""")]
    public async Task Webhook_BadBody_Returns400WithoutFetching(string json)
    {
        var fake = Returning(SampleInvoiceXml());

        var response = await Client(fake).PostAsync(InboundWebhook.Route, Json(json));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(fake.Fetched);
    }

    [Fact]
    public async Task Webhook_OversizedBody_Returns413WithoutFetching()
    {
        var fake = Returning(SampleInvoiceXml());
        var padding = new string(' ', InboundWebhook.MaxBodyBytes);

        var response = await Client(fake).PostAsync(InboundWebhook.Route, Json($$"""{ "guid": "{{DocumentGuid}}" }{{padding}}"""));

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
        Assert.Empty(fake.Fetched);
    }

    [Fact]
    public async Task Webhook_OversizedChunkedBodyWithoutContentLength_Returns413()
    {
        var fake = Returning(SampleInvoiceXml());
        var bytes = Encoding.UTF8.GetBytes($$"""{ "guid": "{{DocumentGuid}}" }""" + new string(' ', InboundWebhook.MaxBodyBytes));
        var content = new StreamContent(new MemoryStream(bytes));
        content.Headers.ContentType = new("application/json");
        content.Headers.ContentLength = null;
        var request = new HttpRequestMessage(HttpMethod.Post, InboundWebhook.Route) { Content = content };
        request.Headers.TransferEncodingChunked = true;

        var response = await Client(fake).SendAsync(request);

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
        Assert.Empty(fake.Fetched);
    }

    [Fact]
    public async Task Webhook_MalformedDocumentFromProvider_Returns422()
    {
        var response = await Client(Returning("<Invoice><unclosed>")).PostAsync(InboundWebhook.Route, Json($$"""{ "guid": "{{DocumentGuid}}" }"""));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Webhook_ProviderFailure_Returns502WithoutLeakingProviderErrors()
    {
        var fake = new FakeAccessPoint(_ => AccessPointResult<InboundDocument>.Fail(401, new AccessPointError("storecove", "secret-ish provider detail")));

        var response = await Client(fake).PostAsync(InboundWebhook.Route, Json($$"""{ "guid": "{{DocumentGuid}}" }"""));

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.DoesNotContain("secret-ish", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Webhook_OutsideDevelopmentWithoutSecret_Returns503WithoutFetching()
    {
        var fake = Returning(SampleInvoiceXml());

        var response = await Client(fake, environment: "Production").PostAsync(InboundWebhook.Route, Json($$"""{ "guid": "{{DocumentGuid}}" }"""));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Empty(fake.Fetched);
    }

    [Fact]
    public async Task Webhook_OutsideDevelopmentWithSecret_Works()
    {
        var fake = Returning(SampleInvoiceXml());
        var request = new HttpRequestMessage(HttpMethod.Post, InboundWebhook.Route) { Content = Json($$"""{ "guid": "{{DocumentGuid.ToUpperInvariant()}}" }""") };
        request.Headers.Add(InboundWebhook.SecretHeader, "s3cret-placeholder");

        var response = await Client(fake, secret: "s3cret-placeholder", environment: "Production").SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal([DocumentGuid], fake.Fetched); // canonical lower-case form
    }

    [Fact]
    public void ApiKeyConfigured_RegistersStorecoveClient()
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.UseSetting("Storecove:ApiKey", "placeholder-not-a-real-key");
            b.UseSetting("Storecove:LegalEntityId", "1");
        });

        using var scope = factory.Services.CreateScope();

        Assert.IsType<StorecoveClient>(scope.ServiceProvider.GetRequiredService<IPeppolAccessPointClient>());
    }

    [Fact]
    public void RecommandProvider_RegistersRecommandClient()
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.UseSetting("AccessPoint:Provider", "Recommand");
            b.UseSetting("Storecove:ApiKey", "placeholder-not-a-real-key");
            b.UseSetting("Recommand:ApiKey", "placeholder-not-a-real-key");
            b.UseSetting("Recommand:ApiSecret", "placeholder-not-a-real-secret");
            b.UseSetting("Recommand:CompanyId", "c_01JQZ8X0M4T7RB6K9V2NDHW3PA");
        });

        using var scope = factory.Services.CreateScope();

        Assert.IsType<RecommandClient>(scope.ServiceProvider.GetRequiredService<IPeppolAccessPointClient>());
    }

    [Fact]
    public void RecommandProviderWithoutKey_RegistersNoClient()
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.UseSetting("AccessPoint:Provider", "recommand");
            b.UseSetting("Storecove:ApiKey", "placeholder-not-a-real-key");
        });

        using var scope = factory.Services.CreateScope();

        Assert.Null(scope.ServiceProvider.GetService<IPeppolAccessPointClient>());
    }

    [Theory]
    [InlineData("billit", "", "c_1")]
    [InlineData("recommand", "", "c_1")]
    [InlineData("recommand", "placeholder-not-a-real-secret", "")]
    [InlineData("recommand", "placeholder-not-a-real-secret", "c_1/../x")]
    public void BadProviderConfiguration_FailsAtStartup(string provider, string secret, string companyId)
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.UseSetting("AccessPoint:Provider", provider);
            b.UseSetting("Recommand:ApiKey", "placeholder-not-a-real-key");
            b.UseSetting("Recommand:ApiSecret", secret);
            b.UseSetting("Recommand:CompanyId", companyId);
        });

        Assert.Throws<InvalidOperationException>(() => factory.Services);
    }

    [Theory]
    [InlineData("doc_01JQZ8X0M4T7RB6K9V2NDHW3PA", "doc_01JQZ8X0M4T7RB6K9V2NDHW3PA")]
    [InlineData("0B6F2A3C-1D4E-4F5A-8B9C-0D1E2F3A4B5C", "0b6f2a3c-1d4e-4f5a-8b9c-0d1e2f3a4b5c")]
    public async Task Webhook_DocumentIdProperty_AcceptsRecommandStyleIdsAndGuids(string id, string expected)
    {
        var fake = Returning(SampleInvoiceXml());

        var response = await Client(fake).PostAsync(InboundWebhook.Route, Json($$"""{ "documentId": "{{id}}" }"""));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal([expected], fake.Fetched);
    }

    [Theory]
    [InlineData("not a uri")]
    [InlineData("http://api.example.invalid/")]
    [InlineData("/api/v2/")]
    [InlineData("ftp://localhost/")]
    public void InvalidBaseUri_FailsAtStartup(string baseUri)
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.UseSetting("Storecove:ApiKey", "placeholder-not-a-real-key");
            b.UseSetting("Storecove:BaseUri", baseUri);
        });

        Assert.Throws<InvalidOperationException>(() => factory.Services);
    }

    [Fact]
    public async Task Webhook_NoProviderConfigured_Returns503()
    {
        var response = await Client(null).PostAsync(InboundWebhook.Route, Json($$"""{ "guid": "{{DocumentGuid}}" }"""));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("wrong", HttpStatusCode.Unauthorized)]
    [InlineData("s3cret-placeholder", HttpStatusCode.OK)]
    public async Task Webhook_WithSecretConfigured_RequiresMatchingHeader(string? header, HttpStatusCode expected)
    {
        var fake = Returning(SampleInvoiceXml());
        var request = new HttpRequestMessage(HttpMethod.Post, InboundWebhook.Route) { Content = Json($$"""{ "guid": "{{DocumentGuid}}" }""") };
        if (header is not null) request.Headers.Add(InboundWebhook.SecretHeader, header);

        var response = await Client(fake, secret: "s3cret-placeholder").SendAsync(request);

        Assert.Equal(expected, response.StatusCode);
        Assert.Equal(expected == HttpStatusCode.OK ? 1 : 0, fake.Fetched.Count);
    }
}
