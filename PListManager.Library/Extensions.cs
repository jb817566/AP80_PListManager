using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AP80_PListManager
{
    public static class Extensions
    {
        public static string SHA1Hash(this string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}