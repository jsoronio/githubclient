using System.Collections.Generic;
using System.Threading.Tasks;
using GitHubClient.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GitHubClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IMemoryCache cache, IConfiguration config, IUserService userService, ILogger<UserController> logger) 
        {
            _cache = cache;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            _logger.LogInformation("Receiving GET Request");

            return Ok(await _userService.GetAll());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(IList<string> logins)
        {
            _logger.LogInformation("Receiving POST Request");

            if (logins != null) {
                _logger.LogInformation($"Accepting POST Params - {JsonConvert.SerializeObject(logins)}");
            }

            return Ok(await _userService.GetAll(logins));
        }
    }
}