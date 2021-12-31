using ImGuiNET;
using Mole.Shared;
using Mole.Shared.Util;
using Serilog;
using System;

namespace Mole.Gui
{
    public static partial class Ui
    {
        public static void Draw(ref IRenderer r)
        {
            try
            {
                renderer = r;
                ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
                //==========Main Menu Bar==========
                if (ImGui.BeginMainMenuBar())
                {
                    //==============File===============
                    if (ImGui.BeginMenu("File"))
                    {
                        //=============Project=============
                        if (ImGui.BeginMenu("Project..."))
                        {
                            // TODO: Show replace confirmation prompt
                            if (ImGui.MenuItem("New Project From ROM", "Ctrl+N"))
                            {
                                Windows["OpenFile"].Open();
                                Windows["OpenFile"].Close += new Action<WindowBase>(OpenFileEventHandler);
                            }

                            if (ImGui.MenuItem("Open Project Folder", "Ctrl+O"))
                            {
                                Windows["OpenProject"].Open();
                                Windows["OpenProject"].Close += new Action<WindowBase>(OpenProjectEventHandler);
                            }

                            // TODO: Load Compressed Project File
                            if (ImGui.MenuItem("Open Compressed Project File", "Ctrl+Shift+O"))
                                Windows["OpenProjectFile"].Open();

                            // TODO: Show confirmation prompt
                            if (ImGui.MenuItem("Save Project", "Ctrl+Shift+S") && Data.Project != null)
                                Data.Project.SaveProject();

                            // TODO: Show confirmation prompt
                            if (ImGui.MenuItem("Close Project"))
                            {
                                Data.Progress = new Progress();
                                Data.Project = null;
                            }

                            ImGui.EndMenu();
                        }

                        //===============New===============
                        if (ImGui.BeginMenu("New..."))
                        {
                            if (ImGui.MenuItem("Level")) { }
                            if (ImGui.MenuItem("Patch")) { }
                            if (ImGui.MenuItem("Block")) { }
                            if (ImGui.MenuItem("Sprite")) { }
                            if (ImGui.MenuItem("Palette")) { }
                            if (ImGui.MenuItem("ExGFX")) { }
                            if (ImGui.MenuItem("UberASM Patch")) { }
                            ImGui.EndMenu();
                        }

                        //==============Open===============
                        if (ImGui.BeginMenu("Open..."))
                        {
                            //==============Level==============
                            if (ImGui.BeginMenu("Level..."))
                            {
                                if (ImGui.MenuItem("By Number")) { }
                                if (ImGui.MenuItem("From Address")) { }
                                if (ImGui.MenuItem("From File")) { }
                                ImGui.EndMenu();
                            }
                            //===============GFX===============
                            if (ImGui.BeginMenu("GFX..."))
                            {
                                if (ImGui.MenuItem("By Number")) { }
                                if (ImGui.MenuItem("From Address")) { }
                                if (ImGui.MenuItem("From File")) { }
                                ImGui.EndMenu();
                            }
                            //=============Palette=============
                            if (ImGui.BeginMenu("Palette..."))
                            {
                                if (ImGui.MenuItem("From Address")) { }
                                if (ImGui.MenuItem("From File")) { }
                                ImGui.EndMenu();
                            }
                            //==============Other==============
                            if (ImGui.MenuItem("Map16 File")) { }
                            if (ImGui.MenuItem("Patch File")) { }
                            if (ImGui.MenuItem("Block File")) { }
                            if (ImGui.MenuItem("Sprite File")) { }
                            if (ImGui.MenuItem("UberASM Patch File")) { }
                            ImGui.EndMenu();
                        }

                        //==============Misc===============
                        ImGui.Separator();
                        if (ImGui.MenuItem("Exit", "Alt+F4"))
                            Environment.Exit(0);
                        ImGui.EndMenu();
                    }

                    //==============Debug==============
                    if (ImGui.BeginMenu("Debug"))
                    {
                        ImGui.MenuItem("Demo Window", null, ref _ShowDemo);
                        ImGui.EndMenu();
                    }

                    //==============Help===============
                    if (ImGui.BeginMenu("Help"))
                    {
                        if (ImGui.MenuItem("About"))
                            Windows["About"].Open();

                        ImGui.EndMenu();
                    }

                    ImGui.EndMainMenuBar();
                }

                //===========Demo Window===========
                if (_ShowDemo)
                    ImGui.ShowDemoWindow(ref _ShowDemo);

                //==========Draw Windows===========
                foreach (var w in Windows.Values)
                    w.Draw(Data, Windows);
            }
            //==========Error Handler==========
            catch (Exception e)
            {
                Log.Error(e, "");
                var w = (Windows["Error"] as Dialogs.Error);
                if (w.e is null)
                {
                    w.e = e;
                    w.Unhandled = true;
                    w.Open();
                    w.Draw(Data, Windows);
                }
            }
        }
    }
}
