using Mole.Gui;
using Serilog;
using System.Reflection;
using System.Runtime.Loader;

namespace Mole
{
    public class RenderManager
    {
        public Dictionary<string, IRenderer> AvailableRenderers = new();
        public void LoadAvailableRenderers()
        {
            Log.Information("Scanning for available renderers...");

            var RenderersPath = Path.Combine(Assembly.GetEntryAssembly().Location.Split('\\').SkipLast(1).Append("Renderers").ToArray());
            if (!Directory.Exists(RenderersPath))
            {
                Log.Warning("No renderers directory found, attempting to create one...");
                Directory.CreateDirectory(RenderersPath);
            }

            // Loop over all files that end with .dll
            foreach (var f in new DirectoryInfo(RenderersPath).GetFiles("*.dll"))
            {
                // Try to load it as a renderer
                Assembly asm = null;
                Log.Information("Attempting to load assembly at {0}", f.FullName);
                try
                {
                    asm = Assembly.LoadFrom(f.FullName);
                }
                catch (Exception e)
                {
                    Log.Error("Could not load assembly at {0}", f.FullName);
                    continue; // try the next .dll file if this one fails
                }

                // Try registering all types that implement IRenderer to AvailableRenderers dictionary
                Log.Information("Searching for renderers in assembly: {0}", f.FullName);
                var Renderers = asm.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IRenderer))).ToArray();
                if (Renderers.Count() == 0) Log.Information("No renderers found in assembly: {0}", f.FullName);
                foreach (var t in Renderers)
                {
                    Log.Information("Found renderer \"{0}\" in assembly: {1}", t.Name, f.FullName);
                    AvailableRenderers.Add(t.Name, (IRenderer)Activator.CreateInstance(t));
                }
            }
        }
        public void RunRenderer(string Renderer)
        {
            var r = AvailableRenderers[Renderer];
            Log.Information("Initializing \"{0}\" renderer", Renderer);
            Ui.renderer = r;
            r.Start(() => { 
                r.BeforeLayout();
                Ui.Draw(ref r);
                r.AfterLayout();
            });
        }
    }
}