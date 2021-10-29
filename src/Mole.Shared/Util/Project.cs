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
        public Gfx Gfx;
        public Rom Rom;
        public CGRam CGRam;
        
        public readonly Dictionary<string, UndoRedo> Stacks = new Dictionary<string, UndoRedo>() {
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
            if (!File.Exists(romPath))
                throw new IOException("ROM doesn't exist!");
            
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            
            new Thread(() => {
                try {
                    progress.Working = true;
                    Rom = new Rom(romPath);
                    CGRam = new CGRam(progress, Rom);
                    Gfx = new Gfx(progress, Rom);

                    Directory.CreateDirectory(dir);
                    RomPath = Path.Combine(dir, $"rom.{Rom.FileName.Split('.').Last()}");
                    StackPaths = new List<string>
                    {
                        // Modify it as you modify stacks dictionary
                        Path.Combine(dir, "stack_test.stk"),
                    };
                    GfxPaths = new List<string>
                    {
                        Path.Combine(dir, "gfx_pointers.bin"),
                        Path.Combine(dir, "gfx_decompressed.bin"),
                        Path.Combine(dir, "exgfx_pointers.bin"),
                        Path.Combine(dir, "exgfx_decompressed.bin"),
                        Path.Combine(dir, "superexgfx_pointers.bin"),
                        Path.Combine(dir, "superexgfx_decompressed.bin")
                    };
                    _root = dir;

                    progress.MaxProgress = 0;
                    progress.CurrentProgress = 0;
                    progress.State = Progress.StateEnum.CopyingRom;
                    File.Copy(Rom.FilePath, RomPath);

                    progress.State = Progress.StateEnum.SavingProject;
                    SaveProject();

                    progress.Loaded = true;
                    progress.Working = false;
                } catch (Exception e) {
                    progress.Exception = e;
                    progress.ShowException = true;
                }
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

            new Thread(() => {
                try {
                    progress.Working = true;
                    RomPath = Path.Combine(dir, Directory.EnumerateFiles(
                        dir, $"rom.*").FirstOrDefault(x => !x.EndsWith("sha1")));
                    Rom = new(RomPath);
                    CGRam = new CGRam(progress, Rom);
                    StackPaths = new List<string> { // Modify it as you modify stacks dictionary
                        Path.Combine(dir, "stack_test.stk"),
                    };
                    GfxPaths = new List<string> {
                        Path.Combine(dir, "gfx_pointers.bin"),
                        Path.Combine(dir, "gfx_decompressed.bin"),
                        Path.Combine(dir, "exgfx_pointers.bin"),
                        Path.Combine(dir, "exgfx_decompressed.bin"),
                        Path.Combine(dir, "superexgfx_pointers.bin"),
                        Path.Combine(dir, "superexgfx_decompressed.bin")
                    };
                    _root = dir;
                    CheckAllHashes();

                    Gfx = new Gfx { SuperExGfxSupported = Rom.RomSize > 512 };
                    Gfx.GfxPointers = Gfx.ImportPointers(File.ReadAllBytes(GfxPaths[0]));
                    Gfx.DecompressedGfx = Gfx.ImportDecompressed(File.ReadAllBytes(GfxPaths[1]));
                    Gfx.ExGfxPointers = Gfx.ImportPointers(File.ReadAllBytes(GfxPaths[2]));
                    Gfx.DecompressedExGfx = Gfx.ImportDecompressed(File.ReadAllBytes(GfxPaths[3]));
                    if (Gfx.SuperExGfxSupported) {
                        Gfx.SuperExGfxPointers = Gfx.ImportPointers(File.ReadAllBytes(GfxPaths[4]));
                        Gfx.DecompressedSuperExGfx = Gfx.ImportDecompressed(File.ReadAllBytes(GfxPaths[5]));
                    }
                
                    progress.Loaded = true;
                    progress.Working = false;
                } catch (Exception e) {
                    progress.Exception = e;
                    progress.ShowException = true;
                }
            }).Start();
        }

        /// <summary>
        /// Check all hashes
        /// </summary>
        public bool CheckAllHashes()
        {
            if (!CheckHash(RomPath)) 
                throw new Exception($"Hash mismatch: {RomPath} ({Hash.Sha1(RomPath)} != {File.ReadAllText($"{RomPath}.sha1")})");
            foreach (var p in StackPaths)
                if (!CheckHash(p))
                    throw new Exception($"Hash mismatch: {p} ({Hash.Sha1(p)} != {File.ReadAllText($"{p}.sha1")})");
            foreach (var p in GfxPaths)
                if (!CheckHash(p)) 
                    throw new Exception($"Hash mismatch: {p} ({Hash.Sha1(p)} != {File.ReadAllText($"{p}.sha1")})");
            return true;
        }
        
        /// <summary>
        /// Saves the project
        /// </summary>
        public void SaveProject()
        {
            File.WriteAllBytes(GfxPaths[0], Gfx.ExportGfxPointers());
            File.WriteAllBytes(GfxPaths[1], Gfx.ExportGfxDecompressed());
            File.WriteAllBytes(GfxPaths[2], Gfx.ExportExGfxPointers());
            File.WriteAllBytes(GfxPaths[3], Gfx.ExportExGfxDecompressed());
            if (Gfx.SuperExGfxSupported) {
                File.WriteAllBytes(GfxPaths[4], Gfx.ExportSuperExGfxPointers());
                File.WriteAllBytes(GfxPaths[5], Gfx.ExportSuperExGfxDecompressed());
            } else {
                File.WriteAllBytes(GfxPaths[4], new byte[1]);
                File.WriteAllBytes(GfxPaths[5], new byte[1]);
            }
            // Modify it as you modify stacks dictionary
            File.WriteAllText(StackPaths[0], Stacks["test"].ExportForProject());
            SaveHash(RomPath);
            foreach (var p in StackPaths) 
                SaveHash(p);
            foreach (var p in GfxPaths) 
                SaveHash(p);
        }

        public static void SaveHash(string path)
            => File.WriteAllText($"{path}.sha1", Hash.Sha1(path));

        public static bool CheckHash(string path)
            => Hash.Sha1(path) == File.ReadAllText($"{path}.sha1");
    }
}