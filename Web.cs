using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace LA.web
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

		public static bool DownloadFile(string url, string name, bool err = false)
		{
			using (var client = new WebClient())
			{
				client.DownloadFile(url, name);
			}
			if(err && !File.Exists(name))
			{
				throw new Exception("File '"+name+"' Failed to Download");
			}
			else
			{
				Console.WriteLine(File.Exists(name) ? "File '" + name + "' Succesfully Downloaded" : "File '{0}' Failed to Download, but an exception was not thrown");
				return File.Exists(name);
			}
		}
	}
}
