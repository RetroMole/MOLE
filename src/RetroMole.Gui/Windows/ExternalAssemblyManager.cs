using ImGuiNET;
using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;

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
                ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_34A8EB);
                ImGui.Text(r.Value.Name);
                ImGui.PopStyleColor();
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_34EBC9);
                ImGui.Text($"v{r.Value.Version}");
                ImGui.PopStyleColor();

                ImGui.SameLine(ImGui.GetWindowWidth() - 55);
                ImGui.Button("On/Off");

                ImGui.TextWrapped(r.Value.Description);

                Widgets.Link.link($"(GH) {string.Join('/', r.Value.Repo.Split('/').TakeLast(3))}", r.Value.Repo);
                ImGui.SameLine(0, 25);
                Widgets.Link.link($"(C) {r.Value.License}", "https://github.com/RetroMole/MOLE/blob/master/LICENSE.md");

                ImGui.SameLine(ImGui.GetWindowWidth() - 90); ImGui.Button("?"); ImGui.SameLine(); ImGui.Button("?"); ImGui.SameLine(); ImGui.Button("?"); ImGui.SameLine(); ImGui.Button("?");
                ImGui.Separator();
            }
            ImGui.EndTabItem();
        }
    }
}
