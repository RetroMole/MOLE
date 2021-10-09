namespace MOLE
{
    class Program
    {

        public static string MOLEVer = "0.0.0";
        public static string LibMoleVer = "0.0.0";
        public static string MOLEUIVer = "0.0.0";
        public static void Main(string[] args)
        {
            // Logging config
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "MOLE.log" };
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);        
            NLog.LogManager.Configuration = config;

            var Logger = NLog.LogManager.GetCurrentClassLogger();
            Logger.Info("LAUNCHING MOLE");

            // Graphics Backend for ImGui.Net
            string g = args.Length >= 1 ? args[0] : "";

            Logger.Info("UI Rendering backend: {0}", g switch
            {
                "d" => "DirectX3D 11",
                "v" => "Vulkan",
                "m" => "Metal",
                "g" or _ => "OpenGL"
            });

            UI UI = new();

            switch (g)
            {
                case "d":
                    _ = new VeldridController.Controller(Veldrid.GraphicsBackend.Direct3D11, UI);
                    break;
                case "v":
                    _ = new VeldridController.Controller(Veldrid.GraphicsBackend.Vulkan, UI);
                    break;
                case "m":
                    _ = new VeldridController.Controller(Veldrid.GraphicsBackend.Metal, UI);
                    break;
                case "g":
                default:
                    using (var game = new XNAController.Controller(UI)) game.Run();
                    break;
            }
        }
    }
}
