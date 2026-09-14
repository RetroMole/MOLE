namespace RetroMole.Core;

using System.Collections;
using System.Numerics;

public abstract class ROM : IEnumerable<byte>, IEnumerator<byte>
{
    protected byte[] data;
    public List<Chunk> Chunks = new();

    public byte this[int i] => data[i];

    public byte Current => GetEnumerator().Current;

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        GetEnumerator().Dispose();
        GC.SuppressFinalize(this);
    }

    public IEnumerator<byte> GetEnumerator() => data.AsEnumerable().GetEnumerator();

    public bool MoveNext() => GetEnumerator().MoveNext();

    public void Reset() => GetEnumerator().Reset();

    IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();

    protected ROM(string path)
        : this(File.ReadAllBytes(Path.GetFullPath(path)))
    {
    }

    protected ROM(byte[] data)
    {
        this.data = data;
    }

    public enum ChunkType
    {
        UNKNOWN,
        Program,
        Graphics,
        Palettes,
        Maps,
        Text,
        Tables,
        Objects,
        Tilesets,
        SpriteObjects,
        AudioSamples,
        AudioTracks,
    }

    public ReadOnlyMemory<byte> Slice(int offset, int length)
        => data.AsMemory(offset, length);

    public class Chunk
    {
        public Chunk(ReadOnlyMemory<byte> data)
        {
            Data = data;
        }

        public ReadOnlyMemory<byte> Data;
        public ChunkType Type;
        public Dictionary<Vector2, SubChunkFormat> ChunkInfo;

        public byte this[int i] => Data.Span[i];
    }

    public abstract class SubChunkFormat
    {
        public abstract dynamic Read(byte[] d, Vector2 r);

        public abstract byte[] Write(dynamic o, Vector2 r);
    }
}
