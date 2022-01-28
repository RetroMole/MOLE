using CommandLine;
using Serilog.Events;

namespace RetroMole.Launch
{
    internal class CLI
    {
        [Option("loglevel", Required = false, Default = LogEventLevel.Information, HelpText = "Specifies the verbosity of Mole's logging output\nValues: Verbose, Debug, Information, Warning, Error, Fatal")]
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

        [Option("renderer", Required = false, Default = "MonogameXNA-Renderer|RetroMole.MonogameXNARenderer.Renderer", HelpText = "Specifies which of the available rendering backends for Mole to use")]
        public string Renderer { get; set; } = "MonogameXNA-Renderer|RetroMole.MonogameXNARenderer.Renderer";

        [Option("file", Required = false, Default = "", SetName = "file", HelpText = "ROM file to open")]
        public string File { get; set; } = "";

        [Option("project", Required = false, Default = "", SetName = "proj", HelpText = "Moleproj file or directory to open")]
        public string Proj { get; set; } = "";

        [Option("gamemodule", Required = false, Default = "SMW-GameModule", HelpText = "Specifies which GameModule to initialize")]
        public string GameModule { get; set; } = "SMW-GameModule";
    }
}
