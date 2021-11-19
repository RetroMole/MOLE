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
    public class GfxEditor : Window
    {
        int CurrPal = 0;
        int CurrGfx = 0;
        int CurrFrmt = 0;
        Pal Pal;
        public override void Draw(Project.UiData data, Dictionary<string, Window> windows)
        {
            if (!ShouldDraw || !data.Progress.Loaded) return;

            ImGui.SetNextWindowSize(new Num.Vector2(600, 900), ImGuiCond.FirstUseEver);
            ImGui.Begin("GFX Editor");

            //for (int j = 0; j < data.Project.Gfx.DecompressedGfx[CurrGfx].Length / chrSize; j++)
            //{
            //    if (j % 16 != 0) ImGui.SameLine(0f, sp);
            //    byte[,] chr = Bpp.GetChr(data.Project.Gfx.DecompressedGfx[CurrGfx], j, (Gfx.Format)CurrFrmt);
            //    var p = ImGui.GetCursorScreenPos();

            //    for (int k = 0; k < chr.Length; k++)
            //    {
            //        var x = k / 8 * sz;
            //        var y = k % 8 * sz;
            //        var c = chr[k % 8, k / 8];
            //        drawList.AddRectFilled(
            //            new Num.Vector2(p.X + x, p.Y + y),
            //            new Num.Vector2(p.X + x + sz, p.Y + y + sz),
            //            Pal[c]
            //        );
            //    }

            //    ImGui.Dummy(new Num.Vector2(sz * 8, sz * 8));
            //}

            ImGui.End();
        }
    }
}
