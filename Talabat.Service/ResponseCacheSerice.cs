using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Repositories;

namespace Talabat.Service
{
    public class ResponseCacheSerice : IResponseCacheSerice
    {
        private readonly IDatabase _database;

        public ResponseCacheSerice(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task CashResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null) return;// out from function
            var option = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var serializerResponse = JsonSerializer.Serialize(response, option);

            await _database.StringSetAsync(cacheKey, serializerResponse, timeToLive);
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(cacheKey);
            if (cachedResponse.IsNullOrEmpty) return null;
            return cachedResponse;
        }
    }
}
