using Microsoft.Extensions.Caching.Memory;
using System;

namespace Stardust.Flux.Server.Services
{
    public class TempStorage : ITempStorage
    {

        private IMemoryCache _cache;

        public TempStorage(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value, TimeSpan.FromSeconds(60));
        }
    }
}
