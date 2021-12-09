using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using ImGuiNET;
using Mole.Shared;
using Mole.Shared.Util;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// Shows wGFX
    /// </summary>
    public class GfxEditor : Window
    {
        int PrevPal, CurrPal, CurrGfx, PrevGfx = 0;
        int CurrFmt;
        int PrevFmt;
        Pal Pal = new(new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        byte[,] RenderableData = new byte[128,128];
        bool IsSExGFXOpen = true;
        Vector2 sz = new(4,4);

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
                                if (ImGui.Button("Expand"))
                                {
                                    IsSExGFXOpen = false;
                                    data.Project.Rom.Expand(2097152);
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.EndPopup();
                            }
                        }
                    }
                    ImGui.EndTabBar();
                }

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
                Enumerable.Range(0, Pal.Length).Select(x => $"Palette {x:X2}").ToArray(),
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

            // DRAW GFX
            var drawlist = ImGui.GetWindowDrawList();
            for (int i = 0; i < RenderableData.GetLength(0); i++)
            {
                for (int j = 0; j < RenderableData.GetLength(1); j++)
                {
                    var cursor = ImGui.GetCursorScreenPos();
                    drawlist.AddRectFilled(
                        cursor + (sz * new Vector2(i, j)),
                        cursor + (sz * new Vector2(i, j)) + sz,
                        Pal.ABGR[RenderableData[i, j]]
                    );
                }
            }
            ImGui.Dummy(sz * new Vector2(RenderableData.GetLength(0), RenderableData.GetLength(1)));
        }

        public void Draw_GFX(Project.UiData data, Dictionary<string, Window> windows)
        {
            if (Widgets.ComboWithArrows("##GFXFileCombo", "GFX",
                data.Project.Gfx.Decompressed.Select((x, i) => $"GFX{i:X2}").ToArray(),
                ref CurrGfx, ref PrevGfx
            ))
            {
                Update_RenderableData(data.Project.Gfx.Format(CurrGfx).FromRawToGfx(data.Project.Gfx.Decompressed[CurrGfx]));
            }

            Pal = data.Project.CGRam.GetPal(CurrPal, data.Project.Gfx.Format(CurrGfx).PalSize);
        }

        public void Draw_ExGFX(Project.UiData data, Dictionary<string, Window> windows)
        {
            if (Widgets.ComboWithArrows("##ExGFXFileCombo", "GFX",
                data.Project.ExGfx.Decompressed.Select((x, i) => $"GFX{i+0x80:X2}").ToArray(),
                ref CurrGfx, ref PrevGfx
            ))
            {
                Update_RenderableData(data.Project.ExGfx.Format(CurrGfx).FromRawToGfx(data.Project.ExGfx.Decompressed[CurrGfx]));
            }

            Pal = data.Project.CGRam.GetPal(CurrPal, data.Project.ExGfx.Format(CurrGfx).PalSize);
        }

        public void Draw_SuperExGFX(Project.UiData data, Dictionary<string, Window> windows)
        {
            if (Widgets.ComboWithArrows("##ExGFXFileCombo", "GFX",
                data.Project.SuperExGfx.Decompressed.Select((x, i) => $"GFX{i + 0x100:X2}").ToArray(),
                ref CurrGfx, ref PrevGfx
            ))
            {
                Update_RenderableData(data.Project.SuperExGfx.Format(CurrGfx).FromRawToGfx(data.Project.SuperExGfx.Decompressed[CurrGfx]));
            }

            Pal = data.Project.CGRam.GetPal(CurrPal, data.Project.SuperExGfx.Format(CurrGfx).PalSize);
        }

        public void Update_RenderableData(byte[][,] chunks)
        {
            RenderableData = new byte[128, 128];

            int x = 0;
            int y = 0;
            // Loop over chunks
            for (int i = 0; i < chunks.Length; i++)
            {
                // Loop over 8 rows vertically
                for (int j = 0; j < 8; j++)
                {
                    // Loop over 8 px horizontally
                    for (int k = 0; k < 8; k++)
                    {
                        if (y == 128)
                        {
                            y = 0;
                            x += 8;
                        }

                        RenderableData[y, x] = chunks[i][k, j];
                        x++;
                    }
                    x -= 8;
                    y++;
                }
            }
        }
    }
}
