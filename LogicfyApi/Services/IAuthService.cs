using LogicfyApi.DTOs;

namespace LogicfyApi.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(string email,string adsoyad, string password);
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<AuthResponse> GetMeAsync(string userId);
        Task LogoutAsync(string userId);
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
    
}
