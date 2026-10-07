using System.Collections.Concurrent;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Integration;
using BePeppolCommerce.Core.Model;
using Microsoft.Extensions.Logging;

namespace BePeppolCommerce.Core.Tests;

/// <summary>
/// Slice 9: every outbound failure path is marked on the source, written to the failed-document log and
/// logged with its stable event id, and none of them carries the XML or a credential.
/// </summary>
public class FailureRecordingTests
{
    private const string ApiKey = "test-key-not-real";
    private static readonly DateTimeOffset Now = new(2026, 10, 7, 9, 30, 0, TimeSpan.Zero);

    private static Order LoadSample() =>
        OrderJson.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json")));

    private sealed class FixedTime : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    public sealed record LogEntry(LogLevel Level, EventId EventId, string Message, Exception? Exception);

    public sealed class CapturingLogger<T> : ILogger<T>
    {
        public ConcurrentQueue<LogEntry> Entries { get; } = new();
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Entries.Enqueue(new LogEntry(logLevel, eventId, formatter(state, exception), exception));
    }

    private sealed class Client(AccessPointResult<string>? reply = null, Exception? throws = null) : IPeppolAccessPointClient
    {
        public Task<AccessPointResult<string>> SendAsync(OutboundDocument document, CancellationToken cancellationToken = default) =>
            throws is not null ? throw throws : Task.FromResult(reply ?? AccessPointResult<string>.Ok("sub-1", 200));

        public Task<AccessPointResult<InboundDocument>> GetInboundAsync(string providerDocumentId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class ThrowingLog : IFailedDocumentLog
    {
        public Task RecordAsync(FailedDocument failure, CancellationToken cancellationToken = default) =>
            throw new IOException("disk full");
    }

    private static async Task<(InMemoryOrderInvoiceSource Source, InMemoryFailedDocumentLog Failures, CapturingLogger<OutboundDispatcher> Logger)> Run(
        IPeppolAccessPointClient client, Order? order = null, string provider = "storecove", IFailedDocumentLog? failures = null)
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", order ?? LoadSample());
        var log = new InMemoryFailedDocumentLog();
        var logger = new CapturingLogger<OutboundDispatcher>();
        var settings = new AccessPointSettings(provider, ApiKey, "42");
        await new OutboundDispatcher(source, new InMemoryAccessPointSettingsProvider(settings), _ => client,
            failures ?? log, logger, new FixedTime()).RunOnceAsync();
        return (source, log, logger);
    }

    private static void AssertRecordedAndLogged(InMemoryOrderInvoiceSource source, InMemoryFailedDocumentLog failures,
        CapturingLogger<OutboundDispatcher> logger, EventId eventId, string reason, bool retryable, LogLevel level)
    {
        var marked = source.Failed["ORD-1"];
        Assert.Equal(reason, marked.Reason);
        Assert.Equal(retryable, marked.Retryable);

        var record = Assert.Single(failures.Entries);
        Assert.Equal(new FailedDocument(Now, DocumentDirection.Outbound, "ORD-1", reason, retryable, marked.Details), record with { Details = marked.Details });
        Assert.Equal(marked.Details, record.Details);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(eventId, entry.EventId);
        Assert.Equal(level, entry.Level);
        Assert.Contains("ORD-1", entry.Message);
        Assert.Contains(reason, entry.Message);

        foreach (var text in record.Details.Append(entry.Message))
        {
            Assert.DoesNotContain("<?xml", text);
            Assert.DoesNotContain("<Invoice", text);
            Assert.DoesNotContain(ApiKey, text);
        }
    }

    [Fact]
    public async Task Validation_failure_is_recorded_and_logged()
    {
        var (source, failures, logger) = await Run(new Client(), LoadSample() with { BuyerReference = null });
        AssertRecordedAndLogged(source, failures, logger, PeppolLogEvents.OutboundValidationFailed, "Validation failed", false, LogLevel.Error);
        Assert.NotEmpty(failures.Entries[0].Details);
    }

    [Fact]
    public async Task Retryable_send_failure_is_recorded_and_logged()
    {
        var (source, failures, logger) = await Run(new Client(AccessPointResult<string>.Fail(503, new AccessPointError("http", "Service Unavailable"))));
        AssertRecordedAndLogged(source, failures, logger, PeppolLogEvents.OutboundSendFailedRetryable, "Access Point unavailable", true, LogLevel.Warning);
        Assert.Equal(["http: Service Unavailable"], failures.Entries[0].Details);
    }

    [Fact]
    public async Task Permanent_send_failure_is_recorded_and_logged()
    {
        var (source, failures, logger) = await Run(new Client(AccessPointResult<string>.Fail(400, new AccessPointError("provider", "bad vat"))));
        AssertRecordedAndLogged(source, failures, logger, PeppolLogEvents.OutboundSendFailedPermanent, "Rejected by Access Point", false, LogLevel.Error);
    }

