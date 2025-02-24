using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shawt.Providers;
using UAParser;

namespace Shawt.Controllers
{
    [Route("r")]
    //[ApiController]
    [AllowAnonymous]
    public class RedirectController(ILinksProvider linksProvider,
        IShortUrlProvider shortUrlProvider,
        ILogger<RedirectController> logger) : ControllerBase
    {

        [Route("{url}", Name = "RedirectToLink")]
        public async Task<IActionResult> Get(string url)
        {
            int id = shortUrlProvider.Decode(url);
            logger.LogDebug("Getting link for {url}", url);
            string originalUrl = linksProvider.GetLink(id);
            if (string.IsNullOrEmpty(originalUrl))
            {
                logger.LogWarning("URL {url} not found", url);
                return NotFound();
            }
            else
            {
                logger.LogInformation("Original URL for {url} found. Here it is: {originalUrl}", url, originalUrl);
                string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                ipAddress = ipAddress == "::1" ? HttpContext.Connection.LocalIpAddress.ToString() : ipAddress;
                string userAgent = HttpContext.Request.Headers.UserAgent;
                (string browser, string os, string device) = GetUserAgentDetails(userAgent);
                await linksProvider.UpdateAccessStats(id, ipAddress, DateTime.Now, userAgent, browser, os, device);
                originalUrl = !originalUrl.StartsWith("HTTP", StringComparison.CurrentCultureIgnoreCase) ? $"http://{originalUrl}" : originalUrl; //DevSkim: ignore DS137138
                logger.LogDebug("Redirecting from {url} to {originalUrl}", url, originalUrl);
                return Redirect(originalUrl);
            }
        }

        private static (string, string, string) GetUserAgentDetails(string userAgent)
        {
            var uaParser = Parser.GetDefault();
            ClientInfo clientInfo = uaParser.Parse(userAgent);
            var browser = $"{clientInfo.UA.ToString()}";
            var os = clientInfo.OS.ToString();
            var device = clientInfo.Device.Family;
            return (browser, os, device);
        }
    }
}