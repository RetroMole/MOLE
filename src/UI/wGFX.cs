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
            ImGui.Text("2BPP Rendering test");

            byte[,] test = BPP.Test2bppPlanar(new byte[] { // link head sprite
                0x00, 0x7E,
                0x00, 0xFF,
                0x7E, 0x81,
                0xFF, 0x00,
                0xDB, 0x7E,
                0xFF, 0x5A,
                0xFF, 0xFF,
                0x7E, 0x66
            });
            var pal = new uint[] // test palette
            {
                0x00000000,
                0xFF30BE6A,
                0xFF3B568F,
                0XFF9AC3EE
            };
            var draw_list = ImGui.GetWindowDrawList();
            var p = ImGui.GetCursorScreenPos();
            var sz = 16;
            for (int i = 0; i < test.Length; i++)
            {
                var x = (i / 8) * sz;
                var y = (i % 8) * sz;
                draw_list.AddRectFilled(
                    new Num.Vector2(p.X + x, p.Y + y),
                    new Num.Vector2(p.X + x + sz, p.Y + y + sz),
                    pal[test[i % 8, i / 8]]
                );
            }
            ImGui.Dummy(new Num.Vector2(sz * 8, sz * 8));
            ImGui.Separator();

            if (gfx != null)
            {
                ImGui.Text("Raw GFX data from open ROM:");
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
