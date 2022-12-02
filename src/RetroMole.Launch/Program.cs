using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using QuickImGuiNET;
using RetroMole.Core;
using Serilog;
using Tomlyn.Model;
using vbk = QuickImGuiNET.Veldrid;

namespace RetroMole;

public static class Launch
{
    private static Backend _backend;
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .MinimumLevel.Debug()
            .CreateLogger();
        Log.Information("RetroMole Launch v0.1");
        
        // Config Sinks/Sources
        var cfgToml = new Config.Toml("RetroMole.cfg", ref _backend);
        var cfgCli = new Config.Cli(args, ref _backend);
        
        // Shadow Backend
        Log.Logger.Information("Initializing Veldrid Backend (shadow)");
        _backend = new vbk.Backend
        {
            // Add Config to shadow backend
            Config = new Config
            {
                _default = new TomlTable
                {
                    { "debug", new TomlTable {
                        {"showMenu", true} 
                    }},
                    { "window", new TomlTable {
                        { "width", 1280 },
                        { "height", 720 }
                    }},

                    { "veldrid", new TomlTable {
                        {"backend", -1 }
                    }},

                    { "serilog", new TomlTable {
                        { "minimumLevel", "Debug" },
                        { "using", new TomlTable {
                            { "Console", "Serilog.Sinks.Console" },
                            { "File", "Serilog.Sinks.File" },
                        }},
                        { "writeTo", new TomlTable {
                            { "Console", new TomlTable {
                                { "outputTemplate", "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}" }
                            }},
                            { "File", new TomlTable {
                                { "outputTemplate", "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}" },
                                { "path", "RetroMole.log" },
                                { "fileSizeLimitBytes", "2000000" },
                                { "rollOnFileSizeLimit", "true" },
                                { "retainedFileCountLimit", "10" },
                                { "rollingInterval", "Day" }
                            }}
                        }}
                    }},
                    {"paths", new TomlTable {
                            {"home", (Environment.OSVersion.Platform is PlatformID.Unix or PlatformID.MacOSX
                                    ? Environment.GetEnvironmentVariable("HOME")
                                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"
                                    )) ?? string.Empty 
                            },
                            { "temp", Path.GetFullPath(Path.Combine(Path.GetTempPath(), "RetroMoleTemp")) },
                            { "exec", Path.GetFullPath(
                                Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? Assembly.GetExecutingAssembly()?.Location)
                                ?? Path.Combine(Path.GetTempPath(), "RetroMoleTemp", "tmpxc")
                            )}
                    }}
                },
                Sinks = new IConfigSink[]
                {
                    cfgToml,
                    cfgCli
                },
                Sources = new IConfigSource[]
                {
                    cfgToml,
                    cfgCli
                }
            },
            
            // Add default Logger to shadow backend
            Logger = Log.Logger,

            // Add Events to shadow backend
            Events = new Dictionary<string, Event>
            {
                {
                    "onMainMenuBar", new Event(new Dictionary<string, Event>
                    {
                        { "Debug", new Event() }
                    })
                },
                { "widgetReg", new Event() }
            },

            // Add Widget Registry to shadow backend
            WidgetReg = new Dictionary<string, Widget>()
        };
        
        // Loading config sources
        _backend.Logger.Information($"Loading default config + ({_backend.Config.Sources.Length}) source(s)");
        _backend.Config.LoadDefault();
        _backend.Config.From(_backend.Config.Sources[0]);
        _backend.Config.From(_backend.Config.Sources[1]);

        // Re-initialize Logger
        _backend.Logger.Information("Initializing new logger using config");
        Log.CloseAndFlush();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.KeyValuePairs(((TomlTable)_backend.Config["serilog"]).SelectMany(kvp =>
                kvp.Key switch
                {
                    "using" => ((TomlTable)kvp.Value).Select(use =>
                        new KeyValuePair<string, string>($"using:{use.Key}", (string)use.Value)),
                    "writeTo" => ((TomlTable)kvp.Value).SelectMany(sink =>
                        ((TomlTable)sink.Value).Select(o =>
                            new KeyValuePair<string, string>($"write-to:{sink.Key}.{o.Key}", (string)o.Value))),
                    _ => new[] { new KeyValuePair<string, string>(kvp.Key, (string)kvp.Value) }
                }
            )).CreateLogger();
        _backend.Logger = Log.Logger;

        // Initialize shadow -> ready backend
        _backend.Logger.Information("Config Done, fully initializing shadow backend");
        _backend.Init();

        // Load Packages
        _backend.Logger.Information("Load Packages (todo)");
        if (!Directory.Exists(Path.Combine(_backend.Config["paths"]["exec"], "Packages")))
            Directory.CreateDirectory(Path.Combine(_backend.Config["paths"]["exec"], "Packages"));
        var packages = ((string[])Directory.GetFileSystemEntries(
                Path.Combine(_backend.Config["paths"]["exec"], "Packages"),
                "*.dll", SearchOption.AllDirectories
            ))
            .Concat(
                (string[])Directory.GetFileSystemEntries(
                    Path.Combine(_backend.Config["paths"]["exec"], "Packages"),
                    "*.mole.pckg", SearchOption.AllDirectories
                )
            )
            .SelectMany(p =>
            {
                var ctx = new AssemblyLoadContext($"{Path.GetFullPath(p)}_{DateTime.Now.ToFileTimeUtc()}");
                var ext = Path.GetExtension(p).ToUpper();
                switch (ext)
                {
                    case ".DLL":
                        return Import.AssemblyPackages(ctx.LoadFromAssemblyPath(Path.GetFullPath(p)), _backend);
                    case ".MOLE.PCKG":
                        return Import.CompressedPackages(p, _backend);
                    default:
                        _backend.Logger.Error($"Invalid Package (doesn't end in .dll or .mole.pckg): {p}");
                        return new List<Package>();
                }
            })
            .ToArray();
        foreach (var p in packages)
        {
            _backend.Logger.Information($"Intializing Package: {p.PackageId} ({p.License.Id})");
            p.Init(ref _backend);
        }
        
        // Initialize RetroMole UI
        var gui = new Gui(_backend, packages);

        // Run backend loop and handle errors
        _backend.Logger.Information("Run Backend loop");
        try
        {
            _backend.Run(gui.Draw, UpdateCallback: gui.Update);
        }
        catch (Exception e)
        {
            _backend.Logger.Error(e.ToString());
            Debugger.Break();
        }
        finally
        {
            _backend.Logger.Information($"EXITING: {Environment.ExitCode}");
            Log.CloseAndFlush();
        }
    }
}