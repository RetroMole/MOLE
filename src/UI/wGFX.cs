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
                for (int gi = 0; gi < gfx.dGFX.Length; gi++)
                {
                    var g = gfx.dGFX[gi];
                    ImGui.Text("GFX" + gi);
                    for (int i = 0; i < g.Length / 32; i++)
                    {
                        string s = "\n";
                        for (int j = 0; j < 32; j++)
                        {
                            s += string.Format("  {0:X2}", g[(j * (g.Length / 32)) + i]);
                        }
                        ImGui.Text(s);
                    }
                    ImGui.Separator();
                }
            }
        }
    }
}
