using System.Numerics;
using ImGuiNET;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// Debug information
    /// </summary>
    public static class About
    {
        public static void Main(bool draw)
        {
            if (draw)
            {
                if (ImGui.Begin("About", ref draw))
                {
                    ImGui.Text(Strings.Copyright);
                    ImGui.Separator();
                    ImGui.Text($"Mole Shared Version: {Strings.MoleSharedVersion}");
                    ImGui.Text($"MOLE GUI Version {Strings.MoleSharedVersion}");
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
        }
    }
}