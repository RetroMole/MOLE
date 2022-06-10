namespace RetroMole.Core.Interfaces;
public class Package
{
    public virtual string Name => String.Empty;
    public virtual string Author => String.Empty;
    public virtual Version Version => new();
    public virtual Uri Repository => new($"https://github.com/{Author}/{Name}");
    public virtual Spdx.SpdxLicense License => Spdx.SpdxLicense.GetById("Unlicense");
    public string PackageID {get => $"{Author}/{Name}@{Version}";}
    public virtual ImGuiController[] Controllers => null;
    public virtual GameModule[] GameModules => null;
}