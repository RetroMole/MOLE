using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MOLE_Back.Utils
{
	/// <summary>
	/// Web Utility Methods
	/// </summary>
	public static class Web
	{
		/// <summary>
		/// Http Request Wrapper
		/// </summary>
		/// <param name="URL">URL to request from</param>
		/// <param name="Headers">Array of strings containing headers where the header and its value are separated by a colon</param>
		/// <returns>Http Response</returns>
		public static HttpWebResponse MakeHttpRequest(string URL, string[] Headers)
        {
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
			req.Headers = new WebHeaderCollection();
			foreach (string h in Headers) 
			{
				if (h.StartsWith("Accept: "))
					req.Accept = h.Replace("Accept: ", String.Empty);
				else req.Headers.Add(h); 
			}
			return (HttpWebResponse)req.GetResponse();
        }

		/// <summary>
		/// Github API Request Wrapper
		/// </summary>
		/// <param name="Endpoint">Endpoint path including URL/Path parameters</param>
		/// <param name="Headers">Extra Headers. Don't include UserAgent, ContentType, AllowAutoRedirect, Accept, or Authorization</param>
		/// <param name="OAuth2Token">Optional OAuth2 Authorization Token</param>
		/// <returns>JSON Response</returns>
		public static JToken MakeGithubAPIRequest(string Endpoint, string[] Headers = null, string OAuth2Token = null) 
		{
			if (Headers == null)
			{
				Headers = (new string[] {
					"UserAgent: Vawlpe/MOLE",
					"ContentType: application/json;charset=utf-8",
					"AllowAutoRedirect: true",
					"Accept: application/vnd.github.v3+json",
				});
			}
			else
			{
				Headers = (string[])Headers.Concat(new string[] {
					"UserAgent: Vawlpe/MOLE",
					"ContentType: application/json;charset=utf-8",
					"AllowAutoRedirect: true",
					"Accept: application/vnd.github.v3+json",
				});
			}
			if (OAuth2Token != null) Headers.Append("Authorization: token " + OAuth2Token);
			var resp = MakeHttpRequest("https://api.github.com/"+Endpoint, Headers);
            using var sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd();
        }

		/// <summary>
		/// Get a list of all Github Releases 
		/// </summary>
		/// <param name="Owner">Repository Owner</param>
		/// <param name="Repo">Repository Name</param>
		/// <param name="Page">Page number</param>
		/// <param name="Per_Page">Amount of entries per page (Max 100)</param>
		/// <returns></returns>
		public static JArray GHGetReleases(string Owner, string Repo, int Page = 1, int Per_Page = 100)
		{
			string URL = String.Format("repos/{0}/{1}/releases?page={2},per_page={3}", Owner, Repo, Page, Per_Page);
			return new JArray(MakeGithubAPIRequest(URL));
		}

		/// <summary>
		/// Get latest non-prerelease, non-draft Release from a Github Repository
		/// </summary>
		/// <param name="Owner"></param>
		/// <param name="Repo"></param>
		/// <returns>Latest non-prerelease, non-draft Release</returns>
		public static JObject GHGetLatestRelease(string Owner, string Repo)
        {
			string URL = String.Format("repos/{0}/{1}/releases/latest", Owner, Repo);
			return (JObject)MakeGithubAPIRequest(URL);
        }
	}
}
