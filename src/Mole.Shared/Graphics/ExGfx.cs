using Mole.Shared.Util;
using System.Linq;

namespace Mole.Shared.Graphics
{
    public class ExGfx : GfxBase
    {
        public ExGfx(ref Rom rom, ref Project.UiData projData) : base(ref rom, ref projData) { }
        // Override LoadFromRom method
        public new void LoadFromRom(ref Rom rom, ref Project.UiData projData)
        {
            // If pointers were loaded from cache skip this step
            if (Pointers is null)
            {
                // Load pointers from ROM
                var ptrBytes = rom.Skip(rom.SnesToPc(0x0FF600)).Take(0x180).ToArray();
                projData.Progress.MaxProgress = 0x80;
                for (var i = 0; i < 0x80; i++)
                {
                    projData.Progress.CurrentProgress = i;
                    Pointers[i] = (uint)((ptrBytes[(i * 3) + 2] << 16) | (ptrBytes[(i * 3) + 1] << 8) | ptrBytes[i * 3]);
                }
            }
            // Load and decompres data
            base.LoadFromRom(ref rom, ref projData);
        }
    }
}
