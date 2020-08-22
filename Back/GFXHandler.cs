using AsarCLR;
using System;
using System.Linq;
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

        /// <summary>
        /// god is dead
        /// </summary>
        /// <returns>sprite</returns>
        public byte[] tempSPR(int s)
        {
            GFXTile[] spr = new GFXTile[]
            {
                new GFXTile(Utils.HexStrToByteArr(Utils.ByteArrToHexStr(ROMHandler.ROM,  0x02FC00, 32)), 4),
                new GFXTile(Utils.HexStrToByteArr(Utils.ByteArrToHexStr(ROMHandler.ROM,  0x02FC00+32, 32)), 4),
                new GFXTile(Utils.HexStrToByteArr(Utils.ByteArrToHexStr(ROMHandler.ROM,  0x02FE00, 32)), 4),
                new GFXTile(Utils.HexStrToByteArr(Utils.ByteArrToHexStr(ROMHandler.ROM,  0x02FE00+32, 32)), 4),

                new GFXTile(Utils.HexStrToByteArr(Utils.ByteArrToHexStr(ROMHandler.ROM,  0x02E080, 32)), 4),
                new GFXTile(Utils.HexStrToByteArr(Utils.ByteArrToHexStr(ROMHandler.ROM,  0x02E080+32, 32)), 4),
                new GFXTile(Utils.HexStrToByteArr(Utils.ByteArrToHexStr(ROMHandler.ROM,  0x02E280, 32)), 4),
                new GFXTile(Utils.HexStrToByteArr(Utils.ByteArrToHexStr(ROMHandler.ROM,  0x02E280+32, 32)), 4)

            };
            
            
            return spr[s].img;
        }
    }

    /// <summary>
    /// 8x8 GFX Tile
    /// </summary>
    public class GFXTile
    {
        public byte[] img;

        /// <summary>
        /// Creates an 8x8 GFX Tile from data as seen in ROM<br/>
        /// Supports: 2BPP (data length 16, 4 colors), 3BPP (data length 24, 8 colors),<br/>
        /// and 4BPP (data length 32, 16 colors)
        /// </summary>
        /// <param name="bpp">Bits per pixel (amount of bitplanes)</param>
        /// <param name="data">Byte array containing the data for an 8x8 GFX Tile as seen in ROM</param>
        public GFXTile(byte[] data,byte bpp=4)
        {
            switch (bpp)
            {
                case 2:
                    byte[] bp0 = new byte[8];
                    byte[] bp1 = new byte[8];
                    img = new byte[64];
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (i % 2 == 1)
                        {
                            bp0[i / 2] = data[i];
                        }
                        else
                        {
                            bp1[i / 2] = data[i];
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            byte b0 = (byte)((bp0[i] & (1 << j)) >> j);
                            byte b1 = (byte)((bp1[i] & (1 << j)) >> j);
                            img[8 * i + j] = (byte)((b0 << 1) | b1);
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Array.Reverse(img, i * 8, 8);
                    }
                    break;

                case 3:
                    bp0 = new byte[8];
                    bp1 = new byte[8];
                    byte[] bp2 = new byte[8];
                    img = new byte[64];
                    for (int i = 0; i < 16; i++)
                    {
                        if (i % 2 == 1) bp0[i / 2] = data[i];
                        if (i % 2 != 1) bp1[i / 2] = data[i];
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        bp2[i] = data[i + 16];
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            byte b0 = (byte)((bp0[i] & (1 << j)) >> j);
                            byte b1 = (byte)((bp1[i] & (1 << j)) >> j);
                            byte b2 = (byte)((bp2[i] & (1 << j)) >> j);
                            img[8 * i + j] = (byte)((b2 << 2) | (b0 << 1) | b1);
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Array.Reverse(img, i * 8, 8);
                    }
                    break;

                case 4:
                    bp0 = new byte[8];
                    bp1 = new byte[8];
                    bp2 = new byte[8];
                    byte[] bp3 = new byte[8];
                    img = new byte[64];
                    for (int i = 0; i < data.Length / 2; i++)
                    {
                        if (i % 2 == 1)
                        {
                            bp0[i / 2] = data[i];
                            bp2[i / 2] = data[i + 16];
                        }
                        else
                        {
                            bp1[i / 2] = data[i];
                            bp3[i / 2] = data[i + 16];
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            byte b0 = (byte)((bp0[i] & (1 << j)) >> j);
                            byte b1 = (byte)((bp1[i] & (1 << j)) >> j);
                            byte b2 = (byte)((bp2[i] & (1 << j)) >> j);
                            byte b3 = (byte)((bp3[i] & (1 << j)) >> j);
                            img[8 * i + j] = (byte)((b2 << 3) | (b3 << 2) | (b0 << 1) | b1);
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Array.Reverse(img, i * 8, 8);
                    }
                    break;
                case 8:
                    throw new Exception("8BPP Is currently Unavailable");
                default:
                    throw new Exception("Invalid BPP parameter, please chose 2, 3, 4, or 8");
            }
        }
    }
}
