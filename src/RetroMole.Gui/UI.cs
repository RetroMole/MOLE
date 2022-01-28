using RetroMole.Core;
using RetroMole.Core.Utility;
using Serilog;

namespace RetroMole.Gui
{
    public static partial class Ui
    {
        public static IRenderer renderer;
        public static readonly Dictionary<string, WindowBase> Windows = new()
            {
                { "About", new Windows.About() },
                //=============Dialogs=============
                { "OpenFile", new Dialogs.FilePicker("Select ROM file", "/", SearchFilter: ".smc,.sfc|.asm") },
                { "OpenProject", new Dialogs.FilePicker("Select Project Directory", "/", OnlyFolders: true) },
                { "OpenProjectFile", new Dialogs.FilePicker("Select Compressed Project File", "/", SearchFilter: ".moleproj") },
                { "Loading", new Dialogs.Loading() },
                { "Error", new Dialogs.Error() }
            };

        public static bool _ShowDemo;
        public static Project.UiData Data = new();
        public static void OpenFileEventHandler(WindowBase window)
        {
            Windows["Loading"].Open();
            var w = window as Dialogs.FilePicker;
            new Task(() =>
            {
                var dir = string.Join('.', w.SelectedFile.Split('.').SkipLast(1)) + "_moleproj";
                Data.Project = new(Data.Progress,
                    dir, w.SelectedFile);
            }).Start();
        }
        public static void OpenProjectEventHandler(WindowBase window)
        {
            Windows["Loading"].Open();
            var w = window as Dialogs.FilePicker;
            new Task(() =>
            {
                var dir = w.CurrentFolder;
                Data.Project = new(Data.Progress, dir);
            }).Start();
        }
        public static void OpenProjectFileEventHandler(WindowBase window)
        {
            Windows["Loading"].Open();
            var w = window as Dialogs.FilePicker;

            string foldername =
                Path.GetFullPath(string.Join("_",
                    string.Join('.',
                        w.SelectedFile.Split('.').SkipLast(1)
                    ),
                    w.SelectedFile.Split('.').Last()
                ));
            Log.Information("Decompressing .moleproj file");
            using (var o = new FileStream($"{foldername}.file", FileMode.OpenOrCreate))
            {
                using var i = new FileStream(Path.GetFullPath(w.SelectedFile), FileMode.Open);
                    ICSharpCode.SharpZipLib.GZip.GZip.Decompress(i, o, false);
            }
            Log.Information("Extracting .moleproj file");
            using (var o = new FileStream($"{foldername}.file", FileMode.Open))
                ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(o)
                     .ExtractContents(foldername, true);

            File.Delete($"{foldername}.file");

            new Task(() =>
            {
                Log.Information("Opening extracted project");
                var dir = foldername;
                Data.Project = new(Data.Progress, dir);
                Windows["RomInfo"].Open();
                Windows["PalEditor"].Open();
                Windows["GfxEditor"].Open();
            }).Start();
        }
    }
}
