using System.IO;
using System.Linq;
using Mole.MonoGame;
using Mole.Shared;
using Mole.Shared.Util;
using Mole.Veldrid;
using Serilog;
using Serilog.Core;
using Veldrid;

namespace Mole.Gui
{
    /// <summary>
    /// Entry class of Mole.Gui
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Serilog Logger
        /// </summary>
        private static readonly Logger Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("mole.log")
            .CreateLogger();
        
        /// <summary>
        /// Entry point of Mole.Gui
        /// </summary>
        public static void Main(string[] args)
        {
            Logger.Information("Mole GUI is launching...");

            if (!Directory.EnumerateFiles(Directory.GetCurrentDirectory(), $"{Asar.DllPath}*").Any()) {
                Logger.Fatal("No Asar binaries were found, exiting...");
                return;
            }

            Asar.Init();

            string g = args.Length >= 1 ? args[0] : "";

            Logger.Information("UI Rendering backend: {0}", g switch
            {
                "vd" => "Veldrid DirectX3D 11",
                "vv" => "Veldrid Vulkan",
                "vm" => "Veldrid Metal",
                "vg" => "Veldrid OpenGL",
                "ve" => "Veldrid OpenGL ES",
                "mg" or _ => "MonoGame OpenGL"
            });

            LoggerEntry.Logger = Logger;
            
            var ui = new Ui();
            switch (g)
            {
                case "vd":
                    VeldridController.Main(GraphicsBackend.Direct3D11, ui);
                    break;
                case "vv":
                    VeldridController.Main(GraphicsBackend.Vulkan, ui);
                    break;
                case "vm":
                    VeldridController.Main(GraphicsBackend.Metal, ui);
                    break;
                case "vg":
                    VeldridController.Main(GraphicsBackend.OpenGL, ui);
                    break;
                case "ve":
                    VeldridController.Main(GraphicsBackend.OpenGLES, ui);
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
