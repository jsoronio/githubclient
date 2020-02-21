using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubClient.Services.Interface
{
    public interface IMemoryCacheService
    {
        void Set(string key, object value);
        object Get(string key);
        bool CheckExists(string key);
    }
}
