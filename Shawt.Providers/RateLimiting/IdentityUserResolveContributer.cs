using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;

namespace Shawt.Providers.RateLimiting;

public class IdentityUserResolveContributer : IClientResolveContributor
{
    public Task<string> ResolveClientAsync(HttpContext httpContext)
    {
        return Task.FromResult(httpContext.User.Identity.Name);
    }
}
