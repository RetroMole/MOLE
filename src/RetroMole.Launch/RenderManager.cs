using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;
using RetroMole.Gui;
using Serilog;

namespace RetroMole.Launch
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

        public void RunRenderer(string Renderer)
        {
            var r = AvailableRenderers[Renderer];
            Log.Information("Initializing \"{0}\" renderer", Renderer);
            Ui.renderer = r;
            if (Program.CLIOpts.File != string.Empty)
            {
                (Ui.Windows["OpenFile"] as Gui.Dialogs.FilePicker).SelectedFile = Path.GetFullPath(Program.CLIOpts.File);
                Events.Ui.OpenFileEventHandler(Ui.Windows["OpenFile"]);
            }
            else if (Program.CLIOpts.Proj != string.Empty)
            {
                if (Program.CLIOpts.Proj.EndsWith(".moleproj"))
                {
                    (Ui.Windows["OpenProjectFile"] as Gui.Dialogs.FilePicker).SelectedFile = Path.GetFullPath(Program.CLIOpts.Proj);
                    Events.Ui.OpenProjectFileEventHandler(Ui.Windows["OpenProjectFile"]);
                }
                else if (Program.CLIOpts.Proj.EndsWith("_moleproj"))
                {
                    (Ui.Windows["OpenProject"] as Gui.Dialogs.FilePicker).CurrentFolder = Path.GetFullPath(Program.CLIOpts.Proj);
                    Events.Ui.OpenProjectEventHandler(Ui.Windows["OpenProject"]);
                }
            }
            r.Start(() => {
                r.BeforeLayout();
                Ui.Draw(ref r);
                r.AfterLayout();
            });
        }
    }
}
