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
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILog _logger;

        private string _key = string.Empty;
        private string _endpoint = string.Empty;
        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;
        private int _maxUsers = 0;

        public UserService(IMemoryCacheService cacheService, IConfiguration configuration, IHttpClientFactory clientFactory, ILog logger)
        {
            _cacheService = cacheService;
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
            _endpoint = _configuration["GitHub:UserEndPoint"];
            _clientId = _configuration["GitHub:ClientId"];
            _clientSecret = _configuration["GitHub:ClientSecret"];
            _key = _configuration["InMemoryCache:Key"];
            _maxUsers = Convert.ToInt32(_configuration["GitHub:MaxUsers"]);
        }

        public async Task<List<UserCacheModel>> GetAll(IList<string> logins = null)
        {
            if (logins == null)
            {
                _logger.Information($"Checking existing memory cache with key - '{_key}'");

                if (_cacheService.CheckExists(_key))
                {
                    _logger.Information($"Fetching Users from existing memory cache");

                    return await GetUsersFromMemoryCache();
                }
                else
                {
                    _logger.Information($"Fetching Users from Github's Api endpoint");

                    return await GetUsersFromApi();
                }
            }
            else
            {
                _logger.Information($"Fetching specified Users only from the exisiting Memory Cache or from the Github's Api endpoint");

                return await GetUsersFromEntries(logins);
            }
        }

        public async Task<List<UserDataModel>> GetApiUsers()
        {
            List<UserDataModel> apiUserList = new List<UserDataModel>();

            try
            {
                _logger.Information("Sending GET Request to Github Api Endpoint");

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://{_endpoint}/users?client_id={_clientId}&client_secret={_clientSecret}");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "localhost");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Connection", "keep-alive");

                _logger.Information($"[GET] https://{_endpoint}/users?client_id={_clientId}&client_secret={_clientSecret}");

                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("HTTP GET Request: Successful");

                    var jsonString = await response.Content.ReadAsStringAsync();

                    _logger.Information($"Response Code: {response.StatusCode}");
                    _logger.Information($"Data Received: {FormatJsonString(jsonString)}");

                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        apiUserList = JsonConvert.DeserializeObject<List<UserDataModel>>(jsonString);

                    }
                }
                else
                {
                    _logger.Error($"Github Api Request was unsuccessful - Status Code ({response.StatusCode})");
                }
            }
            catch (WebException exception)
            {
                _logger.Error(exception, $"Github Api Request encountered an exception error - Status Code ({exception.Status.ToString()})");
            }

            return apiUserList;
        }

        public async Task<UserDataDetailModel> GetApiUserbyLogin(string login)
        {
            UserDataDetailModel userDetail = new UserDataDetailModel();

            try
            {
                _logger.Information("Sending GET Request to Github Api Endpoint");

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://{_endpoint}/users/{login}?client_id={_clientId}&client_secret={_clientSecret}");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "localhost");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Connection", "keep-alive");

                _logger.Information($"[GET] https://{_endpoint}/users/{login}?client_id={_clientId}&client_secret={_clientSecret}");

                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Information("HTTP GET Request: Successful");

                    var jsonString = await response.Content.ReadAsStringAsync();

                    _logger.Information($"Response Code: {response.StatusCode}");
                    _logger.Information($"Data Received: {FormatJsonString(jsonString)}");

                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        userDetail = JsonConvert.DeserializeObject<UserDataDetailModel>(jsonString);
                    }
                }
                else
                {
                    _logger.Error($"Github Api Request was unsuccessful - Status Code ({response.StatusCode})");
                }
            }
            catch (WebException exception)
            {
                _logger.Error(exception, $"Github Api Request encountered an exception error - Status Code ({exception.Status.ToString()})");
            }

            return userDetail;
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

        private string FormatJsonString(string jsonString)
        {
            JToken jt = JToken.Parse(jsonString);
            return jt.ToString(Formatting.Indented);
        }

        private async Task<List<UserCacheModel>> GetUsersFromApi()
        {
            var userCacheList = new List<UserCacheModel>();
            var userList = new List<UserDataModel>();
            var counter = 0;

            userList = await GetApiUsers();

            var loginList = string.Empty;
            loginList = String.Join(";", userList.Select(m => m.login).OrderBy(m => m).ToList());

            _cacheService.Set(_key, loginList);

            foreach (var login in loginList.Split(";"))
            {
                if (counter < _maxUsers)
                {
                    var userDetail = await GetApiUserbyLogin(login);

                    if (!string.IsNullOrEmpty(userDetail.name))
                    {
                        var userCache = UserCacheMapping(userDetail);

                        _cacheService.Set(login, JsonConvert.SerializeObject(userCache));

                        userCacheList.Add(userCache);
                        counter++;
                    }
                }
                else
                {
                    break;
                }
            }

            return userCacheList.OrderBy(m => m.name).ToList();
        }

        private async Task<List<UserCacheModel>> GetUsersFromMemoryCache()
        {
            var userCacheList = new List<UserCacheModel>();
            var loginList = _cacheService.Get(_key);

            if (!string.IsNullOrEmpty(loginList))
            {
                userCacheList = await GetUsersFromSource(loginList.Split(";").ToList());
            }

            return userCacheList.OrderBy(m => m.name).ToList();
        }

        private async Task<List<UserCacheModel>> GetUsersFromEntries(IList<string> logins)
        {
            var userCacheList = new List<UserCacheModel>();

            userCacheList = await GetUsersFromSource(logins.ToList());

            return userCacheList.OrderBy(m => m.name).ToList();
        }

        private async Task<List<UserCacheModel>> GetUsersFromSource(List<string> source)
        {
            var userCacheList = new List<UserCacheModel>();
            var userList = new List<UserDataModel>();
            var counter = 0;

            foreach (var login in source)
            {
                if (counter < _maxUsers)
                {
                    var userCache = new UserCacheModel();

                    if (_cacheService.CheckExists(login))
                    {
                        var cacheObject = _cacheService.Get(login);
                        if (!string.IsNullOrEmpty(cacheObject))
                        {
                            userCache = JsonConvert.DeserializeObject<UserCacheModel>(cacheObject);

                            if (!string.IsNullOrEmpty(userCache.name))
                            {
                                userCacheList.Add(userCache);

                                counter++;
                            }
                        }
                        else
                        {
                            var userDetail = await GetApiUserbyLogin(login);
                            userCache = UserCacheMapping(userDetail);

                            if (!string.IsNullOrEmpty(userCache.name))
                            {
                                _cacheService.Set(login, JsonConvert.SerializeObject(userCache));

                                userCacheList.Add(userCache);

                                counter++;
                            }
                        }
                    }
                    else
                    {
                        var userDetail = await GetApiUserbyLogin(login);
                        userCache = UserCacheMapping(userDetail);

                        if (!string.IsNullOrEmpty(userCache.name))
                        {
                            _cacheService.Set(login, JsonConvert.SerializeObject(userCache));

                            userCacheList.Add(userCache);

                            counter++;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return userCacheList;
        }
    }
}
