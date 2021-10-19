using ImGuiNET;
using System;
using System.Collections.Generic;
using Mole.Shared;
using Num = System.Numerics;

namespace Mole.Gui
{
    public static class WGfx
    {
        public static void Main(Gfx gfx)
        {
            ImGui.SetNextWindowSize(new Num.Vector2(600, 900), ImGuiCond.FirstUseEver);
            ImGui.Begin("GFX");

            if (gfx != null)
            {
                for (int i = 0; i < gfx.Gfx0.Length / 32; i++)
                {
                    string s = "\n";
                    for (int j = 0; j < 32; j++)
                        s += $"  {gfx.Gfx0[(j * (gfx.Gfx0.Length / 32)) + i]:X2}";
                    ImGui.Text(s);
                }
            }
        }
    }
}
