using System;
using System.IO;
using System.Collections.Generic;

namespace LA
{
	public class Lunar_Alchemy 
	{
		public static void Main(string[] args)
		{

			/*MHandler rh = new ROMHandler(String.Format("../../ROMs/Clean/{0}",args[0]));

			Console.WriteLine("Testing HexWrite() and Undo-Redo system");
			Console.WriteLine("Initial Value of first 8 bytes: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.HexWrite("0123456789ABCDEF", 0);

			Console.WriteLine("HexWrite() modified 8 bytes: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();

			Console.WriteLine("Undo: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Redo();

			Console.WriteLine("Redo: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.HexWrite("89ABCDEF", 0);

			Console.WriteLine("HexWrite modified 4 bytes: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();

			Console.WriteLine("Undo: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();

			Console.WriteLine("Undo: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			Console.WriteLine("HexWrite() modified 4 bytes: ");
			rh.HexWrite("89ABCDEF", 0);

			Console.WriteLine("Saving as {0} ... ", rh.ROMName + "_TEST.smc");
			rh.Save();

			Console.Write("Downloading Asar Build From Build History: \n\t recordsNumber: 1, Branch: master, Status: success\n");
			web.Downloader.getAsarBuild("recordsNumber=1&branch=master&Status=success");
			*/

			GFXHandler gh = new GFXHandler(String.Format("../../ROMs/Clean/{0}", args[0]));
			Console.ReadKey();
		}
	}
}