using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;
using ImGuiNET;
using Mole.Shared;
using Mole.Shared.Util;

namespace Mole.Gui.Dialogs
{
    /// <summary>
    /// File Dialog
    /// </summary>
    public class OpenProject : Window
    {
        private string _path = "";
        public override void Draw(Project.UiData data, List<Window> windows)
        {
            if (!ShouldDraw) return;
            
            if (!ImGui.IsPopupOpen("ProjectOpen")) 
                ImGui.OpenPopup("ProjectOpen");

            if (ImGui.IsPopupOpen("ProjectOpen"))
            {
                ImGui.SetNextWindowPos(ImGui.GetMainViewport().Size / 2, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
                if (ImGui.BeginPopupModal("ProjectOpen", ref ShouldDraw))
                {
                    if (ImGui.InputText("Path", ref _path,
                        500, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
                    {
                        ImGui.CloseCurrentPopup();
                        ShouldDraw = false;
                        if (!Directory.Exists(_path)) {
                            LoggerEntry.Logger.Warning("Invalid path: {0}", _path);
                            return;
                        }
                        
                        new Thread(() => {
                            try {
                                data.Project = new Project(data.Progress, _path);
                                windows[4].ShouldDraw = true;
                                windows[5].ShouldDraw = true;
                                windows[6].ShouldDraw = true;
                            } catch (Exception e) {
                                data.Progress.Exception = e;
                                data.Progress.ShowException = true;
                            }
                        }).Start();
                    }

                    if (ImGui.Button("Open"))
                    {
                        ImGui.CloseCurrentPopup();
                        ShouldDraw = false;
                        if (!Directory.Exists(_path)) {
                            LoggerEntry.Logger.Warning("Invalid path: {0}", _path);
                            return;
                        }
                        
                        new Thread(() => {
                            try {
                                data.Project = new Project(data.Progress, _path);
                                windows[4].ShouldDraw = true;
                                windows[5].ShouldDraw = true;
                                windows[6].ShouldDraw = true;
                            } catch (Exception e) {
                                data.Progress.Exception = e;
                                data.Progress.ShowException = true;
                            }
                        }).Start();
                    }
                    
                    ImGui.SetItemDefaultFocus();
                    ImGui.SameLine();
                        
                    if (ImGui.Button("Cancel"))
                    {
                        ImGui.CloseCurrentPopup();
                        ShouldDraw = false;
                    }

                    ImGui.EndPopup();
                }
            }
        }
    }
}