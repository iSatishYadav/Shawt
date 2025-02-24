using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shawt.Data;
using Shawt.Models;

namespace Shawt.Providers;

public class LinksProvider(LinksContext context, IShortUrlProvider shortUrlProvider) : ILinksProvider
{
    public int CreateLink(string originalLink, string userName)
    {
        var link = new Link
        {
            LinkId = Guid.NewGuid(),
            CreatedBy = userName,
            CreatedOn = DateTime.Now,
            OriginalLink = originalLink,
            Clicks = 0
        };
        context.Link.Add(link);
        context.SaveChanges();
        int id = link.Id;
        return id;
    }

    public string GetLink(int shortLink)
    {
        var link = context.Link.Find(shortLink);
        if (link == null)
        {
            return null;
        }
        return link.OriginalLink;
    }

    public async Task<IList<LinkDto>> GetLinksAsync(string createdBy, int take, int skip, bool includeEntriesByApplications)
    {
        IQueryable<Link> linkQueryForUsers = includeEntriesByApplications
            ? context.Link.Where(x => x.CreatedBy == createdBy || context.ApplicationUsers.Where(a => a.UserName.Equals(createdBy)).Select(u => u.ApplicationId.ToString()).Contains(x.CreatedBy))
            : context.Link.Where(x => x.CreatedBy == createdBy);
        var links = await linkQueryForUsers
            .OrderByDescending(x => x.Id)
            .Skip(skip)
            .Take(take)
            .Select(x => new LinkDto
            {
                Id = x.LinkId,
                CreatedOn = x.CreatedOn,
                OriginalLink = x.OriginalLink,
                ShortLink = shortUrlProvider.Encode(x.Id),
                Clicks = x.Clicks
            })
            .ToListAsync();
        return links;
    }

    public async Task<LinkWithLogsDto> GetLinkWithLogsAsync(Guid linkId, string createdBy, bool includeEntriesByApplications)
    {
        IQueryable<Link> queryForLink = context.Link.Where(x => x.LinkId == linkId);
        queryForLink = includeEntriesByApplications
            ? queryForLink.Where(x => x.CreatedBy == createdBy || context.ApplicationUsers.Where(a => a.UserName.Equals(createdBy)).Select(u => u.ApplicationId.ToString()).Contains(x.CreatedBy))
            : queryForLink.Where(x => x.CreatedBy == createdBy);
        var links = await queryForLink
            .Select(x => new LinkWithLogsDto
            {
                LinkId = x.LinkId,
                Clicks = x.Clicks,
                CreatedOn = x.CreatedOn,
                OriginalLink = x.OriginalLink,
                ShortLink = shortUrlProvider.Encode(x.Id),
                Logs = x.Log.Select(x => new LogDto
                {
                    Browser = x.Browser,
                    Device = x.Device,
                    IpAddress = x.IpAddress,
                    Os = x.Os,
                    Timestamp = x.Timestamp,
                    UserAgent = x.UserAgent
                })
            })
            .FirstOrDefaultAsync();
        return links;
    }

    public async Task UpdateAccessStats(int id, string ipAddress, DateTime timestamp, string userAgent, string browser, string os, string device)
    {
        context.Link.Find(id).Clicks++;
        context.Log.Add(new Data.Log
        {
            LinkId = id,
            IpAddress = ipAddress,
            Timestamp = timestamp,
            UserAgent = userAgent,
            Browser = browser,
            Os = os,
            Device = device
        });
        await context.SaveChangesAsync();           
    }
}
