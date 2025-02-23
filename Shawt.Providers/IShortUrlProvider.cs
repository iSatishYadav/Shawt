namespace Shawt.Providers;

public interface IShortUrlProvider
{
    string Encode(int number);
    int Decode(string encodedString);
}
