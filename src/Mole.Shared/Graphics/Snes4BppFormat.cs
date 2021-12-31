using System;
using System.Collections.Generic;
using System.Linq;

namespace Mole.Shared.Graphics
{
    class Snes4BppFormat : FormatBase
    {
        public override int PalSize => 16;
        public override int CharSize => 32;
        public override int ExpectedFileSize => 4096;
        public override byte Index => 4;
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
                var bitplane0 = ch.SkipLast(16).Where((x, i) => (i % 2) == 0).ToList(); // Get even byes (0th bitplane)
                var bitplane1 = ch.SkipLast(16).Where((x, i) => (i % 2) != 0).ToList(); // Get odd bytes (1st bitplane)
                var bitplane2 = ch.Skip(16).Where((x, i) => (i % 2) == 0).ToList();
                var bitplane3 = ch.Skip(16).Where((x, i) => (i % 2) != 0).ToList();
                byte[,] chr = new byte[8, 8];
                for (int i = 0; i < 8; i++) // Loop over bytes
                {
                    for (int j = 0; j < 8; j++) // Loop over bits
                    {
                        var bit0 = (bitplane0.ElementAt(i) >> j) & 1; // Get Jth bit of Ith byte from bp0
                        var bit1 = (bitplane1.ElementAt(i) >> j) & 1; // Get Jth bit of Ith byte from bp1 
                        var bit2 = (bitplane2.ElementAt(i) >> j) & 1; // Get Jth bit of Ith byte from bp2
                        var bit3 = (bitplane3.ElementAt(i) >> j) & 1; // Get Jth bit of Ith byte from bp3
                        chr[i, j ^ 7] = (byte)((bit3 << 3) | (bit2 << 2) | (bit1 << 1) | bit0); // Combine bits into result and flip horizontally
                    }
                }
                res.Add(chr);
            }
            return res.ToArray();
        }
    }
}
