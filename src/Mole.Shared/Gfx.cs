using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mole.Shared.Util;
using Smallhacker.TerraCompress;

namespace Mole.Shared
{
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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

        /// <summary>
        /// Export all GFX info
        /// </summary>
        public byte[] ExportForProject()
        {
            // Header
            var bytes = new List<byte> { SuperExGfxSupported ? (byte)0x1 : (byte)0x0 };
            // Pointers
            foreach (byte b in BitConverter.GetBytes(GfxPointers.Length))
                bytes.Add(b);
            foreach (byte b in BitConverter.GetBytes(ExGfxPointers.Length))
                bytes.Add(b);
            if (SuperExGfxSupported) // Store only if SuperExGfx supported
                foreach (byte b in BitConverter.GetBytes(SuperExGfxPointers.Length))
                    bytes.Add(b);
            // Decompressed
            foreach (byte b in BitConverter.GetBytes(DecompressedGfx.Length))
                bytes.Add(b);
            foreach (byte b in BitConverter.GetBytes(DecompressedExGfx.Length))
                bytes.Add(b);
            if (SuperExGfxSupported) // Store only if SuperExGfx supported
                foreach (byte b in BitConverter.GetBytes(DecompressedSuperExGfx.Length))
                    bytes.Add(b);
            // Data
            // Pointers
            foreach (uint b in GfxPointers)
            foreach(byte bb in BitConverter.GetBytes(b))
                bytes.Add(bb);
            foreach (uint b in ExGfxPointers)
            foreach(byte bb in BitConverter.GetBytes(b))
                bytes.Add(bb);
            if (SuperExGfxSupported) // Store only if SuperExGfx supported
                foreach (uint b in SuperExGfxPointers)
                foreach(byte bb in BitConverter.GetBytes(b))
                    bytes.Add(bb);
            // Decompressed
            foreach (byte[] b in DecompressedGfx)
            foreach (byte bb in b)
                bytes.Add(bb);
            foreach (byte[] b in DecompressedExGfx)
            foreach (byte bb in b)
                bytes.Add(bb);
            if (SuperExGfxSupported) // Store only if SuperExGfx supported
                foreach (byte[] b in DecompressedSuperExGfx)
                foreach (byte bb in b)
                    bytes.Add(bb);
            return bytes.ToArray();
        }

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
