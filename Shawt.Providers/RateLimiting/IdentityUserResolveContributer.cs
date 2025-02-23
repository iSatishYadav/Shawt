using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;

namespace Shawt.Providers.RateLimiting;

public class IdentityUserResolveContributer(IHttpContextAccessor httpContextAccessor) : IClientResolveContributor
{
    public Task<string> ResolveClientAsync(HttpContext httpContext)
    {
        return Task.FromResult(httpContextAccessor.HttpContext.User.Identity.Name);
    }
}
