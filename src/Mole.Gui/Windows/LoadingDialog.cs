using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Mole.Shared.Util;

namespace Mole.Gui.Windows
{
    public class LoadingDialog : Window
    {
        public override void Draw(Project.UiData data, List<Window> windows)
        {
            if (data.Progress.Loaded
                || !data.Progress.Working) return;
            
            if (!ImGui.IsPopupOpen("Loading")) 
                ImGui.OpenPopup("Loading");

            if (ImGui.IsPopupOpen("Loading"))
            {
                ImGui.SetWindowSize(new Vector2());
                ImGui.SetNextWindowPos(ImGui.GetMainViewport().Size / 2, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
                if (ImGui.BeginPopupModal("Loading", ref data.Progress.Working, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    if (data.Progress.ShowException) {
                        ImGui.Text("Exception was thrown!");
                        ImGui.Separator();
                        ImGui.Text(data.Progress.Exception.ToString());
                    } else {
                        ImGui.Text("Project is loading, please wait...");
                        ImGui.Separator();
                        ImGui.Text(data.Progress.State.ToString());
                        ImGui.Text($"Progress: {data.Progress.CurrentProgress} / {data.Progress.MaxProgress}");
                    }
                    
                    ImGui.SetItemDefaultFocus();
                    ImGui.SameLine();
                    ImGui.EndPopup();
                }
            }
        }
    }
}