    [Fact]
    public async Task Client_timeout_is_recorded_and_logged_as_retryable()
    {
        var (source, failures, logger) = await Run(new Client(throws: new TaskCanceledException("timeout")));
        AssertRecordedAndLogged(source, failures, logger, PeppolLogEvents.OutboundSendFailedRetryable, "Access Point unavailable", true, LogLevel.Warning);
    }

    [Fact]
    public async Task Unexpected_exception_is_recorded_and_logged_as_an_error_with_the_exception()
    {
        var boom = new InvalidOperationException("boom");
        var (source, failures, logger) = await Run(new Client(throws: boom));
        AssertRecordedAndLogged(source, failures, logger, PeppolLogEvents.OutboundUnexpectedError, "Unexpected error", true, LogLevel.Error);
        Assert.Same(boom, logger.Entries.Single().Exception);
    }

    [Theory]
    [InlineData("transport", "Peppol delivery failed, retry later", true)]
    [InlineData("recipient_not_found", "Recipient not reachable on Peppol", false)]
    [InlineData("document_not_supported", "Recipient not reachable on Peppol", false)]
    [InlineData("validation", "Rejected by Access Point", false)]
    [InlineData("recipient_rejected", "Rejected by Access Point", false)]
    [InlineData(null, "Rejected by Access Point", false)]
    public async Task Recommand_422_is_classified_by_its_delivery_failure_category(string? category, string reason, bool retryable)
    {
        AccessPointError[] errors = category is null
            ? [new("root", "Failed to send document over Peppol network.")]
            : [new("root", "Failed to send document over Peppol network."), new(RecommandClient.DeliveryFailureSource, category)];
        var reply = AccessPointResult<string>.Fail(422, errors);
        var (source, failures, logger) = await Run(new Client(reply), provider: "recommand");
        AssertRecordedAndLogged(source, failures, logger,
            retryable ? PeppolLogEvents.OutboundSendFailedRetryable : PeppolLogEvents.OutboundSendFailedPermanent,
            reason, retryable, retryable ? LogLevel.Warning : LogLevel.Error);

        // Other providers keep the generic classification: Storecove's 422 meaning is not documented.
        var (storecove, _, _) = await Run(new Client(reply), provider: "storecove");
        Assert.Equal("Rejected by Access Point", storecove.Failed["ORD-1"].Reason);
    }

    private sealed class TimingOutLog : IFailedDocumentLog
    {
        public Task RecordAsync(FailedDocument failure, CancellationToken cancellationToken = default) =>
            throw new TaskCanceledException("database timeout");
    }

    [Fact]
    public async Task A_failure_log_that_times_out_does_not_stop_the_run()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample() with { BuyerReference = null });
        source.Enqueue("ORD-2", LoadSample() with { InvoiceNumber = "INV-2" });
        var logger = new CapturingLogger<OutboundDispatcher>();
        var dispatcher = new OutboundDispatcher(source, new InMemoryAccessPointSettingsProvider(new AccessPointSettings("storecove", ApiKey, "42")),
            _ => new Client(), new TimingOutLog(), logger, new FixedTime());

        var summary = await dispatcher.RunOnceAsync();

        Assert.Equal(new DispatchSummary(true, 1, 0, 1), summary);
        Assert.Contains(logger.Entries, e => e.EventId == PeppolLogEvents.FailureRecordFailed);
    }

    [Fact]
    public async Task A_broken_failure_log_is_logged_and_does_not_stop_the_source_being_marked()
    {
        var (source, _, logger) = await Run(new Client(AccessPointResult<string>.Fail(400, new AccessPointError("provider", "bad"))),
            failures: new ThrowingLog());

        Assert.False(source.Failed["ORD-1"].Retryable);
        Assert.Equal([PeppolLogEvents.FailureRecordFailed, PeppolLogEvents.OutboundSendFailedPermanent],
            logger.Entries.Select(e => e.EventId).ToArray());
        Assert.IsType<IOException>(logger.Entries.First().Exception);
    }

    [Fact]
    public async Task Successful_send_records_nothing_and_logs_one_information_event()
    {
        var (source, failures, logger) = await Run(new Client());
        Assert.Equal("sub-1", source.Sent["ORD-1"]);
        Assert.Empty(failures.Entries);
        var entry = Assert.Single(logger.Entries);
        Assert.Equal((PeppolLogEvents.OutboundSent, LogLevel.Information), (entry.EventId, entry.Level));
    }

    [Fact]
    public async Task In_memory_log_keeps_only_the_newest_entries()
    {
        var log = new InMemoryFailedDocumentLog(capacity: 2);
        foreach (var id in new[] { "A", "B", "C" })
            await log.RecordAsync(new FailedDocument(Now, DocumentDirection.Inbound, id, "r", false, []));
        Assert.Equal(["B", "C"], log.Entries.Select(e => e.DocumentId).ToArray());
        Assert.Throws<ArgumentOutOfRangeException>(() => new InMemoryFailedDocumentLog(0));
    }
}
