using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shawt.Models;
using Shawt.Providers;

namespace Shawt.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LinksController(ILinksProvider linksProvider, IShortUrlProvider shortUrlProvider,
    IConfiguration configuration,
    ILogger<LinksController> logger) : ControllerBase
{
    private readonly string _shortUrlRequestSchemeTarget = configuration?["GeneratedShortUrls:Request:Scheme:To"];
    private readonly string _shortUrlRequestSchemeSource = configuration?["GeneratedShortUrls:Request:Scheme:From"];
    private readonly bool _shouldRemoveRoutePrefix = bool.TryParse(configuration?["GeneratedShortUrls:Prefix:ShouldRemovePrefix"], out bool should) && should;
    private readonly string _prefixToRemove = configuration?["GeneratedShortUrls:Prefix:PrefixToRemove"];

    [HttpGet]
    public async Task<IList<LinkDto>> GetAsync(int skip = 0, int take = 100)
    {
        var userName = User.Identity.Name;
        logger.LogDebug("Getting links for {userName}", userName);
        var links = await linksProvider.GetLinksAsync(userName, take, skip, true);
        logger.LogInformation("Returning {count} links for {userName}", links.Count, userName);
        return [.. links
            .Select(x => new LinkDto
            {
                Id = x.Id,
                Clicks = x.Clicks,
                CreatedOn = x.CreatedOn,
                OriginalLink = x.OriginalLink,
                //HACK: Because the application is hosted under vADC, it doesn't know that it's running in HTTPS, so Url.Link returns link with HTTP, not HTTPS. So replaced HTTP with HTTPS.
                //HACK: Due to conditional API Rate Limiting, /api prefix is added, so manually removed the prefix.
                ShortLink = (_shouldRemoveRoutePrefix && !string.IsNullOrEmpty(_prefixToRemove) ? $"{Url.Link("RedirectToLink", new { url = x.ShortLink })}".Replace(_prefixToRemove, "", StringComparison.InvariantCultureIgnoreCase) : $"{Url.Link("RedirectToLink", new { url = x.ShortLink })}").Replace($"{_shortUrlRequestSchemeSource}://", $"{_shortUrlRequestSchemeTarget}://", StringComparison.InvariantCultureIgnoreCase)
            })];
    }

    [HttpGet("{id:guid}")]
    public async Task<LinkWithLogsDto> GetDetailsAsync(Guid id)
    {
        var userName = User.Identity.Name;
        logger.LogDebug("Getting details of link {id}", id);
        var link = await linksProvider.GetLinkWithLogsAsync(id, userName, true);
        //HACK: Because the application is hosted under vADC, it doesn't know that it's running in HTTPS, so Url.Link returns link with HTTP, not HTTPS. So replaced HTTP with HTTPS.
        link.ShortLink = $"{Url.Link("RedirectToLink", new { url = link.ShortLink })}".Replace(_shortUrlRequestSchemeSource, _shortUrlRequestSchemeTarget, StringComparison.InvariantCultureIgnoreCase);
        //HACK: Due to conditional API Rate Limiting, /api prefix is added, so manually removed the prefix.
        if (_shouldRemoveRoutePrefix && !string.IsNullOrEmpty(_prefixToRemove))
            link.ShortLink = link.ShortLink.Replace(_prefixToRemove, "", StringComparison.InvariantCultureIgnoreCase);
        logger.LogInformation("Returting details of link Id {id}. Original Link {originalLink}. Short Link {shortLink}"
        , id, link.OriginalLink, link.ShortLink);
        return link;
    }

    [HttpPost]
    public IActionResult Post([FromBody] LongUrl longUrl)
    {
        if (ModelState.IsValid)
        {
            var userName = User.Identity.Name;
            logger.LogDebug("Creating short link for {userName} of URL {url}", userName, longUrl?.Url);
            string shortCode = shortUrlProvider.Encode(linksProvider.CreateLink(longUrl?.Url, userName));
            // Due to conditional Rate Limiting at /api, Url.Link prefixes /api to the generated URL.
            // Either find out a way to fix this, or remove /api manually
            var shortenedUrl = Url.Link("RedirectToLink", new { url = shortCode });
            if (!string.IsNullOrEmpty(_shortUrlRequestSchemeTarget) && !string.IsNullOrEmpty(_shortUrlRequestSchemeSource))
                shortenedUrl = shortenedUrl.Replace($"{_shortUrlRequestSchemeSource}://", $"{_shortUrlRequestSchemeTarget}://", StringComparison.InvariantCultureIgnoreCase);
            if (_shouldRemoveRoutePrefix && !string.IsNullOrEmpty(_prefixToRemove))
                shortenedUrl = shortenedUrl.Replace(_prefixToRemove, "", StringComparison.InvariantCultureIgnoreCase);
            logger.LogInformation("Short Link {shortenedUrl} created for {userName} for URL {url}", shortenedUrl, userName, longUrl?.Url);
            return Created(new Uri(shortenedUrl), new LinkDto { ShortLink = shortenedUrl });
        }
        else
        {
            logger.LogWarning("User {userName} entered an invalid URL {url}", User.Identity.Name, longUrl?.Url);
            ModelState.AddModelError("url", "Invalid URL");
            return BadRequest(ModelState);
        }
    }
}