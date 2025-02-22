using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shawt.Providers;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace Shawt.Providers.Tests
{
    [TestClass()]
    public class ShortUrlProviderTests
    {
        [TestMethod()]
        public void EncodeDecode_Should_ReturnOriginalValue_WhenCalledInChain()
        {
            //Run test random number of times, with a random number
            int numberOfTimes;
            numberOfTimes = 1000000;
            var shortUrlProvider = new ShortUrlProvider();
            for (int i = 0; i < numberOfTimes; i++)
            {
                var originalValue = new Random().Next(1, int.MaxValue);
                var encoded = shortUrlProvider.Encode(originalValue);
                var decoded = shortUrlProvider.Decode(encoded);
                Assert.AreEqual(originalValue, decoded);
            }
        }
    }
}