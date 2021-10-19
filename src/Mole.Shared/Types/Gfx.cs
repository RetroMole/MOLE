using System;
using System.Linq;
using Mole.Shared.Util;
using Mole.Util;
using NLog;

namespace Mole.Shared.Types
{
    public class Gfx
    {
        private readonly uint[] _exGfxPointers = new uint[0x80];
        private readonly byte[] _gfx0;
        private readonly uint[] _gfxPointers = new uint[0x34];
        private readonly uint[] _superExGfxPointers = new uint[0xF00];

        public Gfx(ROM rom, Logger logger)
        {
            var low = rom.Skip(rom.SnesToPc(0x00B992)).Take(0x32).ToArray();
            var high = rom.Skip(rom.SnesToPc(0x00B9C4)).Take(0x32).ToArray();
            var bank = rom.Skip(rom.SnesToPc(0x00B9F6)).Take(0x32).ToArray();
            for (var i = 0; i < 0x32; i++)
                _gfxPointers[i] = BitConverter.ToUInt32(new byte[] {low[i], high[i], bank[i], 0});
            _gfxPointers[0x32] = 0x088000;
            _gfxPointers[0x33] = 0x08BFC0;

            var ex = rom.Skip(rom.SnesToPc(0x0FF600)).Take(0x180).ToArray();
            for (var i = 0; i < 0x80; i++) _exGfxPointers[i] = Bytes.B2Ul(ex.Skip(i * 3).Take(3).ToArray());

            if (rom.ROMSize <= 512) {
                logger.Warn("Unexpanded ROM, SuperExGFX can't be used");
                Array.Fill<uint>(_superExGfxPointers, 0xFFFFFF);
            } else {
                var supex = rom.Skip(rom.SnesToPc((int) Bytes.B2Ul(rom.Skip(rom.SnesToPc(0x0FF937)).Take(3).ToArray())))
                    .Take(0x2D00).ToArray();
                for (var i = 0; i < 0xF00; i++) _superExGfxPointers[i] = Bytes.B2Ul(supex.Skip(i * 3).Take(3).ToArray());
            }
            
            _gfx0 = rom.Skip(rom.SnesToPc((int) _gfxPointers[0])).Take(2104).ToArray().LzwDecompress();
        }
    }
}