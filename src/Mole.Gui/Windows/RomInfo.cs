using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using ImGuiNET;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// ROM information
    /// </summary>
    [SuppressMessage("ReSharper", "InterpolatedStringExpressionIsNotIFormattable")]
    public class RomInfo : Window
    {
        public override void Draw(Ui.UiData data, List<Window> windows)
        {
            if (!ShouldDraw) return;
            
            ImGui.SetNextWindowSize(new Vector2(420, 69), ImGuiCond.FirstUseEver);
            ImGui.Begin("ROM Info");
            
            if (data.Rom is not { Loaded: true }) {
                ImGui.Text("Loading ROM, please wait...");
                ImGui.End();
                return;
            }
             
            ImGui.Text($"ROM Filename: {data.Rom.FileName}"); 
            ImGui.Text($"ROM Path: {data.Rom.FilePath}");
            ImGui.Text($"Copier Header Size: {(data.Rom.Header != null ? data.Rom.Header.Length : "None"):X2}");
            ImGui.Separator(); 
            ImGui.Text("Internal ROM Header:");
            ImGui.Text($"  ROM Title: \"{data.Rom.Title}\""); 
            ImGui.Text($"  Mapping Mode: {(data.Rom.FastRom ? "FastROM" : "SlowROM")}, {data.Rom.Mapping}");
            ImGui.Text($"  ROM Size: {data.Rom.RomSize}kb"); 
            ImGui.Text($"  SRAM Size: {data.Rom.SramSize}kb");
            ImGui.Text($"  Region: {data.Rom.Region}");
            ImGui.Text($"  Developer ID: {data.Rom.DevId:X2}");
            ImGui.Text($"  Version: {data.Rom.Version}");
            ImGui.Text($"  Checksum: {data.Rom.Checksum:X4}");
            ImGui.Text($"  Checksum Complement: {data.Rom.ChecksumComplement:X4}");
            ImGui.End();
        }
    }
}