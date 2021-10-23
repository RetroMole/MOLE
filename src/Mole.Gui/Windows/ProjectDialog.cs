using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Numerics;
using System.Threading;
using ImGuiNET;
using Mole.Shared;
using Mole.Shared.Util;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// File Dialog
    /// </summary>
    public class ProjectDialog : Window
    {
        private string _path = "";

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
        public override void Draw(Ui.UiData data, List<Window> windows)
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
                            data.Project = new Project(data.Progress, 
                                Directory.GetDirectoryRoot(_path));
                            windows[2].ShouldDraw = true;
                            windows[3].ShouldDraw = true;
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
                            data.Project = new Project(data.Progress, _path);
                            windows[2].ShouldDraw = true;
                            windows[3].ShouldDraw = true;
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