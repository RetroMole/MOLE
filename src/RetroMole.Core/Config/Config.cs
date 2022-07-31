using Serilog;
using Serilog.Events;

namespace RetroMole.Core;

public static partial class Config
{
    /// <summary>Default configuration object.</summary>
    public static ConfT Default = new ConfT
    {
        Renderer = new ConfT.RenderT
        {
            FullClassName = "RetroMole.Render.Veldrid+Controller",
            Parameters = new[]
            {
                new ConfT.ParamT { Name = "backend", Value = Enum.Parse(Type.GetType("Veldrid.GraphicsBackend, Veldrid, Version=4.8.0.0, Culture=neutral, PublicKeyToken=null"), "OpenGL"), FullEnumName = "Veldrid.GraphicsBackend, Veldrid, Version=4.8.0.0, Culture=neutral, PublicKeyToken=null" }
            }
        },
        Logging = new ConfT.LogT
        {
            MinimumLevel = LogEventLevel.Information,
            Sinks = new[]
            {
                ConfT.LogT.SinkT.Console(LogEventLevel.Information, true, "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"),
                ConfT.LogT.SinkT.File(LogEventLevel.Information, true, "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    Path.Join(GLOBAL.CfgPath, "logs", "RetroMole.log"),
                    RollingInterval.Day, 2000000, true, 7
                )
            }
        }
    };
    /// <summary>Internal representation of configurations.</summary>
    public struct ConfT
    {
        /// <summary>Renderer configuration section.</summary>
        public RenderT Renderer;

        /// <summary>Logging configuration section.</summary>
        public LogT Logging;

        /// <summary>Renderer config section type.</summary>
        public struct RenderT
        {
            public string FullClassName;
            public ParamT[] Parameters;
        }

        /// <summary>Logging config section type.</summary>
        public struct LogT
        {
            /// <summary>Configured sinks to log events to.</summary>
            public SinkT[] Sinks;

            /// <summary>Minimum level of events to log, overrides per-sink logging level option.</summary>
            public LogEventLevel MinimumLevel;

            /// <summary>Logging sink type definition.</summary>
            public struct SinkT
            {
                /// <summary>Type of sink.</summary>
                public TypeT Type;

                /// <summary>Minimum level of events to log to this sink.</summary>
                public LogEventLevel Level;

                /// <summary>Whether to block the thread sending events to this sink when it is full.</summary>
                public bool BlockWhenFull;
                
                /// <summary>Template for log event messages.</summary>
                public string OutputTemplate;

                /// <summary>File sink type only: Path to log file.</summary>
                public string Path;

                /// <summary>"File" sink type only: Interval at which to roll to a new file.</summary>
                public Serilog.RollingInterval RollingInterval;

                /// <summary>"File" sink type only: Maximum size of a log file before rolling to a new file.</summary>
                public int FileSizeLimitBytes;

                /// <summary>"File" sink type only: Whether to roll to a new file when the file size limit is reached.</summary>
                public bool RollOnFileSizeLimit;

                /// <summary>"File" sink type only: Maximum number of log files to keep per interval.</summary>
                public int RetainedFileCountLimit;

                /// <summary>Creates a console sink.</summary>
                /// <param name="level">Minimum level of events to log to this sink.</param>
                /// <param name="blockWhenFull">Whether to block the thread sending events to this sink when it is full.</param>
                /// <param name="outputTemplate">Template for log event messages.</param>
                public static SinkT Console(LogEventLevel level, bool blockWhenFull, string outputTemplate) => new SinkT
                {
                    Type = TypeT.Console,
                    Level = level,
                    BlockWhenFull = blockWhenFull,
                    OutputTemplate = outputTemplate
                };
                /// <summary>Creates a file sink.</summary>
                /// <param name="level">Minimum level of events to log to this sink.</param>
                /// <param name="blockWhenFull">Whether to block the thread sending events to this sink when it is full.</param>
                /// <param name="outputTemplate">Template for log event messages.</param>
                /// <param name="path">Path to log file.</param>
                /// <param name="rollingInterval">Interval at which to roll to a new file.</param>
                /// <param name="fileSizeLimitBytes">Maximum size of a log file before rolling to a new file.</param>
                /// <param name="rollOnFileSizeLimit">Whether to roll to a new file when the file size limit is reached.</param>
                /// <param name="retainedFileCountLimit">Maximum number of log files to keep per interval.</param>
                public static SinkT File(LogEventLevel level, bool blockWhenFull, string outputTemplate, string path, RollingInterval rollingInterval, int fileSizeLimitBytes, bool rollOnFileSizeLimit, int retainedFileCountLimit) => new SinkT
                {
                    Type = TypeT.File,
                    Level = level,
                    BlockWhenFull = blockWhenFull,
                    OutputTemplate = outputTemplate,
                    Path = path,
                    RollingInterval = rollingInterval,
                    FileSizeLimitBytes = fileSizeLimitBytes,
                    RollOnFileSizeLimit = rollOnFileSizeLimit,
                    RetainedFileCountLimit = retainedFileCountLimit
                };

                /// <summary>Sink type enum.</summary>
                public enum TypeT
                {
                    /// <summary>Serilog.Sinks.Console</summary>
                    Console,
                    /// <summary>Serilog.Sinks.File</summary>
                    File
                }
            }
        }

        public struct ParamT
        {
            public string Name;
            public dynamic? Value;
            public string? FullEnumName;
        }
    }
}