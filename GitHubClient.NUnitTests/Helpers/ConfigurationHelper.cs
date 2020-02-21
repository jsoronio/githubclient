using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GitHubClient.NUnitTests.Helpers
{
    public class ConfigurationHelper
    {
        public static IConfiguration ConfigurationMock() 
        {
            var configuration = new Mock<IConfiguration>();

            configuration.SetupGet(m => m[It.Is<string>(s => s == "GitHub:MaxUsers")]).Returns("10");
            configuration.SetupGet(m => m[It.Is<string>(s => s == "InMemoryCache:ExpiresIn")]).Returns("120");
            configuration.SetupGet(m => m[It.Is<string>(s => s == "GitHub:UserEndPoint")]).Returns("api.github.com");
            configuration.SetupGet(m => m[It.Is<string>(s => s == "GitHub:ClientId")]).Returns("d27a75e5b06e26fd1b09");
            configuration.SetupGet(m => m[It.Is<string>(s => s == "GitHub:ClientSecret")]).Returns("898138aba841d66fe6c9918928ae332aa7032515");

            return configuration.Object;
        }
    }
}
