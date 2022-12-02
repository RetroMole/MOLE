using QuickImGuiNET;
using Spdx;

namespace RetroMole.Core;

public abstract class Package
{
    public abstract string Name { get; }
    public abstract string Author { get; }
    public abstract Version Version { get; }
    public abstract SpdxLicense License { get; }
    public virtual Uri Repository => new($"https://github.com/{Author}/{Name}");
    public string PackageId => $"{Author}/{Name}@{Version}";
    public abstract void Init(ref Backend backend);
}