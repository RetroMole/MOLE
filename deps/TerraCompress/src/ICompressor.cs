namespace Smallhacker.TerraCompress
{
    public interface ICompressor
    {
        byte[] Compress(byte[] data);
    }
}
