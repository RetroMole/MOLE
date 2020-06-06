using AsarCLR;
using System;

namespace LA_Back
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

        /// <summary>
        /// god is dead
        /// </summary>
        /// <returns>sprite</returns>
        public byte[] tempSPR()
        {
            return new byte[]  { 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 ,
  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 ,
  0,  0,  0,  0,  8,  8,  8,  8,  8,  0,  0,  0,  0,  0,  0,  0 ,
  0,  0,  0,  8,  9,  9,  5,  9,  9,  8,  8,  0,  0,  0,  0,  0 ,
  0,  0,  0,  8, 13,  1,  5,  4,  8, 13, 13,  8,  0,  0,  0,  0 ,
  0,  0,  2,  2,  2,  2,  2,  2,  2,  9,  9, 13,  8,  0,  0,  0 ,
  0,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2, 13,  9,  8,  0,  0 ,
  0,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2, 13, 13,  8,  0 ,
  0,  0,  2,  2,  1,  1, 14,  1,  1, 14,  2,  2, 13, 13,  8,  0 ,
  0,  0,  0,  0,  1,  2,  6,  2,  1, 14, 14,  2,  2,  6, 14,  0 ,
  0,  0,  3,  6,  1,  2,  6,  2,  1,  6, 14,  2,  2,  6,  3, 14 ,
  0,  0,  3,  6,  6,  6, 14, 14,  6,  6,  2,  2,  2, 14,  3, 14 ,
  0,  0,  3, 14, 14, 14, 14,  3,  2,  6,  6,  2, 14, 14, 14,  0 ,
  0,  2,  2,  2,  2,  2,  2,  2,  2,  2, 14, 14, 14,  2,  2,  0 ,
  0,  0,  2,  2,  2,  2,  2,  2,  6,  6, 14, 14,  2,  2,  0,  0 ,
  0,  0,  0,  3, 14, 14, 14, 14, 14, 14, 14,  3,  2,  0,  0,  0 ,
  0,  0,  0,  0,  3,  3,  3,  8,  8, 13, 13,  8,  0,  0,  0,  0 ,
  0,  0,  0,  3,  1,  1,  1,  3, 13,  8,  9,  9,  8,  0,  0,  0 ,
  0,  0,  3,  1,  1,  1,  1,  1,  3,  9,  9, 13, 13,  8,  0,  0 ,
  0,  0,  3,  1,  1,  1,  1,  1,  3,  9,  9, 13, 13,  8,  0,  0 ,
  0,  0,  0,  3,  1,  1,  1,  3, 13,  9, 13, 13,  8, 10,  0,  0 ,
  0,  0,  0,  1,  3,  3,  3,  8,  8, 13, 13,  8, 10, 10,  0,  0 ,
  0,  0,  0, 10, 11, 12, 12, 12,  8,  8,  8, 10, 11, 10,  0,  0 ,
  0,  0,  0, 10, 11, 12, 12, 12, 12, 12, 10, 11, 11, 10,  0,  0 ,
  0,  0,  0,  0, 10, 11, 10, 12, 12, 11, 11, 11, 10,  0,  0,  0 ,
  0,  0,  0,  0, 10, 11, 10, 12, 12, 11, 11, 11, 10,  0,  0,  0 ,
  0,  0,  0,  0,  0, 10, 10, 10, 10, 10, 10, 10,  0,  0,  0,  0 ,
  0,  0,  0,  0,  2,  3,  2,  3,  3,  3,  3,  3,  0,  0,  0,  0 ,
  0,  0,  0,  2,  5,  2,  5,  3,  3,  3,  3,  2,  0,  0,  0,  0 ,
  0,  0,  0,  2,  2,  2,  2,  2,  2,  2,  2,  2,  0,  0,  0,  0 ,
  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0 };
        }
    }

    /// <summary>
    /// 8x8 4BPPP GFX Tile
    /// </summary>
    public class GFXTile
    {
        int GFXOffset
        {
            get
            {
                Asar.Init();
                var a = Asar.SnesToPc(0x088000);
                Asar.Close();
                return a;
            }
        }
        readonly int Length = 130317;

        /// <summary>
        /// Creates an 8x8 GFX Tile from data as seen in ROM
        /// </summary>
        /// <param name="bpp">Bits per pixel (amount of bitplanes)</param>
        /// <param name="data">Byte array containing the data for an 8x8 GFX Tile as seen in ROM<br/>Supports: 2BPP (length 16, colors 0-3), 3BPP (length 24, colors 0-7),<br/>4BPP (length 32, colors 0-15), and 8BPP (length 64, colors 0-255)</param>
        public GFXTile(byte bpp, byte[] data)
        { 
        }
    }
}
