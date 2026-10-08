using BePeppolCommerce.Core.Inbound;
using BePeppolCommerce.Core.Integration;
using BePeppolCommerce.Core.Model;
using BePeppolCommerce.Core.Ubl;
using BePeppolCommerce.Core.Validation;

namespace BePeppolCommerce.Walkthrough;

/// <summary>
/// The v1 flow end to end, entirely against in-process fakes: build and validate the sample invoice,
/// dispatch it (and one invalid order) through <see cref="OutboundDispatcher"/>, then receive the sent
/// document through the inbound path. Nothing leaves the process and no credential is used.
/// </summary>
public static class Walkthrough
{
    public const string ValidSourceId = "order-1001";
    public const string InvalidSourceId = "order-1002";

    /// <summary>Writes each step to <paramref name="output"/>. Returns 0 when every step ended as expected, 1 otherwise.</summary>
    public static async Task<int> RunAsync(TextWriter output, string? orderJsonPath = null)
    {
        ArgumentNullException.ThrowIfNull(output);
        var ok = true;
        void Check(bool condition, string what)
        {
            if (condition) return;
            ok = false;
            output.WriteLine($"  UNEXPECTED: {what}");
        }

        var path = orderJsonPath ?? Path.Combine(AppContext.BaseDirectory, "sample-order.json");
        var order = OrderJson.Parse(await File.ReadAllTextAsync(path));
        output.WriteLine($"1. Loaded order {order.InvoiceNumber} from {Path.GetFileName(path)}: {order.Seller.Name} -> {order.Buyer.Name}, {order.Lines.Count} lines.");

        output.WriteLine("2. Building the UBL invoice and validating it (XSD + EN16931 + Peppol BIS 3.0 rules).");
        output.WriteLine("   The first validation in a process compiles the rules and takes a few seconds.");
        var validation = PeppolValidator.Validate(PeppolInvoiceBuilder.Build(order));
        output.WriteLine($"   Valid: {validation.IsValid} ({validation.Errors.Count()} errors, {validation.Warnings.Count()} warnings).");
        foreach (var f in validation.Findings)
            output.WriteLine($"   - [{f.Flag}] {f.RuleId}: {f.Message}");
        Check(validation.IsValid, "the sample invoice should be valid.");

        // Same order without a buyer reference (and no order reference, which v1 never emits): the Peppol
        // rules reject it, so it must end in the failed-document log, not at the Access Point.
        var invalid = order with { InvoiceNumber = order.InvoiceNumber + "-NOREF", BuyerReference = null };

        output.WriteLine($"3. Queuing {ValidSourceId} (the sample) and {InvalidSourceId} (same order, BuyerReference removed), then running the dispatcher once.");
        var source = new InMemoryOrderInvoiceSource();
        source.Enqueue(ValidSourceId, order);
        source.Enqueue(InvalidSourceId, invalid);
        var accessPoint = new FakeAccessPoint();
        var failures = new InMemoryFailedDocumentLog();
        var dispatcher = new OutboundDispatcher(source,
            new InMemoryAccessPointSettingsProvider(new AccessPointSettings("fake", "not-a-real-key", "walkthrough")),
            _ => accessPoint, failures);
        var summary = await dispatcher.RunOnceAsync();
        output.WriteLine($"   Sent: {summary.Sent}, failed permanently: {summary.FailedPermanent}, failed retryable: {summary.FailedRetryable}.");
        Check(summary is { Configured: true, Sent: 1, FailedPermanent: 1, FailedRetryable: 0 }, "expected one sent and one permanent failure.");

        source.Sent.TryGetValue(ValidSourceId, out var submissionId);
        output.WriteLine($"   {ValidSourceId} accepted by the fake Access Point as submission {submissionId ?? "(none)"}.");
        foreach (var entry in failures.Entries)
        {
            output.WriteLine($"   Failed-document log: {entry.Direction} {entry.DocumentId}: {entry.Reason} (retryable: {entry.Retryable})");
            foreach (var detail in entry.Details)
                output.WriteLine($"     - {detail}");
        }
        Check(failures.Entries.Count == 1 && failures.Entries[0].DocumentId == InvalidSourceId,
            $"{InvalidSourceId} should be the only entry in the failed-document log.");
        Check(submissionId is not null, $"{ValidSourceId} should have a submission id.");
        if (submissionId is null)
            return 1;

        output.WriteLine($"4. Receiving: fetching document {submissionId} from the fake Access Point and parsing it, as POST /webhooks/inbound does.");
        var fetched = await accessPoint.GetInboundAsync(submissionId);
        var parsed = fetched.Success ? InboundInvoiceParser.Parse(fetched.Value!.UblXml) : InboundParseResult.Fail("fetch failed");
        if (parsed.Invoice is { } inv)
        {
            output.WriteLine($"   Invoice {inv.InvoiceNumber}, issued {inv.IssueDate:yyyy-MM-dd}, {inv.LineCount} lines, payable {inv.PayableAmount} {inv.Currency}.");
            output.WriteLine($"   Seller: {inv.Seller.Name} ({inv.Seller.Endpoint}); buyer: {inv.Buyer.Name} ({inv.Buyer.Endpoint}).");
        }
        else
        {
            output.WriteLine($"   Parse failed: {parsed.Error}");
        }
        Check(parsed.Invoice?.InvoiceNumber == order.InvoiceNumber, "the received invoice should round-trip the invoice number.");

        output.WriteLine(ok ? "Walkthrough finished: every step ended as expected." : "Walkthrough finished with unexpected results (see above).");
        return ok ? 0 : 1;
    }
}
