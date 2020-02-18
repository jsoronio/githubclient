using GitHubClient.Services;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using System.Reflection;
using System.Threading;

namespace GitHubClient.NUnitTests
{
    [TestFixture]
    public class MemoryCacheServiceTests
    {
        private IMemoryCache _memCache;
        private IMemoryCacheService _memCacheService;
        private IConfiguration _configuration;

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
            services.AddScoped<IMemoryCacheService, MemoryCacheService>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddMemoryCache();
            services.AddRazorPages();

            var serviceProvider = services.BuildServiceProvider();
            _memCache = serviceProvider.GetService<IMemoryCache>();

            _memCacheService = new MemoryCacheService(_memCache, _configuration);
            _memCacheService.Set(_configuration["InMemoryCache:Key"], "mojombo;defunkt;pjhyett");
        }

        [Test]
        [TestCase("memcache_key", "memcache_value")]
        [TestCase("test_key", "test_value")]
        public void MemoryCacheShouldSetGivenKeyAndValue(string key, string value)
        {
            _memCacheService.Set(key, value);

            Assert.AreEqual(value, _memCacheService.Get(key));
        }

        [Test]
        public void MemoryCacheShouldFoundDefaultKey()
        {
            Assert.AreEqual(true, _memCacheService.CheckExists(_configuration["InMemoryCache:Key"]));
        }

        [Test]
        [TestCase("mojombo;defunkt;pjhyett")]
        public void MemoryCacheShouldStoredGivenValue(string value)
        {
            Assert.AreEqual(value, _memCacheService.Get(_configuration["InMemoryCache:Key"]));
        }

        [Test]
        [TestCase(5)]
        [TestCase(10)]
        public void MemoryCacheShouldExpireInGivenSeconds(int seconds)
        {
            _memCacheService.Set("testkey", "testvalue", seconds);

            Thread.Sleep((seconds + 2) * 1000);

            Assert.AreEqual(false, _memCacheService.CheckExists("testkey"));
        }
    }
}
