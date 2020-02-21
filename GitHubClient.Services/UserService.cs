using GitHubClient.Models;
using GitHubClient.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GitHubClient.Services
{
    public class UserService : IUserService
    {
        private readonly IMemoryCacheService _cacheService;
        private readonly IConfiguration _configuration;
        private readonly IGithubApiService _gitApiService;

        private string _key = string.Empty;
        private int _maxUsers = 0;

        public UserService(IGithubApiService githubService, IMemoryCacheService cacheService, IConfiguration configuration)
        {
            _cacheService = cacheService;
            _configuration = configuration;
            _gitApiService = githubService;
            _maxUsers = Convert.ToInt32(_configuration["GitHub:MaxUsers"]);
        }

        public async Task<List<GithubUser>> GetUserList(string logins)
        {
            var userList = new List<GithubUser>();
            var counter = 0;

            if (!string.IsNullOrEmpty(logins))
            {
                var userLogins = logins.Split(";").ToList();

                foreach (var login in userLogins)
                {
                    var githubUser = new GithubUser();

                    if (_cacheService.CheckExists(login))
                        githubUser = GetUserFromCache(login);
                    else
                        githubUser = await _gitApiService.GetSingle(login);

                    if (githubUser != null && counter < _maxUsers)
                    {
                        counter++;
                        userList.Add(githubUser);

                        _cacheService.Set(login, githubUser);
                    }
                }
            }

            return userList;
        }

        public GithubUser GetUserFromCache(string login)
        {
            var user = _cacheService.Get(login);

            if (user != null)
                return user as GithubUser;
            else
                return null;
        }

        public async Task<GithubUser> GetUserFromGithubApi(string login)
        {
            var user = await _gitApiService.GetSingle(login);

            if (user != null)
                _cacheService.Set(login, user);
            else
                return null;
           
            return user;
        }
    }
}
