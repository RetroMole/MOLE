using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;

namespace Mole.Shared.Util
{
    public class Project
    {
        private string _root;
        public string RomPath;
        public List<string> StackPaths;
        public List<string> GfxPaths;
        public Rom Rom;
        public CGRam CGRam;
        
        public readonly Dictionary<string, UndoRedo> Stacks = new() {
            { "test", new UndoRedo(80) }
        };
        
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
        }
    }
}