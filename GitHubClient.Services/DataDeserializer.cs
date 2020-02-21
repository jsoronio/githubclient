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
        public GithubUser DeserializeUser(JsonTextReader jsonTextReader)
        {
            GithubUser model = null;
            var currentPropertyName = string.Empty;

            while (jsonTextReader.Read())
            {
                var jsonValue = jsonTextReader.Value;
                if (jsonValue != null && jsonValue.Equals("Not Found"))
                {
                    model = null;
                    break;  
                }

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
                                model.followers = int.Parse(jsonTextReader.Value.ToString());
                                continue;
                            case "public_repos":
                                model.publicRepos = int.Parse(jsonTextReader.Value.ToString());
                                continue;
                        }
                        continue;
                }
            }

            return model;
        }
    }
}
