using System;
using System.IO;
using System.Linq;

namespace MOLE
{
    /// <summary>
    /// Deals with "direct" ROM access
    /// </summary>
    public class ROM
    {
        /// <summary>
        /// UndoRedo system required for ROM operations
        /// </summary>
        public Utils.UndoRedo UR { get; } = new();

        /// <summary>
        /// The currently loaded ROM
        /// </summary>
        public byte this[int index]
        {
            get => _ROM[index];
            set => _ROM[index] = value;
        }
        private byte[] _ROM;

        /// <summary>
        /// Full Path to currently loaded ROM
        /// </summary>
        public string ROMPath
        {
            get => _ROMPath;
            set
            {
                _ROMPath = value;
                ROMName = Path.GetFileName(_ROMPath).Split('.')[0];
            }

        }
        private string _ROMPath;

        /// <summary>
        ///	Name of currently loaded ROM
        /// </summary>
        public string ROMName;

        public byte[] header;

        /// <summary>
        /// Mapper
        /// </summary>
        public MapperType Mapper = MapperType.NoRom;

        public static readonly int[] Sa1Banks = new int[8] { 0 << 20, 1 << 20, -1, -1, 2 << 20, 3 << 20, -1, -1 };


        /// <summary>
        /// Creates a ROMHandler from a ROM Path
        /// </summary>
        /// <param name="path">ROM Path</param>
        public ROM(string path)
        {
            ROMPath = path;

            FileStream fs = new(ROMPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader r = new(fs);
            FileInfo f = new(ROMPath);

            _ROM = r.ReadBytes((int)f.Length);
            int h = _ROM.Length % 0x8000;
            if (h != 0)
            {
                header = _ROM.Take(h).ToArray();
                _ROM = _ROM.Skip(h).ToArray();
            }


            ushort chksum = (ushort)(this[0x07FDF] << 8 | this[0x07FDE]);
            ushort invchksum = (ushort)(this[0x07FDD] << 8 | this[0x07FDC]);
            if ((chksum | invchksum) == 0xFFFF)
            {
                Mapper = MapperType.LoRom;
            }
            else
            {
                chksum = (ushort)(this[0x0FFDF] << 8 | this[0x0FFDE]);
                invchksum = (ushort)(this[0x0FFDD] << 8 | this[0x0FFDC]);
                if ((chksum | invchksum) == 0xFFFF)
                {
                    Mapper = MapperType.HiRom;
                }
            }
        }

        public void HexWrite(byte[] bytes, uint PcAddr, bool UndoEntry)
        {
            byte[] undo = Array.Empty<byte>();
            UR.Do
            (
                () =>
                {
                    undo = _ROM.Skip((int)PcAddr).Take(bytes.Length).ToArray();
                    bytes.CopyTo(_ROM, PcAddr);
                },
                () =>
                {
                    HexWrite(undo, PcAddr, false);
                },
                UndoEntry
            );
        }

        public void Patch(string patch, bool UndoEntry)
        {
            Asarwrittenblock[] diff;
            UR.Do
            (
                () =>
                {
                    Asar.Init();
                    Asar.Patch(patch, ref _ROM);
                    diff = Asar.GetWrittenBlocks();
                    Asar.Close();
                },
                () =>
                {

                },
                UndoEntry
            );
        }

        /// <summary>
        /// Save ROM Changes
        /// </summary>
        public void Save(bool keepHeader = false)
        {
            UR.RedoStack.Clear();
            UR.BackupStack.Clear();
            File.WriteAllBytes(String.Format("TEST_{0}-{1:yyyy-MM-dd-HH-mm}.sfc", ROMName, DateTime.UtcNow), keepHeader ? header.Concat(_ROM).ToArray() : _ROM);

        }

        /// <summary>
        /// Convert SNES address to PC address using this ROMHandler's MapperType
        /// </summary>
        /// <param name="addr">SNES address to convert</param>
        /// <returns>PC address or -1</returns>
        public int SnesToPc(int addr)
        {
            if (addr < 0 || addr > 0xFFFFFF) return -1;//not 24bit
            switch (Mapper)
            {
                case MapperType.LoRom:
                    // randomdude999: The low pages ($0000-$7FFF) of banks 70-7D are used
                    // for SRAM, the high pages are available for ROM data though
                    if ((addr & 0xFE0000) == 0x7E0000 ||//wram
                        (addr & 0x408000) == 0x000000 ||//hardware regs, ram mirrors, other strange junk
                        (addr & 0x708000) == 0x700000)//sram (low parts of banks 70-7D)
                        return -1;
                    addr = (((addr & 0x7F0000) >> 1) | (addr & 0x7FFF));
                    return addr;
                case MapperType.HiRom:
                    if ((addr & 0xFE0000) == 0x7E0000 ||//wram
                    (addr & 0x408000) == 0x000000)//hardware regs, ram mirrors, other strange junk
                        return -1;
                    return addr & 0x3FFFFF;
                case MapperType.ExLoRom:
                    if ((addr & 0xF00000) == 0x700000 ||//wram, sram
                    (addr & 0x408000) == 0x000000)//area that shouldn't be used in lorom
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
                    if ((addr & 0xFE0000) == 0x7E0000 ||//wram
                    (addr & 0x408000) == 0x000000)//hardware regs, ram mirrors, other strange junk
                        return -1;
                    if ((addr & 0xC00000) != 0xC00000)
                        return (addr & 0x3FFFFF) | 0x400000;
                    return addr & 0x3FFFFF;
                case MapperType.SfxRom:
                    // Asar emulates GSU1, because apparently emulators don't support the extra ROM data from GSU2
                    if ((addr & 0x600000) == 0x600000 ||//wram, sram, open bus
                    (addr & 0x408000) == 0x000000 ||//hardware regs, ram mirrors, rom mirrors, other strange junk
                    (addr & 0x800000) == 0x800000)//fastrom isn't valid either in superfx
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
                    if ((addr & 0xC00000) == 0xC00000)//hirom
                    {
                        return (addr & 0x3FFFFF) | 0x400000;
                    }
                    if ((addr & 0xC00000) == 0x000000 || (addr & 0xC00000) == 0x800000)//lorom
                    {
                        if ((addr & 0x008000) == 0x000000) return -1;
                        return (addr & 0x800000) >> 2 | (addr & 0x3F0000) >> 1 | (addr & 0x7FFF);
                    }
                    return -1;
                case MapperType.NoRom:
                    return addr;
            }
            return -1;
        }

        /// <summary>
        /// Convert PC address to SNES address using this ROMHandler's MapperType
        /// </summary>
        /// <param name="addr">PC address to convert</param>
        /// <returns>SNES address or -1</returns>
        public int PcToSnes(int addr)
        {
            if (addr < 0) return -1;
            if (Mapper == MapperType.LoRom)
            {
                if (addr >= 0x400000) return -1;
                addr = ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;
                return addr | 0x800000;
            }
            if (Mapper == MapperType.HiRom)
            {
                if (addr >= 0x400000) return -1;
                return addr | 0xC00000;
            }
            if (Mapper == MapperType.ExLoRom)
            {
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
            }
            if (Mapper == MapperType.ExHiRom)
            {
                if (addr >= 0x800000) return -1;
                if ((addr & 0x400000) != 0) return addr;
                return addr | 0xC00000;
            }
            if (Mapper == MapperType.Sa1Rom)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (Sa1Banks[i] == (addr & 0x700000)) { return 0x008000 | (i << 21) | ((addr & 0x0F8000) << 1) | (addr & 0x7FFF); }
                }
                return -1;
            }
            if (Mapper == MapperType.BigSa1Rom)
            {
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
            }
            if (Mapper == MapperType.SfxRom)
            {
                if (addr >= 0x200000) return -1;
                return ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;
            }
            if (Mapper == MapperType.NoRom)
            {
                return addr;
            }
            return -1;
        }

        public static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        public static uint RotateRight(uint value, int count)
        {
            return (value >> count) | (value << (32 - count));
        }
    }
}
