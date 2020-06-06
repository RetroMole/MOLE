using System;
using AsarCLR;

namespace LA_Back
{
	/// <summary>
	/// LA Backend
	/// </summary>
	public class Lunar_Alchemy 
	{
		public static void Main(string[] args)
		{
			/*
			ROMHandler rh = new ROMHandler(String.Format("{0}",args[0]));

			Console.WriteLine("Testing HexWrite() and Undo-Redo system");
			Console.WriteLine("Initial Value of first 8 bytes: " + Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.HexWrite("0123456789ABCDEF", 0);
			Console.WriteLine("HexWrite() modified 8 bytes: " + Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();
			Console.WriteLine("Undo: " + Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Redo();
			Console.WriteLine("Redo: " + Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.HexWrite("89ABCDEF", 0);
			Console.WriteLine("HexWrite modified 4 bytes: " + Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();
			Console.WriteLine("Undo: " + Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();
			Console.WriteLine("Undo: " + Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.HexWrite("89ABCDEF", 0);
			Console.WriteLine("HexWrite() modified 4 bytes: " + Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			Console.WriteLine("Saving as {0}... ", rh.ROMName + "_TEST.smc");
			rh.Save();
			*/

			//GFXHandler gh = new GFXHandler(String.Format(@"C:\Users\Leuu\source\repos\Lunar Alchemy\TEST\ROMs\Clean\{0}", args[0]));

			//web.Downloader.GetAsarBuild(web.Downloader.PresetAppveyorRequest.LA_BacktestSuccess);
			//web.Downloader.GetAsarLatestRelease();



			/*Asar.Init();
			int[] input = new int[] 
			{ 
				0x088000,
			};
			foreach (int i in input)
			{
				var pc = Asar.SnesToPc(i);
				var snes = Asar.PcToSnes(pc);
				Console.WriteLine("0x{0:X}", i);
				Console.WriteLine("0x{0:X}", pc);
				Console.WriteLine("0x{0:X}", snes);
			}*/

			web.AsarUpdater.CompileAsarHook();

			
		}
	}
}