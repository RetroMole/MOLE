using System;
using System.Reflection;
using Serilog;

namespace RetroMole;
public static class Launch
{
	public static void Main(string[] args)
	{
		var pckgsDir = Path.Combine(Core.Utility.CommonDirectories.Exec, "Packages");

		// Set up Logging
		Log.Logger = new LoggerConfiguration()
    		.WriteTo.Async(a => a.Console(), blockWhenFull: true)
			.WriteTo.Async(a => a.File(Path.Combine(Core.Utility.CommonDirectories.Exec, "RetroMole.log"),
					rollingInterval: RollingInterval.Day,
					fileSizeLimitBytes: 200000000,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 5
				),
				blockWhenFull: true
			)
			.MinimumLevel.Debug()
    		.CreateLogger();
		Log.Information(">-----------------------------<Starting>------------------------------<");
		Log.Information("Loading Packages");

		// Find and load all packages, apply their hooks and import their windows, and just pick the first controller of the first package for now
		var ctrlr = Directory.GetFileSystemEntries(pckgsDir, "*.dll", SearchOption.AllDirectories)
			.Concat(Directory.GetFileSystemEntries(pckgsDir, "*.mole.pckg", SearchOption.AllDirectories))
			.SelectMany(p => Path.GetExtension(p)?.ToUpper() switch {
				".DLL" 		 => Core.Utility.Import.AssemblyPackages(Assembly.LoadFrom(p)),
				".MOLE.PCKG" => Core.Utility.Import.CompressedPackages(p),
				// TODO: Make this work somehow...
				// Expected return type of Switch Case inside SelectMany is: Package[]
				// Actual return type of this case: void
				//_ 			 => Log.Error($"Uhh... how did you.. BAD PACKAGE @ {p}")
				_			 => throw new Exception($"Uhh... how did you.. BAD PACKAGE @ {p}")
			})
			.Select(p => {
				Log.Debug($"Loading Package \"{p.Name}\"...");

				p.ApplyHooks();
				Gui.Windows = Gui.Windows
					.Concat(p.Windows.Cast<Gui.Window>())
					.ToArray();

				Log.Debug($"Package \"{p.Name}\" Loaded Successfully!");
				return p;
			})
			.First().Controllers.First();
			
		Log.Information("Packages Loaded Successfully! Ready to launch...");
		ctrlr.Main(() => Gui.UI());
		Log.Information(">---------------------------<Shutting Down>---------------------------<");
		Log.CloseAndFlush();
	}
}
