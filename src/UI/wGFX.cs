using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using Num = System.Numerics;

namespace MOLE
{
    public partial class UI 
    {
        public static void wGFX()
        {
            ImGui.SetNextWindowSize(new Num.Vector2(600, 900), ImGuiCond.FirstUseEver);

            if (ImGui.Begin("GFX"))
            {
                if (gfx != null)
                {
                    ImGui.Text("2BPP Rendering test");
                    var pal = new uint[] // test palette
                    {
                        0xFF733900,
                        0xFF942994,
                        0xFFFF5AA5,
                        0XFF9CC6DE
                    };
                    var sz = 6;
                    var sp = sz/4;
                    var draw_list = ImGui.GetWindowDrawList();
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Num.Vector2(0, sp)); // Vertical spacing
                    for (int i = 0; i < 8; i++)
                    {
                        ImGui.Dummy(new Num.Vector2(0,0)); // Break from SameLine
                        for (int j = 0; j < 16; j++)
                        {
                            ImGui.SameLine(0f,sp); // Draw 16 items on the same line and handle horizontal spacing

                            byte[,] chr = BPP.Test2bppPlanar(gfx.dGFX[0x2A].Skip((j*16)+(i*256)).Take(16).ToArray());
                            var p = ImGui.GetCursorScreenPos();

                            for (int k = 0; k < chr.Length; k++)
                            {
                                var x = (k / 8) * sz;
                                var y = (k % 8) * sz;
                                draw_list.AddRectFilled(
                                    new Num.Vector2(p.X + x, p.Y + y),
                                    new Num.Vector2(p.X + x + sz, p.Y + y + sz),
                                    pal[chr[k % 8, k / 8]]
                                );
                            }

                            ImGui.Dummy(new Num.Vector2(sz * 8, sz * 8));
                        }
                    }
                    ImGui.PopStyleVar();
                    ImGui.Separator();


                    ImGui.Text("Raw GFX data from open ROM:");
                    for (int gi = 0; gi < gfx.dGFX.Length; gi++)
                    {
                        if (ImGui.CollapsingHeader(String.Format("GFX{0:X2}", gi)))
                        {
                            var g = gfx.dGFX[gi];
                            string s = "";
                            for (int i = 0; i < g.Length / 32; i++)
                            {
                                s += "\n";
                                for (int j = 0; j < 32; j++)
                                {
                                    s += string.Format("  {0:X2}", g[(j * (g.Length / 32)) + i]);
                                }
                            }
                            ImGui.Text(s);
                        }
                    }
                }
            }
        }
    }
}
