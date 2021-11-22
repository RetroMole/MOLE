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
        public Tuple<byte[], int>[] Decompressed;
        private int[] DefaultFormats =
            Enumerable.Repeat(3, 0x27)          // First 0x27 files are 3bpp by default
            .Append(73)                         // GFX27 is Mode 7 3bpp 
            .Concat(Enumerable.Repeat(2, 4))    // GFX28-2B are 2bpp
            .Concat(Enumerable.Repeat(3, 3))    // GFX2C-2E are 3bpp
            .Append(2)                          // GFX2F  is 2bpp
            .Concat(Enumerable.Repeat(3, 4))    // GFX30-33 are 3bpp
            .ToArray();

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
            foreach (var b in Decompressed)
            {
                if (b is null) continue;
                datBytes.Add((byte)b.Item2);
                datBytes.Add((byte)((b.Item1.Length >> 8) & 0xFF));
                datBytes.Add((byte)(b.Item1.Length & 0xFF));
                foreach (var bb in b.Item1)
                    datBytes.Add(bb);
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
            var decomp = new List<Tuple<byte[], int>>();
            if (dat.Length < 20 ||              // Not Enough Data
                ((dat[0] << 8) | dat[1]) != 0)  // Type != 0
                return false;

            dat = dat.Skip(16).ToArray();       // Skip Header
            for (int i = 0; i < dat.Length; i++)
            {
                var fmt = dat[i++];                     // GFX Chunk Format Byte
                var len = (dat[i++] << 8) | dat[i++];   // GFX Chunk Size
                decomp.Add(new Tuple<byte[], int>(dat.Skip(i).Take(len).ToArray(),fmt)); // Load a chunk
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
            Tuple<byte[],int>[] dgfx = new Tuple<byte[],int>[Pointers.Length];
            maxProgress = Pointers.Length;
            int fails = 0;
            for (int i = 0; i < Pointers.Length; i++)
            {
                progress = i;
                try { dgfx[i] = new Tuple<byte[], int>(lz2.Decompress(rom.Pc, (uint)rom.SnesToPc((int)Pointers[i])), DefaultFormats.ElementAtOrDefault(i) == 0 ? 4 : DefaultFormats[i]); }
                catch { LoggerEntry.Logger.Warning($"Failed to decompress {Name}{i:X2}"); fails++; }
            }
            LoggerEntry.Logger.Information($"Done! {fails}/{Pointers.Length} Failures occured.");
            Decompressed = dgfx;
        }
    }
}
