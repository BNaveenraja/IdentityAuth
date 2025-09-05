using System;

namespace AuthEnt.Models.Dto
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpires { get; set; }

        public TokenResponseDto(string accessToken, string refreshToken, DateTime accessTokenExpires)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            AccessTokenExpires = accessTokenExpires;
        }
    }
}
