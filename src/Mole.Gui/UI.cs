using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Mole.Shared;
using Num = System.Numerics;

namespace Mole.Gui
{
    [SuppressMessage("ReSharper", "PositionalPropertyUsedProblem")]
    public class Ui
    {
        private static ImGuiViewportPtr _viewport;
        private static ImGuiIOPtr _io;
        private static readonly List<Delegate> Windows = new()
        {
            new Action(() => WGfx.Main(_gfx))
        };

        private static bool _showFps = true;
        private static bool _showMousePos = true;
        private static bool _showAbout = false;
        private static bool _showDemo = false;
        private static bool _debugOpen;
        private static Rom _rom;
        private static Gfx _gfx;
        private static string _path = "";
        private static bool _filediag;

        public static void Draw()
        {
            _io = ImGui.GetIO();
            _viewport = ImGui.GetMainViewport();

            _debugOpen = _showFps || _showMousePos;

            // Main Menu Bar
            {
                if (ImGui.BeginMainMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Open ROM", "Ctrl+O"))
                            _filediag = true;
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Debug"))
                    {
                        ImGui.MenuItem("FPS", null, ref _showFps);
                        ImGui.MenuItem("Mouse Pos", null, ref _showMousePos);
                        ImGui.MenuItem("Demo Window", null, ref _showDemo);
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Help"))
                    {
                        ImGui.MenuItem("About", null, ref _showAbout);
                        ImGui.EndMenu();
                    }
                    
                    ImGui.EndMainMenuBar();
                }
            }

            // Test
            {
                ImGui.SetNextWindowSize(new Num.Vector2(420, 69), ImGuiCond.FirstUseEver);
                ImGui.Begin("UwU");

                // TheAirBlow note: fuck you
                ImGui.Text("Test Window says HenlOwO, try opening a ROM");
                ImGui.Separator();

                if (_rom != null)
                {
                    ImGui.Text($"ROM FileName: {_rom.FileName}");
                    ImGui.Text($"ROM Path: {_rom.FilePath}");
                    ImGui.Text($"Copier Header: 0x{(_rom.Header != null ? _rom.Header.Length : "None"):X2}");

                    ImGui.Separator();
                    ImGui.Text("Internal ROM Header:");
                    ImGui.Text($"  ROM Title: \"{_rom.Title}\"");
                    ImGui.Text($"  Mapping Mode: {(_rom.FastRom ? "FastROM" : "SlowROM")}, {_rom.Mapping}");
                    ImGui.Text($"  ROM Size: {_rom.RomSize}kb");
                    ImGui.Text($"  SRAM Size: {_rom.SramSize}kb");
                    ImGui.Text($"  Region: {_rom.Region}");
                    ImGui.Text($"  Developer ID: {_rom.DevId:X2}");
                    ImGui.Text($"  Version: {_rom.Version}");
                    ImGui.Text($"  Checksum: {_rom.Checksum:X4}");
                    ImGui.Text($"  Checksum Complement: {_rom.ChecksumComplement:X4}");

                    ImGui.Separator();

                    if (ImGui.CollapsingHeader("GFX Pointers:"))
                    {
                        for (int i = 0; i < 0x34; i++)
                        {
                            ImGui.Text($"  GFX{i:X2} @ ${_gfx.GfxPointers[i]:X6}");
                        }
                    }

                    if (ImGui.CollapsingHeader("ExGFX Pointers:"))
                    {
                        for (int i = 0; i < 0x80; i++)
                        {
                            ImGui.Text($"  ExGFX{(i + 0x80):X2} @ ${_gfx.ExGfxPointers[i]:X6}");
                        }
                    }
                    if (ImGui.CollapsingHeader("SuperExGFX Pointers:"))
                    {
                        for (int i = 0; i < 0xF00; i++)
                        {
                            ImGui.Text($"  ExGFX{i + 0x100:X2} @ ${_gfx.SuperExGfxPointers[i]:X6}");
                        }
                    }
                }

                ImGui.End();
            }

            if (_debugOpen) // Debug
            {
                ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav;
                const float pad = 10f;

                Num.Vector2 workPos = _viewport.Pos;
                Num.Vector2 workSize = _viewport.Pos;
                Num.Vector2 windowPos = new(workPos.X + workSize.X - pad, workPos.Y + workSize.Y - pad);

                ImGui.SetNextWindowPos(windowPos, ImGuiCond.Always, new Num.Vector2(1, 1));
                windowFlags |= ImGuiWindowFlags.NoMove;

                ImGui.SetNextWindowBgAlpha(0.35f);

                if (ImGui.Begin("Debug", ref _debugOpen, windowFlags))
                {
                    if (_showFps) { ImGui.Text($"{1000f / _io.Framerate:F3} ms/frame ({_io.Framerate:F1} FPS)"); }

                    if (_showMousePos)
                    {
                        if (ImGui.IsMousePosValid())
                            ImGui.Text($"Mouse Position: ({_io.MousePos.X},{_io.MousePos.Y})");
                        else
                            ImGui.Text("Mouse Position: <invalid>");
                    }

                    if (ImGui.BeginPopupContextWindow())
                    {
                        if (_debugOpen && ImGui.MenuItem("Close")) { _showFps = false; _showMousePos = false; }
                        ImGui.EndPopup();
                    }

                    ImGui.End();
                }
            }

            ImGui.SetNextWindowSize(new Num.Vector2(900, 400), ImGuiCond.FirstUseEver);
            if (_showAbout)
            {
                if (ImGui.Begin("About", ref _showAbout))
                {
                    ImGui.Text(Strings.Copyright);
                    ImGui.Separator();
                    ImGui.Text($"Mole Shared Version: {Strings.MoleSharedVersion}");
                    ImGui.Text($"    MOLE GUI Version {Strings.MoleSharedVersion}");
                    ImGui.Separator();
                    string libs = 
                        "Libraries:\n" +
                        "All of the following libraries are licensed under their respective Open Source Software licenses:\n";
                    foreach (var lib in Strings.Libraries)
                    {
                        libs += $"  {lib.Name + " v" + lib.Version,-25}{"| " + lib.Repo,-50}{"| " + lib.License}\n";
                    }
                    ImGui.Text(libs);
                }
            }

            if (_showDemo) ImGui.ShowDemoWindow(ref _showDemo);

            foreach (var w in Windows)
                w.DynamicInvoke();

            // File Dialog
            if (_filediag)
            {
                if (!ImGui.IsPopupOpen("RomOpen")) 
                    ImGui.OpenPopup("RomOpen");

                if (ImGui.IsPopupOpen("RomOpen"))
                {
                    ImGui.SetNextWindowPos(ImGui.GetMainViewport().Size / 2, ImGuiCond.Appearing, new Num.Vector2(0.5f, 0.5f));
                    if (ImGui.BeginPopupModal("RomOpen"))
                    {
                        if (ImGui.InputText("Path", ref _path,
                            500, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
                        {
                            ImGui.CloseCurrentPopup();
                            _filediag = false;
                            if (!File.Exists(_path)) {
                                LoggerEntry.Logger.Warning("Invalid path: {0}", _path);
                                return;
                            }
                            _rom = new Rom(_path);
                            _gfx = new Gfx(_rom);
                        }

                        if (ImGui.Button("Open"))
                        {
                            ImGui.CloseCurrentPopup();
                            _filediag = false;
                            if (!File.Exists(_path)) {
                                LoggerEntry.Logger.Warning("Invalid path: {0}", _path);
                                return;
                            }
                            _rom = new Rom(_path);
                            _gfx = new Gfx(_rom);
                        }

                        if (_filediag == false) return;
                        
                        ImGui.SetItemDefaultFocus();
                        ImGui.SameLine();
                        
                        if (ImGui.Button("Cancel"))
                        {
                            ImGui.CloseCurrentPopup();
                            _filediag = false;
                        }

                        ImGui.EndPopup();
                    }
                }
            }
        }
    }
}
