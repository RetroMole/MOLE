using ImGuiNET;

namespace Mole.Gui
{
    public static partial class Widgets
    {
        public static bool ComboWithArrows(string ID, string Text, string[] Items, ref int Selected, ref int Prev)
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            float w = ImGui.CalcItemWidth();
            float spacing = style.ItemInnerSpacing.X;
            float button_sz = ImGui.GetFrameHeight();
            ImGui.PushItemWidth(w - spacing * 2.0f - button_sz * 2.0f);
            ImGui.Text($"{Text}:");
            ImGui.SameLine(0, spacing);
            if (ImGui.BeginCombo($"##{ID}Combo", Items[Selected]))
            {
                for (byte n = 0; n < Items.Length; n++)
                {
                    bool is_selected = (Selected == n);
                    if (ImGui.Selectable(Items[n], is_selected))
                        Selected = n;
                    if (is_selected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();
            ImGui.SameLine(0, spacing);
            if (ImGui.ArrowButton($"##{ID}l", ImGuiDir.Left))
                if (Selected > 0) Selected--;
            ImGui.SameLine(0, spacing);
            if (ImGui.ArrowButton($"##{ID}r", ImGuiDir.Right))
                if (Selected < Items.Length - 1) Selected++;

            return Prev != Selected;
        }
    }
}
