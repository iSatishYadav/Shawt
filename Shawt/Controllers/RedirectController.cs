using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shawt.Providers;
using UAParser;

namespace Shawt.Controllers
{
    //[Route("")]
    [ApiController]
    [AllowAnonymous]
    public class RedirectController : ControllerBase
    {
        private readonly HttpContext _httpContext;
        private readonly ILinksProvider _linkProvider;
        private readonly IShortUrlProvider _shortUrlProvider;
        private readonly ILogger<RedirectController> _logger;

        public RedirectController(ILinksProvider linksProvider,
            IShortUrlProvider shortUrlProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RedirectController> logger)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _linkProvider = linksProvider;
            _shortUrlProvider = shortUrlProvider;
            _logger = logger;
        }

        [Route("-{url}", Name = "RedirectToLink")]
        public IActionResult Get(string url)
        {
            int id = _shortUrlProvider.Decode(url);
            _logger.LogDebug("Getting link for {url}", url);
            string originalUrl = _linkProvider.GetLink(id);
            if (string.IsNullOrEmpty(originalUrl))
            {
                _logger.LogWarning("URL {url} not found", url);
                return NotFound();
            }
            else
            {
                _logger.LogInformation("Original URL for {url} found. Here it is: {originalUrl}", url, originalUrl);
                string ipAddress = _httpContext.Connection.RemoteIpAddress.ToString();
                ipAddress = ipAddress == "::1" ? _httpContext.Connection.LocalIpAddress.ToString() : ipAddress;
                string userAgent = _httpContext.Request.Headers["User-Agent"];
                (string browser, string os, string device) = GetUserAgentDetails(userAgent);
                _linkProvider.UpdateAccessStats(id, ipAddress, DateTime.Now, userAgent, browser, os, device);
                originalUrl = !originalUrl.ToUpper().StartsWith("HTTP") ? $"http://{originalUrl}" : originalUrl; //DevSkim: ignore DS137138
                _logger.LogDebug("Redirecting from {url} to {originalUrl}", url, originalUrl);
                return Redirect(originalUrl);
            }
        }

        private (string, string, string) GetUserAgentDetails(string userAgent)
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