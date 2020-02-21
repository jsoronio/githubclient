using GitHubClient.Models;
using GitHubClient.NUnitTests.Helpers;
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

namespace GitHubClient.NUnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private IMemoryCacheService _memCacheService;
        private IConfiguration _configuration;
        private IGithubApiService _gitHubService;

        [SetUp]
        public void Setup()
        {
            _gitHubService = GithubApiServiceHelper.GithubApiServiceMock();
            _memCacheService = MemoryCacheHelper.MemoryCacheMock();
            _configuration = ConfigurationHelper.ConfigurationMock();
        }

        [Test]
        public async Task Get_UsersEmpty_ReturnsEmpty()
        {
            var logins = "";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var output = await userService.GetUserList(logins);

            Assert.IsEmpty(output);
            Assert.AreEqual(0, output.Count);
        }

        [Test]
        public async Task Get_CachedUsers_ReturnsList()
        {
            _memCacheService.Set("cachedUser1", new GithubUser()
            {
                name = "cached user 1",
                login = "cachedUser1",
                company = "company cache user 1",
                publicRepos = 10,
                followers = 55
            });

            _memCacheService.Set("cachedUser2", new GithubUser()
            {
                name = "cached user 2",
                login = "cachedUser2",
                company = "company cache user 2",
                publicRepos = 25,
                followers = 129
            });

            var logins = "cachedUser1;cachedUser2";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var output = await userService.GetUserList(logins);

            Assert.IsNotEmpty(output);
            Assert.AreEqual(2, output.Count);
        }

        [Test]
        public async Task Get_CachedAndNonExistingUsers_ReturnsList()
        {
            _memCacheService.Set("cachedUser1", new GithubUser()
            {
                name = "cached user 1",
                login = "cachedUser1",
                company = "company cache user 1",
                publicRepos = 10,
                followers = 55
            });

            _memCacheService.Set("cachedUser2", new GithubUser()
            {
                name = "cached user 2",
                login = "cachedUser2",
                company = "company cache user 2",
                publicRepos = 25,
                followers = 129
            });

            var logins = "cachedUser1;cachedUser2;randomuser1;randomuser2;randomuser3";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var output = await userService.GetUserList(logins);

            Assert.IsNotEmpty(output);
            Assert.AreEqual(2, output.Count);
        }

        [Test]
        public async Task Get_CachedAndExistingUsers_ReturnsList()
        {
            _memCacheService.Set("cachedUser1", new GithubUser()
            {
                name = "cached user 1",
                login = "cachedUser1",
                company = "company cache user 1",
                publicRepos = 10,
                followers = 55
            });

            _memCacheService.Set("cachedUser2", new GithubUser()
            {
                name = "cached user 2",
                login = "cachedUser2",
                company = "company cache user 2",
                publicRepos = 25,
                followers = 129
            });

            var logins = "cachedUser1;cachedUser2;user1;user2;user3";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var output = await userService.GetUserList(logins);

            Assert.IsNotEmpty(output);
            Assert.AreEqual(5, output.Count);
        }

        [Test]
        public async Task Get_ExistingUsers_ReturnsList()
        {
            var logins = "user1;user2;user3;user4;user5;user6;user7";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var output = await userService.GetUserList(logins);

            Assert.IsNotEmpty(output);
            Assert.AreEqual(7, output.Count);
        }

        [Test]
        public async Task Get_CachedUser_ReturnsObject()
        {
            var login = "user1";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var result = await userService.GetUserList(login);
            var output = userService.GetUserFromCache(login);

            Assert.IsNotEmpty(result);
            Assert.IsNotNull(output);
        }

        [Test]

        public async Task Get_NonExistingUsers_ReturnsEmpty()
        {
            var logins = "random1;random2;random3";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var output = await userService.GetUserList(logins);

            Assert.IsEmpty(output);
        }

        [Test]
        public async Task Get_ExistingAndNonExistingUser_ReturnsList()
        {
            var logins = "user1;user2;random1;random2";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var output = await userService.GetUserList(logins);

            Assert.AreEqual(2, output.Count);
        }

        [Test]
        public async Task Get_Top10_Users_ReturnsList()
        {
            var logins = "user1;user2;user3;user4;user5;user6;user7;user8;user10;user11;user11;user13";
            var userService = new UserService(_gitHubService, _memCacheService, _configuration);
            var output = await userService.GetUserList(logins);

            Assert.AreEqual(10, output.Count);
        }
    }
}
