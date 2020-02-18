using GitHubClient.Controllers;
using GitHubClient.Models;
using GitHubClient.Services;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class UserControllerTests 
    {
        private IDataDeserializer _dataDeserializer;
        private IMemoryCache _memCache;
        private IConfiguration _configuration;
        private IUserService _userService;
        private ILog _logger;
        private UserController _userController;

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
            services.AddSingleton<DataDeserializer>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddMemoryCache();
            services.AddRazorPages();

            var serviceProvider = services.BuildServiceProvider();
            _memCache = serviceProvider.GetService<IMemoryCache>();
            _userService = serviceProvider.GetService<IUserService>();
            _logger = serviceProvider.GetService<ILog>();
            _dataDeserializer = serviceProvider.GetService<DataDeserializer>();
        }

        [Test]
        public void CheckGithubConfig() 
        {
            Assert.IsNotNull(_configuration["GitHub:ClientId"], "ClientId");
            Assert.IsNotNull(_configuration["GitHub:ClientSecret"], "ClientSecret");
            Assert.IsNotNull(_configuration["GitHub:AuthAccess"], "AuthAccess");
            Assert.IsNotNull(_configuration["GitHub:MaxUsers"], "MaxUsers");
            Assert.IsNotNull(_configuration["GitHub:UserEndPoint"], "UserEndPoint");
        }

        [Test]
        public void CheckInMemoryCacheConfig()
        {
            Assert.IsNotNull(_configuration["InMemoryCache:Key"], "Key");
            Assert.IsNotNull(_configuration["InMemoryCache:ExpiresIn"], "ExpiresIn");
        }

        [Test]
        public async Task GetUsersAsync()
        {
            _userController = new UserController(_userService, _logger);

            var actionResult = await _userController.Get();
            var okResult = actionResult as OkObjectResult;
            var outputData = okResult.Value as List<GithubUser>;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(10, outputData.Count);
            Assert.AreEqual(200, okResult.StatusCode);
        }
    }
}