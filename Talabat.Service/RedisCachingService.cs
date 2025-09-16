using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Service;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.OrderSpecifications;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class RedisCachingService : ICachingService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCachingService> _logger;

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getter, TimeSpan? expiration = null)
        {
            var cachedValue = await _cache.GetAsync(key);
            if (cachedValue != null)
            {
                return JsonSerializer.Deserialize<T>(cachedValue);
            }

            var value = await getter();
            var serializedValue = JsonSerializer.SerializeToUtf8Bytes(value);

            await _cache.SetAsync(key, serializedValue, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            });

            return value;
        }

        public Task RemoveAsync(string key)
        {
            return _cache.RemoveAsync(key);
        }
    }
}