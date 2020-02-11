using GitHubClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubClient.Interface
{
    public interface IUserService
    {
        Task<List<UserCacheModel>> GetAll(IList<string> logins = null);
        Task<List<UserDataModel>> GetApiUsers();
        Task<UserDataDetailModel> GetApiUserbyLogin(string login);
    }
}
