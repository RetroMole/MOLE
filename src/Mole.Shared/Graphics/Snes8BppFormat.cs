using System;
using System.Collections.Generic;
using System.Linq;

namespace Mole.Shared.Graphics
{
    class Snes8BppFormat : FormatBase
    {
        public override int PalSize => 256;
        public override int CharSize => 64;
        public override int ExpectedFileSize => 8192;
        public override byte Index => 8;
        public override byte[] FromGfxToRaw(byte[][,] CharArray)
        {
            throw new NotImplementedException();
        }

        public override byte[][,] FromRawToGfx(byte[] RawData)
        {
            List<byte[,]> res = new();
            for (int c = 0; c < RawData.Length / CharSize; c++) // Loop over characters
            {
                var ch = RawData.Skip(c * CharSize).Take(CharSize); // Get char data
                // Separate all the bitplanes
                var bitplane0 = ch.SkipLast(48).Where((x, i) => (i % 2) == 0).ToList();
                var bitplane1 = ch.SkipLast(48).Where((x, i) => (i % 2) != 0).ToList();
                var bitplane2 = ch.SkipLast(32).Skip(16).Where((x, i) => (i % 2) == 0).ToList();
                var bitplane3 = ch.SkipLast(32).Skip(16).Where((x, i) => (i % 2) != 0).ToList();
                var bitplane4 = ch.SkipLast(16).Skip(32).Where((x, i) => (i % 2) == 0).ToList();
                var bitplane5 = ch.SkipLast(16).Skip(32).Where((x, i) => (i % 2) != 0).ToList();
                var bitplane6 = ch.Skip(48).Where((x, i) => (i % 2) == 0).ToList();
                var bitplane7 = ch.Skip(48).Where((x, i) => (i % 2) != 0).ToList();

                byte[,] chr = new byte[8, 8];
                for (int i = 0; i < 8; i++) // Loop over bytes
                {
                    for (int j = 0; j < 8; j++) // Loop over bits
                    {
                        // Get Jth bit of Ith byte from each bitplane
                        var bit0 = (bitplane0.ElementAt(i) >> j) & 1;
                        var bit1 = (bitplane1.ElementAt(i) >> j) & 1;
                        var bit2 = (bitplane2.ElementAt(i) >> j) & 1;
                        var bit3 = (bitplane3.ElementAt(i) >> j) & 1;
                        var bit4 = (bitplane0.ElementAt(i) >> j) & 1;
                        var bit5 = (bitplane1.ElementAt(i) >> j) & 1;
                        var bit6 = (bitplane2.ElementAt(i) >> j) & 1;
                        var bit7 = (bitplane3.ElementAt(i) >> j) & 1;
                        chr[i, j ^ 7] = (byte)((bit7 << 7) | (bit6 << 6) | (bit5 << 5) | (bit4 << 4) | (bit3 << 3) | (bit2 << 2) | (bit1 << 1) | bit0); // Combine bits into result and flip horizontally
                    }
                }
                res.Add(chr);
            }
            return res.ToArray();
        }
    }
}
