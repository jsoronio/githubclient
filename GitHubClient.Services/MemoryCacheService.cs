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
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILog _logger;

        private string _key = string.Empty;
        private string _endpoint = string.Empty;

        public int SlideDuration { get; set; }

        public MemoryCacheService(IMemoryCache cache, IConfiguration configuration, ILog logger)
        {
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
            _endpoint = _configuration["GitHub:UserEndPoint"];
            _key = _configuration["InMemoryCache:Key"];
            SlideDuration = Convert.ToInt32(_configuration["InMemoryCache:ExpiresIn"]);
        }

        public bool CheckExists(string key)
        {
            bool exists = false;

            string val;
            if (_cache.TryGetValue(_key, out val))
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
            if (key != _key)
            {
                MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();  
                cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(SlideDuration);
                cacheExpirationOptions.SlidingExpiration = TimeSpan.FromSeconds(SlideDuration);
                cacheExpirationOptions.RegisterPostEvictionCallback(TimeoutCallback, this);
                //cacheExpirationOptions.Priority = CacheItemPriority.Normal;

                _cache.Set(key, value, cacheExpirationOptions);
            }
            else
            {
                _cache.Set(key, value);
            }
        }

        private static void TimeoutCallback(object key, object value, EvictionReason reason, object state)
        {

        }
    }
}
