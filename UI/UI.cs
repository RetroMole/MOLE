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
        private static ROM rom;
        static Exception e = new("");
        static string path = "";
        static bool filediag;

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
                        if (ImGui.MenuItem("Open ROM", "Ctrl+O"))
                        {
                            filediag = true;
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Debug"))
                    {
                        ImGui.MenuItem("FPS", null, ref show_fps);
                        ImGui.MenuItem("Mouse Pos", null, ref show_mousepos);
                        ImGui.MenuItem("Demo Window", null, ref show_demo);

                        ImGui.EndMenu();
                    }

                    WFileDialog();

                    ImGui.EndMainMenuBar();
                }
            }

            // Test
            {
                ImGui.SetNextWindowSize(new Num.Vector2(420, 69), ImGuiCond.FirstUseEver);
                ImGui.Begin("UwU");

                ImGui.Text("Test Window says HenlOwO, try opening a ROM");
                ImGui.Separator();

                if (rom != null)
                {
                    ImGui.Text(String.Format("ROM Name: {0}", rom.ROMName));
                    ImGui.Text(String.Format("ROM Path: {0}", rom.ROMPath));
                    ImGui.Text(String.Format("ROM Type: {0}", rom.Mapper.ToString()));
                    ImGui.Text(String.Format("ROM Header: 0x{0:x}", rom.header != null ? rom.header.Length : "None"));
                }

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

        public static void WFileDialog()
        {
            if (filediag)
            {
                if (!ImGui.IsPopupOpen("FileDialog"))
                {
                    ImGui.OpenPopup("FileDialog");
                }
                if (ImGui.IsPopupOpen("FileDialog"))
                {
                    ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.Appearing, new Num.Vector2(0.5f, 0.5f));
                    if (ImGui.BeginPopupModal("FileDialog"))
                    {
                        if (ImGui.InputTextWithHint("Path", @"C:\", ref path, 500, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
                        {
                            try
                            {
                                rom = new(path);
                                e = new();
                                ImGui.CloseCurrentPopup();
                                filediag = false;
                            }
                            catch (Exception ee)
                            {
                                e = ee;
                                ImGui.OpenPopup("err");
                            }
                        }

                        if (ImGui.Button("Open"))
                        {
                            try
                            {
                                rom = new(path);
                                ImGui.CloseCurrentPopup();
                                filediag = false;
                            }
                            catch (Exception ee)
                            {
                                e = ee;
                                ImGui.OpenPopup("err");
                            }

                        }
                        ImGui.SetItemDefaultFocus();
                        ImGui.SameLine();
                        if (ImGui.Button("Cancel"))
                        {
                            ImGui.CloseCurrentPopup();
                            filediag = false;
                        }

                        Werr();

                        ImGui.EndPopup();
                    }
                }
            }
        }

        public static void Werr()
        {
            if (ImGui.IsPopupOpen("err"))
            {
                if (ImGui.BeginPopupModal("err"))
                {
                    ImGui.Text(e.Message);
                    ImGui.Separator();
                    if (ImGui.Button("OK"))
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.EndPopup();
                }
            }
        }
    }
}