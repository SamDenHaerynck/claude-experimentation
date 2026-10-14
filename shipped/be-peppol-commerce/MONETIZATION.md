# Monetization

Nothing below has been tested with a customer. No one has been contacted: `SIGNALS.md` does not exist.

## Model

A flat monthly licence per Optimizely Configured Commerce site for the connector, paid on top of the
customer's own Access Point subscription (the customer contracts with the provider; this product
never touches that money).

**Price hypothesis: 50 to 150 EUR per site per month** (decided day 024, `VALIDATION.md`). It is a
hypothesis, not a measurement. Retrieved evidence for the pieces around it:

| Comparable | Price | Source (fetched day 024) |
|---|---|---|
| Recommand (Access Point) | free to 25 docs a month, 29 to 349 EUR a month tiered | https://www.recommand.eu/pricing |
| Billit (Access Point) | 7.50 to 250 EUR a month tiered | https://www.billit.eu/en-int/pricing/ |
| Qvalia (Access Point) | 9 to 899 EUR a month tiered | https://qvalia.com/pricing |
| Storecove (Access Point) | not public, quote only | https://www.storecove.com/us/en/solutions/peppol-access-point/ |
| Woo2Billit (WooCommerce plugin for Billit, the nearest platform-connector analogue) | no price on its documentation page (refetched day 040) | https://woo2billit.eu/documentation/ |

No page was found that prices a per-platform connector, so the 50 to 150 EUR range rests on judgement
that the connector should cost the same order of magnitude as the Access Point fee it sits beside.
Test it in the first customer conversations; a one-off integration project fee (consultancy work
through the owner's employer) is the other plausible model, since few Belgian installs will want a
subscription before the connector runs inside a real install.

Before any pricing, read the licensing note in `STATE.md`: OpenPEPPOL says it holds copyright on the
Peppol rules and that the BIS may not be redistributed without consent. The built DLL embeds two
Schematron files, so a sold package may need OpenPeppol's consent first.

## Where payment would plug in (nothing is wired)

There is no licensing or billing code in the repo. The natural seams if one is added:

- `OutboundDispatcher.RunOnceAsync` (`src/BePeppolCommerce.Core/Integration/`): check a licence here
  before draining the queue.
- `IAccessPointSettingsProvider.GetAsync`: returns null when sending is off, which already makes the
  dispatcher send nothing; an unlicensed install can return null.
- Sold as a service instead (hosted bridge), the API host would need authentication per customer
  and metering by document, which is not built.

## Legal pages needed

Terms of service; privacy policy and a data processing agreement (invoices carry company and
contact data, so the product processes personal data for the customer); a statement that the product
is software, not a certified Peppol Access Point and not tax advice; the Peppol rules licensing
position above. Have a lawyer review; the repo contains no legal text.

## First acquisition channel

The owner's own Belgian Optimizely Configured Commerce clients and colleagues. First step: send the
EN or NL outreach message in `VALIDATION_KIT.md` to the contacts at two or three clients that sell B2B
in Belgium, and ask a yes or no question: "are you ready for mandatory Peppol invoicing, and how are
you solving it?" Record answers in `SIGNALS.md`. Also ask which Configured Commerce version they run,
because installs still on `net48` cannot use v1.
