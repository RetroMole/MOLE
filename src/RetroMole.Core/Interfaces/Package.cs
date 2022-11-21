using QuickImGuiNET;
using Spdx;

namespace RetroMole.Core.Interfaces;

public abstract class Package
{
    public abstract string Name { get; }
    public abstract string Author { get; }
    public abstract Version Version { get; }
    public abstract SpdxLicense License { get; }
    public virtual Uri Repository => new($"https://github.com/{Author}/{Name}");
    public string PackageID => $"{Author}/{Name}@{Version}";
    public virtual Widget[] Widgets => new Widget[] { };
    public abstract void ApplyHooks();
}