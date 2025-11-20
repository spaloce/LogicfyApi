using LogicfyApi.Data;
using LogicfyApi.DTOs;
using LogicfyApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LogicfyApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthService(UserManager<Kullanici> userManager, IConfiguration configuration, AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<AuthResponse> RegisterAsync(string email, string adSoyad, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Bu email zaten kayıtlı"
                };
            }

            // Roller yoksa otomatik oluştur
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            // Kullanıcı sayısını al
            var userCount = _userManager.Users.Count();

            // İlk kullanıcı admin olur
            var rol = userCount == 0 ? "Admin" : "User";

            var user = new Kullanici
            {
                UserName = email,
                Email = email,
                AdSoyad = adSoyad,
                KayitTarihi = DateTime.Now,
                Rol = rol,
                XP = 0,
                Seviye = 1,
                Streak = 0
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Rol ataması
            await _userManager.AddToRoleAsync(user, rol);

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Kayıt başarılı",
                User = MapToUserDto(user),
                Token = token
            };
        }



        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email veya şifre yanlış"
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, password);
            if (!result)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email veya şifre yanlış"
                };
            }

            user.SonGirisTarihi = DateTime.Now;
            await _userManager.UpdateAsync(user);

            var token = GenerateJwtToken(user);
            return new AuthResponse
            {
                Success = true,
                Message = "Giriş başarılı",
                User = MapToUserDto(user),
                Token = token
            };
        }

        public async Task<AuthResponse> GetMeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Kullanıcı bulunamadı"
                };
            }

            return new AuthResponse
            {
                Success = true,
                User = MapToUserDto(user)
            };
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Opsiyonel: Son giriş veya başka bilgi güncellenebilir
                await _userManager.UpdateAsync(user);
            }
        }

        private string GenerateJwtToken(Kullanici user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.AdSoyad),
                new Claim("Rol", user.Rol)
            }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpirationMinutes"])),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private UserDto MapToUserDto(Kullanici user)
        {
            var dersKayitlari = _context.KullaniciDersKayitlari
                .Where(x => x.KullaniciId == user.Id)
                .Select(x => new KullaniciDersKaydiDto
                {
                    Id = x.Id,
                    DersId = x.DersId,
                    AktifMi = x.AktifMi
                })
                .ToList();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                AdSoyad = user.AdSoyad,
                Rol = user.Rol,
                XP = user.XP,
                Seviye = user.Seviye,
                Streak = user.Streak,
                KayitTarihi = user.KayitTarihi,
                SonGirisTarihi = user.SonGirisTarihi,
                DersKayitlari = dersKayitlari
            };
        }
    }
}
