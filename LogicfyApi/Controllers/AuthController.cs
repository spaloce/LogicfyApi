using LogicfyApi.Requests;
using LogicfyApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        private readonly IHttpContextAccessor _acc;

        public AuthController(IAuthService authService, IConfiguration configuration,IHttpContextAccessor acc)
        {
            _authService = authService;
            _configuration = configuration;
            _acc=acc;
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
            // JWT'yi HttpOnly Cookie olarak yaz
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                // https://localhost için de sorun yok
                SameSite = SameSiteMode.None, // cross-origin istek için şart
                Expires = DateTime.UtcNow.AddHours(12),
                Domain = null                 // localhost testi için DOMAIN YOK
            };

            _acc.HttpContext!.Response.Cookies.Append("logicfy_token", result.Token, cookieOptions);


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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMe()
        {
            // Sadece test için:
            // var debugClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Buraya düşüyorsak → token doğrulanmamıştır
                return Unauthorized(new { message = "Token geçersiz" });
            }

            var result = await _authService.GetMeAsync(userId);
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("me-debug")]
        public IActionResult DebugMe()
        {
            var cookies = Request.Cookies;
            return Ok(cookies);
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpirationMinutes"]))
            };

            Response.Cookies.Append("token", token, cookieOptions);
        }
    }
}
