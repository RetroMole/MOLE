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
        int CurrPal = 0;
        int CurrGfx = 0;
        public override void Draw(Project.UiData data, List<Window> windows)
        {
            if (!ShouldDraw || !data.Progress.Loaded) return;
            
            ImGui.SetNextWindowSize(new Num.Vector2(600, 900), ImGuiCond.FirstUseEver);
            ImGui.Begin("GFX Editor");
            Widgets.ComboWithArrows.New("GFXPal", "Palette",
                new string[]
                {
                    "Palette 0",
                    "Palette 1",
                    "Palette 2",
                    "Palette 3",
                    "Palette 4",
                    "Palette 5",
                    "Palette 6",
                    "Palette 7",
                    "Palette 8",
                    "Palette 9",
                    "Palette A",
                    "Palette B",
                    "Palette C",
                    "Palette D",
                    "Palette E",
                    "Palette F",
                },
                ref CurrPal
            );
            Widgets.ComboWithArrows.New("GFXIndex", "Graphics",
                Enumerable.Range(0,0x34).Select(x => $"GFX{x:X2}").ToArray<string>(),
                ref CurrGfx
            );

            ImGui.Separator();

            var pal = data.Project.CGRam.Get16Pal(CurrPal).ABGR;
            var sz = 6;
            var sp = sz/4;
            var drawList = ImGui.GetWindowDrawList();
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Num.Vector2(0, sp));

            var chrSize = data.Project.Gfx.GfxFormats[CurrGfx] switch
            {
                Gfx.Format.Snes2Bpp => 16,
                Gfx.Format.Snes3Bpp or Gfx.Format.Mode73Bpp => 24,
                Gfx.Format.Snes4Bpp => 32,
                Gfx.Format.Snes8Bpp => 64,
                Gfx.Format.Ambiguous3or4Bpp or _ => data.Project.Gfx.DecompressedGfx[CurrGfx].Length == 0xC00 ? 24 : 32
            };
            for (int j = 0; j < data.Project.Gfx.DecompressedGfx[CurrGfx].Length / chrSize; j++)
            {
                if (j % 16 != 0) ImGui.SameLine(0f, sp);
                byte[,] chr = Bpp.GetChr(data.Project.Gfx.DecompressedGfx[CurrGfx], j, data.Project.Gfx.GfxFormats[CurrGfx]);
                var p = ImGui.GetCursorScreenPos();

                for (int k = 0; k < chr.Length; k++)
                {
                    var x = k / 8 * sz;
                    var y = k % 8 * sz;
                    var c = chr[k % 8, k / 8];
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
