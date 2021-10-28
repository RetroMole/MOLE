using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mole.Shared
{
    public class CGRam
    {
        public bool Loaded;

        public ushort this[int index]
        {
            get => _cgram[index];
            set => _cgram[index] = value;
        }
        private ushort[] _cgram = new ushort[256];

        PalUploadInfo Layer3;
        PalUploadInfo ForeGround;
        PalUploadInfo BerryTile;
        PalUploadInfo BerrySpr;

        PalUploadInfo FG;
        PalUploadInfo BG;
        PalUploadInfo Spr;
        public int CurrentFG;
        public int CurrentBG;
        public int CurrentSpr;

        public static void NewRef(ref CGRam cgram, Rom rom)
        {
            LoggerEntry.Logger.Information("Loading Level Palette info...");
            cgram.Layer3 = new PalUploadInfo(ref rom, 0xAC06, 0xAC0B, 0xAC10, 0xAC15);
            cgram.ForeGround = new PalUploadInfo(ref rom, 0xAC1D, 0xAC22, 0xAC27, 0xAC2C);
            cgram.BerryTile = new PalUploadInfo(ref rom, 0xACBD, 0xACC2, 0xACC7, 0xACCC);
            cgram.BerrySpr = new PalUploadInfo(ref rom, 0xACD4, 0xACD9, 0xACDE, 0xACE3);

            LoggerEntry.Logger.Information("Loading Tileset Specific Palette info");
            cgram.FG = new PalUploadInfo(ref rom, 0xAC42, 0xAC59, 0xAC5E, 0xAC63);
            cgram.BG = new PalUploadInfo(ref rom, 0xAC94, 0xACAB, 0xACB0, 0xACB5);
            cgram.Spr = new PalUploadInfo(ref rom, 0xAC6B, 0xAC82, 0xAC87, 0xAC8C);

            cgram.Loaded = true;
        }
        public static void UploadPalette(ref CGRam cgram, ref Rom rom,
            ushort ptr, ushort off, ushort xs, ushort ys)
        {
            for (var i = 0; i <= ys; i++)
            {
                for (var j = 0; j <= xs; j++)
                {
                    cgram[(off / 2) + j] = (ushort)((rom[rom.SnesToPc(ptr + 1)] << 8) | rom[rom.SnesToPc(ptr)]);
                    ptr += 2;
                }
                off += 32;
            }
        }

        public struct PalUploadInfo
        {
            public ushort Pointer;
            public ushort Index;
            public ushort X;
            public ushort Y;
            public PalUploadInfo(ref Rom rom, ushort ptr, ushort idx, ushort x, ushort y)
            {
                Pointer = (ushort)((rom[rom.SnesToPc(ptr + 1)] << 8) | rom[rom.SnesToPc(ptr)]);
                Index   = (ushort)((rom[rom.SnesToPc(idx + 1)] << 8) | rom[rom.SnesToPc(idx)]);
                X       = (ushort)((rom[rom.SnesToPc(x + 1)] << 8) | rom[rom.SnesToPc(x)]);
                Y       = (ushort)((rom[rom.SnesToPc(y + 1)] << 8) | rom[rom.SnesToPc(y)]);
            }
        }

        public static void GenerateLevelCGRam(ref CGRam cgram, ref Rom rom)
        {
            LoggerEntry.Logger.Information("Generating Level CGRam...");
            cgram._cgram = new ushort[256];
            for (int i = 0; i < 8; i++)
                cgram[1 + (i * 16)] = 0x7FDD;
            for (int i = 0; i < 8; i++)
                cgram[0x81 + (i * 16)] = 0x7FFF;
            UploadPalette(ref cgram, ref rom, cgram.Layer3.Pointer, cgram.Layer3.Index, cgram.Layer3.X, cgram.Layer3.Y);
            UploadPalette(ref cgram, ref rom, cgram.ForeGround.Pointer, cgram.ForeGround.Index, cgram.ForeGround.X, cgram.ForeGround.Y);
            UploadPalette(ref cgram, ref rom, cgram.BerryTile.Pointer, cgram.BerryTile.Index, cgram.BerryTile.X, cgram.BerryTile.Y);
            UploadPalette(ref cgram, ref rom, cgram.BerrySpr.Pointer, cgram.BerrySpr.Index, cgram.BerrySpr.X, cgram.BerrySpr.Y);

            UploadPalette(ref cgram, ref rom,
                (ushort)(cgram.FG.Pointer + rom[rom.SnesToPc(0xABD3) + cgram.CurrentFG]),
                cgram.FG.Index, cgram.FG.X, cgram.FG.Y);
            UploadPalette(ref cgram, ref rom,
                 (ushort)(cgram.BG.Pointer + rom[rom.SnesToPc(0xABD3) + cgram.CurrentBG]),
                 cgram.BG.Index, cgram.BG.X, cgram.BG.Y);
            UploadPalette(ref cgram, ref rom,
                 (ushort)(cgram.Spr.Pointer + rom[rom.SnesToPc(0xABD3) + cgram.CurrentSpr]),
                 cgram.Spr.Index, cgram.Spr.X, cgram.Spr.Y);
        }
    }
}
