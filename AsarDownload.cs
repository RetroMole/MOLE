using System;
using System.CodeDom.Compiler;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
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

			web.WebUtils.DownloadFile
			(
				"https://raw.githubusercontent.com/RPGHacker/asar/master/src/asar-dll-bindings/c_sharp/asar.cs",
				"asar.cs"
			);

			CompileAsarHook();

			// Clean Up
			File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asarhook.dll");
			File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			File.Move("asarhook.dll", AppDomain.CurrentDomain.BaseDirectory+@"\asarhook.dll");
			File.Move("asar.dll - " + BNR + " - " + JID + ".dll", AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			File.Delete("asar.cs");
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			File.Delete("asar.dll - " + BNR + " - " + JID + ".dll");


		}

		public static void GetAsarLatestRelease()
		{
			//Single Request (GITHUB)
			JObject resp = web.WebUtils.GetJsonResponse(@"http://api.github.com/repos/RPGHacker/asar/releases/latest", true);
			string dl = resp.SelectToken("assets", true).First.SelectToken("browser_download_url", true).Value<String>();
			string name = resp.SelectToken("assets", true).First.SelectToken("name", true).Value<String>();

			// Download
			web.WebUtils.DownloadFile(dl, name);

			//	Extract
			var extractPath = Path.GetFullPath(name).Replace(".zip", String.Empty);
			if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				extractPath += Path.DirectorySeparatorChar;
			Directory.CreateDirectory(extractPath);

			string[] directories = new string[] { "dll", @"dll\bindings\c_sharp" };
			using (ZipArchive archive = ZipFile.OpenRead(name))
			{
				var result = from currEntry in archive.Entries
							 where directories.Contains(Path.GetDirectoryName(currEntry.FullName))
							 where !String.IsNullOrEmpty(currEntry.Name)
							 where currEntry.Name.EndsWith(".dll") || currEntry.Name.EndsWith(".cs")
							 select currEntry;

				foreach (ZipArchiveEntry entry in result)
				{
					entry.ExtractToFile(Path.Combine(extractPath, entry.Name), true);
				}
				Directory.SetCurrentDirectory(extractPath);
			}

			CompileAsarHook();
			
			// Clean up
			File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asarhook.dll");
			File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			File.Move("asarhook.dll", AppDomain.CurrentDomain.BaseDirectory+@"\asarhook.dll");
			File.Move("asar.dll", AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			File.Delete("asar.cs");
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			Directory.Delete(extractPath);
			File.Delete(name);
		}

		public static  void CompileAsarHook()
		{
			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerParameters parameters = new CompilerParameters();
			string code = "";

			using (FileStream fs = new FileStream("asar.cs", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader reader = new StreamReader(fs))
				{
					code = reader.ReadToEnd();
				}
			}

			parameters.ReferencedAssemblies.Add("System.Linq.dll");
			parameters.ReferencedAssemblies.Add("System.Core.dll");
			parameters.CompilerOptions = "/unsafe";
			parameters.OutputAssembly = "asarhook.dll";

			parameters.GenerateInMemory = false;
			parameters.GenerateExecutable = false;

			CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

			// Throw Compile Errors
			if (results.Errors.HasErrors)
			{
				StringBuilder sb = new StringBuilder();

				foreach (CompilerError error in results.Errors)
				{
					sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
				}

				throw new InvalidOperationException(sb.ToString());
			}

		}

		public struct PresetAppveyorRequest
		{
			public static string latestSuccess = "recordsNumber=1&branch=master&Status=success";
		}
	}
}