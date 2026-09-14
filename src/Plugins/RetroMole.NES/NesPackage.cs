namespace RetroMole.NES;

using QuickImGuiNET;
using RetroMole.Core;
using RetroMole.Core.Detection;
using RetroMole.NES;
using Spdx;

public class NesPackage : Package
{
    public override string Name => "RetroMole.NES";

    public override string Author => "RetroMole";

    public override Version Version => new Version(0, 1, 0);

    public override SpdxLicense License => SpdxLicense.GetById("GPL-3.0-or-later") ?? throw new Exception("License not found in SPDX database");

    public override void Init(ref Context ctx)
    {
        SystemDefinitionRegistry.Register(new NesRomSystemDefinition());
        ctx.Logger.Information($"Registered NES system definition & it's ({SystemDefinitionRegistry.GetAll().Last().SupportedMappers.Count}) supported address mappers");
    }
}
