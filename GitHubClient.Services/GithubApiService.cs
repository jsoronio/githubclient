using GitHubClient.Models;
using GitHubClient.Services.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GitHubClient.Services
{
    public class GithubApiService : IGithubApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _jsonSerializer;
        private readonly IConfiguration _configuration;
        private readonly ILog _logger;

        private string _endpoint = string.Empty;
        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;

        public GithubApiService(HttpClient httpClient, JsonSerializer jsonSerializer, IConfiguration configuration, ILog logger) 
        {
            _configuration = configuration;
            _logger = logger;
            _endpoint = _configuration["GitHub:UserEndPoint"];
            _clientId = _configuration["GitHub:ClientId"];
            _clientSecret = _configuration["GitHub:ClientSecret"];
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<List<T>> GetList<T>() where T : class
        {
            var apiUserList = new List<T>();

            try
            {
                _logger.Information("Sending GET Request to Github Api Endpoint");

                string githubApi = $"https://{_endpoint}/users?client_id={_clientId}&client_secret={_clientSecret}";

                _logger.Information($"[GET] {githubApi}");

                var request = CreateRequest(githubApi);
                var result = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                _logger.Information($"Response Code: {result.StatusCode}");

                using (var responseStream = await result.Content.ReadAsStreamAsync())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        _logger.Information("Github Api: Successful");

                        return _jsonSerializer.Deserialize<List<T>>(jsonTextReader);
                    }
                }
            }
            catch (WebException exception)
            {
                _logger.Error(exception, $"Github Api Request encountered an exception error - Status Code ({exception.Status.ToString()})");
            }

            return apiUserList;
        }

        public async Task<T> GetSingle<T>(string login) where T : new()
        {
            var userDetail = new T();

            try
            {
                _logger.Information("Sending GET Request to Github Api Endpoint");

                string githubApi = $"https://{_endpoint}/users/{login}?client_id={_clientId}&client_secret={_clientSecret}";

                _logger.Information($"[GET] {githubApi}");

                var request = CreateRequest(githubApi);
                var result = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                _logger.Information($"Response Code: {result.StatusCode}");

                using (var responseStream = await result.Content.ReadAsStreamAsync())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        _logger.Information("Github Api: Successful");

                        return _jsonSerializer.Deserialize<T>(jsonTextReader);
                    }
                }
            }
            catch (WebException exception)
            {
                _logger.Error(exception, $"Github Api Request encountered an exception error - Status Code ({exception.Status.ToString()})");
            }

            return userDetail;
        }

        private static HttpRequestMessage CreateRequest(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            request.Headers.Clear();
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "localhost");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Connection", "keep-alive");

            return request;
        }

        private static string FormatJsonString(string jsonString)
        {
            JToken jt = JToken.Parse(jsonString);
            return jt.ToString(Formatting.Indented);
        }
    }
}
