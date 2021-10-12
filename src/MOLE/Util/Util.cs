using System;

namespace MOLE
{
    public static class Util
    {
        public static uint b2uL(params byte[] bytes)
        {
            var b = new byte[4];
            Array.Copy(bytes, b, bytes.Length);
            return (uint)((b[3] << 24) | (b[2] << 16) | (b[1] << 8) | b[0]);
        }

        public static uint b2uB(params  byte[] bytes)
        {
            var b = new byte[4];
            Array.Copy(bytes, b, bytes.Length);
            return (uint)((b[0] << 24) | (b[1] << 16) | (b[2] << 8) | b[3]);
        }
    }
}
