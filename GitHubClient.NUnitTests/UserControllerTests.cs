using GitHubClient.Controllers;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubClient.NUnitTests
{
    [TestFixture]
    public class UserControllerTests
    {
        private IMemoryCache _memCache;
        private IConfiguration _config;
        private IUserService _users;
        private ILogger<UserController> _logger;

        [SetUp]
        public void Setup() {
            _memCache = new Mock<IMemoryCache>().Object;

            var myConfiguration = new Dictionary<string, string>
            {
                {"Key1", "Value1"},
                {"Nested:Key1", "NestedValue1"},
                {"Nested:Key2", "NestedValue2"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
        }

        [Test]
        public void CheckConfiguration() 
        {
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "default")]).Returns("mock value");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);
        }

        [Test]
        public async Task GetUsersAsync()
        {
            var mockMemoryCache = new Mock<IMemoryCache>();
            var mockConfig = new Mock<IConfiguration>();
            var mockUserService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UserController>>();
            var controller = new UserController(mockMemoryCache.Object, mockConfig.Object, mockUserService.Object, mockLogger.Object);

            var actionResult = await controller.Get();

            Assert.IsInstanceOf<OkObjectResult>(actionResult);
        }
    }
}