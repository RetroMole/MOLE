using Mole.Shared.Util;
using System.Linq;

namespace Mole.Shared.Graphics
{
    public class ExGfx : GfxBase
    {
        public ExGfx(ref Progress progress, ref Rom rom, string ProjDir) : base(ref progress, ref rom, ProjDir) { }
        // Override LoadFromRom method
        public override void LoadFromRom(ref Rom rom, ref Progress progress)
        {
            // If pointers were loaded from cache skip this step
            if (Pointers is null)
            {
                Pointers = new uint[0x180];
                // Load pointers from ROM
                var ptrBytes = rom.Skip(rom.SnesToPc(0x0FF600)).Take(0x180).ToArray();
                progress.MaxProgress = 0x80;
                for (var i = 0; i < 0x80; i++)
                {
                    progress.CurrentProgress = i;
                    Pointers[i] = (uint)((ptrBytes[(i * 3) + 2] << 16) | (ptrBytes[(i * 3) + 1] << 8) | ptrBytes[i * 3]);
                }
            }
            // Load and decompres data
            progress.State = Progress.StateEnum.DecompressingExGfx;
            base.LoadFromRom(ref rom, ref progress);
        }
    }
}
