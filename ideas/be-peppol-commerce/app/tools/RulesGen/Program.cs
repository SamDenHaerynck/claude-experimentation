// Regenerates a committed rules file. See src/BePeppolCommerce.Core/Validation/Rules/SOURCE.md.
// Usage: dotnet run --project tools/RulesGen -- <skeleton-code-dir> <rules.sch> <out.xslt>
using BePeppolCommerce.Core.Validation;

if (args.Length != 3)
{
    Console.Error.WriteLine("usage: RulesGen <skeleton-code-dir> <rules.sch> <out.xslt>");
    return 2;
}

File.WriteAllText(args[2], SchematronCompiler.CompileToXslt(args[0], args[1]));
Console.WriteLine($"wrote {args[2]}");
return 0;
