namespace RetroMole.Tests;

using System.Collections;
using System.IO;
using RetroMole.Core;
using RetroMole.Core.Detection;
using RetroMole.NES;
using Xunit;

public class RomFactorySmokeTests
{
    static RomFactorySmokeTests()
    {
        if (!SystemDefinitionRegistry.GetAll().Any(d => d.SystemName == "NES"))
        {
            SystemDefinitionRegistry.Register(new NesRomSystemDefinition());
        }
    }

    private static string ResolvePath(string path)
    {
        if (Path.IsPathFullyQualified(path))
        {
            return path;
        }

        var baseDir = AppContext.BaseDirectory;
        var repoRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", ".."));
        return Path.GetFullPath(Path.Combine(repoRoot, path));
    }

    public static IEnumerable<object[]> RomPaths
    {
        get
        {
            var single = Environment.GetEnvironmentVariable("RETROMOLE_TEST_ROM");
            if (!string.IsNullOrWhiteSpace(single))
            {
                yield return new object[] { ResolvePath(single.Trim()) };
                yield break;
            }

            var multi = Environment.GetEnvironmentVariable("RETROMOLE_TEST_ROMS");
            if (!string.IsNullOrWhiteSpace(multi))
            {
                var separator = Path.PathSeparator;
                foreach (var part in multi.Split(separator))
                {
                    var trimmed = part.Trim();
                    if (trimmed.Length > 0)
                    {
                        yield return new object[] { ResolvePath(trimmed) };
                    }
                }

                yield break;
            }

            yield break;
        }
    }

    [Theory]
    [MemberData(nameof(RomPaths))]
    public void Load_ReturnsNesRom(string romPath)
    {
        Assert.True(File.Exists(romPath), $"Test ROM not found at {romPath}");

        var rom = RomFactory.Load(romPath);
        Assert.NotNull(rom);
        Assert.IsType<NesRom>(rom);
    }
}
