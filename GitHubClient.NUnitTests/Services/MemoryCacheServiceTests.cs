using GitHubClient.Models;
using GitHubClient.NUnitTests.Helpers;
using GitHubClient.Services;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using System.Reflection;
using System.Threading;

namespace GitHubClient.NUnitTests.Services
{
    [TestFixture]
    public class MemoryCacheServiceTests
    {
        private IMemoryCache _memCache;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();
            _memCache = serviceProvider.GetService<IMemoryCache>();

            _configuration = ConfigurationHelper.ConfigurationMock();
        }

        [Test]
        public void Check_NonExistingKey_ReturnsFalse()
        {
            var key = "randomkey";
            var memoryCache = new MemoryCacheService(_memCache, _configuration);
            var exists = memoryCache.CheckExists(key);

            Assert.IsFalse(exists);
        }

        [Test]
        public void Check_ExistingKey_ReturnsTrue()
        {
            var key = "randomkey";
            var value = "randomvalue";
            var memoryCache = new MemoryCacheService(_memCache, _configuration);

            memoryCache.Set(key, value);

            var exists = memoryCache.CheckExists(key);

            Assert.IsTrue(exists);
        }

        [Test]
        public void Set_Key_ReturnsObject()
        {
            var key = "randomkey";
            var value = new GithubUser()
            {
                login = "user1",
                name = "user 1",
                company = "company 1",
                followers = 100,
                publicRepos = 25
            };
            var memoryCache = new MemoryCacheService(_memCache, _configuration);

            memoryCache.Set(key, value);

            var output = memoryCache.Get(key);
            var userObject = output as GithubUser;

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<GithubUser>(output);
            Assert.AreEqual(value.name, userObject.name);
            Assert.AreEqual(value.login, userObject.login);
            Assert.AreEqual(value.company, userObject.company);
            Assert.AreEqual(value.publicRepos, userObject.publicRepos);
            Assert.AreEqual(value.followers, userObject.followers);
        }

        [Test]
        public void Set_OverrideCache_ReturnsObject()
        {
            var expectedValue = "test override value";
            var key = "randomkey";
            var value = new GithubUser()
            {
                login = "user1",
                name = "user 1",
                company = "company 1",
                followers = 100,
                publicRepos = 25
            };
            var memoryCache = new MemoryCacheService(_memCache, _configuration);

            memoryCache.Set(key, value);

            value.name = expectedValue;
            memoryCache.Set(key, value);

            var output = memoryCache.Get(key);
            var userObject = output as GithubUser;

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<GithubUser>(output);
            Assert.AreEqual(expectedValue, userObject.name);
        }

        [Test]
        public void Get_NonExistingKey_ReturnsNull()
        {
            var key = "randomkey";
            var value = new GithubUser()
            {
                login = "user1",
                name = "user 1",
                company = "company 1",
                followers = 100,
                publicRepos = 25
            };

            var memoryCache = new MemoryCacheService(_memCache, _configuration);
            var output = memoryCache.Get(key);

            Assert.IsNull(output);
        }

        [Test]
        public void Get_ExistingKey_ReturnsObject()
        {
            var key = "randomkey";
            var value = new GithubUser()
            {
                login = "user1",
                name = "user 1",
                company = "company 1",
                followers = 100,
                publicRepos = 25
            };
            var memoryCache = new MemoryCacheService(_memCache, _configuration);

            memoryCache.Set(key, value);
            var output = memoryCache.Get(key);

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<GithubUser>(output);
        }
    }
}
