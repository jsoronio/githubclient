using System.Collections.Generic;
using System.Threading.Tasks;
using GitHubClient.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GitHubClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILog _logger;

        public UserController(IUserService userService, ILog logger) 
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string logins) 
        {
            _logger.Information("Receiving GET Request");

            if (!string.IsNullOrEmpty(logins))
                return Ok(await _userService.GetUserList(logins));
            else
                return BadRequest();
        }
    }
}