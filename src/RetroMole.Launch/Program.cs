using System;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Tommy;

namespace RetroMole;
public static class Launch
{
	public static void Main(string[] args)
	{
		var pckgsDir = Path.Combine(Core.Utility.CommonDirectories.Exec, "Packages");

		// Set up Default Logging
		Log.Logger = new LoggerConfiguration()
    		.WriteTo.Async(a => a.Console(), blockWhenFull: true)
			.WriteTo.Async(a => a.File(Path.Combine(Core.Utility.CommonDirectories.Cfg, "logs", "RetroMole.log"),
					rollingInterval: RollingInterval.Day,
					fileSizeLimitBytes: 200000000,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 7
				),
				blockWhenFull: true
			)
			.MinimumLevel.Debug()
    		.CreateLogger();
		Log.Information(">-----------------------------<Starting>------------------------------<");

		// Read config file
		Log.Information("Reading user config file...");
		var nocfg = !File.Exists(Path.Combine(Core.Utility.CommonDirectories.Cfg, "config.toml"));
		if (nocfg) Log.Warning("No config file found, using default config...");
		else LoadConfig();

		Log.Information("Loading Packages");

		// Find and load all packages, apply their hooks and import their windows, and pick the one from the config file
		var pckgs = Directory.GetFileSystemEntries(pckgsDir, "*.dll", SearchOption.AllDirectories)
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
			});

		var ctrlr = nocfg
				  ? pckgs.First().Controllers.First()
				  : pckgs.Select(p => p.Controllers.First(c => c.GetType().FullName == Config["renderer"].AsString)).First();
			
			
		Log.Information($"Packages Loaded Successfully! Ready to launch ({ctrlr.GetType().FullName})");

		ctrlr.Main(() => Gui.UI());
		Log.Information(">---------------------------<Shutting Down>---------------------------<");
		Log.CloseAndFlush();
	}

	public static TomlTable Config;
	public static void LoadConfig()
	{
		Config = Core.Utility.Import.Config(Path.Combine(Core.Utility.CommonDirectories.Cfg, "config.toml"));

		var logger = new LoggerConfiguration();
		foreach(TomlNode s in Config["logging"]["sinks"].AsArray)
		{
			string type  = s["type"].AsString;
			string level = s["level"].AsString;
			bool blkwf   = s["blockWhenFull"].AsBoolean;

			switch (type)
			{
				case "Console":
					string outputTemplate = s["outputTemplate"].AsString;
					logger.WriteTo.Async(a => a.Console(
							outputTemplate: outputTemplate,
							restrictedToMinimumLevel: (LogEventLevel)Enum.Parse(typeof(LogEventLevel), level)
						), 
						blockWhenFull: blkwf
					);
					break;
				case "File":
					string path = Path.Combine(Core.Utility.CommonDirectories.Cfg, s["path"].AsString);
					string ri = s["rollingInterval"].AsString;
					int fsl = s["fileSizeLimitBytes"].AsInteger;
					bool rofl = s["rollOnFileSizeLimit"].AsBoolean;
					int rfc = s["retainedFileCountLimit"].AsInteger;
					logger.WriteTo.Async(a => a.File(path,
							rollingInterval: (RollingInterval)Enum.Parse(typeof(RollingInterval), ri),
							fileSizeLimitBytes: fsl,
							rollOnFileSizeLimit: rofl,
							retainedFileCountLimit: rfc,
							restrictedToMinimumLevel: (LogEventLevel)Enum.Parse(typeof(LogEventLevel), level)
						),
						blockWhenFull: blkwf
					);
					break;
				default:
					throw new Exception("Unknown logging sink type");
			}
		}
		switch((string)Config["logging"]["minimumLevel"].AsString)
		{
			case "Verbose":
				logger.MinimumLevel.Verbose();
				break;
			case "Debug":
				logger.MinimumLevel.Debug();
				break;
			case "Information":
				logger.MinimumLevel.Information();
				break;
			case "Warning":
				logger.MinimumLevel.Warning();
				break;
			case "Error":
				logger.MinimumLevel.Error();
				break;
			case "Fatal":
				logger.MinimumLevel.Fatal();
				break;
			default:
				throw new Exception("Unknown log level");
		}
		Log.CloseAndFlush();
		Log.Logger = logger.CreateLogger();
		Log.Information("Logging settings loaded from config file.");
	}
}
