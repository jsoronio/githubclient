using GitHubClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitHubClient.Services.Interface
{
    public interface IUserService
    {
        Task<List<GithubUser>> GetUserList(string logins);
        GithubUser GetUserFromCache(string login);
        Task<GithubUser> GetUserFromGithubApi(string login);
    }
}
