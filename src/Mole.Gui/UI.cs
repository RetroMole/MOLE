using ImGuiNET;
using System.Collections.Generic;
using Mole.Gui.Windows;
using Mole.Shared;
using Mole.Shared.Util;

namespace Mole.Gui
{
    public class Ui
    {
        private static readonly List<Window> Windows = new() {
            new About(),
            new FileDialog(),
            new ProjectDialog(),
            new LoadingDialog(),
            new RomInfo(),
            new PalEditor(),
            new WGfx(),
        };

        private static bool _showDemo;
        private static readonly Project.UiData Data = new();

        public static void Draw()
        {
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
            
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open ROM", "Ctrl+N"))
                        Windows[1].ShouldDraw = true;
                    if (ImGui.MenuItem("Open Project", "Ctrl+O"))
                        Windows[2].ShouldDraw = true;
                    if (ImGui.MenuItem("Save Project", "Ctrl+S")
                        && Data.Project != null) Data.Project.SaveProject();
                    if (ImGui.MenuItem("Close Project", "Ctrl+C"))
                    {
                        Data.Progress = new Progress();
                        Data.Project = null;
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Debug"))
                {
                    ImGui.MenuItem("Demo Window", null, ref _showDemo);
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("About"))
                        Windows[0].ShouldDraw = true;

                    ImGui.EndMenu();
                }
                    
                ImGui.EndMainMenuBar();
            }

            if (_showDemo) ImGui.ShowDemoWindow(ref _showDemo);

            foreach (var w in Windows)
                w.Draw(Data, Windows);
        }
    }
}
