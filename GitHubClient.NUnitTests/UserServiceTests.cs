using GitHubClient.Models;
using GitHubClient.Services;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
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
        private IGithubApiService _gitHubService;
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
            services.AddHttpClient<IGithubApiService, GithubApiService>();
            services.AddSingleton<JsonSerializer>();
            services.AddSingleton<ILog, LogNLog>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddMemoryCache();
            services.AddRazorPages();

            var serviceProvider = services.BuildServiceProvider();
            _memCache = serviceProvider.GetService<IMemoryCache>();
            _userService = serviceProvider.GetService<IUserService>();
            _logger = serviceProvider.GetService<ILog>();
            _memCacheService = serviceProvider.GetService<IMemoryCacheService>();
            _gitHubService = serviceProvider.GetService<IGithubApiService>();
        }

        [Test]
        public async Task GetTop10Users() {
            _userService = new UserService(_gitHubService, _memCacheService, _configuration, _logger);

            var result = await _userService.GetList();

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Count);
            Assert.IsInstanceOf<List<UserCacheModel>>(result);
        }

        [Test]
        [TestCase("mojombo", "defunkt")]
        [TestCase("pjhyett", "ezmobius")]
        public async Task GetUsersWithParams(string login1, string login2)
        {
            _userService = new UserService(_gitHubService, _memCacheService, _configuration, _logger);

            var loginList = new List<string>();
            loginList.Add(login1);
            loginList.Add(login2);

            var result = await _userService.GetList(loginList);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsInstanceOf<List<UserCacheModel>>(result);
        }

        [Test]
        public async Task CallGithubApiEndpoint()
        {
            _userService = new UserService(_gitHubService, _memCacheService, _configuration, _logger);

            var result = await _gitHubService.GetList<UserDataModel>();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<UserDataModel>>(result);
        }

        [Test]
        [TestCase("mojombo")]
        [TestCase("pjhyett")]
        [TestCase("ezmobius")]
        public async Task CallGithubApiEndpointWithLogin(string login)
        {
            _userService = new UserService(_gitHubService, _memCacheService, _configuration, _logger);

            var result = await _gitHubService.GetSingle<UserDataDetailModel>(login);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserDataDetailModel>(result);
        }
    }
}
