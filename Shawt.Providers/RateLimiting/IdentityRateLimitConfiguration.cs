using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace Shawt.Providers.RateLimiting;

public class IdentityRateLimitConfiguration(
    IOptions<IpRateLimitOptions> ipOptions,
    IOptions<ClientRateLimitOptions> clientOptions) : RateLimitConfiguration(ipOptions, clientOptions)
{
    public override void RegisterResolvers()
    {
        ClientResolvers.Add(new IdentityUserResolveContributer());
        base.RegisterResolvers();
    }
}
