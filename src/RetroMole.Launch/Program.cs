#pragma warning disable CS8603, CS8618

using Serilog;
using Serilog.Events;
using Tommy;
using CommandLine;

namespace RetroMole;
public static partial class Launch
{
	public static CLI CLIOpts = new();
	public static void Main(string[] args)
	{
		// Parse CLI Arguments
		Parser.Default.ParseArguments<CLI>(args)    // Parse arguments
		.WithParsed(o => { CLIOpts = o; })          // If successful set options to the parsed results
		.WithNotParsed(errs => { if (errs.IsHelp() || errs.IsVersion()) { Environment.Exit(0); } }); // Check for help and version args and just exit Mole


		// Set up Default Logging
		Log.Logger = new LoggerConfiguration()
    		.WriteTo.Async(a => a.Console(), blockWhenFull: true)
			.WriteTo.Async(a => a.File(Path.Combine(Core.GLOBALS.CfgPath, "logs", "RetroMole.log"),
					rollingInterval: RollingInterval.Day,
					fileSizeLimitBytes: 2000000,
					rollOnFileSizeLimit: true,
					retainedFileCountLimit: 12
				),
				blockWhenFull: true
			)
			.MinimumLevel.Debug()
    		.CreateLogger();

		Log.Information(">-----------------------------<Starting>------------------------------<");

		// Read/Load config file
		Log.Information("Reading user config file...");
		
		if (!File.Exists(Path.Combine(Core.GLOBALS.CfgPath, "config.toml")))
			Log.Warning("No config file found, using default config...");
		else LoadConfig();

		Log.Information("Finishing Package initialization (hook application, gui window registration)");
		// Finish Package initialization
		Core.GLOBALS.Packages = Core.GLOBALS.Packages.Select(p => {
			p.ApplyHooks();
			Gui.Windows = Gui.Windows.Concat(p.Windows.Cast<Gui.Window>()).ToArray();
			return p;
		})
		.ToArray();

		Gui.ApplyHooks();
		Core.GLOBALS.CurrentController.Main(() => Gui.UI());
		Log.Information(">---------------------------<Shutting Down>---------------------------<");
		Log.CloseAndFlush();
	}

	public static void LoadConfig()
	{
		var logger = new LoggerConfiguration();
		foreach(TomlNode s in Core.GLOBALS.Config["logging"]["sinks"].AsArray)
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
					string path = Path.Combine(Core.GLOBALS.CfgPath, s["path"].AsString);
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
		switch((string)Core.GLOBALS.Config["logging"]["minimumLevel"].AsString)
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

#pragma warning restore CS8603, CS8618
