using System;
using System.Reflection;
using Mole.Shared;
using ImGuiNET;

namespace Mole.Gui
{
    public struct LibInfo
    {
        public string Name;
        public string Version;
        public string Repo;
        public string License;
    }

    public class Strings
    {
        public const string Copyright =
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

        public static string MoleGuiVersion = typeof(Strings).Assembly.Version.ToString();
        public static string MoleSharedVersion = typeof(Rom).Assembly.Version.ToString();

        public static LibInfo[] Libraries = new LibInfo[] {
            new LibInfo { Name = "ImGui.Net", Version = ImGuiNET.ImGui.GetVersion(), Repo = "https://github.com/mellinoe/ImGui.NET", License = "The MIT License (MIT)" },
            new LibInfo { Name = "Asar", Version = Asar.Ver2Str(Asar.Version()), Repo = "https://github.com/RPGHacker/asar", License = "GNU Lesser General Public License (LGPL)" },
            new LibInfo { Name = "TerraCompress", Version = "1.0", Repo = "https://github.com/Smallhacker/TerraCompress", License = "Zlib/libpng License (zlib)"},
            new LibInfo { Name = "Veldrid", Version = "4.8.0", Repo = "https://github.com/mellinoe/veldrid", License = "The MIT License (MIT)"},
            new LibInfo { Name = "Monogame", Version = "3.8.0.1641", Repo = "https://github.com/MonoGame/MonoGame", License = "Microsoft Public License (Ms-PL)"},
            new LibInfo { Name = "NLog", Version = "5.0.0-preview.2", Repo = "https://github.com/NLog/NLog", License = "BSD 3-Clause \"New\" or \"Revised\" License (BSD-3-Clause)"}
        };
    }
}

