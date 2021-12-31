using ImGuiNET;
using Mole.Shared.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mole.Gui
{
    public static partial class Ui
    {
        public static IRenderer renderer;
        private static readonly Dictionary<string, WindowBase> Windows = new()
            {
                { "About", new Windows.About() },
                //=============Dialogs=============
                { "OpenFile", new Dialogs.FilePicker("Select ROM file", "/", SearchFilter: ".smc,.sfc|.asm") },
                { "OpenProject", new Dialogs.FilePicker("Select Project Directory", "/", OnlyFolders: true) },
                { "OpenProjectFile", new Dialogs.FilePicker("Select Compressed Project File", "/", SearchFilter: ".moleproj") },
                { "Loading", new Dialogs.Loading() },
                { "Error", new Dialogs.Error() },
                //=============Editors=============
                { "RomInfo", new Windows.RomInfo() },
                { "PalEditor", new Windows.PalEditor() },
                { "GfxEditor", new Windows.GfxEditor() }
            };

        private static bool _ShowDemo;
        private static Project.UiData Data = new();
        private static void OpenFileEventHandler(WindowBase window)
        {
            Windows["Loading"].Open();
            var w = window as Dialogs.FilePicker;
            new Task(() =>
            {
                var dir = string.Join('.', w.SelectedFile.Split('.').SkipLast(1)) + "_moleproj";
                Data.Project = new(Data.Progress,
                    dir, w.SelectedFile);
                Windows["RomInfo"].Open();
                Windows["PalEditor"].Open();
                Windows["GfxEditor"].Open();
            }).Start();
        }
        private static void OpenProjectEventHandler(WindowBase window)
        {
            Windows["Loading"].Open();
            var w = window as Dialogs.FilePicker;
            new Task(() =>
            {
                var dir = w.CurrentFolder;
                Data.Project = new(Data.Progress, dir);
                Windows["RomInfo"].Open();
                Windows["PalEditor"].Open();
                Windows["GfxEditor"].Open();
            }).Start();
        }
    }
}
