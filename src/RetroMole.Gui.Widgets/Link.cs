using ImGuiNET;
using RetroMole.Core.Utility;

namespace RetroMole.Gui.Widgets
{
    public static class Link
    {
        private static Dictionary<string, uint> Link_Color = new();
        public static void link(string DisplayText, string URL)
        {
            if (!Link_Color.ContainsKey($"{DisplayText}|{URL}"))
                Link_Color.Add($"{DisplayText}|{URL}", 0xFF_FFAEA3);
            ImGui.PushStyleColor(ImGuiCol.Text, Link_Color[$"{DisplayText}|{URL}"]);
            ImGui.Text(DisplayText);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                Web.OpenBrowser(URL);
            Link_Color[$"{DisplayText}|{URL}"] = ImGui.IsItemHovered() ? 0xFF_FF91D7 : 0xFF_FFAEA3;
            ImGui.PopStyleColor();
        }
    }
}
