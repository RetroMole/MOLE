using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Mole.Shared;
namespace Mole.Gui.Windows
{
    public class PalEditor : Window
    {
        public int PalZoom = 16;
        public override void Draw(Ui.UiData data, List<Window> windows)
        {
            if (!ShouldDraw) return;

            ImGui.SetNextWindowSize(new Vector2(420, 69), ImGuiCond.FirstUseEver);
            ImGui.Begin("Palette Editor");

            if (data.CGRam is not { Loaded: true }) {
                ImGui.Text("Loading Palettes, please wait..");
                ImGui.End();
                return;
            }
            if (ImGui.Button("Save")) LoggerEntry.Logger.Information("Save pal");
            ImGui.SameLine(); if (ImGui.Button("Reload")) LoggerEntry.Logger.Information("Reload pal");
            ImGui.SameLine(); if (ImGui.Button("Undo")) LoggerEntry.Logger.Information("Undo pal");
            ImGui.SameLine(); if (ImGui.Button("Redo")) LoggerEntry.Logger.Information("Redo pal");
            ImGui.SameLine(); if (ImGui.Button("-")) PalZoom -= PalZoom > 8 ? 4 : 0;
            ImGui.SameLine(); ImGui.Text($"x{PalZoom / 4}");
            ImGui.SameLine(); if (ImGui.Button("+")) PalZoom += PalZoom < 64 ? 4 : 0;

            var sp = PalZoom / 4;

            if (ImGui.BeginTabBar("Palette Editor"))
            {
                if (ImGui.BeginTabItem("Level Palettes"))
                {
                    ImGui.Columns(2);
                    ImGui.SetColumnWidth(0, PalZoom * 17);
                    ImGui.Separator();
                    var drawList = ImGui.GetWindowDrawList();
                    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, sp));

                    for (int r = 0; r < 16; r++)
                    {
                        for (int c = 0; c < 16; c++)
                        {
                            var p = ImGui.GetCursorScreenPos();
                            var x = p.X + (c * PalZoom);
                            var y = p.Y + (r * PalZoom);

                            drawList.AddRectFilled(
                                new Vector2(x, y),
                                new Vector2(x + PalZoom, y + PalZoom),
                                Pal.SnesToABGR(data.CGRam[(r * 16) + c])
                            );
                            if (c != 15) ImGui.SameLine();
                        }
                    }

                    ImGui.Dummy(new Vector2(PalZoom * 16, PalZoom * 16f));
                    ImGui.PopStyleVar();

                    ImGui.NextColumn();
                    if (Widgets.ComboWithArrows.New("FG", "FG",
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
                        ref data.CGRam.CurrentFG
                    )) CGRam.GenerateLevelCGRam(ref data.CGRam, ref data.Rom);

                    if (Widgets.ComboWithArrows.New("BG", "BG",
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
                        ref data.CGRam.CurrentBG
                    )) CGRam.GenerateLevelCGRam(ref data.CGRam, ref data.Rom);

                    if (Widgets.ComboWithArrows.New("Spr", "Sprite",
                        new string[]
                        {
                            "Sprie Palette 0",
                            "Sprie Palette 1",
                            "Sprie Palette 2",
                            "Sprie Palette 3",
                            "Sprie Palette 4",
                            "Sprie Palette 5",
                            "Sprie Palette 6",
                            "Sprie Palette 7",
                        },
                        ref data.CGRam.CurrentSpr
                    )) CGRam.GenerateLevelCGRam(ref data.CGRam, ref data.Rom);

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
