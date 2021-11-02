using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mole.Shared.Encodings;
using Mole.Shared.Util;

namespace Mole.Shared
{
    /// <summary>
    /// ROM info and direct operations
    /// </summary>
    public class Rom : IEnumerator<byte>, IEnumerable<byte>
    {
        public bool Loaded = false;

        /// <summary>
        /// Indexer, range indexer, and internal ROM representation
        /// </summary>
        public byte this[int index] => _rom[SnesToPc(index)];
        public byte[] Pc => _rom;
        private byte[] _rom;

        /// <summary>
        /// Full Path to currently loaded ROM
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            private init
            {
                _filePath = value;
                FileName = Path.GetFileName(_filePath);
            }
        }
        
        private readonly string _filePath;
        public string FileName;
        public byte[] Header;
        public byte[] InternalHeader;
        public string Title;
        public bool FastRom; 
        public MapperType Mapping = MapperType.NoRom;
        public double RomSize;
        public double SramSize;
        public string Region;
        public byte DevId;
        public byte Version;
        public ushort Checksum;
        public ushort ChecksumComplement;


        /// <summary>
        /// Constructs a ROM from file at path
        /// </summary>
        /// <param name="path">ROM Path</param>
        public Rom(string path)
        {
            LoggerEntry.Logger.Information("Loading ROM from {0}", path);
            // Load ROM into internal _ROM byte array
            FilePath = path;

            FileStream fs = new(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader r = new(fs);
            FileInfo f = new(FilePath);

            _rom = r.ReadBytes((int)f.Length);
            int h = _rom.Length % 0x8000;
            if (h != 0)
            {
                Header = _rom.Take(h).ToArray();
                _rom = _rom.Skip(h).ToArray();
            }

            // Patch the ROM with an empty patch so it is opened in asar
            // (workaround for not exposing OpenROM and CloseROM in lib asar)
            // This also fixes broken checksums
            Asar.Patch(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Patches/empty.asm"), ref _rom);

            // Rely on asar mapping mode guess until we can access the internal header
            Mapping = Asar.GetMapper();
            InternalHeader = this.Skip(0x00FFC0).Take(32).ToArray();
            FastRom = (InternalHeader[0x15] & 0b00010000) != 0;
            Mapping = (InternalHeader[0x15] & 0b00000111) switch
            {
                0 => MapperType.LoRom,
                1 => MapperType.HiRom,
                2 => MapperType.ExLoRom,
                3 => MapperType.Sa1Rom,
                5 => MapperType.ExHiRom,
                _ => MapperType.InvalidMapper
            };
            if (Mapping != Asar.GetMapper())
                LoggerEntry.Logger.Warning("Mapping mode mismatch: Asar: {0}, Header: {1}", Asar.GetMapper(), Mapping);

            // Load ROM info from internal header
            Title = new string(new Jisx0201().GetChars(InternalHeader.Take(21).ToArray()));
            RomSize = Math.Pow(2,InternalHeader[0x17]);
            SramSize = Math.Pow(2,InternalHeader[0x18]);
            Region = (InternalHeader[0x19] & 0b00000111) switch
            {
                0x00 => "Japan",
                0x01 => "North America",
                0x02 => "Europe",
                0x03 => "Sweden/Scandinavia",
                0x04 => "Finland",
                0x05 => "Denmark",
                0x06 => "France",
                0x07 => "Netherlands",
                0x08 => "Spain",
                0x09 => "Germany",
                0x0A => "Italy",
                0x0B => "China",
                0x0C => "Indonesia",
                0x0D => "Korea",
                0x0F => "Canada",
                0x10 => "Brazil",
                0x11 => "Australia",
                _ => "Unknown"
            };
            DevId = InternalHeader[0x1A];
            Version = InternalHeader[0x1B];
            Checksum = (ushort)((InternalHeader[0x1C] << 8) | InternalHeader[0x1D]);
            ChecksumComplement = (ushort)((InternalHeader[0x1E] << 8) | InternalHeader[0x1F]);

            Loaded = true;
        }

        private static readonly int[] Sa1Banks = new int[8] { 0, 1 << 20, -1, -1, 2 << 20, 3 << 20, -1, -1 };
        /// <summary>
        /// Convert SNES address to PC address using this ROM's MapperType
        /// </summary>
        /// <param name="addr">SNES address to convert</param>
        /// <returns>PC address or -1</returns>
        public int SnesToPc(int addr)
        {
            if (addr < 0 || addr > 0xFFFFFF) return -1;//not 24bit
            switch (Mapping)
            {
                case MapperType.LoRom:
                    // The low pages ($0000-$7FFF) of banks 70-7D are used
                    // for SRAM, the high pages are available for ROM data though
                    if ((addr & 0xFE0000) == 0x7E0000 || // WRAM
                        (addr & 0x408000) == 0x000000 || // hardware regs, ram mirrors, other strange junk
                        (addr & 0x708000) == 0x700000) // sram (low parts of banks 70-7D)
                        return -1;
                    addr = (((addr & 0x7F0000) >> 1) | (addr & 0x7FFF));
                    return addr;
                case MapperType.HiRom:
                    if ((addr & 0xFE0000) == 0x7E0000 || // WRAM
                    (addr & 0x408000) == 0x000000) //hardware regs, ram mirrors, other strange junk
                        return -1;
                    return addr & 0x3FFFFF;
                case MapperType.ExLoRom:
                    if ((addr & 0xF00000) == 0x700000 || // WRAM, SRAM
                    (addr & 0x408000) == 0x000000) //area that shouldn't be used in lorom
                        return -1;
                    if ((addr & 0x800000) != 0)
                    {
                        addr = ((addr & 0x7F0000) >> 1 | (addr & 0x7FFF));
                    }
                    else
                    {
                        addr = ((addr & 0x7F0000) >> 1 | (addr & 0x7FFF)) + 0x400000;
                    }
                    return addr;
                case MapperType.ExHiRom:
                    if ((addr & 0xFE0000) == 0x7E0000 || // VRAM
                    (addr & 0x408000) == 0x000000) // hardware regs, ram mirrors, other strange junk
                        return -1;
                    if ((addr & 0xC00000) != 0xC00000)
                        return (addr & 0x3FFFFF) | 0x400000;
                    return addr & 0x3FFFFF;
                case MapperType.SfxRom:
                    // Asar emulates GSU1, because apparently emulators don't support the extra ROM data from GSU2
                    if ((addr & 0x600000) == 0x600000 || //Wram, sram, open bus
                    (addr & 0x408000) == 0x000000 || // hardware regs, ram mirrors, rom mirrors, other strange junk
                    (addr & 0x800000) == 0x800000) // FastROM isn't valid either in superfx
                        return -1;
                    if ((addr & 0x400000) != 0)
                        return addr & 0x3FFFFF;
                    return (addr & 0x7F0000) >> 1 | (addr & 0x7FFF);
                case MapperType.Sa1Rom:
                    if ((addr & 0x408000) == 0x008000)
                    {
                        return Sa1Banks[(addr & 0xE00000) >> 21] | ((addr & 0x1F0000) >> 1) | (addr & 0x007FFF);
                    }
                    if ((addr & 0xC00000) == 0xC00000)
                    {
                        return Sa1Banks[((addr & 0x100000) >> 20) | ((addr & 0x200000) >> 19)] | (addr & 0x0FFFFF);
                    }
                    return -1;
                case MapperType.BigSa1Rom:
                    if ((addr & 0xC00000) == 0xC00000) // hirom
                    {
                        return (addr & 0x3FFFFF) | 0x400000;
                    }
                    if ((addr & 0xC00000) == 0x000000 || (addr & 0xC00000) == 0x800000) // lorom
                    {
                        if ((addr & 0x008000) == 0x000000) return -1;
                        return (addr & 0x800000) >> 2 | (addr & 0x3F0000) >> 1 | (addr & 0x7FFF);
                    }
                    return -1;
                default:
                    return addr;
            }
        }

        /// <summary>
        /// Convert PC address to SNES address using this ROM's MapperType
        /// </summary>
        /// <param name="addr">PC address to convert</param>
        /// <returns>SNES address or -1</returns>
        public int PcToSnes(int addr)
        {
            if (addr < 0) return -1;
            switch (Mapping)
            {
                case MapperType.InvalidMapper:
                    return -1;

                case MapperType.LoRom:
                    if (addr >= 0x400000) return -1;
                    addr = ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;
                    return addr | 0x800000;

                case MapperType.HiRom:
                    if (addr >= 0x400000) return -1;
                    return addr | 0xC00000;

                case MapperType.Sa1Rom:
                    for (int i = 0; i < 8; i++)
                    {
                        if (Sa1Banks[i] == (addr & 0x700000)) { return 0x008000 | (i << 21) | ((addr & 0x0F8000) << 1) | (addr & 0x7FFF); }
                    }
                    return -1;

                case MapperType.BigSa1Rom:
                    if (addr >= 0x800000) return -1;
                    if ((addr & 0x400000) == 0x400000)
                    {
                        return addr | 0xC00000;
                    }
                    if ((addr & 0x600000) == 0x000000)
                    {
                        return ((addr << 1) & 0x3F0000) | 0x8000 | (addr & 0x7FFF);
                    }
                    if ((addr & 0x600000) == 0x200000)
                    {
                        return 0x800000 | ((addr << 1) & 0x3F0000) | 0x8000 | (addr & 0x7FFF);
                    }
                    return -1;

                case MapperType.SfxRom:
                    if (addr >= 0x200000) return -1;
                    return ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;

                case MapperType.ExLoRom:
                    if (addr >= 0x800000) return -1;
                    if ((addr & 0x400000) != 0)
                    {
                        addr -= 0x400000;
                        addr = ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;
                        return addr;
                    }
                    else
                    {
                        addr = ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;
                        return addr | 0x800000;
                    }

                case MapperType.ExHiRom:
                    if (addr >= 0x800000) return -1;
                    if ((addr & 0x400000) != 0) return addr;
                    return addr | 0xC00000;
                default:
                    return addr;
            }
        }
        
        //Enumerable/Enumerator implementation
        int _position = -1;
        public bool MoveNext()
        {
            _position++;
            return SnesToPc(_position) < _rom.Length;
        }
        public void Reset() => _position = 0;
        public object Current { get => this[_position]; }
        byte IEnumerator<byte>.Current { get => this[_position]; }
        public IEnumerator<byte> GetEnumerator() => this.OfType<byte>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Dispose() {
            GC.SuppressFinalize(this);
        } // TODO: Possible Memory leak, does the enumerator/enumerable stuff need manual disposal?
    }
}
