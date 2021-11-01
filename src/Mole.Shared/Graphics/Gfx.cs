using Mole.Shared.Util;
using System.Linq;

namespace Mole.Shared.Graphics
{
    public class Gfx : GfxBase
    {
        public Gfx(ref Rom rom, ref Project.UiData projData) : base(ref rom, ref projData) { }
        // Override LoadFromRom method
        public new void LoadFromRom(ref Rom rom, ref Project.UiData projData)
        {
            // If pointers were loaded from cache skip this step
            if (Pointers is null)
            {
                // Load pointers from ROM
                var low = rom.Skip(rom.SnesToPc(0x00B992)).Take(0x32).ToArray();
                var high = rom.Skip(rom.SnesToPc(0x00B9C4)).Take(0x32).ToArray();
                var bank = rom.Skip(rom.SnesToPc(0x00B9F6)).Take(0x32).ToArray();
                projData.Progress.MaxProgress = 0x32;
                for (var i = 0; i < 0x32; i++)
                {
                    projData.Progress.CurrentProgress = i;
                    Pointers[i] = (uint)((bank[i] << 16) | (high[i] << 8) | low[i]);
                }
                Pointers[0x32] = 0x088000;
                Pointers[0x33] = 0x08BFC0;
            }
            // Load and decompres data
            base.LoadFromRom(ref rom, ref projData);
        }
    }
}
