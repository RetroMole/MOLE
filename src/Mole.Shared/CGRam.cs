using Mole.Shared.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mole.Shared
{
    public class CGRam : IEnumerator<ushort>, IEnumerable<ushort>
    {
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



        public CGRam(Progress progress, Rom rom)
        {
            progress.State = Progress.StateEnum.LoadingLevelPalInfo;
            LoggerEntry.Logger.Information("Loading Level Palette info...");
            Layer3 = new PalUploadInfo(ref rom, 0xAC06, 0xAC0B, 0xAC10, 0xAC15);
            ForeGround = new PalUploadInfo(ref rom, 0xAC1D, 0xAC22, 0xAC27, 0xAC2C);
            BerryTile = new PalUploadInfo(ref rom, 0xACBD, 0xACC2, 0xACC7, 0xACCC);
            BerrySpr = new PalUploadInfo(ref rom, 0xACD4, 0xACD9, 0xACDE, 0xACE3);

            progress.State = Progress.StateEnum.LoadingTSpecPalInfo;
            LoggerEntry.Logger.Information("Loading Tileset Specific Palette info");
            FG = new PalUploadInfo(ref rom, 0xAC42, 0xAC59, 0xAC5E, 0xAC63);
            BG = new PalUploadInfo(ref rom, 0xAC94, 0xACAB, 0xACB0, 0xACB5);
            Spr = new PalUploadInfo(ref rom, 0xAC6B, 0xAC82, 0xAC87, 0xAC8C);
        }
        public void UploadPalette(ref Rom rom,
            ushort ptr, ushort off, ushort xs, ushort ys)
        {
            for (var i = 0; i <= ys; i++)
            {
                for (var j = 0; j <= xs; j++)
                {
                    this[(off / 2) + j] = (ushort)((rom[ptr + 1] << 8) | rom[ptr]);
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
                Pointer = (ushort)((rom[ptr + 1] << 8) | rom[ptr]);
                Index   = (ushort)((rom[idx + 1] << 8) | rom[idx]);
                X       = (ushort)((rom[x + 1] << 8) | rom[x]);
                Y       = (ushort)((rom[y + 1] << 8) | rom[y]);
            }
        }

        public void GenerateLevelCGRam(ref Rom rom)
        {
            LoggerEntry.Logger.Information("Generating Level CGRam...");
            _cgram = new ushort[256];
            for (int i = 0; i < 8; i++)
                this[1 + (i * 16)] = 0x7FDD;
            for (int i = 0; i < 8; i++)
                this[0x81 + (i * 16)] = 0x7FFF;
            UploadPalette(ref rom, Layer3.Pointer, Layer3.Index, Layer3.X, Layer3.Y);
            UploadPalette(ref rom, ForeGround.Pointer, ForeGround.Index, ForeGround.X, ForeGround.Y);
            UploadPalette(ref rom, BerryTile.Pointer, BerryTile.Index, BerryTile.X, BerryTile.Y);
            UploadPalette(ref rom, BerrySpr.Pointer, BerrySpr.Index, BerrySpr.X, BerrySpr.Y);

            UploadPalette(ref rom,
                (ushort)(FG.Pointer + rom[rom.SnesToPc(0xABD3) + CurrentFG]),
                FG.Index, FG.X, FG.Y);
            UploadPalette(ref rom,
                 (ushort)(BG.Pointer + rom[rom.SnesToPc(0xABD3) + CurrentBG]),
                 BG.Index, BG.X, BG.Y);
            UploadPalette(ref rom,
                 (ushort)(Spr.Pointer + rom[rom.SnesToPc(0xABD3) + CurrentSpr]),
                 Spr.Index, Spr.X, Spr.Y);
        }
        public Pal GetPal(int index, int size) => new(this.Skip(index * size).Take(size).ToArray());

        // IEnum stuff
        private int _position = -1;
        public bool MoveNext()
        {
            _position++;
            return _position < _cgram.Length;
        }
        public void Reset() => _position = 0;
        public object Current { get => _cgram[_position]; }
        ushort IEnumerator<ushort>.Current { get => _cgram[_position]; }
        public IEnumerator<ushort> GetEnumerator() => _cgram.OfType<ushort>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator<ushort>)_cgram.GetEnumerator();
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        } // TODO: Possible Memory leak, does the enumerator/enumerable stuff need manual disposal?
    }
}
