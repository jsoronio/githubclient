using GitHubClient.Controllers;
using GitHubClient.Models;
using GitHubClient.Services;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubClient.NUnitTests
{
    [TestFixture]
    public class MemoryCacheServiceTests
    {
        private IMemoryCache _memCache;
        private IMemoryCacheService _memCacheService;
        private IConfiguration _configuration;
        private IHttpClientFactory _clientFactory;
        private ILog _logger;

        [SetUp]
        public void Setup()
        {
            var relativeTargetProjectParentDir = "";
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var contentRoot = TestHelper.GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("appsettings.json");

            _configuration = configurationBuilder.Build();

            var services = new ServiceCollection();
            services.AddSingleton(_configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<ILog, LogNLog>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddMemoryCache();
            services.AddHttpClient();
            services.AddRazorPages();

            var serviceProvider = services.BuildServiceProvider();
            _memCache = serviceProvider.GetService<IMemoryCache>();
            _logger = serviceProvider.GetService<ILog>();
            _clientFactory = serviceProvider.GetService<IHttpClientFactory>();
            _memCacheService = serviceProvider.GetService<IMemoryCacheService>();

            _memCacheService = new MemoryCacheService(_memCache, _configuration, _logger);
            _memCacheService.Set(_configuration["InMemoryCache:Key"], "mojombo;defunkt;pjhyett");
        }

        [Test]
        [TestCase("memcache_key", "memcache_value")]
        [TestCase("test_key", "test_value")]
        public void SetMemCache(string key, string value)
        {
            _memCacheService.Set(key, value);

            Assert.AreEqual(value, _memCacheService.Get(key));
        }

        [Test]
        public void CheckExistingMemCache()
        {
            Assert.AreEqual(true, _memCacheService.CheckExists(_configuration["InMemoryCache:Key"]));
        }

        [Test]
        [TestCase("mojombo;defunkt;pjhyett")]
        public void GetExistingMemCache(string value)
        {
            Assert.AreEqual(value, _memCacheService.Get(_configuration["InMemoryCache:Key"]));
        }

        [Test]
        [TestCase(5)]
        [TestCase(10)]
        public void CheckMemCacheExpiryInSeconds(int seconds)
        {
            _memCacheService.SlideDuration = seconds;
            _memCacheService.Set("testkey", "testvalue");

            Thread.Sleep((seconds + 2) * 1000);

            Assert.AreEqual(false, _memCacheService.CheckExists("testkey"));
        }
    }
}
