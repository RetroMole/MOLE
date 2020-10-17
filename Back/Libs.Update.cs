using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using MOLE_Back.Properties;

namespace MOLE_Back.Libs
{
	/// <summary>
	/// Verifies and updates libs
	/// </summary>
	public static class Update
	{
		/*
		const int ExpectedAsarVer = 10701;
		const int ExpectedLCVer = 181;

			//	Extract
			string extractPath = Path.GetFullPath(name).Replace(".zip", String.Empty);
			if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)) extractPath += Path.DirectorySeparatorChar;
			Directory.CreateDirectory(extractPath);
			using (ZipArchive archive = ZipFile.OpenRead(name))
			{
				archive.GetEntry(@"dll\asar.dll").ExtractToFile(Path.Combine(extractPath, @"asar.dll"), true);
			}

			// Cleanup
			Directory.SetCurrentDirectory(extractPath);
			File.Delete(name);
			File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			File.Move("asar.dll", AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			Directory.Delete(extractPath);
		*/

		/// <summary>
		/// Make sure all libraries are found, up to date, and compatible
		/// </summary>
		public static void UpdateSequence()
        {
			Console.WriteLine("Start Update Sequence");
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			if (!File.Exists("asar.dll")) Console.WriteLine("Asar Not Found\n");
			//if (!File.Exists("lunarcompress.dll")) Console.WriteLine("LunarCompress Not Found"); // do this later
			if (Settings.Default.UPDATE_asar) // autoupdate on
			{
				if (Settings.Default.UPDATE_asar_mode == "release") // releases only
				{
					Dictionary<string, string> rels = Utils.Web.GetGHReleases("RPGHacker", "Asar");
					Console.WriteLine("DLURL									:	Prerelease?		Draft?");
					foreach (KeyValuePair<string, string> entry in rels)
					{
						Console.WriteLine("{0}	:	{1}", entry.Key, entry.Value);
					}
				}
				else if (Settings.Default.UPDATE_asar_mode == "build") // latest build
				{
					// Request file
					HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
						new Uri("https://random.muncher.se/ftp/asar/windows/win32/build/asar/MinSizeRel/asar.dll"));

					// Add IfModifiedSince Header
					DateTime targetDate = File.GetLastWriteTime("asar.dll");    // Set a target date to the current files modified date
					req.IfModifiedSince = targetDate;
					try
					{
						// Assign the response object of 'HttpWebRequest' to a 'HttpWebResponse' variable.
						HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
						using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
						{
							using (StreamWriter sw = new StreamWriter("asar.dll"))
							{
								//sw.Write(sr.ReadToEnd());
								Console.WriteLine("Asar Updated to build last modified at {0} (dl disabled for tests)", resp.LastModified);
							}
						}
					}
					catch (WebException e)
					{
						if (e.Response != null)
						{
							if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotModified)
								Console.WriteLine("\nFile has not been modified");
							else
								Console.WriteLine("\nUnexpected status code = " + ((HttpWebResponse)e.Response).StatusCode);
						}
						else
						{
							Console.WriteLine("\nUnexpected Web Exception " + e.Message);
						}
					}
				}
			}
		}
	}
}