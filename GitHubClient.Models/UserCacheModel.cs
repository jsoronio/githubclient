using System;

namespace GitHubClient.Models
{
    public class UserCacheModel
    {
        public string name { get; set; }
        public string login { get; set; }
        public string company { get; set; }
        public int numberOfFollowers { get; set; }
        public int numberOfPublicRepos { get; set; }
    }
}
