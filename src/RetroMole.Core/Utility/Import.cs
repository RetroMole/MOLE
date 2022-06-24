using ZipLib = ICSharpCode.SharpZipLib;
using Tommy;
using System.IO;
using System.Reflection;

namespace RetroMole.Core.Utility;

public static class Import
{
    // .NET Assembly (*.dll) containing Core.Interfaces.Package implementations 
    public static Interfaces.Package[] AssemblyPackages(Assembly asm) => asm.DefinedTypes
        .Where(t => t.BaseType == typeof(Interfaces.Package))
        .Select((p, i) => (Interfaces.Package)asm.CreateInstance(p.FullName))
        .ToArray();

    // TAR.GZ file (*.mole.pckg) containing Core.Interfaces.Package implementations 
    public static Interfaces.Package[] CompressedPackages(string path)
    {
        MemoryStream decompressed = new MemoryStream();
        using (var fs = new FileStream(path, FileMode.Open))
        {
            // Decompress
            ZipLib.GZip.GZip.Decompress(fs, decompressed, false);
            
            // Create input tar archive object
            var tar = ZipLib.Tar.TarArchive.CreateInputTarArchive(decompressed);
            
            // Store temp path
            var tempPath = Path.Combine(CommonDirectories.Temp, $"retromoletemp_{Path.GetFileName(path)}_{DateTime.Now.ToFileTimeUtc()}");
            
            // Extract to temp path
            tar.ExtractContents(tempPath);

            // Find all extracted .dll's
            var paths = Directory.GetFileSystemEntries(path, "*.dll", SearchOption.AllDirectories);

            // Load packages
            List<Interfaces.Package> res = new();
            foreach (var f in paths)
            {
                var data = File.ReadAllBytes(f);
                var asm = Assembly.Load(data);
                res.AddRange(AssemblyPackages(asm));
            }

            // Delete temp files
            Directory.Delete(tempPath, true);

            return res.ToArray();
        }   
    }

    public static TomlTable Config(string path) { using(StreamReader reader = File.OpenText(path)) return TOML.Parse(reader); }
}
