using Microsoft.AspNetCore.Identity;
public class ApplicationUser : IdentityUser<int>
{
    // enterprise-friendly extension points
    public string FullName { get; set; }
    public bool IsActive { get; set; } = true;
    public List<RefreshToken> RefreshTokens { get; set; }
}