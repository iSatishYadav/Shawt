using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shawt.Models;

namespace Shawt.Providers
{
    public interface ILinksProvider
    {
        int CreateLink(string originalLink, string userName);
        string GetLink(int shortLink);
        Task<IList<LinkDto>> GetLinksAsync(string createdBy, int take, int skip, bool includeEntriesByApplications);
        string UpdateAccessStats(int id, string ipAddress, DateTime timestamp, string userAgent, string browser, string os, string device);
        Task<LinkWithLogsDto> GetLinkWithLogsAsync(Guid linkId, string createdBy, bool includeEntriesByApplications);
    }
}
