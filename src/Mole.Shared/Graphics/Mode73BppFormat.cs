using System;
using System.Collections.Generic;
using System.Linq;

namespace Mole.Shared.Graphics
{
    class Mode73BppFormat : FormatBase
    {
        public new int PalSize = 8;
        public new int CharSize = 24;
        public new int ExpectedFileSize = 3072;
        public override byte[] FromGfxToRaw(byte[][,] CharArray)
        {
            throw new NotImplementedException();
        }
        public override byte[][,] FromRawToGfx(byte[] RawData)
        {
            throw new NotImplementedException();
        }
    }
}
