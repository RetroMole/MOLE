using System;
using AsarCLR;

namespace LA
{
	public static class Utils
	{
		public static byte[] HexStrToByteArr(string hex)
		{
			if (hex.Length % 2 == 1)
				throw new Exception("The hex string cannot have an odd number of digits");

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
			{
				arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
			}

			return arr;
		}

		private static int GetHexVal(char hex)
		{
			int val = (int)hex;
			//For uppercase A-F letters:
			return val - (val < 58 ? 48 : 55);
			//For lowercase a-f letters:
			//return val - (val < 58 ? 48 : 87);
		}

		public static string ByteArrToHexStr(byte[] bytes, int? count, int offset)
		{
			int b;
			char[] c;
			byte[] nb;

			if (count == null)
			{
				count = bytes.Length * 2;
			}
			nb = new byte[(int)count];
			c = new char[(int)count * 2];

			Array.Copy(bytes, offset, nb, 0, (long)count);

			for (int i = 0; i < count; i++)
			{
				b = nb[i] >> 4;
				c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
				b = nb[i] & 0xF;
				c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
			}
			return new string(c);

		}
	}
}