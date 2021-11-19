using Mole.Shared.Util;
using System.Linq;

namespace Mole.Shared.Graphics
{
    public class SuperExGfx : GfxBase
    {
        public SuperExGfx(ref Progress progress, ref Rom rom, string ProjDir) : base(ref progress, ref rom, ProjDir) { }
        // Override LoadFromRom method
        public override void LoadFromRom(ref Rom rom, ref Progress progress)
        {
            // If pointers were loaded from cache skip this step
            if (Pointers is null)
            {
                Pointers = new uint[0x2D00];
                // Load pointers from ROM
                var startAddr = rom.SnesToPc(0x0FF937);
                var ptrBytes = rom.Skip((rom[startAddr+2] << 16) | (rom[startAddr + 1] << 8) | rom[startAddr]).Take(0x2D00).ToArray();
                progress.MaxProgress = 0xF00;
                for (var i = 0; i < 0xF00; i++)
                {
                    progress.CurrentProgress = i;
                    Pointers[i] = (uint)((ptrBytes[(i * 3) + 2] << 16) | (ptrBytes[(i * 3) + 1] << 8) | ptrBytes[(i * 3)]);
                }
            }
            // Load and decompres data
            progress.State = Progress.StateEnum.DecompressingExGfx;
            base.LoadFromRom(ref rom, ref progress);
        }
    }
}
