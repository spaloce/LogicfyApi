using LogicfyApi.Requests;
using LogicfyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.AdSoyad))
            {
                return BadRequest(new { message = "Email, şifre ve ad soyad gereklidir" });
            }

            var result = await _authService.RegisterAsync(request.Email, request.AdSoyad, request.Password);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            SetTokenCookie(result.Token);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email ve şifre gereklidir" });
            }

            var result = await _authService.LoginAsync(request.Email, request.Password);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            SetTokenCookie(result.Token);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _authService.LogoutAsync(userId);
            }

            Response.Cookies.Delete("token");
            return Ok(new { message = "Çıkış başarılı" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Token geçersiz" });
            }

            var result = await _authService.GetMeAsync(userId);
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpirationMinutes"]))
            };

            Response.Cookies.Append("token", token, cookieOptions);
        }
    }
}
