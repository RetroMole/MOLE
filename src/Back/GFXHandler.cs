using MOLE_Back.Libs;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;

namespace MOLE_Back
{
    /// <summary>
    /// Handles Graphics and Palettes
    /// </summary>
    public class GFXHandler
    {
        /// <summary>
        /// For each GFXHandler you get a free ROMHandler to boot
        /// </summary>
        public ROMHandler ROMHandler { get; protected set; }


		/// <summary>
        /// Creates a GFXHandler from a ROM Path
        /// </summary>
        /// <param name="Path">ROM Path</param>
        public GFXHandler(string Path)
        {
            ROMHandler = new ROMHandler(Path);   
        }

        /// <summary>
        /// Creates a GFXHandler from a ROMHandler
        /// </summary>
        /// <param name="rh">ROMHandler</param>
        public GFXHandler(ROMHandler rh)
        {
            ROMHandler = rh;
        }


        /// <summary>
        /// Converts 24-Bit RGB colors to SNES' 15-Bit BGR colors
        /// </summary>
        /// <param name="R">Red value</param>
        /// <param name="G">Green value</param>
        /// <param name="B">Blue Value</param>
        /// <returns>SNES 15-bit BGR</returns>
        public ushort Col2Pal(uint R, uint G, uint B)
        {
            R >>= 3; // /= 8;
            G >>= 3; // /= 8;
            B >>= 3; // /= 8;
            R += R / 32;
            G += G / 32;
            B += B / 32;
            //return (ushort)((B * 1024) + (G * 32) + R);
	        return (ushort)((B << 10) | (G << 5) | R);
        }

        /// <summary>
        /// Converts 15-Bit BGR colors to SNES' 24-Bit RGB colors
        /// </summary>
        /// <param name="col">15-Bit BGR Value</param>
        /// <returns>Array holding R, G, B Values</returns>
        public uint[] Pal2Col(ushort col)
        {
            uint[] RGB = new uint[3];
            RGB[0] = (uint)(col % 32 * 8);
            RGB[1] = (uint)(col / 32 % 32 * 8);
            RGB[2] = (uint)(col / 1024 % 32 * 8);
            return RGB;
        }

        public unsafe byte[] tempSPR()
        {
            byte[] buff  = new byte[0x10000];
            byte[] pxmap = new byte[0x10000];
            uint size;
            byte[] title = new byte[22];
            string tit = "5355504552204D4152494F574F524C44202020202020";
            byte cntry;
            byte ver;
            byte[] mariogfxpntr = new byte[3];
            uint marioGfxAddress;

            Array.Copy(ROMHandler.ROM, 0x7FC0, title, 0, 22);
            Array.Copy(ROMHandler.ROM, 0x38D8, mariogfxpntr, 0, 2);
            Array.Copy(ROMHandler.ROM, 0x3890, mariogfxpntr, 2, 1);
            marioGfxAddress = LC.SNEStoPC(uint.Parse(Utils.Hex.ByteArrToHexStr(mariogfxpntr), System.Globalization.NumberStyles.HexNumber), 1, 0);
            if (Utils.Hex.CompareBytesAndString(title, tit))
            {
                cntry = ROMHandler.ROM[0x7FD9];
                ver = ROMHandler.ROM[0x7FDB];
                if (ver==0 && cntry==1)
                {
                    if (LC.OpenFile(ROMHandler.ROMPath, 2))
                    {
                        size = LC.Decompress(buff, marioGfxAddress, 0x10000, 1, 0, null);
                        LC.CloseFile();
                        if (size!=0)
                        {
                            if (size > 0x10000) size = 0x10000;
                            LC.CreatePixelMap(buff, pxmap, size / 32, 4);
                        }
                    }
                }
            }
            return pxmap;
        }
    }
}
