#pragma warning disable CS8603

using System.Runtime.Loader;
using Serilog;
using Tommy;

namespace RetroMole.Core;

public static class GLOBALS
{
    public static Interfaces.Package[] Packages =
        Directory.GetFileSystemEntries(
            Path.Combine(Core.GLOBALS.ExecPath, "Packages"),
            "*.dll", SearchOption.AllDirectories
        )
        .Concat(
            Directory.GetFileSystemEntries(
                Path.Combine(Core.GLOBALS.ExecPath, "Packages"),
                "*.mole.pckg", SearchOption.AllDirectories
            )
        )
        .SelectMany(p => {
            var ctx = new AssemblyLoadContext($"{Path.GetFullPath(p)}_{DateTime.Now.ToFileTimeUtc()}");
            switch (Path.GetExtension(p)?.ToUpper())
            {
                case ".DLL":
                    if (p.Contains("RetroMole.Render.Veldrid"))
                        return Core.Utility.Import.AssemblyPackages(
                            ctx.LoadFromAssemblyPath(Path.GetFullPath(p)),
                            Config is null
                                ? "Vulkan"
                                : Config["renderer"]["params"]
                        );
                    Log.Information($"Successfully loaded Package @ {Path.GetFullPath(p)}");
                    return Core.Utility.Import.AssemblyPackages(ctx.LoadFromAssemblyPath(Path.GetFullPath(p)));
                case ".MOLE.PCKG":
                    return Core.Utility.Import.CompressedPackages(p);
                default:
                    Log.Error($"Uhh... how did you.. BAD PACKAGE @ {p}");
                    return null;
            }
        })
        .ToArray();
    public static Core.Interfaces.ImGuiController? CurrentController = Config is null
		? Packages.First().Controllers.First()
		: Packages.Select(p => p.Controllers.First(c => c.GetType().FullName == Config["renderer"].AsString)).First();
    public static TomlTable? Config = File.Exists(Path.Combine(Core.GLOBALS.CfgPath, "config.toml"))
        ? Core.Utility.Import.Config(Path.Combine(Core.GLOBALS.CfgPath, "config.toml"))
        : null;
    public static string HomePath =>
        (  Environment.OSVersion.Platform == PlatformID.Unix
        || Environment.OSVersion.Platform == PlatformID.MacOSX)
        ?  Environment.GetEnvironmentVariable("HOME")                      
        :  Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"
    );
    public static string TempPath => Path.GetTempPath();
    public static string ExecPath => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    public static string CfgPath =>
        (  Environment.OSVersion.Platform == PlatformID.Unix
        || Environment.OSVersion.Platform == PlatformID.MacOSX)
        ?  Path.Combine(HomePath, ".config", "RetroMole")
        :  Path.Combine(HomePath, "AppData", "Roaming", "RetroMole"
    );
}

#pragma warning restore CS8603
