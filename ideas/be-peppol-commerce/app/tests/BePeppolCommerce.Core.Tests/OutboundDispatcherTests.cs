using System.Collections.Concurrent;
using BePeppolCommerce.Core.AccessPoint;
using BePeppolCommerce.Core.Integration;
using BePeppolCommerce.Core.Model;
using BePeppolCommerce.Core.Outbound;

namespace BePeppolCommerce.Core.Tests;

/// <summary>
/// The Slice 6 integration contract, driven through the in-memory fakes: queue an order, run the
/// dispatcher, check what the Access Point received and what the source recorded.
/// </summary>
public class OutboundDispatcherTests
{
    private static readonly AccessPointSettings Settings = new("storecove", "test-key-not-real", "42");

    private static Order LoadSample() =>
        OrderJson.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json")));

    /// <summary>An Access Point client that records every document and answers from a queue of canned results.</summary>
    private sealed class ScriptedClient(params AccessPointResult<string>[] replies) : IPeppolAccessPointClient
    {
        private readonly Queue<AccessPointResult<string>> _replies = new(replies);
        public ConcurrentQueue<OutboundDocument> Received { get; } = new();
        public Exception? Throw { get; init; }
        public Action? BeforeSend { get; init; }

        public Task<AccessPointResult<string>> SendAsync(OutboundDocument document, CancellationToken cancellationToken = default)
        {
            BeforeSend?.Invoke();
            if (Throw is not null) throw Throw;
            Received.Enqueue(document);
            return Task.FromResult(_replies.Count > 0 ? _replies.Dequeue() : AccessPointResult<string>.Ok("sub-" + Received.Count, 200));
        }

        public Task<AccessPointResult<InboundDocument>> GetInboundAsync(string providerDocumentId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private static (OutboundDispatcher Dispatcher, List<AccessPointSettings> FactoryCalls) Create(
        InMemoryOrderInvoiceSource source, IPeppolAccessPointClient client, AccessPointSettings? settings = null)
    {
        var calls = new List<AccessPointSettings>();
        var dispatcher = new OutboundDispatcher(source, new InMemoryAccessPointSettingsProvider(settings),
            s => { calls.Add(s); return client; });
        return (dispatcher, calls);
    }

    [Fact]
    public async Task Not_configured_sends_nothing_and_leaves_the_queue_alone()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        var client = new ScriptedClient();
        var (dispatcher, calls) = Create(source, client, settings: null);

        var summary = await dispatcher.RunOnceAsync();

        Assert.Equal(new DispatchSummary(false, 0, 0, 0), summary);
        Assert.Empty(calls);
        Assert.Single(await source.GetPendingAsync(10));
    }

    [Fact]
    public async Task Valid_order_is_sent_with_the_key_fixed_at_enqueue_and_marked_sent()
    {
        var source = new InMemoryOrderInvoiceSource();
        var queued = source.Enqueue("ORD-1", LoadSample());
        var client = new ScriptedClient(AccessPointResult<string>.Ok("sub-123", 200));
        var (dispatcher, calls) = Create(source, client, Settings);

        var summary = await dispatcher.RunOnceAsync();

        Assert.Equal(new DispatchSummary(true, 1, 0, 0), summary);
        Assert.Equal(Settings, Assert.Single(calls));
        var doc = Assert.Single(client.Received);
        Assert.Equal(queued.IdempotencyKey, doc.IdempotencyKey);
        Assert.StartsWith("<?xml", doc.UblXml);
        Assert.Equal("sub-123", source.Sent["ORD-1"]);
        Assert.Empty(await source.GetPendingAsync(10));
    }

    [Fact]
    public async Task Invalid_order_is_parked_without_reaching_the_access_point()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample() with { BuyerReference = null });
        var client = new ScriptedClient();
        var (dispatcher, _) = Create(source, client, Settings);

        var summary = await dispatcher.RunOnceAsync();

