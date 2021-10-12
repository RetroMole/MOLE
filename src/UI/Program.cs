namespace MOLE
{
    class Program
    {
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

        // Info
        public static string MOLEVer = "0.0.0";
        public static string LibMoleVer = "0.0.0";
        public static string MOLEUIVer = "0.0.0";

        public static string copyright =
            "MOLE is an open source Super Mario World ROM editor and is in no way affiliated with Nintendo.\n\n" +

            "Copyright(C) 2021 Vawlpe\n" +
            "This program is free software: you can redistribute it and / or modify it under the terms of\n" +
            "the GNU General Public License as published by the Free Software Foundation,\n" +
            "either version 3 of the License, or(at your option) any later version.\n" +
            "This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;\n" +
            "without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.\n" +
            "See the GNU General Public License for more details.\n" +
            "You should have received a copy of the GNU General Public License along with this program.\n" +
            "If not, see https://www.gnu.org/licenses/ \n\n" +

             "https://github.com/Vawlpe/MOLE";

        public static LibInfo[] libs = new LibInfo[] {
            new LibInfo { name = "ImGui.Net", ver = ImGuiNET.ImGui.GetVersion(), repo = "https://github.com/mellinoe/ImGui.NET", license = "The MIT License (MIT)" },
            new LibInfo { name = "Asar", ver = Asar.Ver2Str(Asar.Version()), repo = "https://github.com/RPGHacker/asar", license = "GNU Lesser General Public License (LGPL)" },
            new LibInfo { name = "TerraCompress", ver = "1.0", repo = "https://github.com/Smallhacker/TerraCompress", license = "Zlib/libpng License (zlib)"},
            new LibInfo { name = "Veldrid", ver = "4.8.0", repo = "https://github.com/mellinoe/veldrid", license = "The MIT License (MIT)"},
            new LibInfo { name = "Monogame", ver = "3.8.0.1641", repo = "https://github.com/MonoGame/MonoGame", license = "Microsoft Public License (Ms-PL)"},
            new LibInfo { name = "NLog", ver = "5.0.0-preview.2", repo = "https://github.com/NLog/NLog", license = "BSD 3-Clause \"New\" or \"Revised\" License (BSD-3-Clause)"}
        };

        public struct LibInfo
        {
            public string name;
            public string ver;
            public string repo;
            public string license;
        }
    }
}
