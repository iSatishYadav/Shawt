using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Shawt.Providers.RateLimiting
{
    public class IdentityRateLimitConfiguration : RateLimitConfiguration
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityRateLimitConfiguration(
            IHttpContextAccessor httpContextAccessor,
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
            : base(ipOptions, clientOptions)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void RegisterResolvers()
        {
            ClientResolvers.Add(new IdentityUserResolveContributer(_httpContextAccessor));
            base.RegisterResolvers();
        }
    }
}
