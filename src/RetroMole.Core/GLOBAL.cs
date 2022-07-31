#pragma warning disable CS8603

using System.Runtime.Loader;
using System.Reflection;
using Serilog;
using Tommy;

namespace RetroMole.Core;

public static class GLOBAL
{
    //--------------------Paths--------------------//
    public static string HomePath =>
        (  Environment.OSVersion.Platform == PlatformID.Unix
        || Environment.OSVersion.Platform == PlatformID.MacOSX)
        ?  Environment.GetEnvironmentVariable("HOME")                      
        :  Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"
    );
    public static string TempPath => Path.GetFullPath(Path.Combine(Path.GetTempPath(), "RetroMoleTemp"));
    public static string ExecPath => Path.GetFullPath(
        Path.GetDirectoryName
            (  Assembly.GetEntryAssembly()?.Location
            ?? Assembly.GetExecutingAssembly()?.Location
        )?? Path.Combine(TempPath, "tmpxc")
    );
    public static string CfgPath =>
        (  Environment.OSVersion.Platform == PlatformID.Unix
        || Environment.OSVersion.Platform == PlatformID.MacOSX
        )? Path.GetFullPath(Path.Combine(HomePath, ".config", "RetroMole"))
        :  Path.GetFullPath(Path.Combine(HomePath, "AppData", "Roaming", "RetroMole"));
    //--------------------Config--------------------//
        public static Core.Config.ConfT Config = File.Exists(Path.Combine(CfgPath, "config.toml"))
            ? Core.Config.Source.TOML(Import.TOMLFile(Path.Combine(CfgPath, "config.toml")))
            : Core.Config.Default;
    //--------------------Packages--------------------//
    public static Interfaces.Package[] Packages =
        Directory.GetFileSystemEntries(
            Path.Combine(ExecPath, "Packages"),
            "*.dll", SearchOption.AllDirectories
        )
        .Concat(
            Directory.GetFileSystemEntries(
                Path.Combine(ExecPath, "Packages"),
                "*.mole.pckg", SearchOption.AllDirectories
            )
        )
        .SelectMany(p => {
            var ctx = new AssemblyLoadContext($"{Path.GetFullPath(p)}_{DateTime.Now.ToFileTimeUtc()}");
            switch (Path.GetExtension(p)?.ToUpper())
            {
                case ".DLL":
                    if (p.Contains("RetroMole.Render.Veldrid"))
                        return Import.AssemblyPackages(
                            ctx.LoadFromAssemblyPath(Path.GetFullPath(p)),
                            Config.Renderer.Parameters
                        );
                    return Import.AssemblyPackages(ctx.LoadFromAssemblyPath(Path.GetFullPath(p)));
                case ".MOLE.PCKG":
                    return Import.CompressedPackages(p);
                default:
                    Log.Error($"Uhh... how did you.. BAD PACKAGE @ {p}");
                    return null;
            }
        })
        .ToArray();
    public static Core.Interfaces.ImGuiController CurrentController = Packages.Select(p => p.Controllers.First(c => c.GetType().FullName == Config.Renderer.FullClassName)).First();
}

#pragma warning restore CS8603
