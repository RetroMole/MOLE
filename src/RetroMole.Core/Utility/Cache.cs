using System.Collections;
using System.Security.Cryptography;

namespace RetroMole.Core.Utility
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

            using (var fs = File.OpenWrite($"{name}.bin"))
            {
                fs.WriteByte((byte)(((ushort)type >> 8) & 0xFF));
                fs.WriteByte((byte)((ushort)type & 0xFF));
                fs.Write(info);
                fs.Write(data);
            }

            SaveHash($"{name}.bin");
        }
        public static bool TryLoadCache(string name, out object data)
        {
            data = null;
            if (!File.Exists($"{name}.bin.sha1") || // Hash file missing
                !File.Exists($"{name}.bin") ||      // Cache file missing
                !CheckHash($"{name}.bin"))     // Hash mismatch
                return false;                             // Return if any of the above failed

            // If the above tests pass then load the cache and return
            data = LoadCache($"{name}.bin");
            return true;
        }
        public static byte[] LoadCache(string path) => File.ReadAllBytes(path);
        public static void SaveHash(string path) => File.WriteAllBytes($"{path}.sha1", HashAlgorithm.Create("SHA1").ComputeHash(File.ReadAllBytes(path)));
        public static bool CheckHash(string path) => StructuralComparisons.StructuralEqualityComparer.Equals(File.ReadAllBytes($"{path}.sha1"), HashAlgorithm.Create("SHA1").ComputeHash(File.ReadAllBytes(path)));
    }
}
