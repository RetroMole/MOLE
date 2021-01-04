using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MOLE_Back.Libs;

namespace MOLE_Back
{ 
	/// <summary>
	/// Deals with ""direct"" ROM access
	/// </summary>
	public class ROMHandler
	{
		/// <summary>
		/// UndoRedo system required for ROM operations
		/// </summary>
		public UndoRedo UR {get;} = new UndoRedo();
		/// <summary>
		/// The currently loaded ROM
		/// </summary>
		public byte[] ROM => _ROM;
		protected byte[] _ROM;

		/// <summary>
		/// Full Path to currently loaded ROM
		/// </summary>
		public string ROMPath
		{
			get
			{
				return _ROMPath;
			}
			set
			{
				_ROMPath = value;
				ROMName = Path.GetFileName(_ROMPath);
			}
			
		}
		protected string _ROMPath;

		/// <summary>
		///	Name of currently loaded ROM
		/// </summary>
		public string ROMName;

		/// <summary>
		/// Boolean Used to detrmine wether or not the ROM has a 0x200 byte Header
		/// </summary>
		public bool hasHeader = false;

		/// <summary>
		/// Mapper
		/// </summary>
		public MapperType Mapper = MapperType.NoRom;

		public static int[] Sa1Banks = new int[8] { 0 << 20, 1 << 20, -1, -1, 2 << 20, 3 << 20, -1, -1 };


		/// <summary>
		/// Creates a ROMHandler from a ROM Path
		/// </summary>
		/// <param name="path">ROM Path</param>
		public ROMHandler(string path)
		{
			ROMPath = path;
			ROMName = Path.GetFileName(path);
			FileStream fs = new FileStream(ROMPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			BinaryReader r = new BinaryReader(fs);
			
			FileInfo f = new FileInfo(ROMPath);
			_ROM = r.ReadBytes((int)f.Length);
			if (!Utils.Hex.CompareBytesAndString(_ROM, "789C0042")) // dont do this, ill implement checksums later
			{
				_ROM = _ROM.Skip(0x200).ToArray();
				hasHeader = true;
			}
			ushort chksum = BitConverter.ToUInt16(new byte[2] { (byte)ROM[0x07FDF], (byte)ROM[0x07FDE] }, 0);
			ushort invchksum = BitConverter.ToUInt16(new byte[2] { (byte)ROM[0x07FDD], (byte)ROM[0x07FDC] }, 0);
			if ( (chksum | invchksum) == 0xFFFF) 
			{
				Mapper = MapperType.LoRom;
            }
            else
            {
				chksum = BitConverter.ToUInt16(new byte[2] { (byte)ROM[0x0FFDF], (byte)ROM[0x0FFDE] }, 0);
				invchksum = BitConverter.ToUInt16(new byte[2] { (byte)ROM[0x0FFDD], (byte)ROM[0x0FFDC] }, 0);
				if ((chksum | invchksum) == 0xFFFF)
                {
					Mapper = MapperType.HiRom;
                }
			}
		}
		
		/// <summary>
		/// Writes HEX data to ROM
		/// </summary>
		/// <param name="HexStr">String of HEX characters, make sure its an even length</param>
		/// <param name="PcAddr">PC Address to start writing at</param>
		/// <param name="caller"></param>
		public void HexWrite(string HexStr, uint PcAddr, [CallerMemberName] string caller = "")
		{
			string UndoStr = "";
			UR.Do
			(
				() =>
				{
					byte[] HexArr = Utils.Hex.HexStrToByteArr(HexStr);
					UndoStr += Utils.Hex.ByteArrToHexStr(ROM, HexStr.Length/2, (int)PcAddr);
					HexArr.CopyTo(ROM, PcAddr);
				},
				() =>
				{
					HexWrite(UndoStr, PcAddr);
				},
				caller == "Main"
			);
		}
		
		/// <summary>
		/// Applies a Patch to the ROM using Asar
		/// </summary>
		/// <param name="patch">Path to Patch</param>
		/// <param name="caller"></param>
		public void Patch(string patch, [CallerMemberName] string caller = "")
		{
            UR.Do
			(
				() =>
				{
                    Asar.Init();
                    Asar.Patch(patch, ref _ROM);
                    Asar.Close();
				},
				() =>
				{
					// oh god what do i even do here...
				},
				caller == "Main"
			);
		}
		
		/// <summary>
		/// Save ROM Changes
		/// </summary>
		public void Save()
		{
			UR.Do
			(
				() =>
				{
					/*
					// UNFORTUNATELY, SERIALIZING ANONYMOUS LAMBDA EXPRESSIONS AIN'T OK
					// I might have another idea though, TODO i guess

					FileStream s = File.Open("UR.bin", FileMode.OpenOrCreate);
					BinaryFormatter b = new BinaryFormatter();
					b.Serialize(s, UR);
					s.Close();
					*/

					UR.RedoStack.Clear();
 					UR.BackupStack.Clear();

					File.WriteAllBytes(ROMName+"_TEST.smc", ROM);
				},
				() =>
				{
					
				},
				false
			);
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
	}
}