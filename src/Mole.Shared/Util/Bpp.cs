using System.Linq;
using System;
using System.Collections.Generic;

namespace Mole.Shared.Util
{
    public static class Bpp
    {
        public static byte[,] GetChr2bpp(byte[] data, int chr)
        {
            var ch = data.Skip(16 * chr).Take(16); // Get char data
            var bitplane0 = ch.Where((x, i) => (i % 2) == 0).ToList(); // Get even byes (0th bitplane)
            var bitplane1 = ch.Where((x, i) => (i % 2) != 0).ToList(); // Get odd bytes (1st bitplane)
            byte[,] res = new byte[8, 8];
            for (int i = 0; i < 8; i++) // Loop over bytes
            {
                for (int j = 0; j < 8; j++) // Loop over bits
                {
                    var bit0 = (bitplane0.ElementAt(i)>>j)&1; // Get Jth bit into 1st position 
                    var bit1 = (bitplane1.ElementAt(i)>>j)&1; // Get Jth bit into 1st position
                    res[i,j^7] = (byte)((bit1 << 1) | bit0); // Combine bits into result and flip horizontally
                }
            }
            return res;
        }
        public static byte[,] GetChr3bpp(byte[] data, int chr)
        {
            var ch = data.Skip(24 * chr).Take(24); // Get char data
            var bitplane0 = ch.SkipLast(8).Where((x, i) => (i % 2) == 0).ToList(); // Get even byes (0th bitplane)
            var bitplane1 = ch.SkipLast(8).Where((x, i) => (i % 2) != 0).ToList(); // Get odd bytes (1st bitplane)
            var bitplane2 = ch.TakeLast(8); // Get last 8 bytes (2nd bitplane)
            byte[,] res = new byte[8, 8];
            for (int i = 0; i < 8; i++) // Loop over bytes
            {
                for (int j = 0; j < 8; j++) // Loop over bits
                {
                    var bit0 = (bitplane0.ElementAt(i) >> j) & 1; // Get Jth bit of byte from bp0
                    var bit1 = (bitplane1.ElementAt(i) >> j) & 1; // Get Jth bit of byte from bp1 
                    var bit2 = (bitplane2.ElementAt(i) >> j) & 1; // Get Jth bit of byte from bp2
                    res[i, j ^ 7] = (byte)((bit2 << 2) | (bit1 << 1) | bit0); // Combine bits into result and flip horizontally
                }
            }
            return res;
        }
        public static byte[,] GetChr4bpp(byte[] data, int chr) => new byte[,] { };
        public static byte[,] GetChr8bpp(byte[] data, int chr) => new byte[,] { };
        public static byte[,] GetChrMode73bpp(byte[] data, int chr)
        {
            var ch = data.Skip(24 * chr).Take(24); // Get char data
            var bitplane0 = ch.Take(8); // Get even byes (0th bitplane)
            var bitplane1 = ch.Skip(8).Take(8); // Get odd bytes (1st bitplane)
            var bitplane2 = ch.TakeLast(8); // Get last 8 bytes (2nd bitplane)
            byte[,] res = new byte[8, 8];
            for (int i = 0; i < 8; i++) // Loop over bytes
            {
                for (int j = 0; j < 8; j++) // Loop over bits
                {
                    var bit0 = (bitplane0.ElementAt(i) >> j) & 1; // Get Jth bit of byte from bp0
                    var bit1 = (bitplane1.ElementAt(i) >> j) & 1; // Get Jth bit of byte from bp1 
                    var bit2 = (bitplane2.ElementAt(i) >> j) & 1; // Get Jth bit of byte from bp2
                    res[i, j ^ 7] = (byte)((bit2 << 2) | (bit1 << 1) | bit0); // Combine bits into result and flip horizontally
                }
            }
            return res;
        }


        public static byte[,] GetChr(byte[] data, int chr, Gfx.Format format) => format switch
        {
            Gfx.Format.Snes2Bpp => GetChr2bpp(data, chr),
            Gfx.Format.Snes3Bpp => GetChr3bpp(data, chr),
            Gfx.Format.Snes4Bpp => GetChr4bpp(data, chr),
            Gfx.Format.Snes8Bpp => GetChr8bpp(data, chr),
            Gfx.Format.Mode73Bpp => GetChrMode73bpp(data, chr),
            Gfx.Format.Ambiguous3or4Bpp or _ => GetChr(data, chr, data.Length == 0xC00 ? Gfx.Format.Snes3Bpp : Gfx.Format.Snes4Bpp)
        };
    }
}
