using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LA.web
{
	public static class Downloader
	{
		public static void getAsarBuild(string reqstr)
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
			using (var client = new WebClient())
			{
				client.DownloadFile("https://ci.appveyor.com/api/buildjobs/" + JID + "/artifacts/build%2Fasar%2Flibasar.dll", "asar.dll - " + BNR + " - " + JID + ".dll");
			}
			Console.WriteLine("Successfully Downloaded asar dll as: " + "asar.dll - " + BNR + " - " + JID + ".dll");

		}
		
		[Obsolete("Currently Investigating appveyors history system to search more freely",true)]
		public static void getAsarLatestRelease()
		{
			// Initial Request (GITHUB tag)
			JObject resp0 = web.WebUtils.GetJsonResponse("https://api.github.com/repos/RPGHacker/asar/releases/latest");
			string tag = resp0.SelectToken("tag_name", true).Value<String>();
			

		}
	}
}