using GitHubClient.Services.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubClient.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly int _slideDuration;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public MemoryCacheService(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
            _slideDuration = Convert.ToInt32(_configuration["InMemoryCache:ExpiresIn"]);
        }

        public bool CheckExists(string key)
        {
            bool exists = false;

            string val;
            if (_cache.TryGetValue(key, out val))
            {
                exists = true;
            }

            return exists;
        }

        public string Get(string key)
        {
            string val;
            if (_cache.TryGetValue(key, out val))
            {
                return val;
            }

            return string.Empty;
        }

        public void Set(string key, string value)
        {
            _cache.Set(key, value, SetMemoryCacheExpiry(_slideDuration));
        }

        public void Set(string key, string value, int seconds)
        {
            _cache.Set(key, value, SetMemoryCacheExpiry(seconds));
        }

        private MemoryCacheEntryOptions SetMemoryCacheExpiry(int seconds) 
        {
            MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
            cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(seconds);
            cacheExpirationOptions.SlidingExpiration = TimeSpan.FromSeconds(seconds);

            return cacheExpirationOptions;
        }
    }
}
