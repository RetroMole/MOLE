using Tommy;
using Serilog;
using Serilog.Events;


namespace RetroMole.Core;

public static partial class Config
{
    public static partial class Source
    {
        public static ConfT TOML(TomlTable Table) => new ConfT
        {
            Renderer = new ConfT.RenderT
            {
                FullClassName = Table["Renderer"]["FullClassName"].AsString,
                Parameters = Table["Renderer"]["Parameters"].AsArray.RawArray.Select(p => new ConfT.ParamT
                {
                    Name = p["Name"].AsString.Value,
                    Value = p.HasKey("FullEnumName") ? Enum.Parse(Type.GetType(p["FullEnumName"].AsString.Value), p["Value"].AsString.Value)
                            : p.IsString               ? p.AsString.Value
                            : p.IsBoolean              ? p.AsBoolean.Value
                            : p.IsFloat                ? p.AsFloat.Value
                            : p.IsInteger              ? p.AsInteger.Value
                            : throw new("fuck"),
                    FullEnumName = p.HasKey("FullEnumName") ? p["FullEnumName"] : null
                }).ToArray()
            },
            Logging = new ConfT.LogT
            {
                MinimumLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), Table["Logging"]["MinimumLevel"].AsString.Value),
                Sinks = Table["Logging"]["Sinks"].AsArray.RawArray.Select(s => 
                    (ConfT.LogT.SinkT.TypeT)Enum.Parse(typeof(ConfT.LogT.SinkT.TypeT), s["Type"].AsString.Value) switch {
                        ConfT.LogT.SinkT.TypeT.Console => ConfT.LogT.SinkT.Console(
                            (LogEventLevel)Enum.Parse(typeof(LogEventLevel), s["Level"].AsString.Value),
                            s["BlockWhenFull"].AsBoolean.Value,
                            s["OutputTemplate"].AsString.Value
                        ),
                        ConfT.LogT.SinkT.TypeT.File => ConfT.LogT.SinkT.File(
                            (LogEventLevel)Enum.Parse(typeof(LogEventLevel), s["Level"].AsString.Value),
                            s["BlockWhenFull"].AsBoolean.Value,
                            s["OutputTemplate"].AsString.Value,
                            s["Path"].AsString.Value,
                            (RollingInterval)Enum.Parse(typeof(RollingInterval), s["RollingInterval"].AsString.Value),
                            (int)s["FileSizeLimitBytes"].AsInteger.Value,
                            s["RollOnFileSizeLimit"].AsBoolean.Value,
                            (int)s["RetainedFileCountLimit"].AsInteger.Value
                        )
                    }
                ).ToArray()
            }
        };
    }
}