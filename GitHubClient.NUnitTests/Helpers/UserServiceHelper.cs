using GitHubClient.Models;
using GitHubClient.Services.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubClient.NUnitTests.Helpers
{
    public class UserServiceHelper
    {
        public static IUserService UserServiceMock()
        {
            var userService = new Mock<IUserService>();
            var result = new List<GithubUser>();
            var githubUserList = UsersHelper.UsersFake();

            userService.Setup(s => s.GetUserList(It.IsAny<string>()))
                .Callback<string>(logins =>
                {
                    var usernames = logins.Split(";").ToList();
                    result = githubUserList.Where(m => usernames.Contains(m.login)).ToList();
                })
                .ReturnsAsync(() => result);

            foreach (var user in githubUserList)
            {
                userService.Setup(s => s.GetUserFromCache(user.login)).Returns(user);
                userService.Setup(s => s.GetUserFromGithubApi(user.login)).ReturnsAsync(user);
            }

            return userService.Object;
        }
    }
}
