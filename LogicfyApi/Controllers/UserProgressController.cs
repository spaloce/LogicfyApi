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
    [Authorize]
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

        // ===== DERS İLERLEMELERİ =====

        [HttpGet("ders-ilerleme")]
        public IActionResult GetDersIlerleme()
        {
            var userId = GetUserId();
            var ilerleme = _context.KullaniciDersIlerlemeleri
                .Where(x => x.KullaniciId == userId)
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
        public IActionResult GetDersIlerlemeById(int dersId)
        {
            var userId = GetUserId();
            var ilerleme = _context.KullaniciDersIlerlemeleri
                .FirstOrDefault(x => x.KullaniciId == userId && x.DersId == dersId);

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
        public IActionResult GetSoruCevaplar()
        {
            var userId = GetUserId();
            var cevaplar = _context.KullaniciSoruCevaplari
                .Where(x => x.KullaniciId == userId)
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
        public IActionResult GetSoruCevap(int soruId)
        {
            var userId = GetUserId();
            var cevap = _context.KullaniciSoruCevaplari
                .FirstOrDefault(x => x.KullaniciId == userId && x.SoruId == soruId);

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

        [HttpPost("soru-cevap")]
        public async Task<IActionResult> CreateSoruCevap([FromBody] CreateSoruCevapRequest request)
        {
            var userId = GetUserId();

            var cevap = new KullaniciSoruCevap
            {
                KullaniciId = userId,
                SoruId = request.SoruId,
                DogruMu = request.DogruMu,
                CevapJson = request.CevapJson,
                SureMs = request.SureMs
            };

            _context.KullaniciSoruCevaplari.Add(cevap);
            await _context.SaveChangesAsync();

            // XP ekle
            if (request.DogruMu)
            {
                var xpMiktar = request.SureMs > 30000 ? 5 : 10;
                await AddXpAsync(userId, "SoruCevap", xpMiktar);
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

        // ===== XP LOGLARI =====

        [HttpGet("xp-log")]
        public IActionResult GetXpLoglari()
        {
            var userId = GetUserId();
            var logs = _context.KullaniciXpLoglari
                .Where(x => x.KullaniciId == userId)
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
        public IActionResult GetXpIstatistik()
        {
            var userId = GetUserId();
            var toplamXp = _context.KullaniciXpLoglari
                .Where(x => x.KullaniciId == userId)
                .Sum(x => x.Xp);

            var gunlukXp = _context.KullaniciXpLoglari
                .Where(x => x.KullaniciId == userId && x.CreatedAt.Date == DateTime.Today)
                .Sum(x => x.Xp);

            return Ok(new
            {
                toplamXp,
                gunlukXp
            });
        }

        // ===== ÜNİTE İLERLEMESİ =====

        [HttpGet("unite-ilerleme")]
        public IActionResult GetUniteIlerleme()
        {
            var userId = GetUserId();
            var ilerleme = _context.KullaniciUniteIlerlemeleri
                .Where(x => x.KullaniciId == userId)
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
        public IActionResult GetUniteIlerlemeById(int uniteId)
        {
            var userId = GetUserId();
            var ilerleme = _context.KullaniciUniteIlerlemeleri
                .FirstOrDefault(x => x.KullaniciId == userId && x.UniteId == uniteId);

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
        public IActionResult GetKisimIlerleme()
        {
            var userId = GetUserId();
            var ilerleme = _context.KullaniciKisimIlerlemeleri
                .Where(x => x.KullaniciId == userId)
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
        public IActionResult GetKisimIlerlemeById(int kisimId)
        {
            var userId = GetUserId();
            var ilerleme = _context.KullaniciKisimIlerlemeleri
                .FirstOrDefault(x => x.KullaniciId == userId && x.KisimId == kisimId);

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

        // ===== DERS KAYITLARI =====

        [HttpGet("ders-kayit")]
        public IActionResult GetDersKayitlari()
        {
            var userId = GetUserId();
            var kayitlar = _context.KullaniciDersKayitlari
                .Where(x => x.KullaniciId == userId)
                .Select(x => new KullaniciDersKaydiDto
                {
                    Id = x.Id,
                    DersId = x.DersId,
                    AktifMi = x.AktifMi
                })
                .ToList();

            return Ok(kayitlar);
        }

        [HttpPost("ders-kayit")]
        public async Task<IActionResult> CreateDersKayit([FromBody] CreateDersKayitRequest request)
        {
            var userId = GetUserId();

            var existing = _context.KullaniciDersKayitlari
                .FirstOrDefault(x => x.KullaniciId == userId && x.DersId == request.DersId);

            if (existing != null)
            {
                return BadRequest(new { message = "Bu derse zaten kayıtlısınız" });
            }

            var kayit = new KullaniciDersKaydi
            {
                KullaniciId = userId,
                DersId = request.DersId,
                AktifMi = true
            };

            _context.KullaniciDersKayitlari.Add(kayit);
            await _context.SaveChangesAsync();

            return Ok(new KullaniciDersKaydiDto
            {
                Id = kayit.Id,
                DersId = kayit.DersId,
                AktifMi = kayit.AktifMi
            });
        }

        [HttpDelete("ders-kayit/{id}")]
        public async Task<IActionResult> DeleteDersKayit(int id)
        {
            var userId = GetUserId();
            var kayit = _context.KullaniciDersKayitlari
                .FirstOrDefault(x => x.Id == id && x.KullaniciId == userId);

            if (kayit == null)
            {
                return NotFound(new { message = "Kayıt bulunamadı" });
            }

            _context.KullaniciDersKayitlari.Remove(kayit);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kayıt silindi" });
        }

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
