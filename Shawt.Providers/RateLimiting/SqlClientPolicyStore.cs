using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shawt.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shawt.Providers.RateLimiting
{
    public class SqlClientPolicyStore : IClientPolicyStore
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ClientRateLimitOptions _options;
        private readonly ILogger<SqlClientPolicyStore> _logger;

        public SqlClientPolicyStore(IServiceProvider serviceProvider, ClientRateLimitOptions options, ILogger<SqlClientPolicyStore> logger)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _logger = logger;
        }
        public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ClientRateLimitPolicy> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            //This implementation assumes that rules are saved without the pre-fix
            //Default prefix is crlp_
            var clientId = id.Replace($"{_options.ClientPolicyPrefix}_", string.Empty);
            _logger.LogDebug("Getting Rate Limiting Policy for {0}", clientId);
            //var clientId = id.Replace("crlp_", string.Empty);
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RateLimitingContext>();
            Console.WriteLine("Getting Policy for Id: {0} using context: {1}", id, context.GetType().GetHashCode());
            var rules = await context.RateLimitRules.Where(x => x.ClientId == clientId).OrderByDescending(x => x.Id).ToListAsync();

            //var rules = await _context.RateLimitRules.Where(x => x.ClientId == clientId).OrderByDescending(x => x.Id).ToListAsync();
            //Client side GroupBy is not supported.
            //https://github.com/dotnet/efcore/issues/17068
            //Already filtered on Client Id, so there would be only 1 result
            var rule = rules.GroupBy(x => x.ClientId).SingleOrDefault();
            if (rule == null)
            {
                _logger.LogDebug("No policies found for {0}", clientId);
                return (null);
            }
            _logger.LogDebug("{0} policies found for {1}. {2}", rule.Count(), clientId, string.Join(" | ", rule.Select(x=> $"Endpoint: {x.Endpoint}, Limit: {x.Limit}, Period: {x.Period}")));
            return
                 new ClientRateLimitPolicy
                 {
                     ClientId = rule.Key,
                     Rules = rule.Select(r => new RateLimitRule
                     {
                         Endpoint = r.Endpoint,
                         Limit = r.Limit,
                         Period = r.Period
                     }).ToList()
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
}
