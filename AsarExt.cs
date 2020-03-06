using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsarCLR
{
	/// <summary>
	/// Copies of ASAR functions and variables that LA needs but are inaccessible trough the asar hook
	/// </summary>
	class AsarExt
	{
		public static int[] sa1banks = new int[8] { 0 << 20, 1 << 20, -1, -1, 2 << 20, 3 << 20, -1, -1 };
			
		public static int SnesToPc(int addr)
		{
			MapperType mapper = Asar.getmapper();
			if (addr < 0 || addr > 0xFFFFFF) return -1;//not 24bit
			switch (mapper)
			{
				case MapperType.LoRom:
					// randomdude999: The low pages ($0000-$7FFF) of banks 70-7D are used
					// for SRAM, the high pages are available for ROM data though
					if ((addr & 0xFE0000) == 0x7E0000 ||//wram
						(addr & 0x408000) == 0x000000 ||//hardware regs, ram mirrors, other strange junk
						(addr & 0x708000) == 0x700000)//sram (low parts of banks 70-7D)
						return -1;
					addr = ((addr & 0x7F0000) >> 1 | (addr & 0x7FFF));
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
					if ((addr & 0xC00000) != 0xC00000) return (addr & 0x3FFFFF) | 0x400000;
						return addr & 0x3FFFFF;
				case MapperType.SfxRom:
					// Asar emulates GSU1, because apparently emulators don't support the extra ROM data from GSU2
					if ((addr & 0x600000) == 0x600000 ||//wram, sram, open bus
					(addr & 0x408000) == 0x000000 ||//hardware regs, ram mirrors, rom mirrors, other strange junk
					(addr & 0x800000) == 0x800000)//fastrom isn't valid either in superfx
						return -1;
					if ((addr & 0x400000) != 0) 
						return addr & 0x3FFFFF;
					else 
						return (addr & 0x7F0000) >> 1 | (addr & 0x7FFF);
				case MapperType.Sa1Rom:
					if ((addr & 0x408000) == 0x008000)
					{
						return sa1banks[(addr & 0xE00000) >> 21] | ((addr & 0x1F0000) >> 1) | (addr & 0x007FFF);
					}
					if ((addr & 0xC00000) == 0xC00000)
					{
						return sa1banks[((addr & 0x100000) >> 20) | ((addr & 0x200000) >> 19)] | (addr & 0x0FFFFF);
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

		public static int PcToSnes(int addr)
		{
			MapperType mapper = Asar.getmapper();
			if (addr < 0) return -1;
			if (mapper == MapperType.LoRom)
			{
				if (addr >= 0x400000) return -1;
				addr = ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;
				return addr | 0x800000;
			}
			if (mapper == MapperType.HiRom)
			{
				if (addr >= 0x400000) return -1;
				return addr | 0xC00000;
			}
			if (mapper == MapperType.ExLoRom)
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
			if (mapper == MapperType.ExHiRom)
			{
				if (addr >= 0x800000) return -1;
				if ((addr & 0x400000) != 0) return addr;
				return addr | 0xC00000;
			}
			if (mapper == MapperType.Sa1Rom)
			{
				for (int i = 0; i < 8; i++)
				{
					if (sa1banks[i] == (addr & 0x700000)) { return 0x008000 | (i << 21) | ((addr & 0x0F8000) << 1) | (addr & 0x7FFF); }
				}
				return -1;
			}
			if (mapper == MapperType.BigSa1Rom)
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
			if (mapper == MapperType.SfxRom)
			{
				if (addr >= 0x200000) return -1;
				return ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;
			}
			if (mapper == MapperType.NoRom)
			{
				return addr;
			}
			return -1;
		}
	}
}
