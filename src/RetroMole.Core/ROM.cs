using System.Collections;
using System.Numerics;

namespace RetroMole.Core;

public abstract class ROM : IEnumerable<byte>, IEnumerator<byte>
{
    protected byte[] data;
    public List<Chunk> Chunks;

    public byte this[int i] => data[i];
    private int _index = -1;
    public byte Current => data[_index];
    object IEnumerator.Current => Current;
    public void Dispose() { GC.SuppressFinalize(this); }
    public IEnumerator<byte> GetEnumerator() => (data.AsEnumerable()).GetEnumerator();
    public bool MoveNext() { _index++; return true; }
    public void Reset() { _index = 0; }
    IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();
    

    protected ROM(string path) : this(File.ReadAllBytes(Path.GetFullPath(path))) { }
    protected ROM(byte[] data)
    {
        this.data = data;
    }

    public enum ChunkType
    {
        UNKNOWN,
        PRG,
        CHR,
        PAL,
        MAP,
        TXT
    }
    
    public class Chunk : IEnumerable<byte>, IEnumerator<byte>
    {
        public Chunk(byte[] data)
        {
            this.data = data;
        }
        protected byte[] data;
        public ChunkType Type;
        public Dictionary<Vector2, SubChunkFormat> ChunkInfo;
        
        public byte this[int i] => data[i];
        private int _index = -1;
        public byte Current => data[_index];
        object IEnumerator.Current => Current;
        public void Dispose() { GC.SuppressFinalize(this); }
        public IEnumerator<byte> GetEnumerator() => (data.AsEnumerable()).GetEnumerator();
        public bool MoveNext() { _index++; return true; }
        public void Reset() { _index = 0; }
        IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();
    }
    public abstract class SubChunkFormat
    {
        public abstract dynamic Read(byte[] d, Vector2 r);
        public abstract byte[] Write(dynamic o, Vector2 r);
    }
}
