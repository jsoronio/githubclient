using GitHubClient.Models;
using GitHubClient.Services.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitHubClient.Services
{
    public class DataDeserializer : IDataDeserializer
    {
        public List<string> DeserializeList(JsonTextReader jsonTextReader)
        {
            var list = new List<string>();
            var currentPropertyName = string.Empty;

            while (jsonTextReader.Read())
            {
                switch (jsonTextReader.TokenType)
                {
                    case JsonToken.StartObject:
                        continue;
                    case JsonToken.EndObject:
                        continue;
                    case JsonToken.PropertyName:
                        currentPropertyName = jsonTextReader.Value.ToString();
                        continue;
                    case JsonToken.String:
                        switch (currentPropertyName)
                        {
                            case "login":
                                list.Add(jsonTextReader.Value.ToString());
                                continue;
                        }
                        continue;
                    case JsonToken.Integer:
                        continue;
                }
            }

            return list;
        }

        public GithubUser DeserializeUser(JsonTextReader jsonTextReader)
        {
            var model = new GithubUser();
            var currentPropertyName = string.Empty;

            while (jsonTextReader.Read())
            {
                switch (jsonTextReader.TokenType)
                {
                    case JsonToken.StartObject:
                        model = new GithubUser();
                        continue;
                    case JsonToken.EndObject:
                        break;
                    case JsonToken.PropertyName:
                        currentPropertyName = jsonTextReader.Value.ToString();
                        continue;
                    case JsonToken.String:
                        switch (currentPropertyName)
                        {
                            case "name":
                                model.name = jsonTextReader.Value.ToString();
                                continue;
                            case "login":
                                model.login = jsonTextReader.Value.ToString();
                                continue;
                            case "company":
                                model.company = jsonTextReader.Value.ToString();
                                continue;
                        }
                        continue;
                    case JsonToken.Integer:
                        switch (currentPropertyName)
                        {
                            case "followers":
                                model.numberOfFollowers = int.Parse(jsonTextReader.Value.ToString());
                                continue;
                            case "public_repos":
                                model.numberOfPublicRepos = int.Parse(jsonTextReader.Value.ToString());
                                continue;
                        }
                        continue;
                }
            }

            return model;
        }
    }
}
