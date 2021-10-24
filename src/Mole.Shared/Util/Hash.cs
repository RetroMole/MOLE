using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Mole.Shared.Util
{
    public static class Hash
    {
        public static string Sha1(string file) {
            var hash = new SHA1Managed().ComputeHash(File.ReadAllBytes(file));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}