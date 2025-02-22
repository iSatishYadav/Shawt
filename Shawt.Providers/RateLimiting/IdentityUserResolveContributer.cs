using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shawt.Providers.RateLimiting
{
    public class IdentityUserResolveContributer : IClientResolveContributor
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentityUserResolveContributer(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }
        public string ResolveClient()
        {
            return _contextAccessor.HttpContext.User.Identity.Name;
        }        
    }
}
