using AuthEnt.Models.Dto;
using System.Security.Claims;

namespace AuthEnt.Services
{
    public interface ITokenService
    {
        Task<TokenResponseDto> GenerateTokensAsync(ApplicationUser user, string ipAddress);
        ClaimsPrincipal ValidateAccessToken(string token, bool validateLifetime = true);
    }
}
