using ImGuiNET;
using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;

namespace RetroMole.Gui.Windows
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
                ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_00CCFF);
                ImGui.TextWrapped("RetroMole is an open source ROM editor and does not endorse any form of Copyright infringing activities including distribution of ROMs with or without monetary incentive");
                ImGui.PopStyleColor(); ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_96FF9F);
                ImGui.TextWrapped("Copyright (C) 2020-2022 Vawlpe, RetroMole");
                ImGui.PopStyleColor();
                ImGui.TextWrapped(
                    "This program is free software: you can redistribute it and / or modify it under the terms of the " +
                    "GNU General Public License as published by the Free Software Foundation, " +
                    "either version 3 of the License, or (at your option) any later version." +
                    "\nThis program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; " +
                    "without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. " +
                    "See the GNU General Public License for more details. " +
                    "You should have received a copy of the GNU General Public License along with this program." +
                    "\nIf not, see"); ImGui.SetCursorPos(new(ImGui.GetCursorPosX() + ImGui.CalcTextSize("If not, see ").X, ImGui.GetCursorPosY() - ImGui.GetTextLineHeightWithSpacing())); Widgets.Link.link("https://www.gnu.org/licenses/ \n", "https://www.gnu.org/licenses/");
                    Widgets.Link.link("(GH) RetroMole", "https://github.com/RetroMole");
                ImGui.Separator();
                ImGui.Text($"Mole Core Version: v0.0.0.0");
                ImGui.Text($"MOLE GUI Version: v0.0.0.0");
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