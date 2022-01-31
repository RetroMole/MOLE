using ImGuiNET;
using RetroMole.Core.Assemblers;
using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;
using Serilog;
using System.Reflection;

namespace RetroMole.Gui.Windows
{
    public class AsarTest : WindowBase
    {
        int PatchSelected;
        int PatchPrev;
        string PatchesPath;
        string[] Patches;
        byte[] romdat;
        public AsarTest()
        {
            PatchesPath = Path.Combine(Assembly.GetEntryAssembly().Location.Split('\\').SkipLast(1).Append("Patches").ToArray());
            Patches = Directory.GetFiles(PatchesPath).Select(s => s.Split('\\').Last()).ToArray();
        }
        public override void Draw(Project.UiData data, Dictionary<string, WindowBase> windows)
        {
            if (!ShouldDraw)
                return;

            if (!ImGui.Begin("AsarTest"))
                return;

            ImGui.InputText("ROM Path", ref (windows["OpenFile"] as Dialogs.FilePicker).SelectedFile, 4096);
            ImGui.SameLine();
            if(ImGui.Button("Browse..."))
                windows["OpenFile"].Open();
            Widgets.Widgets.ComboWithArrows("AsarPatch", "Patch", Patches, ref PatchSelected, ref PatchPrev
                );

            if (!string.IsNullOrEmpty((windows["OpenFile"] as Dialogs.FilePicker).SelectedFile))
            {
                romdat = File.ReadAllBytes((windows["OpenFile"] as Dialogs.FilePicker).SelectedFile);
            }

            if (ImGui.Button("Patch"))
            {
                Asar.Patch(Path.Combine(PatchesPath, Patches[PatchSelected]), ref romdat);
                File.WriteAllBytes($"{(windows["OpenFile"] as Dialogs.FilePicker).SelectedFile}_{Patches[PatchSelected]}.{(windows["OpenFile"] as Dialogs.FilePicker).SelectedFile.Split('.').Last()}", romdat);
                Asar.Reset();
            }

            ImGui.End();
        }
    }
}
