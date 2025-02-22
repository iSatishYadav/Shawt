using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shawt.Data;
using Shawt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shawt.Providers
{
    public class LinksProvider : ILinksProvider
    {
        private readonly LinksContext _context;
        private readonly IShortUrlProvider _shortUrlProvider;

        public LinksProvider(LinksContext context, IShortUrlProvider shortUrlProvider)
        {
            _context = context;
            _shortUrlProvider = shortUrlProvider;
        }

        public int CreateLink(string originalLink, string userName)
        {
            var link = new Link
            {
                LinkId = Guid.NewGuid(),
                CreatedBy = userName,
                CreatedOn = DateTime.Now,
                OriginalLink = originalLink,
                Stats = Stats.ToJson(new Stats
                {
                    Clicks = 0
                })
            };
            _context.Link.Add(link);
            _context.SaveChanges();
            int id = link.Id;
            return id;
        }

        public string GetLink(int shortLink)
        {
            var link = _context.Link.Find(shortLink);
            if (link == null)
            {
                return null;
            }
            return link.OriginalLink;
        }

        public async Task<IList<LinkDto>> GetLinksAsync(string createdBy, int take, int skip, bool includeEntriesByApplications)
        {
            IQueryable<Link> linkQueryForUsers = includeEntriesByApplications
                ? _context.Link.Where(x => x.CreatedBy == createdBy || _context.ApplicationUsers.Where(a => a.UserName.Equals(createdBy)).Select(u => u.ApplicationId.ToString()).Contains(x.CreatedBy))
                : _context.Link.Where(x => x.CreatedBy == createdBy);
            var links = await linkQueryForUsers
                .OrderByDescending(x => x.Id)
                .Skip(skip)
                .Take(take)
                .Select(x => new LinkDto
                {
                    Id = x.LinkId,
                    CreatedOn = x.CreatedOn,
                    OriginalLink = x.OriginalLink,
                    ShortLink = _shortUrlProvider.Encode(x.Id),
                    Clicks = Stats.FromJson(x.Stats).Clicks
                })
                .ToListAsync();
            return links;
        }

        public async Task<LinkWithLogsDto> GetLinkWithLogsAsync(Guid linkId, string createdBy, bool includeEntriesByApplications)
        {
            IQueryable<Link> queryForLink = _context.Link.Where(x => x.LinkId == linkId);
            queryForLink = includeEntriesByApplications
                ? queryForLink.Where(x => x.CreatedBy == createdBy || _context.ApplicationUsers.Where(a => a.UserName.Equals(createdBy)).Select(u => u.ApplicationId.ToString()).Contains(x.CreatedBy))
                : queryForLink.Where(x => x.CreatedBy == createdBy);
            var links = await queryForLink
                .Select(x => new LinkWithLogsDto
                {
                    LinkId = x.LinkId,
                    Clicks = Stats.FromJson(x.Stats).Clicks,
                    CreatedOn = x.CreatedOn,
                    OriginalLink = x.OriginalLink,
                    ShortLink = _shortUrlProvider.Encode(x.Id),
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

        public string UpdateAccessStats(int id, string ipAddress, DateTime timestamp, string userAgent, string browser, string os, string device)
        {
            var idParmeter = new SqlParameter("@Id", id);
            _context.Database.ExecuteSqlRaw(@"EXEC [dbo].[UpdateStats] @Id, @IpAddress, @TimeStamp, @UserAgent, @Browser, @Os, @Device",
                idParmeter,
                new SqlParameter("@IpAddress", ipAddress),
                new SqlParameter("@TimeStamp", timestamp),
                new SqlParameter("@UserAgent", userAgent),
                new SqlParameter("@Browser", browser),
                new SqlParameter("@Os", os),
                new SqlParameter("@Device", device));
            return null;
        }
    }
}
