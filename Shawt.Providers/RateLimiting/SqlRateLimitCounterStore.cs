using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shawt.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shawt.Providers.RateLimiting
{
    public class SqlRateLimitCounterStore : IRateLimitCounterStore
    {
        private readonly IServiceProvider _serviceProvider;

        public SqlRateLimitCounterStore(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            // return Task.FromResult(_context.RateLimitCounters.AsNoTracking().Any(x => x.Id == id));
        }

        public async Task<RateLimitCounter?> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RateLimitingContext>();
            var counter = await context.RateLimitCounters.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (counter == null)
                return (null);
            return new RateLimitCounter
            {
                Count = counter.Count,
                Timestamp = counter.Timestamp
            };
        }

        public Task RemoveAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task SetAsync(string id, RateLimitCounter? entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RateLimitingContext>();
            var counter = new RateLimitCounters
            {
                Id = id,
                Count = entry.Value.Count,
                Timestamp = entry.Value.Timestamp
            };
            var existingCounter = await context.RateLimitCounters.FindAsync(id);
            if (existingCounter == null)
            {
                context.Add(counter);
            }
            else
            {
                context.Entry(existingCounter).CurrentValues.SetValues(counter);
            }
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException duce)
            {
                foreach (var item in duce.Entries)
                {
                    if (item.Entity is RateLimitCounters)
                    {
                        var proposedValues = item.CurrentValues;
                        var databaseValues = item.GetDatabaseValues();
                        foreach (var property in proposedValues.Properties)
                        {
                            var databaseValue = databaseValues?[property];
                            //DesignChoice: Used database values in case of conflict
                            //var proposedValue = proposedValues[property];
                            proposedValues[property] = databaseValue;
                        }
                        item.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
