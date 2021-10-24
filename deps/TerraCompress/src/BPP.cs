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

        public static byte[,] FlipPlaneH(byte[,] plane)
        {
            int rows = plane.GetLength(0);
            int cols = plane.GetLength(1);

            for (int i = 0; i <= rows - 1; i++)
            {
                int j = 0;
                int k = cols - 1;
                while (j < k)
                {
                    byte temp = plane[i, j];
                    plane[i, j] = plane[i, k];
                    plane[i, k] = temp;
                    j++;
                    k--;
                }
            }
            return plane;
        }

        public static byte[,] FlipPlaneV(byte[,] plane)
        {
            int rows = plane.GetLength(0);
            int cols = plane.GetLength(1);

            for (int i = 0; i <= cols - 1; i++)
            {
                int j = 0;
                int k = rows - 1;
                while (j < k)
                {
                    byte temp = plane[j, i];
                    plane[j, i] = plane[k, i];
                    plane[k, i] = temp;
                    j++;
                    k--;
                }
            }
            return plane;
        }

        public static byte[,] Test1bpp(byte[] data)
        {
            return ToPlane(data);
        }

        public static byte[,] Test2bppPlanar(byte[] data, bool FlipH = true, bool FlipV = false, bool SwapPlanePairs = true)
        {
            var d1 = GetPlaneIntertwined(data, 1);
            var d0 = GetPlaneIntertwined(data, 0);
            if (FlipH)
            {
                d1 = FlipPlaneH(d1);
                d0 = FlipPlaneH(d0);
            }
            if (FlipV)
            {
                d1 = FlipPlaneV(d1);
                d0 = FlipPlaneV(d0);
            }
            return SwapPlanePairs ? CombinePlanes(d1, d0) : CombinePlanes(d0,d1);
        }
    }
}
