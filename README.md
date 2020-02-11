## Web Api Guidelines
## Web Api Routes

The default routing for the web api is set to [GET] https://localhost:44363/api/user.
You can access directly the json data afte the project loads on the browser.                                                                       

```
    Base url: https://localhost:44363

    Submitting GET Request:
    [GET] User List: https://localhost:44363/api/user
        Output: [{
            "name": "Blake Mizerany",
            "login": "bmizerany",
            "company": null,
            "numberOfFollowers": 1250,
            "numberOfPublicRepos": 156
          }]
    
    Submitting POST Request with parameters:
    [POST] User List: https://localhost:44363/api/user
        Input: ["bmizerany"]
        Output: [{
            "name": "Blake Mizerany",
            "login": "bmizerany",
            "company": null,
            "numberOfFollowers": 1250,
            "numberOfPublicRepos": 156
          }]
```

For more convenient, you can access the swagger and access the available api on this link: https://localhost:44363/swagger
You can send request and test all the routes on the swagger directly. Specially when submitting a POST Request with parameters, the form body is already provided in json format.

Accessing Github's Api endpoint must provide Authorization Token to exceed beyond the maxmimum request limit per day. The Authorization token was generated from my personal github account. 
```
    [GET] User List Api: https://api.github.com/users
    [GET] User Detail Api: https://api.github.com/users/{username}

    Request Format:
      Authorization Token : 792cf3edd6a61d4c7b9cf90385ea9ca0753e9c30
      Request Headers:
        Accept : application/json
        User-Agent : localhost
        Cache-Control : no-cache
        Connection : keep-alive
        Authorization: Bearer 792cf3edd6a61d4c7b9cf90385ea9ca0753e9c30
```
