using BePeppolCommerce.Core.Integration;
using System.Net;
using System.Text;
using System.Text.Json;
using BePeppolCommerce.Core.AccessPoint;

namespace BePeppolCommerce.Core.Tests;

public class StorecoveClientTests
{
    private const string Xml = "<Invoice xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2\"/>";
    private const string Guid1 = "3f1c2a8e-5b7d-4c1e-9a0f-2d6b8e4c7a11";
    private const string IdemGuid = "9b2e7c41-0d3a-4f6e-8c15-7a9d2b4e6f80";
    private static readonly PeppolParticipant Buyer = new("0208", "0123456789");

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Request = request;
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(ct);
            return respond(request);
        }
    }

    private static HttpResponseMessage Json(HttpStatusCode code, string json) =>
        new(code) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    private static (StorecoveClient, StubHandler) Create(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        var handler = new StubHandler(respond);
        return (new StorecoveClient(new HttpClient(handler), new StorecoveOptions("test-key-placeholder", 42)), handler);
    }

    [Fact]
    public async Task Send_Success_PostsRawUblAndReturnsGuid()
    {
        var (client, handler) = Create(_ => Json(HttpStatusCode.OK, $"{{\"guid\":\"{Guid1}\"}}"));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer, Guid.Parse(IdemGuid)));

        Assert.True(result.Success);
        Assert.Equal(Guid1, result.Value);
        Assert.Equal(200, result.HttpStatus);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal("https://api.storecove.com/api/v2/document_submissions", handler.Request.RequestUri!.ToString());
        Assert.Equal("Bearer", handler.Request.Headers.Authorization!.Scheme);
        Assert.Equal("test-key-placeholder", handler.Request.Headers.Authorization.Parameter);

        using var body = JsonDocument.Parse(handler.Body!);
        var root = body.RootElement;
        Assert.Equal(42, root.GetProperty("legalEntityId").GetInt32());
        Assert.Equal(IdemGuid, root.GetProperty("idempotencyGuid").GetString());
        var id = root.GetProperty("routing").GetProperty("eIdentifiers")[0];
        Assert.Equal("0208", id.GetProperty("scheme").GetString());
        Assert.Equal("0123456789", id.GetProperty("id").GetString());
        var doc = root.GetProperty("document");
        Assert.Equal("invoice", doc.GetProperty("documentType").GetString());
        var raw = doc.GetProperty("rawDocumentData");
        Assert.Equal("ubl", raw.GetProperty("parseStrategy").GetString());
        Assert.Equal(Xml, Encoding.UTF8.GetString(Convert.FromBase64String(raw.GetProperty("document").GetString()!)));
    }

    [Fact]
    public async Task Send_WithoutIdempotencyKey_OmitsField()
    {
        var (client, handler) = Create(_ => Json(HttpStatusCode.OK, $"{{\"guid\":\"{Guid1}\"}}"));

        await client.SendAsync(new OutboundDocument(Xml, Buyer));

        using var body = JsonDocument.Parse(handler.Body!);
        Assert.False(body.RootElement.TryGetProperty("idempotencyGuid", out _));
    }

    [Fact]
    public async Task Send_422_ReturnsProviderErrors()
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.UnprocessableEntity,
            "[{\"source\":\"routing\",\"details\":\"Receiver not found\"},{\"source\":\"document\",\"details\":\"Invalid UBL\"}]"));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal(422, result.HttpStatus);
        Assert.Equal(new[] { "routing", "document" }, result.Errors.Select(e => e.Source));
        Assert.Equal("Receiver not found", result.Errors[0].Details);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, "")]
    [InlineData(HttpStatusCode.InternalServerError, "<html>oops</html>")]
    public async Task Send_HttpErrorWithoutErrorModel_ReturnsStatusFallback(HttpStatusCode code, string body)
    {
        var (client, _) = Create(_ => new HttpResponseMessage(code) { Content = new StringContent(body) });

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Equal((int)code, result.HttpStatus);
        Assert.Equal("http", Assert.Single(result.Errors).Source);
    }

    [Fact]
    public async Task Send_TransportFailure_ReturnsFailureNotException()
    {
        var (client, _) = Create(_ => throw new HttpRequestException("connection refused"));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Null(result.HttpStatus);
        Assert.Equal("transport", Assert.Single(result.Errors).Source);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("not json")]
    public async Task Send_SuccessWithUnusableBody_ReturnsFailure(string body)
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK, body));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Equal("client", Assert.Single(result.Errors).Source);
    }

    [Fact]
    public async Task Send_CallerCancellation_Throws()
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK, "{}"));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.SendAsync(new OutboundDocument(Xml, Buyer), cts.Token));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetInbound_Success_ReturnsXml(bool base64)
    {
        var original = base64 ? Convert.ToBase64String(Encoding.UTF8.GetBytes(Xml)) : Xml;
        var (client, handler) = Create(_ => Json(HttpStatusCode.OK,
            JsonSerializer.Serialize(new { guid = Guid1, direction = "in", original })));

        var result = await client.GetInboundAsync(Guid1);

        Assert.True(result.Success, string.Join(", ", result.Errors));
        Assert.Equal(Guid1, result.Value!.ProviderDocumentId);
        Assert.Equal(Xml, result.Value.UblXml);
        Assert.Equal(HttpMethod.Get, handler.Request!.Method);
        Assert.Equal($"https://api.storecove.com/api/v2/received_documents/{Guid1}/original", handler.Request.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetInbound_404_ReturnsFailure()
    {
        var (client, _) = Create(_ => new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await client.GetInboundAsync(Guid1);

        Assert.False(result.Success);
        Assert.Equal(404, result.HttpStatus);
    }

    [Theory]
    [InlineData("{\"guid\":\"x\"}")]
    [InlineData("{\"original\":\"bm90IHhtbA==\"}")]
    [InlineData("{\"original\":\"%%%\"}")]
    public async Task GetInbound_NoUsableOriginal_ReturnsFailure(string body)
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK, body));

        var result = await client.GetInboundAsync(Guid1);

        Assert.False(result.Success);
        Assert.Equal("client", Assert.Single(result.Errors).Source);
    }

    [Fact]
    public async Task GetInbound_NonGuidId_FailsWithoutHttpCall()
    {
        var (client, handler) = Create(_ => throw new InvalidOperationException("should not be called"));

        var result = await client.GetInboundAsync("../legal_entities");

        Assert.False(result.Success);
        Assert.Null(handler.Request);
    }

    [Fact]
    public void Constructor_RejectsMissingKey()
    {
        Assert.Throws<ArgumentException>(() => new StorecoveClient(new HttpClient(), new StorecoveOptions(" ", 1)));
    }

    [Fact]
    public void Validate_ValidOptions_ReturnsNoProblems()
    {
        var map = new Dictionary<string, string> { ["0208"] = "XX:TEST" };
        Assert.Empty(StorecoveOptions.Validate(new StorecoveOptions("k", 1, new Uri("https://api.example.com/"), map)));
    }

    [Theory]
    [InlineData(" ", 1, null, "ApiKey")]
    [InlineData("secret-key-placeholder", 0, null, "LegalEntityId")]
    [InlineData("secret-key-placeholder", -1, null, "LegalEntityId")]
    [InlineData("secret-key-placeholder", 1, "http://api.example.com/", "BaseUri")]
    [InlineData("secret-key-placeholder", 1, "file:///api/v2/", "BaseUri")]
    public void Validate_NamesTheBadSettingWithoutItsValue(string key, int legalEntityId, string? baseUri, string setting)
    {
        var problems = StorecoveOptions.Validate(new StorecoveOptions(key, legalEntityId, baseUri is null ? null : new Uri(baseUri)));

        var problem = Assert.Single(problems);
        Assert.StartsWith(setting, problem);
        Assert.DoesNotContain("secret-key-placeholder", problem);
        Assert.Throws<ArgumentException>(() => new StorecoveClient(new HttpClient(), new StorecoveOptions(key, legalEntityId, baseUri is null ? null : new Uri(baseUri))));
    }

    [Theory]
    [InlineData("BE", "XX:TEST")]
    [InlineData("02080", "XX:TEST")]
    [InlineData("0208", " ")]
    public void Validate_RejectsBadSchemeMapEntries(string scheme, string name)
    {
        var options = new StorecoveOptions("k", 1, SchemeMap: new Dictionary<string, string> { [scheme] = name });

        Assert.StartsWith("SchemeMap", Assert.Single(StorecoveOptions.Validate(options)));
    }

    [Fact]
    public async Task CustomBaseUri_WithoutTrailingSlash_KeepsPath()
    {
        var handler = new StubHandler(_ => Json(HttpStatusCode.OK, $"{{\"guid\":\"{Guid1}\"}}"));
        var client = new StorecoveClient(new HttpClient(handler), new StorecoveOptions("k", 1, new Uri("http://localhost:5099/api/v2")));

        await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.Equal("http://localhost:5099/api/v2/document_submissions", handler.Request!.RequestUri!.ToString());
    }

    [Fact]
    public void Constructor_RejectsPlainHttpToNonLoopbackHost()
    {
        Assert.Throws<ArgumentException>(() => new StorecoveClient(new HttpClient(), new StorecoveOptions("k", 1, new Uri("http://api.example.com/"))));
    }

    [Fact]
    public async Task Send_SchemeMap_TranslatesIcdScheme()
    {
        var handler = new StubHandler(_ => Json(HttpStatusCode.OK, $"{{\"guid\":\"{Guid1}\"}}"));
        var map = new Dictionary<string, string> { ["0208"] = "XX:TEST" };
        var client = new StorecoveClient(new HttpClient(handler), new StorecoveOptions("k", 1, SchemeMap: map));

        await client.SendAsync(new OutboundDocument(Xml, Buyer));

        using var body = JsonDocument.Parse(handler.Body!);
        Assert.Equal("XX:TEST", body.RootElement.GetProperty("routing").GetProperty("eIdentifiers")[0].GetProperty("scheme").GetString());
    }

    [Fact]
    public async Task Factory_PassesSchemeMapFromSettings()
    {
        // Review day 037: the dispatcher path built StorecoveOptions without the map, so 0208 went out unmapped.
        var handler = new StubHandler(_ => Json(HttpStatusCode.OK, $"{{\"guid\":\"{Guid1}\"}}"));
        var settings = new AccessPointSettings("storecove", "k", "1", SchemeMap: new Dictionary<string, string> { ["0208"] = "XX:TEST" });
        var client = AccessPointClientFactory.Create(settings, new HttpClient(handler));

        await client.SendAsync(new OutboundDocument(Xml, Buyer));

        using var body = JsonDocument.Parse(handler.Body!);
        Assert.Equal("XX:TEST", body.RootElement.GetProperty("routing").GetProperty("eIdentifiers")[0].GetProperty("scheme").GetString());
        Assert.Throws<ArgumentException>(() => AccessPointClientFactory.Create(
            settings with { SchemeMap = new Dictionary<string, string> { ["BE"] = "XX:TEST" } }, new HttpClient(handler)));
    }

    [Fact]
    public void Options_ToString_HidesApiKey() =>
        Assert.DoesNotContain("secret-key-value", new StorecoveOptions("secret-key-value", 1).ToString());

    [Fact]
    public async Task GetInbound_RawXmlWithBom_ReturnsXml()
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK, JsonSerializer.Serialize(new { guid = Guid1, original = "\uFEFF" + Xml })));

        var result = await client.GetInboundAsync(Guid1);

        Assert.True(result.Success);
        Assert.Equal(Xml, result.Value!.UblXml);
    }

    [Fact]
    public async Task GetInbound_GuidMismatch_ReturnsFailure()
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK,
            JsonSerializer.Serialize(new { guid = "00000000-0000-0000-0000-000000000001", original = Xml })));

        var result = await client.GetInboundAsync(Guid1);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Send_UnsupportedCharset_ReturnsFailureNotException()
    {
        var (client, _) = Create(_ =>
        {
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes("{}"));
            content.Headers.TryAddWithoutValidation("Content-Type", "application/json; charset=bogus-charset");
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        });

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Equal("client", Assert.Single(result.Errors).Source);
    }
}
