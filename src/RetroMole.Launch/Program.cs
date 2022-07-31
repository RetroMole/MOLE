#pragma warning disable CS8603, CS8618

using System.Reflection;
using Serilog;
using Serilog.Events;
using Tommy;
using CommandLine;
using RetroMole.Core;

namespace RetroMole;
public static partial class Launch
{
	public static void Main(string[] args)
	{
		ConfigureLogging();

		Log.Information(">-----------------------------<Starting>------------------------------<");
		Log.Information($"RetroMole.Launch v{Assembly.GetAssembly(typeof(Launch))?.GetName().Version}");
		Log.Information($"RetroMole.Core   v{Assembly.GetAssembly(typeof(Core.GLOBAL))?.GetName().Version}");
		Log.Information($"RetroMole.Gui    v{Assembly.GetAssembly(typeof(Gui))?.GetName().Version}");
		Log.Information($"{GLOBAL.Packages.Length} Packages found");

		Log.Information("Applying Package Hooks & Registering Windows)");
		GLOBAL.Packages = GLOBAL.Packages.Select(p => {
			p.ApplyHooks();
			Gui.Windows = Gui.Windows.Concat(p.Windows.Cast<Gui.Window>()).ToArray();
			return p;
		})
		.ToArray();

		Log.Information("Applying Final GUI Hooks");
		Gui.ApplyHooks();

		Log.Information($"Starting GUI via {GLOBAL.Config.Renderer.FullClassName} w/ {GLOBAL.Config.Renderer.Parameters.Length}	Parameters:\n\t          "
			+ string.Join("\n\t          ", GLOBAL.Config.Renderer.Parameters.Select(p => $"{p.Name}: {p.Value}")));

		GLOBAL.CurrentController.Main(() => Gui.UI());


		// Save config to file
		Log.Information("Saving Config to file");
		if (!File.Exists(Path.Combine(GLOBAL.CfgPath, "config.toml")))
	        Export.TOMLFile(Config.Sink.TOML(GLOBAL.Config), Path.Combine(GLOBAL.CfgPath, "config.toml"));

		Log.Information(">---------------------------<Shutting Down>---------------------------<");
		Log.CloseAndFlush();
	}

	public static void ConfigureLogging()
	{
		var logger = new LoggerConfiguration();
		foreach (Config.ConfT.LogT.SinkT s in GLOBAL.Config.Logging.Sinks)
		{
			switch (s.Type)
			{
				case Config.ConfT.LogT.SinkT.TypeT.Console:
					
					logger.WriteTo.Async(a => a.Console(
							restrictedToMinimumLevel: s.Level,
							outputTemplate: s.OutputTemplate
						), 
						blockWhenFull: s.BlockWhenFull
					);
					break;
				case Config.ConfT.LogT.SinkT.TypeT.File:
					logger.WriteTo.Async(a => a.File( s.Path,
							rollingInterval: 	  	  s.RollingInterval,
							fileSizeLimitBytes: 	  s.FileSizeLimitBytes,
							rollOnFileSizeLimit: 	  s.RollOnFileSizeLimit,
							retainedFileCountLimit:   s.RetainedFileCountLimit,
							restrictedToMinimumLevel: s.Level,
							outputTemplate: 		  s.OutputTemplate
						),
						blockWhenFull: s.BlockWhenFull
					);
					break;
				default:
					Log.Error($"Unknown Logging sink type: {s.Type}");
					break;
			}
		}

		switch(GLOBAL.Config.Logging.MinimumLevel)
		{
			case LogEventLevel.Verbose:
				logger.MinimumLevel.Verbose();
				break;
			case LogEventLevel.Debug:
				logger.MinimumLevel.Debug();
				break;
			case LogEventLevel.Information:
				logger.MinimumLevel.Information();
				break;
			case LogEventLevel.Warning:
				logger.MinimumLevel.Warning();
				break;
			case LogEventLevel.Error:
				logger.MinimumLevel.Error();
				break;
			case LogEventLevel.Fatal:
				logger.MinimumLevel.Fatal();
				break;
			default:
				Log.Error("Unknown minimum log level: {0}", GLOBAL.Config.Logging.MinimumLevel.ToString());
				break;
		}

		Log.CloseAndFlush();
		Log.Logger = logger.CreateLogger();
	}
}

#pragma warning restore CS8603, CS8618
