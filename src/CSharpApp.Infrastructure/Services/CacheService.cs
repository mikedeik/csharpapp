using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Infrastructure.Services {
    public class CacheService : ICacheService {

        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache) {
            _cache = cache;
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> getData, TimeSpan expiration) {
            if (_cache.TryGetValue(cacheKey, out T cachedValue)) {
                return cachedValue;
            }

            var data = await getData();
            _cache.Set(cacheKey, data);
            return data;

        }

        public void Remove(string cacheKey) {
            
            _cache.Remove(cacheKey);
        }
    }
}
