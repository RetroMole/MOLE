using ImGuiNET;
using QuickImGuiNET;
using RetroMole.Core;

namespace RetroMole;

public partial class Gui
{
    public static partial class Widgets
    {
        public class PackageManager : Widget
        {
            public Package[] Packages;

            public PackageManager(Backend backend, string? Name = null, bool AutoRegister = true) : base(backend, Name,
                AutoRegister)
            {
            }

            public override void RenderContent()
            {
                foreach (var p in Packages)
                    ImGui.TextColored(new(218, 165, 32, 255), $"{p.PackageId} ({p.License.Id})");
            }
        }
    }
}