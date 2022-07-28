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
		
		// Set up default Logging
		ConfigureLogging();

		Log.Information(">-----------------------------<Starting>------------------------------<");

		// Read/Load config file
		Log.Information("Reading user config file...");
		
		if (!File.Exists(Path.Combine(Core.GLOBALS.CfgPath, "config.toml")))
		{
			Log.Warning("No config file found, using default config...");
			Core.Utility.Export.Config(
				Core.GLOBALS.DefaultConfig,
				Path.Combine(Core.GLOBALS.CfgPath, "config.toml")
			);
		}
		else
		{
			Log.Information("Config file found, re-configuring logger...");
			ConfigureLogging();
		}

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

	public static void ConfigureLogging()
	{
		var logger = new LoggerConfiguration();
		for (int i = 0; i < Core.GLOBALS.Config["logging"]["sinks"].ChildrenCount; i++)
		{
			TomlTable s = Core.GLOBALS.Config["logging"]["sinks"].AsArray[i]["value"].AsTable;
			string type  = s["type"].AsString;
			string level = s["level"].AsString;
			bool blkwf   = s["blockWhenFull"].AsBoolean;

			if (level == "Fatal")
				Log.Warning("Logging level for {type} sink is set to Fatal.\n\tThis may cause error messages to be lost.\n\tPlease consider lowering the logging level");

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
					Log.Error("Unknown Logging sink type: {0}", type);
					break;
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
				Log.Error("Unknown minimum log level: {0}", Core.GLOBALS.Config["logging"]["minimumLevel"].AsString);
				break;
		}

		if (Core.GLOBALS.Config["logging"]["minimumLevel"].AsString == "Fatal")
			Log.Warning("Logging level set to Fatal, this may cause error messages to be lost, please consider lowering the logging level");

		Log.CloseAndFlush();
		Log.Logger = logger.CreateLogger();
		Log.Information("Logging settings loaded from config file.");
	}
}

#pragma warning restore CS8603, CS8618
