using AuthEnt.Models;
using AuthEnt.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthEnt.Services
{ 
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IOptions<JwtSettings> options, AuthDbContext db, UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = options.Value;
            _db = db;
            _userManager = userManager;
        }

        public async Task<TokenResponseDto> GenerateTokensAsync(ApplicationUser user, string ipAddress)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("uid", user.Id.ToString())
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresDays),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserId = user.Id
            };

            _db.RefreshTokens.Add(refreshTokenEntity);
            await _db.SaveChangesAsync();

            return new TokenResponseDto(accessToken, refreshToken, expires);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal ValidateAccessToken(string token, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = validateLifetime
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, parameters, out var _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
