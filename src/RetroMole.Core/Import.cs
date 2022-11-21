using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using QuickImGuiNET;
using RetroMole.Core.Interfaces;
using Serilog;
using ZipLib = ICSharpCode.SharpZipLib;

namespace RetroMole.Core;

public static class Import
{
    // .NET Assembly (*.dll) containing Core.Interfaces.Package implementations 
    public static IEnumerable<Package> AssemblyPackages(Assembly asm, params object[] extraInstArgs)
    {
        Log.Information(
            $"Loading Assembly Package container @ {Path.GetFullPath(asm.Location)} w/ ARGS = {string.Join(", ", extraInstArgs.Select(a => a.ToString()))}");

        var res = asm.DefinedTypes
            .Where(t => t.BaseType == typeof(Package))
            .Select((p, i) =>
            {
                if (p.FullName != null)
                    return (Package)asm.CreateInstance(p.FullName, args: extraInstArgs, ignoreCase: false,
                        bindingAttr: BindingFlags.Default, binder: null, culture: null, activationAttributes: null)!;
                return null;
            })
            .Where(p => p is not null)!
            .ToArray<Package>();

        Log.Information(
            $"Successfully loaded Assembly Package container @ {Path.GetFullPath(asm.Location)} " +
            $"w/ ARGS = {string.Join(", ", extraInstArgs.Select(a => a.ToString()))}" +
            $"\nFound {res.Length} Package Classes");
        return res;
    }

    // TAR.GZ file (*.mole.pckg) containing Core.Interfaces.Package implementations 
    public static IEnumerable<Package> CompressedPackages(string path, Backend backend, params object[] extraInstArgs)
    {
        Log.Information(
            $"Loading Compressed Package container @ {Path.GetFullPath(path)} w/ ARGS = {string.Join(", ", extraInstArgs.Select(a => a.ToString()))}");
        var decompressed = new MemoryStream();
        using var fs = new FileStream(path, FileMode.Open);
        // Decompress
        GZip.Decompress(fs, decompressed, false);

        // Create input tar archive object
        var tar = TarArchive.CreateInputTarArchive(decompressed, Encoding.UTF8);

        // Store temp path
        var tempPath = Path.Combine(backend.Config["paths"]["temp"], $"{Path.GetFileName(path)}_{DateTime.Now.ToFileTimeUtc()}");

        // Extract to temp path
        tar.ExtractContents(tempPath);

        // Find all extracted .dll's
        var paths = Directory.GetFileSystemEntries(path, "*.dll", SearchOption.AllDirectories);

        // Load packages
        List<Package> res = new();
        foreach (var f in paths)
        {
            var ctx = new AssemblyLoadContext($"{Path.GetFullPath(f)}_{DateTime.Now.ToFileTimeUtc()}");
            var asm = ctx.LoadFromAssemblyPath(Path.GetFullPath(f));
            res.AddRange(AssemblyPackages(asm, extraInstArgs));
        }

        // Delete temp files
        Directory.Delete(tempPath, true);

        Log.Information(
            $"Successfully loaded Compressed Package container @ {Path.GetFullPath(path)} " + 
            $"w/ ARGS = {string.Join(", ", extraInstArgs.Select(a => a.ToString()))}" +
            "\nFound {res.Count} Package Classes");
        return res.ToArray();
    }
}