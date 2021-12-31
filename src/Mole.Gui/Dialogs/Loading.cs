using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Mole.Shared.Util;

namespace Mole.Gui.Dialogs
{
    public class Loading : WindowBase
    {
        public override void Draw(Project.UiData data, Dictionary<string,WindowBase> windows)
        {
            if (data.Progress.Loaded
                || !data.Progress.Working) return;
            
            if (!ImGui.IsPopupOpen("Loading##DialogLoading")) 
                ImGui.OpenPopup("Loading##DialogLoading");

            if (ImGui.IsPopupOpen("Loading##DialogLoading"))
            {
                ImGui.SetNextWindowPos(ImGui.GetMainViewport().Size / 2, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
                if (ImGui.BeginPopupModal("Loading##DialogLoading", ref data.Progress.Working, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    ImGui.Text("Loading, please wait...");
                    ImGui.Separator();
                    ImGui.Text(data.Progress.State.ToString());
                    ImGui.ProgressBar((float)data.Progress.CurrentProgress / (float)data.Progress.MaxProgress, new Vector2(-1, 0), $"Progress: {data.Progress.CurrentProgress} / {data.Progress.MaxProgress}");
                    ImGui.EndPopup();
                }
            }
        }
    }
}