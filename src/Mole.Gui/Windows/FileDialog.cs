using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Numerics;
using System.Threading;
using ImGuiNET;
using Mole.Shared;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// File Dialog
    /// </summary>
    public class FileDialog : Window
    {
        private string _path = "";

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
        public override void Draw(Ui.UiData data, List<Window> windows)
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

                        data.Path = _path;
                        windows[2].ShouldDraw = true;
                        windows[3].ShouldDraw = true;
                        new Thread(() => {
                            data.Rom = new(data.Path);
                            data.Gfx = new();
                            new Thread(() => {
                                Gfx.NewRef(ref data.Gfx, data.Rom);
                            }).Start();
                        }).Start();
                    }

                    if (ImGui.Button("Open"))
                    {
                        ImGui.CloseCurrentPopup();
                        ShouldDraw = false;
                        if (!File.Exists(_path)) {
                            LoggerEntry.Logger.Warning("Invalid path: {0}", _path);
                            return;
                        }
                        
                        data.Path = _path;
                        windows[2].ShouldDraw = true;
                        windows[3].ShouldDraw = true;
                        new Thread(() => {
                            data.Rom = new(data.Path);
                            data.Gfx = new();
                            new Thread(() => {
                                Gfx.NewRef(ref data.Gfx, data.Rom);
                            }).Start();
                        }).Start();
                    }

                    if (ShouldDraw == false) return;
                        
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