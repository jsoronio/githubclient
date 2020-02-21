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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubClient.NUnitTests.Services
{
    [TestFixture]
    public class DataDeserializerTests
    {
        private DataDeserializer _dataDeserializer;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddSingleton<DataDeserializer>();

            var serviceProvider = services.BuildServiceProvider();
            _dataDeserializer = serviceProvider.GetService<DataDeserializer>();
        }

        [Test]
        public void Get_UserProfile_ReturnsObject()
        {
            var fakeUserProfile = UsersHelper.UserProfileFake();
            var fakeJsonData = JsonConvert.SerializeObject(fakeUserProfile);
            var reader = new JsonTextReader(new StringReader(fakeJsonData));
            var output = _dataDeserializer.DeserializeUser(reader);

            Assert.IsNotNull(output);
            Assert.IsInstanceOf<GithubUser>(output);
            Assert.AreEqual("Tom Preston-Werner", output.name);
            Assert.AreEqual("mojombo", output.login);
            Assert.AreEqual("", output.company);
            Assert.AreEqual(21799, output.followers);
            Assert.AreEqual(61, output.publicRepos);
        }
    }
}
