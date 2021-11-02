using Mole.Shared.Util;
using Smallhacker.TerraCompress;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mole.Shared.Graphics
{
    public class GfxBase
    {
        public uint[] Pointers;
        public byte[][] Decompressed;

        public GfxBase(ref Rom rom, ref Project.UiData projData)
        {
            if (LoadFromCache()) return;

            LoadFromRom(ref rom, ref projData);
        }

        public void SaveCache()
        {
            var Name = GetType().Name;

            // Cache pointers
            var ptrBytes = new List<byte>();
            foreach (var p in Pointers)
            {
                ptrBytes.Add((byte)((p >> 24) & 0xFF));
                ptrBytes.Add((byte)((p >> 16) & 0xFF));
                ptrBytes.Add((byte)((p >> 8) & 0xFF));
                ptrBytes.Add((byte)(p & 0xFF));
            }
            Cache.SaveCache(Cache.Type.PointerTable, null, ptrBytes.ToArray(), $"{Name}_ptrs");

            // Cache decompressed data
            var datBytes = new List<byte>();
            foreach (var b in Decompressed)
            {
                if (b == null) continue;
                datBytes.Add((byte)((b.Length >> 8) & 0xFF));
                datBytes.Add((byte)(b.Length & 0xFF));
                foreach (var bb in b)
                    datBytes.Add(bb);
            }
            Cache.SaveCache(Cache.Type.Graphics, null, datBytes.ToArray(), $"{Name}_decomp");
        }
        public bool LoadFromCache()
        {
            var Name = GetType().Name;

            // Try to load cached file, if this fails we try to atleast load the pointers
            if (!Cache.TryLoadCache($"{Name}_decomp", out object datBytes))
            {
                if (!Cache.TryLoadCache($"{Name}_ptrs", out object ptrBytes))
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
            for (int i = 0; i < dat.Length; i++)
            {
                var len = (dat[i] << 8) | dat[i + 1];
                i += 2;
                decomp.Add(dat.Skip(i).Take(len).ToArray());
                i += len - 1;
            }
            Decompressed = decomp.ToArray();
            return true; // Cached data loading was successful
        }
        public void LoadFromRom(ref Rom rom, ref Project.UiData projData) => DecompressGfx(ref rom, ref projData.Progress.CurrentProgress, ref projData.Progress.MaxProgress);
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
    }
}
