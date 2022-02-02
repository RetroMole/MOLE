using RetroMole.Core.Interfaces;
using Serilog;

namespace RetroMole.Events
{
    public static class Ui
    {
        public static event Action OnMenuBar;
        public static void TriggerMenuBar() => OnMenuBar.Invoke();


        // LEGACY
        public static void OpenFileEventHandler(WindowBase window)
        {
            Gui.Ui.Windows["Loading"].Open();
            var w = window as Gui.Dialogs.FilePicker;
            new Task(() =>
            {
                var dir = string.Join('.', w.SelectedFile.Split('.').SkipLast(1)) + "_moleproj";
                Gui.Ui.Data.Project = new(Gui.Ui.Data.Progress,
                    dir, w.SelectedFile);
            }).Start();
        }
        public static void OpenProjectEventHandler(WindowBase window)
        {
            Gui.Ui.Windows["Loading"].Open();
            var w = window as Gui.Dialogs.FilePicker;
            new Task(() =>
            {
                var dir = w.CurrentFolder;
                Gui.Ui.Data.Project = new(Gui.Ui.Data.Progress, dir);
            }).Start();
        }
        public static void OpenProjectFileEventHandler(WindowBase window)
        {
            Gui.Ui.Windows["Loading"].Open();
            var w = window as Gui.Dialogs.FilePicker;

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
                ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(o, System.Text.Encoding.UTF8)
                     .ExtractContents(foldername, true);

            File.Delete($"{foldername}.file");

            new Task(() =>
            {
                Log.Information("Opening extracted project");
                var dir = foldername;
                Gui.Ui.Data.Project = new(Gui.Ui.Data.Progress, dir);
            }).Start();
        }
    }
}
