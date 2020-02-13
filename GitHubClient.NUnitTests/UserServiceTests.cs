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
using System.Threading.Tasks;

namespace GitHubClient.NUnitTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private IMemoryCache _memCache;
        private IMemoryCacheService _memCacheService;
        private IConfiguration _configuration;
        private IUserService _userService;
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
            _userService = serviceProvider.GetService<IUserService>();
            _logger = serviceProvider.GetService<ILog>();
            _memCacheService = serviceProvider.GetService<IMemoryCacheService>();
        }

        [Test]
        public async Task GetTop10Users() {
            _userService = new UserService(_memCacheService, _configuration, _logger);

            var result = await _userService.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Count);
            Assert.IsInstanceOf<List<UserCacheModel>>(result);
        }

        [Test]
        [TestCase("mojombo", "defunkt")]
        [TestCase("pjhyett", "ezmobius")]
        public async Task GetUsersWithParams(string login1, string login2)
        {
            _userService = new UserService(_memCacheService, _configuration, _logger);

            var loginList = new List<string>();
            loginList.Add(login1);
            loginList.Add(login2);

            var result = await _userService.GetAll(loginList);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsInstanceOf<List<UserCacheModel>>(result);
        }

        [Test]
        public async Task CallGithubApiEndpoint()
        {
            _userService = new UserService(_memCacheService, _configuration, _logger);

            var result = await _userService.GetApiUsers();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<UserDataModel>>(result);
        }

        [Test]
        [TestCase("mojombo")]
        [TestCase("pjhyett")]
        [TestCase("ezmobius")]
        public async Task CallGithubApiEndpointWithLogin(string login)
        {
            _userService = new UserService(_memCacheService, _configuration, _logger);

            var result = await _userService.GetApiUserbyLogin(login);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserDataDetailModel>(result);
        }
    }
}
