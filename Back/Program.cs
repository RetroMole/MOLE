using System;
using MOLE_Back.Libs;

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
			Update.VerifyLibs();

			Console.ReadKey(true);
		}
	}
}