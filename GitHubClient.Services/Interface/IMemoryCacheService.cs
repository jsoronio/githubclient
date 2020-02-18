using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubClient.Services.Interface
{
    public interface IMemoryCacheService
    {
        void Set(string key, string value);
        void Set(string key, string value, int seconds);
        string Get(string key);
        bool CheckExists(string key);
    }
}
