namespace RetroMole;

using ImGuiNET;
using QuickImGuiNET;
using RetroMole.Core;

public partial class Gui
{
    public static partial class Widgets
    {
        public class PackageManager : Widget
        {
            public Package[] Packages;

            public PackageManager(Context ctx, string? name = null, bool autoRegister = true)
                : base(ctx, name,
                autoRegister)
            {
            }

            public override void RenderContent()
            {
                foreach (var p in Packages)
                {
                    ImGui.TextColored(new(218, 165, 32, 255), $"{p.PackageId} ({p.License.Id})");
                }
            }
        }
    }
}
