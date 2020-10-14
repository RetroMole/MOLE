using System;
using MOLE_Back.Libs;
using MOLE_Back.Properties;

namespace MOLE_Back
{
	/// <summary>
	/// LA Backend
	/// </summary>
	public class MOLE 
	{
		public static void Main(string[] args)
		{
			//Asar.Init();
			//Console.WriteLine(Asar.Version());
			//Asar.Close();
			//Console.WriteLine(LC.Version());

			Console.WriteLine("Settings:\n" +
				"	Asar autoupdate = {0}\n" +
				"	Asar autoupdate mode = {1}\n", Settings.Default.UPDATE_asar, Settings.Default.UPDATE_asar_mode);
			Update.VerifyLibs();

			Settings.Default.UPDATE_asar_mode = "build";
			Console.WriteLine("\n\nSettings:\n" +
				"	Asar autoupdate = {0}\n" +
				"	Asar autoupdate mode = {1}\n", Settings.Default.UPDATE_asar, Settings.Default.UPDATE_asar_mode);
			Update.VerifyLibs();

			Console.ReadKey(true);
		}
	}
}