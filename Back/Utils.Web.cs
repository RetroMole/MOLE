using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MOLE_Back.Utils
{
	/// <summary>
	/// Web Utility Methods
	/// </summary>
	public static class Web
	{
		public static Dictionary<String, String> GetGHReleases(string Owner, string Repo, int Page = 1, int Per_Page = 100)
		{
			Dictionary<string, string> Releases = new Dictionary<string, string>();

			string URL = String.Format("https://api.github.com/repos/{0}/{1}/releases?page={2},per_page={3}", Owner, Repo, Page, Per_Page);
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);

			req.UserAgent = "Vawlpe/MOLE";
			req.ContentType = "application/json; charset=utf-8";
			req.Accept = "application/vnd.github.v3+json";
			req.AllowAutoRedirect = true;


			HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
			JArray parsed = new JArray();
			using (var sr = new StreamReader(resp.GetResponseStream()))
			{
				parsed = JArray.Parse(sr.ReadToEnd());
			}

			for (int i = 0; i < parsed.Count; i++)
			{
				JObject asst = (JObject)parsed[i].SelectToken("assets", true).First;
				Releases.Add(asst.SelectToken("browser_download_url", true).Value<String>(), parsed[i].SelectToken("prerelease", true).Value<String>());
			}
			return Releases;
		}
	}
}
