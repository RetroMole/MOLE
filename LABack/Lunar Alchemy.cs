using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace LA
{
	public class Lunar_Alchemy 
	{
		public static void Main(string[] args)
		{

			/*ROMHandler rh = new ROMHandler(String.Format("../../ROMs/Clean/{0}",args[0]));

			Console.WriteLine("Testing HexWrite() and Undo-Redo system");
			Console.WriteLine("Initial Value of first 8 bytes: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.HexWrite("0123456789ABCDEF", 0);

			Console.WriteLine("HexWrite() modified 8 bytes: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();

			Console.WriteLine("Undo: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Redo();

			Console.WriteLine("Redo: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.HexWrite("89ABCDEF", 0);

			Console.WriteLine("HexWrite modified 4 bytes: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();

			Console.WriteLine("Undo: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			rh.UR.Undo();

			Console.WriteLine("Undo: ");
			Console.WriteLine(Utils.ByteArrToHexStr(rh.ROM, 8, 0));

			Console.WriteLine("HexWrite() modified 4 bytes: ");
			rh.HexWrite("89ABCDEF", 0);

			Console.WriteLine("Saving as {0} ... ", rh.ROMName + "_TEST.smc");
			rh.Save();
			*/

			//GFXHandler gh = new GFXHandler(String.Format("../../ROMs/Clean/{0}", args[0]));

			//web.Downloader.GetAsarBuild(web.Downloader.PresetAppveyorRequest.latestSuccess);
			//web.Downloader.GetAsarLatestRelease();


			/*
			AsarCLR.Asar.init();
			int[] input = new int[] 
			{ 
				0x088000
			};
			foreach (int i in input)
			{
				var pc = AsarCLR.AsarExt.SnesToPc(i);
				var snes = AsarCLR.AsarExt.PcToSnes(pc);
				Console.WriteLine("0x{0:X}", i);
				Console.WriteLine("0x{0:X}", pc);
				Console.WriteLine("0x{0:X}", snes);
			}
			*/


			// Request (LOGIN)
			HttpWebRequest req0 = (HttpWebRequest)WebRequest.Create("https://dev.opendrive.com/api/v1/session/login.json");
			req0.ContentType = "application/json; charset=utf-8";
			req0.Method = "POST";

			// BODY
			string postData0 = "{ \"username\" : \"bertiectozer@gmail.com\", \"passwd\" : \"jazmin6215\"}";
			ASCIIEncoding encoding = new ASCIIEncoding();
			byte[] byte0 = encoding.GetBytes(postData0);
			req0.ContentType = "application/json";
			req0.ContentLength = byte0.Length;
			Stream newStream0 = req0.GetRequestStream();
			newStream0.Write(byte0, 0, byte0.Length);

			// Response
			var resp0 = (HttpWebResponse)req0.GetResponse();
			var parsed0 = new JObject();
			using (var sr = new StreamReader(resp0.GetResponseStream()))
			{
				parsed0 = JObject.Parse(sr.ReadToEnd());
			}
			string SessionID = parsed0.SelectToken("SessionID", true).Value<String>();

			Console.WriteLine("Session ID: " + SessionID);





			// Request (CREATE)
			HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create("https://dev.opendrive.com/api/v1/upload/create_file.json");
			req1.ContentType = "application/json; charset=utf-8";
			req1.Method = "POST";

			// BODY
			string postData1 = "{ \"session_id\" : \""+SessionID+"\", \"folder_id\" : \"NTBfMTA4MTMwM18\", \"file_name\" : \"TEST_API.txt\"}";
			byte[] byte1 = encoding.GetBytes(postData1);
			req1.ContentLength = byte1.Length;
			Stream newStream1 = req1.GetRequestStream();
			newStream1.Write(byte1, 0, byte1.Length);

			// Response
			var resp1 = (HttpWebResponse)req1.GetResponse();
			var parsed1 = new JObject();
			using (var sr = new StreamReader(resp1.GetResponseStream()))
			{
				parsed1 = JObject.Parse(sr.ReadToEnd());
			}
			string FileID = parsed1.SelectToken("FileId").Value<String>();
			string TempLocation = parsed1.SelectToken("TempLocation").Value<String>();

			int FileSize;
			using (var sr = new StreamReader(new FileStream("TEST_API.txt", FileMode.Open, FileAccess.ReadWrite)))
			{
				byte[] bytex = encoding.GetBytes(sr.ReadToEnd());
				FileSize = bytex.Length;
			}

			Console.WriteLine("FileID: " + FileID);
			Console.WriteLine("FileSize: " + FileSize);
			Console.WriteLine("TempLocation: " + TempLocation);




			// Request (OPEN)
			HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create("https://dev.opendrive.com/api/v1/upload/open_file_upload.json");
			req2.ContentType = "application/json; charset=utf-8";
			req2.Method = "POST";

			// BODY
			string postData2 = "{ \"session_id\" : \"" + SessionID + "\", \"file_id\" : \"" + FileID + "\", \"file_size\" : " + FileSize + "}";
			byte[] byte2 = encoding.GetBytes(postData2);
			req2.ContentLength = byte2.Length;
			Stream newStream2 = req2.GetRequestStream();
			newStream2.Write(byte2, 0, byte2.Length);

			// Response
			var resp2 = (HttpWebResponse)req2.GetResponse();
			var parsed2 = new JObject();
			using (var sr = new StreamReader(resp2.GetResponseStream()))
			{
				parsed2 = JObject.Parse(sr.ReadToEnd());
			}




			// Request (UPLOAD)
			HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create("https://dev.opendrive.com/api/v1/upload/upload_file_chunk.json");
			req3.ContentType = "x-www-form-urlencoded; charset=utf-8";
			req3.Method = "POST";

			// BODY
			string postData3 = "{\"session_id\" : \""+SessionID+ "\", \"file_id\" : \"" + FileID + "\", \"temp_location\" : \"" + TempLocation + "\", \"chunk_offset\" : 0, \"chunk_size\" : \"" + FileSize + "\"}";
			byte[] byte3 = encoding.GetBytes(postData3);
			req3.ContentLength = byte3.Length;
			Stream newStream3 = req3.GetRequestStream();
			newStream3.Write(byte3, 0, byte3.Length);

			// Response
			var resp3 = (HttpWebResponse)req3.GetResponse();
			var parsed3 = new JObject();
			using (var sr = new StreamReader(resp3.GetResponseStream()))
			{
				parsed3 = JObject.Parse(sr.ReadToEnd());
			}
			Console.WriteLine(parsed3.ToString());

			Console.ReadKey();
		}
	}
}