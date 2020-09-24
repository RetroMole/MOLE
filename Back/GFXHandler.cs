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
            LC.OpenFile(ROMHandler.ROMPath, 2);
            byte[] GFX = new byte[32808];
            uint size = LC.Decompress(GFX, LC.SNEStoPC(0x088000, (uint)LC.AddressFlags.LC_LOROM, (uint)LC.Header.LC_HEADER), 32808, (uint)LC.CompressionFormats.LC_LZ2, 0, null);
            
            byte[] spr = new byte[size];
            LC.CreatePixelMap(GFX, spr, size/64, (uint)LC.GraphicsFormats.LC_4BPP);
            return spr;
        }
    }
}
