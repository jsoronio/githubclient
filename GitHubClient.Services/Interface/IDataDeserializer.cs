using GitHubClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubClient.Services.Interface
{
    public interface IDataDeserializer
    {
        GithubUser DeserializeUser(JsonTextReader jsonTextReader);
    }
}
