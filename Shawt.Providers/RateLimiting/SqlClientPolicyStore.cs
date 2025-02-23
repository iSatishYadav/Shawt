using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shawt.Data;

namespace Shawt.Providers.RateLimiting;

public class SqlClientPolicyStore(IServiceProvider serviceProvider, ClientRateLimitOptions options, ILogger<SqlClientPolicyStore> logger) : IClientPolicyStore
{
    public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ClientRateLimitPolicy> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        //This implementation assumes that rules are saved without the pre-fix
        //Default prefix is crlp_
        var clientId = id.Replace($"{options.ClientPolicyPrefix}_", string.Empty);
        logger.LogDebug("Getting Rate Limiting Policy for {clientId}", clientId);
        //var clientId = id.Replace("crlp_", string.Empty);
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RateLimitingContext>();
        Console.WriteLine("Getting Policy for Id: {id} using context: {context}", id, context.GetType().GetHashCode());
        var rules = await context.RateLimitRules.Where(x => x.ClientId == clientId).OrderByDescending(x => x.Id).ToListAsync(cancellationToken: cancellationToken);

        //var rules = await _context.RateLimitRules.Where(x => x.ClientId == clientId).OrderByDescending(x => x.Id).ToListAsync();
        //Client side GroupBy is not supported.
        //https://github.com/dotnet/efcore/issues/17068
        //Already filtered on Client Id, so there would be only 1 result
        var rule = rules.GroupBy(x => x.ClientId).SingleOrDefault();
        if (rule == null)
        {
            logger.LogDebug("No policies found for {clientId}", clientId);
            return (null);
        }
        logger.LogDebug("{count} policies found for {clientId}. {policy}", rule.Count(), clientId, string.Join(" | ", rule.Select(x => $"Endpoint: {x.Endpoint}, Limit: {x.RequestLimit}, Period: {x.Period}")));
        return
             new ClientRateLimitPolicy
             {
                 ClientId = rule.Key,
                 Rules = [.. rule.Select(r => new RateLimitRule
                 {
                     Endpoint = r.Endpoint,
                     Limit = r.RequestLimit,
                     Period = r.Period
                 })]
             };
    }

    public Task RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SeedAsync()
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(string id, ClientRateLimitPolicy entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
