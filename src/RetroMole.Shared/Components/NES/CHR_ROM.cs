using RetroMole.Core.Interfaces;
using System.Collections;

namespace RetroMole.Core.Components.NES
{
    public class CHR_ROM : IRom, IEnumerator<byte>, IEnumerable<byte>
    {
        public CHR_ROM(byte[] data)
        { _rom = data; }
        // Internal representation, indexer, and IEnum... implementation
        private byte[] _rom;
        public byte this[int index] => _rom[index];
        private int Index = -1;
        public byte Current => _rom[Index];
        object IEnumerator.Current => Current;
        public void Dispose() { GC.SuppressFinalize(this); }
        public IEnumerator<byte> GetEnumerator() => ((IEnumerable<byte>)_rom).GetEnumerator();
        public bool MoveNext() { Index++; return true; }
        public void Reset() { Index = 0; }
        IEnumerator IEnumerable.GetEnumerator() => _rom.GetEnumerator();
    }
}
