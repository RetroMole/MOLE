using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetroMole.Core.Utility
{
    public class Project
    {

        // Project Data
        private string _root;
        public string OutputRomPath;
        public string CleanRomPath;
        
        public class UiData
        {
            public Project Project;
            public Progress Progress = new();
        }

        /// <summary>
        /// Initialize project from a ROM file
        /// </summary>
        /// <param name="progress">Progress object</param>
        /// <param name="dir">Directory path</param>
        /// <param name="romPath">ROM path</param>
        public Project(Progress progress, string dir, string romPath)
        {
            if (!File.Exists(romPath))
                throw new IOException("ROM doesn't exist!");

            new Task(() =>
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

                progress.State = Progress.StateEnum.SavingProject;
                SaveProject();

                progress.Loaded = true;
                progress.Working = false;
            }).Start();
        }

        /// <summary>
        /// Initialize project from an existing _moleproj directory
        /// </summary>
        /// <param name="progress">Progress object</param>
        /// <param name="dir">Directory path</param>
        public Project(Progress progress, string dir)
        {
            if (!Directory.Exists(dir))
                throw new IOException("Directory doesn't exist!");
            new Task(() =>
            {
                _root = dir;
                progress.Working = true;
                OutputRomPath = Path.Combine(_root, Directory.EnumerateFiles(
                    _root, "OutputROM.*").Where(x => !x.EndsWith(".sha1")).First());
                CleanRomPath = Path.Combine(_root, Directory.EnumerateFiles(
                    _root, "CleanROM.*").Where(x => !x.EndsWith(".sha1")).First());
                
                progress.Loaded = true;
                progress.Working = false;
            }).Start();
        }
        
        /// <summary>
        /// TODO: Initialize project from a compressed .moleproj file
        /// </summary>
        /// <param name="progress">Progress object</param>
        /// <param name="projPath">Compressed project path</param>
        /// <param name="cleanRomPath">Clean ROM path</param>
        public Project(Progress progress, string projPath, string cleanRomPath, object ignoreThis = null) { }

        /// <summary>
        /// Saves the project
        /// </summary>
        public void SaveProject()
        {
            Cache.SaveHash(OutputRomPath);
        }
    }
}