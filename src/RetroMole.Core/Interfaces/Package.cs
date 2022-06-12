namespace RetroMole.Core.Interfaces;
public abstract class Package
{
    public abstract string Name {get;}
    public abstract string Author {get;}
    public abstract Version Version {get;}
    public abstract Spdx.SpdxLicense License {get;}
    public virtual Uri Repository {get => new($"https://github.com/{Author}/{Name}");}
    public string PackageID {get => $"{Author}/{Name}@{Version}";}
    public virtual ImGuiController[] Controllers => null;
    public abstract void ApplyHooks();
}