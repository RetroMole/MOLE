using System.Linq;
using System;

namespace MOLE
{
    public static class BPP
    {
        public static byte[,] ToPlane(byte[] data)
        {
            byte[,] res = new byte[data.Length, 8];
            for (int r = 0; r < data.Length; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    res[r, c] = (byte)((data[r] & (1 << c)) == 0 ? 0 : 1);
                }
            }
            return res;
        }

        public static byte[] GetEveryNthElement(byte[] data, int n)
        {
            return data.Where((x, i) => ((i+1) % n) == 0).ToArray();
        }

        public static byte[,] GetPlaneIntertwined(byte[] data, int plane)
        {
            // TODO: This'll probably mess up with 3BPP and upward due to bitplanes being stored in pairs and due to not limiting the amount of elements to get 
            return plane switch
            {
                0 => ToPlane(GetEveryNthElement(data.Prepend<byte>(0).ToArray(), 2)),
                _ => ToPlane(GetEveryNthElement(data, plane + 1))
            };
        }
        public static byte[,] GetPlanePlanar(byte[] data, int plane)
        {
            return ToPlane(data.Skip(plane * 8).Take(8).ToArray());
        }

        public static byte[,] CombinePlanes(params byte[][,] planes)
        {
            byte[,] res = planes[0];
            for (int i = 1; i < planes.Length; i++)
            {
                var p = planes[i];
                for (int r = 0; r < p.GetLength(0); r++)
                {
                    for (int c = 0; c < p.GetLength(1); c++)
                    {
                        res[r, c] = (byte)((res[r,c] << i) | p[r, c]);
                    }
                }
            }
            return res;
        }

        public static byte[,] Test1bpp(byte[] data)
        {
            return ToPlane(data);
        }

        public static byte[,] Test2bppPlanar(byte[] data)
        {
            return CombinePlanes(GetPlaneIntertwined(data,0), GetPlaneIntertwined(data,1));
        }
    }
}
