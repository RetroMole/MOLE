using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LA.web
{
	public static class Downloader
	{
		public static void GetAsarBuild(string reqstr)
		{
			// Initial Request (Appveyor BNR)
			JObject resp0 = web.WebUtils.GetJsonResponse("https://ci.appveyor.com/api/projects/RPGHacker/asar/history?" + reqstr);
			string BNR = resp0.SelectToken("builds", true)[0].SelectToken("buildNumber", true).Value<String>();
			Console.WriteLine("BNR: " + BNR);

			// Secondary Request (Appveyor JID)
			JObject resp1 = web.WebUtils.GetJsonResponse("https://ci.appveyor.com/api/projects/RPGHacker/asar/build/" + BNR);
			string JID = resp1.SelectToken("build.jobs", true).First.SelectToken("jobId", true).Value<String>();
			Console.WriteLine("JID: " + JID);

			// Download
			web.WebUtils.DownloadFile
			(
				"https://ci.appveyor.com/api/buildjobs/" + JID + "/artifacts/build%2Fasar%2Flibasar.dll",
				"asar.dll - " + BNR + " - " + JID + ".dll"
			);

		}
		
		public static void GetAsarLatestRelease()
		{
			//Single Request (GITHUB)
			JObject resp = web.WebUtils.GetJsonResponse(@"http://api.github.com/repos/RPGHacker/asar/releases/latest", true);
			string dl = resp.SelectToken("assets", true).First.SelectToken("browser_download_url",true).Value<String>();
			string name = resp.SelectToken("assets", true).First.SelectToken("name", true).Value<String>();

			// Download
			web.WebUtils.DownloadFile(dl, name);

			var extractPath = Path.GetFullPath(name).Replace(".zip",String.Empty);

			if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				extractPath += Path.DirectorySeparatorChar;

			using (ZipArchive archive = ZipFile.Open(name, ZipArchiveMode.Update))
			{
				foreach (ZipArchiveEntry entry in archive.Entries)
				{
					if (entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || 
						entry.FullName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
					{
						string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
						if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
							entry.ExtractToFile(destinationPath);
					}
				}
			}

			Directory.SetCurrentDirectory(extractPath);

		}
	}
}