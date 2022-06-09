using RetroMole;

namespace RetroMole.Render;

public partial class Veldrid : RetroMole.Core.Package
{
    public override string Name => "VeldridController";
    public override string Author => "RetroMole";
    public override Version Version => new(0,0,1,0);
    public override Spdx.SpdxLicense License => Spdx.SpdxLicense.GetById("GPL-3.0-or-later");
    public override Core.ImGuiController[] Controllers => new Core.ImGuiController[] { new Controller(1280, 720) };
}