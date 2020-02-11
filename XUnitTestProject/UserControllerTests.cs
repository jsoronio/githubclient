using GitHubClient.Controllers;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject
{
    public class UserControllerTests
    {
        public UserControllerTests()
        {

        }

        [Fact]
        public async Task GetUsersAsync()
        {
            var mockMemoryCache = new Mock<IMemoryCache>();
            var mockConfig = new Mock<IConfiguration>();
            var mockUserService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UserController>>();
            var controller = new UserController(mockMemoryCache.Object, mockConfig.Object, mockUserService.Object, mockLogger.Object);

            var actionResult = await controller.Get();

            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetUsersWithInputAsync()
        {
            var mockMemoryCache = new Mock<IMemoryCache>();
            var mockConfig = new Mock<IConfiguration>();
            var mockUserService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UserController>>();
            var controller = new UserController(mockMemoryCache.Object, mockConfig.Object, mockUserService.Object, mockLogger.Object);

            var logins = new List<string>() { "bmizerany", "brynary" };
            var actionResult = await controller.Post(logins);

            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetUsersWithEmptyInputAsync()
        {
            var mockMemoryCache = new Mock<IMemoryCache>();
            var mockConfig = new Mock<IConfiguration>();
            var mockUserService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UserController>>();
            var controller = new UserController(mockMemoryCache.Object, mockConfig.Object, mockUserService.Object, mockLogger.Object);

            var actionResult = await controller.Post(null);

            Assert.IsType<OkObjectResult>(actionResult);
        }
    }
}
