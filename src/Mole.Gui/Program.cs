using System.IO;
using System.Linq;
using Mole.Veldrid;
using Mole.MonoGame;
using Mole.Shared.Util;
using Veldrid;

namespace Mole.Gui
{
    /// <summary>
    /// Entry class of Mole.Gui
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point of Mole.Gui
        /// </summary>
        public static void Main(string[] args)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "mole.log" };
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);        
            NLog.LogManager.Configuration = config;

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Mole GUI is launching...");

            if (!Directory.EnumerateFiles(Directory.GetCurrentDirectory(), $"{Asar.DllPath}*").Any()) {
                logger.Error("No Asar binaries were found, exiting...");
                return;
            }

            Asar.Init();

            string g = args.Length >= 1 ? args[0] : "";

            logger.Info("UI Rendering backend: {0}", g switch
            {
                "d" => "DirectX3D 11",
                "v" => "Vulkan",
                "m" => "Metal",
                "g" or _ => "OpenGL"
            });

            var ui = new Ui();
            switch (g)
            {
                case "d":
                    _ = new VeldridController(GraphicsBackend.Direct3D11, ui);
                    break;
                case "v":
                    _ = new VeldridController(GraphicsBackend.Vulkan, ui);
                    break;
                case "m":
                    _ = new VeldridController(GraphicsBackend.Metal, ui);
                    break;
                default:
                    using (var game = new MonoGameController(ui))
                        game.Run();
                    break;
            }

            Asar.Close();
        }
    }
}
