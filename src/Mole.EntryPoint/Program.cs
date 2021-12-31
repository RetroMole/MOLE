using CommandLine;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Reflection;

namespace Mole
{
    public static class Program
    {
        private static readonly RenderManager RenderManager = new();
        private static Options Opts = new();
        private class Options
        {
            [Option("loglevel", Required = false, Default = LogEventLevel.Information, HelpText = "Specifies the verbosity of Mole's logging output\nValues: Verbose, Debug, Information, Warning, Error, Fatal")]
            public LogEventLevel LogLevel { get; set; }
            [Option("renderer", Required = false, Default = "MonogameRenderer", HelpText = "Specifies which of the available rendering backends for Mole to use")]
            public string Renderer {get; set;}
        }
        public static void Main(string[] args)
        {
            // Parse CLI Arguments
            Parser.Default.ParseArguments<Options>(args)    // Parse arguments
            .WithParsed(o => { Opts = o; })              // If successful set options to the parsed results
            .WithNotParsed(errs => { if (errs.IsHelp() || errs.IsVersion()) { Environment.Exit(0); } }); // Check for help and version args and just exit Mole

            // Setup Logger
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()          // Log to console
            .WriteTo.File("mole.log")   // Log to file
            .MinimumLevel.ControlledBy(new LoggingLevelSwitch(Opts.LogLevel)) // Set minimum logging level to output
            .CreateLogger();

            Log.Information("Mole v{0} Starting...", Assembly.GetExecutingAssembly().GetName().Version);

            // Load available renderers
            RenderManager.LoadAvailableRenderers();

            // RUN UI
            RenderManager.RunRenderer(Opts.Renderer);

            // Dispose of Logger
            Log.CloseAndFlush();
        }
    }
}
 