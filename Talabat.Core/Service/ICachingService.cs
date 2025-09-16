using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Service
{
    public interface ICachingService
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getter, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
}
