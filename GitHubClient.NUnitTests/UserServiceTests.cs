using GitHubClient.Models;
using GitHubClient.Services;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GitHubClient.NUnitTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private IDataDeserializer _dataDeserializer;
        private IMemoryCache _memCache;
        private IMemoryCacheService _memCacheService;
        private IConfiguration _configuration;
        private IUserService _userService;
        private IGithubApiService _gitHubService;
        private ILog _logger;
        private List<GithubUser> _result;

        [SetUp]
        public async Task Setup()
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
            services.AddHttpClient<IGithubApiService, GithubApiService>();
            services.AddSingleton<JsonSerializer>();
            services.AddSingleton<ILog, LogNLog>();
            services.AddSingleton<DataDeserializer>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddMemoryCache();
            services.AddRazorPages();

            var serviceProvider = services.BuildServiceProvider();
            _memCache = serviceProvider.GetService<IMemoryCache>();
            _userService = serviceProvider.GetService<IUserService>();
            _logger = serviceProvider.GetService<ILog>();
            _memCacheService = serviceProvider.GetService<IMemoryCacheService>();
            _gitHubService = serviceProvider.GetService<IGithubApiService>();
            _dataDeserializer = serviceProvider.GetService<DataDeserializer>();

            _userService = new UserService(_gitHubService, _memCacheService, _configuration, _logger);
            _result = await _userService.GetList();
        }

        [Test, Order(1)]
        public void GetInitialTop10Users() 
        {
            Assert.IsNotNull(_result);
            Assert.AreEqual(10, _result.Count);
        }

        [Test, Order(2)]
        public async Task GetInMemoryCacheUsers()
        {
            var result = await _userService.GetList();

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Count);
        }

        [Test, Order(3)]
        public void CheckMemCacheStored()
        {
            var logins = _memCacheService.Get(_configuration["InMemoryCache:Key"]);

            Assert.IsNotNull(logins);
            Assert.IsTrue(logins.Split(";").Any());
            Assert.IsTrue(logins.Split(";").ToList().Count() > 10);
        }

        [Test, Order(4)]
        public void CheckIndividualUserKeysInMemoryCache()
        {
            var logins = _memCacheService.Get(_configuration["InMemoryCache:Key"]);

            Assert.IsNotNull(logins);
            Assert.IsTrue(logins.Split(";").Any());
            Assert.IsTrue(logins.Split(";").ToList().Count() > 10);
                                                                               
            foreach (var login in logins.Split(";").ToList())
            {
                if (!string.IsNullOrEmpty(login))
                {
                    var user = _memCacheService.Get(login);

                    if (!string.IsNullOrEmpty(user)) 
                    {
                        Assert.IsNotNull(user);

                        var githubUser = JsonConvert.DeserializeObject<GithubUser>(user);

                        Assert.AreEqual(login, githubUser.login);
                    }
                }
            }
        }
    }
}
