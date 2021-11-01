using System.IO;
using System.Security.Cryptography;

namespace Mole.Shared.Util
{
    public static partial class Cache
    {
        public enum Type
        {
            Graphics,
            PointerTable
        }
        public static void SaveCache(Type type, byte[] info, byte[] data, string name)
        {
            if (info is null) info = new byte[14];
            using (var fs = File.OpenWrite($"{name}.cache.bin"))
            {
                fs.WriteByte((byte)(((ushort)type >> 8) & 0xFF));
                fs.WriteByte((byte)((ushort)type & 0xFF));
                fs.Write(info);
                fs.Write(data);
            }
                SaveHash($"{name}.cache.bin.sha1");
        }
        public static bool TryLoadCache(string name, out object data)
        {
            bool hash = CheckHash($"{name}.cache.bin.sha1");
            if (hash) data = LoadCache($"{name}.cache.bin"); else data = null;
            return hash;
        }
        public static byte[] LoadCache(string path) => File.ReadAllBytes(path);
        public static void SaveHash(string path) => File.WriteAllBytes(path, new SHA1Managed().ComputeHash(File.ReadAllBytes(path)));
        public static bool CheckHash(string path) => File.ReadAllBytes(path) == new SHA1Managed().ComputeHash(File.ReadAllBytes(path));
    }
}
