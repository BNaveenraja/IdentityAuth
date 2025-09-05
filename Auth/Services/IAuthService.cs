using Auth.Models;

namespace Auth.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequest model);
        Task<LoginResponse> LoginAsync(LoginRequest model);
    }

    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
