namespace Smallhacker.TerraCompress
{
    public interface IDecompressor
    {
        byte[] Decompress(byte[] compressedData, uint start);
    }
}
