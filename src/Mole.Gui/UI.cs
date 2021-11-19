using Mole.Shared.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mole.Gui
{
    public partial class Ui
    {
        private static readonly Dictionary<string, Window> Windows = new()
        {
            { "About", new Windows.About() },
            //=============Dialogs=============
            { "OpenFile", new Dialogs.FilePicker("Select ROM file", "/", SearchFilter: ".smc,.sfc|.asm") },
            { "OpenProject", new Dialogs.FilePicker("Select Project Directory", "/", OnlyFolders: true) },
            { "OpenProjectFile", new Dialogs.FilePicker("Select Compressed Project File", "/", SearchFilter: ".moleproj") },
            { "OpenTestMulti", new Dialogs.FilePicker("Test Multiple Selections", "/", MultipleSelections: true, SearchFilter: ".smc,.sfc") },
            { "Loading", new Dialogs.Loading() },
            { "Error", new Dialogs.Error() },
            //=============Editors=============
            { "RomInfo", new Windows.RomInfo() },
            { "PalEditor", new Windows.PalEditor() },
            { "GfxEditor", new Windows.GfxEditor() }
        };

        private static bool _showDemo;
        private static readonly Project.UiData Data = new();
        private static void OpenFileEventHandler(Window window)
        {
            Windows["Loading"].ShouldDraw = true;
            var w = window as Dialogs.FilePicker;
            new Task(() =>
            {
                var dir = string.Join('.', w.SelectedFile.Split('.').SkipLast(1)) + "_moleproj";
                Data.Project = new Project(Data.Progress,
                    dir, w.SelectedFile);
                Windows["RomInfo"].ShouldDraw = true;
                Windows["PalEditor"].ShouldDraw = true;
                Windows["GfxEditor"].ShouldDraw = true;
            }).Start();
        }
    }
}
