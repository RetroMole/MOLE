using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ImGuiNET;
using Mole.Shared;
using Mole.Shared.Util;
using Serilog.Core;
using System.Numerics;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// Shows wGFX
    /// </summary>
    public class GfxEditor : Window
    {
        int PrevPal, CurrPal, CurrGfx, PrevGfx = 0;
        int PalSize = 16;
        int CurrFmt;
        int PrevFmt;
        bool IsSExGFXOpen = true;
        Pal Pal;
        public override void Draw(Project.UiData data, Dictionary<string, Window> windows)
        {
            if (!ShouldDraw || !data.Progress.Loaded) return;

            ImGui.SetNextWindowSize(new(600, 900), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("GFX Editor"))
            {
                Draw_MenuBar(data, windows);
                if (ImGui.BeginTabBar("##GFXTypeTabs"))
                {
                    if (ImGui.BeginTabItem("GFX"))
                    {
                        IsSExGFXOpen = true;
                        Draw_GFX(data, windows);
                        Draw_Shared(data, windows);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("ExGFX"))
                    {
                        IsSExGFXOpen = true;
                        Draw_ExGFX(data, windows);
                        Draw_Shared(data, windows);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("SuperExGfx",ref IsSExGFXOpen))
                    {
                        if (data.Project.SuperExGfxSupported)
                        {
                            Draw_SuperExGFX(data, windows);
                            Draw_Shared(data, windows);
                            ImGui.EndTabItem();
                        }
                        else
                        {
                            ImGui.OpenPopup("SuperExGfx not supported##ExpandPromptSExGFX");
                            ImGui.SetNextWindowSize(new(350, 100));
                            if (ImGui.BeginPopupModal("SuperExGfx not supported##ExpandPromptSExGFX",ref IsSExGFXOpen,ImGuiWindowFlags.NoResize))
                            {
                                ImGui.TextWrapped("Whoops, it seems this ROM is not expanded yet, and therefore SuperExGfx can't be used...\n" +
                                    "Do you wish to expand the ROM to 2MiB?");
                                if (ImGui.Button("Cancel"))
                                {
                                    IsSExGFXOpen = false;
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.SameLine();
                                ImGui.Button("Expand");
                                ImGui.EndPopup();
                            }
                        }
                    }
                    ImGui.EndTabBar();
                }

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

        public void Draw_MenuBar(Project.UiData data, Dictionary<string, Window> windows)
        {
            if (ImGui.BeginMenuBar())
            {
                ImGui.MenuItem("OwO");
                ImGui.EndMenuBar();
            }
        }

        public void Draw_Shared(Project.UiData data, Dictionary<string, Window> windows)
        {
            Widgets.ComboWithArrows("##PalComboGfx", "Palette",
                Enumerable.Range(0, 256/PalSize).Select(x => $"Palette {x:X2}").ToArray(),
                ref CurrPal, ref PrevPal
            );
            Widgets.ComboWithArrows("##FormatComboGfx", "Format",
                Project.Formats.Values.Select(
                    x => Regex.Replace(
                        x.GetType().Name, "([0-9]?[A-Z])", " $1", RegexOptions.Compiled
                    ).Trim()
                ).ToArray(),
                ref CurrFmt, ref PrevFmt
            );
        }

        public void Draw_GFX(Project.UiData data, Dictionary<string, Window> windows)
        {
            if (Widgets.ComboWithArrows("##GFXFileCombo", "GFX",
                data.Project.Gfx.Decompressed.Select((x,i) => $"GFX{i:X2}").ToArray(),
                ref CurrGfx, ref PrevGfx
            ))
            {
                CurrFmt = data.Project.Gfx.Decompressed[CurrGfx].Item2 switch
                {
                    2 => 0,
                    4 => 2,
                    8 => 3,
                    73 => 4,
                    3 or _ => 1
                };
            }
        }

        public void Draw_ExGFX(Project.UiData data, Dictionary<string, Window> windows)
        {
            Widgets.ComboWithArrows("##ExGFXFileCombo", "GFX",
                data.Project.ExGfx.Decompressed.Select((x, i) => $"GFX{i+0x80:X2}").ToArray(),
                ref CurrGfx, ref PrevGfx
            );
        }

        public void Draw_SuperExGFX(Project.UiData data, Dictionary<string, Window> windows)
        {
            Widgets.ComboWithArrows("##ExGFXFileCombo", "GFX",
                data.Project.SuperExGfx.Decompressed.Select((x, i) => $"GFX{i + 0x100:X2}").ToArray(),
                ref CurrGfx, ref PrevGfx
            );
        }
    }
}
