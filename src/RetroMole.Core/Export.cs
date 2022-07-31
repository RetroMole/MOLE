using Tommy;

namespace RetroMole.Core;

public static class Export
{
    public static void TOMLFile(TomlTable tbl, string path)
    {
        using (TextWriter tw = File.CreateText(path))
        {
            tbl.WriteTo(tw);
            tw.Flush();
        }
    }
}