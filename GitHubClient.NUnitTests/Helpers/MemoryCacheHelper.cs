using GitHubClient.Models;
using GitHubClient.Services.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitHubClient.NUnitTests.Helpers
{
    public class MemoryCacheHelper
    {
        protected static List<GithubUser> userList;
        public static IMemoryCacheService MemoryCacheMock()
        {
            var memoryCache = new Mock<IMemoryCacheService>();
            userList = new List<GithubUser>();

            var flag = false;
            memoryCache.Setup(s => s.CheckExists(It.IsAny<string>()))
            .Callback<string>(key =>
            {
                flag = false;
                if (!string.IsNullOrEmpty(key))
                {
                    var ouput = userList.Where(o => o.login == key).ToList();
                    if (ouput != null && ouput.Any())
                    {
                        flag = true;
                    }
                }
            })
            .Returns(() => flag);

            memoryCache.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((key, value) =>
            {
                userList.Add(value as GithubUser);
            });

            var githubUser = new GithubUser();
            memoryCache.Setup(s => s.Get(It.IsAny<string>()))
            .Callback<string>(key =>
            {
                githubUser = new GithubUser();
                githubUser = userList.Where(m => m.login == key).FirstOrDefault();
            })
            .Returns(() => githubUser);

            return memoryCache.Object;
        }

    }
}
