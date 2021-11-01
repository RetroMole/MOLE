using Mole.Shared.Util;
using System.Linq;

namespace Mole.Shared.Graphics
{
    public class SuperExGfx : GfxBase
    {
        public SuperExGfx(ref Rom rom, ref Project.UiData projData) : base(ref rom, ref projData) { }
        // Override LoadFromRom method
        public new void LoadFromRom(ref Rom rom, ref Project.UiData projData)
        {
            // If pointers were loaded from cache skip this step
            if (Pointers is null)
            {
                // Load pointers from ROM
                var startAddr = rom.SnesToPc(0x0FF937);
                var ptrBytes = rom.Skip((rom.SnesToPc(rom[startAddr+2]) << 16) | (rom.SnesToPc(rom[startAddr + 1]) << 8) | rom.SnesToPc(rom[startAddr])).Take(0x2D00).ToArray();
                projData.Progress.MaxProgress = 0xF00;
                for (var i = 0; i < 0xF00; i++)
                {
                    projData.Progress.CurrentProgress = i;
                    Pointers[i] = (uint)((ptrBytes[(i * 3) + 2] << 16) | (ptrBytes[(i * 3) + 1] << 8) | ptrBytes[(i * 3)]);
                }
            }
            // Load and decompres data
            base.LoadFromRom(ref rom, ref projData);
        }
    }
}
