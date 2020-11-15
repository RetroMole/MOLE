using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using MOLE_Back.Properties;
using System.IO.Compression;
using System.Runtime.Remoting.Messaging;

namespace MOLE_Back.Libs
{
	/// <summary>
	/// Verifies and updates libs
	/// </summary>
	public static class Update
	{
		/// <summary>
		/// Make sure all libraries are found, up to date, and compatible
		/// </summary>
		public static void UpdateSequence()
        {
			Console.WriteLine("Dependency Update Sequence");
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			//if (!File.Exists("lunarcompress.dll")) Console.WriteLine("LunarCompress Not Found"); // do this later
			if (Settings.Default.UPDATE_Asar != "disabled") // autoupdate on
			{
				bool IgnoreTime = !File.Exists("asar.dll");
				if (IgnoreTime) Console.WriteLine("Asar Not Found, Defaulting to latest {0}", Settings.Default.UPDATE_Asar);
				if (Settings.Default.UPDATE_Asar == "release") // releases
				{
					JObject latestrel = Utils.Web.GHGetLatestRelease("RPGHacker", "Asar");
					if (IgnoreTime || latestrel.SelectToken("tag_name").Value<string>() != "v"+Asar.ver2str(Asar.Version()))
					{
						string url = latestrel.SelectToken("assets")[0].SelectToken("browser_download_url").Value<string>();
						string zipname = Regex.Replace(url, "https://github.com/RPGHacker/asar/releases/download/.*/", String.Empty);
						new WebClient().DownloadFile(url, zipname);

						//	Extract
						string extractPath = Path.GetFullPath(zipname).Replace(".zip", String.Empty);
						if (extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)) extractPath += Path.DirectorySeparatorChar;
						Directory.CreateDirectory(extractPath);
						using (ZipArchive archive = ZipFile.OpenRead(zipname))
						{
							archive.GetEntry(@"dll/asar.dll").ExtractToFile(Path.Combine(extractPath, "asar.dll"), true);
						}

						// Cleanup
						File.Delete(zipname);
						File.Replace(extractPath + @"\asar.dll", AppDomain.CurrentDomain.BaseDirectory + @"\asar.dll", null);
						Directory.Delete(extractPath);
					}
				}
				else if (Settings.Default.UPDATE_Asar == "build") // latest build
				{
					try
					{
						string[] headers = new string[] {
							"Allow-Auto-Redirect: true",
							"User-Agent: Vawlpe/MOLE"
						};
						if (!IgnoreTime) headers.Append("If-Modified-Since: " + File.GetLastWriteTime("Asar.dll"));

                        HttpWebResponse resp = Utils.Web.MakeHttpRequest("https://random.muncher.se/ftp/asar/windows/win32/build/asar/MinSizeRel/asar.dll", headers);
						new StreamWriter("asar.dll").Write(new StreamReader(resp.GetResponseStream()).ReadToEnd());
                        Console.WriteLine("Asar Updated to build last modified at {0}", resp.LastModified);
                    }
					catch (WebException e)
					{
						if (e.Response != null)
						{
							if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotModified)
								Console.WriteLine("\nFile has not been modified");
							else
								throw new Exception("\nUnexpected status code = " + ((HttpWebResponse)e.Response).StatusCode);
						}
						else
						{
							throw new Exception("\nUnexpected Web Exception " + e.Message);
						}
					}
				}
			}
		}
	}
}