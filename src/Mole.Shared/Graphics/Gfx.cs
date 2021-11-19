using Mole.Shared.Util;
using System.Linq;

namespace Mole.Shared.Graphics
{
    public class Gfx : GfxBase
    {
        public Gfx(ref Progress progress, ref Rom rom, string ProjDir) : base(ref progress, ref rom, ProjDir) { }
        // Override LoadFromRom method
        public override void LoadFromRom(ref Rom rom, ref Progress progress)
        {
            // If pointers were loaded from cache skip this step
            if (Pointers is null)
            {
                Pointers = new uint[0x34];
                // Load pointers from ROM
                var low = rom.Skip(rom.SnesToPc(0x00B992)).Take(0x32).ToArray();
                var high = rom.Skip(rom.SnesToPc(0x00B9C4)).Take(0x32).ToArray();
                var bank = rom.Skip(rom.SnesToPc(0x00B9F6)).Take(0x32).ToArray();
                progress.MaxProgress = 0x32;
                for (var i = 0; i < 0x32; i++)
                {
                    progress.CurrentProgress = i;
                    Pointers[i] = (uint)((bank[i] << 16) | (high[i] << 8) | low[i]);
                }
                Pointers[0x32] = 0x088000;
                Pointers[0x33] = 0x08BFC0;
            }
            progress.State = Progress.StateEnum.DecompressingGfx;
            // Load and decompres data
            base.LoadFromRom(ref rom, ref progress);
        }
    }
}
