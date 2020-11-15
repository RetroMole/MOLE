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
		/// ROMType
		/// </summary>
		public LC.AddressFlags ROMType = LC.AddressFlags.LC_NOBANK;

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
			if (!Utils.Hex.CompareBytesAndString(_ROM, "789C0042"))
			{
				_ROM = _ROM.Skip(0x200).ToArray();
				hasHeader = true;
			}
			ushort chksum = BitConverter.ToUInt16(new byte[2] { (byte)ROM[0x07FDF], (byte)ROM[0x07FDE] }, 0);
			ushort invchksum = BitConverter.ToUInt16(new byte[2] { (byte)ROM[0x07FDD], (byte)ROM[0x07FDC] }, 0);
			if ( (chksum | invchksum) == 0xFFFF) 
			{
				ROMType = LC.AddressFlags.LC_LOROM;
            }
            else
            {
				chksum = BitConverter.ToUInt16(new byte[2] { (byte)ROM[0x0FFDF], (byte)ROM[0x0FFDE] }, 0);
				invchksum = BitConverter.ToUInt16(new byte[2] { (byte)ROM[0x0FFDD], (byte)ROM[0x0FFDC] }, 0);
				if ((chksum | invchksum) == 0xFFFF)
                {
					ROMType = LC.AddressFlags.LC_HIROM;
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
	}
}