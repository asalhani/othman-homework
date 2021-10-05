using System;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Models.Interfaces;
using Newtonsoft.Json;

namespace Models.Impl
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            var redisCustomerList = _cache.GetAsync(key).Result;
            if (redisCustomerList != null)
            {
                var serializedCustomerList = Encoding.UTF8.GetString(redisCustomerList);  
                var weatherList = JsonConvert.DeserializeObject<T>(serializedCustomerList);
                return weatherList;
            }

            return default;
        }

        public void Set<T>(string key, T value)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8),
                SlidingExpiration = TimeSpan.FromMinutes(1000)
            };

           var  serializedCustomerList = JsonConvert.SerializeObject(value);  
           var redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);  
            
           _cache.SetAsync(key, redisCustomerList, options).Wait();  
        }
    }
}