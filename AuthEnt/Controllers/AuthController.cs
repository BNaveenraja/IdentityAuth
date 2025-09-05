using AuthEnt.Models.Dto;
using AuthEnt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthEnt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ---------------------- REGISTER ----------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var origin = $"{Request.Scheme}://{Request.Host.Value}";
            var result = await _authService.RegisterAsync(dto, origin);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return StatusCode(201);
        }

        // ---------------------- LOGIN ----------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            try
            {
                var tokens = await _authService.LoginAsync(dto, ip);
                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // ---------------------- REFRESH TOKEN ----------------------
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            try
            {
                var tokens = await _authService.RefreshTokenAsync(dto.RefreshToken, ip);
                return Ok(tokens);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // ---------------------- REVOKE TOKEN ----------------------
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _authService.RevokeTokenAsync(dto.RefreshToken, ip);
            return NoContent();
        }

        // ---------------------- GET CURRENT USER ----------------------
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _authService.GetMeAsync(User);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }
}
