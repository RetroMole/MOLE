using System;
using System.Linq;

namespace MOLE_Back.Utils
{
	/// <summary>
	/// Hex Utility Methods
	/// </summary>
	public static class Hex
	{
		/// <summary>
		/// Converts a HEX string to an array of Bytes
		/// </summary>
		/// <param name="hex">HEX String, make sure it has an even length</param>
		/// <returns>Byte array containing the values from the HEX string</returns>
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

		/// <summary>
		/// Gets the HEX value a character represents, meant for internal use in other functions
		/// </summary>
		/// <param name="hex"></param>
		/// <returns></returns>
		public static int GetHexVal(char hex)
		{
			int val = (int)hex;
			//For uppercase A-F letters:
			return val - (val < 58 ? 48 : 55);
			//For lowercase a-f letters:
			//return val - (val < 58 ? 48 : 87);
		}

		/// <summary>
		/// Converts an array of bytes to a string of HEX
		/// </summary>
		/// <param name="bytes">Byte Array to convert</param>
		/// <param name="offset">Offset into the array</param>
		/// <param name="count">Amount of bytes to be copied</param>
		/// <returns>String of HEX characters representing the data in the input Byte Array</returns>
		public static string ByteArrToHexStr(byte[] bytes, int offset = 0, int count = -1)
		{
			int b;
			char[] c;
			byte[] nb;

			if (count == -1) count = bytes.Length;
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

		public static bool CompareBytesAndString(byte[] bytes, string hex, int offset = 0)
        {
			if (hex.Length % 2 == 1)
				throw new Exception("The hex string cannot have an odd number of digits");

			byte[] nb = new byte[hex.Length/2];
			Array.Copy(bytes, nb, nb.Length);
			byte[] cmp = HexStrToByteArr(hex);

			return nb.SequenceEqual(cmp);
        }
	}
}