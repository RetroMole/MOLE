﻿namespace Mole.Shared
{
    /// <summary>
    /// Palette class, holds information about a SNES Palette and allows for conversion to and from 8-bit RGB
    /// </summary>
    public class Pal
    {
        /// <summary>
        /// 8-bit RGB representation of the palette
        /// </summary>
        public uint this[int index]
        {
            get => SnesToRgb8(Snes.Pal[index]);
            set => Snes.Pal[index] = Rgb8ToSnes(value);
        }

        /// <summary>
        /// big-bwain wrapper
        /// </summary>
        public class PalSnes
        {
            /// <summary>
            /// Internal Palette representation, an array of SNES 5-bit BGR colors
            /// </summary>
            public ushort[] Pal;

            /// <summary>
            /// SNES 5-bit BGR representation of the palette
            /// </summary>
            public ushort this[int index] => Pal[index];
        }
        /// <summary>
        /// SNES 5-bit BGR representation of the palette
        /// </summary>
        public PalSnes Snes = new();

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="pal">Array of SNES 5-bit BGR colors</param>
        public Pal(ushort[] pal)
        {
            Snes.Pal = pal;
        }

        /// <summary>
        /// RGB Constructor
        /// </summary>
        /// <param name="pal">Array of 8-bit RGB colors</param>
        public Pal(uint[] pal)
        {
            Snes.Pal = new ushort[pal.Length];
            for (int i = 0; i < pal.Length; i++)
            {
                Snes.Pal[i] = Rgb8ToSnes(pal[i]);
            }
        }

        /// <summary>
        /// Convert 8-bit RGB to the SNES' color format
        /// </summary>
        /// <param name="rgb">8-bit RGB value</param>
        /// <returns>SNES 5-bit BGR color</returns>
        public static ushort Rgb8ToSnes(uint rgb)
        {
            uint r = (rgb >> 16) & 0xFF;
            uint g = (rgb >> 8) & 0xFF;
            uint b = rgb & 0xFF;

            return (ushort)((r >> 3) | ((g >> 3) << 5) | ((b >> 3) << 10));
        }

        /// <summary>
        /// Converts SNES colors to 8-bit RGB
        /// </summary>
        /// <param name="bgr">SNES 5-bit BGR value</param>
        /// <returns>8-bit RGB value</returns>
        public static uint SnesToRgb8(ushort bgr)
        {
            uint b = (uint)((bgr >> 10) & 0x1F) * 255 / 31;
            uint g = (uint)((bgr >> 5) & 0x1F) * 255 / 31;
            uint r = (uint)(bgr & 0x1F) * 255 / 31;

            return (r << 16) | (g << 8) | b;
        }
    }
}
