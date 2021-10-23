﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mole.Shared.Util;
using Smallhacker.TerraCompress;

namespace Mole.Shared
{
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
    public class Gfx
    {
        public uint[] ExGfxPointers = new uint[0x80];
        public uint[] GfxPointers = new uint[0x34];
        public uint[] SuperExGfxPointers = new uint[0xF00];
        public bool SuperExGfxSupported = true;
        public byte[][] DecompressedGfx;
        public byte[][] DecompressedExGfx;
        public byte[][] DecompressedSuperExGfx;

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public Gfx(Progress progress, Rom rom)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            progress.State = Progress.StateEnum.LoadingGfx;
            LoggerEntry.Logger.Information("Getting GFX Pointers...");
            var low = rom.Skip(rom.SnesToPc(0x00B992)).Take(0x32).ToArray();
            var high = rom.Skip(rom.SnesToPc(0x00B9C4)).Take(0x32).ToArray();
            var bank = rom.Skip(rom.SnesToPc(0x00B9F6)).Take(0x32).ToArray();
            progress.MaxProgress = 0x32;
            for (var i = 0; i < 0x32; i++) {
                progress.CurrentProgress = i;
                GfxPointers[i] = BitConverter.ToUInt32(new byte[] {low[i], high[i], bank[i], 0});
            }
            GfxPointers[0x32] = 0x088000;
            GfxPointers[0x33] = 0x08BFC0;

            progress.State = Progress.StateEnum.LoadingExGfx;
            LoggerEntry.Logger.Information("Getting ExGFX Pointers...");
            var ex = rom.Skip(rom.SnesToPc(0x0FF600)).Take(0x180).ToArray();
            progress.MaxProgress = 0x80;
            for (var i = 0; i < 0x80; i++) {
                progress.CurrentProgress = i;
                ExGfxPointers[i] = Helper.B2Ul(ex.Skip(i * 3).Take(3).ToArray());
            }

            progress.State = Progress.StateEnum.LoadingSuperExGfx;
            if (rom.RomSize <= 512) {
                LoggerEntry.Logger.Warning("Unexpanded ROM, SuperExGFX can't be used");
                SuperExGfxSupported = false;
            } else {
                LoggerEntry.Logger.Information("Getting SuperExGFX Pointers...");
                var supex = rom.Skip(rom.SnesToPc((int) Helper.B2Ul(rom.Skip(rom.SnesToPc(0x0FF937)).Take(3).ToArray())))
                    .Take(0x2D00).ToArray();
                progress.MaxProgress = 0xF00;
                for (var i = 0; i < 0xF00; i++) {
                    progress.CurrentProgress = i;
                    SuperExGfxPointers[i] = Helper.B2Ul(supex.Skip(i * 3).Take(3).ToArray());
                }
            }

            DecompressedGfx = DecompressGfx(GfxPointers, rom, ref progress.CurrentProgress, ref progress.MaxProgress, ref progress.State, 
                Progress.StateEnum.DecompressingGfx, Progress.StateEnum.CleaningUpGfx, "GFX");
            
            DecompressedExGfx = DecompressGfx(ExGfxPointers, rom, ref progress.CurrentProgress, ref progress.MaxProgress, ref progress.State, 
                Progress.StateEnum.DecompressingExGfx, Progress.StateEnum.CleaningUpExGfx, "ExGFX");
            
            if (SuperExGfxSupported)
                DecompressedSuperExGfx = DecompressGfx(SuperExGfxPointers, rom, ref progress.CurrentProgress, ref progress.MaxProgress, ref progress.State,
                    Progress.StateEnum.DecompressingSuperExGfx, Progress.StateEnum.CleaningUpSuperExGfx, "SuperExGFX");
        }

        public Gfx() { }
        
        private byte[] ExportTemplatePointers(uint[] array)
        {
            var bytes = new List<byte>();
            foreach (var b in array)
                foreach (var bb in BitConverter.GetBytes(b))
                    bytes.Add(bb);
            return bytes.ToArray();
        }
        
        private byte[] ExportTemplateDecompressed(byte[][] array)
        {
            var bytes = new List<byte>();
            foreach (var b in array) {
                if (b == null) continue;
                foreach (var bb in b)
                    bytes.Add(bb);
            }
            return bytes.ToArray();
        }
        
        public uint[] ImportPointers(byte[] array)
        {
            var bytes = new List<uint>();
            for (int i = 0; i < array.Length; i += 4)
                bytes.Add(BitConverter.ToUInt32(array.Skip(i).Take(4).ToArray()));
            return bytes.ToArray();
        }
        
        public byte[][] ImportDecompressed(byte[] array)
        {
            var bytes = new List<byte[]>();
            for (int i = 0; i < array.Length; i += 64)
                bytes.Add(array.Skip(i).Take(64).ToArray());
            return bytes.ToArray();
        }

        public byte[] ExportGfxPointers() => ExportTemplatePointers(GfxPointers);
        public byte[] ExportGfxDecompressed() => ExportTemplateDecompressed(DecompressedGfx);
        public byte[] ExportExGfxPointers() => ExportTemplatePointers(ExGfxPointers);
        public byte[] ExportExGfxDecompressed() => ExportTemplateDecompressed(DecompressedExGfx);
        public byte[] ExportSuperExGfxPointers() => ExportTemplatePointers(SuperExGfxPointers);
        public byte[] ExportSuperExGfxDecompressed() => ExportTemplateDecompressed(DecompressedSuperExGfx);

        /// <summary>
        /// Decompresses GFX
        /// </summary>
        /// <param name="gfxPointers">GFX Pointers</param>
        /// <param name="rom">ROM</param>
        /// <param name="progress">Progress</param>
        /// <param name="maxProgress">Max Progress</param>
        /// <param name="state">State reference</param>
        /// <param name="decompressing">Decompressing State</param>
        /// <param name="cleaning">Cleaning State</param>
        /// <param name="postfix">Logger postfix</param>
        /// <returns>Decompressed GFX</returns>
        // ReSharper disable once RedundantAssignment
        public static byte[][] DecompressGfx(uint[] gfxPointers, Rom rom, ref int progress, ref int maxProgress, ref Progress.StateEnum state, 
            Progress.StateEnum decompressing, Progress.StateEnum cleaning, string postfix)
        {
            state = decompressing;
            LoggerEntry.Logger.Information($"Decompressing {postfix}...");
            Lz2 lz2 = new();
            byte[][] dgfx = new byte[gfxPointers.Length][];
            maxProgress = gfxPointers.Length;
            for (int i = 0; i < gfxPointers.Length; i++) {
                progress = i;
                try { dgfx[i] = lz2.Decompress(rom.ToArray(), (uint)rom.SnesToPc((int) gfxPointers[i])); }
                catch { LoggerEntry.Logger.Information($"Failed to decompress {i} {postfix}"); }
            }
            state = cleaning;
            LoggerEntry.Logger.Information($"Cleaning up {postfix}...");
            maxProgress = dgfx.GetLength(0);
            for (int i = 0; i < dgfx.GetLength(0); i++) {
                progress = i;
                try { dgfx[i] = dgfx[i].Take(64).ToArray(); }
                catch { LoggerEntry.Logger.Information($"Failed to clean up {i} {postfix}"); }
            }
            LoggerEntry.Logger.Information($"Done!");
            return dgfx;
        }
    }
}
