using System.Collections.Generic;

using System.Numerics;
using ImGuiNET;

namespace Mole.Gui.Windows
{
    public class LoadingDialog : Window
    {
        public override void Draw(Ui.UiData data, List<Window> windows)
        {
            if (data.Progress == null
                || data.Progress.Loaded) return;
            
            if (!ImGui.IsPopupOpen("Loading")) 
                ImGui.OpenPopup("Loading");

            if (ImGui.IsPopupOpen("Loading"))
            {
                ImGui.SetNextWindowPos(ImGui.GetMainViewport().Size / 2, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
                if (ImGui.BeginPopupModal("Loading", ref ShouldDraw))
                {
                    ImGui.Text("Project is loading, please wait...");
                    ImGui.Separator();
                    ImGui.Text(data.Progress.State.ToString());
                    ImGui.Text($"Progress: {data.Progress.CurrentProgress} / {data.Progress.MaxProgress}");
                    ImGui.EndPopup();
                }
            }
        }
    }
}