using System.Net;
using System.Text;
using System.Text.Json;
using BePeppolCommerce.Core.AccessPoint;

namespace BePeppolCommerce.Core.Tests;

public class RecommandClientTests
{
    private const string Xml = "<Invoice xmlns=\"urn:oasis:names:specification:ubl:schema:xsd:Invoice-2\"/>";
    private const string DocId = "doc_01JQZ8X0M4T7RB6K9V2NDHW3PA";
    private const string CompanyId = "c_01JQZ8X0M4T7RB6K9V2NDHW3PA";
    private static readonly PeppolParticipant Buyer = new("0208", "0123456789");

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string? Body { get; private set; }
        public int Calls { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Calls++;
            Request = request;
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(ct);
            return respond(request);
        }
    }

    private static HttpResponseMessage Json(HttpStatusCode code, string json) =>
        new(code) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    private static RecommandOptions Options(Uri? baseUri = null) =>
        new("test-key-placeholder", "test-secret-placeholder", CompanyId, baseUri);

    private static (RecommandClient, StubHandler) Create(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        var handler = new StubHandler(respond);
        return (new RecommandClient(new HttpClient(handler), Options()), handler);
    }

    private static string GetBody(string direction = "incoming", string? xml = Xml, string id = DocId, string companyId = CompanyId) =>
        JsonSerializer.Serialize(new { success = true, document = new { id, companyId, direction, xml } });

    [Fact]
    public async Task Send_Success_PostsRawXmlWithBasicAuthAndReturnsId()
    {
        var (client, handler) = Create(_ => Json(HttpStatusCode.OK,
            $$"""{"success":true,"sentOverPeppol":true,"sentOverEmail":false,"id":"{{DocId}}","deliveryStatus":"pending"}"""));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer, Guid.NewGuid()));

        Assert.True(result.Success);
        Assert.Equal(DocId, result.Value);
        Assert.Equal(200, result.HttpStatus);
        Assert.Equal(HttpMethod.Post, handler.Request!.Method);
        Assert.Equal($"https://app.recommand.eu/api/v1/{CompanyId}/send", handler.Request.RequestUri!.ToString());
        Assert.Equal("Basic", handler.Request.Headers.Authorization!.Scheme);
        Assert.Equal("test-key-placeholder:test-secret-placeholder",
            Encoding.UTF8.GetString(Convert.FromBase64String(handler.Request.Headers.Authorization.Parameter!)));

        using var body = JsonDocument.Parse(handler.Body!);
        var root = body.RootElement;
        Assert.Equal("0208:0123456789", root.GetProperty("recipient").GetString());
        Assert.Equal("xml", root.GetProperty("documentType").GetString());
        Assert.Equal(Xml, root.GetProperty("document").GetString());
        // The spec has no idempotency field on send; nothing else may be invented into the body.
        Assert.Equal(3, root.EnumerateObject().Count());
    }

    [Fact]
    public async Task Send_ValidationError_ReturnsFieldErrors()
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.BadRequest,
            """{"success":false,"errors":{"recipient":["Invalid Peppol address"],"document":["Not valid XML","Missing root"]}}"""));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Equal(400, result.HttpStatus);
        Assert.Equal(
            [new("recipient", "Invalid Peppol address"), new("document", "Not valid XML"), new("document", "Missing root")],
            result.Errors);
    }

    [Fact]
    public async Task Send_PeppolDeliveryFailure_KeepsTheCategory()
    {
        // Shape from utils/pipelines/sending/index.ts in the Recommand repo: errors plus deliveryFailure.
        var (client, _) = Create(_ => Json(HttpStatusCode.UnprocessableEntity,
            """{"success":false,"errors":{"root":["Failed to send document over Peppol network."]},"deliveryFailure":{"channel":"peppol","category":"transport"}}"""));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.Equal(422, result.HttpStatus);
        Assert.Equal(
            [new("root", "Failed to send document over Peppol network."), new(RecommandClient.DeliveryFailureSource, "transport")],
            result.Errors);
    }

    [Theory]
    [InlineData(HttpStatusCode.UnprocessableEntity, "")]
    [InlineData(HttpStatusCode.Unauthorized, "not json")]
    [InlineData(HttpStatusCode.InternalServerError, """{"success":false}""")]
    [InlineData(HttpStatusCode.BadGateway, """{"success":false,"errors":{}}""")]
    public async Task Send_ErrorWithoutUsableBody_FallsBackToStatus(HttpStatusCode code, string body)
    {
        var (client, _) = Create(_ => Json(code, body));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Equal((int)code, result.HttpStatus);
        Assert.Equal("http", Assert.Single(result.Errors).Source);
    }

    [Theory]
    [InlineData("""{"success":true}""")]
    [InlineData("""{"success":false,"id":"doc_1"}""")]
    [InlineData("""{"id":"doc_1"}""")]
    [InlineData("""{"success":true,"id":""}""")]
    [InlineData("not json")]
    public async Task Send_SuccessStatusWithUnusableBody_IsAFailure(string body)
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK, body));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Equal(200, result.HttpStatus);
        Assert.Equal("client", Assert.Single(result.Errors).Source);
    }

    [Fact]
    public async Task Send_DeliveredByEmailInsteadOfPeppol_IsAFailure()
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK,
            $$"""{"success":true,"sentOverPeppol":false,"sentOverEmail":true,"id":"{{DocId}}"}"""));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Equal(200, result.HttpStatus);
        var error = Assert.Single(result.Errors);
        Assert.Equal("provider", error.Source);
        Assert.Contains(DocId, error.Details);
    }

    [Fact]
    public async Task Send_TransportError_IsAFailureNotAnException()
    {
        var (client, _) = Create(_ => throw new HttpRequestException("connection refused"));

        var result = await client.SendAsync(new OutboundDocument(Xml, Buyer));

        Assert.False(result.Success);
        Assert.Null(result.HttpStatus);
        Assert.Equal(new AccessPointError("transport", "connection refused"), Assert.Single(result.Errors));
    }

    [Fact]
    public async Task Send_Timeout_IsAFailure_ButCallerCancellationThrows()
    {
        var (client, _) = Create(_ => throw new TaskCanceledException("timeout"));
        var timedOut = await client.SendAsync(new OutboundDocument(Xml, Buyer));
        Assert.Equal("transport", Assert.Single(timedOut.Errors).Source);

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var (cancelled, _) = Create(_ => throw new TaskCanceledException());
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => cancelled.SendAsync(new OutboundDocument(Xml, Buyer), cts.Token));
    }

    [Fact]
    public async Task GetInbound_Success_ReturnsXml()
    {
        var (client, handler) = Create(_ => Json(HttpStatusCode.OK, GetBody(xml: "﻿  " + Xml)));

        var result = await client.GetInboundAsync(DocId);

        Assert.True(result.Success);
        Assert.Equal(new InboundDocument(DocId, Xml), result.Value);
        Assert.Equal(HttpMethod.Get, handler.Request!.Method);
        Assert.Equal($"https://app.recommand.eu/api/v1/documents/{DocId}", handler.Request.RequestUri!.ToString());
        Assert.Equal("Basic", handler.Request.Headers.Authorization!.Scheme);
    }

    [Fact]
    public async Task GetInbound_NotFound_ReturnsProviderError()
    {
        // Shape observed from Recommand's API for an unknown route on day 033.
        var (client, _) = Create(_ => Json(HttpStatusCode.NotFound, """{"success":false,"errors":{"root":["Not found"]}}"""));

        var result = await client.GetInboundAsync(DocId);

        Assert.False(result.Success);
        Assert.Equal(404, result.HttpStatus);
        Assert.Equal(new AccessPointError("root", "Not found"), Assert.Single(result.Errors));
    }

    [Theory]
    [InlineData("outgoing", Xml, DocId)]
    [InlineData("incoming", null, DocId)]
    [InlineData("incoming", "", DocId)]
    [InlineData("incoming", "not xml", DocId)]
    [InlineData("incoming", Xml, "doc_other")]
    [InlineData("incoming", Xml, DocId, "c_another_company")]
    public async Task GetInbound_UnusableDocument_IsAFailure(string direction, string? xml, string id, string companyId = CompanyId)
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK, GetBody(direction, xml, id, companyId)));

        var result = await client.GetInboundAsync(DocId);

        Assert.False(result.Success);
        Assert.Equal("client", Assert.Single(result.Errors).Source);
    }

    [Theory]
    [InlineData("..")]
    [InlineData("doc/../../companies")]
    [InlineData("doc_1?x=1")]
    [InlineData("doc_1\n")]
    [InlineData("doc 1")]
    public async Task GetInbound_IdOutsideTheExpectedFormat_IsRefusedWithoutACall(string id)
    {
        var (client, handler) = Create(_ => Json(HttpStatusCode.OK, GetBody()));

        var result = await client.GetInboundAsync(id);

        Assert.False(result.Success);
        Assert.Equal(0, handler.Calls);
    }

    [Fact]
    public async Task NullArguments_Throw()
    {
        var (client, _) = Create(_ => Json(HttpStatusCode.OK, "{}"));
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.SendAsync(null!));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => client.GetInboundAsync(" "));
    }

    [Theory]
    [InlineData("", "s", CompanyId, null)]
    [InlineData("k", "", CompanyId, null)]
    [InlineData("k:x", "s", CompanyId, null)]
    [InlineData("k", "s", "", null)]
    [InlineData("k", "s", "c_1/../x", null)]
    [InlineData("k", "s", CompanyId, "http://api.example.invalid/")]
    [InlineData("k", "s", CompanyId, "ftp://localhost/")]
    public void Constructor_RejectsBadOptions(string key, string secret, string companyId, string? baseUri)
    {
        var options = new RecommandOptions(key, secret, companyId, baseUri is null ? null : new Uri(baseUri));
        Assert.Throws<ArgumentException>(() => new RecommandClient(new HttpClient(), options));
    }

    [Theory]
    [InlineData("", "secret-placeholder", CompanyId, null, "ApiKey")]
    [InlineData("k:x", "secret-placeholder", CompanyId, null, "ApiKey")]
    [InlineData("k", " ", CompanyId, null, "ApiSecret")]
    [InlineData("k", "secret-placeholder", "c_1/../x", null, "CompanyId")]
    [InlineData("k", "secret-placeholder", CompanyId, "http://api.example.invalid/", "BaseUri")]
    public void Validate_NamesTheBadSettingWithoutItsValue(string key, string secret, string companyId, string? baseUri, string setting)
    {
        var problems = RecommandOptions.Validate(new RecommandOptions(key, secret, companyId, baseUri is null ? null : new Uri(baseUri)));

        var problem = Assert.Single(problems);
        Assert.StartsWith(setting, problem);
        Assert.DoesNotContain("secret-placeholder", problem);
        Assert.DoesNotContain("../x", problem);
    }

    [Fact]
    public void Validate_ReportsEveryProblem()
    {
        Assert.Equal(3, RecommandOptions.Validate(new RecommandOptions("", "", "")).Count);
        Assert.Empty(RecommandOptions.Validate(Options()));
    }

    [Fact]
    public async Task LoopbackBaseUriWithoutTrailingSlash_IsAllowedForTests()
    {
        var handler = new StubHandler(_ => Json(HttpStatusCode.OK, GetBody()));
        var client = new RecommandClient(new HttpClient(handler), Options(new Uri("http://127.0.0.1:5099/base")));

        await client.GetInboundAsync(DocId);

        Assert.Equal($"http://127.0.0.1:5099/base/api/v1/documents/{DocId}", handler.Request!.RequestUri!.ToString());
    }

    [Fact]
    public void Options_ToString_HidesKeyAndSecret()
    {
        var text = Options().ToString();
        Assert.DoesNotContain("test-key-placeholder", text);
        Assert.DoesNotContain("test-secret-placeholder", text);
        Assert.Contains(CompanyId, text);
    }
}
