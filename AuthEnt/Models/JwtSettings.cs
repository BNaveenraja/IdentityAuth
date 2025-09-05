namespace AuthEnt.Models
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int AccessTokenExpiresMinutes { get; set; }
        public int RefreshTokenExpiresDays { get; set; }
    }
}
