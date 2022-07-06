using ImGuiNET;

namespace RetroMole;

public static partial class Gui
{
    public static class Utility
    {
        public static void WithColors(Action Draw, params (ImGuiCol, uint)[] colors)
        {
            foreach (var col in colors)
                ImGui.PushStyleColor(col.Item1, col.Item2);
            Draw();
            ImGui.PopStyleColor(colors.Count());
        }

        public static void WithSameLine(params Action[] Elems)
        {
            Elems[0]();
            for (int i = 1; i < Elems.Length; i++)
            {
                ImGui.SameLine();
                Elems[i]();
            }
        }
        
        public static void WithDisabled(bool disabled, Action Draw)
        {
            ImGui.BeginDisabled(disabled);
            Draw();
            ImGui.EndDisabled();
        }
    }
}
