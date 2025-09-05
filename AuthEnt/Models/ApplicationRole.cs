using Microsoft.AspNetCore.Identity;
public class ApplicationRole : IdentityRole<int>
{
    // place for role metadata
    public string Description { get; set; }
}