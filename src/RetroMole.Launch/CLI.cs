using CommandLine;
using Serilog.Events;

namespace RetroMole;
public static partial class Launch
{
    public class CLI
    {
        [Option("vk_backend", Required = false, Default = Veldrid.GraphicsBackend.Vulkan, HelpText = "Specifies which veldird graphics backend to use")]
        public Veldrid.GraphicsBackend Renderer { get; set; } = Veldrid.GraphicsBackend.Vulkan;
    }
}