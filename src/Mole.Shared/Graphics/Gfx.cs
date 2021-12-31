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

        // Defaults
        public override FormatBase Format(int idx) => idx switch {
            int n when n <= 0x26 => Project.Formats[3],                // First 0x26 are 3bpp
            int n when n == 0x27 => Project.Formats[73],               // 0x27 is Mode7 3bpp
            int n when n >= 0x28 && n <= 0x2B => Project.Formats[2],   // 0x28-0x2B is 2bpp 
            int n when n >= 0x2C && n <=0x2E => Project.Formats[3],    // 0x2C-0x2E are 3bpp
            int n when n == 0x2F => Project.Formats[2],                // 0x2F is 2bpp
            int n when n >= 0x30 => Project.Formats[3],                // 0x30-0x33 are 3bpp
            _ => Project.Formats[3]
        };
    }
}
