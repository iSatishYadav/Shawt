using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;

namespace Shawt.Providers.RateLimiting
{
    public class IdentityUserResolveContributer : IClientResolveContributor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentityUserResolveContributer(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            return Task.FromResult(_contextAccessor.HttpContext.User.Identity.Name);
        }
    }
}
