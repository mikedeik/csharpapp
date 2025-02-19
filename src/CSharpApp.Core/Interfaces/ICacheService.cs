using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Interfaces {
    public interface ICacheService {

        Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> getData, TimeSpan expiration);
        void Remove(string cacheKey);
    }
}
