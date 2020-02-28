using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using AsarCLR;

namespace LA
{
	public enum RomMapping
	{
		LoRom,
		HiRom,
		Sa1Rom,
		LoRomFastRom,
		HiRomFastRom,
		ExLoRom,
		ExHiRom
	}

	public class ROMHandler
	{
		public UndoRedo UR {get;} = new UndoRedo();
		public byte[] ROM 
		{
			get 
			{
				return _ROM;
			}

		}
		protected byte[] _ROM;

		public RomMapping ROMMapping = RomMapping.LoRom;
		public string ROMName
		{
			get;
			protected set;
		}
		
		public ROMHandler(string Name)
		{
			ROMName = Name;
			FileStream fs = new FileStream(ROMName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			BinaryReader r = new BinaryReader(fs);
			
			FileInfo f = new FileInfo(ROMName);
			_ROM = r.ReadBytes((int)f.Length);
		}
		
		public void HexWrite(string HexStr, uint PcAddr, [CallerMemberName] string caller = "")
		{
			string UndoStr = "";
			UR.Do
			(
				() =>
				{
					byte[] HexArr = Utils.HexStrToByteArr(HexStr);
					
					UndoStr += Utils.ByteArrToHexStr(_ROM, HexStr.Length/2, (int)PcAddr);
					
					HexArr.CopyTo(_ROM, PcAddr);
				},
				() =>
				{
					HexWrite(UndoStr, PcAddr);
				},
				caller == "Main"
			);
		}
		
		public void Patch(string patch, [CallerMemberName] string caller = "")
		{
			UR.Do
			(
				() =>
				{
					Asar.init();
					Asar.patch(patch, ref _ROM);
					Asar.close();
				},
				() =>
				{
					
				},
				caller == "Main"
			);
		}
		
		public void Save()
		{
			UR.Do
			(
				() =>
				{
					/*
					 * UNFORTUNATELY, SERIALIZING ANONYMOUS LAMBDA EXPRESSIONS AIN'T OK
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