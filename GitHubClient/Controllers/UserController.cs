using System.Collections.Generic;
using System.Threading.Tasks;
using GitHubClient.Services.Interface;
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
        private readonly IUserService _userService;
        private readonly ILog _logger;

        public UserController(IConfiguration config, IUserService userService, ILog logger) 
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            _logger.Information("Receiving GET Request");

            return Ok(await _userService.GetList());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(IList<string> logins)
        {
            _logger.Information("Receiving POST Request");

            if (logins != null) {
                _logger.Information($"Accepting POST Params - {JsonConvert.SerializeObject(logins)}");
            }

            return Ok(await _userService.GetList(logins));
        }
    }
}