using AuthEnt.Models.Dto;
using AuthEnt.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthEnt.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AuthDbContext _db;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, AuthDbContext db)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _db = db;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterRequestDto dto, string origin)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already in use." });
            }

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FullName = dto.FullName,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded) return createResult;

            // add default role
            await _userManager.AddToRoleAsync(user, "User");

            return IdentityResult.Success;
        }

        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto dto, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) throw new InvalidOperationException("Invalid credentials");

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) throw new InvalidOperationException("Invalid credentials");

            return await _tokenService.GenerateTokensAsync(user, ipAddress);
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress)
        {
            var tokenEntity = await _db.RefreshTokens.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.Expires < DateTime.UtcNow)
                throw new InvalidOperationException("Invalid refresh token");

            tokenEntity.IsRevoked = true;

            var newTokens = await _tokenService.GenerateTokensAsync(tokenEntity.User, ipAddress);
            tokenEntity.ReplacedByToken = newTokens.RefreshToken;
            await _db.SaveChangesAsync();

            return newTokens;
        }

        public async Task RevokeTokenAsync(string refreshToken, string ipAddress)
        {
            var tokenEntity = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (tokenEntity == null) return;
            tokenEntity.IsRevoked = true;
            await _db.SaveChangesAsync();
        }

        public async Task<UserDto> GetMeAsync(ClaimsPrincipal userPrincipal)
        {
            var uid = userPrincipal.FindFirstValue("uid") ?? userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uid == null) return null;
            var user = await _userManager.FindByIdAsync(uid);
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto(user.Id, user.Email, user.FullName, roles);
        }
    }
}
