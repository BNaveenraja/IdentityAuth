using AuthEnt.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace AuthEnt.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequestDto dto, string origin);
        Task<TokenResponseDto> LoginAsync(LoginRequestDto dto, string ip);
        Task<TokenResponseDto> RefreshAsync(string token, string ip);
        Task RevokeRefreshTokenAsync(string token, string ip);
    }
}
