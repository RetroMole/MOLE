#pragma warning disable CS8619, CS8600, CS0618, CS8604

using ZipLib = ICSharpCode.SharpZipLib;
using Tommy;
using Serilog;
using System.Reflection;
using System.Runtime.Loader;

namespace RetroMole.Core;

public static class Import
{
    // .NET Assembly (*.dll) containing Core.Interfaces.Package implementations 
    public static Interfaces.Package[] AssemblyPackages(Assembly asm, params object[] extraInstArgs)
    {
        Log.Information($"Loading Assembly Package container @ {Path.GetFullPath(asm.Location)} w/ ARGS = {String.Join(", ", extraInstArgs.Select(a => a.ToString()))}");

        var res = asm.DefinedTypes
            .Where(t => t.BaseType == typeof(Interfaces.Package))
            .Select((p, i) => (Interfaces.Package)asm.CreateInstance(p.FullName, args: extraInstArgs, ignoreCase: false, bindingAttr: BindingFlags.Default, binder: null, culture: null, activationAttributes: null))
            .ToArray();

        Log.Information($"Successfully loaded Assembly Package container @ {Path.GetFullPath(asm.Location)} w/ ARGS = {String.Join(", ", extraInstArgs.Select(a => a.ToString()))}\nFound {res.Length} Package Classes");
        return res;
    }

    // TAR.GZ file (*.mole.pckg) containing Core.Interfaces.Package implementations 
    public static Interfaces.Package[] CompressedPackages(string path, params object[] extraInstArgs)
    {
        Log.Information($"Loading Compressed Package container @ {Path.GetFullPath(path)} w/ ARGS = {String.Join(", ", extraInstArgs.Select(a => a.ToString()))}");
        MemoryStream decompressed = new MemoryStream();
        using (var fs = new FileStream(path, FileMode.Open))
        {
            // Decompress
            ZipLib.GZip.GZip.Decompress(fs, decompressed, false);
            
            // Create input tar archive object
            var tar = ZipLib.Tar.TarArchive.CreateInputTarArchive(decompressed);
            
            // Store temp path
            var tempPath = Path.Combine(GLOBAL.TempPath, $"{Path.GetFileName(path)}_{DateTime.Now.ToFileTimeUtc()}");
            
            // Extract to temp path
            tar.ExtractContents(tempPath);

            // Find all extracted .dll's
            var paths = Directory.GetFileSystemEntries(path, "*.dll", SearchOption.AllDirectories);

            // Load packages
            List<Interfaces.Package> res = new();
            foreach (var f in paths)
            {
                var ctx = new AssemblyLoadContext($"{Path.GetFullPath(f)}_{DateTime.Now.ToFileTimeUtc()}");
                var asm = ctx.LoadFromAssemblyPath(Path.GetFullPath(f));
                res.AddRange(AssemblyPackages(asm, extraInstArgs));
            }

            // Delete temp files
            Directory.Delete(tempPath, true);

            Log.Information($"Successfully loaded Compressed Package container @ {Path.GetFullPath(path)} w/ ARGS = {String.Join(", ", extraInstArgs.Select(a => a.ToString()))}\nFound {res.Count} Package Classes");
            return res.ToArray();
        }   
    }

    public static TomlTable Config(string path) { using(StreamReader reader = File.OpenText(path)) return TOML.Parse(reader); }

}
#pragma warning restore CS8619, CS8600, CS0618, CS8604
