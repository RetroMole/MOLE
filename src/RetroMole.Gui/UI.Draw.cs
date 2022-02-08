using ImGuiNET;
using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;
using Serilog;

namespace RetroMole.Gui
{
    public static partial class Ui
    {
        public static void Draw(ref RenderManager _r, ref ModuleManager _m)
        {
            try
            {
                Rmngr = _r;
                GMmngr = _m;

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
                            // TODO: Show confirmation prompt
                            if (ImGui.MenuItem("New Project From ROM", "Ctrl+N"))
                            {
                                Windows["OpenFile"].Open();
                                Windows["OpenFile"].OnClose += new Action<WindowBase>(OpenFileEventHandler);
                            }

                            if (ImGui.MenuItem("Open Project Folder", "Ctrl+O"))
                            {
                                Windows["OpenProject"].Open();
                                Windows["OpenProject"].OnClose += new Action<WindowBase>(OpenProjectEventHandler);
                            }

                            if (ImGui.MenuItem("Open Compressed Project File", "Ctrl+Shift+O"))
                            {
                                Windows["OpenProjectFile"].Open();
                                Windows["OpenProjectFile"].OnClose += new Action<WindowBase>(OpenProjectFileEventHandler);
                            }

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
                            ImGui.EndMenu();
                        }

                        //==============Open===============
                        if (ImGui.BeginMenu("Open..."))
                        {
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
                        if (ImGui.MenuItem("Asar Test"))
                            Windows["AsarTest"].Open();
                        ImGui.EndMenu();
                    }

                    //==============Help===============
                    if (ImGui.BeginMenu("Help"))
                    {
                        if (ImGui.MenuItem("About"))
                            Windows["About"].Open();

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Extra"))
                    {
                        Events.Ui.TriggerMenuBar();
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
                foreach (var m in GMmngr.AvailableModules)
                    foreach (var w in m.Value.Windows.Values)
                        w.Draw(Data, Windows);
            }
            //==========Error Handler==========
            catch (Exception e)
            {
                Log.Error(e, "");
                //var w = (Windows["Error"] as Dialogs.Error);
                //if (w.e is null)
                //{
                //    w.e = e;
                //    w.Unhandled = true;
                //    w.Open();
                //    w.Draw(Data, Windows);
                //}
            }
        }
    }
}
