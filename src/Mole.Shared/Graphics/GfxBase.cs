using Mole.Shared.Util;
using Smallhacker.TerraCompress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mole.Shared.Graphics
{
    public class GfxBase
    {
        public uint[] Pointers;
        public byte[][] Decompressed;

        public GfxBase(ref Progress progress, ref Rom rom, string ProjDir)
        {
            if (LoadFromCache(ProjDir)) return; // Try to load from the projects cache

            LoadFromRom(ref rom, ref progress); // If that fails then load from the ROM
        }

        public void SaveCache(string dir)
        {
            var Name = Path.Combine(dir, GetType().Name);

            // Cache pointers
            var ptrBytes = new List<byte>();
            foreach (var p in Pointers)
            {
                ptrBytes.Add((byte)((p >> 24) & 0xFF));
                ptrBytes.Add((byte)((p >> 16) & 0xFF));
                ptrBytes.Add((byte)((p >> 8) & 0xFF));
                ptrBytes.Add((byte)(p & 0xFF));
            }
            Cache.SaveCache(Cache.Type.PointerTable, null, ptrBytes.ToArray(), $"{Name}_ptr");

            // Cache decompressed data
            var datBytes = new List<byte>();
            //foreach (var b in Decompressed)
            for (int i = 0; i < Decompressed.Length; i++)
            {
                var b = Decompressed[i];
                if (b is null) continue;
                datBytes.Add(Format(i).Index);
                datBytes.Add((byte)((b.Length >> 8) & 0xFF));
                datBytes.Add((byte)(b.Length & 0xFF));
                datBytes = datBytes.Concat(b).ToList();
            }
            Cache.SaveCache(Cache.Type.Graphics, null, datBytes.ToArray(), $"{Name}_dat");
        }
        public bool LoadFromCache(string ProjDir)
        {
            if (ProjDir is null) return false;
            var Name = Path.Combine(ProjDir, GetType().Name);

            // Try to load cached file, if this fails we try to atleast load the pointers
            if (!Cache.TryLoadCache($"{Name}_dat", out object datBytes))
            {
                if (Cache.TryLoadCache($"{Name}_ptr", out object ptrBytes))
                {
                    // Load cached pointers into Pointers array
                    var p = (byte[])ptrBytes;
                    var ptrs = new List<uint>();
                    for (int i = 0; i < p.Length; i += 4)
                        ptrs.Add((uint)((p[i + 2] << 16) | (p[i + 1] << 8) | p[i]));
                    Pointers = ptrs.ToArray();
                }
                return false; // Cache loading failed, so we return false, even if the pointers loaded fine
            }
            
            // Load cached data into Decompressed array
            byte[] dat = (byte[])datBytes;
            var decomp = new List<byte[]>();
            if (dat.Length < 20 ||              // Not Enough Data
                ((dat[0] << 8) | dat[1]) != 0)  // Type != 0
                return false;

            dat = dat.Skip(16).ToArray();       // Skip Header
            for (int i = 0; i < dat.Length; i++)
            {
                var fmt = dat[i++];                     // GFX Chunk Format Byte
                var len = (dat[i++] << 8) | dat[i++];   // GFX Chunk Size
                decomp.Add(dat.Skip(i).Take(len).ToArray()); // Load a chunk
                i += len - 1; // Skip to next chunk
            }
            Decompressed = decomp.ToArray();
            return true; // Cached data loading was successful
        }
        public virtual void LoadFromRom(ref Rom rom, ref Progress progress) => DecompressGfx(ref rom, ref progress.CurrentProgress, ref progress.MaxProgress);
        public void DecompressGfx(ref Rom rom, ref int progress, ref int maxProgress)
        {
            var Name = GetType().Name;
            LoggerEntry.Logger.Information($"Decompressing {Name}...");
            Lz2 lz2 = new();
            byte[][] dgfx = new byte[Pointers.Length][];
            maxProgress = Pointers.Length;
            int fails = 0;
            for (int i = 0; i < Pointers.Length; i++)
            {
                progress = i;
                try { dgfx[i] = lz2.Decompress(rom.Pc, (uint)rom.SnesToPc((int)Pointers[i])); }
                catch { LoggerEntry.Logger.Warning($"Failed to decompress {Name}{i:X2}"); fails++; }
            }
            LoggerEntry.Logger.Information($"Done! {fails}/{Pointers.Length} Failures occured.");
            Decompressed = dgfx;
        }

        public FormatBase Format(int idx) => Format(idx);
    }
}
