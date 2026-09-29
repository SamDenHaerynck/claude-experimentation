# Slice 2a spike: EN16931 / Peppol BIS 3.0 Schematron from .NET 8

Throwaway investigation code (day 027, 2026-09-29). Not part of `app/` or its solution, and not
built or tested by `dotnet test`. Kept so Slice 2b can reproduce the result. The decision it led to
is in `../../../../DECISIONS.md` (day 027 entry) and `../../PLAN.md` (Stack, Slice 2b).

## What it does

1. `gen <order.json> <out.xml>`: runs `PeppolInvoiceBuilder.Build` on an order fixture.
2. `<skeleton-dir> <rules.sch> <invoice.xml> <out-dir>`: compiles an ISO Schematron file to XSLT 2.0
   with the ISO skeleton (`iso_dsdl_include.xsl` → `iso_abstract_expand.xsl` →
   `iso_svrl_for_xslt2.xsl`), then runs it on the invoice with Saxon-HE and prints each
   `svrl:failed-assert`.
3. `xsd <UBL-Invoice-2.1.xsd> <invoice.xml>`: validates against the UBL 2.1 XSD with .NET's
   `XmlSchemaSet` (the xmldsig import needs `DtdProcessing.Parse` on the schema reader).

Saxon-HE 12.5 (Java, MPL-2.0) is turned into a .NET assembly at build time by IKVM 8.16.1
(`IkvmReference` on a local jar).

## Reproduce (Linux, .NET 8 SDK on PATH)

```
cd ideas/be-peppol-commerce/spikes/2a-schematron
mkdir -p jars out
M=https://repo1.maven.org/maven2
curl -sSLo jars/Saxon-HE-12.5.jar        $M/net/sf/saxon/Saxon-HE/12.5/Saxon-HE-12.5.jar
curl -sSLo jars/xmlresolver-5.2.2.jar    $M/org/xmlresolver/xmlresolver/5.2.2/xmlresolver-5.2.2.jar
git clone https://github.com/Schematron/schematron.git && git -C schematron checkout -q 77dcd36
git clone --depth 1 --branch v3.0.20 https://github.com/OpenPEPPOL/peppol-bis-invoice-3.git /tmp/peppol-bis-invoice-3
dotnet build                               # first build ~1 min (IKVM compiles the jar)
dotnet run --no-build -- gen ../../app/tests/BePeppolCommerce.Core.Tests/Fixtures/sample-order.json out/sample-invoice.xml
for s in CEN-EN16931-UBL PEPPOL-EN16931-UBL; do
  dotnet run --no-build -- schematron/trunk/schematron/code /tmp/peppol-bis-invoice-3/rules/sch/$s.sch out/sample-invoice.xml out
done
# negative controls and XSD
python3 - <<'PY'
import re
s=open('out/sample-invoice.xml',encoding='utf-8-sig').read()
b=re.sub(r'(<cbc:PayableAmount currencyID="EUR">)[^<]*',r'\g<1>1.00',s).replace('schemeID="0208"','schemeID="9999"',1)
b=re.sub(r'<cbc:BuyerReference>.*?</cbc:BuyerReference>','',b); open('out/broken-invoice.xml','w').write(b)
open('out/xsd-broken.xml','w').write(s.replace('<cbc:IssueDate>','<cbc:Bogus>x</cbc:Bogus><cbc:IssueDate>',1))
PY
for s in CEN-EN16931-UBL PEPPOL-EN16931-UBL; do
  dotnet run --no-build -- schematron/trunk/schematron/code /tmp/peppol-bis-invoice-3/rules/sch/$s.sch out/broken-invoice.xml out
done
curl -sSLo /tmp/ubl21.zip http://docs.oasis-open.org/ubl/os-UBL-2.1/UBL-2.1.zip && unzip -qo /tmp/ubl21.zip 'xsd/*' -d /tmp/ubl21
dotnet run --no-build -- xsd /tmp/ubl21/xsd/maindoc/UBL-Invoice-2.1.xsd out/sample-invoice.xml
dotnet run --no-build -- xsd /tmp/ubl21/xsd/maindoc/UBL-Invoice-2.1.xsd out/xsd-broken.xml
```

Maven Central rate-limited this sandbox once (HTTP 429 returned as a 96-byte text file); check
with `file jars/*` that both are real JARs. `IkvmMavenSdk`'s `MavenReference` failed here only
because its embedded Java trust store does not know the sandbox proxy's CA; it has not been tried
on a normal network.

## Results on day 027

| Input | Rule set | Failed asserts |
|---|---|---|
| Slice-1 fixture | CEN-EN16931-UBL.sch 1.3.15 (73 rules fired) | 0 |
| Slice-1 fixture | PEPPOL-EN16931-UBL.sch 3.0.20 (43 rules fired) | 0 |
| Slice-1 fixture | UBL-Invoice-2.1.xsd | 0 |
| Broken copy: PayableAmount 1.00, endpoint scheme 9999, BuyerReference removed | CEN | BR-CO-16, BR-CL-25 |
| same | PEPPOL | PEPPOL-EN16931-R003, PEPPOL-EN16931-CL008 |
| Copy with an unknown `cbc:Bogus` element | XSD | 1 error (invalid child element) |

Timing on this container: compiling the .sch to XSLT took about 2 to 3 s per file; validating one
invoice took about 0.6 to 1.3 s cold, in a new process each time. The generated XSLT is about 1.0 MB
(CEN) and 0.24 MB (PEPPOL). These timings are cold, one process per run; warm latency is not measured.
The build output (`bin/`) is 337 MB, mostly IKVM images and per-platform `runtimes/`.
