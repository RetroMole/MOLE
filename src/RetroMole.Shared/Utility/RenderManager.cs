using RetroMole.Core.Interfaces;
using Serilog;

namespace RetroMole.Core.Utility
{
    public class RenderManager : DynamicAssemblyLoader
    {
        public Dictionary<string, IRenderer> AvailableRenderers = new();
        public void LoadAssemblies(string AssemblyDir, byte[] PublicKeyToken)
        {
            Log.Information("Scanning for available Renderers...");

            if (!Directory.Exists(AssemblyDir))
            {
                Log.Warning("No Renderers directory found, attempting to create one...");
                Directory.CreateDirectory(AssemblyDir);
            }

            // Loop over all files that end with .dll and attempt to load them as Renderer assemblies
            foreach (var f in new DirectoryInfo(AssemblyDir).GetFiles("*.dll"))
            {
                Log.Information("Attempting to load Renderer assembly at {0}", f.FullName);
                VerifyAssembly<IRenderer>(f.FullName, PublicKeyToken, out var newTypes);
                foreach (var t in newTypes)
                {
                    AvailableRenderers.Add(t.Key, t.Value);
                }
            }
        }
    }
}
