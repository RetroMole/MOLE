using System;
using System.IO;
using System.Runtime.CompilerServices;
using AsarCLR;

namespace LA_Back
{ 
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

		public string ROMName;
		
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
                    Asar.Init();
                    Asar.Patch(patch, ref _ROM);
                    Asar.Close();
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
					// UNFORTUNATELY, SERIALIZING ANONYMOUS LA_BackMBDA EXPRESSIONS AIN'T OK
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