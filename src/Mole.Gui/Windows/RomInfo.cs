using System.Numerics;
using ImGuiNET;
using Mole.Shared;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// ROM information
    /// </summary>
    public static class RomInfo
    {
        public static void Main(Rom rom, Gfx gfx)
        {
             ImGui.SetNextWindowSize(new Vector2(420, 69), ImGuiCond.FirstUseEver);
                ImGui.Begin("ROM Info");
                
                ImGui.Text("You will see ROM information here");
                ImGui.Separator();

                if (rom != null)
                {
                    ImGui.Text($"ROM FileName: {rom.FileName}");
                    ImGui.Text($"ROM Path: {rom.FilePath}");
                    ImGui.Text($"Copier Header: 0x{(rom.Header != null ? rom.Header.Length : "None"):X2}");

                    ImGui.Separator();
                    ImGui.Text("Internal ROM Header:");
                    ImGui.Text($"  ROM Title: \"{rom.Title}\"");
                    ImGui.Text($"  Mapping Mode: {(rom.FastRom ? "FastROM" : "SlowROM")}, {rom.Mapping}");
                    ImGui.Text($"  ROM Size: {rom.RomSize}kb");
                    ImGui.Text($"  SRAM Size: {rom.SramSize}kb");
                    ImGui.Text($"  Region: {rom.Region}");
                    ImGui.Text($"  Developer ID: {rom.DevId:X2}");
                    ImGui.Text($"  Version: {rom.Version}");
                    ImGui.Text($"  Checksum: {rom.Checksum:X4}");
                    ImGui.Text($"  Checksum Complement: {rom.ChecksumComplement:X4}");

                    ImGui.Separator();
                    if (ImGui.CollapsingHeader("GFX Pointers:"))
                    {
                        for (int i = 0; i < 0x34; i++)
                        {
                            ImGui.Text($"  GFX{i:X2} @ ${gfx.GfxPointers[i]:X6}");
                        }
                    }

                    if (ImGui.CollapsingHeader("ExGFX Pointers:"))
                    {
                        for (int i = 0; i < 0x80; i++)
                        {
                            ImGui.Text($"  ExGFX{(i + 0x80):X2} @ ${gfx.ExGfxPointers[i]:X6}");
                        }
                    }
                    
                    if (ImGui.CollapsingHeader("SuperExGFX Pointers:"))
                    {
                        for (int i = 0; i < 0xF00; i++)
                        {
                            ImGui.Text($"  ExGFX{i + 0x100:X2} @ ${gfx.SuperExGfxPointers[i]:X6}");
                        }
                    }
                }

                ImGui.End();
        }
    }
}