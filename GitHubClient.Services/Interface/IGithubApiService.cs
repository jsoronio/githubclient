using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitHubClient.Services.Interface
{
    public interface IGithubApiService
    {
        Task<List<T>> GetList<T>() where T : class; 
        Task<T> GetSingle<T>(string login) where T : new();
    }
}
