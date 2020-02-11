using GitHubClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitHubClient.Services.Interface
{
    public interface IUserService
    {
        Task<List<UserCacheModel>> GetAll(IList<string> logins = null);
        Task<List<UserDataModel>> GetApiUsers();
        Task<UserDataDetailModel> GetApiUserbyLogin(string login);
    }
}
