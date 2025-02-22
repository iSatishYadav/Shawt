using System.Linq;
using System.Text;

namespace Shawt.Providers
{
    public class ShortUrlProvider : IShortUrlProvider
    {
        private const string _alphabets = "23456789bcdfghjkmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ-_";
        private static readonly int _base = _alphabets.Length;
        public int Decode(string encodedString)
        {
            var num = 0;
            for (var i = 0; i < encodedString.Length; i++)
            {
                num = num * _base + _alphabets.IndexOf(encodedString.ElementAt(i));
            }
            return num;
        }

        public string Encode(int number)
        {
            var sb = new StringBuilder();
            while (number > 0)
            {
                sb.Insert(0, _alphabets.ElementAt(number % _base));
                number /= _base;
            }
            return sb.ToString();
        }
    }
}
