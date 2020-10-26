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

			Settings.Default.UPDATE_Asar = "release";
			Console.WriteLine("Settings: UPDATE_Asar = {0}", Settings.Default.UPDATE_Asar);
			Update.UpdateSequence();

			Settings.Default.UPDATE_Asar = "build";
			Console.WriteLine("Settings: UPDATE_Asar = {0}", Settings.Default.UPDATE_Asar);
			Update.UpdateSequence();

			Console.ReadKey(true);
		}
	}
}