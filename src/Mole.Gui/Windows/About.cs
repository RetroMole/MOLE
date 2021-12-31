using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Mole.Shared.Util;
using Serilog;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// Debug information
    /// </summary>
    public class About : WindowBase
    {
        public override void Draw(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            if (!ShouldDraw) return;

            if (ImGui.Begin("About", ref ShouldDraw))
            {
                ImGui.Text(
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
                    "https://github.com/Vawlpe/MOLE");
                ImGui.Separator();
                ImGui.Text($"Mole Shared Version: 0");
                ImGui.Text($"MOLE GUI Version: 0");
                ImGui.Separator();
                string libs =
                    "Libraries:\n" +
                    "All of the following libraries are licensed under their respective Open Source Software licenses:\n";
                ImGui.Text(libs);
                ImGui.End();
            }
        }
    }
}