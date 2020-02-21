using GitHubClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubClient.NUnitTests.Helpers
{
    public class UsersHelper
    {
        public static List<GithubUser> UsersFake()
        {
            var githubUserList = new List<GithubUser>()
            {
                new GithubUser()
                {
                    name = "user 1",
                    login = "user1",
                    company = "company 1",
                    publicRepos = 100,
                    followers = 250
                },
                new GithubUser()
                {
                    name = "user 2",
                    login = "user2",
                    company = "company 2",
                    publicRepos = 15,
                    followers = 60
                },
                new GithubUser()
                {
                    name = "user 3",
                    login = "user3",
                    company = "company 3",
                    publicRepos = 300,
                    followers = 1500
                },
                new GithubUser()
                {
                    name = "user 4",
                    login = "user4",
                    company = "company 4",
                    publicRepos = 120,
                    followers = 700
                },
                new GithubUser()
                {
                    name = "user 5",
                    login = "user5",
                    company = "company 5",
                    publicRepos = 20,
                    followers = 15
                },
                new GithubUser()
                {
                    name = "user 6",
                    login = "user6",
                    company = "company 6",
                    publicRepos = 8,
                    followers = 40
                },
                new GithubUser()
                {
                    name = "user 7",
                    login = "user7",
                    company = "company 7",
                    publicRepos = 18,
                    followers = 100
                },
                new GithubUser()
                {
                    name = "user 8",
                    login = "user8",
                    company = "company 8",
                    publicRepos = 8,
                    followers = 200
                },
                new GithubUser()
                {
                    name = "user 9",
                    login = "user9",
                    company = "company 9",
                    publicRepos = 8,
                    followers = 200
                },
                new GithubUser()
                {
                    name = "user 10",
                    login = "user10",
                    company = "company 10",
                    publicRepos = 8,
                    followers = 200
                },
                new GithubUser()
                {
                    name = "user 11",
                    login = "user11",
                    company = "company 11",
                    publicRepos = 8,
                    followers = 200
                },
                new GithubUser()
                {
                    name = "user 12",
                    login = "user12",
                    company = "company 12",
                    publicRepos = 8,
                    followers = 200
                }
            };

            return githubUserList;
        }

        public static object UserProfileFake()
        {
            var expectedContent = new
            {
                login = "mojombo",
                id = 1,
                node_id = "MDQ6VXNlcjE=",
                avatar_url = "https=//avatars0.githubusercontent.com/u/1?v=4",
                gravatar_id = "",
                url = "https=//api.github.com/users/mojombo",
                html_url = "https=//github.com/mojombo",
                followers_url = "https=//api.github.com/users/mojombo/followers",
                following_url = "https=//api.github.com/users/mojombo/following{/other_user}",
                gists_url = "https=//api.github.com/users/mojombo/gists{/gist_id}",
                starred_url = "https=//api.github.com/users/mojombo/starred{/owner}{/repo}",
                subscriptions_url = "https=//api.github.com/users/mojombo/subscriptions",
                organizations_url = "https=//api.github.com/users/mojombo/orgs",
                repos_url = "https=//api.github.com/users/mojombo/repos",
                events_url = "https=//api.github.com/users/mojombo/events{/privacy}",
                received_events_url = "https=//api.github.com/users/mojombo/received_events",
                type = "User",
                site_admin = false,
                name = "Tom Preston-Werner",
                company = "",
                blog = "http=//tom.preston-werner.com",
                location = "San Francisco",
                email = "",
                hireable = "",
                bio = "",
                public_repos = 61,
                public_gists = 62,
                followers = 21799,
                following = 11,
                created_at = "2007-10-20T05=24=19Z",
                updated_at = "2020-02-15T23=56=47Z"
            };

            return expectedContent;
        }
    }
}
