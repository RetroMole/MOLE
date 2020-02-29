using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace LA.web
{
    public static class WebUtils
    {
        public static JObject GetJsonResponse(string url)
        {
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.ContentType = "application/json; charset=utf-8";
			var resp = (HttpWebResponse)req.GetResponse();
			var parsed = new JObject();
			using (var sr = new StreamReader(resp.GetResponseStream()))
			{
				parsed = JObject.Parse(sr.ReadToEnd());
			}
			return parsed;
		}		
    }
}
