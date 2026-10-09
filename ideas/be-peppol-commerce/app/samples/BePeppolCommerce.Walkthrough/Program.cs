using BePeppolCommerce.Walkthrough;

// Optional first argument: path to an order JSON file; defaults to the bundled sample order.
return await Walkthrough.RunAsync(Console.Out, args.FirstOrDefault());
