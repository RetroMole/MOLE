using System;
using System.CodeDom.Compiler;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using Newtonsoft.Json.Linq;

namespace LA_Back.web
{
	/// <summary>
	/// Deals with downloading Asar updates, and patching the hook and compiling it
	/// </summary>
	public static class AsarUpdater
	{
		/// <summary>
		/// Get any Asar build from Appveyor CI
		/// </summary>
		/// <param name="reqstr">Request String used to filter Appveyor build history (if you're unsure, check PresetAppveyorRequests</param>
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
				"asar.dll - " + BNR + " - " + JID + ".dll",
				false
			);

			web.WebUtils.DownloadFile
			(
				"https://raw.githubusercontent.com/RPGHacker/asar/master/src/asar-dll-bindings/c_sharp/asar.cs",
				"asar.cs"
			);

			CompileAsarHook();

			// Clean Up
			//File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asarhook.dll");
			//File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			//File.Move("asarhook.dll", AppDomain.CurrentDomain.BaseDirectory+@"\asarhook.dll");
			//File.Move("asar.dll - " + BNR + " - " + JID + ".dll", AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			//File.Delete("asar.cs");
			//Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			//File.Delete("asar.dll - " + BNR + " - " + JID + ".dll");


		}

		/// <summary>
		/// Get the latest build of Asar from the official github
		/// </summary>
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
			//File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asarhook.dll");
			//File.Delete(AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			//File.Move("asarhook.dll", AppDomain.CurrentDomain.BaseDirectory+@"\asarhook.dll");
			//File.Move("asar.dll", AppDomain.CurrentDomain.BaseDirectory+@"\asar.dll");
			//File.Delete("asar.cs");
			//Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			//Directory.Delete(extractPath);
			//File.Delete(name);
		}

		/// <summary>
		/// Compiles the Asar C# Hook
		/// </summary>
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

			PatchAsarHook(ref code);

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

		/// <summary>
		/// Moddifies and patches the Asar Hook code
		/// </summary>
		/// <param name="code">Ref to Asar Hook code</param>
		public static void PatchAsarHook(ref string code)
		{
			string add;
			using (FileStream fs = new FileStream("AsarExtPatch.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader reader = new StreamReader(fs))
				{
					add = reader.ReadToEnd();
				}
			}
			code = code.Replace("init", 			 "Init");
			code = code.Replace("close", 			 "Close");
			code = code.Replace("reset", 			 "Reset");

			code = code.Replace("patch", 			 "Patch");
			code = code.Replace("math", 			 "Math");
			code = code.Replace("maxromsize", 		 "MaxRomSize");

			code = code.Replace(" version", 		 " Version");
			code = code.Replace("apiversion", 		 "ApiVersion");

			code = code.Replace("getalldefines", 	 "GetAllDefines");
			code = code.Replace("getdefine", 		 "GetDefine");
			code = code.Replace("geterrors", 		 "GetErrors");
			code = code.Replace("getlabels", 		 "GetLabels");
			code = code.Replace("getlabelval", 	 	 "GetLabelVal");
			code = code.Replace("getmapper", 		 "GetMapper");
			code = code.Replace("getprints", 		 "GetPrints");
			code = code.Replace("getsymbolsfile", 	 "GetSymbolsFile");
			code = code.Replace("getwarnings", 	 	 "GetWarnings");
			code = code.Replace("getwrittenblocks",  "GetWrittenBlocks");
			code = code.Replace("resolvedefines", 	 "ResolveDefines");

			illfixitlater(ref code);

			code = code.Replace("Asardefine", 		 "AsarDefine");
			code = code.Replace("Asarerror", 		 "AsarError");
			code = code.Replace("Asarlabel", 		 "AsarLabel");
			code = code.Replace("Asarwrittenblock",  "AsarWrittenBlock");
			
			code = code.Replace("class Asar\n    {", "class Asar\n    {        " + add + "\n");
		}

		/// <summary>
		/// Collection of presets for Appveyor request strings
		/// </summary>
		public struct PresetAppveyorRequests
		{
			/// <summary>
			/// Latest successful build
			/// </summary>
			public static string latestSuccess = "recordsNumber=1&branch=master&Status=success";
		}

		/// <summary>
		/// god is dead
		/// </summary>
		/// <param name="code">Ref to Asar Hook Code</param>
		public static void illfixitlater(ref string code)
		{
			code = code.Replace("_Init", 			 "_init");
			code = code.Replace("_Close", 			 "_close");
			code = code.Replace("_Reset", 			 "_reset");

			code = code.Replace("_Patch", 			 "_patch");
			code = code.Replace("_Math", 			 "_math");
			code = code.Replace("_MaxRomSize", 		 "_maxRomSize");

			code = code.Replace("_Version", 		 "_version");
			code = code.Replace("_ApiVersion", 		 "_apiversion");

			code = code.Replace("_GetAllDefines", 	 "_getalldefines");
			code = code.Replace("_GetDefine", 		 "_getdefine");
			code = code.Replace("_GetErrors", 		 "_geterrors");
			code = code.Replace("_GetLabels", 		 "_getlabels");
			code = code.Replace("_GetLabelVal", 	 "_getlabelval");
			code = code.Replace("_GetMapper", 		 "_getmapper");
			code = code.Replace("_GetPrints", 		 "_getprints");
			code = code.Replace("_GetSymbolsFile", 	 "_getsymbolsfile");
			code = code.Replace("_GetWarnings", 	 "_getwarnings");
			code = code.Replace("_GetWrittenBlocks", "_getwrittenblocks");
			code = code.Replace("_ResolveDefines", 	 "_resolvedefines");
		}
	}
}