namespace Mole.Shared.Types
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
            get => SNESToRGB8(snes._pal[index]);
            set => snes._pal[index] = RGB8ToSNES(value);
        }

        /// <summary>
        /// big-bwain wrapper
        /// </summary>
        public class Pal_SNES
        {
            /// <summary>
            /// Internal Palette representation, an array of SNES 5-bit BGR colors
            /// </summary>
            public ushort[] _pal;

            /// <summary>
            /// SNES 5-bit BGR representation of the palette
            /// </summary>
            public ushort this[int index] => _pal[index];
        }
        /// <summary>
        /// SNES 5-bit BGR representation of the palette
        /// </summary>
        public Pal_SNES snes = new();

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="pal">Array of SNES 5-bit BGR colors</param>
        public Pal(ushort[] pal)
        {
            snes._pal = pal;
        }

        /// <summary>
        /// RGB Constructor
        /// </summary>
        /// <param name="pal">Array of 8-bit RGB colors</param>
        public Pal(uint[] pal)
        {
            snes._pal = new ushort[pal.Length];
            for (int i = 0; i < pal.Length; i++)
            {
                snes._pal[i] = RGB8ToSNES(pal[i]);
            }
        }

        /// <summary>
        /// Convert 8-bit RGB to the SNES' color format
        /// </summary>
        /// <param name="RGB">8-bit RGB value</param>
        /// <returns>SNES 5-bit BGR color</returns>
        public static ushort RGB8ToSNES(uint RGB)
        {
            uint r = (RGB >> 16) & 0xFF;
            uint g = (RGB >> 8) & 0xFF;
            uint b = RGB & 0xFF;

            return (ushort)((r >> 3) | ((g >> 3) << 5) | ((b >> 3) << 10));
        }

        /// <summary>
        /// Converts SNES colors to 8-bit RGB
        /// </summary>
        /// <param name="BGR">SNES 5-bit BGR value</param>
        /// <returns>8-bit RGB value</returns>
        public static uint SNESToRGB8(ushort BGR)
        {
            uint b = (uint)((BGR >> 10) & 0x1F) * 255 / 31;
            uint g = (uint)((BGR >> 5) & 0x1F) * 255 / 31;
            uint r = (uint)(BGR & 0x1F) * 255 / 31;

            return (r << 16) | (g << 8) | b;
        }
    }
}
