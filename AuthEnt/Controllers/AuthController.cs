
using AuthEnt.Models;
using AuthEnt.Models.Dto;
using AuthEnt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var origin = Request.Scheme + "://" + Request.Host.Value;
            var res = await _authService.RegisterAsync(dto, origin);
            if (!res.Succeeded) return BadRequest(res.Errors.Select(e => e.Description));
            return StatusCode(201);
        }

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

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] AuthEnt.Models.RefreshRequestDto dto)
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

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] AuthEnt.Models.RefreshRequestDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _authService.RevokeTokenAsync(dto.RefreshToken, ip);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var me = await _authService.GetMeAsync(User);
            return Ok(me);
        }
    }
}
