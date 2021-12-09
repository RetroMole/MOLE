using Mole.Shared.Util;
using System;
using System.Collections.Generic;
using ImGuiNET;
using System.Numerics;

namespace Mole.Gui.Dialogs
{
    public class Error : Window
    {
        public Exception e = new();
        public string Message =
            "Uh-oh! An unexpected error occured, usually we have checks for these, but it seems this one slipped past us!\n" +
            "If you'd like to help us fix it, please open an issue on github, including your logs,\n" +
            "your device specifications, and how you encountered this problem, Thank You~!";
        public string Message2 =
            "Alternatively, you can opt-in to automatically report these errors in the future.\n" +
            "This implies allowing us to collect your system information, configuration files, and error logs.\n" +
            "Note that personal information of any kind will NOT be collected,\n" +
            "if you'd like to read up on exactly what is being collected, please visit github.com/vawlpe/mole/wiki/Telemetry";
        public bool Unhandled;
        public bool Telemetry = App.Default.Telemetry;
        public override void Draw(Project.UiData data, Dictionary<string, Window> windows)
        {
            App.Default.Telemetry = Telemetry;
            App.Default.Save();
            if (!ShouldDraw)
            {
                if (ImGui.IsPopupOpen("Error"))
                    ImGui.CloseCurrentPopup();
                e = null;
                return;
            }
            if (!ImGui.IsPopupOpen("Error"))
                ImGui.OpenPopup("Error");
            ImGui.SetNextWindowSize(ImGui.GetMainViewport().Size,ImGuiCond.Always);
            ImGui.SetNextWindowPos(new Vector2(ImGui.GetMainViewport().Size.X/2, ImGui.GetMainViewport().Size.Y/2),ImGuiCond.Always, new Vector2(0.5f,0.5f));
            if (ImGui.BeginPopupModal("Error", ref ShouldDraw, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoMove))
            {
                ImGui.TextWrapped(Message);

                ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0095FF);
                ImGui.TextWrapped(Message2);
                ImGui.PopStyleColor();
                ImGui.Checkbox("Automatically Report Future Problems", ref Telemetry);

                if (ImGui.BeginChildFrame(0, new(0,-23), ImGuiWindowFlags.AlwaysAutoResize))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0000FF);
                    ImGui.TextWrapped(e.ToString());
                    ImGui.PopStyleColor();
                    ImGui.EndChildFrame();
                }

                ImGui.Button("Copy log path");
                ImGui.SameLine();
                ImGui.Button("Report Error");
                if (!Unhandled)
                {
                    ImGui.SameLine();
                    if (ImGui.Button("Continue"))
                        ShouldDraw = false;
                }

                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Text, 0xFF3030FF);
                if (ImGui.Button("Force Exit Mole")) Environment.Exit(Environment.ExitCode);
                ImGui.PopStyleColor();

                ImGui.EndPopup();
            }
        }
    }
}
