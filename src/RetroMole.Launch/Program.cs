#pragma warning disable CS8603, CS8618

using Serilog;
using Serilog.Events;
using Tommy;
using CommandLine;
using RetroMole.Core;

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
		
		if (!File.Exists(Path.Combine(GLOBAL.CfgPath, "config.toml")))
		{
			Log.Warning("No config file found, using default config...");
			Core.Export.Config(
				GLOBAL.DefaultConfig,
				Path.Combine(GLOBAL.CfgPath, "config.toml")
			);
		}
		else
		{
			Log.Information("Config file found, re-configuring logger...");
			ConfigureLogging();
		}

		Log.Information("Finishing Package initialization (hook application, gui window registration)");
		// Finish Package initialization
		GLOBAL.Packages = GLOBAL.Packages.Select(p => {
			p.ApplyHooks();
			Gui.Windows = Gui.Windows.Concat(p.Windows.Cast<Gui.Window>()).ToArray();
			return p;
		})
		.ToArray();

		Gui.ApplyHooks();
		GLOBAL.CurrentController.Main(() => Gui.UI());
		Log.Information(">---------------------------<Shutting Down>---------------------------<");
		Log.CloseAndFlush();
	}

	public static void ConfigureLogging()
	{
		var logger = new LoggerConfiguration();
		for (int i = 0; i < GLOBAL.Config["logging"]["sinks"].ChildrenCount; i++)
		{
			TomlTable s = GLOBAL.Config["logging"]["sinks"].AsArray[i].AsTable;

			string type  = s["type"].AsString;
			string level = s["level"].AsString ?? GLOBAL.Config["logging"]["minimumLevel"].AsString.Value;

			if (level == "Fatal")
			Log.Warning($"Logging level for {i + 1}{(i + 1 % 10) switch { 1 => "st", 2 => "nd", 3 => "rd", _ => "th"}} sink ({type}) is set to Fatal.\n" +
				"\t       This may cause error messages to be lost.\n" +
				"\t       Please consider lowering the logging level"
			);

			switch (type)
			{
				case "Console":
					
					logger.WriteTo.Async(a => a.Console(
							restrictedToMinimumLevel: (LogEventLevel)Enum.Parse(typeof(LogEventLevel), level),
							outputTemplate: s["outputTemplate"].AsString
						), 
						blockWhenFull: s["blockWhenFull"].AsBoolean
					);
					break;
				case "File":
					logger.WriteTo.Async(a => a.File( s["path"].AsString.Value,
							rollingInterval: 	  	  (RollingInterval)Enum.Parse(typeof(RollingInterval), s["rollingInterval"].AsString),
							fileSizeLimitBytes: 	  s["fileSizeLimitBytes"].AsInteger,
							rollOnFileSizeLimit: 	  s["rollOnFileSizeLimit"].AsBoolean,
							retainedFileCountLimit:   s["retainedFileCountLimit"].AsInteger,
							restrictedToMinimumLevel: (LogEventLevel)Enum.Parse(typeof(LogEventLevel), level),
							outputTemplate: 		  s["outputTemplate"].AsString
						),
						blockWhenFull: s["blockWhenFull"].AsBoolean
					);
					break;
				default:
					Log.Error($"Unknown Logging sink type: {type}");
					break;
			}
		}

		switch(GLOBAL.Config["logging"]["minimumLevel"].AsString.Value)
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
				Log.Error("Unknown minimum log level: {0}", GLOBAL.Config["logging"]["minimumLevel"].AsString);
				break;
		}

		if (GLOBAL.Config["logging"]["minimumLevel"].AsString == "Fatal")
			Log.Warning("Minimum Logging level set to Fatal, this may cause error messages to be lost, please consider lowering the logging level");

		Log.CloseAndFlush();
		Log.Logger = logger.CreateLogger();
		Log.Information("Logging settings loaded from config file.");
	}
}

#pragma warning restore CS8603, CS8618
