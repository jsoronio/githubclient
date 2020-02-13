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
        private int _counter = 0;
        private int _maxUsers = 0;

        private List<UserCacheModel> _userList;

        public UserService(IGithubApiService githubService, IMemoryCacheService cacheService, IConfiguration configuration, ILog logger)
        {
            _cacheService = cacheService;
            _configuration = configuration;
            _logger = logger;
            _key = _configuration["InMemoryCache:Key"];
            _maxUsers = Convert.ToInt32(_configuration["GitHub:MaxUsers"]);
            _gitApiService = githubService;
            _userList = new List<UserCacheModel>();
            _counter = 0;
        }

        public async Task<List<UserCacheModel>> GetList()
        {
            _logger.Information($"Checking existing memory cache with key - '{_key}'");

            if (_cacheService.CheckExists(_key))
            {
                _logger.Information($"Fetching Users from existing memory cache");

                return await GetTopUsersFromMemCache();
            }
            else
            {
                _logger.Information($"Fetching Users from Github's Api endpoint");

                return await GetTopUsersFromGithubApi();
            }
        }

        public async Task<List<UserCacheModel>> GetList(IList<string> logins)
        {
            _logger.Information($"Fetching specified Users only from the exisiting Memory Cache or from the Github's Api endpoint");

            return await GetUserList(logins.ToList());
        }

        #region Private methods
        private async Task<List<UserCacheModel>> GetTopUsersFromMemCache()
        {
            var userCacheList = new List<UserCacheModel>();
            var loginList = _cacheService.Get(_key);

            if (!string.IsNullOrEmpty(loginList))
            {
                userCacheList = await GetUserList(loginList.Split(";").ToList());
            }

            return userCacheList;
        }

        private async Task<List<UserCacheModel>> GetTopUsersFromGithubApi()
        {
            var loginList = string.Empty;
            var userList = await _gitApiService.GetList<UserDataModel>();

            loginList = String.Join(";", userList.Select(m => m.login).ToList());

            _cacheService.Set(_key, loginList);

            return await GetUserList(userList.Select(m => m.login).ToList());
        }

        private async Task<List<UserCacheModel>> GetUserList(List<string> source)
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

            return new List<UserCacheModel>();
        }

        private UserCacheModel UserCacheMapping(UserDataDetailModel model)
        {
            var userCache = new UserCacheModel();
            userCache.company = model.company;
            userCache.login = model.login;
            userCache.name = model.name;
            userCache.numberOfFollowers = model.followers;
            userCache.numberOfPublicRepos = model.public_repos;

            return userCache;
        }

        private void GetSingleUserFromMemCache(string login) 
        {
            var cacheObject = _cacheService.Get(login);
            var userCache = JsonConvert.DeserializeObject<UserCacheModel>(cacheObject);

            if (!string.IsNullOrEmpty(userCache.name))
            {
                _userList.Add(userCache);
                _counter++;
            }
        }

        private async Task GetSingleUserFromGithubApi(string login)
        {
            var userDetail = await _gitApiService.GetSingle<UserDataDetailModel>(login);
            var userCache = UserCacheMapping(userDetail);

            if (!string.IsNullOrEmpty(userCache.name))
            {
                _cacheService.Set(login, JsonConvert.SerializeObject(userCache));
                _userList.Add(userCache);
                _counter++;
            }
        }
        #endregion
    }
}
