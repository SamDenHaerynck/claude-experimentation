using BePeppolCommerce.Walkthrough;

namespace BePeppolCommerce.Core.Tests;

/// <summary>Slice 10: the documented walkthrough runs end to end against fakes and reports every step as expected.</summary>
public class WalkthroughTests
{
    [Fact]
    public async Task Walkthrough_runs_end_to_end_against_fakes()
    {
        var output = new StringWriter();

        var exitCode = await Walkthrough.Walkthrough.RunAsync(output,
            Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample-order.json"));

        var text = output.ToString();
        Assert.True(exitCode == 0, text);
        Assert.Contains("Valid: True", text);
        Assert.Contains("Sent: 1, failed permanently: 1, failed retryable: 0.", text);
        Assert.Contains($"Outbound {Walkthrough.Walkthrough.InvalidSourceId}: Validation failed", text);
        Assert.Contains("PEPPOL-EN16931-R003", text);
        Assert.Contains("Invoice INV-2026-0001", text);
        Assert.DoesNotContain("UNEXPECTED", text);
        Assert.DoesNotContain("not-a-real-key", text);
    }
}
