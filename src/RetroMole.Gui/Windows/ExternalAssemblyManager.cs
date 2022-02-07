using ImGuiNET;
using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;
using System.Numerics;

namespace RetroMole.Gui.Windows
{
    public class ExternalAssemblyManager : WindowBase
    {
        public new bool ShouldDraw = true;
        public override void Draw(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            if (!ShouldDraw)
                return;

            if (!ImGui.Begin("External Assembly Manager", ImGuiWindowFlags.MenuBar))
                return;

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("Dummy"))
                {
                    if (ImGui.MenuItem("Dummy2"))
                    {
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            if (ImGui.BeginTabBar("##Tabs"))
            {
                if (ImGui.BeginTabItem("GameModules"))
                    Draw_GameModules(data, windows);

                if (ImGui.BeginTabItem("Renderers"))
                    Draw_Renderers(data, windows);

                ImGui.EndTabBar();
            }
            ImGui.End();
        }

        public void Draw_GameModules(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            ImGui.EndTabItem();
        }

        public void Draw_Renderers(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            foreach (var r in Ui.Rmngr.AvailableRenderers)
            {
                if (ImGui.BeginChildFrame((uint)r.Key.GetHashCode(), new(750,69)))
                {
                    ImGui.Text(r.Value.Name);
                    ImGui.SameLine();
                    ImGui.Text($"v{r.Value.Version}");
                    ImGui.SameLine();
                    ImGui.Button("On/Off");
                    ImGui.TextWrapped(r.Value.Description);
                    ImGui.Button($"(GH) {string.Join('/', r.Value.Repo.Split('/').TakeLast(3))}");
                    ImGui.SameLine();
                    ImGui.Button($"(C) {r.Value.License}");
                    ImGui.SameLine(); ImGui.Button("?"); ImGui.SameLine(); ImGui.Button("?"); ImGui.SameLine(); ImGui.Button("?"); ImGui.SameLine(); ImGui.Button("?");
                    ImGui.EndChildFrame();
                }
            }
            ImGui.EndTabItem();
        }
    }
}
