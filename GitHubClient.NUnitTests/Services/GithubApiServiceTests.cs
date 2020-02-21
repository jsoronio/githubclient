using GitHubClient.Models;
using GitHubClient.NUnitTests.Helpers;
using GitHubClient.Services;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace GitHubClient.NUnitTests.Services
{
    [TestFixture]
    public class GithubApiServiceTests
    {
        private IConfiguration _configuration;
        private ILog _logger;
        private DataDeserializer _dataDeserializer;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<DataDeserializer>();
            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            _httpClient = new HttpClient();
            _dataDeserializer = serviceProvider.GetService<DataDeserializer>();
            _configuration = ConfigurationHelper.ConfigurationMock();
            _logger = new LogNLog();
        }

        [Test]
        public async Task Get_ExistingUser_ByLogin_ReturnObject()
        {
            var login = "mojombo";
            var githubApi = new GithubApiService(_httpClient, _dataDeserializer, _configuration, _logger);
            var output = await githubApi.GetSingle(login);

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<GithubUser>(output);
            Assert.AreEqual("Tom Preston-Werner", output.name);
            Assert.AreEqual("mojombo", output.login);
            Assert.AreEqual(null, output.company);
            Assert.AreEqual(21799, output.followers);
            Assert.AreEqual(61, output.publicRepos);
        }

        [Test]
        public async Task Get_NonExistingUser_ByLogin_ReturnNull()
        {
            var login = "randomzzzuser";
            var githubApi = new GithubApiService(_httpClient, _dataDeserializer, _configuration, _logger);
            var output = await githubApi.GetSingle(login);

            Assert.IsNull(output);
        }
    }
}
