using System;

namespace LA
{
	public static class Utils
	{
		public static byte[] HexStrToByteArr(string hex)
		{
			if (hex.Length % 2 == 1)
				throw new Exception("The hex string cannot have an odd number of digits");

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
			{
				arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
			}

			return arr;
		}

		private static int GetHexVal(char hex)
		{
			int val = (int)hex;
			//For uppercase A-F letters:
			return val - (val < 58 ? 48 : 55);
			//For lowercase a-f letters:
			//return val - (val < 58 ? 48 : 87);
		}

		public static string ByteArrToHexStr(byte[] bytes, int? count, int offset)
		{
			int b;
			char[] c;
			byte[] nb;

			if (count == null)
			{
				count = bytes.Length * 2;
			}
			nb = new byte[(int)count];
			c = new char[(int)count * 2];

			Array.Copy(bytes, offset, nb, 0, (long)count);

			for (int i = 0; i < count; i++)
			{
				b = nb[i] >> 4;
				c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
				b = nb[i] & 0xF;
				c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
			}
			return new string(c);

		}

		public static int SnesToPc(int addr, RomMapping rm, bool header)
		{
			switch (rm)
			{
				case RomMapping.LoRom:
					if (addr < 0 || addr > 0xFFFFFF ||//not 24bit
					(addr & 0xFE0000) == 0x7E0000 ||//wram
					(addr & 0x408000) == 0x000000)//hardware regs
						return -1;
					addr = ((addr & 0x7F0000) >> 1 | (addr & 0x7FFF));
					if (header) addr += 512;
					break;
				case RomMapping.HiRom:
					break;
				case RomMapping.Sa1Rom:
					break;
				case RomMapping.ExLoRom:
					break;
				case RomMapping.ExHiRom:
					break;
				case RomMapping.HiRomFastRom:
					break;
				case RomMapping.LoRomFastRom:
					break;
			}
			return addr;
		}

		public static int PcToSnes(int addr, RomMapping rm, bool header)
		{
			switch (rm)
			{
				case RomMapping.LoRom:
					if (header) addr -= 512;
					if (addr < 0 || addr >= 0x400000) return -1;
					addr = ((addr << 1) & 0x7F0000) | (addr & 0x7FFF) | 0x8000;
					if ((addr & 0xF00000) == 0x700000) addr |= 0x800000;
					break;
				case RomMapping.HiRom:
					break;
				case RomMapping.Sa1Rom:
					break;
				case RomMapping.ExLoRom:
					break;
				case RomMapping.ExHiRom:
					break;
				case RomMapping.HiRomFastRom:
					break;
				case RomMapping.LoRomFastRom:
					break;
			}
			return addr;

		}
}