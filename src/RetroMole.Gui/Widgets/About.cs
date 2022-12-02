using ImGuiNET;
using QuickImGuiNET;
using Spdx;

namespace RetroMole;

public partial class Gui
{
    public static partial class Widgets
    {
        public class About : Widget
        {
            private readonly QuickImGuiNET.Widgets.Link _link; //Re-usable object

            private readonly List<(string, string, Version, SpdxLicense)> _libraries = new()
            {
                ("RPGHacker/asar", "https://github.com/RPGHacker/asar", new(1, 81),
                    SpdxLicense.GetById("LGPL-3.0-or-later") ?? throw new Exception("oopsie")),
                ("Vawlpe/QuickImGuiNET", "https://github.com/Vawlpe/QuickImGuiNET", new(0, 1),
                    SpdxLicense.GetById("GPL-3.0-or-later") ?? throw new Exception("oopsie")),
                ("mellinoe/ImGui.NET", "https://github.com/mellinoe/ImGui.NET", new(1, 87, 3),
                    SpdxLicense.GetById("MIT") ?? throw new Exception("oopsie")),
                ("cimgui/cimgui", "https://github.com/cimgui/cimgui", new(1, 87, 3),
                    SpdxLicense.GetById("MIT") ?? throw new Exception("oopsie")),
                ("ocornut/imgui", "https://github.com/ocornut/imgui", new(1, 87, 3),
                    SpdxLicense.GetById("MIT") ?? throw new Exception("oopsie")),
                ("serilog/serilog", "https://github.com/serilog/serilog", new(1, 12, 1),
                    SpdxLicense.GetById("Apache-2.0") ?? throw new Exception("oopsie")),
                ("serilog/serilog-sinks-console", "https://github.com/serilog/serilog-sinks-console", new(4, 1, 1),
                    SpdxLicense.GetById("Apache-2.0") ?? throw new Exception("oopsie")),
                ("serilog/serilog-sinks-file", "https://github.com/serilog/serilog-sinks-file", new(5, 0, 1),
                    SpdxLicense.GetById("Apache-2.0") ?? throw new Exception("oopsie")),
                ("patrik/Spdx", "https://www.nuget.org/packages/Spdx", new(0, 8, 0),
                    SpdxLicense.GetById("MIT") ?? throw new Exception("oopsie")),
                ("dezhidiki/Tommy", "https://github.com/dezhidiki/Tommy", new(3, 1, 2),
                    SpdxLicense.GetById("MIT") ?? throw new Exception("oopsie")),
                ("icsharpcode/SharpZipLib", "https://github.com/icsharpcode/SharpZipLib", new(1, 4, 1),
                    SpdxLicense.GetById("MIT") ?? throw new Exception("oopsie")),
                ("SixLabors/ImageSharp", "https://github.com/SixLabors/ImageSharp", new(2, 1, 3),
                    SpdxLicense.GetById("Apache-2.0") ?? throw new Exception("oopsie"))
            };

            public About(Backend backend, string? Name = null, bool AutoRegister = true) : base(backend, Name,
                AutoRegister)
            {
                // Link
                _link = new(backend, $"{Name}_Link", false);
                backend.Events["widgetReg"].Children.Add($"{Name}_LicenseLink",
                    new Event(new Dictionary<string, Event>
                    {
                        { "open", new Event() },
                        { "close", new Event() },
                        { "toggle", new Event() }
                    }));
            }

            public override void RenderContent()
            {
                ImGui.TextWrapped(
                    "Open-source plugin-based cross-platform retro-game level-editor for various 8/16-bit console games "
                    + "with support for editing common graphics and map formats, applying/creating IPS/BPS patches, "
                    + "applying assembly patches, with an IDE-like interface, custom build system and basic debugger, "
                    + "for experienced or fresh ROM-hackers, players, and developers alike~!");

                if (ImGui.CollapsingHeader("LICENSE"))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_00CCFF);
                    ImGui.TextWrapped(
                        "RetroMole is an open source ROM editor and does not endorse any form of Copyright infringing activities including distribution of ROMs with or without monetary incentive");
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_96FF9F);
                    ImGui.TextWrapped(
                        "Copyright (C) 2019-2022 - Hazel (Vawlpe), RetroMole org members and contributors");
                    ImGui.PopStyleColor(2);

                    ImGui.TextWrapped(
                        "This program is free software: you can redistribute it and / or modify it under the terms of the " +
                        "GNU General Public License as published by the Free Software Foundation, " +
                        "either version 3 of the License, or (at your option) any later version." +
                        "\nThis program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; " +
                        "without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. " +
                        "See the GNU General Public License for more details. " +
                        "You should have received a copy of the GNU General Public License along with this program." +
                        "\nIf not, see");

                    ImGui.SetCursorPos(new(
                        ImGui.GetCursorPosX() + ImGui.CalcTextSize("If not, see ").X,
                        ImGui.GetCursorPosY() - ImGui.GetTextLineHeightWithSpacing()));

                    _link.DisplayText = "GPL-3.0-or-later";
                    _link.Url = "https://spdx.org/licenses/GPL-3.0-or-later";
                    _link.Render();
                }

                if (ImGui.CollapsingHeader("Version"))
                {
                    _link.DisplayText = "https://github.com/RetroMole/";
                    _link.Url = "https://github.com/RetroMole/";
                    _link.Render();
                    ImGui.Text("RetroMole Launch v0.1");
                    ImGui.Text("RetroMole GUI    v0.1");
                    ImGui.Text("RetroMole Core   v0.1");
                }

                if (ImGui.CollapsingHeader("Libraries"))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_00CCFF);
                    ImGui.TextWrapped(
                        "All of the following libraries are licensed under their respective Open Source Software licenses:");
                    ImGui.PopStyleColor();

                    if (ImGui.BeginTable("AboutLibrariesTbl", 3))
                    {
                        foreach (var lib in _libraries)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            _link.DisplayText = lib.Item1;
                            _link.Url = lib.Item2;
                            _link.Render();
                            ImGui.TableNextColumn();
                            ImGui.Text($"v{lib.Item3}");
                            ImGui.TableNextColumn();
                            _link.DisplayText = lib.Item4.Id;
                            _link.Url = $"https://spdx.org/licenses/{lib.Item4.Id}";
                            _link.Render();
                        }

                        ImGui.EndTable();
                    }
                }

                if (ImGui.CollapsingHeader("Resources"))
                {
                    ImGui.TextWrapped("" +
                                      "RetroMole Icon   by Tob#3035\n" +
                                      "RetroMole Splash by Vawlpe\n"
                    );
                }
            }
        }
    }
}