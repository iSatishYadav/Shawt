using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Shawt.Providers.RateLimiting;

public class IdentityRateLimitConfiguration(
    IHttpContextAccessor httpContextAccessor,
    IOptions<IpRateLimitOptions> ipOptions,
    IOptions<ClientRateLimitOptions> clientOptions) : RateLimitConfiguration(ipOptions, clientOptions)
{
    public override void RegisterResolvers()
    {
        ClientResolvers.Add(new IdentityUserResolveContributer(httpContextAccessor));
        base.RegisterResolvers();
    }
}
