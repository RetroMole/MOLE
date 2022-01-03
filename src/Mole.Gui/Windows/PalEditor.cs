using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Mole.Shared;
using Mole.Shared.Util;
using Serilog;

namespace Mole.Gui.Windows
{
    public class PalEditor : WindowBase
    {
        public int PalZoom = 16;
        public IntPtr? TextureID;
        public uint[,] TextureData = new uint[16,16];
        public Vector3 col = new();
        public override void Draw(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            if (!ShouldDraw || !data.Progress.Loaded) return;

            ImGui.SetNextWindowSize(new Vector2(420, 69), ImGuiCond.FirstUseEver);
            ImGui.Begin("Palette Editor");

            if (ImGui.Button("Save")) Log.Information("Save pal");
            ImGui.SameLine(); if (ImGui.Button("Reload")) Log.Information("Reload pal");
            ImGui.SameLine(); if (ImGui.Button("Undo")) Log.Information("Undo pal");
            ImGui.SameLine(); if (ImGui.Button("Redo")) Log.Information("Redo pal");
            ImGui.SameLine(); if (ImGui.Button("-")) PalZoom -= PalZoom > 8 ? 4 : 0;
            ImGui.SameLine(); ImGui.Text($"x{PalZoom / 4}");
            ImGui.SameLine(); if (ImGui.Button("+")) PalZoom += PalZoom < 64 ? 4 : 0;

            if (ImGui.BeginTabBar("Palette Editor"))
            {
                if (ImGui.BeginTabItem("Level Palettes"))
                {
                    ImGui.Columns(2,"##PalSeparator",false);
                    ImGui.SetColumnWidth(0, PalZoom * 17);

                    TextureData = new uint[16, 16];
                    for (int i = 0; i < TextureData.Length; i++)
                    {
                        TextureData[i / 16, i % 16] = data.Project.CGRam.GetPal(0, 256).ABGR[i];
                    }

                    if (!TextureID.HasValue)
                        TextureID = Ui.renderer.BindTexture(TextureData);
                    else
                        Ui.renderer.UpdateTexture((IntPtr)TextureID,TextureData);

                    var pos = ImGui.GetCursorScreenPos();
                    ImGui.Image((IntPtr)TextureID, new Vector2(16, 16) * PalZoom);
                    if (ImGui.IsItemClicked())
                    {
                        int x = (int)((ImGui.GetIO().MousePos.X - pos.X) / PalZoom);
                        int y = (int)((ImGui.GetIO().MousePos.Y - pos.Y) / PalZoom);
                        col = new Vector3(TextureData[y, x] & 0xFF, (TextureData[y, x] >> 8) & 0xFF, (TextureData[y, x] >> 16) & 0xFF);
                    }

                    ImGui.NextColumn();
                    if (Widgets.ComboWithArrows("FG", "FG",
                        new string[] {
                            "FG Palette 0",
                            "FG Palette 1",
                            "FG Palette 2",
                            "FG Palette 3",
                            "FG Palette 4",
                            "FG Palette 5",
                            "FG Palette 6",
                            "FG Palette 7"
                        },
                        ref data.Project.CGRam.CurrentFG,
                        ref data.Project.CGRam.PrevFG
                    )) data.Project.CGRam.GenerateLevelCGRam(ref data.Project.Rom);

                    if (Widgets.ComboWithArrows("BG", "BG",
                        new string[] {
                            "BG Palette 0",
                            "BG Palette 1",
                            "BG Palette 2",
                            "BG Palette 3",
                            "BG Palette 4",
                            "BG Palette 5",
                            "BG Palette 6",
                            "BG Palette 7"
                        },
                        ref data.Project.CGRam.CurrentBG,
                        ref data.Project.CGRam.PrevBG
                    )) data.Project.CGRam.GenerateLevelCGRam(ref data.Project.Rom);

                    if (Widgets.ComboWithArrows("Spr", "Sprite",
                        new string[]
                        {
                            "Sprite Palette 0",
                            "Sprite Palette 1",
                            "Sprite Palette 2",
                            "Sprite Palette 3",
                            "Sprite Palette 4",
                            "Sprite Palette 5",
                            "Sprite Palette 6",
                            "Sprite Palette 7",
                        },
                        ref data.Project.CGRam.CurrentSpr,
                        ref data.Project.CGRam.PrevSpr
                    )) data.Project.CGRam.GenerateLevelCGRam(ref data.Project.Rom);
                    ImGui.Text("Color:"); ImGui.SameLine();
                    ImGui.PushItemWidth(ImGui.CalcItemWidth() - ImGui.GetStyle().ItemInnerSpacing.X * 2.0f - ImGui.GetFrameHeight() * 2.0f - 6 * 7.0f);
                    ImGui.ColorEdit3("Color", ref col);
                    ImGui.PopItemWidth();

                    ImGui.Columns(1);
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Overworld Palettes"))
                {
                    ImGui.Text("To-Do: OW Palettes, submap-specific palettes, special world passed submap specific palettes, etc...");
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Misc Palettes"))
                {
                    ImGui.Text("To-Do: Player palettes, koopalings palettes, bowser palettes, animated palettes,  magikoopa palette, big boo boss palette, etc...");
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }

            ImGui.End();
        }
    }
}