        Assert.Equal(new DispatchSummary(true, 0, 0, 1), summary);
        Assert.Empty(client.Received);
        var failure = source.Failed["ORD-1"];
        Assert.False(failure.Retryable);
        Assert.Equal("Validation failed", failure.Reason);
        Assert.Contains(failure.Details, d => d.StartsWith("PEPPOL-EN16931-R003"));
        Assert.Empty(await source.GetPendingAsync(10));
    }

    [Fact]
    public async Task Order_without_lines_is_parked_as_a_validation_failure()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample() with { Lines = [] });
        var (dispatcher, _) = Create(source, new ScriptedClient(), Settings);

        Assert.Equal(new DispatchSummary(true, 0, 0, 1), await dispatcher.RunOnceAsync());
        Assert.Contains(source.Failed["ORD-1"].Details, d => d.StartsWith(OutboundInvoiceSender.BuildRuleId));
    }

    [Fact]
    public async Task Provider_outage_stays_queued_and_the_retry_reuses_the_same_key()
    {
        var source = new InMemoryOrderInvoiceSource();
        var queued = source.Enqueue("ORD-1", LoadSample());
        var client = new ScriptedClient(
            AccessPointResult<string>.Fail(503, new AccessPointError("http", "Service Unavailable")),
            AccessPointResult<string>.Ok("sub-9", 200));
        var (dispatcher, _) = Create(source, client, Settings);

        Assert.Equal(new DispatchSummary(true, 0, 1, 0), await dispatcher.RunOnceAsync());
        Assert.True(source.Failed["ORD-1"].Retryable);
        Assert.Single(await source.GetPendingAsync(10));

        Assert.Equal(new DispatchSummary(true, 1, 0, 0), await dispatcher.RunOnceAsync());
        Assert.Equal("sub-9", source.Sent["ORD-1"]);
        Assert.False(source.Failed.ContainsKey("ORD-1"));
        Assert.All(client.Received, d => Assert.Equal(queued.IdempotencyKey, d.IdempotencyKey));
        Assert.Equal(2, client.Received.Count);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(408, true)]
    [InlineData(429, true)]
    [InlineData(500, true)]
    [InlineData(502, true)]
    [InlineData(202, false)]
    [InlineData(401, true)]
    [InlineData(403, true)]
    [InlineData(404, true)]
    [InlineData(400, false)]
    [InlineData(409, false)]
    [InlineData(422, false)]
    public async Task Send_failures_are_classified_by_http_status(int? status, bool retryable)
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        var client = new ScriptedClient(AccessPointResult<string>.Fail(status, new AccessPointError("provider", "nope")));
        var (dispatcher, _) = Create(source, client, Settings);

        await dispatcher.RunOnceAsync();

        var failure = source.Failed["ORD-1"];
        Assert.Equal(retryable, failure.Retryable);
        Assert.Equal(status switch
        {
            202 => "Accepted by Access Point, response unreadable",
            401 or 403 or 404 => "Access Point configuration error",
            _ => retryable ? "Access Point unavailable" : "Rejected by Access Point",
        }, failure.Reason);
        Assert.Equal(["provider: nope"], failure.Details);
        Assert.Equal(retryable ? 1 : 0, (await source.GetPendingAsync(10)).Count);
    }

    [Fact]
    public async Task Unexpected_exception_keeps_that_invoice_queued_and_stops_the_run()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        source.Enqueue("ORD-2", LoadSample() with { InvoiceNumber = "INV-2" });
        var client = new ScriptedClient { Throw = new InvalidOperationException("boom") };
        var (dispatcher, _) = Create(source, client, Settings);

        Assert.Equal(new DispatchSummary(true, 0, 1, 0), await dispatcher.RunOnceAsync());
        var failure = source.Failed["ORD-1"];
        Assert.True(failure.Retryable);
        Assert.Equal(["InvalidOperationException: boom"], failure.Details);
        Assert.Equal(["ORD-2", "ORD-1"], (await source.GetPendingAsync(10)).Select(p => p.SourceId));
    }

    [Fact]
    public async Task Caller_cancellation_escapes_and_records_nothing()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        using var cts = new CancellationTokenSource();
        var client = new ScriptedClient
        {
            BeforeSend = cts.Cancel,
            Throw = new OperationCanceledException(cts.Token),
        };
        var (dispatcher, _) = Create(source, client, Settings);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => dispatcher.RunOnceAsync(cancellationToken: cts.Token));
        Assert.Empty(source.Failed);
        Assert.Single(await source.GetPendingAsync(10));
    }

    [Fact]
    public async Task Client_timeout_is_a_retryable_failure_not_an_abort()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        var client = new ScriptedClient { Throw = new TaskCanceledException("HttpClient.Timeout") };
        var (dispatcher, _) = Create(source, client, Settings);

        Assert.Equal(new DispatchSummary(true, 0, 1, 0), await dispatcher.RunOnceAsync());
        Assert.True(source.Failed["ORD-1"].Retryable);
        Assert.Single(await source.GetPendingAsync(10));
    }

    [Fact]
    public async Task Outage_stops_the_run_and_the_failed_invoice_does_not_block_newer_ones()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        source.Enqueue("ORD-2", LoadSample() with { InvoiceNumber = "INV-2" });
        var client = new ScriptedClient(
            AccessPointResult<string>.Fail(503, new AccessPointError("http", "Service Unavailable")),
            AccessPointResult<string>.Ok("sub-2", 200),
            AccessPointResult<string>.Ok("sub-1", 200));
        var (dispatcher, _) = Create(source, client, Settings);

        Assert.Equal(new DispatchSummary(true, 0, 1, 0), await dispatcher.RunOnceAsync());
        Assert.Single(client.Received);
        Assert.Equal(["ORD-2", "ORD-1"], (await source.GetPendingAsync(10)).Select(p => p.SourceId));

        Assert.Equal(new DispatchSummary(true, 2, 0, 0), await dispatcher.RunOnceAsync());
        Assert.Equal("sub-2", source.Sent["ORD-2"]);
        Assert.Equal("sub-1", source.Sent["ORD-1"]);
    }

    [Fact]
    public async Task Orders_queued_after_a_retryable_failure_go_ahead_of_it()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        var client = new ScriptedClient(AccessPointResult<string>.Fail(503, new AccessPointError("http", "down")));
        var (dispatcher, _) = Create(source, client, Settings);
        await dispatcher.RunOnceAsync();

        source.Enqueue("ORD-2", LoadSample() with { InvoiceNumber = "INV-2" });
        source.Enqueue("ORD-3", LoadSample() with { InvoiceNumber = "INV-3" });

        Assert.Equal(["ORD-2", "ORD-3", "ORD-1"], (await source.GetPendingAsync(10)).Select(p => p.SourceId));

        // Once ORD-1 leaves the queue, new orders go to the back again (the retrying set stays in step).
        await source.MarkFailedAsync("ORD-1", new DispatchFailure("Rejected by Access Point", false, []));
        source.Enqueue("ORD-4", LoadSample() with { InvoiceNumber = "INV-4" });
        Assert.Equal(["ORD-2", "ORD-3", "ORD-4"], (await source.GetPendingAsync(10)).Select(p => p.SourceId));
    }

    [Fact]
    public async Task Caller_cancellation_after_the_provider_accepted_still_records_the_send()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        source.Enqueue("ORD-2", LoadSample() with { InvoiceNumber = "INV-2" });
        using var cts = new CancellationTokenSource();
        var client = new ScriptedClient { BeforeSend = cts.Cancel };
        var (dispatcher, _) = Create(source, client, Settings);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => dispatcher.RunOnceAsync(cancellationToken: cts.Token));
        Assert.Equal("sub-1", source.Sent["ORD-1"]);
        Assert.Equal("ORD-2", Assert.Single(await source.GetPendingAsync(10)).SourceId);
    }

    [Fact]
    public async Task Batch_size_limits_one_run()
    {
        var source = new InMemoryOrderInvoiceSource();
        for (var i = 1; i <= 3; i++)
            source.Enqueue($"ORD-{i}", LoadSample() with { InvoiceNumber = $"INV-{i}" });
        var client = new ScriptedClient();
        var (dispatcher, _) = Create(source, client, Settings);

        Assert.Equal(2, (await dispatcher.RunOnceAsync(batchSize: 2)).Sent);
        Assert.Equal("ORD-3", Assert.Single(await source.GetPendingAsync(10)).SourceId);
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => dispatcher.RunOnceAsync(batchSize: 0));
    }

    [Fact]
    public void Factory_builds_a_storecove_client_and_rejects_bad_settings()
    {
        using var http = new HttpClient();
        Assert.IsType<StorecoveClient>(AccessPointClientFactory.Create(Settings with { Provider = "Storecove" }, http));
        Assert.Throws<ArgumentException>(() => AccessPointClientFactory.Create(Settings with { Provider = "billit" }, http));
        Assert.Throws<ArgumentException>(() => AccessPointClientFactory.Create(Settings with { AccountId = "abc" }, http));
        Assert.Throws<ArgumentException>(() => AccessPointClientFactory.Create(Settings with { AccountId = "0" }, http));
        Assert.Throws<ArgumentException>(() => AccessPointClientFactory.Create(Settings with { ApiKey = " " }, http));
    }

    [Fact]
    public void Settings_ToString_hides_the_api_key()
    {
        Assert.DoesNotContain("test-key-not-real", Settings.ToString());
        Assert.Contains("storecove", Settings.ToString());
    }

    [Fact]
    public async Task Fake_source_enforces_its_contract()
    {
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue("ORD-1", LoadSample());
        Assert.Throws<InvalidOperationException>(() => source.Enqueue("ORD-1", LoadSample()));
        await source.MarkSentAsync("ORD-1", "sub-1");
        Assert.Throws<InvalidOperationException>(() => source.Enqueue("ORD-1", LoadSample()));
        await Assert.ThrowsAsync<InvalidOperationException>(() => source.MarkSentAsync("ORD-404", "sub"));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            source.MarkFailedAsync("ORD-404", new DispatchFailure("x", true, [])));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => source.GetPendingAsync(0));
    }
}
