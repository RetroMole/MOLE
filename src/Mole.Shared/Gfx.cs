using System;
using System.Collections.Generic;
using System.Linq;
using Mole.Shared.Util;
using Smallhacker.TerraCompress;

namespace Mole.Shared
{
    public class Gfx
    {
        public uint[] ExGfxPointers = new uint[0x80];
        public uint[] GfxPointers = new uint[0x34];
        public uint[] SuperExGfxPointers = new uint[0xF00];
        public bool SuperExGfxSupported = true;
        public byte[][] DecompressedGfx;
        public byte[][] DecompressedExGfx;
        public byte[][] DecompressedSuperExGfx;
        public Format[] GfxFormats = Enumerable.Repeat(Format.Ambiguous3or4Bpp, 0x27) //00-26
        .Concat(new Format[]
        {
            Format.Mode73Bpp,       //27
            Format.Snes2Bpp,        //28
            Format.Snes2Bpp,        //29
            Format.Snes2Bpp,        //2A
            Format.Snes2Bpp,        //2A
            Format.Ambiguous3or4Bpp,//2C
            Format.Ambiguous3or4Bpp,//2D
            Format.Ambiguous3or4Bpp,//2E
            Format.Snes2Bpp,        //2F
            Format.Snes4Bpp,        //30
            Format.Snes4Bpp,        //31
            Format.Ambiguous3or4Bpp,//32
            Format.Ambiguous3or4Bpp //33
        }).ToArray();
        public enum Format
        {
            Ambiguous3or4Bpp,
            Snes2Bpp,
            Snes3Bpp,
            Snes4Bpp,
            Snes8Bpp,
            Mode73Bpp
        }

        public Gfx(Progress progress, Rom rom)
        {
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
                Progress.StateEnum.DecompressingGfx, "GFX");
            
            DecompressedExGfx = DecompressGfx(ExGfxPointers, rom, ref progress.CurrentProgress, ref progress.MaxProgress, ref progress.State, 
                Progress.StateEnum.DecompressingExGfx, "ExGFX");
            
            if (SuperExGfxSupported)
                DecompressedSuperExGfx = DecompressGfx(SuperExGfxPointers, rom, ref progress.CurrentProgress, ref progress.MaxProgress, ref progress.State,
                    Progress.StateEnum.DecompressingSuperExGfx, "SuperExGFX");
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
            var res = new List<byte>();
            foreach (var b in array) {
                if (b == null) continue;
                res.Add((byte)((b.Length >> 8) & 0xFF));
                res.Add((byte)(b.Length & 0xFF));
                foreach (var bb in b)
                    res.Add(bb);
            }
            return res.ToArray();
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
            var res = new List<byte[]>();
            for (int i = 0; i < array.Length; i++)
            {
                var len = (array[i] << 8) | array[i+1];
                i += 2;
                res.Add(array.Skip(i).Take(len).ToArray());
                i += len-1;
            }
            return res.ToArray();
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
        /// <param name="postfix">Logger postfix</param>
        /// <returns>Decompressed GFX</returns>
        public static byte[][] DecompressGfx(uint[] gfxPointers, Rom rom, ref int progress, ref int maxProgress, ref Progress.StateEnum state, 
            Progress.StateEnum decompressing, string postfix)
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
            LoggerEntry.Logger.Information($"Done!");
            return dgfx;
        }
    }
}
