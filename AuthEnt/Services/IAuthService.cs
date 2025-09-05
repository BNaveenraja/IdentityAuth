using AuthEnt.Models.Dto;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthEnt.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequestDto dto, string origin);
        Task<TokenResponseDto> LoginAsync(LoginRequestDto dto, string ipAddress);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress);
        Task RevokeTokenAsync(string refreshToken, string ipAddress);
        Task<UserDto> GetMeAsync(ClaimsPrincipal userPrincipal);
    }
}
