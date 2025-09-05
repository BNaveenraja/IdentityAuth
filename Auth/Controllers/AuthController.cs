using Microsoft.AspNetCore.Mvc;
using Auth.Services;
using Auth.Models;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            var result = await _authService.RegisterAsync(model);
            if (result.Contains("exists") || result.Contains("failed"))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var result = await _authService.LoginAsync(model);
            if (result == null)
                return Unauthorized("Invalid credentials");

            return Ok(result);
        }
    }
}
