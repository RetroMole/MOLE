using System;
using System.Collections.Generic;
using System.Security;

namespace Mole.Util
{
    public static class Lzw
    {
        public static int CompressedSize { get; set; }

        public static int DeCompressedSize { get; set; }

        public static double Ratio => (double) CompressedSize / DeCompressedSize * 100.0;

        public static byte[] LzwCompress(this byte[] iBuf)
        {
            if (iBuf == null) throw new Exception("Input buffer is null.");
            if (iBuf.Length == 0) throw new Exception("Input buffer is empty.");
            DeCompressedSize = iBuf.Length;
            var dictionary = new Dictionary<List<byte>, int>(new ArrayComparer());
            for (var i = 0; i < 256; i++)
            {
                var e = new List<byte> {(byte) i};
                dictionary.Add(e, i);
            }

            var window = new List<byte>();
            var oBuf = new List<int>();
            foreach (var b in iBuf)
            {
                var windowChain = new List<byte>(window) {b};
                if (dictionary.ContainsKey(windowChain))
                {
                    window.Clear();
                    window.AddRange(windowChain);
                }
                else
                {
                    if (dictionary.ContainsKey(window))
                        oBuf.Add(dictionary[window]);
                    else
                        throw new Exception("Error Encoding.");
                    CompressedSize = oBuf.Count;
                    dictionary.Add(windowChain, dictionary.Count);
                    window.Clear();
                    window.Add(b);
                }
            }

            if (window.Count != 0)
            {
                oBuf.Add(dictionary[window]);
                CompressedSize = oBuf.Count;
            }

            return GetBytes(oBuf.ToArray());
        }

        public static byte[] LzwDecompress(this byte[] Bufi)
        {
            if (Bufi == null) throw new Exception("Input buffer is null.");
            if (Bufi.Length == 0) throw new Exception("Input buffer is empty.");
            var iBufi = Ia(Bufi);
            var iBuf = new List<int>(iBufi);
            CompressedSize = iBuf.Count;
            var dictionary = new Dictionary<int, List<byte>>();
            for (var i = 0; i < 256; i++)
            {
                var e = new List<byte> {(byte) i};
                dictionary.Add(i, e);
            }

            var window = dictionary[iBuf[0]];
            iBuf.RemoveAt(0);
            var oBuf = new List<byte>(window);
            foreach (var k in iBuf)
            {
                var entry = new List<byte>();
                if (dictionary.ContainsKey(k))
                    entry.AddRange(dictionary[k]);
                else if (k == dictionary.Count)
                    entry.AddRange(Add(window.ToArray(), new[] {window.ToArray()[0]}));
                if (entry.Count > 0)
                {
                    oBuf.AddRange(entry);
                    DeCompressedSize = oBuf.Count;
                    dictionary.Add(dictionary.Count, new List<byte>(Add(window.ToArray(), new[] {entry.ToArray()[0]})));
                    window = entry;
                }
            }

            return oBuf.ToArray();
        }

        private static byte[] GetBytes(int[] value)
        {
            if (value == null)
                throw new Exception("GetBytes (int[]) object cannot be null.");
            var numArray = new byte[value.Length * 4];
            Buffer.BlockCopy(value, 0, numArray, 0, numArray.Length);
            return numArray;
        }

        private static byte[] Add(byte[] left, byte[] right)
        {
            var l1 = left.Length;
            var l2 = right.Length;
            var ret = new byte[l1 + l2];
            Buffer.BlockCopy(left, 0, ret, 0, l1);
            Buffer.BlockCopy(right, 0, ret, l1, l2);
            return ret;
        }

        private static int[] Ia(byte[] ba)
        {
            var bal = ba.Length;
            var int32Count = bal / 4 + (bal % 4 == 0 ? 0 : 1);
            var arr = new int[int32Count];
            Buffer.BlockCopy(ba, 0, arr, 0, bal);
            return arr;
        }

        [SecuritySafeCritical]
        private static unsafe bool Compare(byte[] a1, byte[] a2)
        {
            if (a1 == null && a2 == null)
                return true;
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1 = a1, p2 = a2)
            {
                var Len = a1.Length;
                byte* x1 = p1, x2 = p2;
                while (Len > 7)
                {
                    if (*(long*) x2 != *(long*) x1)
                        return false;
                    x1 += 8;
                    x2 += 8;
                    Len -= 8;
                }

                switch (Len % 8)
                {
                    case 0:
                        break;
                    case 7:
                        if (*(int*) x2 != *(int*) x1)
                            return false;
                        x1 += 4;
                        x2 += 4;
                        if (*(short*) x2 != *(short*) x1)
                            return false;
                        x1 += 2;
                        x2 += 2;
                        if (*x2 != *x1)
                            return false;
                        break;
                    case 6:
                        if (*(int*) x2 != *(int*) x1)
                            return false;
                        x1 += 4;
                        x2 += 4;
                        if (*(short*) x2 != *(short*) x1)
                            return false;
                        break;
                    case 5:
                        if (*(int*) x2 != *(int*) x1)
                            return false;
                        x1 += 4;
                        x2 += 4;
                        if (*x2 != *x1)
                            return false;
                        break;
                    case 4:
                        if (*(int*) x2 != *(int*) x1)
                            return false;
                        break;
                    case 3:
                        if (*(short*) x2 != *(short*) x1)
                            return false;
                        x1 += 2;
                        x2 += 2;
                        if (*x2 != *x1)
                            return false;
                        break;
                    case 2:
                        if (*(short*) x2 != *(short*) x1)
                            return false;
                        break;
                    case 1:
                        if (*x2 != *x1)
                            return false;
                        break;
                }

                return true;
            }
        }

        private class ArrayComparer : IEqualityComparer<List<byte>>
        {
            public bool Equals(List<byte> left, List<byte> right)
            {
                if (left == null || right == null)
                    return false;
                return Compare(left.ToArray(), right.ToArray());
            }

            public unsafe int GetHashCode(List<byte> obj)
            {
                var obj1 = obj.ToArray();
                var cbSize = obj1.Length;
                var hash = 0x811C9DC5;
                fixed (byte* pb = obj1)
                {
                    var nb = pb;
                    while (cbSize >= 4)
                    {
                        hash ^= *(uint*) nb;
                        hash *= 0x1000193;
                        nb += 4;
                        cbSize -= 4;
                    }

                    switch (cbSize & 3)
                    {
                        case 3:
                            hash ^= *(uint*) (nb + 2);
                            hash *= 0x1000193;
                            goto case 2;
                        case 2:
                            hash ^= *(uint*) (nb + 1);
                            hash *= 0x1000193;
                            goto case 1;
                        case 1:
                            hash ^= *nb;
                            hash *= 0x1000193;
                            break;
                    }
                }

                return (int) hash;
            }
        }
    }
}