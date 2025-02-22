using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shawt.Providers.RateLimiting
{
    public class IdentityRateLimitConfiguration : RateLimitConfiguration
    {
        public IdentityRateLimitConfiguration(
            IHttpContextAccessor httpContextAccessor,
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
            : base(httpContextAccessor, ipOptions, clientOptions) { }

        protected override void RegisterResolvers()
        {
            ClientResolvers.Add(new IdentityUserResolveContributer(HttpContextAccessor));
            base.RegisterResolvers();
        }
    }
}
