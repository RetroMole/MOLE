using ImGuiNET;
using Veldrid;
namespace RetroMole.Render;

public partial class Veldrid : Core.Interfaces.Package
{
    private static GraphicsBackend _graphicsBackend = GraphicsBackend.Vulkan;
    public Veldrid(params object[] args) => _graphicsBackend = (GraphicsBackend)args[0];
    public override string Name => "VeldridController";
    public override string Author => "RetroMole";
    public override Version Version => new(0,0,1,0);
    public override Spdx.SpdxLicense License => Spdx.SpdxLicense.GetById("GPL-3.0-or-later");
    public override Core.Interfaces.ImGuiController[] Controllers => new Core.Interfaces.ImGuiController[] { new Controller(1280, 720, _graphicsBackend) };
    public override object[] Windows => new object[] { };
    public override void ApplyHooks() => Core.Hooks.UI.OnMainMenuBar += () => ImGui.MenuItem("Test Button");
}
