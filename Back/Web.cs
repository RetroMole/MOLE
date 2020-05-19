using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace LA_Back.web
{
    public static class WebUtils
    {
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
