using Tommy;

namespace RetroMole.Core.Utility
{
    public static class ConfigLoader
    {
        public static TomlTable FromTOML(string FilePath) => TOML.Parse(File.OpenText(FilePath));

        public static TomlTable FromTOML(List<String> Lines)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            foreach (var line in Lines)
                writer.WriteLine(line);
            writer.Flush();
            stream.Position = 0;

            return FromTOML(stream);
        }
        public static TomlTable FromTOML(Stream Stream) => TOML.Parse(new StreamReader(Stream));
    }
}
