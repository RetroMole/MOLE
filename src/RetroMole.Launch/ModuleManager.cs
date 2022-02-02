using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;
using Serilog;

namespace RetroMole.Launch
{
    public class ModuleManager : DynamicAssemblyLoader
    {
        public Dictionary<string, IGameModule> AvailableModules = new();
        public void LoadAssemblies(string AssemblyDir, byte[] PublicKeyToken)
        {
            Log.Information("Scanning for available GameModules...");

            if (!Directory.Exists(AssemblyDir))
            {
                Log.Warning("No GameModules directory found, attempting to create one...");
                Directory.CreateDirectory(AssemblyDir);
            }

            // Loop over all files that end with .dll and attempt to load them as Renderer assemblies
            foreach (var f in new DirectoryInfo(AssemblyDir).GetFiles("*.dll"))
            {
                Log.Information("Attempting to load GameModules assembly at {0}", f.FullName);
                VerifyAssembly<IGameModule>(f.FullName, PublicKeyToken, out var newTypes);
                foreach (var t in newTypes)
                {
                    AvailableModules.Add(t.Key, t.Value);
                }
            }

            // Loop over modules and initialize
            foreach (var m in AvailableModules)
            {
                m.Value.HookEvents();
                foreach (var w in m.Value.Windows)
                    Gui.Ui.Windows.Add($"{m.Key}|{w.Key}", w.Value);
            }
        }
    }
}
