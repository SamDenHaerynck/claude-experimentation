using System.Collections.Concurrent;
using System.Net;
using System.Text;
using BePeppolCommerce.Api;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Integration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace BePeppolCommerce.Api.Tests;

/// <summary>
/// Slice 9, inbound side: a fetch or parse failure is both written to the host's failed-document log and
/// logged with its stable event id; a body without a document id is logged only.
/// </summary>
public class InboundFailureRecordingTests
{
    private const string DocumentGuid = "0b6f2a3c-1d4e-4f5a-8b9c-0d1e2f3a4b5c";

    private sealed record LogEntry(string Category, EventId EventId, string Message);

    private sealed class CapturingProvider : ILoggerProvider
    {
        public ConcurrentQueue<LogEntry> Entries { get; } = new();
        public ILogger CreateLogger(string categoryName) => new Logger(categoryName, Entries);
        public void Dispose() { }

        private sealed class Logger(string category, ConcurrentQueue<LogEntry> entries) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
                entries.Enqueue(new LogEntry(category, eventId, formatter(state, exception)));
        }
    }

    private sealed class FakeAccessPoint(AccessPointResult<InboundDocument> reply) : IPeppolAccessPointClient
    {
        public Task<AccessPointResult<string>> SendAsync(OutboundDocument document, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<AccessPointResult<InboundDocument>> GetInboundAsync(string providerDocumentId, CancellationToken cancellationToken = default) =>
            Task.FromResult(reply);
    }

    private static async Task<(HttpStatusCode Status, InMemoryFailedDocumentLog Failures, LogEntry[] Events)> Post(
        AccessPointResult<InboundDocument> reply, string body)
    {
        var logs = new CapturingProvider();
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.ConfigureLogging(l => l.AddProvider(logs));
            b.ConfigureTestServices(services =>
            {
                services.RemoveAll<IPeppolAccessPointClient>();
                services.AddSingleton<IPeppolAccessPointClient>(new FakeAccessPoint(reply));
            });
        });
        var response = await factory.CreateClient().PostAsync(InboundWebhook.Route, new StringContent(body, Encoding.UTF8, "application/json"));
        var failures = (InMemoryFailedDocumentLog)factory.Services.GetRequiredService<IFailedDocumentLog>();
        var events = logs.Entries.Where(e => e.Category == typeof(InboundWebhook).FullName).ToArray();
        return (response.StatusCode, failures, events);
    }

    private static string GuidBody => $$"""{ "guid": "{{DocumentGuid}}" }""";

    [Theory]
    [InlineData(503, true)]
    [InlineData(null, true)]
    [InlineData(401, false)]
    [InlineData(404, false)]
    public async Task Fetch_failure_is_recorded_and_logged(int? status, bool retryable)
    {
        var (code, failures, events) = await Post(
            AccessPointResult<InboundDocument>.Fail(status, new AccessPointError("provider", "down")), GuidBody);

        Assert.Equal(HttpStatusCode.BadGateway, code);
        var record = Assert.Single(failures.Entries);
        Assert.Equal((DocumentDirection.Inbound, DocumentGuid, "Fetch from Access Point failed", retryable),
            (record.Direction, record.DocumentId, record.Reason, record.Retryable));
        Assert.Equal(["provider: down"], record.Details);
        var entry = Assert.Single(events);
        Assert.Equal(PeppolLogEvents.InboundFetchFailed, entry.EventId);
        Assert.Contains(DocumentGuid, entry.Message);
    }

    [Fact]
    public async Task Parse_failure_is_recorded_and_logged_without_the_xml()
    {
        const string xml = "<Invoice><unclosed>";
        var (code, failures, events) = await Post(
            AccessPointResult<InboundDocument>.Ok(new InboundDocument(DocumentGuid, xml), 200), GuidBody);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, code);
        var record = Assert.Single(failures.Entries);
        Assert.Equal((DocumentDirection.Inbound, DocumentGuid, "Unparseable document", false),
            (record.Direction, record.DocumentId, record.Reason, record.Retryable));
        var entry = Assert.Single(events);
        Assert.Equal(PeppolLogEvents.InboundParseFailed, entry.EventId);
        foreach (var text in record.Details.Append(entry.Message))
            Assert.DoesNotContain(xml, text);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("not json")]
    public async Task Body_without_a_document_id_is_logged_but_not_recorded(string body)
    {
        var (code, failures, events) = await Post(AccessPointResult<InboundDocument>.Fail(500), body);

        Assert.Equal(HttpStatusCode.BadRequest, code);
        Assert.Empty(failures.Entries);
        Assert.Equal(PeppolLogEvents.InboundPayloadRejected, Assert.Single(events).EventId);
    }
}
