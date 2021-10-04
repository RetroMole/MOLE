using ImGuiNET;
using System;
using System.Collections.Generic;
using Num = System.Numerics;

namespace MOLE
{
    public partial class UI
    {
        private static ImGuiViewportPtr viewport;
        private static ImGuiIOPtr io;
        private static readonly List<Delegate> windows = new()
        {
        };
        static bool show_fps = true;
        static bool show_mousepos = true;
        static bool show_demo = false;
        static bool debug_open;

        public static void Draw()
        {

            io = ImGui.GetIO();
            viewport = ImGui.GetMainViewport();

            debug_open = show_fps || show_mousepos;

            // Main Menu Bar
            {
                if (ImGui.BeginMainMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Open ROM", "Ctrl+O")) {}

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Debug"))
                    {
                        ImGui.MenuItem("FPS", null, ref show_fps);
                        ImGui.MenuItem("Mouse Pos", null, ref show_mousepos);
                        ImGui.MenuItem("Demo Window", null, ref show_demo);

                        ImGui.EndMenu();
                    }

                    ImGui.EndMainMenuBar();
                }
            }

            // Test
            {
                ImGui.SetNextWindowSize(new Num.Vector2(420, 69), ImGuiCond.FirstUseEver);
                ImGui.Begin("OwO");

                ImGui.Text("Test window lol");
                ImGui.Separator();

                ImGui.End();
            }

            if (debug_open) // Debug
            {
                ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;
                const float PAD = 10f;

                Num.Vector2 work_pos = viewport.WorkPos;
                Num.Vector2 work_size = viewport.WorkSize;
                Num.Vector2 window_pos = new(work_pos.X + work_size.X - PAD, work_pos.Y + work_size.Y - PAD);

                ImGui.SetNextWindowPos(window_pos, ImGuiCond.Always, new Num.Vector2(1, 1));
                window_flags |= ImGuiWindowFlags.NoMove;

                ImGui.SetNextWindowBgAlpha(0.35f);

                if (ImGui.Begin("Debug", ref debug_open, window_flags))
                {
                    if (show_fps) { ImGui.Text(string.Format("{0:F3} ms/frame ({1:F1} FPS)", 1000f / io.Framerate, io.Framerate)); }

                    if (show_mousepos)
                    {
                        if (ImGui.IsMousePosValid())
                            ImGui.Text(String.Format("Mouse Position: ({0},{1})", io.MousePos.X, io.MousePos.Y));
                        else
                            ImGui.Text("Mouse Position: <invalid>");
                    }


                    if (ImGui.BeginPopupContextWindow())
                    {
                        if (debug_open && ImGui.MenuItem("Close")) { show_fps = false; show_mousepos = false; }
                        ImGui.EndPopup();
                    }

                    ImGui.End();
                }
            }
            if (show_demo) ImGui.ShowDemoWindow(ref show_demo);

            // Draw any and all other windows
            foreach (var w in windows)
            {
                w.DynamicInvoke();
            }
        }
    }
}