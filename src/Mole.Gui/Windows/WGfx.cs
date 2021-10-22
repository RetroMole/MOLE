using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Mole.Shared;
using Mole.Shared.Util;
using Serilog.Core;
using Num = System.Numerics;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// Shows wGFX
    /// </summary>
    public class WGfx : Window
    {
        public override void Draw(Ui.UiData data, List<Window> windows)
        {
            if (!ShouldDraw || data.Gfx == null) return;
            
            ImGui.SetNextWindowSize(new Num.Vector2(600, 900), ImGuiCond.FirstUseEver);
            ImGui.Begin("GFX Info");
            
            if (data.Gfx is not { Loaded: true }) {
                ImGui.Text("Loading GFX, please wait...");
                ImGui.Separator();
                ImGui.Text(data.Gfx.State.ToString());
                ImGui.Text($"Progress: 0x{data.Gfx.Progress:X2} / 0x{data.Gfx.MaxProgress:X2}");
                ImGui.End();
                return;
            }
            ImGui.Text("GFX");
            ImGui.Text($"Pointers: 0x{data.Gfx.GfxPointers.Length:X2}");
            ImGui.Text($"Decompressed: 0x{data.Gfx.DecompressedGfx.Length:X2}");
            ImGui.Separator();
            ImGui.Text("ExGFX");
            ImGui.Text($"Pointers: 0x{data.Gfx.ExGfxPointers.Length:X2}");
            ImGui.Text($"Decompressed: 0x{data.Gfx.DecompressedExGfx.Length:X2}");
            ImGui.Separator();
            ImGui.Text("SuperExGFX");
            ImGui.Text($"Pointers: 0x{data.Gfx.SuperExGfxPointers.Length:X2}");
            ImGui.Text($"Supported: {data.Gfx.SuperExGfxSupported}");
            if (data.Gfx.SuperExGfxSupported)
                ImGui.Text($"Decompressed: 0x{data.Gfx.DecompressedSuperExGfx.Length:X2}");
            ImGui.Separator();
            ImGui.Text("Rendering");

            var pal = new uint[] {
                0xFF733900,
                0xFF942994,
                0xFFFF5AA5,
                0xFF9CC6DE,
                0xFF564589,
                0xFF5A879A,
                0xFF4F86DC,
                0xFF5986ED,
                0xFFE90E5D,
                0xFFE1D26F,
                0xFF25C2F2,
                0xFF5721C1,
                0xFFEDEF6E,
                0xFFC123F5,
                0xFFA1FCF6
            };
            var sz = 6;
            var sp = sz/4;
            var drawList = ImGui.GetWindowDrawList();
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Num.Vector2(0, sp));
            for (int i = 0; i < data.Gfx.DecompressedGfx.Length; i++)
            {
                if (i % 16 == 0)
                    ImGui.Dummy(new Num.Vector2(0,0));

                ImGui.SameLine(0f, sp);
                byte[,] chr = Bpp.BppPlanar2(data.Gfx.DecompressedGfx[i]);
                var p = ImGui.GetCursorScreenPos();

                for (int k = 0; k < chr.Length; k++)
                {
                    var x = k / 8 * sz;
                    var y = k % 8 * sz;
                    LoggerEntry.Logger.Information($"0x{i:X2}, {k % 8} {k / 8}, {k}, {chr.GetLength(0)} {chr.GetLength(1)}");
                    var c = chr[k / 8, k % 8];
                    if (c >= pal.Length)
                    {
                        drawList.AddRectFilled(
                            new Num.Vector2(p.X + x, p.Y + y),
                            new Num.Vector2(p.X + x + sz, p.Y + y + sz),
                            0xFFA1FCF6
                        );
                        return;
                    }
                    drawList.AddRectFilled(
                        new Num.Vector2(p.X + x, p.Y + y),
                        new Num.Vector2(p.X + x + sz, p.Y + y + sz),
                        pal[c]
                    );
                }

                ImGui.Dummy(new Num.Vector2(sz * 8, sz * 8));
            }

            ImGui.PopStyleVar();
            ImGui.End();
        }
    }
}
