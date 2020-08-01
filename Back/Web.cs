using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace MOLE_Back.web
{
	/// <summary>
	/// Utility class for random Web-related methods
	/// </summary>
    public static class WebUtils
    {
		/// <summary>
		/// Gets a JSON response from a website (http(s) request))
		/// </summary>
		/// <param name="url">URL to request from</param>
		/// <param name="github">If true adds a special required UserAgent header to the request for github</param>
		/// <returns></returns>
        public static JObject GetJsonResponse(string url, bool github = false)
        {
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			if (github) req.UserAgent = @"Vawlpe/Lunar-Alchemy";
			req.ContentType = "application/json; charset=utf-8";
			var resp = (HttpWebResponse)req.GetResponse();
			var parsed = new JObject();
			using (var sr = new StreamReader(resp.GetResponseStream()))
			{
				parsed = JObject.Parse(sr.ReadToEnd());
			}
			return parsed;
		}

		/// <summary>
		/// Downloads a file from a URL
		/// </summary>
		/// <param name="url">Source URL</param>
		/// <param name="name">Destination Path</param>
		/// <param name="err">If false it will stop errors from throwing (will still display them)</param>
		/// <returns>True if file successfully downloaded</returns>
		public static bool DownloadFile(string url, string name, bool err = true)
		{
			try
			{
				using (var client = new WebClient())
				{
					client.DownloadFile(url, name);
				}
			}
			catch
			{
				if (err)
				{
					throw new Exception(String.Format("File '{0}' Failed to Download",name));
				}
				else
				{
					Console.WriteLine("File '{0}' Failed to Download, but an exception was deliberately not thrown",name);
				}
				return false;
			}
			Console.WriteLine("File '{0}' Failed to Download", name);
			return true;
		}
	}
}
