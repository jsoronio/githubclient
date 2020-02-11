using GitHubClient.Controllers;
using GitHubClient.Interface;
using GitHubClient.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject
{
    public class MemoryCacheTests
    {
        public MemoryCacheTests() 
        {
            var services = new ServiceCollection();
        }

        [Theory]
        [InlineData("testKey", "testValue")]
        public void SetMemCacheKeyValue(string key, string value) 
        {
            var mockMemoryCache = new Mock<IMemoryCache>();
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<MemoryCacheService>>();
            var mockMemoryCacheService = new MemoryCacheService(mockMemoryCache.Object, mockConfig.Object, mockLogger.Object);

            mockMemoryCacheService.Set(key, value);

            Assert.Equal(mockMemoryCacheService.Get(key), value);
        } 
    }
}
