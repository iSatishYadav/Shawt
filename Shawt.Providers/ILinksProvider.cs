using Shawt.Data;
using Shawt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
