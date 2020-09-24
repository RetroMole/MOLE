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
		/// Gets an HTTP response from a website
		/// </summary>
		/// <param name="URL">URL to request from</param>
		/// <param name="UserAgent">UserAgent for the request, defaults to github repo name for github requests</param>
		/// <param name="ContentType">What Type of Content to return, defaults to JSON</param>
		/// <returns></returns>
		public static JObject GetHttpResponse(string URL, string UserAgent = @"Vawlpe/MOLE", string ContentType = @"application/json; charset=utf-8")
        {
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
			req.UserAgent = UserAgent;
			req.ContentType = ContentType;
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
		public static bool DownloadFile(string url, string name, bool err = false)
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
