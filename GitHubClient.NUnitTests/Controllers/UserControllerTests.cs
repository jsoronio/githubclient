using GitHubClient.Controllers;
using GitHubClient.Models;
using GitHubClient.NUnitTests.Helpers;
using GitHubClient.Services;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GitHubClient.NUnitTests.Controllers
{
    [TestFixture]
    public class UserControllerTests 
    {
        private UserController _userController;
        private IUserService _userService;
        private ILog _logger;

        [SetUp]
        public void Setup()
        {
             _userService = UserServiceHelper.UserServiceMock();
             _logger = new LogNLog();
        }

        [Test]
        public async Task Get_WithInputExisting_ReturnList()
        {
            _userController = new UserController(_userService, _logger);

            var usernames = "user1;user2;user3";
            var actionResult = await _userController.Get(usernames);
            var okResult = actionResult as OkObjectResult;
            var outputData = okResult.Value as IList<GithubUser>;

            Assert.AreEqual(3, outputData.Count);
        }

        [Test]
        public async Task Get_WithInputNonExisting_ReturnList()
        {
            _userController = new UserController(_userService, _logger);

            var usernames = "user1;user2;user3;randomuserone;randomusertwo";
            var actionResult = await _userController.Get(usernames);
            var okResult = actionResult as OkObjectResult;
            var outputData = okResult.Value as IList<GithubUser>;

            Assert.AreEqual(3, outputData.Count);
        }

        [Test]
        public async Task Get_WithInputNonExisting_ReturnEmptyList()
        {
            _userController = new UserController(_userService, _logger);

            var usernames = "randomuserone;randomusertwo;randomuserthree";
            var actionResult = await _userController.Get(usernames);
            var okResult = actionResult as OkObjectResult;
            var outputData = okResult.Value as IList<GithubUser>;

            Assert.IsEmpty(outputData);
        }


        [Test]
        public async Task Get_WithSingleInput_ReturnList()
        {
            _userController = new UserController(_userService, _logger);

            var username = "user1";
            var actionResult = await _userController.Get(username);
            var okResult = actionResult as OkObjectResult;
            var outputData = okResult.Value as List<GithubUser>;
            var expectedObject = new GithubUser()
            {
                name = "user 1",
                login = "user1",
                company = "company 1",
                publicRepos = 100,
                followers = 250
            };
            var output = outputData.FirstOrDefault();

            Assert.AreEqual(expectedObject.name, output.name);
            Assert.AreEqual(expectedObject.login, output.login);
            Assert.AreEqual(expectedObject.company, output.company);
            Assert.AreEqual(expectedObject.publicRepos, output.publicRepos);
            Assert.AreEqual(expectedObject.followers, output.followers);
            Assert.AreEqual(1, outputData.Count);
        }

        [Test]
        public async Task Get_WithInputEmpty_ReturnBadRequest()
        {
            _userController = new UserController(_userService, _logger);

            var usernames = string.Empty;
            var actionResult = await _userController.Get(usernames);
            var badRequest = actionResult as BadRequestResult;

            Assert.IsInstanceOf<BadRequestResult>(badRequest);
        }
    }
}