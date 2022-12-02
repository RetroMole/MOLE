using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using QuickImGuiNET;
using ZipLib = ICSharpCode.SharpZipLib;

namespace RetroMole.Core;

public static class Import
{
    // .NET Assembly (*.dll) containing Core.Interfaces.Package implementations 
    public static IEnumerable<Package> AssemblyPackages(Assembly asm, Backend backend)
    {
        backend.Logger.Information(
            $"Loading Assembly Package container @ {Path.GetFullPath(asm.Location)}");

        var res = asm.DefinedTypes
            .Where(t => t.BaseType == typeof(Package))
            .Select(p => (Package)asm.CreateInstance(p.FullName, args: null, ignoreCase: false,
                        bindingAttr: BindingFlags.Default, binder: null, culture: null, activationAttributes: null)
                        ?? throw new Exception()
            ).ToList();

        backend.Logger.Information($"Found {res.Count} Packages in Container: {Path.GetFullPath(asm.Location)}");
        return res;
    }

    // TAR.GZ file (*.mole.pckg) containing Core.Interfaces.Package implementations 
    public static IEnumerable<Package> CompressedPackages(string path, Backend backend)
    {
        backend.Logger.Information(
            $"Loading Compressed Package container: {Path.GetFullPath(path)}");
        using var fs = new FileStream(path, FileMode.Open);
        
        // Decompress
        var decompressed = new MemoryStream();
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
            res.AddRange(AssemblyPackages(asm, backend));
        }

        // Delete temp files
        Directory.Delete(tempPath, true);

        backend.Logger.Information($"Found {res.Count} Packages in Container: {Path.GetFullPath(path)}");
        return res.ToArray();
    }
}