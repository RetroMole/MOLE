using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mole.Gui.Windows;
using Mole.Shared;
using Num = System.Numerics;

namespace Mole.Gui
{
    [SuppressMessage("ReSharper", "PositionalPropertyUsedProblem")]
    public class Ui
    {
        private static ImGuiViewportPtr _viewport;
        private static ImGuiIOPtr _io;
        private static readonly List<Delegate> Windows = new()
        {
            new Action(() => WGfx.Main(_gfx)),
            new Action(() => RomInfo.Main(_rom, _gfx)),
            new Action(() => About.Main(_showAbout)),
            new Action(() => FileDialog.Main(_filediag, ref _rom, ref _gfx, ref _filediag, ref _path))
        };

        private static bool _showFps = true;
        private static bool _showMousePos = true;
        private static bool _showAbout = false;
        private static bool _showDemo = false;
        private static Rom _rom;
        private static Gfx _gfx;
        private static string _path = "";
        private static bool _filediag;

        public static void Draw()
        {
            _io = ImGui.GetIO();
            _viewport = ImGui.GetMainViewport();
            
            // Top Bar
            {
                if (ImGui.BeginMainMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Open ROM", "Ctrl+O"))
                            _filediag = true;
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Debug"))
                    {
                        ImGui.MenuItem("FPS", null, ref _showFps);
                        ImGui.MenuItem("Mouse Pos", null, ref _showMousePos);
                        ImGui.MenuItem("Demo Window", null, ref _showDemo);
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Help"))
                    {
                        ImGui.MenuItem("About", null, ref _showAbout);
                        ImGui.EndMenu();
                    }
                    
                    ImGui.EndMainMenuBar();
                }
            }

            if (_showDemo) ImGui.ShowDemoWindow(ref _showDemo);

            foreach (var w in Windows)
                w.DynamicInvoke();
        }
    }
}
