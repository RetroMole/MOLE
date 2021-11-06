using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;
using Mole.Shared;
using Mole.Shared.Util;

namespace Mole.Gui.Dialogs
{
    /// <summary>
    /// File Dialog
    /// </summary>
    public class OpenFile : Window
    {
        public string _path;
        public override void Draw(Project.UiData data, List<Window> windows)
        {
            if (!ShouldDraw) return;
            
            if (!ImGui.IsPopupOpen("RomOpen")) 
                ImGui.OpenPopup("RomOpen");

            if (ImGui.IsPopupOpen("RomOpen"))
            {
                ImGui.SetNextWindowPos(ImGui.GetMainViewport().Size / 2, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
                if (ImGui.BeginPopupModal("RomOpen", ref ShouldDraw))
                {
                    if (ImGui.InputText("Path", ref _path,
                        500, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
                    {
                        ImGui.CloseCurrentPopup();
                        ShouldDraw = false;
                        if (!File.Exists(_path)) {
                            LoggerEntry.Logger.Warning("Invalid path: {0}", _path);
                            return;
                        }

                        var task = new Task(() =>
                        {
                            var dir = string.Join('.', _path.Split('.').SkipLast(1)) + "_moleproj";
                            Directory.CreateDirectory(dir);
                            data.Project = new Project(data.Progress,
                                dir, _path);
                            windows[4].ShouldDraw = true;
                            windows[5].ShouldDraw = true;
                            windows[6].ShouldDraw = true;
                        });

                        task.ContinueWith(t => {
                            data.Progress.Exception = t.Exception;
                            data.Progress.ShowException = true;
                        }, TaskContinuationOptions.OnlyOnFaulted);
                        
                        task.Start();
                    }

                    if (ImGui.Button("Open"))
                    {
                        ImGui.CloseCurrentPopup();
                        ShouldDraw = false;
                        if (!File.Exists(_path)) {
                            LoggerEntry.Logger.Warning("Invalid path: {0}", _path);
                            return;
                        }

                        var task = new Task(() =>
                        {
                            var dir = string.Join('.', _path.Split('.').SkipLast(1)) + "_moleproj";
                            Directory.CreateDirectory(dir);
                            data.Project = new Project(data.Progress,
                                dir, _path);
                            windows[4].ShouldDraw = true;
                            windows[5].ShouldDraw = true;
                            windows[6].ShouldDraw = true;
                        });

                        task.ContinueWith(t => {
                            data.Progress.Exception = t.Exception;
                            data.Progress.ShowException = true;
                        }, TaskContinuationOptions.OnlyOnFaulted);
                        
                        task.Start();
                    }

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