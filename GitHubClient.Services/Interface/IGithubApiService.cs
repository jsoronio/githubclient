using GitHubClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitHubClient.Services.Interface
{
    public interface IGithubApiService
    {
        Task<string> GetLogins();
        Task<GithubUser> GetSingle(string login);
    }
}
