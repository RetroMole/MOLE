using System;
using System.Reflection;
using CommandLine;
using RetroMole.Core;
using RetroMole.Core.Assemblers;
using Serilog;
using Serilog.Core;

namespace RetroMole.Launch
{
    public static class Program
    {
        internal static CLI CLIOpts = new();
        internal static byte[] PublicKeyToken = new byte[] { 0xb5, 0xb0, 0xb7, 0xb0, 0x18, 0x3f, 0xe3, 0xfd};
        internal static RenderManager RMngr = new();
        public static void Main(string[] args)
        {
            // Parse CLI Arguments
            Parser.Default.ParseArguments<CLI>(args)    // Parse arguments
            .WithParsed(o => { CLIOpts = o; })          // If successful set options to the parsed results
            .WithNotParsed(errs => { if (errs.IsHelp() || errs.IsVersion()) { Environment.Exit(0); } }); // Check for help and version args and just exit Mole

            // Setup Logger
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()          // Log to console
            .WriteTo.File("RetroMole_CORE.log")   // Log to file
            .MinimumLevel.ControlledBy(new LoggingLevelSwitch(CLIOpts.LogLevel)) // Set minimum logging level to output
            .CreateLogger();

            // Initialize Asar
            Log.Information("Initializing Asar...");
            if (Asar.Init())
                Log.Information("Successfully initialized Asar {0}", Asar.Ver2Str(Asar.Version()));
            else
                Log.Warning("Could not initialize Asar");

            Log.Information("RetroMole v{0} Starting...", Assembly.GetExecutingAssembly().GetName().Version);

            // Load available renderers
            RMngr.LoadAssemblies(Path.Combine(Assembly.GetEntryAssembly().Location.Split('\\').SkipLast(1).Append("Renderers").ToArray()), PublicKeyToken);

            // RUN UI
            RMngr.RunRenderer(CLIOpts.Renderer);

            // Dispose of Logger
            Log.CloseAndFlush();
        }
    }
}