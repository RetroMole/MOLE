using Tommy;

namespace RetroMole.Core.Utility;

public static class Export
{
    public static void Config(TomlTable config, string path)
    {
        using (TextWriter tw = File.CreateText(path))
        {
            config.WriteTo(tw);
            tw.Flush();
        }
    }
}