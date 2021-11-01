namespace Mole.Shared.Graphics
{
    public abstract class FormatBase
    {
        public int PalSize;
        public int CharSize;
        public int ExpectedFileSize;
        public abstract byte[][,] FromRawToGfx(byte[] RawData);
        public abstract byte[] FromGfxToRaw(byte[][,] CharArray);
    }
}
