using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Mole.Shared.Util
{
    public class Project
    {
        // Statics
        public static Dictionary<int, Graphics.FormatBase> Formats = new()
        {
            { 2, new Graphics.Snes2BppFormat() },
            { 3, new Graphics.Snes3BppFormat() },
            { 4, new Graphics.Snes4BppFormat() },
            { 8, new Graphics.Snes8BppFormat() },
            { 73, new Graphics.Mode73BppFormat() }
        };

        // Project Data
        private string _root;
        public string OutputRomPath;
        public string CleanRomPath;
        public Rom Rom;
        public Graphics.CGRam CGRam;
        public Graphics.Gfx Gfx;
        public Graphics.ExGfx ExGfx;
        public Graphics.SuperExGfx SuperExGfx;
        public bool SuperExGfxSupported;
        
        public class UiData
        {
            public Project Project;
            public Progress Progress = new();
        }

        /// <summary>
        /// Create an instance of Project from scratch
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <param name="dir">Directory</param>
        /// <param name="romPath">ROM</param>
        public Project(Progress progress, string dir, string romPath)
        {
            if (!File.Exists(romPath))
                throw new IOException("ROM doesn't exist!");

            new Thread(() =>
            {
                progress.Working = true;

                if (Directory.Exists(dir)) Directory.Delete(dir, true); //TODO: Show confirmation popup, also there's an UnauthorizedAccessException
                Directory.CreateDirectory(dir);
                _root = dir;
                OutputRomPath = Path.Combine(_root, $"OutputROM.{romPath.Split('.').Last()}");
                CleanRomPath = Path.Combine(_root, $"CleanROM.{romPath.Split('.').Last()}");
                

                progress.MaxProgress = 2;
                progress.CurrentProgress = 0;
                progress.State = Progress.StateEnum.CopyingRom;
                File.Copy(romPath, CleanRomPath, true);
                progress.CurrentProgress = 1;
                File.Copy(romPath, OutputRomPath, true);
                progress.CurrentProgress = 2;

                progress.MaxProgress = 1;
                progress.CurrentProgress = 0;
                progress.State = Progress.StateEnum.LoadingRom;
                Rom = new Rom(OutputRomPath);
                progress.CurrentProgress = 1;

                progress.MaxProgress = progress.CurrentProgress = 0;
                CGRam = new Graphics.CGRam(progress, Rom);

                progress.State = Progress.StateEnum.LoadingGfx;
                Gfx = new Graphics.Gfx(ref progress, ref Rom, _root);

                progress.State = Progress.StateEnum.LoadingExGfx;
                ExGfx = new Graphics.ExGfx(ref progress, ref Rom, _root);

                SuperExGfxSupported = Rom.RomSize > 512;
                if (SuperExGfxSupported)
                {
                    progress.State = Progress.StateEnum.LoadingSuperExGfx;
                    SuperExGfx = new Graphics.SuperExGfx(ref progress, ref Rom, _root);
                }

                progress.State = Progress.StateEnum.SavingProject;
                SaveProject();

                progress.Loaded = true;
                progress.Working = false;
            }).Start();
        }

        /// <summary>
        /// Initializes project from an existing one
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <param name="dir">Directory</param>
        public Project(Progress progress, string dir)
        {
            if (!Directory.Exists(dir))
                throw new IOException("Directory doesn't exist!");
            new Thread(() =>
            {
                _root = dir;
                progress.Working = true;
                OutputRomPath = Path.Combine(_root, Directory.EnumerateFiles(
                    _root, "OutputROM.*").Where(x => !x.EndsWith(".sha1")).First());
                CleanRomPath = Path.Combine(_root, Directory.EnumerateFiles(
                    _root, "CleanROM.*").Where(x => !x.EndsWith(".sha1")).First());

                Rom = new(OutputRomPath);
                CGRam = new Graphics.CGRam(progress, Rom);

                SuperExGfxSupported = Rom.RomSize > 512;

                Gfx = new Graphics.Gfx(ref progress, ref Rom, _root);
                ExGfx = new Graphics.ExGfx(ref progress, ref Rom, _root);
                if (SuperExGfxSupported)
                    SuperExGfx = new Graphics.SuperExGfx(ref progress, ref Rom, _root);
                
                progress.Loaded = true;
                progress.Working = false;
            }).Start();
        }
        
        /// <summary>
        /// Saves the project
        /// </summary>
        public void SaveProject()
        {
            Gfx.SaveCache(_root);
            ExGfx.SaveCache(_root);
            if (SuperExGfxSupported)
                SuperExGfx.SaveCache(_root);
            Cache.SaveHash(OutputRomPath);
            Rom.Dispose();
        }
    }
}