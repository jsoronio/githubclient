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
        private readonly ILog _logger;

        private string _key = string.Empty;
        private const int _keyExpiresIn = 240;
        private int _counter = 0;
        private int _maxUsers = 0;

        private List<GithubUser> _userList;

        public UserService(IGithubApiService githubService, IMemoryCacheService cacheService, IConfiguration configuration, ILog logger)
        {
            _cacheService = cacheService;
            _configuration = configuration;
            _logger = logger;
            _key = _configuration["InMemoryCache:Key"];
            _maxUsers = Convert.ToInt32(_configuration["GitHub:MaxUsers"]);
            _gitApiService = githubService;
            _userList = new List<GithubUser>();
            _counter = 0;
        }

        public async Task<List<GithubUser>> GetList()
        {
            _logger.Information($"Checking existing memory cache with key - '{_key}'");

            if (_cacheService.CheckExists(_key))
            {
                _logger.Information($"Fetching Users from existing memory cache");

                return await GetTopUsersFromMemoryCache();
            }
            else
            {
                _logger.Information($"Fetching Users from Github's Api endpoint");

                return await GetTopUsersFromGithubApi();
            }
        }

        #region Private methods
        private async Task<List<GithubUser>> GetTopUsersFromMemoryCache()
        {
            var userCacheList = new List<GithubUser>();
            var loginList = _cacheService.Get(_key);

            if (!string.IsNullOrEmpty(loginList))
            {
                userCacheList = await GetUserList(loginList.Split(";").ToList());
            }

            return userCacheList;
        }

        private async Task<List<GithubUser>> GetTopUsersFromGithubApi()
        {
            var logins = await _gitApiService.GetLogins();
            var users = logins.Split(";").ToList();

            _cacheService.Set(_key, logins, _keyExpiresIn);

            return await GetUserList(users.Select(m => m).ToList());
        }

        private async Task<List<GithubUser>> GetUserList(List<string> source)
        {
            if (source != null && source.Any())
            {
                foreach (var login in source)
                {
                    if (_counter < _maxUsers)
                    {
                        if (_cacheService.CheckExists(login))
                           GetSingleUserFromMemCache(login);
                        else
                           await GetSingleUserFromGithubApi(login);
                    }
                    else
                        break;
                }

                return _userList.OrderBy(m => m.name).ToList();
            }

            return new List<GithubUser>();
        }

        private void GetSingleUserFromMemCache(string login) 
        {
            var cacheObject = _cacheService.Get(login);
            var userCache = JsonConvert.DeserializeObject<GithubUser>(cacheObject);

            if (!string.IsNullOrEmpty(userCache.name))
            {
                _userList.Add(userCache);
                _counter++;
            }
        }

        private async Task GetSingleUserFromGithubApi(string login)
        {
            var user = await _gitApiService.GetSingle(login);

            if (!string.IsNullOrEmpty(user.name))
            {
                _cacheService.Set(login, JsonConvert.SerializeObject(user));
                _userList.Add(user);
                _counter++;
            }
        }
        #endregion
    }
}
