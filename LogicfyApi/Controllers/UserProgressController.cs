using LogicfyApi.Data;
using LogicfyApi.DTOs;
using LogicfyApi.Models;
using LogicfyApi.Requests;
using LogicfyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogicfyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UserProgressController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Kullanici> _userManager;

        public UserProgressController(AppDbContext context, UserManager<Kullanici> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        // Yardımcı metod: Query parametresinden veya authenticated kullanıcıdan userId al
        private string GetTargetUserId(string requestedUserId = null)
        {
            // Eğer query parametresinden userId gelmişse ve admin yetkisi varsa onu kullan
            if (!string.IsNullOrEmpty(requestedUserId) && User.IsInRole("Admin"))
            {
                return requestedUserId;
            }

            // Aksi takdirde authenticated kullanıcının ID'sini döndür
            return GetUserId();
        }

        // ===== DERS İLERLEMELERİ =====

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users
                .Select(u => new {
                    u.Id,
                    u.AdSoyad,
                    u.Email,
                    u.Seviye,
                    u.XP
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("ders-ilerleme")]
        public IActionResult GetDersIlerleme([FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var ilerleme = _context.KullaniciDersIlerlemeleri
                .Where(x => x.KullaniciId == targetUserId)
                .Select(x => new KullaniciDersIlerlemeDto
                {
                    Id = x.Id,
                    DersId = x.DersId,
                    TamamlananSoruSayisi = x.TamamlananSoruSayisi,
                    ToplamSoruSayisi = x.ToplamSoruSayisi,
                    IlerlemeOrani = x.IlerlemeOrani,
                    TamamlandiMi = x.TamamlandiMi
                })
                .ToList();

            return Ok(ilerleme);
        }

        [HttpGet("ders-ilerleme/{dersId}")]
        public IActionResult GetDersIlerlemeById(int dersId, [FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var ilerleme = _context.KullaniciDersIlerlemeleri
                .FirstOrDefault(x => x.KullaniciId == targetUserId && x.DersId == dersId);

            if (ilerleme == null)
            {
                return NotFound(new { message = "İlerleme bulunamadı" });
            }

            return Ok(new KullaniciDersIlerlemeDto
            {
                Id = ilerleme.Id,
                DersId = ilerleme.DersId,
                TamamlananSoruSayisi = ilerleme.TamamlananSoruSayisi,
                ToplamSoruSayisi = ilerleme.ToplamSoruSayisi,
                IlerlemeOrani = ilerleme.IlerlemeOrani,
                TamamlandiMi = ilerleme.TamamlandiMi
            });
        }

        // ===== SORU CEVAPLAR =====

        [HttpGet("soru-cevap")]
        public IActionResult GetSoruCevaplar([FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var cevaplar = _context.KullaniciSoruCevaplari
                .Where(x => x.KullaniciId == targetUserId)
                .Select(x => new KullaniciSoruCevapDto
                {
                    Id = x.Id,
                    SoruId = x.SoruId,
                    DogruMu = x.DogruMu,
                    CevapJson = x.CevapJson,
                    SureMs = x.SureMs,
                    CreatedAt = x.CreatedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Ok(cevaplar);
        }

        [HttpGet("soru-cevap/{soruId}")]
        public IActionResult GetSoruCevap(int soruId, [FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var cevap = _context.KullaniciSoruCevaplari
                .FirstOrDefault(x => x.KullaniciId == targetUserId && x.SoruId == soruId);

            if (cevap == null)
            {
                return NotFound(new { message = "Cevap bulunamadı" });
            }

            return Ok(new KullaniciSoruCevapDto
            {
                Id = cevap.Id,
                SoruId = cevap.SoruId,
                DogruMu = cevap.DogruMu,
                CevapJson = cevap.CevapJson,
                SureMs = cevap.SureMs,
                CreatedAt = cevap.CreatedAt
            });
        }

        // Diğer metodları da aynı şekilde güncelleyin...
        // [HttpGet("unite-ilerleme")]
        // [HttpGet("kisim-ilerleme")]
        // [HttpGet("xp-log")]
        // [HttpGet("xp-istatistik")]

        // Yukarıdaki gibi tüm GET metodlarına [FromQuery] string userId = null parametresi ekleyin
        // ve targetUserId'yi kullanın

        // ===== ÜNİTE İLERLEMESİ =====

        [HttpGet("unite-ilerleme")]
        public IActionResult GetUniteIlerleme([FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var ilerleme = _context.KullaniciUniteIlerlemeleri
                .Where(x => x.KullaniciId == targetUserId)
                .Select(x => new KullaniciUnitProgressDto
                {
                    Id = x.Id,
                    UniteId = x.UniteId,
                    TamamlananDersSayisi = x.TamamlananDersSayisi,
                    ToplamDersSayisi = x.ToplamDersSayisi,
                    IlerlemeOrani = x.IlerlemeOrani
                })
                .ToList();

            return Ok(ilerleme);
        }

        [HttpGet("unite-ilerleme/{uniteId}")]
        public IActionResult GetUniteIlerlemeById(int uniteId, [FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var ilerleme = _context.KullaniciUniteIlerlemeleri
                .FirstOrDefault(x => x.KullaniciId == targetUserId && x.UniteId == uniteId);

            if (ilerleme == null)
            {
                return NotFound(new { message = "Ünite ilerleme bulunamadı" });
            }

            return Ok(new KullaniciUnitProgressDto
            {
                Id = ilerleme.Id,
                UniteId = ilerleme.UniteId,
                TamamlananDersSayisi = ilerleme.TamamlananDersSayisi,
                ToplamDersSayisi = ilerleme.ToplamDersSayisi,
                IlerlemeOrani = ilerleme.IlerlemeOrani
            });
        }

        // ===== KISIM İLERLEMESİ =====

        [HttpGet("kisim-ilerleme")]
        public IActionResult GetKisimIlerleme([FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var ilerleme = _context.KullaniciKisimIlerlemeleri
                .Where(x => x.KullaniciId == targetUserId)
                .Select(x => new KullaniciKisimProgressDto
                {
                    Id = x.Id,
                    KisimId = x.KisimId,
                    TamamlananDersSayisi = x.TamamlananDersSayisi,
                    ToplamDersSayisi = x.ToplamDersSayisi,
                    IlerlemeOrani = x.IlerlemeOrani
                })
                .ToList();

            return Ok(ilerleme);
        }

        [HttpGet("kisim-ilerleme/{kisimId}")]
        public IActionResult GetKisimIlerlemeById(int kisimId, [FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var ilerleme = _context.KullaniciKisimIlerlemeleri
                .FirstOrDefault(x => x.KullaniciId == targetUserId && x.KisimId == kisimId);

            if (ilerleme == null)
            {
                return NotFound(new { message = "Kısım ilerleme bulunamadı" });
            }

            return Ok(new KullaniciKisimProgressDto
            {
                Id = ilerleme.Id,
                KisimId = ilerleme.KisimId,
                TamamlananDersSayisi = ilerleme.TamamlananDersSayisi,
                ToplamDersSayisi = ilerleme.ToplamDersSayisi,
                IlerlemeOrani = ilerleme.IlerlemeOrani
            });
        }

        // ===== XP LOGLARI =====

        [HttpGet("xp-log")]
        public IActionResult GetXpLoglari([FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var logs = _context.KullaniciXpLoglari
                .Where(x => x.KullaniciId == targetUserId)
                .Select(x => new KullaniciXpLogDto
                {
                    Id = x.Id,
                    Kaynak = x.Kaynak,
                    Xp = x.Xp,
                    CreatedAt = x.CreatedAt
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Ok(logs);
        }

        [HttpGet("xp-istatistik")]
        public IActionResult GetXpIstatistik([FromQuery] string userId = null)
        {
            var targetUserId = GetTargetUserId(userId);

            var toplamXp = _context.KullaniciXpLoglari
                .Where(x => x.KullaniciId == targetUserId)
                .Sum(x => x.Xp);

            var gunlukXp = _context.KullaniciXpLoglari
                .Where(x => x.KullaniciId == targetUserId && x.CreatedAt.Date == DateTime.Today)
                .Sum(x => x.Xp);

            return Ok(new
            {
                toplamXp,
                gunlukXp
            });
        }

        // Diğer metodlar aynı kalabilir...

        // ===== YARDIMCI METOTLAR =====

        private async Task AddXpAsync(string userId, string kaynak, int xp)
        {
            var log = new KullaniciXpLog
            {
                KullaniciId = userId,
                Kaynak = kaynak,
                Xp = xp
            };

            _context.KullaniciXpLoglari.Add(log);

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.XP += xp;

                // Seviye hesaplaması (her 100 XP'de bir seviye)
                user.Seviye = (user.XP / 100) + 1;

                await _userManager.UpdateAsync(user);
            }

            await _context.SaveChangesAsync();
        }
    }
}