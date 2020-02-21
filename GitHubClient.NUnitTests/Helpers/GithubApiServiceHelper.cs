using GitHubClient.Models;
using GitHubClient.Services.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitHubClient.NUnitTests.Helpers
{
    public class GithubApiServiceHelper 
    {
        public static IGithubApiService GithubApiServiceMock()
        {
            var githubApi = new Mock<IGithubApiService>();
            var githubUserList = UsersHelper.UsersFake();

            var githubUser = new GithubUser();
            githubApi.Setup(s => s.GetSingle(It.IsAny<string>()))
            .Callback<string>(key =>
            {
                githubUser = new GithubUser();
                githubUser = githubUserList.Where(m => m.login == key).FirstOrDefault();
            })
            .ReturnsAsync(() => githubUser);

            return githubApi.Object;
        }
    }
}
