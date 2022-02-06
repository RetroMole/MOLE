using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;

namespace RetroMole.Gui
{
    public static partial class Ui
    {
        public static RenderManager Rmngr;
        public static ModuleManager GMmngr;
        public static readonly Dictionary<string, WindowBase> Windows = new()
            {
                { "About", new Windows.About() },
                { "AsarTest", new Windows.AsarTest() },
                //=============Dialogs=============
                { "OpenFile", new Dialogs.FilePicker("Select ROM file", "/", SearchFilter: ".smc,.sfc|.asm") },
                { "OpenProject", new Dialogs.FilePicker("Select Project Directory", "/", OnlyFolders: true) },
                { "OpenProjectFile", new Dialogs.FilePicker("Select Compressed Project File", "/", SearchFilter: ".moleproj") },
                { "Loading", new Dialogs.Loading() },
                { "Error", new Dialogs.Error() }
            };

        public static bool _ShowDemo;
        public static Project.UiData Data = new();
    }
}
