using System.IO;
using System.Numerics;
using ImGuiNET;
using Mole.Shared;

namespace Mole.Gui.Windows
{
    public class FileDialog
    {
        public static void Main(bool draw, ref Rom rom, ref Gfx gfx, ref bool filediag, ref string path)
        {
            if (draw)
            {
                if (!ImGui.IsPopupOpen("RomOpen")) 
                    ImGui.OpenPopup("RomOpen");

                if (ImGui.IsPopupOpen("RomOpen"))
                {
                    ImGui.SetNextWindowPos(ImGui.GetMainViewport().Size / 2, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
                    if (ImGui.BeginPopupModal("RomOpen"))
                    {
                        if (ImGui.InputText("Path", ref path,
                            500, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
                        {
                            ImGui.CloseCurrentPopup();
                            filediag = false;
                            if (!File.Exists(path)) {
                                LoggerEntry.Logger.Warning("Invalid path: {0}", path);
                                return;
                            }
                            rom = new Rom(path);
                            gfx = new Gfx(rom);
                        }

                        if (ImGui.Button("Open"))
                        {
                            ImGui.CloseCurrentPopup();
                            filediag = false;
                            if (!File.Exists(path)) {
                                LoggerEntry.Logger.Warning("Invalid path: {0}", path);
                                return;
                            }
                            rom = new Rom(path);
                            gfx = new Gfx(rom);
                        }

                        if (filediag == false) return;
                        
                        ImGui.SetItemDefaultFocus();
                        ImGui.SameLine();
                        
                        if (ImGui.Button("Cancel"))
                        {
                            ImGui.CloseCurrentPopup();
                            filediag = false;
                        }

                        ImGui.EndPopup();
                    }
                }
            }
        }
    }
}