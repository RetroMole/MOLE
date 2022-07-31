using Tommy;

namespace RetroMole.Core;

public static partial class Config
{
    public static partial class Sink
    {
        public static TomlTable TOML(ConfT Config)
        {
            var tbl = new TomlTable
            {
                ["Renderer"] = new TomlTable
                {
                    ["FullClassName"] = Config.Renderer.FullClassName,
                    ["Parameters"] = new TomlArray { IsTableArray = true }
                },
                ["Logging"] = new TomlTable
                {
                    ["MinimumLevel"] = Config.Logging.MinimumLevel.ToString(),
                    ["Sinks"] = new TomlArray { IsTableArray = true }
                }
            };

            tbl["Renderer"]["Parameters"].AddRange(
                Config.Renderer.Parameters.Select(p => (p.FullEnumName is null
                ? new TomlTable
                {
                    ["Name"] = p.Name,
                    ["Value"] = p.Value,
                }
                : new TomlTable
                {
                    ["Name"] = p.Name,
                    ["Value"] = Enum.GetName(Type.GetType(p.FullEnumName), p.Value),
                    ["FullEnumName"] = p.FullEnumName.ToString(),
                })
            ));

            tbl["Logging"]["Sinks"].AddRange(
                Config.Logging.Sinks.Select(s => s.Type switch {
                    ConfT.LogT.SinkT.TypeT.Console => new TomlTable
                    {
                        ["Type"] = s.Type.ToString(),
                        ["Level"] = s.Level.ToString(),
                        ["BlockWhenFull"] = s.BlockWhenFull,
                        ["OutputTemplate"] = s.OutputTemplate
                    },
                    ConfT.LogT.SinkT.TypeT.File => new TomlTable
                    {
                        ["Type"] = s.Type.ToString(),
                        ["Level"] = s.Level.ToString(),
                        ["BlockWhenFull"] = s.BlockWhenFull,
                        ["OutputTemplate"] = s.OutputTemplate,
                        ["Path"] = s.Path,
                        ["RollingInterval"] = s.RollingInterval.ToString(),
                        ["FileSizeLimitBytes"] = s.FileSizeLimitBytes,
                        ["RollOnFileSizeLimit"] = s.RollOnFileSizeLimit,
                        ["RetainedFileCountLimit"] = s.RetainedFileCountLimit
                    }
                })
            );

            return tbl;
        }
    }
}