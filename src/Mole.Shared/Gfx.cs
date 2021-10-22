using System;
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
        public enum StateEnum
        {
            LoadingGfx,
            LoadingExGfx,
            LoadingSuperExGfx,
            DecompressingGfx,
            CleaningUpGfx,
            DecompressingExGfx,
            CleaningUpExGfx,
            DecompressingSuperExGfx,
            CleaningUpSuperExGfx,
        }
        
        public uint[] ExGfxPointers = new uint[0x80];
        public uint[] GfxPointers = new uint[0x34];
        public uint[] SuperExGfxPointers = new uint[0xF00];
        public bool SuperExGfxSupported = true;
        public byte[][] DecompressedGfx;
        public byte[][] DecompressedExGfx;
        public byte[][] DecompressedSuperExGfx;
        public int MaxProgress = 0;
        public int Progress = 0;
        public bool Loaded;
        public StateEnum State;

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public static void NewRef(ref Gfx gfx, Rom rom)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            gfx.State = StateEnum.LoadingGfx;
            LoggerEntry.Logger.Information("Getting GFX Pointers...");
            var low = rom.Skip(rom.SnesToPc(0x00B992)).Take(0x32).ToArray();
            var high = rom.Skip(rom.SnesToPc(0x00B9C4)).Take(0x32).ToArray();
            var bank = rom.Skip(rom.SnesToPc(0x00B9F6)).Take(0x32).ToArray();
            gfx.MaxProgress = 0x32;
            for (var i = 0; i < 0x32; i++) {
                gfx.Progress = i;
                gfx.GfxPointers[i] = BitConverter.ToUInt32(new byte[] {low[i], high[i], bank[i], 0});
            }
            gfx.GfxPointers[0x32] = 0x088000;
            gfx.GfxPointers[0x33] = 0x08BFC0;

            gfx.State = StateEnum.LoadingExGfx;
            LoggerEntry.Logger.Information("Getting ExGFX Pointers...");
            var ex = rom.Skip(rom.SnesToPc(0x0FF600)).Take(0x180).ToArray();
            gfx.MaxProgress = 0x80;
            for (var i = 0; i < 0x80; i++) {
                gfx.Progress = i;
                gfx.ExGfxPointers[i] = Helper.B2Ul(ex.Skip(i * 3).Take(3).ToArray());
            }

            gfx.State = StateEnum.LoadingSuperExGfx;
            if (rom.RomSize <= 512) {
                LoggerEntry.Logger.Warning("Unexpanded ROM, SuperExGFX can't be used");
                gfx.SuperExGfxSupported = false;
            } else {
                LoggerEntry.Logger.Information("Getting SuperExGFX Pointers...");
                var supex = rom.Skip(rom.SnesToPc((int) Helper.B2Ul(rom.Skip(rom.SnesToPc(0x0FF937)).Take(3).ToArray())))
                    .Take(0x2D00).ToArray();
                gfx.MaxProgress = 0xF00;
                for (var i = 0; i < 0xF00; i++) {
                    gfx.Progress = i;
                    gfx.SuperExGfxPointers[i] = Helper.B2Ul(supex.Skip(i * 3).Take(3).ToArray());
                }
            }

            gfx.DecompressedGfx = DecompressGfx(gfx.GfxPointers, rom, ref gfx.Progress, ref gfx.MaxProgress, ref gfx.State, 
                StateEnum.DecompressingGfx, StateEnum.CleaningUpGfx, "GFX");
            
            gfx.DecompressedExGfx = DecompressGfx(gfx.ExGfxPointers, rom, ref gfx.Progress, ref gfx.MaxProgress, ref gfx.State, 
                StateEnum.DecompressingExGfx, StateEnum.CleaningUpExGfx, "ExGFX");
            
            if (gfx.SuperExGfxSupported)
                gfx.DecompressedSuperExGfx = DecompressGfx(gfx.SuperExGfxPointers, rom, ref gfx.Progress, ref gfx.MaxProgress, ref gfx.State,
                    StateEnum.DecompressingSuperExGfx, StateEnum.CleaningUpSuperExGfx, "SuperExGFX");
            
            gfx.Loaded = true;
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
        public static byte[][] DecompressGfx(uint[] gfxPointers, Rom rom, ref int progress, ref int maxProgress, ref StateEnum state, 
            StateEnum decompressing, StateEnum cleaning, string postfix)
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
