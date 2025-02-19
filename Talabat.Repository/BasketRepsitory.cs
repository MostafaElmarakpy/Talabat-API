//using StackExchange.Redis;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Talabat.Core.Entities;
//using Talabat.Core.Repositories;

//namespace Talabat.Repository
//{
//    public class BasketRepsitory : IBasketRepsitory
//    {
//        private readonly IDatabase _database;

//        public BasketRepsitory(IConnectionMultiplexer redis)
//        {
//            _database = redis.GetDatabase();
//        }

//        public Task<bool> DeleteBsketAcync(string basketId)
//        {
//            return _database.KeyDeleteAsync(basketId);
//        }

//        public async Task<CustomerBasket> GetBasketAcync(string basketId)
//        {
//            var basket = await _database.StringGetAsync(basketId);
//            return basket.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
//        }

//        public async Task<CustomerBasket> UpdateBsketAcync(CustomerBasket basket)
//        {
//            var createOrUpdate = await _database.StringSetAsync(basket.id, JsonSerializer.Serialize(basket),TimeSpan.FromDays(30));
//            if (!createOrUpdate) return null;
//            return await GetBasketAcync(basket.id);
//        }
//    }
//}

using System.Text.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.Repository;

public class BasketRepsoitory : IBasketRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<BasketRepsoitory> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public BasketRepsoitory(IConnectionMultiplexer redis, ILogger<BasketRepsoitory> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;

        // إعداد الخيارات المخصصة لـ JsonSerializer
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<CustomerBasket> GetBasketAsync(string basketId)
    {
        if (string.IsNullOrWhiteSpace(basketId))
        {
            _logger.LogWarning("Invalid basketId provided to GetBasketAsync.");
            return null;
        }

        var data = await _database.StringGetAsync(basketId);
        if (data.IsNullOrEmpty)
        {
            _logger.LogInformation($"Basket with id {basketId} not found.");
            return null;
        }

        return JsonSerializer.Deserialize<CustomerBasket>(data, _jsonOptions);
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        if (basket == null || string.IsNullOrWhiteSpace(basket.Id))
        {
            _logger.LogWarning("Invalid basket provided to UpdateBasketAsync.");
            return null;
        }

        var createdOrUpdated = await _database.StringSetAsync(
            basket.Id,
            JsonSerializer.Serialize(basket, _jsonOptions),
            TimeSpan.FromDays(30)
        );

        if (!createdOrUpdated)
        {
            _logger.LogError($"Failed to update basket with id {basket.Id}.");
            return null;
        }

        _logger.LogInformation($"Basket with id {basket.Id} updated successfully.");
        return await GetBasketAsync(basket.Id);
    }

    public async Task<bool> DeleteBasketAsync(string basketId)
    {
        if (string.IsNullOrWhiteSpace(basketId))
        {
            _logger.LogWarning("Invalid basketId provided to DeleteBasketAsync.");
            return false;
        }

        var deleted = await _database.KeyDeleteAsync(basketId);
        if (deleted)
        {
            _logger.LogInformation($"Basket with id {basketId} deleted successfully.");
        }
        else
        {
            _logger.LogWarning($"Basket with id {basketId} not found for deletion.");
        }

        return deleted;
    }



}

