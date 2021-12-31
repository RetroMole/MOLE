namespace Mole.Shared.Graphics
{
    public abstract class FormatBase
    {
        public abstract int PalSize { get; }
        public abstract int CharSize { get; }
        public abstract int ExpectedFileSize { get; }
        public abstract byte Index { get; }
        public abstract byte[][,] FromRawToGfx(byte[] RawData);
        public abstract byte[] FromGfxToRaw(byte[][,] CharArray);
    }
}
