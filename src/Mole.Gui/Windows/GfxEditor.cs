using System;
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
    public class GfxEditor : WindowBase
    {
        int PrevPal, CurrPal, CurrGfx, PrevGfx, CurrFmt, PrevFmt = 0; // Values for combos

        Shared.Graphics.FormatBase Fmt = Project.Formats[3]; // Default to 3bpp
        Pal Pal = new(new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        Shared.Graphics.GfxBase Gfx;

        uint[,] RenderableData = new uint[128,128];
        IntPtr? RenderTexture;
        bool IsSExGFXOpen = true;
        Vector2 sz = new(4,4);

        public override void Draw(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            if (!ShouldDraw || !data.Progress.Loaded) return;

            ImGui.SetNextWindowSize(new(600, 900), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("GFX Editor",ImGuiWindowFlags.MenuBar))
            {
                Draw_MenuBar();
                if (ImGui.BeginTabBar("##GFXTypeTabs"))
                {
                    if (ImGui.BeginTabItem("GFX"))
                    {
                        IsSExGFXOpen = true;
                        Gfx = data.Project.Gfx;
                        ImGui.Columns(2, "GfxEditorLayout", false);
                        Draw_Render();
                        ImGui.NextColumn();
                        Draw_Combos(data, windows);
                        ImGui.Columns(1);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("ExGFX"))
                    {
                        IsSExGFXOpen = true;
                        Gfx = data.Project.ExGfx;
                        ImGui.Columns(2, "GfxEditorLayout", false);
                        Draw_Render();
                        ImGui.NextColumn();
                        Draw_Combos(data, windows);
                        ImGui.Columns(1);
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("SuperExGfx",ref IsSExGFXOpen))
                    {
                        if (data.Project.SuperExGfxSupported)
                        {
                            Gfx = data.Project.SuperExGfx;
                            ImGui.Separator();
                            ImGui.Columns(2, "GfxEditorLayout", false);
                            Draw_Render();
                            ImGui.NextColumn();
                            Draw_Combos(data, windows);
                            ImGui.Columns(1);
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
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }
                ImGui.End();
            }
        }
        public void Draw_Render()
        {
            ImGui.SetColumnWidth(0, 132 * sz.X);
            if (RenderTexture.HasValue)
                ImGui.Image((IntPtr)RenderTexture, new Vector2(128,128) * sz);
        }

        public void Draw_MenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                if(ImGui.BeginMenu("OwO"))
                {
                    ImGui.MenuItem("UwU");
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
        }

        public void Draw_Combos(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            Widgets.ComboWithArrows("##PalComboGfx", "Palette",
                Enumerable.Range(0, Pal.Length).Select(x => $"Palette {x:X2}").ToArray(),
                ref CurrPal, ref PrevPal
            );

            if (Widgets.ComboWithArrows("##FormatComboGfx", "Format",
                Project.Formats.Values.Select(
                    x => Regex.Replace(
                        x.GetType().Name, "([0-9]?[A-Z])", " $1", RegexOptions.Compiled
                    ).Trim()
                ).ToArray(),
                ref CurrFmt, ref PrevFmt
            ))
            {
                Fmt = CurrFmt switch
                {
                    0 => Project.Formats[2],
                    2 => Project.Formats[4],
                    3 => Project.Formats[8],
                    4 => Project.Formats[73],
                    1 or _ => Project.Formats[3]
                };
            }

            if (Widgets.ComboWithArrows("##GFXFileCombo", "GFX",
                data.Project.Gfx.Decompressed.Select((x, i) => $"GFX{i:X2}").ToArray(),
                ref CurrGfx, ref PrevGfx
            ))
            {
                Fmt = Gfx.Format(CurrGfx);
            }

            if (CurrPal >= Fmt.PalSize)
                CurrPal = Fmt.PalSize - 1;
            try { Update_RenderableData(Fmt.FromRawToGfx(Gfx.Decompressed[CurrGfx])); } catch { }
            Pal = data.Project.CGRam.GetPal(CurrPal, Fmt.PalSize);
        }

        public void Update_RenderableData(byte[][,] chunks)
        {
            RenderableData = new uint[128, 128];
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

                        RenderableData[x, y] = Pal.ABGR[chunks[i][k, j]];

                        x++;
                    }
                    x -= 8;
                    y++;
                }
            }

            if (!RenderTexture.HasValue)
                RenderTexture = Ui.renderer.BindTexture(RenderableData);
            else
                Ui.renderer.UpdateTexture((IntPtr)RenderTexture, RenderableData);
        }
    }
}
