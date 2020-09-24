using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.CSharp;
using Newtonsoft.Json.Linq;

namespace MOLE_Back.Libs
{
	/// <summary>
	/// Verifies and updates libs
	/// </summary>
	public static class Update
	{
		const int ExpectedAsarVer = 10701;
		const int ExpectedLCVer = 181;

		/// <summary>
		/// Get any Asar build from Appveyor CI
		/// </summary>
		/// <param name="reqstr">Request String used to filter Appveyor build history (if unsure, check PresetAppveyorRequests</param>
		public static void GetAsarBuild(string reqstr)
		{
			// Initial Request (Appveyor BNR)
			JObject resp0 = web.WebUtils.GetHttpResponse("https://ci.appveyor.com/api/projects/RPGHacker/asar/history?" + reqstr);
			string BNR = resp0.SelectToken("builds", true)[0].SelectToken("buildNumber", true).Value<String>();
			Console.WriteLine("BNR: " + BNR);

			// Secondary Request (Appveyor JID)
			JObject resp1 = web.WebUtils.GetHttpResponse("https://ci.appveyor.com/api/projects/RPGHacker/asar/build/" + BNR);
			string JID = resp1.SelectToken("build.jobs", true).First.SelectToken("jobId", true).Value<String>();
			Console.WriteLine("JID: " + JID);

			// Download
			web.WebUtils.DownloadFile
			(
				"https://ci.appveyor.com/api/buildjobs/" + JID + "/artifacts/build%2Fasar%2Flibasar.dll",
				"asar.dll - BNR." + BNR + " - JID." + JID + ".dll",
				true
			);

			// Cleanup
			File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			File.Move("asar.dll - BNR." + BNR + " - JID." + JID + ".dll", AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			File.Delete("asar.dll - BNR." + BNR + " - JID." + JID + ".dll");
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);


		}

		/// <summary>
		/// Get the latest build of Asar from the official github
		/// </summary>
		public static void GetAsarLatestRelease()
		{
			//Single Request (GITHUB)
			JObject resp = web.WebUtils.GetHttpResponse(@"http://api.github.com/repos/RPGHacker/asar/releases/latest");
			string dl = resp.SelectToken("assets", true).First.SelectToken("browser_download_url", true).Value<String>();
			string name = resp.SelectToken("assets", true).First.SelectToken("name", true).Value<String>();

			// Download
			web.WebUtils.DownloadFile(dl, name);

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
		}

		/// <summary>
		/// Make sure all libraries are found, up to date, and compatible
		/// </summary>
		public static void VerifyLibs()
        {
			Console.WriteLine("Verifying Libs...        Expected | Current");
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			if (File.Exists("asar.dll"))
            {
				Asar.Init();
				Console.WriteLine("Asar DLL found:             {0} |   {1}", ExpectedAsarVer, Asar.Version());
				if (Asar.Version() != ExpectedAsarVer) throw new Exception("Asar Version Not Compatible");
				Asar.Close();
            }
			else throw new Exception("Asar Not Found");

			if (File.Exists("lunarcompress.dll"))
			{
				Console.WriteLine("LunarCompress DLL found:      {0} |     {1}", ExpectedLCVer, LC.Version());
				if (LC.Version() != ExpectedLCVer) throw new Exception("LunarCompress Version Not Compatible");
			}
			else throw new Exception("LunarCompress Not Found");
		}

		/// <summary>
		/// Collection of presets for Appveyor request strings
		/// </summary>
		public struct PresetAppveyorRequests
		{
			/// <summary>
			/// Latest successful build
			/// </summary>
			public static string LatestSuccess = "recordsNumber=1&branch=master&Status=success";
		}
	}
}