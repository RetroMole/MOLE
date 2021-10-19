using ImGuiNET;
using System;
using System.Collections.Generic;
using Num = System.Numerics;

namespace MOLE
{
    public partial class UI 
    {
        public static void wGFX()
        {
            ImGui.SetNextWindowSize(new Num.Vector2(600, 900), ImGuiCond.FirstUseEver);
            ImGui.Begin("GFX");

            if (gfx != null)
            {
                for (int i = 0; i < gfx.Gfx0.Length / 32; i++)
                {
                    string s = "\n";
                    for (int j = 0; j < 32; j++)
                    {
                        s += string.Format("  {0:X2}", gfx.Gfx0[(j * (gfx.Gfx0.Length / 32)) + i]);
                    }
                    ImGui.Text(s);
                }
                
            }
        }
    }
}
