using GitHubClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubClient.Interface
{
    public interface IMemoryCacheService
    {
        void Set(string key, string value);
        string Get(string key);
        bool CheckExists(string key);
    }
}
