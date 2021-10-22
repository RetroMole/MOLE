using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;

namespace Mole.Shared.Util
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class Project
    {
        private string _root;
        public string RomPath;
        public string StackPath;
        public string GfxPath;
        public Gfx Gfx;
        public Rom Rom;
        public readonly UndoRedo UndoRedo = new();

        /// <summary>
        /// Create an instance of Project from scratch
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <param name="dir">Directory</param>
        /// <param name="romPath">ROM</param>
        public Project(Progress progress, string dir, string romPath)
        {
            if (Directory.Exists(dir))
                throw new IOException("Directory already exists!");

            if (!File.Exists(romPath))
                throw new IOException("ROM doesn't exist!");
            
            new Thread(() => {
                Rom = new Rom(romPath);
                Gfx = new Gfx(progress, Rom);
            
                Directory.CreateDirectory(dir);
                RomPath = Path.Combine(dir, $"rom.{Rom.FileName.Split('.').Last()}");
                StackPath = Path.Combine(dir, "stack.stk");
                GfxPath = Path.Combine(dir, "gfx.dat");
                _root = dir;

                progress.State = Progress.StateEnum.CopyingRom;
                File.Copy(Rom.FilePath, RomPath);
                
                progress.State = Progress.StateEnum.CreatingProjectFiles;
                File.WriteAllBytes(GfxPath, Gfx.ExportForProject());
                File.Create(StackPath);
            
                progress.State = Progress.StateEnum.SavingHashes;
                SaveHash(StackPath);
                SaveHash(RomPath);
                SaveHash(GfxPath);
                
                progress.Loaded = true;
            }).Start();
        }

        /// <summary>
        /// Initializes project from an existing one
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <param name="dir">Directory</param>
        public Project(Progress progress, string dir)
        {
            
        }

        /// <summary>
        /// Saves the project
        /// </summary>
        public void SaveProject()
        {
            File.WriteAllBytes(GfxPath, Gfx.ExportForProject());
            File.WriteAllText(StackPath, UndoRedo.ExportForProject());
            SaveHash(StackPath);
            SaveHash(RomPath);
            SaveHash(GfxPath);
        }

        public static void SaveHash(string path)
            => File.WriteAllText($"{path}.sha1", Hash.Sha1(path));
    }
}