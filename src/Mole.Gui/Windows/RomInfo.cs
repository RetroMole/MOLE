using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Mole.Shared.Util;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// ROM information
    /// </summary>
    public class RomInfo : WindowBase
    {
        public override void Draw(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            if (!ShouldDraw || !data.Progress.Loaded) return;
            
            ImGui.SetNextWindowSize(new Vector2(420, 69), ImGuiCond.FirstUseEver);
            ImGui.Begin("ROM Info");
            ImGui.Text($"ROM Filename: {data.Project.Rom.FileName}"); 
            ImGui.Text($"ROM Path: {data.Project.Rom.FilePath}");
            ImGui.Text($"Copier Header Size: {(data.Project.Rom.Header != null ? data.Project.Rom.Header.Length : "None"):X2}");
            ImGui.Separator(); 
            ImGui.Text("Internal ROM Header:");
            ImGui.Text($"  ROM Title: \"{data.Project.Rom.Title}\""); 
            ImGui.Text($"  Mapping Mode: {(data.Project.Rom.FastRom ? "FastROM" : "SlowROM")}, {data.Project.Rom.Mapping}");
            ImGui.Text($"  ROM Size: {data.Project.Rom.RomSize}kb"); 
            ImGui.Text($"  SRAM Size: {data.Project.Rom.SramSize}kb");
            ImGui.Text($"  Region: {data.Project.Rom.Region}");
            ImGui.Text($"  Developer ID: {data.Project.Rom.DevId:X2}");
            ImGui.Text($"  Version: {data.Project.Rom.Version}");
            ImGui.Text($"  Checksum: {data.Project.Rom.Checksum:X4}");
            ImGui.Text($"  Checksum Complement: {data.Project.Rom.ChecksumComplement:X4}");
            ImGui.End();
        }
    }
}