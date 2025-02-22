using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shawt.Providers
{
    public interface IShortUrlProvider
    {
        string Encode(int number);
        int Decode(string encodedString);
    }
}
