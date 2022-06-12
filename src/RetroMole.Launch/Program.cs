using System;
using System.Reflection;
using Serilog;

namespace RetroMole;
public static class Launch
{
	public static void Main(string[] args)
	{
		var pckgsDir = Path.Combine(Core.Utility.CommonDirectories.Exec, "Packages");

		Log.Logger = new LoggerConfiguration()
    		.WriteTo.Console()
    		.CreateLogger();

		Log.Information("OwO");

		// Find all .dll files in dir 
		Directory.GetFileSystemEntries(pckgsDir, "*.dll", SearchOption.AllDirectories)
		// Find all .mole.pckg files in dir
		.Concat(Directory.GetFileSystemEntries(pckgsDir, "*.mole.pckg", SearchOption.AllDirectories))
		// Load all of the packages from these files
		.SelectMany(p => Path.GetExtension(p)?.ToUpper() switch {
			".DLL" 		 => Core.Utility.Import.AssemblyPackages(Assembly.LoadFrom(p)),
			".MOLE.PCKG" => Core.Utility.Import.CompressedPackages(p),
			_ 			 => throw new Exception($"Uhh... how did you.. BAD PACKAGE @ {p}")
		})
		// Apply any hooks these packages may have
		.Select(p => {
			p.ApplyHooks();
			return p;
		})
		// For now just run the first Controller of the first package given the test UI callback
		.First().Controllers.First()
		.Main(() => Gui.Main.TestUI());
	}
}
